# Plan: upload robust pentru fișiere mari (chunked / direct-to-blob)

## 1. Scop

- Permite upload de ZIP-uri **> 1GB** (imagini/audio/full story) fără a depăși limitele HTTP/multipart ale aplicației.
- Reduce încărcarea pe backend: datele **nu** trec prin API, ci direct în Blob Storage.
- Opțional: **resumable** (reluare după întrerupere) pentru fișiere foarte mari.

---

## 2. Situația actuală

| Flux | Limită actuală | Cum ajunge în blob |
|------|-----------------|--------------------|
| Import Images (ZIP) | 1GB (după mărire) | Request multipart → backend → `ReadFormAsync` → upload blob |
| Import Audio (ZIP) | 1GB | La fel |
| Full Story Import (ZIP) | 600MB (Kestrel/Form) | La fel |
| Tile/Cover (single file) | SAS: client PUT direct | `request-upload` → SAS URL → client PUT → (commit) |

Problema: la import images/audio/full story, **întregul body** trece prin backend; Kestrel/Form au limite și consumă memorie.

---

## 3. Opțiuni de design

### A. Direct-to-blob (recomandat pentru 1GB–5GB)

- Backend **nu** primește bytes; doar emite un **SAS URL** (write) către un blob cu path cunoscut.
- Clientul uploadează **direct** în Azure Blob (PUT pe SAS URL).
- După upload, clientul apelează backend **confirm-import** (storyId, jobId, blobPath) → backend creează job-ul și pune în coadă; worker-ul citește ZIP-ul din blob (stream) și procesează.

**Avantaje:** Simplu, fără chunking, fără limite de request body pe API. Același pattern ca pentru tile/cover.  
**Dezavantaj:** Un singur PUT; dacă conexiunea cade, se reia uploadul de la zero (dacă nu implementezi block blob).

### B. Chunked (block blob) – pentru fișiere foarte mari sau resumable

- Client împarte fișierul în **blocuri** (ex. 4MB).
- Pentru fiecare bloc: backend (sau un SAS per bloc) permite upload; client PUT bloc cu `blockId` (base64).
- La final: client apelează **put-block-list** cu lista de `blockId` în ordine → blob-ul e finalizat.
- Dacă uploadul e întrerupt, clientul poate relua doar blocurile lipsă (trebuie persistență a listei de blocuri deja uplodate).

**Avantaje:** Resumable, progres granular, potrivit pentru 5GB+.  
**Dezavantaj:** Mai mult de implementat (FE + eventual backend pentru lease/session).

### C. Hibrid (prag de mărime)

- Sub X GB: flux actual (multipart prin backend) sau direct-to-blob cu un singur PUT.
- Peste X GB: obligatoriu direct-to-blob; opțional block blob + resumable.

---

## 4. Recomandare: Direct-to-blob (fără chunking) ca prim pas

- Refolsești pattern-ul existent **request-upload** (SAS) din `RequestUploadEndpoint`.
- Noul flux:
  1. **POST** `/api/{locale}/stories/{storyId}/import-images/request-upload` (sau `import-audio`, `import-full`)  
     Body: `{ "fileName": "import.zip", "expectedSize": 1234567890 }`  
     Răspuns: `{ "putUrl": "https://...?sv=...", "blobPath": "draft/image-import/{jobId}.zip", "jobId": "..." }`
  2. Backend: creează **job** (Queued), generează **SAS write** pentru `blobPath`, returnează `putUrl` + `jobId` + `blobPath`.
  3. **Client**: uploadează fișierul pe `putUrl` (PUT), fără a trimite nimic prin API-ul tău.
  4. **POST** `/api/{locale}/stories/{storyId}/import-images/confirm-upload`  
     Body: `{ "jobId": "...", "blobPath": "...", "size": 1234567890 }`  
     Backend: verifică că blob-ul există (și eventual size), pune job-ul în coadă (sau îl marchează „ready”), worker-ul procesează.

Nu mai există limită de request body pe API pentru payload-ul ZIP; singura limită e cea a Azure Blob (și eventual timeout-ul pentru PUT-ul client → blob).

---

## 5. Pași de implementare (planning)

### Faza 1: Request-upload + Confirm pentru Import Images

| # | Task | Detaliu |
|---|------|--------|
| 1.1 | Endpoint **request-upload** pentru import images | POST `.../import-images/request-upload`. Body: fileName, expectedSize (optional). Creează StoryImageImportJob (Queued), generează SAS pentru `draft/image-import/{jobId}.zip`, returnează putUrl, jobId, blobPath. |
| 1.2 | Endpoint **confirm-upload** pentru import images | POST `.../import-images/confirm-upload`. Body: jobId, blobPath, size (optional). Verifică job, verifică că blob există (GetProperties), marchează job-ul ca „uploaded” și pune în coadă (sau deja e în coadă și worker-ul verifică existența blob-ului). |
| 1.3 | Worker **StoryImageImportQueueWorker** | La procesare: citește ZIP din blob (stream), nu din request. Comportament identic ca acum, doar sursa stream-ului e blob-ul. |
| 1.4 | FE: flux nou pentru „upload mare” | Dacă `file.size > Pragma` (ex. 800MB): folosește request-upload → PUT pe putUrl → confirm-upload. Altfel: păstrează fluxul actual (multipart) pentru compatibilitate. |
| 1.5 | Config / prag | Config `StoryEditor:ImportImages:DirectUploadSizeThresholdBytes` (optional). Peste acest prag, FE folosește doar fluxul direct-to-blob. |

### Faza 2: Același pattern pentru Import Audio

| # | Task | Detaliu |
|---|------|--------|
| 2.1 | Request-upload + confirm pentru import audio | Aceeași logică ca 1.1–1.2, pentru `import-audio` și `StoryAudioImportJob`. |
| 2.2 | Worker audio | Citește ZIP din blob (stream). |
| 2.3 | FE | Prag opțional, același tip de flux. |

### Faza 3: Full Story Import (opțional)

| # | Task | Detaliu |
|---|------|--------|
| 3.1 | Request-upload + confirm pentru full import | Path blob ex. `imports/{userId}/{storyId}/{jobId}/full.zip`. Job: StoryImportJob. |
| 3.2 | Worker full import | Deschide ZIP din blob, procesează ca acum. |
| 3.3 | FE | Alegere flux după mărime. |

### Faza 4 (viitor): Chunked / resumable

| # | Task | Detaliu |
|---|------|--------|
| 4.1 | Initiate chunked upload | POST cu metadata (fileName, totalSize, partSize). Backend returnează uploadId/sessionId, eventual SAS per bloc sau un SAS pe container cu restricții. |
| 4.2 | Upload blocuri | PUT fiecare bloc (block blob). Client păstrează lista blockId. |
| 4.3 | Finalize | POST cu uploadId + ordered blockIds → backend sau client apelează PutBlockList pe SAS (dacă permisiunile o permit). |
| 4.4 | Resumable | Persistență (DB sau blob metadata) a blocilor deja uplodate; la reluare, client trimite doar blocurile lipsă. |

---

## 6. Detalii tehnice utile

- **SAS:** Același `IBlobSasService`; pentru write pe un blob: `GetPutSasAsync(container, blobPath, contentType, validity)`. Validity mare pentru fișiere mari (ex. 1 oră).
- **Blob path:** Păstrați convenția actuală: `draft/image-import/{jobId}.zip`, `draft/audio-import/{jobId}.zip`, `imports/...` pentru full.
- **Verificare confirm:** `BlobClient.ExistsAsync()` și eventual `GetProperties()` pentru size, ca să eviți pornirea job-ului înainte ca uploadul să fie complet.
- **Timeout:** PUT-ul client → Azure poate avea timeout mare (browser/axios); pentru blob, Azure acceptă requesturi lungi. Dacă e nevoie, mărești timeout doar pe client pentru acel PUT.
- **Limită Azure Blob:** Single blob până la ~190TB; nu e problema. Limită reală e timeout-ul și stabilitatea rețelei.

---

## 7. Rezumat

- **Faza 1–2:** Direct-to-blob pentru import images și import audio (request-upload → client PUT → confirm-upload → worker citește din blob). Elimină limita de 1GB pe API.
- **Faza 3:** Același pattern pentru full story import.
- **Faza 4:** Chunked + resumable doar dacă există cerință pentru fișiere foarte mari (multi-GB) sau conexiuni instabile.

După implementarea Fazelor 1–2, poți menține și fluxul multipart actual ca fallback pentru fișiere mici (sau îl poți deprecia treptat și folosi doar direct-to-blob pentru toate importurile de tip ZIP).
