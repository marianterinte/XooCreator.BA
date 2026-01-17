# Discuție: Strategie Cache pentru Lista de Povești

## Scop
Discutăm ce proprietăți user-dependent ar trebui să fie eliminate din lista de marketplace și să rămână doar în story details, pentru a permite cache-ul complet al listei.

---

## Situația Actuală

### Ce avem acum în `StoryMarketplaceItemDto` (lista):
```csharp
public bool IsPurchased { get; init; } = false;  // ❌ User-dependent
public bool IsOwned { get; init; } = false;      // ❌ User-dependent
```

### Ce avem în `StoryDetailsDto` (details):
```csharp
public bool IsPurchased { get; init; }           // ✅ User-dependent
public bool IsOwned { get; init; }              // ✅ User-dependent
public bool IsCompleted { get; init; }         // ✅ User-dependent
public int ProgressPercentage { get; init; }    // ✅ User-dependent
// ... toate celelalte proprietăți user-specific
```

---

## Unde se folosesc `isPurchased` și `isOwned` în UI

### 1. **story-card.html** (card-urile din listă)
```html
<!-- CSS classes pentru styling -->
[class.purchased]="story.isPurchased"
[class.to-buy]="!story.isPurchased && story.priceInCredits > 0"
```

**Folosit pentru:**
- Styling vizual (probabil schimbă culoarea card-ului sau afișează badge "Purchased")
- Diferențiere vizuală între povești cumpărate vs necumpărate

### 2. **story-card.ts** (logică de business)
```typescript
public readonly canPurchase = computed(() => {
  return !this.story.isPurchased && this.userCredits >= this.story.priceInCredits;
});

public readonly hasInsufficientCredits = computed(() => {
  return !this.story.isPurchased && this.userCredits < this.story.priceInCredits;
});
```

**Folosit pentru:**
- Determină dacă să afișeze buton "Purchase"
- Determină dacă să afișeze mesaj "Insufficient Credits"

### 3. **story-search.html** (căutare)
```html
<span class="suggestion-price" *ngIf="!suggestion.isPurchased">
  💎 {{ suggestion.priceInCredits }}
</span>
<span class="suggestion-status" *ngIf="suggestion.isPurchased">
  ✅ Purchased
</span>
<span class="suggestion-progress" *ngIf="suggestion.isPurchased && suggestion.progressPercentage > 0">
  {{ suggestion.progressPercentage }}% complete
</span>
```

**Folosit pentru:**
- Afișare preț în rezultatele de căutare
- Afișare status "Purchased"
- Afișare progress (dar `progressPercentage` nu e în lista de marketplace, deci probabil e 0 sau undefined)

### 4. **story-details.html** (detaliile poveștii)
```html
<div class="epic-note" *ngIf="isAlchimaliaEpic() && !storyDetails()!.isPurchased && !isAdmin()">
<div class="meta-item" *ngIf="!storyDetails()!.isPurchased && storyDetails()!.priceInCredits > 0">
<div class="progress-section" *ngIf="storyDetails()!.isPurchased">
```

**Folosit pentru:**
- Afișare condiționată de secțiuni
- Logică de business pentru butoane

---

## Analiză: Ce ar trebui să fie în listă vs details?

### Opțiunea 1: Eliminăm `isPurchased` și `isOwned` din listă ❌

**Avantaje:**
- ✅ Lista poate fi cache-uită complet (100% global)
- ✅ Fără query-uri per-user pentru listă
- ✅ Cache-ul devine mult mai simplu și mai eficient

**Dezavantaje:**
- ❌ Nu mai putem face styling diferit pentru povești cumpărate în listă
- ❌ Nu mai putem afișa "Purchased" badge în card-uri
- ❌ Nu mai putem afișa prețul condiționat în search results
- ❌ UX mai slab: user-ul nu vede imediat ce povești are deja cumpărate

**Impact UX:**
- User-ul trebuie să intre pe fiecare story detail pentru a vedea dacă e cumpărat
- Nu mai poate vedea rapid în listă ce povești are deja
- Styling-ul card-urilor devine uniform (nu mai distingem vizual cumpărate vs necumpărate)

---

### Opțiunea 2: Păstrăm `isPurchased` și `isOwned` în listă ✅ (Recomandat)

**Avantaje:**
- ✅ UX mai bun: user-ul vede imediat ce povești are cumpărate
- ✅ Styling diferit pentru povești cumpărate (badge-uri, culori)
- ✅ Logică de business în listă (butoane, prețuri condiționate)
- ✅ Search results mai informative

**Dezavantaje:**
- ❌ Trebuie să facem query per-user pentru lista curentă (pagina curentă)
- ❌ Cache-ul nu poate fi 100% global

**Impact Performance:**
- Query-ul per-user este **lightweight** (doar pentru pagina curentă, batch query)
- Deja implementat eficient în `GetMarketplaceStoriesWithPaginationAsync`:
  ```csharp
  // Query doar pentru pagina curentă (ex: 20 items)
  var pageStoryIds = pageItems.Select(p => p.StoryId).ToList();
  var purchasedIds = await _context.StoryPurchases
      .AsNoTracking()
      .Where(sp => sp.UserId == userId && pageStoryIds.Contains(sp.StoryId))
      .Select(sp => sp.StoryId)
      .ToListAsync();
  ```
- Overhead minim: 1 query mic per request (doar pentru pagina curentă)

---

### Opțiunea 3: Hybrid - Cache per-user pentru `isPurchased`/`isOwned` set-uri

**Idee:**
- Cache global pentru lista de povești (fără `isPurchased`/`isOwned`)
- Cache per-user pentru set-uri de story IDs: `purchasedStoryIds`, `ownedStoryIds`
- TTL scurt pentru cache-ul per-user (5-10 minute)
- Overlay în memorie: combinăm lista globală cu set-urile per-user

**Avantaje:**
- ✅ Lista globală rămâne cache-uită complet
- ✅ Query-ul per-user se face mai rar (doar când expiră cache-ul per-user)
- ✅ UX păstrează toate funcționalitățile

**Dezavantaje:**
- ❌ Complexitate mai mare (2 straturi de cache)
- ❌ Cache per-user poate crește dacă ai mulți useri activi simultan
- ❌ Invalidare mai complexă (trebuie să invalidezi cache-ul per-user când user-ul cumpără o poveste)

**Implementare:**
```csharp
// Cache key per-user
var purchasedKey = $"user:{userId}:purchased:stories";
var ownedKey = $"user:{userId}:owned:stories";

// TTL: 5-10 minute
var purchasedIds = await _cache.GetOrCreateAsync(purchasedKey, async entry => {
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    return await LoadPurchasedStoryIdsAsync(userId);
});
```

---

## Recomandare: Opțiunea 2 (Păstrăm în listă)

### De ce?

1. **UX este critic**: User-ul trebuie să vadă imediat ce povești are cumpărate în listă
2. **Performance este deja bună**: Query-ul per-user este lightweight (doar pentru pagina curentă)
3. **Complexitate minimă**: Strategia actuală este simplă și funcționează bine
4. **Cache-ul global rămâne eficient**: Majoritatea datelor (title, summary, price, etc.) sunt cache-uite

### Ce putem optimiza?

1. **Cache per-user pentru set-uri** (Opțiunea 3) - dacă vrem să reducem și mai mult query-urile
2. **Invalidare cache per-user** când user-ul cumpără o poveste (pentru Opțiunea 3)
3. **Batch query-uri** pentru mai mulți useri simultan (dacă e nevoie)

---

## Proprietăți care NU ar trebui să fie în listă

### ✅ Deja corect implementat (nu sunt în listă):

| Proprietate | De ce nu e în listă | Unde e folosită |
|------------|---------------------|-----------------|
| `isCompleted` | Se schimbă prea des (la fiecare citire) | Story details |
| `progressPercentage` | Se schimbă prea des | Story details |
| `completedTiles` | Se schimbă prea des | Story details |
| `lastReadTileId` | Se schimbă la fiecare lectură | Story details |
| `lastReadAt` | Se schimbă la fiecare lectură | Story details |
| `isLiked` | User-specific, nu e critic în listă | Story details |
| `userReview` | User-specific, nu e critic în listă | Story details |

**Concluzie**: Acestea sunt corect implementate - rămân doar în story details.

---

## Proprietăți care AR TREBUI să fie în listă

### ✅ Corect implementat (sunt în listă):

| Proprietate | De ce e în listă | Impact dacă eliminăm |
|------------|------------------|---------------------|
| `isPurchased` | UX critic - user-ul trebuie să vadă ce are cumpărat | ❌ UX mai slab |
| `isOwned` | UX critic - determină dacă poate juca | ❌ Logică de business afectată |

**Concluzie**: Acestea ar trebui să rămână în listă pentru UX și logică de business.

---

## Strategia Recomandată

### 1. Cache Global (per locale)
```csharp
// Cache key: marketplace:stories:base:{locale}
// TTL: 60 minute
// Conține: toate proprietățile GLOBAL (fără isPurchased/isOwned)
```

### 2. Overlay Per-User (lightweight)
```csharp
// Query doar pentru pagina curentă (ex: 20 items)
// Batch query pentru isPurchased/isOwned
// Aplicat în memorie peste lista cache-uită
```

### 3. Story Details (per request)
```csharp
// Query separat pentru fiecare story detail
// Include toate proprietățile user-dependent
// Nu se cache-uiește (prea user-specific)
```

---

## Optimizări Posibile (Opțiunea 3 - Hybrid)

Dacă vrem să reducem și mai mult query-urile per-user:

### Cache Per-User pentru Set-uri
```csharp
// Cache key: user:{userId}:purchased:stories
// TTL: 5-10 minute
// Conține: HashSet<string> de story IDs cumpărate

// Cache key: user:{userId}:owned:stories  
// TTL: 5-10 minute
// Conține: HashSet<Guid> de story definition IDs owned
```

### Invalidare Cache Per-User
```csharp
// Când user-ul cumpără o poveste:
await _cache.RemoveAsync($"user:{userId}:purchased:stories");
await _cache.RemoveAsync($"user:{userId}:owned:stories");

// Sau update incremental (mai eficient):
var purchasedSet = await GetPurchasedStoriesSetAsync(userId);
purchasedSet.Add(storyId);
await _cache.SetAsync($"user:{userId}:purchased:stories", purchasedSet, ttl);
```

### Avantaje Opțiunea 3:
- ✅ Reducere query-uri DB (doar când expiră cache-ul per-user)
- ✅ Performance mai bună pentru useri care navighează mult
- ✅ UX păstrează toate funcționalitățile

### Dezavantaje Opțiunea 3:
- ❌ Complexitate mai mare
- ❌ Memorie mai multă (cache per-user)
- ❌ Invalidare mai complexă

---

## Concluzie Finală

### Recomandare: **Opțiunea 1** (Eliminăm `isPurchased`/`isOwned` din listă) ✅

**Motive:**
1. ✅ Cache 100% global - fără query-uri per-user pentru listă
2. ✅ Performance maximă - lista se încarcă instant din cache
3. ✅ Complexitate minimă - fără overlay per-user
4. ✅ `isPurchased`/`isOwned` se folosesc DOAR pentru styling în listă (CSS classes), nu pentru logică de business
5. ✅ Logică de business (`canPurchase`, `hasInsufficientCredits`) se folosește DOAR în story details, nu în listă
6. ✅ Story details are deja `isPurchased`/`isOwned` - nu se pierde funcționalitatea

**Impact minim:**
- CSS classes `purchased` și `to-buy` nu vor mai funcționa în listă (doar styling vizual, nu critic)
- Search results vor trebui actualizate (sau să nu mai afișeze prețul condiționat)
- Story details rămâne neschimbat (are deja toate proprietățile user-dependent)

### Optimizare Opțională: **Opțiunea 3** (Cache per-user pentru set-uri)

**Când să implementăm:**
- Dacă observăm că query-urile per-user devin bottleneck
- Dacă avem mulți useri activi simultan care navighează mult
- Dacă vrem să reducem și mai mult load-ul pe DB

**Când NU să implementăm:**
- Dacă performance-ul actual este suficient
- Dacă vrem să păstrăm complexitatea minimă
- Dacă nu avem probleme cu query-urile per-user

---

## Întrebări pentru Discuție

1. **UX**: Este critic ca user-ul să vadă `isPurchased`/`isOwned` în listă?
   - Răspuns: **DA** - este foarte important pentru UX

2. **Performance**: Query-urile per-user sunt bottleneck?
   - Răspuns: **NU** - sunt lightweight și eficiente

3. **Complexitate**: Vrem să adăugăm complexitate pentru optimizări minore?
   - Răspuns: **NU** - strategia actuală este suficientă

4. **Cache per-user**: Merită complexitatea pentru cache per-user?
   - Răspuns: **DEPINDE** - doar dacă avem probleme reale de performance

---

## Recomandare Finală

### Stories: Eliminăm `isPurchased` și `isOwned` din listă ✅

**Strategia optimă pentru stories:**
- Cache 100% global pentru lista de stories (fără `isPurchased`/`isOwned`)
- `isPurchased`/`isOwned` rămân doar în story details
- Fără query-uri per-user pentru listă

### Epics: Deja Optimizate ✅

**Situația actuală pentru epics:**
- Cache 100% global implementat deja
- Lista de epics nu are proprietăți user-dependent
- `UserReview` se încarcă doar în epic details
- **Nu este nevoie de modificări** - servește ca model pentru stories

**Vezi**: `EPICS_CACHE_ANALYSIS.md` pentru detalii complete despre cache-ul pentru epics.
