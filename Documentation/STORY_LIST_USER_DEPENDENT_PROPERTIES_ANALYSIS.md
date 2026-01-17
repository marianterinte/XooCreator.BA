# Analiză Proprietăți User-Dependent în Lista de Povești

## Scop
Identificarea tuturor proprietăților care sunt user-dependent în lista de povești și analiza de ce sunt necesare și de unde vin.

---

## 1. Unde se face cache pentru lista de povești

### Backend Cache
- **Loc**: `Features/TalesOfAlchimalia/Market/Caching/MarketplaceCatalogCache.cs`
- **Cache Keys**:
  - `marketplace:stories:base:{locale}` - datele de bază ale poveștilor (global, per locale)
  - `marketplace:stories:stats` - statistici globale (readersCount, averageRating, totalReviews)
- **TTL**: 
  - Base: 60 minute (configurabil)
  - Stats: 10 minute (configurabil)

### Repository care folosește cache-ul
- **Loc**: `Features/TalesOfAlchimalia/Market/StoriesMarketplaceRepository.cs`
- **Metodă**: `GetMarketplaceStoriesWithPaginationAsync`
- **Strategie**: 
  - Cache global pentru datele de bază (per locale)
  - Overlay per-user aplicat doar pe pagina curentă (batch query)

---

## 2. Proprietăți User-Dependent

### 2.1. Proprietăți de Ownership și Purchase

| Proprietate | Sursă DB | De unde vine | De ce ne trebuie | User-Dependent? |
|------------|----------|--------------|------------------|-----------------|
| **`isPurchased`** | `StoryPurchases` | Query per-user pe pagina curentă | Afișare buton "Play" vs "Purchase", determină dacă poate juca | ✅ DA |
| **`isOwned`** | `UserOwnedStories` + `StoryPurchases` | Query per-user pe pagina curentă | Determină dacă utilizatorul are acces gratuit (owned) sau trebuie să cumpere | ✅ DA |

**Detalii**:
- `isOwned = isPurchased || exists(UserOwnedStories)` (linia 188 în `StoriesMarketplaceRepository.cs`)
- Query-ul se face doar pentru story-urile din pagina curentă (batch query eficient)
- Folosit în UI pentru a determina dacă să afișeze buton "Play" sau "Purchase"

---

### 2.2. Proprietăți de Progress și Completion

| Proprietate | Sursă DB | De unde vine | De ce ne trebuie | User-Dependent? |
|------------|----------|--------------|------------------|-----------------|
| **`isCompleted`** | `UserStoryReadProgress` | Calculat din progresul user-ului | Afișare badge "Completed", filtrare, statistici | ✅ DA |
| **`progressPercentage`** | `UserStoryReadProgress` | Calculat: `(completedTiles / totalTiles) * 100` | Afișare progress bar, determină starea poveștii | ✅ DA |
| **`completedTiles`** | `UserStoryReadProgress` | Count din `UserStoryReadProgress` pentru story | Calcul progres, determină dacă e completă | ✅ DA |
| **`totalTiles`** | `StoryDefinition.Tiles` | Din definiția poveștii (global) | Calcul progres, afișare | ❌ NU (global) |
| **`lastReadTileId`** | `UserStoryReadProgress` | Ultimul tile citit de user | Continuare lectură de unde a rămas | ✅ DA |
| **`lastReadAt`** | `UserStoryReadProgress` | Data ultimei lecturi | Afișare "Last read", sortare | ✅ DA |

**Detalii**:
- Aceste proprietăți **NU** sunt în `StoryMarketplaceItemDto` (lista de marketplace)
- Sunt în `StoryDetailsDto` (detaliile unei povești specifice)
- Query-ul se face în `GetStoryDetailsAsync` (linia 496-566 în `StoriesMarketplaceRepository.cs`)
- Folosite în UI pentru:
  - Progress bar în story card
  - Badge "Completed"
  - Continuare lectură de unde a rămas
  - Filtrare după status (completed/in-progress/not-started)

**Notă**: În lista de marketplace, `completionStatus` este în request dar **NU se aplică** în repository (linia 27 din `MARKETPLACE_CACHING_ANALYSIS.md`)

---

### 2.3. Proprietăți de Like și Review

| Proprietate | Sursă DB | De unde vine | De ce ne trebuie | User-Dependent? |
|------------|----------|--------------|------------------|-----------------|
| **`isLiked`** | `StoryLikes` | Query per-user pentru story-ul curent | Afișare iconiță like plină/golă, toggle like | ✅ DA |
| **`likesCount`** | `StoryLikes` | Agregat global (din cache stats) | Afișare număr total de like-uri | ❌ NU (global) |
| **`userReview`** | `StoryReviews` | Query per-user pentru story-ul curent | Afișare review-ul user-ului, editare review | ✅ DA |
| **`averageRating`** | `StoryReviews` | Agregat global (din cache stats) | Afișare rating mediu | ❌ NU (global) |
| **`totalReviews`** | `StoryReviews` | Agregat global (din cache stats) | Afișare număr total de review-uri | ❌ NU (global) |

**Detalii**:
- `isLiked` și `userReview` sunt user-dependent
- `likesCount`, `averageRating`, `totalReviews` sunt agregate globale (din cache)
- Query-ul pentru `isLiked` se face în `GetStoryDetailsAsync` (linia 547-549)
- Query-ul pentru `userReview` se face în mapper-ul `StoryDetailsMapper`

---

### 2.4. Proprietăți de Filtrare User-Specific

| Proprietate | Sursă DB | De unde vine | De ce ne trebuie | User-Dependent? |
|------------|----------|--------------|------------------|-----------------|
| **`AutoFilterStoriesByAge`** | `AlchimaliaUsers` | Query per-user la începutul request-ului | Filtrare automată după age groups selectate de user | ✅ DA |
| **`SelectedAgeGroupIds`** | `AlchimaliaUsers` | Query per-user la începutul request-ului | Lista de age groups selectate pentru filtrare automată | ✅ DA |

**Detalii**:
- Acestea NU sunt proprietăți ale poveștii, ci preferințe ale user-ului
- Se citesc în `GetMarketplaceStoriesWithPaginationAsync` (linia 82-85)
- Se aplică ca filtru pe lista cache-uită (linia 116-120)
- **Impact major**: Lista finală de povești poate fi **diferită per user** chiar cu aceleași query params
- Folosite pentru filtrare automată din Parent Dashboard

---

### 2.5. Proprietăți de Unlocked Heroes

| Proprietate | Sursă DB | De unde vine | De ce ne trebuie | User-Dependent? |
|------------|----------|--------------|------------------|-----------------|
| **`unlockedStoryHeroes`** | Calculat din progresul user-ului | Din `UserStoryReadProgress` sau alte surse | Afișare eroi deblocați, badge-uri | ✅ DA |

**Detalii**:
- Apare în `StoryDetailsDto` dar nu în `StoryMarketplaceItemDto` (lista)
- Probabil calculat din progresul user-ului sau din alte surse de date

---

## 3. Proprietăți care vin de pe server (Backend → Frontend)

### 3.1. Din Cache Global (per locale)

Acestea sunt cache-uite și sunt aceleași pentru toți utilizatorii (per locale):

```typescript
// Din StoryMarketplaceBaseItem (cache)
id: string
title: string                    // Localizat per locale
coverImageUrl?: string
createdBy?: Guid
createdByName?: string
summary?: string                  // Localizat per locale
priceInCredits: number
ageRating: string
characters: string[]
tags: string[]                    // Topic IDs
createdAt: DateTime
storyTopic?: string
storyType: StoryType
status: StoryStatus
availableLanguages: string[]
isEvaluative: bool
ageGroupIds: string[]             // Pentru filtrare auto-age
```

### 3.2. Din Cache Stats (global)

```typescript
// Din StoryMarketplaceStats (cache)
readersCount: int                 // Agregat global
averageRating: double             // Agregat global
totalReviews: int                  // Agregat global
likesCount: int                   // Agregat global (query separat)
```

### 3.3. Din Query Per-User (overlay)

Acestea se calculează per request pentru user-ul curent:

```typescript
// Din query per-user (batch pentru pagina curentă)
isPurchased: bool                  // Din StoryPurchases
isOwned: bool                      // Din UserOwnedStories + StoryPurchases
isLiked: bool                      // Din StoryLikes (doar pentru story details)
userReview?: StoryReviewDto        // Din StoryReviews (doar pentru story details)
```

### 3.4. Din Query Per-User pentru Story Details

Acestea se calculează doar când se încarcă detaliile unei povești specifice:

```typescript
// Din GetStoryDetailsAsync
isCompleted: bool                 // Calculat din UserStoryReadProgress
progressPercentage: int            // Calculat din UserStoryReadProgress
completedTiles: int               // Count din UserStoryReadProgress
totalTiles: int                   // Din StoryDefinition.Tiles (global)
lastReadTileId?: string           // Din UserStoryReadProgress
lastReadAt?: DateTime            // Din UserStoryReadProgress
unlockedStoryHeroes: string[]     // Calculat din progres
```

---

## 4. Proprietăți care NU sunt în Backend DTO dar sunt în Frontend Interface

Următoarele proprietăți sunt în `StoryMarketplaceItem` (frontend) dar **NU** sunt trimise de backend:

```typescript
// În frontend dar NU în backend DTO
region: string                     // Calculat din storyId (frontend)
difficulty: string                 // Calculat din storyId (frontend)
isFeatured: boolean                // Nu există în backend
isNew: boolean                     // Nu există în backend
estimatedReadingTime: number       // Nu există în backend
```

**Notă**: Acestea sunt probabil calculate sau hardcodate în frontend.

---

## 5. Rezumat: Proprietăți User-Dependent

### În Lista de Marketplace (`StoryMarketplaceItemDto`)

| Proprietate | User-Dependent? | De ce |
|------------|----------------|-------|
| `isPurchased` | ✅ DA | Din `StoryPurchases` per user |
| `isOwned` | ✅ DA | Din `UserOwnedStories` + `StoryPurchases` per user |
| `AutoFilterStoriesByAge` | ✅ DA | Preferință user (filtrează lista) |
| `SelectedAgeGroupIds` | ✅ DA | Preferință user (filtrează lista) |

### În Story Details (`StoryDetailsDto`)

| Proprietate | User-Dependent? | De ce |
|------------|----------------|-------|
| `isPurchased` | ✅ DA | Din `StoryPurchases` per user |
| `isOwned` | ✅ DA | Din `UserOwnedStories` + `StoryPurchases` per user |
| `isCompleted` | ✅ DA | Calculat din `UserStoryReadProgress` |
| `progressPercentage` | ✅ DA | Calculat din `UserStoryReadProgress` |
| `completedTiles` | ✅ DA | Count din `UserStoryReadProgress` |
| `lastReadTileId` | ✅ DA | Din `UserStoryReadProgress` |
| `lastReadAt` | ✅ DA | Din `UserStoryReadProgress` |
| `isLiked` | ✅ DA | Din `StoryLikes` per user |
| `userReview` | ✅ DA | Din `StoryReviews` per user |
| `unlockedStoryHeroes` | ✅ DA | Calculat din progresul user-ului |

---

## 6. Impact asupra Cache-ului

### Cache Global (eficient)
- Datele de bază (title, summary, price, etc.) sunt cache-uite per locale
- Statisticile globale (readersCount, averageRating) sunt cache-uite
- Cache-ul nu explodează per-user

### Overlay Per-User (lightweight)
- Query-uri mici și eficiente doar pentru pagina curentă
- Batch queries pentru `isPurchased` și `isOwned`
- Preferințele user (`AutoFilterStoriesByAge`) se aplică pe lista cache-uită

### Story Details (per request)
- Query-uri separate pentru fiecare story detail
- Nu se cache-uiesc (sunt prea user-specific)
- Include toate proprietățile user-dependent

---

## 7. Recomandări

1. **Păstrați strategia actuală**:
   - Cache global pentru datele de bază
   - Overlay per-user lightweight pentru `isPurchased`/`isOwned`
   - Query separat pentru story details

2. **Optimizări posibile**:
   - Cache per-user pentru `isPurchased`/`isOwned` set-uri (TTL scurt, 5-10 min)
   - Cache per-user pentru preferințe (`AutoFilterStoriesByAge`, `SelectedAgeGroupIds`)
   - Considerați cache pentru `isLiked` dacă se folosește des în listă

3. **Proprietăți care NU ar trebui cache-uite per-user**:
   - `progressPercentage`, `isCompleted` - se schimbă prea des
   - `lastReadTileId`, `lastReadAt` - se schimbă la fiecare lectură
   - `userReview` - se schimbă când user-ul editează review-ul

---

## 8. Concluzie

**Proprietăți user-dependent în lista de povești**:
1. `isPurchased` - ✅ User-dependent, query per request
2. `isOwned` - ✅ User-dependent, query per request
3. `AutoFilterStoriesByAge` - ✅ User-dependent, filtrează lista
4. `SelectedAgeGroupIds` - ✅ User-dependent, filtrează lista

**Proprietăți user-dependent în story details**:
- Toate proprietățile de progress (`isCompleted`, `progressPercentage`, etc.)
- `isLiked`, `userReview`
- `unlockedStoryHeroes`

**Strategia actuală este optimă** pentru cache-ul global cu overlay per-user lightweight. Nu recomandăm cache per-user pentru proprietățile de progress (se schimbă prea des).
