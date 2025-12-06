# Background Jobs – Extindere pentru Import / Fork / Export

## 0. Obiectiv
- Separarea operațiilor grele din Story Editor pe **cozi dedicate** (schema “Opțiunea 2”) pentru a elibera thread-urile HTTP și a preveni blocajele pe planul Azure App Service **B1**.
- Inițial rămânem pe același plan și worker (BackgroundService în webapp), dar definim clar limitele de concurență și protecția de memorie.
- Joburile actuale/planificate:
  - `publish` (existent)
  - `import-full-story`
  - `fork-copy-assets`
  - `export-draft`
  - `export-published` (opțional, faza 2)

---

## 1. Arhitectură Cozi
| Job / operație            | Queue Name                 | Worker hosted in | Max concurrency | Observații |
|---------------------------|----------------------------|------------------|-----------------|------------|
| Publish Story             | `story-publish-queue`      | App Service B1   | 1               | implementat conform planului actual |
| Import Full Story         | `story-import-queue`       | App Service B1   | 1               | heavy CPU + mem; limite stricte |
| Fork Story (craft clone)  | `story-fork-queue`         | App Service B1   | 1               | clona structura + pregătește job assets |
| Fork Copy Assets          | `story-fork-assets-queue`  | App Service B1   | 1               | I/O intensive; protecție pentru asset collisions |
| Export Draft (faza 2)     | `story-export-draft-queue` | App Service B1   | 1 (opțional)    | generează ZIP + SAS |
| Export Published (faza 2) | `story-export-published-queue` | App Service B1 | 1 (opțional)    | idem, dar pe content publicat |

### 1.1 Configurație comună
- `appsettings.*.json`
  ```json
  {
    "AzureStorage": {
      "ConnectionString": "...",
      "Queues": {
        "Publish": "story-publish-queue",
        "Import": "story-import-queue",
        "Fork": "story-fork-queue",
        "ForkAssets": "story-fork-assets-queue",
        "ExportDraft": "story-export-draft-queue",
        "ExportPublished": "story-export-published-queue"
      }
    }
  }
  ```
- `IQueueDispatcher` generic + implementare `AzureQueueDispatcher` acceptă `queueName`.
- Worker-ele se înregistrează individual (`AddHostedService<StoryImportQueueWorker>()` etc.).
- `SemaphoreSlim` global (DI) pentru limitarea joburilor heavy simultane (ex. `MaxHeavyJobs = 1` pentru B1).

---

## 2. Import Full Story Job

### 2.1 Model & persistare
- Tabel nou `StoryImportJobs` (similar cu publish): `Id`, `StoryId`, `OwnerUserId`, `RequestedByEmail`, `ZipFilename`, `ZipBlobPath`, `Status`, `QueuedAtUtc`, `StartedAtUtc`, `CompletedAtUtc`, `Errors`, `WarningsCount`, `AssetsCount`, `TotalBytes`.
- În endpoint:
  1. Upload inițial ZIP în blob temporar (`imports/drafts/{userId}/{storyId}/{timestamp}.zip`).
  2. Creează job `Queued` + enqueue mesaj minimal `{ jobId, storyId, zipBlobPath }`.
  3. Returnează `202 Accepted` cu jobId + status.

### 2.2 Worker (`StoryImportQueueWorker`)
- `TryReceive` cu `visibilityTimeout` 5 minute, `MaxDequeueCount = 3`.
- Pași:
  1. Deserializare mesaj → load job.
  2. Verifică `StoryCraft` existență, spațiu disponibil etc.
  3. Descarcă streamul ZIP din blob; folosește **streaming** (fără buffer complet) → unzip chunked + upload asset direct cu `BlobClient.OpenWrite`.
  4. Creează craft/traduceri/tile-uri exact ca endpointul actual.
  5. Curăță blobul temporar după succes.
  6. Actualizează metrici și status (`Completed/Failed`).
- Protecție memorie:
  - `GC.TryStartNoGCRegion` nu e necesar; ne bazăm pe streaming.
  - `MaxParallelUploads = 2`, cu `SemaphoreSlim`.
  - Logging `WorkingSet` înainte/după job.

### 2.3 Endpoint `GET /api/stories/import-jobs/{jobId}` (status)
- Returnează status + warnings + errors.

---

## 3. Fork Story – pipeline async (craft + assets)

### 3.1 Endpoint `POST /api/stories/{storyId}/fork`
- Validează utilizatorul și sursa (`draft` sau `published`).
- Generează `targetStoryId` nou.
- Creează `StoryForkJob` (`SourceStoryId`, `SourceType`, `CopyAssets`, `TargetOwnerUserId/Email`, `TargetStoryId`, metrici preliminare) și îl persistă în `StoryForkJobs`.
- Trimite mesaj `{ jobId, targetStoryId }` în `story-fork-queue`.
- Returnează **`202 Accepted`** cu payload (`jobId`, `storyId`, `status`, `queueName`, `copyAssets` etc.) pentru ca UI să afișeze imediat starea **Queued**.

### 3.2 Worker `StoryForkQueueWorker`
- Preia mesajul, marchează jobul `Running`, setează `StartedAtUtc`.
- Încarcă sursa:
  - `draft` → include tiles/answers/translations.
  - `published` → include definiția completă.
- Verifică dacă draftul țintă există (retry) și îl reutilizează dacă e cazul.
- Creează noul craft cu `StoryCopyService` (`isCopy=false` pentru fork).
- Dacă `CopyAssets == true`:
  - Generează `StoryForkAssetJob` reutilizând helper-ul din endpoint.
  - Stochează `AssetJobId/Status` în `StoryForkJob` și trimite mesaj în `story-fork-assets-queue`.
- Actualizează `StoryForkJob` (`Completed/Failed`, `WarningSummary`, `ErrorMessage`, `SourceTiles`, `SourceTranslations`, `AssetJobId/Status`).

### 3.3 Worker `StoryForkAssetsQueueWorker`
- Continuă să proceseze `StoryForkAssetJob` (copiere blob-uri draft/published → draft).
- Pe succes/eroare actualizează și `StoryForkJob.AssetJobStatus` pentru vizualizare UI.
- Păstrează retry logic (`DequeueCount <= 3`), marchează `Failed` după retry-uri eșuate.

### 3.4 UX & status
- Endpoint status (viitor): `GET /api/stories/fork-jobs/{jobId}` pentru polling.
- Banner UI afișează `forkAssetsJobInfo` + status-ul jobului principal (Queued/Running/Completed/Failed).
- Frontend-ul (listă/editor/reader) persistă jobId în sessionStorage, afişează mesajul de aşteptare şi navighează automat când worker-ul finalizează (`StoryForkJobPollingService`).

---

## 4. Export Jobs (faza 2 – opțional)

### 4.1 Draft & Published
- Endpoint `GET` -> pune job în coadă corespunzătoare, returnează `202` + jobId.
- Worker:
  - Generează ZIP în stream, îl urcă în container `exports/{storyId}/{jobId}.zip`.
  - Setează job `Completed` cu `DownloadUri` (SAS valid 30 min).
- UI face polling job status și, când e gata, redirecționează downloadul.

---

## 5. Implementare incrementală
1. **Infra comuna**
   - Config queue names, `AzureQueueDispatcher`, `MaxHeavyJobs`.
2. **Import job** (prioritatea cea mai mare).
   - Schema + endpoints status + worker.
3. **Fork copy assets job**.
4. **Telemetry & guard rails**
   - Application Insights: metrici `ImportStory_Duration`, `ForkAssets_Duration`, queue backlog, `WorkingSet` pre/post.
   - Alerts pentru `Failed` > N într-o oră.
5. **Export jobs** (după ce stabilizăm primele două).

---

## 6. Considerații privind Memory Leaks / Resource caps
- Folosim `await using` pentru toate stream-urile și `CancellationToken`.
- După fiecare job logăm `GC.GetTotalMemory(false)` și `Environment.WorkingSet`.
- Limităm `Parallel.ForEachAsync` la valori mici și evităm `MemoryStream` mari (stream direct).
- Manual `Dispose` pentru `ZipArchive`, `QueueClient`, `BlobClient` (folosite prin factory).
- În worker setăm `ServicePointManager.DefaultConnectionLimit = 16` (dar nu creștem agresiv).

---

## 7. Următorii pași aprobați
1. Pregătim migrările SQL pentru `StoryImportJobs` + (dacă e nevoie) `StoryForkAssetJobs`.
2. Implementăm infrastructura comună (queue dispatcher, base worker).
3. Refactorizăm endpoint-urile `ImportFullStory` și `ForkStory` să enqueuie joburi.
4. Scriem worker-ele `StoryImportQueueWorker` și `StoryForkAssetsWorker`.
5. Ajustăm UI pentru status asincron (banner + refresh).

*După implementare monitorizăm încărcarea pe App Service B1; dacă joburile concurează, discutăm mutarea importului într-un plan separat.*

