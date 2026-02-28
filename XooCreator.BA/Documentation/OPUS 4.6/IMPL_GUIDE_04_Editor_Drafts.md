# Ghid de Implementare 04 — Editor & Drafts

**Referință:** `PROBLEMS_SCAN_28.feb.2026.md`  
**Probleme acoperite:** EDT-01 → EDT-09  
**Efort estimat:** 3–5 zile  
**Impact:** Mare — afectează viteza de editing și publishing

**Implementat (fără breaking changes):** Task 1, 2, 3, 4, 5, 7, 8, 9. **Neimplementat:** Task 6 (Streaming ZIP — schimbare de flow/API).

---

## Task 1: Fix Dialog Nodes N+1 (EDT-01) ✅

### Fișier: `Features/Story-Editor/Services/Content/StoryTileUpdater.cs`

### Problema
Liniile ~174–183: Per fiecare dialog tile, se face un query separat cu Include-uri complexe pentru dialog nodes.

### Modificare

**Pas 1:** Înainte de loop-ul prin tiles, pre-load toate dialog nodes:

```csharp
// Colectează toate dialog tile IDs
var dialogTileIds = tiles
    .Where(t => string.Equals(t.Type, "dialog", StringComparison.OrdinalIgnoreCase)
        && t.DialogTile?.Id != null && t.DialogTile.Id != Guid.Empty)
    .Select(t => t.DialogTile!.Id)
    .Distinct()
    .ToList();

// Un singur query pentru toate nodes
Dictionary<Guid, List<StoryCraftDialogNode>> nodesByDialogTileId = new();
if (dialogTileIds.Count > 0)
{
    var allNodes = await _context.StoryCraftDialogNodes
        .Include(n => n.Translations)
        .Include(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
        .Include(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
        .Where(n => dialogTileIds.Contains(n.StoryCraftDialogTileId))
        .ToListAsync(ct);

    nodesByDialogTileId = allNodes
        .GroupBy(n => n.StoryCraftDialogTileId)
        .ToDictionary(g => g.Key, g => g.ToList());
}
```

**Pas 2:** În `UpdateDialogDataAsync`, înlocuiește query-ul individual:

```csharp
// ÎNAINTE
var existingNodes = await _context.StoryCraftDialogNodes
    .Include(n => n.Translations)
    .Include(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
    .Include(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
    .Where(n => n.StoryCraftDialogTileId == dialogTile.Id)
    .ToListAsync(ct);

// DUPĂ — primește nodes ca parametru
// Schimbă semnătura: adaugă `List<StoryCraftDialogNode>? preloadedNodes = null`
var existingNodes = preloadedNodes
    ?? await _context.StoryCraftDialogNodes
        .Include(n => n.Translations)
        .Include(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
        .Include(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
        .Where(n => n.StoryCraftDialogTileId == dialogTile.Id)
        .ToListAsync(ct);
```

**Pas 3:** La apelul din loop, pasează nodes din dicționar:

```csharp
var preloaded = nodesByDialogTileId.TryGetValue(dialogTile.Id, out var nodes) ? nodes : null;
await UpdateDialogDataAsync(dialogTile, dto.DialogData, preloaded, ct);
```

---

## Task 2: Fix ListStoryCraftsEndpoint — Filter în DB (EDT-02) ✅

### Fișier: `Features/Story-Editor/Endpoints/ListStoryCraftsEndpoint.cs`

### Problema
Liniile ~59–69: Încarcă TOATE craft-urile, apoi filtrare în memorie pentru `wantAssigned`/`wantClaimable`.

### Modificare

**Pas 1:** Adaugă metode noi în `IStoryCraftsRepository` și `StoryCraftsRepository`:

```csharp
// În IStoryCraftsRepository.cs
Task<List<StoryCraft>> ListByAssignedReviewerAsync(Guid reviewerUserId, CancellationToken ct = default);
Task<List<StoryCraft>> ListClaimableAsync(CancellationToken ct = default);
```

```csharp
// În StoryCraftsRepository.cs
public async Task<List<StoryCraft>> ListByAssignedReviewerAsync(Guid reviewerUserId, CancellationToken ct = default)
{
    return await _context.StoryCrafts
        .AsNoTracking()
        .Include(s => s.Translations)
        .Where(x => x.AssignedReviewerUserId == reviewerUserId)
        .OrderByDescending(x => x.UpdatedAt)
        .ToListAsync(ct);
}

public async Task<List<StoryCraft>> ListClaimableAsync(CancellationToken ct = default)
{
    return await _context.StoryCrafts
        .AsNoTracking()
        .Include(s => s.Translations)
        .Where(x => x.AssignedReviewerUserId == null
            && (x.Status == "review" || x.Status == "submitted"))
        .OrderByDescending(x => x.UpdatedAt)
        .ToListAsync(ct);
}
```

**Pas 2:** În endpoint, folosește metodele noi:

```csharp
// ÎNAINTE
var fullList = user.IsAdmin
    ? await ep._crafts.ListAllAsync(ct)
    : await ep._crafts.ListByOwnerAsync(user.Id, ct);

if (wantAssigned)
    fullList = fullList.Where(c => c.AssignedReviewerUserId == user.Id).ToList();
else if (wantClaimable)
    fullList = fullList.Where(c => c.AssignedReviewerUserId == null && ...).ToList();

// DUPĂ
List<StoryCraft> fullList;
if (wantAssigned)
    fullList = await ep._crafts.ListByAssignedReviewerAsync(user.Id, ct);
else if (wantClaimable)
    fullList = await ep._crafts.ListClaimableAsync(ct);
else if (user.IsAdmin)
    fullList = await ep._crafts.ListAllAsync(ct);
else
    fullList = await ep._crafts.ListByOwnerAsync(user.Id, ct);
```

---

## Task 3: AsNoTracking pe StoryCraftsRepository (EDT-03) ✅

### Fișier: `Features/Story-Editor/Repositories/StoryCraftsRepository.cs`

### Problema
`GetAsync`, `GetWithLanguageAsync`, `ListByOwnerAsync`, `ListAllAsync` nu folosesc `AsNoTracking()`.

### Modificare

**Abordare:** Creează overload-uri cu trailing `bool tracked = false` parameter sau metode separate.

**Opțiunea simplă — adaugă `.AsNoTracking()` pe metodele de listare:**

```csharp
// ListByOwnerAsync
public async Task<List<StoryCraft>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
{
    return await _context.StoryCrafts
        .AsNoTracking()  // <-- ADAUGĂ
        .Include(s => s.Translations)
        .Where(x => x.OwnerUserId == ownerUserId)
        .OrderByDescending(x => x.UpdatedAt)
        .ToListAsync(ct);
}

// ListAllAsync
public async Task<List<StoryCraft>> ListAllAsync(CancellationToken ct = default)
{
    return await _context.StoryCrafts
        .AsNoTracking()  // <-- ADAUGĂ
        .Include(s => s.Translations)
        .OrderByDescending(x => x.UpdatedAt)
        .ToListAsync(ct);
}
```

**Pentru `GetAsync` și `GetWithLanguageAsync`:**

Aceste metode sunt folosite atât pentru read (GetStoryForEditAsync) cât și pentru write (SaveDraftAsync). 

**Opțiune recomandată:** Adaugă un parametru `bool readOnly = false`:

```csharp
public async Task<StoryCraft?> GetAsync(string storyId, bool readOnly = false, CancellationToken ct = default)
{
    var query = _context.StoryCrafts
        .Include(x => x.Translations)
        // ... all includes ...
        .AsSplitQuery();

    if (readOnly)
        query = query.AsNoTracking();

    return await query.FirstOrDefaultAsync(x => x.StoryId == storyId, ct);
}
```

Apoi la apeluri:
- `GetStoryForEditAsync` → `GetAsync(storyId, readOnly: true)`
- `SaveDraftAsync` → `GetAsync(storyId, readOnly: false)` (default)

---

## Task 4: Fix Double Craft Load în SaveStoryEditEndpoint (EDT-04) ✅

### Fișier: `Features/Story-Editor/Endpoints/SaveStoryEditEndpoint.cs`

### Problema
Craft-ul e loadat o dată pentru validare (linia ~106), apoi a doua oară în `SaveDraftAsync` (linia ~138).

### Modificare

**Pas 1:** Modifică `SaveDraftAsync` sau `StoryEditorService` să accepte un craft deja loadat:

```csharp
// În IStoryEditorService sau endpoint
public async Task SaveDraftAsync(
    Guid ownerUserId, string storyId, string languageCode, 
    EditableStoryDto dto, StoryCraft? existingCraft = null,
    bool bypassOwnershipCheck = false, CancellationToken ct = default)
{
    var craft = existingCraft ?? await _crafts.GetAsync(storyId, ct: ct);
    // ... restul logicii
}
```

**Pas 2:** În endpoint, pasează craft-ul deja loadat:

```csharp
// La validare (deja existent)
var craft = await ep._crafts.GetAsync(storyId, ct: ct);
// ... validare ownership etc.

// La save (pasează craft-ul)
await ep._editorService.SaveDraftAsync(userId, storyId, lang, dto, existingCraft: craft, ct: ct);
```

---

## Task 5: Fix Double Craft Load în Publishing (EDT-05) ✅

### Fișier: `Features/Story-Editor/Services/StoryPublishingService.cs`

### Problema
Liniile ~37–64: `UpsertFromCraftAsync` reîncarcă craft-ul cu aceleași Include-uri, deși caller-ul (StoryPublishQueueWorker) l-a deja loadat.

### Modificare

**Adaugă overload:**

```csharp
public async Task UpsertFromCraftAsync(StoryCraft craft, string languageCode, CancellationToken ct = default)
{
    // Skip reload — use provided craft
    await UpsertFromCraftInternalAsync(craft, languageCode, ct);
}

public async Task UpsertFromCraftAsync(Guid craftId, string languageCode, CancellationToken ct = default)
{
    var craft = await _db.StoryCrafts
        .Include(c => c.Translations)
        // ... all includes ...
        .FirstOrDefaultAsync(c => c.Id == craftId, ct);
    
    if (craft == null) return;
    await UpsertFromCraftInternalAsync(craft, languageCode, ct);
}

private async Task UpsertFromCraftInternalAsync(StoryCraft craft, string languageCode, CancellationToken ct)
{
    // Logica existentă de publish
}
```

**În StoryPublishQueueWorker:** Apelează `UpsertFromCraftAsync(craft, lang, ct)` cu craft-ul deja loadat.

---

## Task 6: Fix StoryExportService — Streaming ZIP (EDT-06)

### Fișier: `Features/Story-Editor/Services/StoryExportService.cs`

### Problema
ZIP-ul e construit complet în `MemoryStream`, apoi `ms.ToArray()` — memorie mare.

### Modificare

**Abordare:** Stream direct în blob storage. Asta necesită schimbarea flow-ului de export.

```csharp
// ÎNAINTE
using var ms = new MemoryStream();
using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
{
    // ... add entries
}
ms.Position = 0;
var bytes = ms.ToArray(); // Duplicare memorie!

// DUPĂ
using var ms = new MemoryStream();
using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
{
    // ... add entries
}
ms.Position = 0;
// Upload direct din stream, fără ToArray()
await blobClient.UploadAsync(ms, overwrite: true, cancellationToken: ct);
```

Dacă `ToArray()` e folosit pentru a returna bytes la client:
```csharp
// Alternativă: returnează stream-ul direct
ms.Position = 0;
return Results.File(ms, "application/zip", $"story-{storyId}.zip");
```

---

## Task 7: Fix PublishStoryEndpoint — StoryId.ToLower() (EDT-07) ✅

### Fișier: `Features/Story-Editor/Endpoints/PublishStoryEndpoint.cs`

### Problema
Liniile ~330–332, 338: `j.StoryId.ToLower() == storyIdLower` — nu poate folosi index.

### Modificare

```csharp
// ÎNAINTE
var storyIdLower = storyId.ToLower();
var existingJob = await ep._context.StoryPublishJobs
    .Where(j => j.StoryId.ToLower() == storyIdLower && j.Status == "queued")
    .FirstOrDefaultAsync(ct);

// DUPĂ
var existingJob = await ep._context.StoryPublishJobs
    .Where(j => EF.Functions.ILike(j.StoryId, storyId) && j.Status == "queued")
    .FirstOrDefaultAsync(ct);
```

Sau mai bine, dacă StoryId-urile sunt consistente:
```csharp
var existingJob = await ep._context.StoryPublishJobs
    .Where(j => j.StoryId == storyId && j.Status == "queued")
    .FirstOrDefaultAsync(ct);
```

---

## Task 8: Fix StoryDraftManager.EnsureDraftAsync (EDT-08) ✅

### Fișier: `Features/Story-Editor/Services/Content/StoryDraftManager.cs`

### Problema
Liniile ~25–37: `GetAsync` încarcă craft complet doar ca să verifice existența.

### Modificare

```csharp
// ÎNAINTE
public async Task EnsureDraftAsync(string storyId, Guid ownerUserId, CancellationToken ct = default)
{
    var existing = await _crafts.GetAsync(storyId, ct: ct);
    if (existing != null) return;
    await _crafts.CreateAsync(storyId, ownerUserId, ct);
}

// DUPĂ
public async Task EnsureDraftAsync(string storyId, Guid ownerUserId, CancellationToken ct = default)
{
    var exists = await _context.StoryCrafts
        .AnyAsync(x => x.StoryId == storyId, ct);
    if (exists) return;
    await _crafts.CreateAsync(storyId, ownerUserId, ct);
}
```

**NOTĂ:** Trebuie injectat `XooDbContext` în `StoryDraftManager` dacă nu e deja disponibil. Alternativ, adaugă o metodă `ExistsAsync(string storyId)` în `IStoryCraftsRepository`.

---

## Task 9: Cache pentru GetAvailableLanguagesAsync (EDT-09) ✅

### Fișier: `Features/Story-Editor/Repositories/StoryCraftsRepository.cs`

### Problema
Liniile ~327–349: Limbile sunt query-uite din DB la fiecare request.

### Modificare

Adaugă cache scurt folosind `IAppCache` sau `IMemoryCache`:

```csharp
public async Task<List<string>> GetAvailableLanguagesAsync(string storyId, CancellationToken ct = default)
{
    var cacheKey = $"story_languages_{storyId}";
    
    if (_cache != null && _cache.TryGetValue(cacheKey, out List<string>? cached))
        return cached ?? new List<string>();

    var languages = await _context.StoryCraftTranslations
        .AsNoTracking()
        .Where(t => t.StoryCraft.StoryId == storyId)
        .Select(t => t.LanguageCode)
        .Distinct()
        .ToListAsync(ct);

    if (_cache != null)
    {
        _cache.Set(cacheKey, languages, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            Size = 1
        });
    }

    return languages;
}
```

**NOTĂ:** Injectează `IMemoryCache` în `StoryCraftsRepository` dacă nu e deja disponibil.

---

## Ordinea de implementare recomandată

1. **Task 3** (AsNoTracking pe crafts) — simplu, impact larg
2. **Task 7** (StoryId.ToLower fix) — simplu, fix index
3. **Task 8** (EnsureDraft AnyAsync) — simplu
4. **Task 2** (ListStoryCrafts filter DB) — mediu, impact mare
5. **Task 1** (Dialog nodes batch) — mediu, impact mare
6. **Task 4** (Double load SaveStoryEdit) — mediu
7. **Task 5** (Double load Publishing) — mediu
8. **Task 9** (Cache languages) — simplu
9. **Task 6** (Streaming ZIP) — complex

---

*Ghid generat pe baza analizei din PROBLEMS_SCAN_28.feb.2026.md*
