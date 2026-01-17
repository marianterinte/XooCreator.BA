# Analiză Cache pentru Epics Marketplace

## Scop
Analiza implementării cache-ului pentru lista de epics și compararea cu strategia pentru stories.

---

## 1. Situația Actuală pentru Epics

### Cache Implementation ✅

Epics au **deja cache 100% global implementat** - lista de epics este complet cache-uită fără proprietăți user-dependent.

#### Backend Cache
- **Loc**: `Features/TalesOfAlchimalia/Market/Caching/MarketplaceCatalogCache.cs`
- **Cache Keys**:
  - `marketplace:epics:base:{locale}` - datele de bază ale epic-urilor (global, per locale)
  - `marketplace:epics:stats` - statistici globale (readersCount, averageRating, totalReviews)
- **TTL**: 
  - Base: 60 minute (configurabil în `MarketplaceCacheOptions.BaseTtlMinutes`)
  - Stats: 10 minute (configurabil în `MarketplaceCacheOptions.StatsTtlMinutes`)

#### Repository Implementation
- **Loc**: `Features/TalesOfAlchimalia/Market/Repositories/EpicsMarketplaceRepository.cs`
- **Metodă**: `GetMarketplaceEpicsWithPaginationAsync`
- **Strategie**: Cache global complet - fără query-uri per-user pentru listă

---

## 2. Proprietăți în EpicMarketplaceItemDto (Lista)

### Proprietăți Global (Cache-uite)

| Proprietate | Sursă | Cache? | De ce |
|------------|-------|--------|-------|
| `id` | `StoryEpicDefinition.Id` | ✅ DA | Global |
| `name` | `StoryEpicDefinition.Translations` (localizat) | ✅ DA | Global, per locale |
| `description` | `StoryEpicDefinition.Translations` (localizat) | ✅ DA | Global, per locale |
| `coverImageUrl` | `StoryEpicDefinition.CoverImageUrl` | ✅ DA | Global |
| `createdBy` | `StoryEpicDefinition.OwnerUserId` | ✅ DA | Global |
| `createdByName` | `StoryEpicDefinition.Owner.Name/Email` | ✅ DA | Global |
| `createdAt` | `StoryEpicDefinition.CreatedAt` | ✅ DA | Global |
| `publishedAtUtc` | `StoryEpicDefinition.PublishedAtUtc` | ✅ DA | Global |
| `storyCount` | `StoryEpicDefinition.StoryNodes.Count` | ✅ DA | Global |
| `regionCount` | `StoryEpicDefinition.Regions.Count` | ✅ DA | Global |
| `readersCount` | `EpicReaders` (agregat) | ✅ DA | Global (din cache stats) |
| `averageRating` | `EpicReviews` (agregat) | ✅ DA | Global (din cache stats) |

### Proprietăți User-Dependent

| Proprietate | În Listă? | În Details? | De ce |
|------------|----------|-------------|-------|
| `isPurchased` | ❌ NU | ❌ NU | Epics nu se cumpără |
| `isOwned` | ❌ NU | ❌ NU | Epics nu se dețin |
| `isFavorite` | ❌ NU | ✅ DA (separat) | Doar în details (query separat) |
| `userReview` | ❌ NU | ✅ DA | Doar în details (query per request) |

**Concluzie**: Lista de epics **NU are proprietăți user-dependent** - este 100% globală.

---

## 3. Comparație: Stories vs Epics

### Stories (Înainte de Optimizare)

| Aspect | Stories (Actual) | Epics (Actual) |
|--------|------------------|----------------|
| **Cache Global** | ✅ DA | ✅ DA |
| **Proprietăți User-Dependent în Listă** | ✅ DA (`isPurchased`, `isOwned`) | ❌ NU |
| **Query-uri Per-User pentru Listă** | ✅ DA (batch query) | ❌ NU |
| **Cache 100% Global** | ❌ NU | ✅ DA |

### Stories (După Optimizare - Eliminare `isPurchased`/`isOwned`)

| Aspect | Stories (După) | Epics (Actual) |
|--------|----------------|----------------|
| **Cache Global** | ✅ DA | ✅ DA |
| **Proprietăți User-Dependent în Listă** | ❌ NU | ❌ NU |
| **Query-uri Per-User pentru Listă** | ❌ NU | ❌ NU |
| **Cache 100% Global** | ✅ DA | ✅ DA |

**Concluzie**: După eliminarea `isPurchased`/`isOwned` din stories, ambele (stories și epics) vor avea cache 100% global.

---

## 4. Implementare Cache pentru Epics

### 4.1. Cache Base (per locale)

```csharp
// Cache key: marketplace:epics:base:{locale}
// TTL: 60 minute (configurabil)
// Conține: toate proprietățile globale (fără user-dependent)

private async Task<IReadOnlyList<EpicMarketplaceBaseItem>> LoadEpicsBaseAsync(string locale, CancellationToken ct)
{
    var epics = await db.StoryEpicDefinitions
        .AsNoTracking()
        .Include(e => e.Owner)
        .Include(e => e.StoryNodes)
        .Include(e => e.Regions)
        .Include(e => e.Translations)
        .Where(e => e.Status == "published" && e.PublishedAtUtc != null)
        .AsSplitQuery()
        .ToListAsync(ct);

    // Mapping cu localizare per locale
    // ...
}
```

### 4.2. Cache Stats (global)

```csharp
// Cache key: marketplace:epics:stats
// TTL: 10 minute (configurabil)
// Conține: readersCount, averageRating, totalReviews per epic

private async Task<IReadOnlyDictionary<string, EpicMarketplaceStats>> LoadEpicStatsAsync(CancellationToken ct)
{
    var readers = await db.EpicReaders
        .AsNoTracking()
        .GroupBy(er => er.EpicId)
        .Select(g => new { EpicId = g.Key, ReadersCount = g.Count() })
        .ToListAsync(ct);

    var reviews = await db.EpicReviews
        .AsNoTracking()
        .Where(r => r.IsActive)
        .GroupBy(r => r.EpicId)
        .Select(g => new {
            EpicId = g.Key,
            AverageRating = g.Average(x => (double)x.Rating),
            TotalReviews = g.Count()
        })
        .ToListAsync(ct);

    // Mapping stats
    // ...
}
```

### 4.3. Repository Usage

```csharp
public async Task<(List<EpicMarketplaceItemDto> Epics, int TotalCount, bool HasMore)> 
    GetMarketplaceEpicsWithPaginationAsync(Guid userId, string locale, SearchEpicsRequest request)
{
    // ✅ Cache global - fără query-uri per-user
    var baseItems = await _catalogCache.GetEpicsBaseAsync(normalizedLocale, CancellationToken.None);
    var stats = await _catalogCache.GetEpicStatsAsync(CancellationToken.None);

    // ✅ Filtrare și sortare în memorie (din cache)
    IEnumerable<EpicMarketplaceBaseItem> q = baseItems;
    
    // Search filter
    if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
        q = q.Where(e => e.SearchTexts.Any(t => 
            t.Contains(search, StringComparison.OrdinalIgnoreCase)));
    }

    // Sorting
    q = sortBy switch {
        "name" => sortDesc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),
        "readers" => sortDesc ? q.OrderByDescending(e => stats.TryGetValue(e.EpicId, out var st) ? st.ReadersCount : 0) : ...,
        "rating" => sortDesc ? q.OrderByDescending(e => stats.TryGetValue(e.EpicId, out var st) ? st.AverageRating : 0.0) : ...,
        _ => sortDesc ? q.OrderByDescending(e => e.PublishedAtUtc) : q.OrderBy(e => e.PublishedAtUtc)
    };

    // ✅ Paginare în memorie
    var filtered = q.ToList();
    var pageItems = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    // ✅ Mapping direct - fără query-uri per-user
    var dtoList = pageItems.Select(epic => {
        stats.TryGetValue(epic.EpicId, out var st);
        return new EpicMarketplaceItemDto {
            // ... toate proprietățile globale
            ReadersCount = st.ReadersCount,
            AverageRating = st.AverageRating
        };
    }).ToList();

    return (dtoList, totalCount, hasMore);
}
```

---

## 5. Epic Details (User-Dependent)

### Proprietăți User-Dependent în EpicDetailsDto

| Proprietate | Sursă DB | De unde vine | De ce ne trebuie |
|------------|----------|--------------|------------------|
| **`userReview`** | `EpicReviews` | Query per-user pentru epic-ul curent | Afișare review-ul user-ului, editare review |

### Implementation

```csharp
public async Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale)
{
    // Query epic-ul din DB (nu din cache - details se încarcă per request)
    var epic = await _context.StoryEpicDefinitions
        .Include(e => e.Owner)
        .Include(e => e.StoryNodes)
        .Include(e => e.Regions)
        .Include(e => e.Translations)
        .FirstOrDefaultAsync(e => e.Id == epicId && e.Status == "published");

    // ✅ Query per-user DOAR pentru UserReview
    var userEpicReview = await _context.EpicReviews
        .Include(r => r.User)
        .FirstOrDefaultAsync(r => r.EpicId == epicId && r.UserId == userId && r.IsActive);

    // Mapping cu UserReview
    return new EpicDetailsDto {
        // ... toate proprietățile globale
        UserReview = userReview  // ✅ User-dependent
    };
}
```

**Notă**: Epic details nu se cache-uiește (prea user-specific pentru `UserReview`).

---

## 6. Invalidare Cache pentru Epics

### Când se invalidează cache-ul

1. **După publish epic**:
   - Cache-ul base (`marketplace:epics:base:{locale}`) se invalidează
   - Cache-ul stats (`marketplace:epics:stats`) se invalidează

2. **După update epic** (dacă e publicat):
   - Cache-ul base se invalidează
   - Cache-ul stats rămâne (stats nu se schimbă la update)

3. **După review epic** (dacă vrem fresh stats):
   - Cache-ul stats se invalidează (TTL scurt - 10 minute)
   - Cache-ul base rămâne

### Implementation

```csharp
// În EpicPublishQueueJob sau după publish
public void ResetAll()
{
    foreach (var locale in _knownEpicLocales.Keys)
    {
        _cache.Remove(GetEpicsBaseKey(locale));
    }
    _cache.Remove(EpicStatsKey);
}
```

---

## 7. Beneficii Cache pentru Epics

### ✅ Performance Maximă
- Lista de epics se încarcă instant din cache
- Fără query-uri DB per request pentru listă
- Reducere semnificativă a load-ului pe DB

### ✅ Scalabilitate
- Cache-ul poate fi distribuit (Redis) în viitor
- Fără query-uri per-user = mai puține conexiuni DB

### ✅ Complexitate Minimă
- Fără overlay per-user
- Fără batch queries
- Cod simplu și ușor de întreținut

### ✅ UX Păstrată
- Epic details are `UserReview` când e nevoie
- Lista este rapidă și responsive

---

## 8. Concluzie

### ✅ Epics sunt deja optimizate

**Situația actuală pentru epics:**
- ✅ Cache 100% global implementat
- ✅ Fără proprietăți user-dependent în listă
- ✅ Fără query-uri per-user pentru listă
- ✅ Performance maximă

**Nu este nevoie de modificări pentru epics** - implementarea actuală este deja optimă și servește ca model pentru optimizarea stories.

### Comparație Finală

| Aspect | Stories (După Optimizare) | Epics (Actual) |
|--------|---------------------------|----------------|
| **Cache 100% Global** | ✅ DA | ✅ DA |
| **Proprietăți User-Dependent în Listă** | ❌ NU | ❌ NU |
| **Query-uri Per-User pentru Listă** | ❌ NU | ❌ NU |
| **User-Dependent în Details** | ✅ DA | ✅ DA (`UserReview`) |
| **Status** | 🔄 De optimizat | ✅ Deja optimizat |

**Recomandare**: După eliminarea `isPurchased`/`isOwned` din stories, ambele (stories și epics) vor avea aceeași strategie de cache optimă.
