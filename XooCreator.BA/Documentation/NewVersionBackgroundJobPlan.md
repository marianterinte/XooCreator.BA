# New Version – Background Job cu Azure Storage Queue

## 📋 Obiectiv

Să mutăm procesarea "Create New Version" într-un **job de fundal** care rulează din același App Service (via `BackgroundService`), declanșat de mesaje într-o **Azure Storage Queue**. Endpointul devine rapid (`202 Accepted`), iar worker-ul procesează pe rând job-urile, max **1 job activ per story**.

**Diferențe față de Publish:**
- Input: `StoryDefinition` (published) → Output: `StoryCraft` (draft nou)
- Nu șterge nimic (păstrează published story)
- Asset copying: published → draft storage
- **Fără timeout** - polling continuă până se termină job-ul

---

## 🗂️ Plan de Implementare (Baby Steps)

### ✅ Pas 1: Backend - Database & Entities
**Status:** ✅ Completed

- [x] Creează `StoryVersionJob` entity class
- [x] Creează `StoryVersionJobStatus` static class
- [x] Script SQL: `V0019__add_story_version_jobs.sql`
- [x] Adaugă `DbSet<StoryVersionJob>` în `XooDbContext`
- [x] Configurare EF Core mapping în `XooDbContext.OnModelCreating`

**Fișiere de creat/modificat:**
- `XooCreator.BA/Data/Entities/StoryVersionJob.cs` (nou)
- `XooCreator.BA/Database/Scripts/V00XX__add_story_version_jobs.sql` (nou)
- `XooCreator.BA/Data/XooDbContext.cs` (modificat)

---

### ✅ Pas 2: Backend - Queue Infrastructure
**Status:** ✅ Completed

- [x] Creează `IStoryVersionQueue` interface
- [x] Implementează `StoryVersionQueue` class
- [x] Adaugă configurare în `appsettings.*.json`: `AzureStorage:Queues:Version`
- [x] Înregistrare în DI (`ServiceCollectionExtensions.cs`)

**Fișiere de creat/modificat:**
- `XooCreator.BA/Infrastructure/Services/Queue/IStoryVersionQueue.cs` (nou)
- `XooCreator.BA/Infrastructure/Services/Queue/StoryVersionQueue.cs` (nou)
- `XooCreator.BA/appsettings.*.json` (modificat)
- `XooCreator.BA/Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` (modificat)

---

### ✅ Pas 3: Backend - Worker
**Status:** ✅ Completed

- [x] Creează `StoryVersionQueueWorker` class (BackgroundService)
- [x] Implementează logica de procesare (mută din `CreateVersionEndpoint`)
- [x] Error handling și retry logic (max 3 retries)
- [x] Înregistrare în `Program.cs`

**Fișiere de creat/modificat:**
- `XooCreator.BA/Features/Story-Editor/Services/StoryVersionQueueWorker.cs` (nou)
- `XooCreator.BA/Program.cs` (modificat)

**Logica worker:**
1. Primește mesaj din queue cu `JobId`, `StoryId`, `BaseVersion`
2. Încarcă `StoryVersionJob` din DB
3. Verifică dacă job-ul mai e valid (nu e Completed/Failed/Superseded)
4. Încarcă `StoryDefinition` (published story)
5. Creează `StoryCraft` nou folosind `IStoryCopyService.CreateCopyFromDefinitionAsync`
6. Copiază asset-uri folosind `IStoryAssetCopyService.CopyPublishedToDraftAsync`
7. Marchează job ca `Completed`

---

### ✅ Pas 4: Backend - Endpoint Refactor
**Status:** ✅ Completed

- [x] Modifică `CreateVersionEndpoint.HandlePost` să returneze `202 Accepted`
- [x] Creează `StoryVersionJob` în DB cu status `Queued`
- [x] Trimite mesaj în queue
- [x] Adaugă endpoint GET pentru status: `GET /api/stories/{storyId}/version-jobs/{jobId}`
- [x] Adaugă `VersionJobStatusResponse` record

**Fișiere de modificat:**
- `XooCreator.BA/Features/Story-Editor/Endpoints/CreateVersionEndpoint.cs` (modificat)

**Contract nou:**
- Request: `POST /api/stories/{storyId}/create-version` → `202 Accepted` cu `{ jobId: Guid }`
- Status: `GET /api/stories/{storyId}/version-jobs/{jobId}` → `200 OK` cu `VersionJobStatusResponse`

---

### ✅ Pas 5: Frontend - Services
**Status:** ✅ Completed

- [x] Creează `StoryVersionPollingService` (similar cu `StoryPublishPollingService`)
- [x] Modifică `StoryEditorService.createVersion()` să returneze `Observable<VersionJobResponse>`
- [x] Adaugă `getVersionJobStatus(storyId, jobId)` method în `StoryEditorService`

**Fișiere de creat/modificat:**
- `XooCreator/xoo-creator/src/app/story/story-editor/services/story-version-polling.service.ts` (nou)
- `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.service.ts` (modificat)

**Diferențe față de Publish polling:**
- **Fără timeout** - polling continuă până când status = `Completed` sau `Failed`
- Interval: 5 secunde (la fel)
- Max attempts: **infinit** (sau foarte mare, ex. 10000)

---

### ✅ Pas 6: Frontend - UI Components
**Status:** ✅ Completed

- [x] Adaugă suport pentru `Version` în `JobType` enum și translation keys
- [x] Integrare în `story-editor.component` (banner + polling)
- [x] Integrare în `story-editor-list.component` (banner + polling)
- [x] Integrare în `story-reading.component` (polling, fără banner - navighează direct)
- [x] Gestionare state pentru job-uri active
- [x] Cleanup subscriptions la component destroy

**Fișiere de creat/modificat:**
- `XooCreator/xoo-creator/src/app/story/story-editor/components/version-status-banner/version-status-banner.component.ts` (nou)
- `XooCreator/xoo-creator/src/app/story/story-editor/components/version-status-banner/version-status-banner.component.html` (nou)
- `XooCreator/xoo-creator/src/app/story/story-editor/components/version-status-banner/version-status-banner.component.css` (nou)
- `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts` (modificat)
- `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.html` (modificat)
- `XooCreator/xoo-creator/src/app/story/story-editor-list/story-editor-list.component.ts` (modificat)
- `XooCreator/xoo-creator/src/app/story/story-editor-list/story-editor-list.component.html` (modificat)
- `XooCreator/xoo-creator/src/app/story/story-reading/story-reading.component.ts` (modificat)
- `XooCreator/xoo-creator/src/app/story/story-reading/story-reading.component.html` (modificat)

**Banner styling:**
- Similar cu Publish banner dar cu culoare diferită (ex. albastru/cyan în loc de verde)
- Mesaje specifice pentru "Creating new version..."

---

### ✅ Pas 7: Testing & Polish
**Status:** ⏳ Ready for Testing

- [x] Implementare completă backend + frontend
- [ ] Teste end-to-end: CreateVersion → Queue → Worker → Polling → UI update
- [ ] Error handling: test când job-ul eșuează
- [ ] Test când există deja un job activ (superseded logic)
- [ ] Logging și monitoring
- [ ] Verificare cleanup subscriptions

---

## 📊 Model de Date

### StoryVersionJob Entity

```csharp
public class StoryVersionJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public int BaseVersion { get; set; }  // Versiunea publicată de la care se creează draft-ul
    public string Status { get; set; } = StoryVersionJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class StoryVersionJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Superseded = "Superseded";
}
```

### Queue Payload

```json
{
  "jobId": "guid",
  "storyId": "string",
  "baseVersion": 5
}
```

### API Responses

**CreateVersion Response:**
```typescript
interface VersionJobResponse {
  jobId: string;
}
```

**Job Status Response:**
```typescript
interface VersionJobStatusResponse {
  jobId: string;
  storyId: string;
  status: 'Queued' | 'Running' | 'Completed' | 'Failed' | 'Superseded';
  queuedAtUtc: string;
  startedAtUtc?: string;
  completedAtUtc?: string;
  errorMessage?: string;
  dequeueCount: number;
  baseVersion: number;
}
```

---

## 🔧 Configurare

### appsettings.json

```json
{
  "AzureStorage": {
    "ConnectionString": "...",
    "Queues": {
      "Publish": "story-publish-queue",
      "Version": "story-version-queue",  // NOU
      "Import": "story-import-queue",
      "Fork": "story-fork-queue",
      "ForkAssets": "story-fork-assets-queue",
      "Export": "story-export-queue"
    }
  }
}
```

---

## 📝 Note de Implementare

### Worker Logic Details

1. **Verificare job valid:**
   - Dacă job-ul nu există sau e `Completed/Failed/Superseded` → skip
   - Dacă există alt job `Running` pentru același `StoryId` → marchează curentul `Superseded`

2. **Procesare:**
   - Încarcă `StoryDefinition` cu toate include-urile necesare
   - Verifică că story-ul e `Published`
   - Verifică că nu există deja un draft (dacă există → `Failed` cu mesaj)
   - Creează `StoryCraft` folosind `IStoryCopyService`
   - Copiază asset-uri folosind `IStoryAssetCopyService`
   - Marchează job `Completed`

3. **Error Handling:**
   - Dacă `DequeueCount >= 3` → `Failed` și `CompleteAsync`
   - Altfel → nu face `CompleteAsync` (queue va re-livra mesajul)

### Frontend Polling

- **Fără timeout** - polling continuă până când status = `Completed` sau `Failed`
- Interval: 5 secunde
- Max attempts: foarte mare (ex. 10000) sau infinit (folosind `takeWhile`)

### UI Banner

- Culoare diferită de Publish banner (ex. cyan/blue)
- Mesaje specifice:
  - Queued: "Creating new version..."
  - Running: "Creating new version... This may take a few moments."
  - Completed: "New version created successfully!"
  - Failed: "Failed to create new version: {errorMessage}"

---

## 🚀 Status Implementare

**Ultima actualizare:** 2025-01-XX

**Progres general:** 6.5/7 pași compleți (implementare completă, ready for testing)

- ✅ Pas 1: Backend - Database & Entities ✅
- ✅ Pas 2: Backend - Queue Infrastructure ✅
- ✅ Pas 3: Backend - Worker ✅
- ✅ Pas 4: Backend - Endpoint Refactor ✅
- ✅ Pas 5: Frontend - Services ✅
- ✅ Pas 6: Frontend - UI Components ✅
- ⏳ Pas 7: Testing & Polish
- ✅ Pas 3: Backend - Worker
- ✅ Pas 4: Backend - Endpoint Refactor
- ✅ Pas 5: Frontend - Services
- ✅ Pas 6: Frontend - UI Components
- ✅ Pas 7: Testing & Polish

---

## 📚 Referințe

- `PublishBackgroundJobPlan.md` - Planul pentru Publish (similar)
- `StoryPublishQueueWorker.cs` - Worker de referință
- `CreateVersionEndpoint.cs` - Endpoint actual (de refactorizat)
- `StoryPublishPollingService.ts` - Service de polling de referință

