# Asset Replacement Strategy - Ștergerea Asset-urilor Vechi la Upload

## Overview
Acest document descrie strategia aleasă pentru ștergerea automată a asset-urilor vechi (imagini, audio, video) când utilizatorul face upload la un asset nou pentru a înlocui unul existent.

## Problema
Când un utilizator face upload la o nouă imagine/audio/video pentru a înlocui un asset existent, vechiul asset rămâne în Azure Storage, ocupând spațiu inutil și creând confuzie.

## Abordări Analizate

### Abordarea 1: Sincronizare cu Database (ALESĂ ✅)
**Descriere:** La upload, backend-ul obține asset-ul vechi din baza de date și îl șterge dacă există și este diferit de cel nou.

**Avantaje:**
- ✅ Impact minim: nu schimbă structura existentă de foldere
- ✅ Mai sigură: șterge doar ce nu mai este referențiat în DB
- ✅ Flexibilă: poate fi extinsă pentru cleanup periodic de fișiere orfane
- ✅ Nu necesită JSON complet, doar identificarea tile-ului și limbii

**Dezavantaje:**
- ⚠️ Necesită query la DB pentru a obține asset-ul vechi
- ⚠️ Logică ușor mai complexă (comparare și ștergere condiționată)

### Abordarea 2: Organizare pe Foldere (NEAlesă)
**Descriere:** Restructurare a folderelor astfel încât fiecare asset să aibă folderul său dedicat. La upload, se șterge tot din folderul respectiv înainte de a face upload la noul fișier.

**Avantaje:**
- ✅ Simplu: ștergi tot din folder, apoi uploadezi
- ✅ Rapid și predictibil

**Dezavantaje:**
- ❌ Necesită restructurare majoră a arhitecturii de foldere
- ❌ Modificări în toate locurile unde se construiesc path-uri (import, export, fork, copy, etc.)
- ❌ Risc: dacă există bug-uri, poate șterge fișiere care încă sunt folosite
- ❌ Migrare: trebuie mutat conținutul existent în noua structură

## Implementare - Abordarea 1

### Structura Asset-urilor în Database

#### Imagini (Language-Agnostic)
- **Draft:** `StoryCraftTile.ImageUrl` (filename doar)
- **Published:** `StoryTile.ImageUrl` (path complet)

#### Audio (Language-Specific)
- **Draft:** `StoryCraftTileTranslation.AudioUrl` (filename doar)
- **Published:** `StoryTileTranslation.AudioUrl` (path complet)

#### Video (Language-Specific)
- **Draft:** `StoryCraftTileTranslation.VideoUrl` (filename doar)
- **Published:** `StoryTileTranslation.VideoUrl` (path complet)

### Fluxul de Upload

#### Pentru Imagini (Tile Images)
1. Frontend trimite request la `/api/assets/request-upload` cu:
   - `storyId`
   - `tileId`
   - `kind: "tile-image"`
   - `fileName` (noul fișier)
   - `lang` (ignorat pentru imagini)

2. Backend (`RequestUploadEndpoint`):
   - Obține `StoryCraft` din DB folosind `storyId`
   - Găsește `StoryCraftTile` cu `TileId` = `tileId`
   - Verifică `tile.ImageUrl` (vechiul filename)
   - Dacă există și este diferit de noul filename:
     - Construiește path-ul vechi: `draft/u/{email}/stories/{storyId}/{oldImageUrl}`
     - Șterge blob-ul vechi din Azure Storage
   - Continuă cu upload-ul noului fișier

#### Pentru Audio (Tile Audio)
1. Frontend trimite request la `/api/assets/request-upload` cu:
   - `storyId`
   - `tileId`
   - `kind: "tile-audio"`
   - `fileName` (noul fișier)
   - `lang` (limba pentru care se face upload)

2. Backend:
   - Obține `StoryCraft` din DB folosind `storyId`
   - Găsește `StoryCraftTile` cu `TileId` = `tileId`
   - Găsește `StoryCraftTileTranslation` cu `LanguageCode` = `lang`
   - Verifică `translation.AudioUrl` (vechiul filename)
   - Dacă există și este diferit de noul filename:
     - Construiește path-ul vechi: `draft/u/{email}/stories/{storyId}/{lang}/{oldAudioUrl}`
     - Șterge blob-ul vechi din Azure Storage
   - Continuă cu upload-ul noului fișier

#### Pentru Video (Tile Video)
1. Frontend trimite request la `/api/assets/request-upload` cu:
   - `storyId`
   - `tileId`
   - `kind: "video"`
   - `fileName` (noul fișier)
   - `lang` (limba pentru care se face upload)

2. Backend:
   - Obține `StoryCraft` din DB folosind `storyId`
   - Găsește `StoryCraftTile` cu `TileId` = `tileId`
   - Găsește `StoryCraftTileTranslation` cu `LanguageCode` = `lang`
   - Verifică `translation.VideoUrl` (vechiul filename)
   - Dacă există și este diferit de noul filename:
     - Construiește path-ul vechi: `draft/u/{email}/stories/{storyId}/{lang}/{oldVideoUrl}`
     - Șterge blob-ul vechi din Azure Storage
   - Continuă cu upload-ul noului fișier

### Locul Implementării

**Implementare:** Logica de ștergere a asset-urilor vechi se face în `RequestUploadEndpoint.HandlePost()` pentru că:
- Se execută imediat la upload, înainte de a face upload la noul fișier
- Utilizatorul poate face upload la mai multe asset-uri fără să salveze story-ul
- Asigură că vechiul asset este șters imediat, nu doar la salvare
- Evită acumularea de asset-uri vechi în storage

**Metodă:** `DeleteOldAssetIfNeededAsync()` verifică și șterge asset-ul vechi pentru:
- Cover image (`craft.CoverImageUrl`)
- Tile image (`tile.ImageUrl`)
- Tile audio (`tileTranslation.AudioUrl` - language-specific)
- Tile video (`tileTranslation.VideoUrl` - language-specific)

### Exemple de Path-uri

#### Draft Paths (pentru ștergere)
- **Image:** `draft/u/user@example.com/stories/my-story/image1.png`
- **Audio:** `draft/u/user@example.com/stories/my-story/ro-ro/audio1.mp3`
- **Video:** `draft/u/user@example.com/stories/my-story/ro-ro/video1.mp4`

#### Construirea Path-ului
Folosim `StoryAssetPathMapper.BuildDraftPath()` pentru a construi path-ul consistent:
```csharp
var asset = new StoryAssetPathMapper.AssetInfo(oldFilename, assetType, lang);
var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, userEmail, storyId);
```

### Gestionarea Erorilor

- Dacă ștergerea vechiului asset eșuează, nu trebuie să blocheze upload-ul noului asset
- Logăm warning-uri pentru debugging, dar continuăm cu upload-ul
- Erorile de ștergere nu trebuie să afecteze experiența utilizatorului

### Edge Cases

1. **Primul upload (nu există asset vechi):**
   - Verificăm dacă `ImageUrl`/`AudioUrl`/`VideoUrl` este null sau empty
   - Dacă da, nu facem nimic (nu există ce șterge)

2. **Același filename:**
   - Comparăm vechiul filename cu cel nou
   - Dacă sunt identice, nu ștergem (probabil overwrite direct în Azure)

3. **Tile nou (nu există în DB):**
   - Dacă tile-ul nu există încă în DB, nu există asset vechi de șters

4. **Translation nouă (pentru audio/video):**
   - Dacă nu există translation pentru limba respectivă, nu există asset vechi de șters

## Beneficii

1. **Cleanup automat:** Nu mai rămân fișiere vechi în storage
2. **Economie de spațiu:** Ștergerea automată reduce consumul de storage
3. **Claritate:** Fiecare tile are un singur asset de fiecare tip
4. **Siguranță:** Ștergerea se face doar pentru asset-uri care nu mai sunt referențiate

## Extensii Viitoare

Această abordare poate fi extinsă pentru:
- **Cleanup periodic:** Job care identifică și șterge fișiere orfane (care nu mai sunt referențiate în niciun story)
- **Analiză de storage:** Raportare despre asset-uri duplicate sau neutilizate
- **Optimizare:** Identificarea asset-urilor care pot fi optimizate (compresie, etc.)

## Status

- ✅ **Documentație:** Completă
- ✅ **Implementare pentru Imagini (Tile):** Implementată în `RequestUploadEndpoint.HandlePost()`
- ✅ **Implementare pentru Audio:** Implementată în `RequestUploadEndpoint.HandlePost()`
- ✅ **Implementare pentru Video:** Implementată în `RequestUploadEndpoint.HandlePost()`
- ✅ **Implementare pentru Cover Image:** Implementată în `RequestUploadEndpoint.HandlePost()`

**Notă:** Ștergerea asset-urilor vechi se face imediat la upload, nu la salvare, pentru a permite utilizatorilor să facă upload la mai multe asset-uri fără să salveze story-ul.

## Note

- Implementarea actuală pentru imagini este în `StoryTileUpdater.UpdateTilesAsync()`
- Pentru audio și video, trebuie adăugată logică similară în același loc
- Trebuie să verificăm și cazul cover image (dacă se aplică același principiu)

