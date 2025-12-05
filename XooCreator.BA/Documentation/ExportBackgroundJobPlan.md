# Export Story – Background Job cu Azure Storage Queue

## Overview

Acest plan descrie implementarea funcționalității **Full Export** cu queue și polling, similar cu mecanismul existent pentru Import, Fork și Publish. Export-ul va fi procesat în background, eliberând UI-ul și permițând utilizatorilor să continue lucrul în timp ce export-ul se procesează.

## Obiectiv

- Mutarea procesării export-ului (generarea ZIP cu JSON + media) într-un **job de fundal** care rulează din același App Service (via `BackgroundService`), declanșat de mesaje într-o **Azure Storage Queue**.
- Endpoint-ul de export devine rapid (`202 Accepted`), iar worker-ul procesează pe rând job-urile.
- Frontend-ul face polling pentru status și descarcă ZIP-ul când job-ul este completat.

---

## Status Implementare

### ✅ Pași Completați

#### 1. Model de date pentru job-uri
- ✅ **Entitate `StoryExportJob`** creată în `XooCreator.BA/Data/Entities/StoryExportJob.cs`
  - Câmpuri: `Id`, `StoryId`, `OwnerUserId`, `RequestedByEmail`, `Locale`, `IsDraft`, `Status`, `DequeueCount`, `QueuedAtUtc`, `StartedAtUtc`, `CompletedAtUtc`, `ErrorMessage`, `ZipBlobPath`, `ZipFileName`, `ZipSizeBytes`, `MediaCount`, `LanguageCount`
  - Status constants: `Queued`, `Running`, `Completed`, `Failed`

- ✅ **Migrare SQL** creată: `Database/Scripts/V0017__add_story_export_jobs.sql`
  - Tabel `StoryExportJobs` cu toate câmpurile necesare
  - Indexe pentru performanță: `IX_StoryExportJobs_StoryId_Status`, `IX_StoryExportJobs_QueuedAtUtc`, `IX_StoryExportJobs_OwnerUserId`

- ✅ **DbSet adăugat** în `XooDbContext.cs`: `public DbSet<StoryExportJob> StoryExportJobs => Set<StoryExportJob>();`

#### 2. Storage Queue & configurare
- ✅ **Interfață `IStoryExportQueue`** creată în `Infrastructure/Services/Queue/IStoryExportQueue.cs`
- ✅ **Implementare `StoryExportQueue`** creată în `Infrastructure/Services/Queue/StoryExportQueue.cs`
  - Folosește `Azure.Storage.Queues.QueueClient`
  - Queue name: `story-export-full-export-queue` (default), configurabil via `AzureStorage:Queues:Export`
  - Dev: `story-export-full-export-queue-dev`
  - Production: `story-export-full-export-queue`

- ✅ **Înregistrare în DI** în `ServiceCollectionExtensions.cs`:
  ```csharp
  services.AddSingleton<IStoryExportQueue, StoryExportQueue>();
  ```

#### 3. BackgroundService – `StoryExportQueueWorker`
- ✅ **Worker creat** în `Features/Story-Editor/Services/StoryExportQueueWorker.cs`
  - Extinde `BackgroundService`
  - Citește mesaje din queue
  - Procesează job-urile (logică de procesare trebuie completată)
  - Gestionează retry logic (max 3 dequeue attempts)

- ✅ **Worker înregistrat** în `Program.cs`:
  ```csharp
  builder.Services.AddHostedService<StoryExportQueueWorker>();
  ```

---

### ✅ Pași Completați (Continuare)

#### 4. Serviciu Export Reutilizabil
- ✅ **Interfață `IStoryExportService`** creată
- ✅ **Implementare `StoryExportService`** creată
  - Metode: `ExportPublishedStoryAsync()` și `ExportDraftStoryAsync()`
  - Returnează `ExportResult` cu toate datele necesare
- ✅ **Înregistrat în DI** în `ServiceCollectionExtensions.cs`

#### 5. Refactorizare Endpoint-uri Export
- ✅ **`ExportPublishedStoryEndpoint`** refactorizat
  - Creează `StoryExportJob` cu `IsDraft = false`
  - Enqueue în queue
  - Returnează `202 Accepted` cu `jobId`
- ✅ **`ExportDraftStoryEndpoint`** refactorizat
  - Creează `StoryExportJob` cu `IsDraft = true`
  - Enqueue în queue
  - Returnează `202 Accepted` cu `jobId`

#### 6. Worker Completat
- ✅ **`StoryExportQueueWorker`** completat
  - Procesează job-urile din queue
  - Folosește `IStoryExportService` pentru generarea ZIP-ului
  - Salvează ZIP-ul în blob storage (`exports/{jobId}/{fileName}`)
  - Actualizează job-ul cu rezultatele (ZipBlobPath, ZipFileName, ZipSizeBytes, MediaCount, LanguageCount)

#### 7. Endpoint Status Polling
- ✅ **`GetExportJobStatusEndpoint`** creat
  - Route: `GET /api/stories/{storyId}/export-jobs/{jobId}`
  - Returnează status job + SAS URL pentru download când este completat
  - Verifică permisiuni (owner sau admin)

### 🔄 Pași În Progres

#### 8. (N/A - Backend completat)

**Fișiere de modificat:**
- `Features/Story-Editor/Endpoints/ExportPublishedStoryEndpoint.cs`
- `Features/Story-Editor/Endpoints/ExportDraftStoryEndpoint.cs`

**Pași necesari:**

1. **Extragere logică de export în serviciu reutilizabil**
   - Creează `IStoryExportService` și `StoryExportService`
   - Metode necesare:
     - `Task<ExportResult> ExportPublishedStoryAsync(StoryDefinition def, string locale, CancellationToken ct)`
     - `Task<ExportResult> ExportDraftStoryAsync(StoryCraft craft, string locale, string ownerEmail, CancellationToken ct)`
   - `ExportResult` să conțină: `ZipBytes`, `FileName`, `MediaCount`, `LanguageCount`, `ZipSizeBytes`

2. **Modificare `ExportPublishedStoryEndpoint.HandleGet`**
   - Validare user + permisiuni (păstrează logica existentă)
   - În loc să genereze ZIP direct, să:
     - Creeze `StoryExportJob` cu `IsDraft = false`
     - Salveze job-ul în DB cu `Status = Queued`
     - Enqueue în Azure Storage Queue
     - Returneze `202 Accepted` cu payload:
       ```json
       {
         "jobId": "guid",
         "status": "Queued"
       }
       ```

3. **Modificare `ExportDraftStoryEndpoint.HandleGet`**
   - Similar cu `ExportPublishedStoryEndpoint`
   - Creează job cu `IsDraft = true`

4. **Completare `StoryExportQueueWorker.ProcessPublishedExportAsync`**
   - Încarcă `StoryDefinition` din DB
   - Apelează `IStoryExportService.ExportPublishedStoryAsync`
   - Salvează ZIP-ul în blob storage (draft container sau un container dedicat pentru export-uri)
   - Actualizează job-ul: `ZipBlobPath`, `ZipFileName`, `ZipSizeBytes`, `MediaCount`, `Status = Completed`

5. **Completare `StoryExportQueueWorker.ProcessDraftExportAsync`**
   - Similar cu `ProcessPublishedExportAsync`
   - Folosește `IStoryExportService.ExportDraftStoryAsync`

---

### ✅ Pași Completați (Frontend)

#### 6. Frontend - Serviciu Polling
- ✅ **`StoryExportJobPollingService`** creat în `story-editor/services/story-export-job-polling.service.ts`
  - Poll interval: 5 secunde
  - Max attempts: 120 (10 minute)
  - Continuă polling cât timp `status === 'Queued' || status === 'Running'`

#### 7. Frontend - Modificare StoryEditorService
- ✅ **Interfețe adăugate**: `ExportJobResponse` și `ExportJobStatusResponse`
- ✅ **Metodă `exportFullStoryAsync()`** modificată
  - Returnează `Observable<ExportJobResponse>` cu `jobId`
  - Gestionează `202 Accepted` response
  - Auto-detectare published vs draft
- ✅ **Metodă `getExportJobStatus()`** adăugată
  - Polling pentru status job

#### 8. Frontend - Modificare StoryEditorComponent
- ✅ **Signal-uri adăugate**:
  - `exportJobStatus = signal<ExportJobStatusResponse | null>(null)`
  - `exportJobError = signal<string | null>(null)`
  - `exportJobLoading = signal(false)`
  - `exportJobPollingSubscription?: Subscription`
- ✅ **Metodă `exportFullStory()`** refactorizată
  - Apelează `exportFullStoryAsync()` pentru a obține `jobId`
  - Pornește polling via `StoryExportJobPollingService`
  - Gestionează completarea și eșecul job-ului
- ✅ **Metode helper adăugate**:
  - `startExportJobPolling()` - pornește polling
  - `onExportJobCompleted()` - gestionează completarea
  - `onExportJobFailed()` - gestionează eșecul
  - `downloadExportZip()` - descarcă ZIP din SAS URL
- ✅ **UI updates** în `story-editor.component.html`
  - Loading modal pentru export job
  - Progress overlay cu status și jobId
  - Error display

### ⏳ Pași Rămași (Opțional - Traduceri)

#### 6. Frontend - Serviciu Polling

**Fișier nou:** `XooCreator/xoo-creator/src/app/story/story-editor/services/story-export-job-polling.service.ts`

**Funcționalitate:**
- Similar cu `StoryPublishPollingService`, `StoryForkJobPollingService`, `StoryImportJobPollingService`
- Metodă `pollJob(jobId: string): Observable<ExportJobStatusResponse>`
- Poll interval: 5 secunde
- Max attempts: 120 (10 minute)
- Continuă polling cât timp `status === 'Queued' || status === 'Running'`

**Interfață TypeScript:**
```typescript
export interface ExportJobStatusResponse {
  jobId: string;
  storyId: string;
  status: 'Queued' | 'Running' | 'Completed' | 'Failed';
  queuedAtUtc: string;
  startedAtUtc?: string;
  completedAtUtc?: string;
  errorMessage?: string;
  zipDownloadUrl?: string;
  zipFileName?: string;
  zipSizeBytes?: number;
  mediaCount?: number;
  languageCount?: number;
}
```

#### 7. Frontend - Modificare StoryEditorService

**Fișier:** `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.service.ts`

**Modificări:**
1. **Modificare `exportFullStory()`**
   - În loc să returneze `Observable<Blob>` direct
   - Să facă POST la endpoint-ul de export (care acum returnează `202 Accepted`)
   - Să returneze `Observable<ExportJobResponse>` cu `jobId`

2. **Adăugare metode noi:**
   ```typescript
   exportFullStoryAsync(storyId: string, isPublished?: boolean): Observable<ExportJobResponse>
   getExportJobStatus(jobId: string): Observable<ExportJobStatusResponse>
   ```

**Interfață:**
```typescript
export interface ExportJobResponse {
  jobId: string;
  status: string;
}
```

#### 8. Frontend - Modificare StoryEditorComponent

**Fișier:** `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts`

**Modificări:**
1. **Modificare `exportFullStory()`**
   - Apelează `editorService.exportFullStoryAsync()`
   - Primește `jobId`
   - Pornește polling via `StoryExportJobPollingService`
   - Afișează loading modal cu progress
   - Când job-ul este `Completed`:
     - Descarcă ZIP-ul din `zipDownloadUrl` (SAS URL)
     - Sau face request la endpoint de download cu `jobId`
   - Când job-ul este `Failed`:
     - Afișează eroare

2. **Adăugare state management:**
   ```typescript
   exportJobStatus = signal<ExportJobStatusResponse | null>(null);
   exportJobLoading = signal(false);
   exportJobError = signal<string | null>(null);
   private exportJobPollingSubscription?: Subscription;
   ```

3. **Adăugare metode:**
   ```typescript
   private startExportJobPolling(jobId: string): void
   private onExportJobCompleted(status: ExportJobStatusResponse): void
   private downloadExportZip(downloadUrl: string, fileName: string): void
   ```

#### 9. Frontend - UI Updates

**Fișier:** `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.html`

**Modificări:**
- Adăugare loading modal pentru export (similar cu fork/publish/import)
- Afișare progress: "Exporting story...", "Preparing ZIP...", "Downloading assets..."
- Când job-ul este completat, afișare "Export completed! Downloading..."

---

## Structură Fișiere Create/Modificate

### Backend - Fișiere Noi
- ✅ `Data/Entities/StoryExportJob.cs`
- ✅ `Infrastructure/Services/Queue/IStoryExportQueue.cs`
- ✅ `Infrastructure/Services/Queue/StoryExportQueue.cs`
- ✅ `Features/Story-Editor/Services/StoryExportQueueWorker.cs`
- ✅ `Database/Scripts/V0017__add_story_export_jobs.sql`
- ⏳ `Features/Story-Editor/Services/IStoryExportService.cs` (de creat)
- ⏳ `Features/Story-Editor/Services/StoryExportService.cs` (de creat)
- ⏳ `Features/Story-Editor/Endpoints/GetExportJobStatusEndpoint.cs` (de creat)

### Backend - Fișiere Modificate
- ✅ `Data/XooDbContext.cs` - adăugat `DbSet<StoryExportJob>`
- ✅ `Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` - înregistrat `IStoryExportQueue`
- ✅ `Program.cs` - înregistrat `StoryExportQueueWorker`
- ⏳ `Features/Story-Editor/Endpoints/ExportPublishedStoryEndpoint.cs` - refactorizare
- ⏳ `Features/Story-Editor/Endpoints/ExportDraftStoryEndpoint.cs` - refactorizare

### Frontend - Fișiere Noi
- ⏳ `story-editor/services/story-export-job-polling.service.ts`

### Frontend - Fișiere Modificate
- ⏳ `story-editor/story-editor.service.ts` - modificare `exportFullStory()`
- ⏳ `story-editor/story-editor.component.ts` - modificare `exportFullStory()`
- ⏳ `story-editor/story-editor.component.html` - UI updates

---

## Configurare Queue

### appsettings.Local.json (Dev)
```json
{
  "AzureStorage": {
    "Queues": {
      "Export": "story-export-full-export-queue-dev"
    }
  }
}
```

### appsettings.Production.json
```json
{
  "AzureStorage": {
    "Queues": {
      "Export": "story-export-full-export-queue"
    }
  }
}
```

**Notă:** Queue-urile vor fi create automat de worker când rulează (via `CreateIfNotExistsAsync`).

---

## Flow Complet

### 1. User inițiază Export
```
Frontend: User click "Full Export"
  ↓
Frontend: POST /api/{locale}/stories/{storyId}/export (sau /export-draft)
  ↓
Backend: Validează user + permisiuni
  ↓
Backend: Creează StoryExportJob (Status = Queued)
  ↓
Backend: Salvează job în DB
  ↓
Backend: Enqueue în Azure Storage Queue
  ↓
Backend: Returnează 202 Accepted { jobId, status: "Queued" }
  ↓
Frontend: Primește jobId, pornește polling
```

### 2. Worker procesează Export
```
Worker: Citește mesaj din queue
  ↓
Worker: Încarcă StoryExportJob din DB
  ↓
Worker: Actualizează Status = Running, StartedAtUtc
  ↓
Worker: Apelează IStoryExportService.ExportPublishedStoryAsync() sau ExportDraftStoryAsync()
  ↓
Worker: Generează ZIP (JSON + media)
  ↓
Worker: Salvează ZIP în blob storage
  ↓
Worker: Actualizează job: ZipBlobPath, ZipFileName, ZipSizeBytes, MediaCount, Status = Completed
  ↓
Worker: Delete message din queue
```

### 3. Frontend polling și download
```
Frontend: Polling GET /api/stories/{storyId}/export-jobs/{jobId} (la fiecare 5 sec)
  ↓
Backend: Returnează status (Queued → Running → Completed)
  ↓
Frontend: Când status = Completed:
  ↓
Frontend: Descarcă ZIP din zipDownloadUrl (SAS URL)
  ↓
Frontend: Save ZIP local cu numele zipFileName
```

---

## Testare

### Testare Backend
1. ✅ Verifică că tabelul `StoryExportJobs` este creat (după migrare)
2. ⏳ Testează endpoint-ul de export returnează `202 Accepted` cu `jobId`
3. ⏳ Verifică că mesajul este pus în queue
4. ⏳ Verifică că worker-ul procesează mesajul
5. ⏳ Verifică că ZIP-ul este generat și salvat în blob storage
6. ⏳ Verifică că job-ul este actualizat cu status `Completed`
7. ⏳ Testează endpoint-ul de status polling

### Testare Frontend
1. ⏳ Testează că export-ul pornește polling
2. ⏳ Verifică că loading modal este afișat
3. ⏳ Verifică că ZIP-ul este descărcat când job-ul este completat
4. ⏳ Testează error handling când job-ul eșuează

---

## Note Importante

1. **Blob Storage pentru Export-uri:**
   - Decide unde să salvezi ZIP-urile generate:
     - Opțiune 1: Container dedicat `story-exports` (recomandat)
     - Opțiune 2: Container `alchimalia-drafts-dev` (pentru draft exports)
     - Opțiune 3: Container temporar cu cleanup automat după X zile

2. **SAS URL pentru Download:**
   - Când job-ul este completat, generează SAS URL cu expirare (ex. 1 oră)
   - Sau creează endpoint dedicat de download: `GET /api/stories/{storyId}/export-jobs/{jobId}/download`

3. **Cleanup:**
   - Consideră cleanup automat pentru ZIP-uri vechi (ex. șterge după 7 zile)
   - Sau cleanup manual când user-ul descarcă ZIP-ul

4. **Error Handling:**
   - Worker-ul retry automat (max 3 dequeue attempts)
   - Dacă eșuează după 3 încercări, job-ul rămâne cu `Status = Failed`
   - Frontend-ul afișează eroare și permite retry manual

---

## Următorii Pași

1. **Extragere logică de export în serviciu** (`IStoryExportService`)
2. **Refactorizare endpoint-uri** să folosească queue
3. **Completare worker** să proceseze export-urile
4. **Endpoint status polling**
5. **Frontend polling service**
6. **Frontend integration**

---

**Data creării:** 2025-01-XX  
**Ultima actualizare:** 2025-01-XX
