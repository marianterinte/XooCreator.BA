# Plan: Serviciu de compresie imagini *on-the-fly* peste Azure Blob Storage

## 0. Context

Aplicatie .NET hostată în Azure (App Service / Web App)  
Stocare fișiere: Azure Blob Storage  
Problema: imaginile urcate sunt mari (~2 MB / poză, raport 4:5) și vrem:
- **același format / rezoluție logică**, doar
- **fișiere mult mai mici** (compresie JPEG/WebP),  
- fără să generăm manual mai multe dimensiuni pentru fiecare fișier.

Soluție: un mic **serviciu HTTP** care:
1. citește originalul din Blob,
2. îl recomprimă la calitate mai mică,
3. cache-uiește rezultatul într-un container separat,
4. îl trimite browser-ului.

---

## 1. Obiective

- Să nu modificăm modul în care upload-ăm imaginile (le păstrăm pe toate în format “original”).
- Să avem un **singur URL public** pentru imagini, care:
  - trimite versiunea optimizată dacă există,
  - altfel o generează “din mers”.
- Să nu stricăm raportul 4:5.  
- Să putem controla calitatea printr-un query parameter (`?q=75`).
- Să putem pune ulterior un **CDN** în fața serviciului (ex. Azure Front Door / CDN).

---

## 2. Arhitectură propusă

### 2.1 Componente

1. **Azure Blob Storage**
   - Container `images-original`: versiuni brute, necomprimate (upload direct din aplicație).
   - Container `images-optimized`: versiuni recomprimate (generate automat).

2. **Serviciu HTTP .NET (Minimal API / Web API / Azure Function)**
   - Endpoint generic: `GET /img/{*blobPath}?q=75`
   - Logica:
     1. Calculează numele fișierului optimizat (`<nume>_q<quality>.jpg`).
     2. Verifică în `images-optimized` dacă există.
     3. Dacă **da** → îl servește direct.
     4. Dacă **nu**:
        - ia originalul din `images-original`,
        - îl recomprimă (aceeași dimensiune, quality mai mic),
        - îl urcă în `images-optimized`,
        - îl trimite clientului.

3. **Front-end (Angular / altul)**
   - în loc să pună URL direct de Blob, folosește:
     ```html
     <img src="https://api.domeniul-tau.com/img/covers/story123.jpg?q=75" />
     ```

4. **(Opțional, mai târziu) Azure Front Door / CDN**
   - Pus în fața serviciului `/img/...` pentru cache la edge.

---

## 3. Design endpoint HTTP

### 3.1 Semnătură

- URL: `/img/{*blobPath}`
- Parametri query:
  - `q` (optional): calitatea JPEG (40–95). Default: 75.

### 3.2 Mapping între path și Blob

- `blobPath` = calea imaginii în containerul `images-original`.
  - exemplu: `covers/story123.jpg`
- Nume fișier optimizat: `covers/story123_q75.jpg` în `images-optimized`.

### 3.3 Pseudocod flux

1. Parsează `blobPath`, `q` (`quality`).
2. `optimizedName = nameWithoutExt + "_q" + quality + ".jpg"`.
3. `optimizedPath = (directory + "/" + optimizedName)`.
4. `optimizedBlob = images-optimized / optimizedPath`.

5. Dacă `optimizedBlob.Exists()`:
   - setează `Content-Type: image/jpeg`.
   - returnează stream-ul blob-ului.

6. Altfel:
   - `originalBlob = images-original / blobPath`.
   - dacă *nu* există → `404`.
   - încarcă stream original în ImageSharp:
     - `image = Image.Load(originalStream)`.
   - salvează în memorie cu:
     - `image.SaveAsJpeg(ms, new JpegEncoder { Quality = quality })`.
   - încarcă `ms` în `optimizedBlob`.
   - trimite `ms` la client cu `Content-Type: image/jpeg` și `Cache-Control: public, max-age=31536000`.

---

## 4. Exemplu de implementare (.NET Minimal API)

> Notă: e doar schelet – în proiect se adaugă handling de erori, logging, security (SAS / token), etc.

```csharp
// Program.cs
using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

var builder = WebApplication.CreateBuilder(args);

// BlobServiceClient din connection string-ul tău
builder.Services.AddSingleton(_ =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage")));

var app = builder.Build();

app.MapGet("/img/{*blobPath}", async (
    string blobPath,
    int? q,
    BlobServiceClient blobService,
    HttpResponse response) =>
{
    var quality = Math.Clamp(q ?? 75, 40, 95);  // quality 40–95
    var originalContainer = blobService.GetBlobContainerClient("images-original");
    var optimizedContainer = blobService.GetBlobContainerClient("images-optimized");

    await optimizedContainer.CreateIfNotExistsAsync();

    var ext = Path.GetExtension(blobPath);
    var nameWithoutExt = Path.GetFileNameWithoutExtension(blobPath);
    var dir = Path.GetDirectoryName(blobPath)?.Replace("\", "/");

    var optimizedName = $"{nameWithoutExt}_q{quality}.jpg";
    var optimizedPath = string.IsNullOrEmpty(dir)
        ? optimizedName
        : $"{dir}/{optimizedName}";

    var optimizedBlob = optimizedContainer.GetBlobClient(optimizedPath);

    // 1. deja există varianta comprimată?
    if (await optimizedBlob.ExistsAsync())
    {
        response.ContentType = "image/jpeg";
        await (await optimizedBlob.OpenReadAsync()).CopyToAsync(response.Body);
        return;
    }

    // 2. luăm originalul
    var originalBlob = originalContainer.GetBlobClient(blobPath);
    if (!await originalBlob.ExistsAsync())
    {
        response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    await using var originalStream = await originalBlob.OpenReadAsync();

    // 3. recomprimăm cu aceeași dimensiune, doar quality mai mic
    using var image = await Image.LoadAsync(originalStream);
    await using var ms = new MemoryStream();
    await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = quality });

    ms.Position = 0;

    // 4. salvăm varianta comprimată
    await optimizedBlob.UploadAsync(ms, overwrite: true);

    // 5. trimitem către client
    ms.Position = 0;
    response.ContentType = "image/jpeg";
    response.Headers.CacheControl = "public,max-age=31536000"; // 1 an
    await ms.CopyToAsync(response.Body);
});

app.Run();
```

---

## 5. Integrare în front-end

Exemplu simplu de `<img>`:

```html
<img
  src="https://api.exemplu.com/img/covers/story123.jpg?q=75"
  alt="Story cover"
/>
```

Mai târziu poți combina cu `srcset` / `sizes` dacă vrei:
- aceeași logică de compresie, dar eventual și resize (max width).

---

## 6. Performance, securitate, costuri

- **Performance**
  - Prima cerere pentru fiecare imagine este mai lentă (recomprimare).
  - Ulterior, toate cererile pentru același `blobPath + q` vin din `images-optimized` (cache).
  - Cu CDN în față, cele mai multe cereri nici nu mai lovesc serviciul.

- **Securitate**
  - Poți proteja endpointul cu:
    - SAS tokens,
    - header custom,
    - sau să permiți direct doar GET public dacă nu sunt imagini sensibile.

- **Costuri**
  - Mai multe operații Blob (read original, write optimizat, read optimizat).
  - Mai mult storage (două versiuni ale aceleiași imagini).
  - Dar în schimb, **bandwidth mai mic** și timp de încărcare mult mai bun pentru client.

---

## 7. Rezumatul discuției noastre

> Nu este transcriere mot-à-mot, ci rezumat structurat.

1. **Tu:**  
   - Ai un App Service .NET în Azure și un Postgres B1ms în Canada.  
   - Aplicația face lucruri mai grele (upload zip, unzip, scris în DB, upload poze/audio în Blob).  
   - Te îngrijorează performance-ul, memory leak-uri etc.

2. **Eu:**  
   - Am zis că pentru 1–10 useri DB-ul nu e probabil bottleneck-ul principal, ci:
     - latența mare între App Service și DB dacă nu sunt în același region,
     - taskurile heavy (upload + unzip + procesare) pe un App Service mic (B1).
   - Am sugerat:
     - să pui App, DB și Blob în același region,
     - să faci procesarea zip-urilor în streaming și ideal în joburi de background (queue + worker).

3. **Tu (alt subiect):**  
   - Ai imagini în Blob de ~2 MB și te întrebai dacă există mecanism automat de resize în funcție de ecran.

4. **Eu:**  
   - Blob Storage nu știe singur să resize/comprime; trebuie:
     - ori să generezi dimensiuni multiple la upload,
     - ori să faci **resize/compress on-the-fly** cu un serviciu HTTP.

5. **Tu:**  
   - Ai spus că nu vrei să generezi multe dimensiuni, vrei doar **comprimare**, nu și resize (păstrezi raport 4:5).

6. **Eu:**  
   - Am propus exact arhitectura de mai sus:
     - container original + container optimizat,
     - endpoint `/img/{*path}?q=75`,
     - recomprimare pe primul hit + cache în Blob + `Cache-Control` mare.

7. **Tu:**  
   - Ai cerut un fișier `.md` cu planul + discuția → acest document.

---

## 8. Next steps concrete

1. Configurezi:
   - `BlobStorage` connection string în appsettings.
   - două containere: `images-original`, `images-optimized`.

2. Integrezi codul minimal API de mai sus într-un proiect .NET existent (sau Azure Function HTTP).

3. Schimbi front-end-ul să folosească noile URL-uri `/img/...`.

4. Monitorizezi:
   - CPU și memorie pe App Service,
   - timp mediu de răspuns pe endpointul `/img`.

5. (Opțional) Adaugi:
   - resize cu max width/height,
   - suport pentru WebP/AVIF,
   - CDN în față.

---
