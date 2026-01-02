# Plan: Compresie Imagini și Servire Responsive (s/m/l)

## 0. Context și Obiective

Aplicația folosește imagini în format 4:5 pentru povești publicate. Toate imaginile trebuie să rămână în format 4:5, dar vrem să:
- **Comprimăm imaginile** în 3 variante de calitate: **s** (small/calitate mică), **m** (medium/calitate medie), **l** (large/calitate mare = original)
- **Servim automat** varianta corespunzătoare în funcție de tipul de device (mobile/tablet/desktop)
- **Procesăm** atât pozele noi la publicare, cât și pozele existente din containerul `alchimaliacontent`

---

## 1. Specificații Tehnice

### 1.1 Format și Calități

- **Aspect ratio**: Target **4:5**.
  - **Nu facem crop și nu facem padding/letterbox** în backend.
  - **Imaginile care nu sunt deja ~4:5 (în toleranță)** vor fi **lăsate în pace** (skip la generarea s/m).
  - Pentru imagini noi presupunem că vin deja 4:5 (editor/upload).
- **Variante de compresie**:
  - **s** (small): pentru mobile (fișier mic)
  - **m** (medium): pentru tablet/desktop mediu
  - **l** (large): **originalul existent** (canonical) – nu îl mutăm în alt folder și nu îl procesăm obligatoriu

### 1.2 Structura Folderelor în Blob Storage

**Container**: `alchimaliacontent`

**Structură existentă** (păstrată):
```
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/{filename}
```

**Structură extinsă** (cu foldere s/m; originalul rămâne la rădăcină):
```
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/s/{filename}
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/m/{filename}
```

**Exemplu concret**:
```
images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/s/1.bg.webp
images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/m/1.bg.webp
images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/1.bg.webp   (original/canonical)
```

**Notă**: 
- **Imaginile nu au `{lang}` în path** (sunt comune pentru toate limbile)
- Originalul (large/canonical) rămâne în path-ul existent (fără `l/`)
- Folderele `s/` și `m/` conțin variantele comprimate/optimizate
- **Numele fișierelor rămân identice** în toate folderele (best practice)

### 1.3 Format de Output

- **Phase 1 (recomandat, risc minim)**: păstrăm extensia/numele fișierului pentru `s/` și `m/` (ex: dacă originalul e `.png`, derivatul rămâne `.png`).
  - Motiv: în codul actual, API/DB livrează un singur string URL; schimbarea extensiei implică migrare/strategie de compatibilitate.
- **Phase 2 (opțional)**: standardizăm upload-urile noi la `.webp` în editor și atunci toate derivatele devin natural `.webp`.
- **Cover images / reward / region / heroes**: procesate la fel ca tile images (s/m + original)

---

## 2. Arhitectură Backend

### 2.1 Bibliotecă de Procesare

**ImageSharp** (SixLabors.ImageSharp) - bibliotecă cross-platform, performantă, recomandată pentru .NET 8.

**Nu folosim** `System.Drawing.Common` (deprecated, probleme pe Linux).

### 2.2 Serviciu de Compresie Imagini

**Nou serviciu**: `IImageCompressionService`

**Responsabilități**:
- Generează variantele **s** și **m** pentru o imagine publicată (originalul rămâne canonical)
- **Validează aspect ratio ~4:5** (toleranță configurabilă)
  - Dacă nu e 4:5 → **skip** (nu crop, nu padding) și lăsăm doar originalul
- (Phase 1) păstrează extensia (nu schimbăm formatul)
- Salvează în folderele corespunzătoare (`s/`, `m/`)
- Păstrează același nume de fișier în toate folderele (best practice)
- Returnează informații despre fișierele generate

**Interfață propusă**:
```csharp
public interface IImageCompressionService
{
    /// <summary>
    /// Generează variantele s/m pentru o imagine. Dacă imaginea nu este ~4:5, face skip.
    /// </summary>
    /// <param name="sourceBlobPath">Calea către imaginea originală în blob storage</param>
    /// <param name="targetBasePath">Calea de bază pentru folderele s/m (fără /s, /m)</param>
    /// <param name="filename">Numele fișierului (cu extensie)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Rezultat cu informații despre fișierele generate</returns>
    Task<ImageCompressionResult> CompressAndSaveAsync(
        string sourceBlobPath,
        string targetBasePath,
        string filename,
        CancellationToken ct);
}

public record ImageCompressionResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public long OriginalSizeBytes { get; init; }
    public long SmallSizeBytes { get; init; }
    public long MediumSizeBytes { get; init; }
    public string SmallPath { get; init; } = string.Empty;
    public string MediumPath { get; init; } = string.Empty;
    public bool SkippedBecauseNotFourByFive { get; init; }
}
```

### 2.3 Integrare în Procesul de Publicare

**Modificare în `StoryPublishAssetService`**:

Când copiem o imagine de la draft la published:
1. Citim imaginea originală din draft
2. Copiem originalul în published (canonical) conform path-ului existent
3. Dacă imaginea este deja ~4:5 → generăm **s** și **m** în published în folderele `s/` și `m/`
4. Dacă nu este ~4:5 → **skip** (rămâne doar originalul)
5. **Aplicăm pentru toate imaginile** din `images/`:
   - stories: tile images + cover images
   - epics: epic cover + reward images + region images
   - heroes: imagini sub `images/heroes/...`

**Flux actual**:
```
Draft → Published (original)
```

**Flux nou**:
```
Draft → Published/{filename} (original/canonical)
     → Published/s/{filename} (optimizat pentru mobile; doar dacă originalul e ~4:5)
     → Published/m/{filename} (optimizat; doar dacă originalul e ~4:5)
```

**Notă**: 
- Toate variantele au același nume de fișier (ex: `1.bg.webp`)
- Path-ul complet: `images/tales-of-alchimalia/stories/{userEmail}/{storyId}/s/1.bg.webp`

### 2.4 Backfill pentru Poze Existente (fără job permanent)

Nu introducem un BackgroundService nou “permanent”.

**Caracteristici**:
- Se declanșează manual din **admin-dashboard** prin endpoint API
- Procesează toate imaginile din `images/` din containerul `alchimaliacontent`
- **Idempotent**:
  - dacă `s/` și `m/` există → skip
  - dacă imaginea nu e ~4:5 → skip
  - dacă a eșuat o parte → rulezi din nou, ce e deja ok va fi skipped

**Endpoint propus**:
- `POST /api/admin/image-compression/backfill-images` (Admin)
  - pornește procesarea (sincron sau cu jobId + polling)
- `GET /api/admin/image-compression/backfill-images/{jobId}` (Admin) – dacă alegem varianta async

**Configurare în `appsettings.json`**:
```json
{
  "ImageCompression": {
    "ProcessBatchSize": 10,
    "SmallQuality": 65,
    "MediumQuality": 80,
    "FourByFiveTolerance": 0.01,
    "SmallMaxWidth": 480,
    "MediumMaxWidth": 768
  }
}
```

---

## 3. Arhitectură Frontend (Angular)

### 3.1 Detectare Device și Stocare în LocalStorage

**Nou serviciu**: `DeviceDetectionService`

**Responsabilități**:
- Detectează tipul de device (mobile/tablet/desktop)
- Stochează preferința în `localStorage` (key: `deviceImageSize`)
- Returnează size-ul corespunzător: `'s' | 'm' | 'l'`

**Logica de detectare**:
```typescript
// Detectare bazată pe:
// 1. User-Agent (mobile/tablet detection)
// 2. Screen width (window.innerWidth)
// 3. Connection type (navigator.connection?.effectiveType)

// Reguli:
// - Mobile (< 768px) → 's'
// - Tablet (768px - 1024px) → 'm'
// - Desktop (> 1024px) → 'l'
```

### 3.2 Interceptor Global pentru Imagini

**Nou serviciu**: `ImageUrlInterceptorService`

**Responsabilități**:
- Interceptează toate URL-urile de imagini din aplicație
- Transformă URL-urile pentru a include folderul corespunzător (**s/m**)
- Dacă varianta `s/` sau `m/` nu există (sau a fost skipped pentru non-4:5), aplicația trebuie să aibă **fallback la original**

**Exemplu transformare**:
```
Input:  images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/1.bg.webp
Output: images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/s/1.bg.webp  (dacă device = mobile)
```

**Notă**: 
- Nu există `{lang}` în path pentru imagini (sunt comune pentru toate limbile)
- Folderul s/m/l se adaugă înainte de numele fișierului

**Integrare**:
- Modifică `ImageAdapterService` existent pentru a folosi `ImageUrlInterceptorService`
- Sau creează un nou serviciu care înlocuiește `ImageAdapterService.getImageUrl()`

### 3.3 Modificări în Serviciile Existente

**Fișiere de modificat**:
1. `image-adapter.service.ts` - adaugă logica de interceptare
2. `story-editor-image-adapter.service.ts` - adaugă logica de interceptare
3. Toate componentele care folosesc `getImageUrl()` vor beneficia automat

**Exemplu de utilizare**:
```typescript
// Înainte:
const imageUrl = this.imageAdapter.getImageUrl('images/tales-of-alchimalia/stories/.../image.webp');

// După (același cod, dar interceptor-ul transformă automat):
const imageUrl = this.imageAdapter.getImageUrl('images/tales-of-alchimalia/stories/.../image.webp');
// → 'images/tales-of-alchimalia/stories/.../s/image.webp' (dacă device = mobile)
```

---

## 4. Plan de Implementare (Pași)

### Faza 1: Backend - Infrastructură de Compresie

#### ✅ Pas 1.1: Adăugare ImageSharp
- [ ] Adaugă `SixLabors.ImageSharp` în `XooCreator.BA.csproj`
- [ ] Adaugă `SixLabors.ImageSharp.Web` pentru suport WebP

#### ✅ Pas 1.2: Creare Serviciu de Compresie
- [ ] Creează `IImageCompressionService` interface
- [ ] Implementează `ImageCompressionService`:
  - Validare aspect ratio ~4:5 (toleranță configurabilă)
  - Dacă nu e 4:5 → **skip** (nu crop/padding)
  - Resize + compresie pentru s/m
  - Salvare în blob storage (foldere s/m)

#### ✅ Pas 1.3: Integrare în Publish Flow
- [ ] Modifică `StoryPublishAssetService.CopyAssetsToPublishedAsync()`:
  - Pentru **toate imaginile** (stories + epics + heroes), generează s/m (doar dacă imaginea e ~4:5)
  - Generează și salvează variantele: `s/`, `m/` (originalul rămâne canonical)
  - Folosește structura extinsă: `.../s/{filename}` și `.../m/{filename}`
  - Păstrează același nume de fișier în toate folderele

#### ✅ Pas 1.4: Configurare
- [ ] Adaugă configurare în `appsettings.json`:
  - Calități pentru s/m/l
  - Feature flag pentru job

### Faza 2: Backend - Backfill manual pentru Poze Existente

#### ✅ Pas 2.1: Endpoint Manual Trigger (Admin)
- [ ] Creează endpoint `POST /api/admin/image-compression/backfill-images`
- [ ] Protejat cu rol `Admin`
- [ ] Idempotent: skip dacă există s/m, skip dacă nu e 4:5
- [ ] (Opțional) jobId + polling pentru status

### Faza 3: Frontend - Detectare Device

#### ✅ Pas 3.1: Serviciu Detectare Device
- [ ] Creează `DeviceDetectionService`:
  - Detectare bazată pe screen width + user agent
  - Stocare în `localStorage`
  - Metodă `getImageSize(): 's' | 'm' | 'l'`

#### ✅ Pas 3.2: Serviciu Interceptor Imagini
- [ ] Creează `ImageUrlInterceptorService`:
  - Metodă `interceptImageUrl(originalUrl: string): string`
  - Detectare suport WebP
  - Transformare URL: adaugă folderul s/m/l înainte de numele fișierului
  - Exemplu: `images/tales-of-alchimalia/stories/.../1.bg.webp` → `images/tales-of-alchimalia/stories/.../s/1.bg.webp`

### Faza 4: Frontend - Integrare

#### ✅ Pas 4.1: Modificare ImageAdapterService
- [ ] Modifică `ImageAdapterService.getImageUrl()`:
  - Apelează `ImageUrlInterceptorService.interceptImageUrl()`
  - Transformă URL-urile pentru a include folderul s/m/l corespunzător
  - Păstrează compatibilitatea cu codul existent
  - **Structură**: `images/tales-of-alchimalia/stories/{userEmail}/{storyId}/s/{filename}`

#### ✅ Pas 4.2: Modificare StoryEditorImageAdapterService
- [ ] Aplică aceeași logică de interceptare

#### ✅ Pas 4.3: Testare
- [ ] Testează pe mobile (s)
- [ ] Testează pe tablet (m)
- [ ] Testează pe desktop (l)
- [ ] Verifică fallback la original dacă varianta nu există

### Faza 5: Testing și Optimizare

#### ✅ Pas 5.1: Testare End-to-End
- [ ] Publicare poveste nouă → verifică generare s/m/l
- [ ] Job procesare poze existente → verifică generare
- [ ] Frontend → verifică servirea corectă pe diferite device-uri

#### ✅ Pas 5.2: Monitoring
- [ ] Adaugă logging pentru compresie
- [ ] Măsoară timp de procesare
- [ ] Măsoară reducere dimensiune fișiere

---

## 5. Detalii Tehnice

### 5.1 Algoritm de Compresie

**Pentru toate variantele (s/m/l)**:
```csharp
// 1. Încărcare imagine originală
// 2. FORȚARE aspect ratio 4:5 (crop center - mai puțin dăunător)
// 3. Conversie în WebP
// 4. Compresie cu calitatea corespunzătoare
```

**Pentru varianta s (small)**:
```csharp
// Calitate: 65
// Format: WebP
// Aspect ratio: FORȚAT 4:5 (crop center)
```

**Pentru varianta m (medium)**:
```csharp
// Calitate: 80
// Format: WebP
// Aspect ratio: FORȚAT 4:5 (crop center)
```

**Pentru varianta l (large)**:
```csharp
// Calitate: 95 (calitate maximă)
// Format: WebP
// Aspect ratio: FORȚAT 4:5 (crop center)
// NOTĂ: Aceasta este varianta de calitate maximă, dar tot comprimată în WebP
```

### 5.2 Regula 4:5 (fără crop/padding)

Regulă: **dacă imaginea nu e deja ~4:5, nu o modificăm**.
- La publish/backfill, doar logăm și facem skip pentru s/m.
- Pentru imaginile noi se impune 4:5 în editor/upload (înainte de publish).

### 5.3 Fallback Strategy

**Backend**:
- Dacă generarea eșuează sau e skipped (non-4:5), păstrăm doar originalul (canonical)
- Logăm eroarea dar nu blocăm procesul de publicare

**Frontend**:
- Dacă varianta s/m nu există, fallback la original (canonical)

### 5.4 Performance Considerations

**Backend**:
- Compresia se face **asincron** în job-ul de publicare (nu blocăm request-ul)
- Job-ul pentru poze existente procesează în batch-uri (configurabil, default 10)
- Folosim `CancellationToken` pentru a permite anularea

**Frontend**:
- Detectarea device-ului se face o singură dată la inițializare
- Stocare în `localStorage` pentru a evita re-detectarea la fiecare request
- Interceptor-ul este lightweight (doar transformare string)

---

## 6. Configurare și Deployment

### 6.1 Variabile de Mediu

**Backend (`appsettings.json`)**:
```json
{
  "ImageCompression": {
    "EnableBackgroundJob": false,
    "BackgroundJobIntervalMinutes": 60,
    "ProcessBatchSize": 10,
    "SmallQuality": 65,
    "MediumQuality": 80,
    "LargeQuality": 95,
    "PreferredFormat": "WebP",
    "EnableJpegFallback": true
  }
}
```

**Frontend (`environment.ts`)**:
```typescript
export const environment = {
  // ... existing config
  imageCompression: {
    enabled: true,
    defaultSize: 'm', // fallback dacă detectarea eșuează
    localStorageKey: 'deviceImageSize'
  }
};
```

### 6.2 Feature Flags

**Backend**:
- (Opțional) `ImageCompression:Enabled` - activează/dezactivează generarea s/m la publish
- Backfill-ul e manual (endpoint Admin), nu job periodic

**Frontend**:
- `imageCompression.enabled` - activează/dezactivează interceptarea URL-urilor

### 6.3 Rollout Strategy

1. **Faza 1**: Implementare backend (compresie la publicare)
2. **Faza 2**: Testare pe staging cu poze noi
3. **Faza 3**: Activare job pentru poze existente (feature flag ON)
4. **Faza 4**: Implementare frontend (interceptor)
5. **Faza 5**: Testare end-to-end
6. **Faza 6**: Deploy production

---

## 7. Monitoring și Metrics

### 7.1 Metrics de Urmărit

**Backend**:
- Număr imagini comprimate (s/m/l)
- Timp mediu de compresie per imagine
- Reducere medie de dimensiune (s vs l, m vs l)
- Rate de eroare la compresie

**Frontend**:
- Distribuție device-uri (s/m/l)
- Rate de fallback la original
- Timp de încărcare imagini (before/after)

### 7.2 Logging

**Backend**:
```csharp
_logger.LogInformation(
    "Image compression completed: storyId={StoryId} filename={Filename} " +
    "originalSize={OriginalSize} smallSize={SmallSize} mediumSize={MediumSize} " +
    "reduction={ReductionPercent}%",
    storyId, filename, originalSize, smallSize, mediumSize, reductionPercent);
```

**Frontend**:
```typescript
console.log(`[ImageInterceptor] Transformed URL: ${originalUrl} → ${transformedUrl} (size: ${size})`);
```

---

## 8. Riscuri și Mitigări

### 8.1 Risc: Compresia eșuează
**Mitigare**: Fallback la original (l), logăm eroarea, nu blocăm publicarea

### 8.2 Risc: Job-ul pentru poze existente consumă prea multe resurse
**Mitigare**: 
- Feature flag pentru activare/dezactivare
- Procesare în batch-uri mici
- Interval configurable între batch-uri

### 8.3 Risc: Frontend servește varianta greșită
**Mitigare**: 
- Fallback automat la original (l) dacă varianta s/m nu există
- Validare în interceptor

### 8.4 Risc: Browsere vechi nu suportă WebP
**Mitigare**: 
- Detectare suport WebP în frontend
- Fallback automat la JPEG (dacă există varianta JPEG)

### 8.5 Risc: Aspect ratio nu e 4:5
**Mitigare**: 
- **Forțare obligatorie** aspect ratio 4:5 prin crop center
- Crop center păstrează partea centrală (mai puțin dăunător decât resize)
- Logăm warning dacă a fost necesar crop

---

## 9. Next Steps

1. **Review acest plan** cu echipa
2. **Prioritizare** fazelor de implementare
3. **Estimare** timp pentru fiecare fază
4. **Creare task-uri** în sistemul de tracking (Jira/GitHub Issues/etc.)
5. **Start implementare** Faza 1 (Backend - Infrastructură)

---

## 10. Referințe și Resurse

- [SixLabors.ImageSharp Documentation](https://docs.sixlabors.com/articles/imagesharp/)
- [WebP Support Detection](https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/decode)
- [Azure Blob Storage Best Practices](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction)

---

**Data creării**: 2024
**Ultima actualizare**: 2024
**Status**: 📋 Plan de implementare
