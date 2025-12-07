# Plan: Compresie Imagini și Servire Responsive (s/m/l)

## 0. Context și Obiective

Aplicația folosește imagini în format 4:5 pentru povești publicate. Toate imaginile trebuie să rămână în format 4:5, dar vrem să:
- **Comprimăm imaginile** în 3 variante de calitate: **s** (small/calitate mică), **m** (medium/calitate medie), **l** (large/calitate mare = original)
- **Servim automat** varianta corespunzătoare în funcție de tipul de device (mobile/tablet/desktop)
- **Procesăm** atât pozele noi la publicare, cât și pozele existente din containerul `alchimaliacontent`

---

## 1. Specificații Tehnice

### 1.1 Format și Calități

- **Format**: Toate imaginile rămân **4:5** (aspect ratio fix)
- **Variante de compresie**:
  - **s** (small): Calitate JPEG/WebP = 60-65 (pentru mobile, conexiuni lente)
  - **m** (medium): Calitate JPEG/WebP = 75-80 (pentru tablet, conexiuni medii)
  - **l** (large): Calitate JPEG/WebP = 90-95 (pentru desktop, conexiuni rapide) = **original păstrat**

### 1.2 Structura Folderelor în Blob Storage

**Container**: `alchimaliacontent`

**Structură existentă** (păstrată):
```
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/{filename}
```

**Structură extinsă** (cu foldere s/m/l):
```
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/s/{filename}
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/m/{filename}
images/tales-of-alchimalia/stories/{userEmail}/{storyId}/l/{filename}
```

**Exemplu concret**:
```
images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/s/1.bg.webp
images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/m/1.bg.webp
images/tales-of-alchimalia/stories/seed@alchimalia.com/pufpuf-s1/l/1.bg.webp
```

**Notă**: 
- **Imaginile nu au `{lang}` în path** (sunt comune pentru toate limbile)
- Folderul `l/` conține imaginile originale (calitate maximă)
- Folderele `s/` și `m/` conțin variantele comprimate
- **Numele fișierelor rămân identice** în toate folderele (best practice)

### 1.3 Format de Output

- **Format pentru toate variantele (s/m/l)**: **WebP** (best practice pentru mobile optimizat)
- **Fallback**: **JPEG** (pentru browsere vechi care nu suportă WebP)
- **Detectare automată** a suportului browser în frontend
- **Cover images**: Procesate la fel ca tile images (s/m/l)

---

## 2. Arhitectură Backend

### 2.1 Bibliotecă de Procesare

**ImageSharp** (SixLabors.ImageSharp) - bibliotecă cross-platform, performantă, recomandată pentru .NET 8.

**Nu folosim** `System.Drawing.Common` (deprecated, probleme pe Linux).

### 2.2 Serviciu de Compresie Imagini

**Nou serviciu**: `IImageCompressionService`

**Responsabilități**:
- Comprimă o imagine în cele 3 variante (s, m, l)
- **Forțează aspect ratio 4:5** (crop center - mai puțin dăunător decât resize cu padding)
- Convertește toate variantele în **WebP** (optimizat pentru mobile)
- Salvează în folderele corespunzătoare (`s/`, `m/`, `l/`)
- Păstrează același nume de fișier în toate folderele (best practice)
- Returnează informații despre fișierele generate

**Interfață propusă**:
```csharp
public interface IImageCompressionService
{
    /// <summary>
    /// Comprimă o imagine în cele 3 variante (s, m, l) și le salvează în blob storage.
    /// </summary>
    /// <param name="sourceBlobPath">Calea către imaginea originală în blob storage</param>
    /// <param name="targetBasePath">Calea de bază pentru folderele s/m/l (fără /s, /m, /l)</param>
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
    public long LargeSizeBytes { get; init; }
    public string SmallPath { get; init; } = string.Empty;
    public string MediumPath { get; init; } = string.Empty;
    public string LargePath { get; init; } = string.Empty;
}
```

### 2.3 Integrare în Procesul de Publicare

**Modificare în `StoryPublishAssetService`**:

Când copiem o imagine de la draft la published:
1. Citim imaginea originală din draft
2. **Forțăm aspect ratio 4:5** (crop center dacă e nevoie)
3. Generăm cele 3 variante (s, m, l) în format WebP
4. Salvăm în folderele corespunzătoare: `s/`, `m/`, `l/`
5. **Aplicăm pentru toate imaginile**: tile images și cover images

**Flux actual**:
```
Draft → Published (original)
```

**Flux nou**:
```
Draft → Published/l/ (original, calitate 95, WebP, 4:5 forced)
     → Published/s/ (comprimat, calitate 65, WebP, 4:5 forced)
     → Published/m/ (comprimat, calitate 80, WebP, 4:5 forced)
```

**Notă**: 
- Toate variantele au același nume de fișier (ex: `1.bg.webp`)
- Path-ul complet: `images/tales-of-alchimalia/stories/{userEmail}/{storyId}/s/1.bg.webp`

### 2.4 Job pentru Procesare Poze Existente

**Nou job**: `ImageCompressionBackgroundJob`

**Caracteristici**:
- Rulează ca **BackgroundService** (similar cu `StoryPublishQueueWorker`)
- Procesează toate imaginile din containerul `alchimaliacontent`
- **Feature flag** pentru activare/dezactivare: `ImageCompression:EnableBackgroundJob`
- Poate fi declanșat manual prin endpoint API sau rulează periodic (nocturn)

**Structură propusă**:
```csharp
public class ImageCompressionBackgroundJob : BackgroundService
{
    // Parcurge toate folderele images/stories/{storyId}/{lang}/
    // Pentru fiecare imagine găsită:
    //   1. Verifică dacă există deja variantele s/m/l
    //   2. Dacă nu există, generează-le
    //   3. Salvează în folderele corespunzătoare
}
```

**Configurare în `appsettings.json`**:
```json
{
  "ImageCompression": {
    "EnableBackgroundJob": false,
    "BackgroundJobIntervalMinutes": 60,
    "ProcessBatchSize": 10,
    "SmallQuality": 65,
    "MediumQuality": 80,
    "LargeQuality": 95
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
- Transformă URL-urile pentru a include folderul corespunzător (s/m/l)
- Detectează suportul WebP și alege formatul corespunzător

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
  - Metodă pentru compresie JPEG
  - Metodă pentru compresie WebP
  - Validare aspect ratio 4:5
  - Salvare în blob storage (foldere s/m/l)

#### ✅ Pas 1.3: Integrare în Publish Flow
- [ ] Modifică `StoryPublishAssetService.CopyAssetsToPublishedAsync()`:
  - Pentru **toate imaginile** (tile images + cover images), apelează `IImageCompressionService.CompressAndSaveAsync()`
  - Generează și salvează toate cele 3 variante: `s/`, `m/`, `l/`
  - Folosește structura extinsă: `images/tales-of-alchimalia/stories/{userEmail}/{storyId}/s/{filename}`
  - Păstrează același nume de fișier în toate folderele

#### ✅ Pas 1.4: Configurare
- [ ] Adaugă configurare în `appsettings.json`:
  - Calități pentru s/m/l
  - Feature flag pentru job

### Faza 2: Backend - Job pentru Poze Existente

#### ✅ Pas 2.1: Creare Job
- [ ] Creează `ImageCompressionBackgroundJob` (BackgroundService)
- [ ] Implementează logica de parcurgere a containerului `alchimaliacontent`
- [ ] Verificare existență variante s/m/l (skip dacă există deja)
- [ ] Procesare în batch-uri (configurabil)

#### ✅ Pas 2.2: Endpoint Manual Trigger
- [ ] Creează endpoint `POST /api/admin/image-compression/process-all`
- [ ] Protejat cu rol `Admin`
- [ ] Returnează status job (queued/running/completed)

#### ✅ Pas 2.3: Înregistrare în DI
- [ ] Adaugă `ImageCompressionBackgroundJob` în `Program.cs` sau `ServiceCollectionExtensions.cs`
- [ ] Respectă feature flag-ul `ImageCompression:EnableBackgroundJob`

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

### 5.2 Forțare Aspect Ratio 4:5 (Crop Center)

**Algoritm**:
```csharp
// 1. Calculăm aspect ratio-ul actual
var currentRatio = (float)image.Width / image.Height;
var targetRatio = 4f / 5f; // 0.8

// 2. Dacă nu e exact 4:5, aplicăm crop center
if (Math.Abs(currentRatio - targetRatio) > 0.01) // toleranță 1%
{
    // Calculăm dimensiunile pentru crop center
    int cropWidth, cropHeight;
    
    if (currentRatio > targetRatio)
    {
        // Imaginea e mai lată decât 4:5 → crop lățime
        cropHeight = image.Height;
        cropWidth = (int)(cropHeight * targetRatio);
    }
    else
    {
        // Imaginea e mai înaltă decât 4:5 → crop înălțime
        cropWidth = image.Width;
        cropHeight = (int)(cropWidth / targetRatio);
    }
    
    // Crop center
    var x = (image.Width - cropWidth) / 2;
    var y = (image.Height - cropHeight) / 2;
    image = image.Clone(ctx => ctx.Crop(new Rectangle(x, y, cropWidth, cropHeight)));
}

// 3. Continuăm cu compresia
```

**De ce crop center și nu resize cu padding?**
- Crop center păstrează calitatea originală (nu adaugă pixeli artificiali)
- Mai puțin dăunător decât resize cu padding (care poate distorsiona imaginea)
- Păstrează partea centrală a imaginii (de obicei cea mai importantă)

### 5.3 Fallback Strategy

**Backend**:
- Dacă compresia eșuează pentru s sau m, păstrăm doar l (original)
- Logăm eroarea dar nu blocăm procesul de publicare

**Frontend**:
- Dacă varianta s/m/l nu există, fallback la l (original)
- Dacă WebP nu e suportat, fallback la JPEG

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
- `ImageCompression:EnableBackgroundJob` - activează/dezactivează job-ul pentru poze existente
- Poate fi setat per environment (dev/staging/prod)

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
