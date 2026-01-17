# Eliminare `isPurchased` și `isOwned` din Lista de Marketplace

## Scop
Eliminăm `isPurchased` și `isOwned` din `StoryMarketplaceItemDto` (lista de marketplace) pentru a permite cache 100% global, fără query-uri per-user.

**Notă**: Epics au deja cache 100% global implementat - lista de epics nu are proprietăți user-dependent.

---

## Analiză: Unde se folosesc `isPurchased` și `isOwned`

### 1. **story-card.html** (lista de marketplace)
```html
[class.purchased]="story.isPurchased"
[class.to-buy]="!story.isPurchased && story.priceInCredits > 0"
```

**Folosit pentru:**
- ✅ CSS classes pentru styling vizual (border colors)
- ❌ NU se folosește pentru logică de business în listă

**Impact dacă eliminăm:**
- CSS classes `purchased` și `to-buy` nu vor mai funcționa
- Card-urile vor avea styling uniform (fără diferențiere vizuală)
- **NU este critic** - doar styling vizual

### 2. **story-card.ts** (computed properties)
```typescript
public readonly canPurchase = computed(() => {
  return !this.story.isPurchased && this.userCredits >= this.story.priceInCredits;
});

public readonly hasInsufficientCredits = computed(() => {
  return !this.story.isPurchased && this.userCredits < this.story.priceInCredits;
});
```

**Folosit pentru:**
- ❌ **NU se folosește în template-ul story-card.html**
- ✅ Se folosește DOAR în `story-details.html` (care are deja `isPurchased`/`isOwned`)

**Impact dacă eliminăm:**
- **NU are impact** - computed properties nu sunt folosite în listă

### 3. **story-search.html** (search results)
```html
<span class="suggestion-price" *ngIf="!suggestion.isPurchased">
  💎 {{ suggestion.priceInCredits }}
</span>
<span class="suggestion-status" *ngIf="suggestion.isPurchased">
  ✅ Purchased
</span>
```

**Folosit pentru:**
- Afișare preț condiționat în search results
- Afișare status "Purchased"

**Impact dacă eliminăm:**
- Search results vor trebui actualizate
- Poate afișa prețul pentru toate poveștile (sau să nu mai afișeze prețul deloc)
- **NU este critic** - search results nu sunt parte din lista principală de marketplace

### 4. **story-details.html** (detaliile poveștii)
```html
<div class="epic-note" *ngIf="isAlchimaliaEpic() && !storyDetails()!.isPurchased && !isAdmin()">
<div class="meta-item" *ngIf="!storyDetails()!.isPurchased && storyDetails()!.priceInCredits > 0">
<div class="progress-section" *ngIf="storyDetails()!.isPurchased">
```

**Folosit pentru:**
- Logică de business pentru butoane
- Afișare condiționată de secțiuni

**Impact dacă eliminăm:**
- **NU are impact** - story details are deja `isPurchased`/`isOwned` în `StoryDetailsDto`

---

## Concluzie: Putem elimina din listă

### ✅ Se poate elimina pentru că:
1. **NU se folosește pentru logică de business în listă** - doar pentru styling vizual
2. **Story details are deja** `isPurchased`/`isOwned` - nu se pierde funcționalitatea
3. **Cache 100% global** - fără query-uri per-user pentru listă
4. **Performance maximă** - lista se încarcă instant din cache

### ⚠️ Impact minim:
1. **CSS classes** `purchased` și `to-buy` nu vor mai funcționa în listă (doar styling vizual)
2. **Search results** vor trebui actualizate (sau să nu mai afișeze prețul condiționat)

---

## Cache pentru Epics (Deja Implementat ✅)

### Situația Actuală pentru Epics

Epics au **deja cache 100% global implementat** - lista de epics este complet cache-uită fără proprietăți user-dependent.

#### EpicMarketplaceItemDto (Lista)
```csharp
public record EpicMarketplaceItemDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public int StoryCount { get; init; }
    public int RegionCount { get; init; }
    public int ReadersCount { get; init; }        // ✅ Global (din cache stats)
    public double AverageRating { get; init; }     // ✅ Global (din cache stats)
    // ❌ NU are isPurchased/isOwned/isFavorite - deja optimizat!
}
```

#### EpicDetailsDto (Details)
```csharp
public record EpicDetailsDto
{
    // ... toate proprietățile globale din lista
    public EpicReviewDto? UserReview { get; init; }  // ✅ User-dependent (doar în details)
    // ❌ NU are isPurchased/isOwned - epics nu se cumpără
}
```

### Cache Implementation pentru Epics

#### Backend Cache
- **Loc**: `Features/TalesOfAlchimalia/Market/Caching/MarketplaceCatalogCache.cs`
- **Cache Keys**:
  - `marketplace:epics:base:{locale}` - datele de bază ale epic-urilor (global, per locale)
  - `marketplace:epics:stats` - statistici globale (readersCount, averageRating, totalReviews)
- **TTL**: 
  - Base: 12 ore (720 minute) - configurabil în `MarketplaceCacheOptions.BaseTtlMinutes`
  - Stats: 10 minute (configurabil)

#### Repository Implementation
```csharp
// În EpicsMarketplaceRepository.GetMarketplaceEpicsWithPaginationAsync
// ✅ Cache global - fără query-uri per-user
var baseItems = await _catalogCache.GetEpicsBaseAsync(normalizedLocale, CancellationToken.None);
var stats = await _catalogCache.GetEpicStatsAsync(CancellationToken.None);

// ✅ Filtrare și sortare în memorie (din cache)
IEnumerable<EpicMarketplaceBaseItem> q = baseItems;
// ... filtrare și sortare ...

// ✅ Mapping direct - fără query-uri per-user
var dtoList = pageItems.Select(epic => {
    stats.TryGetValue(epic.EpicId, out var st);
    return new EpicMarketplaceItemDto { /* ... */ };
}).ToList();
```

#### Epic Details (User-Dependent)
```csharp
// În GetEpicDetailsAsync - query per-user DOAR pentru UserReview
var userEpicReview = await _context.EpicReviews
    .Include(r => r.User)
    .FirstOrDefaultAsync(r => r.EpicId == epicId && r.UserId == userId && r.IsActive);
```

### Concluzie pentru Epics

✅ **Epics sunt deja optimizate**:
- Lista este 100% cache-uită (fără proprietăți user-dependent)
- Fără query-uri per-user pentru listă
- `UserReview` se încarcă doar în epic details (per request)
- Cache-ul funcționează perfect pentru epics

**Nu este nevoie de modificări pentru epics** - implementarea actuală este deja optimă.

---

## Plan de Implementare

### Backend: Eliminare din DTO
```csharp
// În StoryMarketplaceItemDto - ELIMINĂ:
public bool IsPurchased { get; init; } = false;  // ❌ ELIMINĂ
public bool IsOwned { get; init; } = false;      // ❌ ELIMINĂ

// Păstrăm în StoryDetailsDto (nu se schimbă):
public bool IsPurchased { get; init; }           // ✅ RĂMÂNE
public bool IsOwned { get; init; }              // ✅ RĂMÂNE
```

### Backend: Eliminare query-uri per-user
```csharp
// În StoriesMarketplaceRepository.GetMarketplaceStoriesWithPaginationAsync
// ELIMINĂ query-urile pentru isPurchased/isOwned:
// ❌ ELIMINĂ:
var purchasedIds = await _context.StoryPurchases
    .AsNoTracking()
    .Where(sp => sp.UserId == userId && pageStoryIds.Contains(sp.StoryId))
    .Select(sp => sp.StoryId)
    .ToListAsync();

var ownedDefIds = await _context.UserOwnedStories
    .AsNoTracking()
    .Where(uos => uos.UserId == userId && defIds.Contains(uos.StoryDefinitionId))
    .Select(uos => uos.StoryDefinitionId)
    .ToListAsync();

// ❌ ELIMINĂ din mapping:
IsPurchased = isPurchased,
IsOwned = isOwned,
```

### Frontend: Actualizare TypeScript Interface
```typescript
// În story-marketplace.types.ts
export interface StoryMarketplaceItem {
  // ... alte proprietăți
  // ❌ ELIMINĂ:
  // isPurchased: boolean;
  // isOwned: boolean;
}
```

### Frontend: Actualizare story-card.html
```html
<!-- ELIMINĂ CSS classes care depind de isPurchased -->
<!-- ❌ ELIMINĂ: -->
<!-- [class.purchased]="story.isPurchased" -->
<!-- [class.to-buy]="!story.isPurchased && story.priceInCredits > 0" -->
```

### Frontend: Actualizare story-card.ts
```typescript
// ELIMINĂ computed properties care nu sunt folosite în template
// (sau păstrează-le dacă vrei să le folosești în viitor, dar vor returna false)
```

### Frontend: Actualizare story-search.html
```html
<!-- Actualizare search results - poți afișa prețul pentru toate poveștile -->
<!-- SAU elimină condiția isPurchased -->
<span class="suggestion-price">
  💎 {{ suggestion.priceInCredits }}
</span>
<!-- ELIMINĂ: -->
<!-- <span class="suggestion-status" *ngIf="suggestion.isPurchased"> -->
```

---

## Beneficii

### ✅ Cache 100% Global
- Lista de povești se cache-uiește complet (fără query-uri per-user)
- Cache key: `marketplace:stories:base:{locale}`
- TTL: 12 ore (720 minute) - configurabil în `MarketplaceCacheOptions.BaseTtlMinutes`
- Motiv: Story-urile nu se publică des, deci lista nu se schimbă frecvent

### ✅ Performance Maximă
- Fără query-uri DB per-user pentru listă
- Lista se încarcă instant din cache
- Reducere semnificativă a load-ului pe DB

### ✅ Complexitate Minimă
- Fără overlay per-user
- Fără batch queries pentru isPurchased/isOwned
- Cod mai simplu și mai ușor de întreținut

### ✅ Funcționalitate Păstrată
- Story details are deja `isPurchased`/`isOwned`
- Logică de business rămâne în story details
- NU se pierde funcționalitatea critică

---

## Impact UX

### ⚠️ Impact Minim (doar styling vizual)
1. **Card-urile din listă** nu vor mai avea styling diferit pentru povești cumpărate
   - Border color uniform (fără `purchased` class)
   - Background uniform (fără gradient pentru `purchased`)

2. **Search results** nu vor mai afișa "Purchased" status
   - Poate afișa prețul pentru toate poveștile
   - SAU să nu mai afișeze prețul deloc

### ✅ Funcționalitate Păstrată
- User-ul poate vedea `isPurchased`/`isOwned` în story details
- Logică de business (`canPurchase`, butoane) funcționează în story details
- NU se pierde funcționalitatea critică

---

## Recomandare Finală

**✅ ELIMINĂM `isPurchased` și `isOwned` din listă**

**Motive:**
1. Cache 100% global - performance maximă
2. Impact minim - doar styling vizual
3. Funcționalitate păstrată - story details are deja proprietățile
4. Complexitate minimă - fără query-uri per-user

**Pași:**
1. Backend: Elimină din `StoryMarketplaceItemDto`
2. Backend: Elimină query-urile per-user pentru isPurchased/isOwned
3. Frontend: Actualizează TypeScript interface
4. Frontend: Elimină CSS classes din story-card.html
5. Frontend: Actualizează story-search.html (opțional)
