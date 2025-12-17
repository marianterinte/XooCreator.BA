# Analiză: Story Fork pentru Seeded Stories (Tree of Light) și Migrare la Story Epic

**Data**: 17 Decembrie 2025  
**Scop**: Analiza posibilității de a face fork la story-urile seeded din Tree of Light și migrarea acestora la Story Epic

## 1. Rezumat Executiv

### 1.1 Situația Curentă
- **Tree of Light** conține 8 story-uri seeded de la `seed@alchimalia.com`
- Story-urile sunt **publicate direct în StoryDefinition** (nu există StoryCraft pentru ele)
- Fork-ul pentru seeded stories este **blocat la nivel de UI** (frontend)
- **Nu există restricții la nivel de backend** pentru fork-ul seeded stories

### 1.2 Ce Se Copiază la Fork (Verificat ✅)
La fork, se copiază **100% complet**:
- ✅ **Toate translațiile** (ro-ro, en-us, hu-hu) - titles, summaries, captions, text, questions
- ✅ **Toate tile-urile** (pages, quiz, video) - cu toate proprietățile
- ✅ **Toate quiz-urile** - answers, translations, IsCorrect flags, tokens (rewards)
- ✅ **Toate imaginile** - cover image + tile images (copiate fizic din published la draft storage)
- ✅ **Toate audio-urile** - pentru fiecare limbă (copiate fizic)
- ✅ **Toate video-urile** - pentru fiecare limbă (copiate fizic)
- ✅ **Metadata** - topics, age groups, unlocked heroes, IsEvaluative, etc.

### 1.3 Oportunitate
Există o oportunitate de a permite fork-ul seeded stories pentru a facilita:
1. **Editarea** story-urilor din Tree of Light
2. **Migrarea** acestora de la seed data la Story Epic (editat și managed prin UI)
3. **Publicarea** unei versiuni proprii a Tree of Light Epic

### 1.4 Recomandare
**DA**, este posibil și recomandat să permitem fork-ul seeded stories cu următoarea abordare:
1. **Eliminarea restricției de UI** care blochează fork-ul pentru seeded stories
2. **Crearea de story crafts** prin fork pentru fiecare story din Tree of Light
3. **Editarea** story-urilor forked în editor
4. **Publicarea** story-urilor editate
5. **Crearea unui Story Epic** care referențiază story-urile publicate
6. **Publicarea Story Epic-ului** pentru a înlocui Tree of Light seeded

---

## 2. Ce Se Copiază la Fork - Analiză Detaliată

### 2.1 Conținut de Story (Metadata și Text)

#### ✅ Story-Level Data
Fork-ul copiază **toate** datele de story prin `StorySourceMapper.MapFromDefinition()`:

| Element | Se Copiază? | Detalii |
|---------|------------|---------|
| **StoryType** | ✅ DA | Tipul story-ului (e.g., "interactive", "narrative") |
| **StoryTopic** | ✅ DA | Topicul story-ului (e.g., "adventure", "science") |
| **CoverImageUrl** | ✅ DA | Doar numele fișierului (path-ul se reconstruiește) |
| **PriceInCredits** | ✅ DA | Prețul în credite |
| **AuthorName** | ⚠️ NULL | Se setează NULL la fork, apoi se setează la numele user-ului curent |
| **ClassicAuthorId** | ✅ DA | ID-ul autorului clasic (dacă există) |
| **BaseVersion** | ✅ DA | Versiunea de bază (din definition.Version) |
| **IsEvaluative** | ✅ DA | Flag pentru quiz evaluative |
| **Topics** | ✅ DA | Lista de topic IDs (story topics) |
| **AgeGroups** | ✅ DA | Lista de age group IDs |
| **UnlockedStoryHeroes** | ✅ DA | Lista de hero IDs deblocați |

**Cod Sursă**: `StorySourceMapper.cs`, linii 76-130

### 2.2 Translații (Toate Limbile)

#### ✅ Story Translations
Fork-ul copiază **toate translațiile** story-ului:

```csharp
Translations = definition.Translations.Select(t => new TranslationCloneData
{
    LanguageCode = t.LanguageCode,          // ✅ Limba (ro-ro, en-us, hu-hu)
    Title = t.Title,                         // ✅ Titlul în limba respectivă
    Summary = definition.Summary             // ✅ Summary-ul (același pentru toate limbile)
}).ToList()
```

**Exemplu pentru Tree of Light**:
- `intro-pufpuf` are 3 translații: **ro-ro**, **en-us**, **hu-hu**
- **TOATE** cele 3 translații se copiază în fork

**Cod Sursă**: `StorySourceMapper.cs`, linii 88-95

### 2.3 Tile-uri (Pagini și Quiz-uri)

#### ✅ Tiles și Tile Translations
Fork-ul copiază **toate tile-urile** cu **toate translațiile** lor:

```csharp
Tiles = definition.Tiles.OrderBy(t => t.SortOrder).Select(tile => new TileCloneData
{
    TileId = tile.TileId,                    // ✅ ID-ul tile-ului
    Type = tile.Type,                        // ✅ Tipul (page, quiz, video)
    ImageUrl = ExtractFileName(tile.ImageUrl), // ✅ Numele imaginii (fără path)
    Translations = tile.Translations.Select(tt => new TileTranslationCloneData
    {
        LanguageCode = tt.LanguageCode,      // ✅ Limba
        Caption = tt.Caption ?? string.Empty, // ✅ Caption-ul
        Text = tt.Text ?? string.Empty,      // ✅ Textul paginii
        Question = tt.Question ?? string.Empty, // ✅ Întrebarea quiz-ului
        AudioUrl = ExtractFileName(tt.AudioUrl), // ✅ Numele fișierului audio
        VideoUrl = ExtractFileName(tt.VideoUrl)  // ✅ Numele fișierului video
    }).ToList(),
    ...
}).ToList()
```

**Important**:
- ✅ **Toate tile-urile** se copiază (pages, quiz, video)
- ✅ **Toate translațiile** fiecărui tile se copiază
- ✅ **Caption, Text, Question** se copiază pentru fiecare limbă
- ✅ **AudioUrl și VideoUrl** se copiază pentru fiecare limbă (sunt language-specific)

**Cod Sursă**: `StorySourceMapper.cs`, linii 96-109

### 2.4 Quiz-uri (Answers și Tokens)

#### ✅ Answers și Answer Translations
Fork-ul copiază **toate răspunsurile** quiz-urilor cu **toate translațiile**:

```csharp
Answers = tile.Answers.OrderBy(a => a.SortOrder).Select(answer => new AnswerCloneData
{
    AnswerId = answer.AnswerId,              // ✅ ID-ul răspunsului
    IsCorrect = answer.IsCorrect,            // ✅ Flag pentru răspunsul corect
    Translations = answer.Translations.Select(at => new AnswerTranslationCloneData
    {
        LanguageCode = at.LanguageCode,      // ✅ Limba
        Text = at.Text ?? string.Empty       // ✅ Textul răspunsului
    }).ToList(),
    Tokens = answer.Tokens.Select(token => new TokenCloneData
    {
        Type = token.Type ?? string.Empty,   // ✅ Tipul token-ului (e.g., "courage")
        Value = token.Value ?? string.Empty, // ✅ Valoarea token-ului
        Quantity = token.Quantity            // ✅ Cantitatea
    }).ToList()
}).ToList()
```

**Important**:
- ✅ **Toate răspunsurile** quiz-urilor se copiază
- ✅ **IsCorrect flag** se păstrează
- ✅ **Toate translațiile** fiecărui răspuns se copiază
- ✅ **Tokens-urile** (rewards) se copiază (pentru răspunsuri corecte)

**Cod Sursă**: `StorySourceMapper.cs`, linii 110-125

### 2.5 Asset-uri (Images, Audio, Video)

#### ✅ Asset Collection și Copy Process

Fork-ul folosește `StoryAssetCopyService` pentru a **copia fizic** toate asset-urile.

##### Pasul 1: Colectarea Asset-urilor (`CollectFromDefinition`)

```csharp
public List<AssetInfo> CollectFromDefinition(StoryDefinition definition)
{
    var results = new List<AssetInfo>();
    
    // ✅ Cover Image
    if (!string.IsNullOrWhiteSpace(definition.CoverImageUrl))
        results.Add(new AssetInfo(Path.GetFileName(definition.CoverImageUrl), AssetType.Image, null));
    
    foreach (var tile in definition.Tiles)
    {
        // ✅ Tile Images (language-agnostic)
        if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            results.Add(new AssetInfo(Path.GetFileName(tile.ImageUrl), AssetType.Image, null));
        
        foreach (var translation in tile.Translations)
        {
            // ✅ Audio (language-specific)
            if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                results.Add(new AssetInfo(Path.GetFileName(translation.AudioUrl), AssetType.Audio, translation.LanguageCode));
            
            // ✅ Video (language-specific)
            if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
                results.Add(new AssetInfo(Path.GetFileName(translation.VideoUrl), AssetType.Video, translation.LanguageCode));
        }
    }
    
    return results;
}
```

**Cod Sursă**: `StoryAssetCopyService.cs`, linii 83-114

##### Pasul 2: Copierea Asset-urilor (`CopyPublishedToDraftAsync`)

```csharp
public async Task<AssetCopyResult> CopyPublishedToDraftAsync(
    IEnumerable<AssetInfo> assets,
    string publishedOwnerEmail,     // e.g., "seed@alchimalia.com"
    string sourceStoryId,           // e.g., "intro-pufpuf"
    string targetEmail,             // e.g., "john.doe@example.com"
    string targetStoryId,           // e.g., "john-doe-s1"
    CancellationToken ct)
{
    foreach (var asset in assets)
    {
        // Sursa: Published container
        var sourcePath = BuildPublishedPath(asset, publishedOwnerEmail, sourceStoryId);
        // e.g., "images/tol/stories/seed@alchimalia.com/intro-pufpuf/1.puf-puf-flying.png"
        
        var sourceClient = _sasService.GetBlobClient(_sasService.PublishedContainer, sourcePath);
        
        if (!await sourceClient.ExistsAsync(ct))
        {
            _logger.LogWarning("Source asset not found: {Path}", sourcePath);
            continue; // ⚠️ Skip dacă asset-ul nu există
        }
        
        // Destinația: Draft container
        var targetPath = BuildDraftPath(asset, targetEmail, targetStoryId);
        // e.g., "drafts/john.doe@example.com/john-doe-s1/1.puf-puf-flying.png"
        
        var copyResult = await CopyAssetWithPollingAsync(...);
        
        if (copyResult.HasError)
        {
            return copyResult; // ❌ Oprește dacă un asset eșuează
        }
    }
    
    return AssetCopyResult.Success();
}
```

**Cod Sursă**: `StoryAssetCopyService.cs`, linii 169-220

##### Tipuri de Asset-uri Copiate

| Asset Type | Language-Specific? | Locație Sursă (Published) | Locație Destinație (Draft) |
|------------|-------------------|---------------------------|---------------------------|
| **Cover Image** | ❌ NU | `images/tol/stories/{owner}/{storyId}/0.Cover.png` | `drafts/{user-email}/{newStoryId}/0.Cover.png` |
| **Tile Images** | ❌ NU | `images/tol/stories/{owner}/{storyId}/1.image.png` | `drafts/{user-email}/{newStoryId}/1.image.png` |
| **Audio** | ✅ DA | `audio/{owner}/{storyId}/{lang}/audio.wav` | `drafts/{user-email}/{newStoryId}/{lang}/audio.wav` |
| **Video** | ✅ DA | `video/{owner}/{storyId}/{lang}/video.mp4` | `drafts/{user-email}/{newStoryId}/{lang}/video.mp4` |

**Important**:
- ✅ **Images** sunt language-agnostic (aceeași imagine pentru toate limbile)
- ✅ **Audio** și **Video** sunt language-specific (fiecare limbă are propriul fișier)

##### Mecanismul de Copiere

```csharp
private async Task<AssetCopyResult> CopyAssetWithPollingAsync(...)
{
    // 1. Generează SAS token pentru sursa (read-only, 10 minute)
    var sourceSas = await _sasService.GetReadSasAsync(sourceContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);
    
    // 2. Inițiază copierea asincronă (Azure Blob Copy)
    var copyOperation = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);
    
    // 3. Polling pentru status (max 30 secunde)
    var pollUntil = DateTime.UtcNow.AddSeconds(30);
    while (true)
    {
        var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
        var copyStatus = props.Value.CopyStatus;
        
        if (copyStatus == CopyStatus.Success)
            break; // ✅ Succes
        
        if (copyStatus == CopyStatus.Failed || copyStatus == CopyStatus.Aborted)
            return AssetCopyResult.CopyFailed(...); // ❌ Eșuat
        
        if (DateTime.UtcNow > pollUntil)
            return AssetCopyResult.CopyTimeout(...); // ⏱️ Timeout
        
        await Task.Delay(250, ct); // ⏳ Așteaptă 250ms
    }
    
    return AssetCopyResult.Success();
}
```

**Cod Sursă**: `StoryAssetCopyService.cs`, linii 222-286

**Caracteristici**:
- ✅ **Copy asincron** (server-side copy în Azure Blob Storage)
- ✅ **Polling cu timeout** (30 secunde)
- ✅ **Retry logic** la nivel de service
- ⚠️ **Skip pe asset lipsă** (nu oprește job-ul întreg)
- ❌ **Fail pe primul error** (după skip-urile de asset lipsă)

### 2.6 Fluxul Complet de Fork pentru Seeded Story

#### Exemplu: Fork `intro-pufpuf` (Seeded Story din Tree of Light)

##### Date Inițiale:
- **StoryId**: `intro-pufpuf`
- **Owner**: `seed@alchimalia.com` (CreatedBy userId)
- **Translații**: ro-ro, en-us, hu-hu
- **Tiles**: 5 tiles (4 pages + 1 quiz)
- **Assets**: 
  - 1 cover image: `0.Cover.png`
  - 4 tile images: `1.puf-puf-flying.png`, `2.puf-puf-hurt.png`, `3.puf-puf-crystal.png`, `puf-puf-home-planet.png`
  - Audio/Video (dacă există) pentru fiecare limbă

##### Procesul de Fork:

**1. Backend: Crearea Job-ului**
```json
POST /api/stories/intro-pufpuf/fork
{
  "copyAssets": true
}

Response:
{
  "jobId": "guid-1234-5678",
  "storyId": "john-doe-s1",
  "originalStoryId": "intro-pufpuf",
  "copyAssets": true,
  "status": "Queued"
}
```

**2. Job Processing: Crearea StoryCraft**
- ✅ Încarcă `StoryDefinition` pentru `intro-pufpuf` (cu toate includes)
- ✅ Mapează la `StoryCloneData`:
  - 3 translații (ro-ro, en-us, hu-hu) cu title și summary
  - 5 tiles cu toate translațiile (caption, text, question)
  - Quiz answers cu toate translațiile și tokens
  - Topics, age groups, unlocked heroes
- ✅ Creează `StoryCraft` nou:
  - `StoryId` = `john-doe-s1`
  - `OwnerUserId` = current user
  - `Status` = Draft
  - `AuthorName` = current user name (NU "Alchimalia Seed")
- ✅ Salvează în DB

**3. Asset Copy Job: Copierea Fișierelor**
- ✅ Colectează toate asset-urile:
  - 1 cover image
  - 4 tile images
  - Audio/Video pentru ro-ro, en-us, hu-hu (dacă există)
- ✅ Pentru fiecare asset:
  - Sursa: `images/tol/stories/seed@alchimalia.com/intro-pufpuf/...`
  - Destinație: `drafts/john.doe@example.com/john-doe-s1/...`
  - Copiază prin Azure Blob Copy (server-side)
  - Polling pentru status (30 secunde timeout)

**4. Rezultat Final:**
- ✅ `StoryCraft` în DB cu `StoryId` = `john-doe-s1`, owner = current user
- ✅ **3 translații** complete (ro-ro, en-us, hu-hu)
- ✅ **5 tiles** complete cu toate translațiile
- ✅ **Quiz-uri** complete cu answers, translations, tokens
- ✅ **Toate asset-urile** copiate în draft storage
- ✅ User-ul poate edita story-ul în Story Editor
- ✅ User-ul poate publica story-ul → `StoryDefinition` nou

### 2.7 Rezumat: Ce SE Copiază vs Ce NU Se Copiază

#### ✅ CE SE COPIAZĂ (100% Complete)

| Categorie | Detalii |
|-----------|---------|
| **Story Metadata** | StoryType, StoryTopic, CoverImageUrl, PriceInCredits, ClassicAuthorId, BaseVersion, IsEvaluative |
| **Translații Story** | **TOATE** translațiile (ro-ro, en-us, hu-hu, etc.) cu Title și Summary |
| **Tiles** | **TOATE** tile-urile (pages, quiz, video) cu TileId, Type, ImageUrl, SortOrder |
| **Tile Translations** | **TOATE** translațiile tile-urilor cu Caption, Text, Question, AudioUrl, VideoUrl |
| **Quiz Answers** | **TOATE** răspunsurile cu AnswerId, IsCorrect, SortOrder |
| **Answer Translations** | **TOATE** translațiile răspunsurilor cu Text |
| **Tokens** | **TOATE** tokens-urile (rewards) cu Type, Value, Quantity |
| **Topics** | **TOATE** topic IDs |
| **Age Groups** | **TOATE** age group IDs |
| **Unlocked Heroes** | **TOATE** hero IDs deblocați |
| **Images** | **TOATE** imaginile (cover + tile images) - copiate fizic din published la draft storage |
| **Audio** | **TOATE** fișierele audio pentru **TOATE** limbile - copiate fizic |
| **Video** | **TOATE** fișierele video pentru **TOATE** limbile - copiate fizic |

#### ❌ CE NU SE COPIAZĂ / SE RESETEAZĂ

| Element | Comportament |
|---------|-------------|
| **AuthorName** | Se setează NULL, apoi se setează la numele user-ului curent (NU se copiază "Alchimalia Seed") |
| **OwnerUserId** | Se setează la current user (NU se copiază seed userId) |
| **CreatedBy** | Va fi setat la current user când se publică (NU se copiază seed userId) |
| **Status** | Se setează la `Draft` (NU se copiază `Published`) |
| **Version** | Se resetează la 0 în craft (dar se păstrează în `BaseVersion`) |
| **CreatedAt** | Se setează la `DateTime.UtcNow` |
| **UpdatedAt** | Se setează la `DateTime.UtcNow` |

#### ⚠️ COMPORTAMENT SPECIAL

| Situație | Comportament |
|----------|-------------|
| **Asset lipsă** | Se skip-ează (se loghează warning), job-ul continuă |
| **Asset copy eșuat** | Job-ul se oprește cu error (după primul eșec) |
| **Craft există deja** | Se refolosește craft-ul existent (nu se creează unul nou) |
| **Title în translații** | Pentru Copy se adaugă prefix "Copy of" (pentru Fork NU se adaugă) |

---

## 3. Analiza Story Fork - Cum Funcționează Momentan

### 2.1 Backend - Fork Story Endpoint

**Locație**: `XooCreator.BA/Features/Story-Editor/Endpoints/ForkStoryEndpoint.cs`

**Endpoint**: `POST /api/stories/{storyId}/fork`

**Request Body**:
```json
{
  "copyAssets": true
}
```

**Proces**:
1. **Autorizare**: Verifică dacă user-ul are rol `Creator` sau `Admin`
2. **Validare storyId**: Nu acceptă `"new"` ca storyId
3. **Încărcare story sursă**: 
   - Prioritizează `StoryDefinition` (published) peste `StoryCraft` (draft)
   - Metodă: `LoadSourceStoryAsync(storyId, ct)`
4. **Validare sursa**: Fork este permis **doar pentru published stories**
   ```csharp
   if (craft != null)
   {
       return TypedResults.BadRequest("Fork is only available for published stories. Use Copy for draft stories.");
   }
   ```
5. **Generare newStoryId**: Folosește `StoryIdGenerator` pentru a genera un ID unic bazat pe user
6. **Creare StoryForkJob**: 
   - Job asincron care copiază story-ul și assets-urile
   - Status inițial: `Queued`
7. **Procesare job**: 
   - Metoda `ProcessForkJobAsync()` creează `StoryCraft` nou
   - Owner-ul craft-ului = user-ul care a făcut fork
   - Assets-urile sunt copiate din published storage la draft storage (dacă `copyAssets = true`)

**Key Points**:
- ✅ Fork funcționează pentru **orice StoryDefinition publicat**
- ✅ Fork creează un **StoryCraft nou** (draft) cu owner-ul = current user
- ✅ **NU există verificări pentru owner-ul original** (poate fi oricine, inclusiv seed@alchimalia.com)
- ✅ Assets-urile sunt copiate automat

### 2.2 Frontend - Fork Story UI

**Locații**:
- `story-editor-list.component.ts` - lista de stories în editor
- `story-reading.component.ts` - story reading page
- `story-controls.component.html` - butoanele de control (Fork, Create Version, etc.)

**Condiții pentru afișarea butonului Fork**:

#### În Story Editor List (`story-editor-list.component.ts`, linia 1843-1845):
```typescript
canForkStory(story: CreatedStoryDto): boolean {
  return this.isCreator() && this.getStoryStatus(story) === 'published';
}
```
- ✅ User trebuie să fie Creator
- ✅ Story trebuie să fie Published
- ✅ **NU verifică dacă story-ul este seeded**

#### În Story Reading (`story-controls.component.html`, linia 33-38):
```html
<button 
  *ngIf="canEdit() && isPublished && !isSeedStory" 
  class="fork-button" 
  (click)="onFork()">
</button>
```
- ✅ User trebuie să aibă permisiuni de edit (Creator)
- ✅ Story trebuie să fie Published
- ❌ **Story NU trebuie să fie seeded** (`!isSeedStory`)

**Problema**: Restricția `!isSeedStory` **blochează fork-ul pentru seeded stories la nivel de UI**.

### 2.3 Verificarea isSeedStory

**Locație**: `story-reading.component.ts`, linia 415-451

**Logică**:
```typescript
private async checkOwnershipAndStatus(): Promise<void> {
  const currentStory = this.stateService.content();
  const ownerEmail = currentStory?.ownerEmail?.toLowerCase()?.trim();
  
  const response = await firstValueFrom(this.myLibrary.loadCreated());
  const stories = response?.stories || [];
  const story = stories.find(s => (s.storyId || s.id) === this.storyId);
  
  if (story) {
    // Story este în created stories - user este owner
    this.isSeedStory.set(false);
  } else {
    // Story NU este în created stories - user NU este owner
    
    // Verifică dacă este seeded:
    const coverImageUrl = currentStory?.coverImageUrl?.toLowerCase() || '';
    const isSeedByEmail = !ownerEmail || ownerEmail === 'seed@alchimalia.com';
    const isSeedByImagePath = coverImageUrl.includes('seed@alchimalia.com');
    this.isSeedStory.set(isSeedByEmail || isSeedByImagePath);
  }
}
```

**Condiții pentru isSeedStory = true**:
1. Story-ul **NU** este în lista de created stories a user-ului curent
2. ȘI:
   - `ownerEmail` este `null`, `empty`, sau `'seed@alchimalia.com'`
   - SAU `coverImageUrl` conține `'seed@alchimalia.com'`

---

## 4. Tree of Light - Structura și Story-urile Seeded

### 3.1 Arhitectura Tree of Light

**Scop**: Un epic pre-configurat care organizează story-urile în regiuni (planete/lumi) cu reguli de deblocare.

**Entități Principale**:
- `TreeConfiguration` - configurația epic-ului (e.g., "puf-puf-journey-v1")
- `TreeRegion` - regiunile/planetele (e.g., Gateway, Terra, Lunaria)
- `TreeStoryNode` - asociază story-uri cu regiuni
- `TreeUnlockRule` - reguli de deblocare între regiuni

**Locație Seed Data**: `XooCreator.BA/Data/SeedData/TreeOfLight/puf-puf-journey-v1.json`

### 3.2 Story-urile din Tree of Light

#### Lista Story-uri Seeded (din `puf-puf-journey-v1.json`):

| StoryId | Regiune | SortOrder | RewardImageUrl |
|---------|---------|-----------|----------------|
| `intro-pufpuf` | gateway | 1 | images/tol/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png |
| `terra-s1` | terra | 1 | images/tol/stories/seed@alchimalia.com/terra-s1/0.Cover.png |
| `terra-s2` | terra | 2 | images/tol/stories/seed@alchimalia.com/terra-s2/0.Cover.png |
| `lunaria-s1` | lunaria | 1 | images/tol/stories/seed@alchimalia.com/lunaria-s1/0.Cover.png |
| `lunaria-s2` | lunaria | 2 | images/tol/stories/seed@alchimalia.com/lunaria-s2/0.Cover.png |
| `mechanika-s1` | mechanika | 1 | images/worlds/pyron.jpg |
| `sylvaria-s1` | sylvaria | 1 | images/worlds/sylvaria.jpg |
| `crystalia-s1` | crystalia | 1 | images/worlds/crystalia.jpg |

**Total**: **8 story-uri seeded**

#### Unlock Rules (Sample):

1. **Rule 1**: Completează `intro-pufpuf` → deblochează regiunea `terra`
2. **Rule 2**: Completează `terra-s1` ȘI `terra-s2` → deblochează regiunea `lunaria`
3. **Rule 3+**: Alte reguli pentru deblocarea regiunilor următoare

### 3.3 Structura Story-urilor Seeded

**Locație**: `XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/i18n/{locale}/*.json`

**Exemplu**: `intro-pufpuf.json` (ro-ro):
```json
{
  "storyId": "intro-pufpuf",
  "title": "Marea Călătorie",
  "coverImageUrl": "images/tol/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png",
  "sortOrder": 1,
  "unlockedStoryHeroes": ["puf-puf"],
  "tiles": [
    {
      "tileId": "1",
      "type": "page",
      "caption": "Chemarea",
      "text": "...",
      "imageUrl": "images/tol/stories/seed@alchimalia.com/intro-pufpuf/1.puf-puf-flying.png"
    },
    ...
  ]
}
```

**Caracteristici**:
- **Owner**: `seed@alchimalia.com` (creat prin `SeedUserHelper`)
- **Status**: Published (`StoryDefinition`)
- **Assets**: Stocate în published storage (`images/tol/stories/seed@alchimalia.com/...`)
- **Translations**: Disponibile în ro-ro, en-us, hu-hu

### 3.4 Procesul de Seeding

**Locație**: `XooCreator.BA/Features/Story-Editor/Repositories/StoriesRepository.cs`, linia 215-658

**Metodă**: `SeedStoriesAsync()`

**Proces**:
1. Verifică dacă story-urile principale există deja în `StoryDefinition`
2. Încarcă story-urile din JSON files (`Data/SeedData/Stories/seed@alchimalia.com/i18n/{locale}/*.json`)
3. Mapează din `StorySeedData` la `StoryDefinition` (prin `StoryDefinitionMapper.MapFromSeedData()`)
4. Setează `CreatedBy` = `seedUserId` (obținut prin `SeedUserHelper.GetOrCreateSeedUserIdAsync()`)
5. Adaugă story-urile în `StoryDefinitions` (direct published, fără craft)
6. Procesează translațiile pentru fiecare locale (ro-ro, en-us, hu-hu)

**Key Point**: Story-urile seeded sunt create **direct în StoryDefinition**, fără StoryCraft.

---

## 5. Posibilitatea de Fork pentru Seeded Stories

### 4.1 Analiza Restricțiilor

#### Backend: ✅ NU există restricții
- Fork endpoint acceptă **orice StoryDefinition publicat**
- Nu verifică owner-ul original
- Fork creează un `StoryCraft` nou cu owner-ul = current user
- Assets-urile sunt copiate din published storage la draft storage

#### Frontend: ❌ Există restricții
- Butonul Fork este **ascuns** pentru seeded stories (`!isSeedStory`)
- Restricția este **doar la nivel de UI**
- Dacă se apelează direct endpoint-ul de fork, funcționează fără probleme

### 4.2 Scenariul de Fork pentru Seeded Story

**Presupunem**: User-ul face fork la `intro-pufpuf`

**Proces**:
1. **Backend**: `POST /api/stories/intro-pufpuf/fork` cu `{ "copyAssets": true }`
2. **Backend**: Încarcă `StoryDefinition` pentru `intro-pufpuf` (owner = seed@alchimalia.com)
3. **Backend**: Generează `newStoryId` = `{user-firstname}-{user-lastname}-s1` (e.g., `john-doe-s1`)
4. **Backend**: Creează `StoryForkJob` cu:
   - `SourceStoryId` = `intro-pufpuf`
   - `SourceType` = `Published`
   - `TargetOwnerUserId` = `{current-user-id}`
   - `TargetStoryId` = `john-doe-s1`
5. **Backend**: Job processing:
   - Mapează `StoryDefinition` → `StoryCloneData`
   - Creează `StoryCraft` nou:
     - `StoryId` = `john-doe-s1`
     - `OwnerUserId` = `{current-user-id}`
     - `Status` = `Draft`
     - `AuthorName` = `{current-user-name}` (NU "Alchimalia Seed")
   - Copiază assets-urile:
     - Sursa: `images/tol/stories/seed@alchimalia.com/intro-pufpuf/...`
     - Destinație: `drafts/{user-email}/john-doe-s1/...`
6. **Backend**: Job completed, user-ul are acum un draft `john-doe-s1`
7. **Frontend**: User-ul poate edita draft-ul în Story Editor
8. **Frontend**: User-ul poate publica draft-ul → creează `StoryDefinition` pentru `john-doe-s1`

**Rezultat**: ✅ User-ul are acum o copie editabilă a story-ului seeded.

### 4.3 Testare Manuală (Recomandată)

Pentru a verifica că fork-ul funcționează:
1. **Apelează direct endpoint-ul** (prin Postman/cURL/DevTools):
   ```bash
   POST http://localhost:5000/api/stories/intro-pufpuf/fork
   Authorization: Bearer {token}
   Content-Type: application/json
   
   {
     "copyAssets": true
   }
   ```
2. **Verifică răspunsul**: ar trebui să primești un `jobId`
3. **Monitorizează job-ul**: verifică status-ul job-ului prin `GET /api/stories/fork/jobs/{jobId}`
4. **Verifică rezultatul**: când job-ul este `Completed`, verifică că există un nou `StoryCraft` în DB

---

## 6. Migrarea Tree of Light la Story Epic

### 5.1 Motivația Migrării

**Probleme cu Tree of Light seeded**:
1. **Hard-coded**: Tree of Light este seed-uit din JSON files
2. **Non-editable**: Nu poate fi modificat prin UI
3. **Single configuration**: Doar o singură configurație (puf-puf-journey-v1)
4. **Seed dependency**: Toate story-urile sunt legate de seed@alchimalia.com

**Beneficiile Story Epic**:
1. **Editable**: Story Epic poate fi creat și editat în UI
2. **Multiple epics**: Pot fi create multiple epics de către utilizatori
3. **Ownership**: Fiecare epic are un owner (user sau admin)
4. **Publishing**: Story Epic poate fi publicat/unpublished prin UI
5. **Versioning**: Story Epic suportă versioning (draft → published)

### 5.2 Arhitectura Story Epic vs Tree of Light

#### Comparație Entități:

| Tree of Light | Story Epic | Scop |
|---------------|------------|------|
| `TreeConfiguration` | `StoryEpicCraft` / `StoryEpicDefinition` | Configurația epic-ului |
| `TreeRegion` | `StoryRegionCraft` / `StoryRegionDefinition` | Regiuni/Planete |
| `TreeStoryNode` | `EpicStoryNodeCraft` / `EpicStoryNodeDefinition` | Story nodes în regiuni |
| `TreeUnlockRule` | `EpicUnlockRuleCraft` / `EpicUnlockRuleDefinition` | Reguli de deblocare |

**Similarități**:
- Ambele organizează story-uri în regiuni
- Ambele au reguli de deblocare
- Ambele pot fi vizualizate într-un graph/tree network

**Diferențe**:
- **Tree of Light**: seeded, global, read-only, single config
- **Story Epic**: editable, owned by user, versionable, multiple configs

### 5.3 Procesul de Migrare - Plan Recomandat

#### Faza 1: Fork Seeded Stories
1. **Elimină restricția UI** pentru fork-ul seeded stories:
   - Modifică `story-controls.component.html`:
     ```html
     <!-- BEFORE -->
     <button *ngIf="canEdit() && isPublished && !isSeedStory" ...>
     
     <!-- AFTER -->
     <button *ngIf="canEdit() && isPublished" ...>
     ```
2. **Fork fiecare story** din Tree of Light:
   - `intro-pufpuf` → `your-name-intro-pufpuf-s1`
   - `terra-s1` → `your-name-terra-s1-s1`
   - `terra-s2` → `your-name-terra-s2-s1`
   - `lunaria-s1` → `your-name-lunaria-s1-s1`
   - `lunaria-s2` → `your-name-lunaria-s2-s1`
   - `mechanika-s1` → `your-name-mechanika-s1-s1`
   - `sylvaria-s1` → `your-name-sylvaria-s1-s1`
   - `crystalia-s1` → `your-name-crystalia-s1-s1`

#### Faza 2: Editare și Publicare Story-uri
1. **Editează fiecare draft** în Story Editor (opțional):
   - Modifică conținutul
   - Adaugă/modifică tile-uri
   - Ajustează traducerile
2. **Publică fiecare story**:
   - Status: Published
   - Assets-uri copiate în published storage

#### Faza 3: Crearea Story Epic-ului
1. **Creează un nou Story Epic** în Story Epic Editor:
   - Epic ID: `tree-of-light-v2` (sau alt nume)
   - Epic Name: `Tree of Light V2`
2. **Adaugă regiunile** (copiind din Tree of Light):
   - Gateway
   - Terra
   - Lunaria
   - Mechanika
   - Sylvaria
   - Crystalia
   - Pyron
   - Vulkara
   - Aqualia
   - Aetherion
   - Umbra
3. **Adaugă story nodes**:
   - Asociază fiecare story publicat cu regiunea corespunzătoare
   - Setează `sortOrder` pentru fiecare
4. **Adaugă unlock rules**:
   - Copiază regulile din Tree of Light
   - Ajustează `storyId` pentru a referenția story-urile noi publicate

#### Faza 4: Publicarea Story Epic-ului
1. **Validează epic-ul** în preview:
   - Verifică că toate story-urile sunt published
   - Verifică că toate regiunile sunt definite
   - Verifică că toate unlock rules sunt corecte
2. **Publică epic-ul**:
   - Status: Published
   - Assets-uri copiate în published storage
   - `StoryEpicDefinition` creat în DB

#### Faza 5: Migrarea Frontend-ului
1. **Modifică Tree of Light component** pentru a încărca din Story Epic:
   - În loc de `/api/{locale}/tree-of-light/state`
   - Apelează `/api/{locale}/story-epic/{epicId}/state`
2. **Păstrează backward compatibility**:
   - Dacă nu există Story Epic publicat, folosește Tree of Light seeded
   - Dacă există Story Epic publicat, folosește Story Epic

### 5.4 Avantajele Migrării

1. **Editabilitate**: Poți edita story-urile și epic-ul prin UI
2. **Versioning**: Poți crea versiuni noi ale epic-ului
3. **Multiple Epics**: Poți crea multiple tree-uri (e.g., "Journey 1", "Journey 2")
4. **Ownership**: Fiecare epic are un owner care poate controla publicarea
5. **Unpublishing**: Poți unpublica un epic dacă nu mai este relevant
6. **Testing**: Poți testa epic-ul în draft mode înainte de a-l publica

---

## 7. Provocări și Soluții

### 6.1 Provocare 1: Assets-urile Seeded

**Problemă**: Assets-urile seeded sunt în `images/tol/stories/seed@alchimalia.com/...`

**Soluție**:
- La fork, assets-urile sunt copiate automat în draft storage (`drafts/{user-email}/{storyId}/...`)
- La publicare, assets-urile sunt copiate în published storage (`images/tol/stories/{user-email}/{storyId}/...`)
- Originalele rămân neatinse în seed storage

### 6.2 Provocare 2: Story IDs

**Problemă**: Story-urile forked vor avea ID-uri noi (e.g., `john-doe-s1` în loc de `intro-pufpuf`)

**Soluție**:
- În Story Epic, referențiază story-urile publicate prin ID-ul lor nou
- Păstrează un mapping între story-urile originale și cele forked (opțional, pentru documentație)

### 6.3 Provocare 3: Translațiile

**Problemă**: Seeded stories au 3 traduceri (ro-ro, en-us, hu-hu)

**Soluție**:
- La fork, toate traducerile sunt copiate în `StoryCraft`
- User-ul poate edita fiecare traducere în Story Editor
- La publicare, toate traducerile sunt copiate în `StoryDefinition`

### 6.4 Provocare 4: Heroes Unlocked

**Problemă**: Seeded stories deblochează eroi (e.g., `intro-pufpuf` deblochează `puf-puf`)

**Soluție**:
- La fork, `unlockedStoryHeroes` este copiat în `StoryCraft`
- User-ul poate modifica lista de eroi deblocați în Story Editor
- În Story Epic, poți referi eroii prin `EpicHeroReference`

### 6.5 Provocare 5: Backward Compatibility

**Problemă**: Utilizatorii existenți au progres în Tree of Light seeded

**Soluție**:
- **Opțiunea 1**: Păstrează Tree of Light seeded și adaugă Story Epic paralel
  - Frontend-ul verifică dacă există Story Epic publicat
  - Dacă DA, folosește Story Epic
  - Dacă NU, folosește Tree of Light seeded
- **Opțiunea 2**: Migrează progresul utilizatorilor
  - Creează un script de migrare care copiază progresul din Tree of Light în Story Epic
  - Mapează story-urile vechi la cele noi (prin `storyId` mapping)

---

## 8. Concluzie și Recomandări

### 8.1 Răspuns la Întrebări

#### 1. **Cum se face momentan Story Fork?**
- Fork este disponibil pentru orice StoryDefinition publicat
- Creează un StoryCraft nou (draft) cu owner-ul = current user
- ✅ **Copiază 100% complet**: toate translațiile, quiz-urile, și assets-urile (images, audio, video)
- Assets-urile sunt copiate automat prin Azure Blob Copy (server-side)
- ❌ **Blocat la nivel de UI pentru seeded stories** (dar funcționează la nivel de backend)

#### 1a. **Se copiază toate translațiile, quiz-urile, audio, video, images?**
- ✅ **DA, 100% complet!** Vezi secțiunea 2 pentru detalii complete
- **Translații**: TOATE limbile (ro-ro, en-us, hu-hu) cu titles, summaries, captions, text, questions
- **Quiz-uri**: TOATE răspunsurile cu translations, IsCorrect flags, tokens (rewards)
- **Images**: TOATE (cover + tile images) - copiate fizic din published la draft storage
- **Audio**: TOATE fișierele pentru TOATE limbile - copiate fizic
- **Video**: TOATE fișierele pentru TOATE limbile - copiate fizic
- **Metadata**: Topics, age groups, unlocked heroes, IsEvaluative, etc.

#### 2. **Putem face Story Fork pentru Tree of Light stories?**
- ✅ **DA**, backend-ul permite fork pentru seeded stories
- ❌ **NU**, frontend-ul blochează butonul Fork pentru seeded stories
- **Soluție**: Elimină restricția `!isSeedStory` din frontend

#### 3. **Putem crea Story Craft din seeded stories?**
- ✅ **DA**, prin fork se creează automat StoryCraft
- Craft-ul poate fi editat în Story Editor
- Craft-ul poate fi publicat → StoryDefinition nou

#### 4. **Putem migra seeded data la Story Epic?**
- ✅ **DA**, procesul recomandat:
  1. Fork toate story-urile seeded
  2. Editează și publică story-urile forked
  3. Creează Story Epic în editor
  4. Adaugă regiuni, story nodes, și unlock rules
  5. Publică Story Epic

### 8.2 Recomandări Prioritizate

#### Prioritate 1: Permite Fork pentru Seeded Stories
- **Task**: Elimină restricția `!isSeedStory` din `story-controls.component.html`
- **Effort**: 1 linie de cod
- **Beneficiu**: Permite fork-ul immediate pentru seeded stories

#### Prioritate 2: Testare Manuală
- **Task**: Testează fork pentru `intro-pufpuf`
- **Effort**: 30 minute
- **Beneficiu**: Validează că procesul funcționează end-to-end

#### Prioritate 3: Fork Toate Story-urile Tree of Light
- **Task**: Fork cele 8 story-uri din Tree of Light
- **Effort**: 1-2 ore (cu editare opțională)
- **Beneficiu**: Story-uri editable în Story Editor

#### Prioritate 4: Creează Story Epic pentru Tree of Light
- **Task**: Creează Story Epic în editor cu story-urile forked
- **Effort**: 2-3 ore
- **Beneficiu**: Tree of Light editable și versionable

#### Prioritate 5: Publică Story Epic
- **Task**: Publică Story Epic-ul
- **Effort**: 30 minute
- **Beneficiu**: Story Epic disponibil pentru utilizatori

#### Prioritate 6 (Opțional): Migrează Frontend
- **Task**: Modifică Tree of Light component pentru a încărca din Story Epic
- **Effort**: 2-4 ore
- **Beneficiu**: Elimină dependența de seed data

### 8.3 Avantaje Finale

1. **Flexibilitate**: Poți edita story-urile și epic-ul prin UI
2. **Multiple Versions**: Poți crea versiuni noi ale Tree of Light
3. **Community**: Alți utilizatori pot crea propriile epic-uri (inspirate din Tree of Light)
4. **Maintenance**: Mai ușor de întreținut (fără seed data hard-coded)
5. **Evolution**: Poți evolua Tree of Light fără rebuild și redeploy

### 8.4 Risc Minim

- **Backend-ul deja suportă fork** pentru published stories
- **Nu sunt necesare modificări de backend** (doar eliminarea restricției UI)
- **Procesul este reversibil** (poți păstra seed data ca fallback)
- **Nu afectează utilizatorii existenți** (progresul lor rămâne intact)

---

## 9. Anexe

### 9.1 Cod de Modificat (Frontend)

**Fișier**: `XooCreator/xoo-creator/src/app/story/story-reading/components/story-controls/story-controls.component.html`

**Linia**: 33-38

```html
<!-- BEFORE -->
<button 
  *ngIf="canEdit() && isPublished && !isSeedStory" 
  class="fork-button" 
  type="button" 
  (click)="onFork()" 
  [title]="'storyreading_fork' | translate">
  <!-- SVG icon -->
</button>

<!-- AFTER -->
<button 
  *ngIf="canEdit() && isPublished" 
  class="fork-button" 
  type="button" 
  (click)="onFork()" 
  [title]="'storyreading_fork' | translate">
  <!-- SVG icon -->
</button>
```

**Motivație**: Permite fork-ul pentru toate story-urile published, inclusiv cele seeded.

### 9.2 Story-uri de Forked

| Original StoryId | Forked StoryId (Example) | Region | Description |
|------------------|--------------------------|--------|-------------|
| `intro-pufpuf` | `john-doe-intro-pufpuf-s1` | gateway | Intro story with Puf-Puf |
| `terra-s1` | `john-doe-terra-s1-s1` | terra | First story in Terra |
| `terra-s2` | `john-doe-terra-s2-s1` | terra | Second story in Terra |
| `lunaria-s1` | `john-doe-lunaria-s1-s1` | lunaria | First story in Lunaria |
| `lunaria-s2` | `john-doe-lunaria-s2-s1` | lunaria | Second story in Lunaria |
| `mechanika-s1` | `john-doe-mechanika-s1-s1` | mechanika | First story in Mechanika |
| `sylvaria-s1` | `john-doe-sylvaria-s1-s1` | sylvaria | First story in Sylvaria |
| `crystalia-s1` | `john-doe-crystalia-s1-s1` | crystalia | First story in Crystalia |

### 9.3 Referințe Codul Sursă

#### Backend:
- **Fork Endpoint**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Endpoints/ForkStoryEndpoint.cs`
- **Fork Job Processing**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Endpoints/ForkStoryEndpoint.JobProcessing.cs`
- **Story Copy Service**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/StoryCopyService.cs`
- **Story Asset Copy Service**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/StoryAssetCopyService.cs` (copiere images, audio, video)
- **Story Source Mapper**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/Cloning/StorySourceMapper.cs` (mapare translations, tiles, quiz)
- **Story Cloner**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/Cloning/StoryCloner.cs` (creare StoryCraft din clone data)
- **Seed Stories**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Repositories/StoriesRepository.cs` (linia 215-658)

#### Frontend:
- **Story Controls**: `XooCreator/xoo-creator/src/app/story/story-reading/components/story-controls/story-controls.component.html`
- **Story Reading**: `XooCreator/xoo-creator/src/app/story/story-reading/story-reading.component.ts`
- **Story Editor List**: `XooCreator/xoo-creator/src/app/story/story-editor-list/story-editor-list.component.ts`
- **Story Editor Service**: `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.service.ts`

#### Story Epic:
- **Story Epic Service**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Story-Epic/Services/StoryEpicService.cs`
- **Story Epic Publishing**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Story-Epic/Services/StoryEpicPublishingService.cs`
- **Publish Endpoint**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Story-Epic/Endpoints/PublishStoryEpicEndpoint.cs`

#### Tree of Light:
- **Seed Data**: `XooCreator.BA/XooCreator.BA/Data/SeedData/TreeOfLight/puf-puf-journey-v1.json`
- **Seeded Stories**: `XooCreator.BA/XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/i18n/{locale}/*.json`
- **Analysis Doc**: `XooCreator/xoo-creator/002.Documentation/Story-Epic/02_TREE_OF_LIGHT_ANALYSIS.md`

### 9.4 Documentație Relevantă

- **Fork Story Bug Analysis**: `XooCreator.BA/XooCreator.BA/Documentation/FORK_STORY_BUG_ANALYSIS.md`
- **Fork Author Issue**: `XooCreator/xoo-creator/002.Documentation/StoryEditorLatestVersion/10_FORK_AUTHOR_ISSUE.md`
- **Story Copy/Fork/Version Plan**: `XooCreator/xoo-creator/002.Documentation/StoryEditor/STORY_COPY_FORK_VERSION_PLAN.md`
- **Tree of Light Analysis**: `XooCreator/xoo-creator/002.Documentation/Story-Epic/02_TREE_OF_LIGHT_ANALYSIS.md`

---

**Autor**: AI Analysis  
**Status**: Draft  
**Următorii Pași**: Testare manuală și implementare incrementală

