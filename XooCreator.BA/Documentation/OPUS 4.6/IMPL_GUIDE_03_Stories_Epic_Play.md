# Ghid de Implementare 03 — Stories & Epic Play/Reading

**Referință:** `PROBLEMS_SCAN_28.feb.2026.md`  
**Probleme acoperite:** STR-01 → STR-11  
**Efort estimat:** 3–4 zile  
**Impact:** Mare — afectează experiența de reading/play

**Implementat (fără breaking changes):** Task 1 (Quiz answers filter în DB), Task 3 (Batch evaluation updates), Task 4 (Hero image batch), Task 5 (AsNoTracking pe StoriesRepository + EpicProgressRepository), Task 8 (Story details tiles overfetch), Task 9 (Reduce verbose logging). **Neimplementat:** Task 2 (DB pagination User Created Stories — sortare cross-surse), Task 6 (StoriesService sequential calls — risc concurență DbContext), Task 7 (ILike/index — păstrat ILike, fără schimbare index).

---

## Task 1: Fix Quiz Answers — Filter în DB (STR-01)

### Fișier: `Features/Stories/Endpoints/GetStoryQuizAnswersEndpoint.cs`

### Problema
Liniile 51–60: Încarcă TOATE quiz answers ale userului, apoi filtrează by storyId în memorie cu `StringComparison.OrdinalIgnoreCase`.

### Modificare

**Înlocuiește** (liniile 49–60):
```csharp
// Get all quiz answers for this user and story
// We get the latest answers (most recent session) for each tile
var allAnswers = await ep._context.StoryQuizAnswers
    .Where(a => a.UserId == userId.Value)
    .ToListAsync(ct);

// Filter in memory for case-insensitive StoryId match
var storyAnswers = allAnswers
    .Where(a => string.Equals(a.StoryId, storyId, StringComparison.OrdinalIgnoreCase))
    .GroupBy(a => a.TileId)
    .Select(g => g.OrderByDescending(a => a.AnsweredAt).First()) // Get most recent answer for each tile
    .ToList();
```

**Cu:**
```csharp
var storyAnswers = await ep._context.StoryQuizAnswers
    .AsNoTracking()
    .Where(a => a.UserId == userId.Value
        && EF.Functions.ILike(a.StoryId, storyId))
    .ToListAsync(ct);

// Get most recent answer for each tile (in memory — set is now small)
var latestAnswers = storyAnswers
    .GroupBy(a => a.TileId)
    .Select(g => g.OrderByDescending(a => a.AnsweredAt).First())
    .ToList();
```

Și actualizează referința la `storyAnswers` → `latestAnswers` în construcția response-ului (liniile 61–69):
```csharp
var response = new StoryQuizAnswersResponse
{
    Answers = latestAnswers.Select(a => new QuizAnswerDto
    {
        TileId = a.TileId,
        SelectedAnswerId = a.SelectedAnswerId,
        IsCorrect = a.IsCorrect
    }).ToList()
};
```

---

## Task 2: Fix User Created Stories — DB Pagination (STR-02)

### Fișier: `Features/Library/Endpoints/GetUserCreatedStoriesEndpoint.cs`

### Problema
Încarcă toate published stories + toate drafts, apoi paginare în memorie.

### Abordare
Transformă query-urile să folosească `Skip`/`Take` direct în SQL. Aceasta e o schimbare mai complexă care necesită restructurarea logicii.

### Modificare conceptuală:

```csharp
// Pas 1: Count total din ambele surse
var publishedCount = await publishedStoriesQuery.CountAsync(ct);
var draftCount = await draftStoriesQuery.CountAsync(ct);
var totalCount = publishedCount + draftCount;

// Pas 2: Determină ce trebuie luat din fiecare sursă
var skip = Math.Max(0, (page - 1) * pageSize);

// Dacă skip < publishedCount, începem din published
List<UserCreatedStoryDto> pageStories;
if (skip < publishedCount)
{
    var fromPublished = await publishedStoriesQuery
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync(ct);

    var remaining = pageSize - fromPublished.Count;
    if (remaining > 0)
    {
        var fromDrafts = await draftStoriesQuery
            .Take(remaining)
            .ToListAsync(ct);
        pageStories = fromPublished.Concat(fromDrafts).ToList();
    }
    else
    {
        pageStories = fromPublished;
    }
}
else
{
    var draftSkip = skip - publishedCount;
    pageStories = await draftStoriesQuery
        .Skip(draftSkip)
        .Take(pageSize)
        .ToListAsync(ct);
}
```

### NOTĂ
Exact cum se face sorting-ul între published și drafts determină dacă abordarea de mai sus funcționează. Dacă sorting-ul e cross-surse (ex: sort by date indiferent de status), va trebui un UNION sau un query unificat.

---

## Task 3: Fix Evaluation N+1 Updates (STR-03)

### Fișier: `Features/Stories/Endpoints/CompleteEvaluationEndpoint.cs`

### Problema
Liniile 239–256: Loop de `FirstOrDefaultAsync` per answer pentru update.

### Modificare

**Înlocuiește** (liniile 237–259):
```csharp
// Update all stored IsCorrect values to match current story version
if (answersToUpdate.Any())
{
    foreach (var (id, newIsCorrect) in answersToUpdate)
    {
        var answerToUpdate = await ep._context.StoryQuizAnswers
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        if (answerToUpdate != null)
        {
            var oldIsCorrect = answerToUpdate.IsCorrect;
            answerToUpdate.IsCorrect = newIsCorrect;

            if (oldIsCorrect != newIsCorrect)
            {
                ep._logger.LogInformation(
                    "Updated stored IsCorrect: AnswerId={AnswerId} OldIsCorrect={OldIsCorrect} NewIsCorrect={NewIsCorrect}",
                    id, oldIsCorrect, newIsCorrect);
            }
        }
    }

    await ep._context.SaveChangesAsync(ct);
}
```

**Cu:**
```csharp
if (answersToUpdate.Count > 0)
{
    var idsToUpdate = answersToUpdate.Select(x => x.Id).ToList();
    var updateDict = answersToUpdate.ToDictionary(x => x.Id, x => x.NewIsCorrect);
    
    var answersFromDb = await ep._context.StoryQuizAnswers
        .Where(a => idsToUpdate.Contains(a.Id))
        .ToListAsync(ct);

    foreach (var answer in answersFromDb)
    {
        if (updateDict.TryGetValue(answer.Id, out var newIsCorrect) && answer.IsCorrect != newIsCorrect)
        {
            ep._logger.LogDebug(
                "Updated stored IsCorrect: AnswerId={AnswerId} {Old}->{New}",
                answer.Id, answer.IsCorrect, newIsCorrect);
            answer.IsCorrect = newIsCorrect;
        }
    }

    await ep._context.SaveChangesAsync(ct);
}
```

### Explicație
Un singur query cu `Contains` în loc de N query-uri individuale. De asemenea, schimbat `LogInformation` → `LogDebug` (STR-10).

---

## Task 4: Fix Hero Image N+1 (STR-04)

### Fișier: `Features/Story-Editor/Story-Epic/Services/StoryEpicProgressService.cs`

### Problema
`GetHeroImageUrlAsync` e apelat per hero într-un loop, fiecare e un DB call.

### Modificare

**Pas 1:** Adaugă o metodă batch în `StoryEpicProgressService`:

```csharp
private async Task<Dictionary<string, string>> BatchGetHeroImageUrlsAsync(
    IEnumerable<string> heroIds, CancellationToken ct)
{
    var idList = heroIds.Distinct().ToList();
    if (idList.Count == 0) return new Dictionary<string, string>();

    // Try definitions first
    var definitionImages = await _context.EpicHeroDefinitions
        .AsNoTracking()
        .Where(h => idList.Contains(h.Id) && h.ImageUrl != null)
        .Select(h => new { h.Id, h.ImageUrl })
        .ToDictionaryAsync(h => h.Id, h => h.ImageUrl ?? "", ct);

    // For missing IDs, try crafts
    var missingIds = idList.Where(id => !definitionImages.ContainsKey(id)).ToList();
    if (missingIds.Count > 0)
    {
        var craftImages = await _context.EpicHeroCrafts
            .AsNoTracking()
            .Where(h => missingIds.Contains(h.Id) && h.ImageUrl != null)
            .Select(h => new { h.Id, h.ImageUrl })
            .ToDictionaryAsync(h => h.Id, h => h.ImageUrl ?? "", ct);

        foreach (var kvp in craftImages)
            definitionImages[kvp.Key] = kvp.Value;
    }

    return definitionImages;
}
```

**Pas 2:** Înainte de loop-ul cu heroes, pre-load toate imaginile:

```csharp
// Colectează toate heroIds din heroReferences
var allHeroIds = heroReferences
    .Where(h => string.IsNullOrEmpty(h.HeroImageUrl))
    .Select(h => h.HeroId)
    .Distinct()
    .ToList();

var heroImageMap = await BatchGetHeroImageUrlsAsync(allHeroIds, ct);

// În loop, înlocuiește:
// var imageUrl = heroRef.HeroImageUrl ?? await GetHeroImageUrlAsync(heroRef.HeroId, ct);
// Cu:
var imageUrl = heroRef.HeroImageUrl 
    ?? (heroImageMap.TryGetValue(heroRef.HeroId, out var cachedUrl) ? cachedUrl : "");
```

---

## Task 5: AsNoTracking pe Stories/Epic Repositories (STR-05, STR-07)

### Fișier: `Features/Story-Editor/Repositories/StoriesRepository.cs`

**`GetAllStoriesAsync` (liniile ~33–63):** Adaugă `.AsNoTracking()` după `_context.StoryDefinitions`:
```csharp
var query = _context.StoryDefinitions
    .AsNoTracking()
    .Include(s => s.Translations)
    // ... rest of includes
```

**`GetStoryDefinitionByIdAsync` (liniile ~129–164):** Adaugă `.AsNoTracking()`:
```csharp
var story = await _context.StoryDefinitions
    .AsNoTracking()
    .Include(s => s.Translations)
    // ...
```

**ATENȚIE:** `GetStoryDefinitionByIdAsync` este folosit și în `CompleteEvaluationEndpoint`. Verifică că nu se fac update-uri pe obiectul returnat. Din analiza codului, endpoint-ul nu modifică story-ul, deci e safe.

### Fișier: `Features/Story-Editor/Story-Epic/Repositories/EpicProgressRepository.cs`

**`GetEpicProgressAsync` (liniile ~17–27):** Adaugă `.AsNoTracking()`:
```csharp
return await _context.EpicProgress
    .AsNoTracking()
    .Where(p => p.UserId == userId && p.EpicId == epicId)
    // ...
```

**`GetEpicStoryProgressAsync` (liniile ~31–41):** Same.

---

## Task 6: Fix StoriesService — Call-uri Secvențiale (STR-08)

### Fișier: `Features/Stories/Services/StoriesService.cs`

### Problema
`GetStoryByIdAsync` (liniile ~45–98) execută multiple call-uri secvențiale independente.

### Modificare

Identifică call-urile independente și rulează-le cu `Task.WhenAll`. Atenție la shared `DbContext` — nu poți rula query-uri EF Core în paralel pe același context.

**Abordare sigură — combinare unde posibil:**

Dacă `GetUserStoryProgressAsync` și `GetStoryCompletionStatusAsync` pot fi combinate într-un singur query:

```csharp
// Combine progress + completion into one query
var progress = await _repository.GetUserStoryProgressWithCompletionAsync(userId, storyId);
```

Sau, dacă story este deja în cache marketplace:
```csharp
var story = await _repository.GetStoryByIdAsync(storyId, locale);
// Apoi rulează progress și languages în secvență (aceeași contextul DB)
var userProgress = await _repository.GetUserStoryProgressAsync(userId, storyId);
```

---

## Task 7: Fix ILike pe StoryId — Index Usage (STR-09)

### Fișier: `Features/Story-Editor/Repositories/StoriesRepository.cs`

### Problema
Liniile 174, 201, 232, 253, 292: `EF.Functions.ILike(p.StoryId, storyId)` previne utilizarea indexului.

### Opțiunea A — Normalizare la insert (recomandată pe termen lung):
Toate `StoryId`-urile sunt salvate lowercase. Apoi folosești `==` simplu:
```csharp
.Where(p => p.StoryId == storyId.ToLowerInvariant())
```

### Opțiunea B — Functional index (rapid de implementat):
Adaugă un script SQL:
```sql
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_user_story_read_progress_storyid_lower
    ON "UserStoryReadProgress" (LOWER("StoryId"));
```

Și continuă să folosești `ILike`.

### Opțiunea C — Folosește `==` direct:
Dacă StoryId-urile sunt deja consistente ca format:
```csharp
// ÎNAINTE
.Where(p => EF.Functions.ILike(p.StoryId, storyId))

// DUPĂ
.Where(p => p.StoryId == storyId)
```

### Recomandare: Opțiunea C dacă datele sunt deja consistente. Opțiunea B ca backup.

---

## Task 8: Fix Story Details Overfetching Tiles (STR-06)

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/StoriesMarketplaceRepository.cs`

### Problema
Liniile ~527–536: `Include(s => s.Tiles)` doar pentru `def.Tiles.Count`.

### Modificare

**Elimină** `.Include(s => s.Tiles)` din query-ul de story details.

**Adaugă** un count separat:
```csharp
var totalTiles = await _context.StoryTiles
    .AsNoTracking()
    .CountAsync(t => t.StoryDefinitionId == def.Id);
```

Sau proiectează direct:
```csharp
var totalTiles = await _context.StoryDefinitions
    .AsNoTracking()
    .Where(sd => sd.Id == def.Id)
    .Select(sd => sd.Tiles.Count)
    .FirstOrDefaultAsync();
```

---

## Task 9: Reduce Verbose Logging (STR-10)

### Fișiere:
- `Features/Stories/Endpoints/CompleteEvaluationEndpoint.cs`
- `Features/Stories/Endpoints/SubmitQuizAnswerEndpoint.cs`

### Modificare

Caută toate `ep._logger.LogInformation(` pe hot paths și schimbă în `ep._logger.LogDebug(`:

```csharp
// ÎNAINTE
ep._logger.LogInformation("CompleteEvaluation: Loaded story...");
ep._logger.LogInformation("Quiz answer (from DB):...");
ep._logger.LogInformation("Recalculated IsCorrect:...");

// DUPĂ
ep._logger.LogDebug("CompleteEvaluation: Loaded story...");
ep._logger.LogDebug("Quiz answer (from DB):...");
ep._logger.LogDebug("Recalculated IsCorrect:...");
```

Menține `LogWarning` și `LogError` neschimbate. Schimbă doar `LogInformation` care logează detalii per-tile/per-answer.

---

## Ordinea de implementare recomandată

1. **Task 1** (Quiz answers filter) — cel mai simplu, impact mare
2. **Task 5** (AsNoTracking) — simplu, se aplică rapid
3. **Task 3** (Batch evaluation updates) — impact mediu
4. **Task 4** (Hero image batch) — impact mare pe epic progress
5. **Task 9** (Reduce logging) — simplu, reduce I/O overhead
6. **Task 8** (Tiles overfetch) — simplu
7. **Task 7** (ILike index) — necesită decizie de abordare
8. **Task 2** (DB pagination) — complex, necesită restructurare
9. **Task 6** (Sequential calls) — complex, atenție la EF concurrency

---

*Ghid generat pe baza analizei din PROBLEMS_SCAN_28.feb.2026.md*
