## Publish Story – Background Job cu Azure Storage Queue (Baby Steps)

### 0. Obiectiv

- Să mutăm partea grea de publish (delta/full + copiere asset-uri) într-un **job de fundal** care rulează din același App Service (via `BackgroundService`), declanșat de mesaje într-o **Azure Storage Queue**.
- Endpointul de publish devine rapid (`202 Accepted`), iar worker-ul procesează pe rând job-urile, max **1 job activ per story**.

---

### 1. Model de date pentru job-uri

1.1. Creează entitatea `StoryPublishJob`:
- `Id` (Guid, PK)
- `StoryId` (string, index)
- `OwnerUserId` (Guid)
- `RequestedByEmail` (string)
- `LangTag` (string, ex. `ro-ro`)
- `DraftVersion` (int) – valoarea `StoryCraft.LastDraftVersion` la momentul enqueue
- `ForceFull` (bool)
- `Status` (enum/string: `Queued`, `Running`, `Completed`, `Failed`, `Superseded`)
- `DequeueCount` (int)
- `QueuedAtUtc`, `StartedAtUtc`, `CompletedAtUtc`
- `ErrorMessage` (string, opțional)

1.2. Migrare:
- Adaugă tabelul `StoryPublishJobs` via script SQL nou `V0012__add_story_publish_jobs.sql`.
- Indexe:
  - `IX_StoryPublishJobs_StoryId_Status` (`StoryId`, `Status`)
  - eventual `IX_StoryPublishJobs_QueuedAtUtc`.

---

### 2. Storage Queue & configurare

2.1. În `appsettings.*.json`:
- `AzureStorage:ConnectionString`
- `AzureStorage:PublishQueueName` (ex. `story-publish-queue`).

2.2. Wrapper simplu:
- Serviciu `IStoryPublishQueue` cu metoda:
  - `Task EnqueueAsync(StoryPublishJob job, CancellationToken ct)`
- Folosește `QueueClient` din `Azure.Storage.Queues` pentru a trimite mesaje către coadă; worker-ul citește direct din coadă.

---

### 3. Endpoint `POST /api/stories/{storyId}/publish` (request → queue)

3.1. Contract request:
- Body opțional: `{ "forceFull": boolean }` (deja existent).

3.2. Comportament nou:
- Validează user + permisiuni + `StoryCraft` + status (Approved).
- Încarcă `StoryCraft` și citește `LastDraftVersion` curent.
- Creează `StoryPublishJob` cu:
  - `JobId = Guid.NewGuid()`,
  - `DraftVersion = craft.LastDraftVersion`,
  - `ForceFull` din body.
- Salvează job-ul în DB cu `Status = Queued`.
- Enqueue în Azure Storage Queue un mesaj JSON minimal:
  ```json
  {
    "jobId": "guid",
    "storyId": "string",
    "draftVersion": 7
  }
  ```
  (worker-ul reîncarcă restul din DB).
- Răspuns: `202 Accepted` cu payload:
  ```json
  {
    "jobId": "guid",
    "status": "Queued"
  }
  ```

3.3. Reguli:
- Dacă există deja un job `Queued` sau `Running` pentru același `StoryId`, fie:
  - returnăm `409 Conflict` (“publish already in progress”), fie
  - îl marcăm pe cel vechi `Superseded` și enqueuim job nou (se decide la implementare; recomand varianta `Superseded`).

---

### 4. BackgroundService – `StoryPublishQueueWorker`

4.1. Înregistrare:
- În `Program.cs`: `services.AddHostedService<StoryPublishQueueWorker>();`.

4.2. Flux principal (loop) – implementat în `StoryPublishQueueWorker`:
- `while (!stoppingToken.IsCancellationRequested)`:
  - `TryReceiveAsync` (1 mesaj, ex. `visibilityTimeout=30s`).
  - Dacă nu e mesaj → `Task.Delay(2-5s)`.
  - Pentru mesaj:
    - Parsează `jobId`, `storyId`, `draftVersion`.
    - Încarcă `StoryPublishJob` din DB.
    - Dacă nu există sau e deja `Completed/Failed/Superseded` → `CompleteAsync` + continuă.
    - Dacă există alt job `Running` pentru același `StoryId` → marchează curentul `Superseded` și `CompleteAsync`.
    - Actualizează job-ul la `Running`, setează `StartedAtUtc`, incrementează `DequeueCount`.
    - Apelează metoda `ProcessJobAsync(job, stoppingToken)`.
    - La succes: `Status = Completed`, `CompletedAtUtc`, `CompleteAsync`.
    - La eșec: dacă `DequeueCount >= 3` → `Status = Failed`, `ErrorMessage = ex.Message`, `CompleteAsync`; altfel **nu** chemăm `CompleteAsync` (queue-ul va re-livra mesajul).

4.3. `ProcessJobAsync` (parte din worker):
- Încarcă `StoryCraft` + `StoryDefinition` pentru `job.StoryId`.
- Verifică:
  - dacă `craft.LastDraftVersion < job.DraftVersion` → marchează job `Failed` (`"draft version no longer exists"`).
  - dacă `craft.LastDraftVersion > job.DraftVersion` → marchează job `Superseded` (nu publicăm versiuni vechi).
- Apelează `StoryPublishingService.UpsertFromCraftAsync(craft, email, langTag, job.ForceFull, ct)` (delta/full + asset links).
- La final șterge changelog-ul ≤ `LastDraftVersion` (deja în service) și actualizează job-ul la `Completed`.

---

### 5. UX & status tracking

5.1. Frontend:
- **Editorul** și **Lista de story-crafts** arată un banner după publish:  
  „Publish în curs… povestea va apărea ca *Published* în listă când job-ul este gata."
- Banner-ul apare în ambele locații:
  - În editor (`story-editor.component`): când se face publish din editor
  - În listă (`story-editor-list.component`): când se face publish din butonul "Publish" din listă
- Banner-ul afișează status-ul în timp real (Queued → Running → Completed/Failed) prin polling
- Lista de stories se bazează deja pe `Status` din backend; când publish-ul termină, statusul devine `Published` și utilizatorul îl vede la refresh.

5.2. Endpoint de status:
- `GET /api/stories/{storyId}/publish-jobs/{jobId}`:
  - returnează status-ul job-ului (`Queued`, `Running`, `Completed`, `Failed`, `Superseded`) + eventual `ErrorMessage`.
  - Folosit de frontend pentru polling (interval: 5 secunde, max 120 încercări = 10 minute).

---

### 6. Baby steps implementare

1. **Schema & entitate job**  
   - Creează `StoryPublishJob` + `StoryPublishJobs` (script `V0012__add_story_publish_jobs.sql`) și adaugă `DbSet<StoryPublishJob>` în `XooDbContext`.
2. **Wrapper pentru queue**  
   - Implementă `IStoryPublishQueue` cu `Azure.Storage.Queues` și înregistrează-l în DI.
3. **Refactor endpoint publish**  
   - `PublishStoryEndpoint` să nu mai apeleze direct `StoryPublishingService`, ci să: creeze job, enqueuie mesaj, răspundă `202`.
4. **Implementare `StoryPublishQueueWorker`**  
   - BackgroundService cu bucla de procesare, reguli de `DequeueCount` + status update.
5. **Integrare cu `StoryPublishingService`**  
   - Folosește implementarea actuală (delta/full + asset links); nu schimbăm logică internă, doar o apelăm din worker.
6. **Hardening**  
   - Loguri clare (jobId, storyId, draftVersion, status, durata).
   - Timeout-uri rezonabile per job (ex. 30–60s) + retry simplu.
7. **FE și documentație**  
   - Ajustează texte în editor (publish async) și actualizează `BackendPublishForkImportOptimization.md` să trimită la acest plan (`PublishBackgroundJobPlan.md`).


