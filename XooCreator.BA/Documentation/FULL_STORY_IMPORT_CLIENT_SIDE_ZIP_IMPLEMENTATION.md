# Full Story Import – Client-Side ZIP: implementare completă

Document rezumativ al implementării fluxului de import full story cu procesare ZIP în browser (client-side), endpoint-uri noi separate și păstrarea logicii vechi marcate ca obsolete.

---

## 1. Rezumat

- **Flux nou (activ):** Browserul deschide ZIP-ul cu JSZip, extrage manifestul și inflatează `dialogRef`, apelează **prepare-from-manifest** și **confirm-from-assets**, uploadează fiecare asset direct în Blob prin SAS. Worker-ul procesează job-uri cu `StagingPrefix` + `ManifestBlobPath` (citește manifest + assets din staging, upload la draft, CreateStoryCraft, cleanup).
- **Flux vechi (păstrat, marcat obsolete):** Endpoint-urile **request-upload** și **confirm-upload** rămân implementate; clientul poate încărca ZIP-ul întreg; worker-ul procesează job-uri cu `ZipBlobPath` ca înainte. Nu s-a șters nicio logică, doar s-a marcat ca obsolete pentru că fluxul preferat este cel client-side.

---

## 2. Backend

### 2.1 Baza de date

- **Script:** `Database/Scripts/V00106__add_story_import_job_staging_columns.sql`
  - Coloane noi pe `StoryImportJobs`: `StagingPrefix` (VARCHAR(512) NULL), `ManifestBlobPath` (VARCHAR(512) NULL).
  - `ZipBlobPath` devine nullable (pentru job-urile „from assets” nu avem ZIP în Blob).

### 2.2 Entity și configurare

- **`Data/Entities/StoryImportJob.cs`:** `ZipBlobPath` este acum `string?`; adăugate `StagingPrefix` și `ManifestBlobPath` (nullable).
- **`Data/Configurations/StoryImportJobConfiguration.cs`:** `ZipBlobPath` nu mai este required; configurare pentru `StagingPrefix` și `ManifestBlobPath` (max length 512).

### 2.3 Endpoint-uri noi (flux client-side ZIP)

Fișier: **`Features/Story-Editor/Endpoints/ImportFullStoryEndpoint.ClientSideZip.cs`**

| Endpoint | Descriere |
|----------|-----------|
| **POST** `/api/{locale}/stories/import-full/prepare-from-manifest` | Body: `manifest` (JSON), `includeImages`, `includeAudio`, `includeVideo`. Validează manifest, uploadează manifestul în Blob la `imports/{userId}/temp/{uploadId}/manifest.json`, generează SAS per asset la `{stagingPrefix}/assets/{normalizedPath}`. Returnează `uploadId` și `assetPutUrls: [{ path, putUrl }]`. |
| **POST** `/api/{locale}/stories/import-full/confirm-from-assets` | Body: `uploadId`, `includeImages`, `includeAudio`, `includeVideo`. Citește manifestul din Blob, rezolvă storyId, verifică job-uri blocate, creează `StoryImportJob` cu `StagingPrefix`, `ManifestBlobPath`, `ZipBlobPath = null`, enqueue, returnează 202 cu `jobId`, `storyId`, etc. |

### 2.4 Endpoint-uri vechi (păstrate, marcate obsolete)

Fișier: **`Features/Story-Editor/Endpoints/ImportFullStoryEndpoint.DirectToBlob.cs`**

- **POST** `/api/{locale}/stories/import-full/request-upload` – marcat `[Obsolete("Prefer prepare-from-manifest + confirm-from-assets (client-side ZIP). Kept for backward compatibility.")]`
- **POST** `/api/{locale}/stories/import-full/confirm-upload` – același atribut `[Obsolete]`
- Comportament: neschimbat (client uploadează ZIP întreg la SAS; la confirm server citește manifest din ZIP, creează job cu `ZipBlobPath`, fără staging).

### 2.5 Procesare job în worker

Fișier: **`Features/Story-Editor/Endpoints/ImportFullStoryEndpoint.cs`** și **`ImportFullStoryEndpoint.JobProcessing.cs`**

- **`UploadAssetsFromStagingAsync`:** citește fiecare asset de la `{stagingPrefix}/assets/{normalizedPath}`, validează extensie/mărime, uploadează la `BuildDraftPath` în containerul draft (același cod ca la fluxul ZIP).
- **`ProcessImportJobFromStagingAsync`:** încarcă manifestul de la `ManifestBlobPath`, parsează JSON, `CollectExpectedAssets`, `UploadAssetsFromStagingAsync`, `CreateStoryCraftFromJsonAsync`, apoi **`DeleteStagingBlobsAsync`** (șterge manifest + toate blobs sub `stagingPrefix/`).
- **`DeleteStagingBlobsAsync`:** folosește `GetBlobsAsync(prefix: stagingPrefix + "/", cancellationToken: ct)` și șterge fiecare blob.

### 2.6 Worker – ramuri

Fișier: **`Features/Story-Editor/Services/StoryImportQueueWorker.cs`**

- Dacă `ManifestBlobPath` și `StagingPrefix` sunt setate → **`ProcessImportJobFromStagingAsync`** (flux nou).
- Altfel dacă `ZipBlobPath` este setat → flux vechi: deschide ZIP din Blob, **`ProcessImportJobAsync`**, la succes șterge ZIP-ul; comentariu în cod: *"Legacy: process whole ZIP from blob (request-upload + confirm-upload flow). Prefer client-side ZIP (ManifestBlobPath + StagingPrefix)."*
- Altfel → job eșuat (nici ZipBlobPath, nici StagingPrefix).

---

## 3. Frontend

### 3.1 Serviciu

Fișier: **`src/app/story/story-editor/story-editor.service.ts`**

- **`importFullStory(file, options?)`** folosește acum **fluxul client-side ZIP** (nu mai uploadează ZIP-ul întreg).
- **Opțiuni:** `includeImages`, `includeAudio`, `includeVideo`, **`onProgress?(uploaded, total)`** pentru progress la upload assets.
- **Tipuri noi:** `PrepareFromManifestResponse`, `AssetPutUrl` (path, putUrl).

Flux în **`importFullStoryClientSideZip`**:

1. Încarcă fișierul cu **JSZip.loadAsync(file)**.
2. Găsește manifestul: intrare cu nume care conține `manifest/` și se termină cu `story.json`.
3. Citește JSON-ul manifestului, apoi **inflatează dialogRef**: pentru tile-uri de tip `dialog` cu `dialogRef`, citește fișierul dialog din ZIP și înlocuiește în manifest cu `dialogRootNodeId` și `dialogNodes` (analog server `InflateManifestDialogRefs`).
4. **POST** `prepare-from-manifest** cu manifest și opțiuni → primește `uploadId` și `assetPutUrls`.
5. Pentru fiecare `{ path, putUrl }`: găsește intrarea în ZIP după path normalizat (`findZipEntryByPath`), extrage blob, **PUT** la `putUrl` cu header-e `x-ms-blob-type: BlockBlob` și `Content-Type` potrivit; apelează **`onProgress(uploaded, total)`**.
6. **POST** `confirm-from-assets` cu `uploadId` și opțiuni → primește răspunsul job (jobId, storyId, queueName, etc.).
7. Returnează același tip **ImportFullStoryJobResponse**; polling-ul de status rămâne neschimbat.

Helpers: **`normalizeZipPath`** (aliniat la server: `\` → `/`, trim, collapse `//`), **`findZipEntryByPath`** (potrivire case-insensitive), **`getContentTypeForPath`**, **`inflateManifestDialogRefs`** (async, citește dialog JSON din ZIP).

### 3.2 Lista de story-uri (modal Import full story)

Fișier: **`src/app/story/story-editor-list/story-editor-list.component.ts`**

- La apelul **`importFullStory(data.file, { ... })`** s-a adăugat **`onProgress: (count, total) => { this.importUploadCount.set(count); this.importUploadTotal.set(total); }`** astfel încât UI-ul afișează progresul la upload (count/total) în timpul încărcării asset-urilor.

---

## 4. Ce nu s-a șters

- Endpoint-urile **request-upload** și **confirm-upload** sunt în continuare disponibile; doar au atributul **`[Obsolete]`** și un comentariu în fișier.
- Worker-ul păstrează ramura pentru **ZipBlobPath** (procesare ZIP din Blob), cu un comentariu că e legacy.
- Toată logica de validare, rate limit, auth, CreateStoryCraft, etc. este comună sau reutilizată; nu a fost eliminat cod pentru fluxul vechi.

---

## 5. Fișiere modificate / adăugate

| Zonă | Fișier |
|------|--------|
| DB | `Database/Scripts/V00106__add_story_import_job_staging_columns.sql` |
| BE | `Data/Entities/StoryImportJob.cs` |
| BE | `Data/Configurations/StoryImportJobConfiguration.cs` |
| BE | `Features/Story-Editor/Endpoints/ImportFullStoryEndpoint.ClientSideZip.cs` (nou) |
| BE | `Features/Story-Editor/Endpoints/ImportFullStoryEndpoint.DirectToBlob.cs` (Obsolete pe handlers) |
| BE | `Features/Story-Editor/Endpoints/ImportFullStoryEndpoint.cs`, `ImportFullStoryEndpoint.JobProcessing.cs` (UploadAssetsFromStagingAsync, ProcessImportJobFromStagingAsync, DeleteStagingBlobsAsync) |
| BE | `Features/Story-Editor/Services/StoryImportQueueWorker.cs` (ramură staging vs ZIP + comentariu legacy) |
| FE | `src/app/story/story-editor/story-editor.service.ts` (importFullStoryClientSideZip, JSZip, prepare/confirm, progress) |
| FE | `src/app/story/story-editor-list/story-editor-list.component.ts` (onProgress) |
| Doc | `Documentation/FULL_STORY_IMPORT_CLIENT_SIDE_ZIP_PLAN.md` (planul inițial) |
| Doc | **`Documentation/FULL_STORY_IMPORT_CLIENT_SIDE_ZIP_IMPLEMENTATION.md`** (acest fișier) |

---

## 6. Rulare migrare DB

Pe fiecare mediu (dev, staging, prod) trebuie rulat scriptul:

- **V00106__add_story_import_job_staging_columns.sql** (sau migrarea EF echivalentă), astfel încât tabelul `StoryImportJobs` să aibă coloanele `StagingPrefix`, `ManifestBlobPath` și `ZipBlobPath` nullable.

---

## 7. Verificare rapidă

1. **Flux nou:** Import full story din UI → se deschide ZIP în browser, se vede progress la „uploading” (count/total), apoi job queued → worker procesează din staging și șterge staging-ul la succes.
2. **Flux vechi:** Dacă un client (sau un test) apelează în continuare **request-upload** + upload ZIP + **confirm-upload**, job-ul este creat cu `ZipBlobPath` și procesat ca înainte; nu este afectat de noile endpoint-uri.
