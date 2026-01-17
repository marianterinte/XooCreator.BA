# Marketplace caching analysis (`/tales-of-alchimalia`)

## Scop
Să stabilim **cât de user-specific** este marketplace-ul (`/tales-of-alchimalia/market` pentru stories și `/tales-of-alchimalia/market/epics` pentru epics), ce date sunt **vitale** pentru UI și ce strategie de cache are sens fără breaking changes.

> Concluzia “de 30s”: **Stories marketplace list este user-dependent** (cel puțin prin `IsPurchased` / `IsOwned` și prin auto-age-filter din setările user-ului). **Epics marketplace list este preponderent global** (lista nu include câmpuri per-user; doar epic details include `UserReview`).

---

## 1) Ce endpoint-uri există și ce întorc

### Stories list
- **Endpoint**: `GET /api/{locale}/tales-of-alchimalia/market`
- **Handler**: `Features/TalesOfAlchimalia/Market/Endpoints/GetMarketplaceStoriesEndpoint.cs`
- **Service**: `Features/TalesOfAlchimalia/Market/StoriesMarketplaceService.cs`
- **Repository**: `Features/TalesOfAlchimalia/Market/StoriesMarketplaceRepository.cs`
- **Response model**: `GetMarketplaceStoriesResponse` → `List<StoryMarketplaceItemDto>` (în `DTOs/StoriesMarketplaceDtos.cs`)

**Câmpuri în `StoryMarketplaceItemDto` (backend):**
- `id`, `title`, `coverImageUrl`, `createdBy`, `createdByName`, `summary`, `priceInCredits`
- `ageRating`, `characters`, `tags` (topic IDs), `createdAt`
- `storyTopic`, `storyType`, `status`, `availableLanguages`
- `isPurchased`, `isOwned` (**per-user**)
- `readersCount`, `averageRating`, `totalReviews` (**global aggregates**)
- `isEvaluative`

**Important**: request-ul include și `completionStatus`, dar în implementarea actuală din repository/service **nu se aplică** (nu există filtrare per user pe progres în list).

### Epics list
- **Endpoint**: `GET /api/{locale}/tales-of-alchimalia/market/epics`
- **Handler**: `Features/TalesOfAlchimalia/Market/Endpoints/GetMarketplaceEpicsEndpoint.cs`
- **Service**: `Features/TalesOfAlchimalia/Market/Services/EpicsMarketplaceService.cs`
- **Repository**: `Features/TalesOfAlchimalia/Market/Repositories/EpicsMarketplaceRepository.cs`
- **Response model**: `GetMarketplaceEpicsResponse` → `List<EpicMarketplaceItemDto>`

**Câmpuri în `EpicMarketplaceItemDto` (backend):**
- `id`, `name`, `description`, `coverImageUrl`
- `createdBy`, `createdByName`, `createdAt`, `publishedAtUtc`
- `storyCount`, `regionCount`
- `readersCount`, `averageRating`

### Detalii (relevante pentru “user-specific”)
- **Story details**: `GetStoryDetailsAsync` calculează progresul userului (`UserStoryReadProgress`), `IsPurchased/IsOwned`, plus statistici review/readers.
- **Epic details**: include `UserReview` (review-ul userului curent), deci **detaliile sunt user-dependent** chiar dacă lista este mai “globală”.

---

## 2) Ce este user-specific vs global (azi, în cod)

### Stories list (user-dependent)
În `StoriesMarketplaceRepository.GetMarketplaceStoriesWithPaginationAsync`:
- Se citește user-ul (`AlchimaliaUsers`) pentru:
  - `AutoFilterStoriesByAge`
  - `SelectedAgeGroupIds`
  => deci **lista poate fi diferită per user** chiar cu aceeași cerere (aceleași query params).

În `MapToMarketplaceItemFromDefinitionAsync` (per story):
- `IsPurchased` = există în `StoryPurchases` pentru `userId`
- `IsOwned` = există în `UserOwnedStories` pentru `userId`

### Stories list (global / agregate)
În `MapToMarketplaceListAsync`:
- `ReadersCount` agregat din `StoryReaders` (global)
- `AverageRating/TotalReviews` agregate din `StoryReviews` (global)
În plus:
- `Summary` este citit din fișier JSON (seed/i18n) în funcție de locale (`GetSummaryFromJson`) – global, dar **I/O pe disk**.
- `CreatedByName` este încărcat din DB (global), dar prin query per item.

### Epics list (în mare global)
În `EpicsMarketplaceRepository.GetMarketplaceEpicsWithPaginationAsync`:
- Nu există `isOwned/isPurchased/isFavorite` în DTO-ul de list; `userId` este primit dar lista nu pare să depindă de el.
- `readersCount` și `averageRating` sunt agregate globale (din `EpicReaders`, `EpicReviews`).

### Concluzie pe “cât de user specific e”
- **Stories list**: user-specific (minim `IsPurchased/IsOwned` + auto-age-filter).
- **Epics list**: în practică global (lista), dar **epic details** e user-specific (`UserReview`).

---

## 3) Ce pare “vital” pentru FE (fără să rupem ceva)

Frontend-ul folosește `TalesMarketplaceService.getMarketplaceStories()` și pune `response.stories` direct în state (`tales-of-alchimalia.ts`), fără mapping suplimentar în componentă.

În UI, se folosesc multe câmpuri din `StoryMarketplaceItem` (TS interface), dar backend-ul **nu trimite** o parte din ele (ex: `region`, `difficulty`, `isFeatured`, `isNew`, `estimatedReadingTime`, `progressPercentage`, `isCompleted`, etc).

Observații:
- Filtrele de UI pentru regions/age/characters apar în component, dar în template se pasează `availableRegions=[]` / `availableAgeRatings=[]` / `availableCharacters=[]` (deci în practică nu sunt active ca listă de opțiuni).
- Totuși, request-ul către backend trimite `regions`, `ageRatings`, `characters`, `categories`, `difficulties`, `topics`.

**Pentru caching, partea vitală (azi, pe wire) pare să fie** tot ce vine din backend DTO-ul real:
- `id`, `title`, `coverImageUrl`, `summary`, `priceInCredits`
- `availableLanguages`, `tags`, `characters`, `ageRating`, `isEvaluative`
- `isPurchased`, `isOwned`
- `readersCount`, `averageRating`, `totalReviews`
- `createdByName` (pentru afișare autor)

---

## 4) Strategia propusă de cache: ce e bun, ce trebuie ajustat

### Ideea ta: cache “2 colecții” (stories + epics) și paginare în memorie
E o direcție bună pentru B1 (CPU mic, DB roundtrips scumpe), dar trebuie să ținem cont de:
- **User-specific**: pentru stories, `IsPurchased/IsOwned` și auto-age-filter fac ca “lista finală” să fie diferită per user.
- **Locale**: title/summary/description sunt locale-dependent → cache key per locale (cel puțin).
- **Sort/filters**: dacă facem totul in-memory, trebuie să reproducem aceleași reguli ca DB (sau să acceptăm diferențe). Azi, multe filtre sunt aplicate în DB (search `ILike`, topics, isEvaluative, status/storyType implicit, exclude `IsPartOfEpic`).

### Recomandare (fără breaking changes): cache în 2 straturi
1) **Global “base data” cache (per locale)**:
   - pentru stories: lista de “published + active + !IsPartOfEpic (+ storyType implicit Indie dacă nu e cerut altceva)” cu câmpuri global/stabile:
     - `id`, `title(locale)`, `summary(locale)`, `coverImageUrl`, `priceInCredits`, `tags`, `characters`, `ageRating`, `availableLanguages`, `isEvaluative`, `createdBy`, `createdByName`, `createdAt`
     - agregate: `readersCount`, `averageRating`, `totalReviews`
     - (opțional, pentru a susține auto-age-filter fără DB) lista de `ageGroupIds` per story
   - pentru epics: lista de epics publicate cu `name/description(locale)`, `storyCount`, `regionCount`, `readersCount`, `averageRating`, etc.

2) **Per-user overlay (lightweight, fără DB full scan)**:
   - pentru stories:
     - set de storyIds purchased (din `StoryPurchases` pentru user)
     - set de storyDefinitionIds owned (sau storyIds derivat) (din `UserOwnedStories` pentru user)
     - (opțional, dacă vrem să respectăm exact logica de azi) user preference: `AutoFilterStoriesByAge` + `SelectedAgeGroupIds`
   - overlay-ul se aplică peste “base list” în memorie, apoi se face paginare.

Avantaj: cache-ul mare (stories/epics) nu explodează per-user, iar per-user query-ul devine mic și rapid.

### TTL & invalidare la publish
- **TTL 12 ore (720 minute)**: optim pentru că "publish-urile nu sunt dese", deci lista nu se schimbă frecvent:
  - trebuie să acceptăm că "readersCount/rating" pot fi "stale" până la TTL (stats au TTL separat de 10 minute).
  - dacă vrem "fresh" după publish, invalidarea explicită ajută mult (se face automat după publish job).
- **Invalidare la final de publish job**: foarte bună ca “cache busting” (stories list se schimbă).
  - Observație: invalidarea trebuie să fie per-locale sau “all locales”.
  - Dacă în viitor scalezi pe mai multe instanțe, invalidarea în memory cache trebuie replicată (distributed cache sau message bus). Pe B1 single-instance e OK.

### Parametru reglabil din admin-dashboard
Cel mai safe approach:
- **Config în DB** (ex: `Settings` table) + `IOptionsMonitor`/un service care citește valoarea și o aplică la `MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow`.
- Alternativ: config în `appsettings` + redeploy (mai simplu, dar nu “din dashboard”).

---

## 5) Memorie: cât ar putea costa?
Rough order-of-magnitude (nu exact):
- Un item “light” (story marketplace) cu string-uri + liste mici poate ajunge ușor la **~0.5–3 KB** în JSON; în .NET objects în memorie poate fi mai mult (overhead obiecte + string intern).
- Exemplu:
  - 500 stories * 2 KB ≈ 1 MB (JSON), dar în memorie poate fi câțiva MB.
  - Per-locale: dacă ai 3 limbi, multiplici.

Pe B1 (1.75 GB RAM) asta e ok, dar important e:
- să cache-uim un “shape” cât mai **slab** (DTO light / proiecție), nu entități EF cu graf mare.
- să evităm capturarea de grafuri EF tracked (folosim `AsNoTracking` și proiecții).

---

## 6) Întrebări deschise (ca să alegem strategia finală)
1) Vrem ca marketplace list să includă (acum sau în viitor) progresul (`isCompleted`/`progressPercentage`)? Dacă da, cache-ul devine mai user-heavy (sau overlay cu agregare per user).
2) Auto-age-filter: trebuie să fie aplicat strict identic cu DB? (Azi e DB filter pe `AgeGroups`). Pentru in-memory filtering, trebuie să avem ageGroupIds per story în “base cache”.
3) Acceptăm că rating/readers se pot “stale” până la TTL, sau vrem un TTL separat mai scurt pentru agregate?

---

## Next steps (după ce discutăm)
Dacă e “go” pe caching:
- Implementăm un `IMarketplaceCache` (in-memory) cu chei per locale.
- Separăm “base data” (global) de “user overlay”.
- Adăugăm invalidare la publish job completion (un singur call).
- Adăugăm TTL configurabil (inițial din `appsettings`, apoi din DB/admin endpoint).

---

## Status implementare (în repo acum)
Am implementat “quick win” caching pe backend pentru marketplace, fără breaking changes:
- **Cache base catalog**:
  - stories: `marketplace:stories:base:{locale}`
  - epics: `marketplace:epics:base:{locale}`
  - TTL: `MarketplaceCache:BaseTtlMinutes` (default 720 = 12 ore)
- **Cache stats (volatil)**:
  - stories: `marketplace:stories:stats`
  - epics: `marketplace:epics:stats`
  - TTL: `MarketplaceCache:StatsTtlMinutes` (default 10)
- **Overlay per user**:
  - aplicat per-request, dar doar pe pagina curentă (batch query), pentru `IsPurchased/IsOwned`
- **Invalidare cache (reset total, all locales)**:
  - după `StoryPublishQueueWorker` job completed
  - după `EpicPublishQueueJob` job completed
- **Admin endpoint pentru reset manual**:
  - `POST /api/admin/cache/marketplace/reset` (Admin role)
- **Emergency kill-switch (disable cache)**:
  - `GET /api/admin/cache/marketplace/status` (Admin role) – vezi `Enabled` + TTL-uri
  - `POST /api/admin/cache/marketplace/enabled` body `{ "enabled": true|false }` (Admin role)
  - Notă: toggle-ul e un **runtime override** (nu persistă după restart) – ideal îl persistăm ulterior în DB dacă vrei.



