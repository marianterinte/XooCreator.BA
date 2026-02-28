# Ghid de Implementare 02 — Marketplace & Reviews

**Referință:** `PROBLEMS_SCAN_28.feb.2026.md`  
**Probleme acoperite:** MKT-01 → MKT-17  
**Efort estimat:** 3–5 zile  
**Impact:** Mare — marketplace e una din zonele cele mai accesate

**Implementat (fără breaking changes):** Task 1, 2, 3, 4, 5, 7. **Neimplementat:** Task 6 (N+1 StoryDetailsMapper — necesită verificare Include/structură), Task 8 (Purchase flow paralel — risc concurență DbContext).

---

## Task 1: SQL Aggregation pentru Review Statistics (MKT-03, MKT-04, MKT-05, MKT-06) ✅

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/StoryReviewsRepository.cs`

### 1A. Fix `GetReviewStatisticsAsync` (story reviews)

**Caută** metoda `GetReviewStatisticsAsync` care face:
```csharp
var reviews = await _context.StoryReviews
    .Where(r => r.StoryId == storyId && r.IsActive)
    .ToListAsync();
var totalCount = reviews.Count;
var averageRating = reviews.Average(r => r.Rating);
var ratingDistribution = reviews.GroupBy(r => r.Rating)...
```

**Înlocuiește** cu:
```csharp
public async Task<ReviewStatisticsDto> GetReviewStatisticsAsync(string storyId)
{
    var stats = await _context.StoryReviews
        .AsNoTracking()
        .Where(r => r.StoryId == storyId && r.IsActive)
        .GroupBy(r => r.Rating)
        .Select(g => new { Rating = g.Key, Count = g.Count() })
        .ToListAsync();

    var totalCount = stats.Sum(s => s.Count);
    var averageRating = totalCount > 0
        ? stats.Sum(s => s.Rating * s.Count) / (double)totalCount
        : 0;

    var ratingDistribution = Enumerable.Range(1, 5)
        .ToDictionary(
            r => r,
            r => stats.FirstOrDefault(s => s.Rating == r)?.Count ?? 0);

    return new ReviewStatisticsDto
    {
        TotalCount = totalCount,
        AverageRating = Math.Round(averageRating, 1),
        RatingDistribution = ratingDistribution
    };
}
```

### 1B. Fix `GetGlobalReviewStatisticsAsync`

**Caută** metoda care face `await _context.StoryReviews.Where(r => r.IsActive).ToListAsync()`.

**Înlocuiește** cu aceeași abordare de SQL aggregation:
```csharp
public async Task<GlobalReviewStatisticsDto> GetGlobalReviewStatisticsAsync()
{
    var stats = await _context.StoryReviews
        .AsNoTracking()
        .Where(r => r.IsActive)
        .GroupBy(r => r.Rating)
        .Select(g => new { Rating = g.Key, Count = g.Count() })
        .ToListAsync();

    var totalCount = stats.Sum(s => s.Count);
    var averageRating = totalCount > 0
        ? stats.Sum(s => s.Rating * s.Count) / (double)totalCount
        : 0;

    return new GlobalReviewStatisticsDto
    {
        TotalReviews = totalCount,
        AverageRating = Math.Round(averageRating, 1),
        RatingDistribution = Enumerable.Range(1, 5)
            .ToDictionary(r => r, r => stats.FirstOrDefault(s => s.Rating == r)?.Count ?? 0)
    };
}
```

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/EpicReviewsRepository.cs`

### 1C. Fix `GetReviewStatisticsAsync` (epic reviews)

Aplică exact același pattern ca la 1A dar pe `_context.EpicReviews` cu `EpicId`.

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/EpicsMarketplaceRepository.cs`

### 1D. Fix inline review stats în `GetEpicDetailsAsync`

**Caută** (liniile ~165–172):
```csharp
var epicReviews = await _context.EpicReviews
    .Where(r => r.EpicId == epicId && r.IsActive)
    .ToListAsync();
var totalReviews = epicReviews.Count;
var avgRating = totalReviews > 0 ? epicReviews.Average(r => r.Rating) : 0.0;
```

**Înlocuiește** cu:
```csharp
var reviewStats = await _context.EpicReviews
    .AsNoTracking()
    .Where(r => r.EpicId == epicId && r.IsActive)
    .GroupBy(r => 1)
    .Select(g => new
    {
        Total = g.Count(),
        Avg = g.Average(r => (double)r.Rating)
    })
    .FirstOrDefaultAsync();

var totalReviews = reviewStats?.Total ?? 0;
var avgRating = reviewStats?.Avg ?? 0.0;
```

---

## Task 2: Fix Favorites — Load by IDs (MKT-01, MKT-02) ✅

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/FavoritesRepository.cs`

### 2A. Fix story favorites

**Caută** pattern-ul `PageSize = 10000` (liniile ~95–99).

**Abordare nouă:** Mai întâi obține ID-urile favorite, apoi încarcă doar acele stories.

**Înlocuiește** metoda cu:
```csharp
public async Task<List<StoryMarketplaceItemDto>> GetUserFavoriteStoriesAsync(Guid userId, string locale)
{
    var favoriteStoryDefIds = await _context.UserFavoriteStories
        .AsNoTracking()
        .Where(f => f.UserId == userId)
        .Select(f => f.StoryDefinitionId)
        .ToListAsync();

    if (favoriteStoryDefIds.Count == 0)
        return new List<StoryMarketplaceItemDto>();

    var favoriteStoryIds = await _context.StoryDefinitions
        .AsNoTracking()
        .Where(sd => favoriteStoryDefIds.Contains(sd.Id) && sd.IsActive)
        .Select(sd => sd.StoryId)
        .ToListAsync();

    // Reutilizează marketplace repository dar cu filtru pe IDs
    var (stories, _, _) = await _marketplaceRepository.GetMarketplaceStoriesWithPaginationAsync(
        userId,
        locale,
        new SearchStoriesRequest
        {
            Page = 1,
            PageSize = favoriteStoryIds.Count,
            StoryIds = favoriteStoryIds // TREBUIE adăugat acest filtru în SearchStoriesRequest
        });

    return stories;
}
```

**ATENȚIE:** Trebuie adăugat un câmp `StoryIds` în `SearchStoriesRequest` și suport în `GetMarketplaceStoriesWithPaginationAsync` pentru a filtra pe o listă de IDs. Alternativ, poți crea o metodă separată `GetStoriesByIdsAsync(List<string> storyIds, string locale)` în `StoriesMarketplaceRepository`.

### Alternativă mai simplă (fără a modifica SearchStoriesRequest):

Adaugă o nouă metodă în `StoriesMarketplaceRepository`:
```csharp
public async Task<List<StoryMarketplaceItemDto>> GetStoriesByStoryIdsAsync(
    Guid userId, string locale, List<string> storyIds)
{
    if (storyIds.Count == 0) return new List<StoryMarketplaceItemDto>();

    // Folosește cache-ul marketplace dacă e disponibil
    var (allStories, _, _) = await GetMarketplaceStoriesWithPaginationAsync(
        userId, locale,
        new SearchStoriesRequest { Page = 1, PageSize = storyIds.Count });

    // Filtrare simplă — dacă cache-ul are deja stories
    return allStories
        .Where(s => storyIds.Contains(s.StoryId, StringComparer.OrdinalIgnoreCase))
        .ToList();
}
```

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/EpicFavoritesRepository.cs`

### 2B. Fix epic favorites

Aplică același pattern: obține `favoriteEpicIds` din `UserFavoriteEpics`, apoi încarcă doar acele epics.

---

## Task 3: AsNoTracking pe toate query-urile read-only (MKT-09) ✅

### Fișiere afectate și locuri exacte:

**EpicsMarketplaceRepository.cs:**
- `GetEpicDetailsAsync` — adaugă `.AsNoTracking()` pe query-ul principal (linia ~146)
- `GetEpicReadersCountAsync` — adaugă `.AsNoTracking()` (linia ~307)

**EpicReviewsRepository.cs:**
- `GetEpicReviewsAsync` — adaugă `.AsNoTracking()` pe query-ul principal (linia ~114)

**EpicFavoritesRepository.cs:**
- Toate query-urile cu `_context.StoryEpicDefinitions` și `_context.UserFavoriteEpics` care sunt read-only — adaugă `.AsNoTracking()`

**StoryReviewsRepository.cs:**
- `GetStoryReviewsAsync` — adaugă `.AsNoTracking()` pe query-ul principal (linia ~116)

**FavoritesRepository.cs:**
- Toate query-urile read-only — adaugă `.AsNoTracking()`

**StoriesMarketplaceRepository.cs:**
- `GetUserPurchasedStoriesAsync` — adaugă `.AsNoTracking()` (linia ~382)
- `GetComputedPriceAsync` — adaugă `.AsNoTracking()` (linia ~447)

**StoryLikesRepository.cs / EpicLikesRepository.cs:**
- Query-uri de count/check (IsLiked, GetLikesCount) — adaugă `.AsNoTracking()`
- **NU adăuga** pe query-urile de toggle (Add/Remove) care necesită tracking

### Pattern de modificare:
```csharp
// ÎNAINTE
var result = await _context.StoryReviews
    .Where(r => r.StoryId == storyId)
    .ToListAsync();

// DUPĂ
var result = await _context.StoryReviews
    .AsNoTracking()
    .Where(r => r.StoryId == storyId)
    .ToListAsync();
```

---

## Task 4: Paralelizare GetReadersSummaryEndpoint (MKT-10) ✅

### Fișier: `Features/TalesOfAlchimalia/Market/Endpoints/GetReadersSummaryEndpoint.cs`

### Modificare

**Caută** (liniile ~53–69) call-urile secvențiale:
```csharp
var totalReaders = await ep._marketplaceService.GetTotalReadersAsync();
var leaderboard = await ep.BuildLeaderboardAsync(ct);
var trendPoints = await ep._marketplaceService.GetReadersTrendAsync(7);
var correlationRaw = await ep._marketplaceService.GetReadersVsReviewsAsync(10);
```

**Înlocuiește** cu:
```csharp
var totalReadersTask = ep._marketplaceService.GetTotalReadersAsync();
var leaderboardTask = ep.BuildLeaderboardAsync(ct);
var trendPointsTask = ep._marketplaceService.GetReadersTrendAsync(7);
var correlationRawTask = ep._marketplaceService.GetReadersVsReviewsAsync(10);

await Task.WhenAll(totalReadersTask, leaderboardTask, trendPointsTask, correlationRawTask);

var totalReaders = totalReadersTask.Result;
var leaderboard = leaderboardTask.Result;
var trendPoints = trendPointsTask.Result;
var correlationRaw = correlationRawTask.Result;
```

**ATENȚIE:** Aceste task-uri folosesc `XooDbContext` care e scoped. `Task.WhenAll` pe aceleași context instance poate cauza probleme de concurrency cu EF Core. 

**Alternativă mai sigură** — folosește `IServiceScopeFactory`:
```csharp
// Sau pur și simplu rulează 2 grupuri secvențiale (reduce de la 4 la 2 round-trips):
var totalReadersTask = ep._marketplaceService.GetTotalReadersAsync();
var trendPointsTask = ep._marketplaceService.GetReadersTrendAsync(7);
await Task.WhenAll(totalReadersTask, trendPointsTask);

var leaderboard = await ep.BuildLeaderboardAsync(ct);
var correlationRaw = await ep._marketplaceService.GetReadersVsReviewsAsync(10);
```

---

## Task 5: File I/O Async (MKT-11) ✅

### Fișiere afectate:
- `StoriesMarketplaceRepository.cs` — `GetSummaryFromJson` method
- `MarketplaceCatalogCache.cs` — cache load methods
- `Mappers/StoryDetailsMapper.cs` — `GetSummaryFromJson`

### Pattern de modificare:
```csharp
// ÎNAINTE
var json = File.ReadAllText(filePath);

// DUPĂ
var json = await File.ReadAllTextAsync(filePath);
```

Asigură-te că metoda calling devine `async` dacă nu era deja.

---

## Task 6: Fix N+1 în StoryDetailsMapper (MKT-07)

### Fișier: `Features/TalesOfAlchimalia/Market/Mappers/StoryDetailsMapper.cs`

### Problema
Liniile ~44–47: query separat pentru author name.  
Liniile ~59–66: query separat pentru user review.

### Modificare

**Pentru author:** Include `Owner` sau `CreatedBy` user în query-ul principal din `StoriesMarketplaceRepository.GetStoryDetailsAsync`:

```csharp
// În GetStoryDetailsAsync, la query-ul principal, adaugă:
.Include(s => s.CoAuthors).ThenInclude(c => c.User)
// Și proiectează authorName direct:
var authorName = def.CoAuthors?.FirstOrDefault()?.User?.Name 
    ?? await _context.AlchimaliaUsers
        .AsNoTracking()
        .Where(u => u.Id == def.CreatedBy)
        .Select(u => u.Name)
        .FirstOrDefaultAsync();
```

**Pentru user review:** Combine cu query-ul principal prin Include sau subquery.

---

## Task 7: Fix FavoritesRepository.IsFavoriteAsync (MKT-16) ✅

### Fișier: `Features/TalesOfAlchimalia/Market/Repositories/FavoritesRepository.cs`

**Caută** (liniile ~73–82):
```csharp
var storyDef = await _context.StoryDefinitions
    .FirstOrDefaultAsync(s => s.StoryId == storyId);
...
return await _context.UserFavoriteStories
    .AnyAsync(f => f.UserId == userId && f.StoryDefinitionId == storyDef.Id);
```

**Înlocuiește** cu un singur query:
```csharp
public async Task<bool> IsFavoriteAsync(Guid userId, string storyId)
{
    return await _context.UserFavoriteStories
        .AsNoTracking()
        .AnyAsync(f => f.UserId == userId
            && f.StoryDefinition.StoryId == storyId);
}
```

---

## Task 8: Fix Purchase Flow — Reduce DB Calls (MKT-17)

### Fișier: `Features/TalesOfAlchimalia/Market/Services/StoriesMarketplaceService.cs`

### Problema
`PurchaseStoryAsync` face 5 DB calls secvențiale.

### Modificare — combinare story lookup cu price:

```csharp
public async Task<PurchaseStoryResponse> PurchaseStoryAsync(Guid userId, PurchaseStoryRequest request)
{
    // Combine story lookup + isPurchased into parallel queries
    var defTask = _context.StoryDefinitions
        .AsNoTracking()
        .FirstOrDefaultAsync(s => s.StoryId == request.StoryId && s.IsActive);
    var isPurchasedTask = _repository.IsStoryPurchasedAsync(userId, request.StoryId);
    
    await Task.WhenAll(defTask, isPurchasedTask);
    
    var def = defTask.Result;
    if (def == null)
        return new PurchaseStoryResponse { Success = false, ErrorMessage = "Story not found" };
    
    if (isPurchasedTask.Result)
        return new PurchaseStoryResponse { Success = false, ErrorMessage = "Story already purchased" };

    var price = await _repository.GetComputedPriceAsync(request.StoryId);
    var purchaseSuccess = await _repository.PurchaseStoryAsync(userId, request.StoryId, price);

    if (!purchaseSuccess)
    {
        var wallet = await _context.CreditWallets.AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId);
        var currentBalance = wallet?.DiscoveryBalance ?? 0;
        return new PurchaseStoryResponse
        {
            Success = false,
            ErrorMessage = currentBalance < price ? "Insufficient credits" : "Purchase failed",
            RemainingCredits = currentBalance
        };
    }

    await _repository.EnsureStoryReaderAsync(userId, request.StoryId, StoryAcquisitionSource.Purchase);
    
    var updatedWallet = await _context.CreditWallets.AsNoTracking()
        .FirstOrDefaultAsync(w => w.UserId == userId);
    
    return new PurchaseStoryResponse
    {
        Success = true,
        RemainingCredits = updatedWallet?.DiscoveryBalance ?? 0,
        CreditsSpent = price
    };
}
```

---

## Ordinea de implementare recomandată

1. **Task 1** (SQL aggregation) — cel mai mare impact pe performanță
2. **Task 3** (AsNoTracking) — simplu, se poate face rapid
3. **Task 2** (Favorites by IDs) — reduce memory cu 95%
4. **Task 7** (IsFavorite single query) — simplu
5. **Task 5** (File I/O async) — simplu
6. **Task 6** (StoryDetailsMapper N+1) — mediu
7. **Task 4** (Paralelizare readers) — mediu (atenție la EF concurrency)
8. **Task 8** (Purchase flow) — opțional, impact mai mic

---

*Ghid generat pe baza analizei din PROBLEMS_SCAN_28.feb.2026.md*
