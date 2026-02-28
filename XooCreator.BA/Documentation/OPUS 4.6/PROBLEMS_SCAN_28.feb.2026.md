# XooCreator Backend — Performance Problems Scan

**Data:** 28 Februarie 2026  
**Server curent:** Azure B1 (1.75 GB RAM, 1 vCPU)  
**Stack:** ASP.NET Core 8, EF Core 8, PostgreSQL (Npgsql), Azure Storage Blobs/Queues

---

## Cuprins

1. [Rezumat executiv](#rezumat-executiv)
2. [Probleme CRITICE](#probleme-critice)
3. [Probleme MEDII](#probleme-medii)
4. [Probleme MINORE](#probleme-minore)
5. [Recomandări pentru indecși DB](#recomandări-pentru-indecși-db)
6. [Prioritizare recomandată](#prioritizare-recomandată)

## Ghiduri de Implementare Detaliate

Fiecare problemă are instrucțiuni pas-cu-pas în ghidurile de mai jos:

| Ghid | Fișier | Acoperă |
|------|--------|---------|
| **01 — Infrastructure Quick Wins** | `IMPL_GUIDE_01_Infrastructure_Quick_Wins.md` | INF-01→12: Auth0 cache, connection pool, compression, NoTracking, GC sampling, BlobSas |
| **02 — Marketplace & Reviews** | `IMPL_GUIDE_02_Marketplace_Reviews.md` | MKT-01→17: SQL aggregation, favorites by IDs, AsNoTracking, file I/O, N+1 |
| **03 — Stories & Epic Play** | `IMPL_GUIDE_03_Stories_Epic_Play.md` | STR-01→11: Quiz answers, pagination, batch updates, hero images, ILike index |
| **04 — Editor & Drafts** | `IMPL_GUIDE_04_Editor_Drafts.md` | EDT-01→09: Dialog nodes N+1, craft filter, double loads, ZIP streaming |
| **05 — Arhitectură & DB Indexes** | `IMPL_GUIDE_05_Architecture_DB_Indexes.md` | INF-01: Workers separation, HTTP/2, indexes SQL script, UserProfile, cache |

---

## Rezumat executiv

| Severitate | Număr probleme |
|------------|----------------|
| CRITICE    | 18             |
| MEDII      | 20             |
| MINORE     | 13             |
| **TOTAL**  | **51**         |

Principalele categorii de probleme identificate:
- **20+ HostedServices pe B1** — concurență pe CPU/RAM cu API-ul
- **Cache Auth0 invalidat la fiecare request** — lovitură DB pe fiecare request autentificat
- **Încărcarea a 10.000 de elemente în memorie** — favorites/marketplace
- **Lipsa `AsNoTracking()`** pe majoritatea query-urilor read-only
- **N+1 queries** în mai multe locuri (reviews, heroes, dialog nodes, quiz answers)
- **Calcul statistici în memorie** în loc de SQL aggregation
- **Fișiere I/O sincrone** pe calea request-ului
- **Lipsa response compression** (Brotli/Gzip)
- **Lipsa connection pool configuration** pentru PostgreSQL
- **Paginare în memorie** în loc de DB-level pagination

---

## Probleme CRITICE

### INF-01: 20 HostedServices concurează pe B1

**Fișier:** `Program.cs` (liniile 58–77)

**Problema:** 20 de `BackgroundService` (publish, version, import, export, fork, audio, image, translation, cleanup, epic, hero, animal) rulează în același proces cu API-ul pe B1 (1 vCPU, 1.75 GB RAM). Concurează pentru CPU, memorie și conexiuni DB.

**Impact:** Încetinirea API-ului, posibil OOM, latență crescută la request-uri.

**Fix recomandat:**
- Mută workerii grei (import/export/publish/audio) într-o Azure Function sau un Worker Service separat
- Sau folosește un singur worker "meta" care procesează mai multe queue-uri cu paralelism limitat
- Sau activează workerii selectiv per mediu (doar workerii necesari pe B1)

---

### INF-02: Cache Auth0 invalidat înainte de fiecare fetch

**Fișier:** `Infrastructure\Services\Auth0UserService.cs` (liniile 70–72)

**Problema:** `InvalidateUserCache(auth0Id)` este apelat înainte de `EnsureAsync`, deci cache-ul nu ajută niciodată la primul request. Fiecare request autentificat face un DB call pentru user.

**Fix recomandat:** Elimină `InvalidateUserCache` de dinainte de `EnsureAsync`. Invalidează cache-ul doar când datele userului se actualizează (ex: după `SaveChangesAsync` în `EnsureAsync` sau dintr-un endpoint admin).

---

### INF-03: Lipsa configurării connection pool PostgreSQL

**Fișier:** `Infrastructure\Configuration\DatabaseConfiguration.cs` (liniile 28–32)

**Problema:** Nu sunt setate `MinPoolSize`, `MaxPoolSize`, `CommandTimeout`. Default Npgsql pool = 100 conexiuni, ceea ce e prea mult pentru B1.

**Fix recomandat:**
```
MinPoolSize = 2;
MaxPoolSize = 20;
CommandTimeout = 30;
```

---

### INF-04: `BuildServiceProvider()` în configurația DI

**Fișier:** `Infrastructure\Configuration\DatabaseConfiguration.cs` (linia 41)

**Problema:** `services.BuildServiceProvider()` creează un service provider nou, ducând la instanțe duplicate și probleme DI.

**Fix recomandat:** Înregistrează `IdempotentMigrationCommandInterceptor` în DI și rezolvă-l din `IServiceProvider` după build, sau pasează `null` pentru logger la interceptor.

---

### MKT-01: Încărcare 10.000 stories pentru favorites

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\FavoritesRepository.cs` (liniile 95–99)

**Problema:** Încarcă până la 10.000 de stories din marketplace ca să filtreze favoriteele — `PageSize = 10000`.

**Fix recomandat:** Adaugă o metodă `GetStoriesByIdsAsync(storyIds, locale)` care încarcă doar stories cu ID-urile favorite.

---

### MKT-02: Încărcare 10.000 epics pentru favorites

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\EpicFavoritesRepository.cs` (liniile 93–100)

**Problema:** Același pattern — încarcă 10.000 epics pentru filtrarea favoritelor.

**Fix recomandat:** Adaugă `GetEpicsByIdsAsync(epicIds, locale)` și folosește-l pentru favorites.

---

### MKT-03: Statistici reviews calculate în memorie (Stories)

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\StoryReviewsRepository.cs` (liniile 164–191)

**Problema:** `GetReviewStatisticsAsync` încarcă TOATE review-urile unui story în memorie cu `ToListAsync()`, apoi calculează `Count`, `Average`, `GroupBy`.

**Fix recomandat:** SQL aggregation:
```csharp
var stats = await _context.StoryReviews
    .AsNoTracking()
    .Where(r => r.StoryId == storyId && r.IsActive)
    .GroupBy(r => r.Rating)
    .Select(g => new { Rating = g.Key, Count = g.Count() })
    .ToListAsync();
```

---

### MKT-04: Statistici reviews calculate în memorie (Epics)

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\EpicReviewsRepository.cs` (liniile 163–189)

**Problema:** Același pattern ca MKT-03 dar pentru epic reviews.

**Fix recomandat:** Aceeași abordare — SQL aggregation cu `GroupBy` + `Count()`.

---

### MKT-05: Statistici globale reviews — full table scan

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\StoryReviewsRepository.cs` (liniile 194–214)

**Problema:** `GetGlobalReviewStatisticsAsync` încarcă TOATE review-urile active din baza de date. Full table scan.

**Fix recomandat:** SQL aggregation + posibil un materialized view sau cache pentru admin stats.

---

### MKT-06: Încărcare toate review-urile epic pentru details

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\EpicsMarketplaceRepository.cs` (liniile 165–172)

**Problema:** `GetEpicDetailsAsync` încarcă toate review-urile unui epic doar ca să calculeze count și average.

**Fix recomandat:** SQL aggregation inline:
```csharp
var stats = await _context.EpicReviews
    .AsNoTracking()
    .Where(r => r.EpicId == epicId && r.IsActive)
    .GroupBy(r => 1)
    .Select(g => new { Total = g.Count(), Avg = g.Average(r => (double)r.Rating) })
    .FirstOrDefaultAsync();
```

---

### MKT-07: N+1 queries pentru autor și review în StoryDetailsMapper

**Fișier:** `Features\TalesOfAlchimalia\Market\Mappers\StoryDetailsMapper.cs` (liniile 44–47, 59–66)

**Problema:** Query separat pentru autor (`AlchimaliaUser`) și pentru user review (`GetUserReviewAsync`) per story details request.

**Fix recomandat:** Include `Owner`/`CreatedBy` user în query-ul principal. Combină user review lookup cu query-ul principal.

---

### MKT-08: N+1 potențial în MapToDtoAsync

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\EpicReviewsRepository.cs` (liniile 192–217)

**Problema:** Fallback `review.User = await _context.AlchimaliaUsers.FirstOrDefaultAsync(...)` per review dacă `Include(r => r.User)` lipsește.

**Fix recomandat:** Proiectează direct în DTO din query, evitând maparea manuală.

---

### STR-01: GetStoryQuizAnswersEndpoint — încarcă toate răspunsurile, filtrează în memorie

**Fișier:** `Features\Stories\Endpoints\GetStoryQuizAnswersEndpoint.cs` (liniile 50–59)

**Problema:** Încarcă toate quiz answers ale userului, apoi filtrează by `storyId` cu `StringComparison.OrdinalIgnoreCase` în memorie.

**Fix recomandat:** Filtrare în database:
```csharp
var storyAnswers = await _context.StoryQuizAnswers
    .AsNoTracking()
    .Where(a => a.UserId == userId && EF.Functions.ILike(a.StoryId, storyId))
    .GroupBy(a => a.TileId)
    .Select(g => g.OrderByDescending(a => a.AnsweredAt).First())
    .ToListAsync(ct);
```

---

### STR-02: GetUserCreatedStoriesEndpoint — încarcă tot, paginare în memorie

**Fișier:** `Features\Library\Endpoints\GetUserCreatedStoriesEndpoint.cs` (liniile 48–59, 95–98, 165–166)

**Problema:** Încarcă toate published stories + toate drafts, apoi paginare în memorie cu `Skip`/`Take`.

**Fix recomandat:** Mută paginarea în database. Folosește query-uri separate cu `Skip`/`Take` în SQL.

---

### STR-03: CompleteEvaluationEndpoint — N+1 updates

**Fișier:** `Features\Stories\Endpoints\CompleteEvaluationEndpoint.cs` (liniile 244–264)

**Problema:** Update per answer în loop — `FirstOrDefaultAsync` per fiecare răspuns.

**Fix recomandat:** Batch update — încarcă toate answers cu `Where(a => ids.Contains(a.Id))` și actualizează-le în lot.

---

### STR-04: StoryEpicProgressService — N+1 hero image lookups

**Fișier:** `Features\Story-Editor\Story-Epic\Services\StoryEpicProgressService.cs` (liniile 384–414, 430–439)

**Problema:** `GetHeroImageUrlAsync` apelat per hero într-un loop. Fiecare hero = un DB call separat.

**Fix recomandat:** Batch-load hero image URLs:
```csharp
var heroIds = heroReferences.Select(h => h.HeroId).Distinct().ToList();
var heroImages = await _context.EpicHeroDefinitions
    .Where(h => heroIds.Contains(h.Id))
    .Select(h => new { h.Id, h.ImageUrl })
    .ToDictionaryAsync(h => h.Id, h => h.ImageUrl, ct);
```

---

### EDT-01: StoryTileUpdater — N+1 dialog nodes query

**Fișier:** `Features\Story-Editor\Services\Content\StoryTileUpdater.cs` (liniile 174–183)

**Problema:** Per fiecare dialog tile, se face un query separat pentru nodes cu Include-uri complexe.

**Fix recomandat:** Încarcă toate dialog nodes pentru craft-ul curent într-un singur query, grupate by `StoryCraftDialogTileId`.

---

### EDT-02: ListStoryCraftsEndpoint — load all, filter in memory

**Fișier:** `Features\Story-Editor\Endpoints\ListStoryCraftsEndpoint.cs` (liniile 59–69)

**Problema:** Pentru `wantAssigned` și `wantClaimable`, încarcă toate craft-urile, apoi filtrare în memorie.

**Fix recomandat:** Adaugă metode repository dedicate: `ListByAssignedReviewerAsync`, `ListClaimableAsync` cu filtrare și paginare DB-level.

---

## Probleme MEDII

### INF-05: Lipsa response compression (Brotli/Gzip)

**Fișier:** `Program.cs`

**Problema:** Nu există `AddResponseCompression` sau `UseResponseCompression`. Toate response-urile JSON sunt trimise necomprimate.

**Fix recomandat:**
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
app.UseResponseCompression();
```

---

### INF-06: Lipsa HTTP/2

**Fișier:** `Program.cs`

**Problema:** Kestrel nu e configurat pentru HTTP/2. Multiplexing-ul ar reduce latența.

**Fix recomandat:** Configurare Kestrel:
```csharp
listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
```

---

### INF-07: MemoryCache fără limită de dimensiune

**Fișier:** `Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs` (linia 47)

**Problema:** `AddMemoryCache()` fără `SizeLimit`. Cache-ul poate crește nelimitat pe B1.

**Fix recomandat:** Setează `SizeLimit` și `CompactionPercentage` + `Size` pe fiecare entry.

---

### INF-08: BlobSasService — `CreateIfNotExistsAsync` la fiecare SAS

**Fișier:** `Infrastructure\Services\Blob\BlobSasService.cs` (liniile 43–46)

**Problema:** `GetPutSasAsync` apelează `CreateIfNotExistsAsync` pe container la fiecare request. Extra Azure Storage call.

**Fix recomandat:** Apelează `CreateIfNotExistsAsync` o singură dată la startup sau cache-uiește rezultatul.

---

### INF-09: TreeOfHeroesRepository — load all hero definitions

**Fișier:** `Features\TreeOfHeroes\Repositories\TreeOfHeroesRepository.cs` (liniile 261–341)

**Problema:** `GetTreeOfHeroesConfigAsync` încarcă toate `HeroDefinitionDefinitions` cu Include-uri. Heavy pe memorie.

**Fix recomandat:** Proiecție doar pe coloanele necesare. Cache config 24h (se schimbă rar).

---

### INF-10: UserProfileService — 6+ query-uri secvențiale

**Fișier:** `Features\User\Services\UserProfileService.cs` (liniile 32–79)

**Problema:** `GetUserProfileAsync` execută 6+ DB calls secvențial: user, wallet, hasEverPurchased, lastPurchase, lastTransaction, BuilderConfig, BodyParts, AnimalDefinitions.

**Fix recomandat:** Combină în mai puține query-uri sau rulează cele independente cu `Task.WhenAll`. Posibil view/stored procedure.

---

### MKT-09: Missing `AsNoTracking()` pe query-uri read-only (Marketplace)

**Fișiere afectate:**
- `EpicsMarketplaceRepository.cs` — liniile 146–154, 166–167, 177–179, 307–309
- `EpicReviewsRepository.cs` — liniile 114–136
- `EpicFavoritesRepository.cs` — liniile 27–28, 35–36, 55–56, 68–69, 75–78, 84–86
- `StoryReviewsRepository.cs` — liniile 116–136
- `FavoritesRepository.cs` — liniile 27–29, 55–56, 61–62, 75–81
- `StoriesMarketplaceRepository.cs` — liniile 382–396, 447–450
- `AcquireFreeStoryEndpoint.cs` — liniile 46–47, 55–56

**Fix recomandat:** Adaugă `.AsNoTracking()` pe toate query-urile read-only. Menține tracking doar unde entitățile sunt modificate.

---

### MKT-10: GetReadersSummaryEndpoint — call-uri secvențiale

**Fișier:** `Features\TalesOfAlchimalia\Market\Endpoints\GetReadersSummaryEndpoint.cs` (liniile 53–69)

**Problema:** 4 call-uri async independente rulate secvențial.

**Fix recomandat:** `Task.WhenAll` pentru call-urile independente.

---

### MKT-11: File I/O sincron pe calea request-ului

**Fișiere:**
- `StoriesMarketplaceRepository.cs` (liniile 498–523) — `File.ReadAllText`
- `MarketplaceCatalogCache.cs` (liniile 371–376) — `File.ReadAllText`

**Problema:** `File.ReadAllText` blochează thread-ul. Pe B1 cu puține thread-uri, asta reduce throughput-ul.

**Fix recomandat:** `File.ReadAllTextAsync` + cache in-memory pentru fișierele citite frecvent.

---

### MKT-12: Direct DbContext usage în endpoint

**Fișier:** `Features\TalesOfAlchimalia\Market\Endpoints\GetReadersSummaryEndpoint.cs` (liniile 116–125)

**Problema:** Endpoint-ul folosește `XooDbContext` direct, fără repository, fără `AsNoTracking()`.

**Fix recomandat:** Mută logica în repository. Adaugă `AsNoTracking()`.

---

### STR-05: Missing `AsNoTracking()` pe StoriesRepository read queries

**Fișier:** `Features\Story-Editor\Repositories\StoriesRepository.cs` (liniile 33–63, 129–164)

**Problema:** `GetAllStoriesAsync` și `GetStoryDefinitionByIdAsync` sunt read-only dar track-uite.

**Fix recomandat:** Adaugă `.AsNoTracking()` pe ambele metode.

---

### STR-06: Overfetching tiles pentru story details

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\StoriesMarketplaceRepository.cs` (liniile 527–536)

**Problema:** `Include(s => s.Tiles)` doar ca să calculeze `totalTiles = def.Tiles.Count`. Încarcă toate tile-urile.

**Fix recomandat:** Query separat: `await _context.StoryTiles.CountAsync(t => t.StoryDefinitionId == defId)`.

---

### STR-07: Missing `AsNoTracking()` pe EpicProgressRepository

**Fișier:** `Features\Story-Editor\Story-Epic\Repositories\EpicProgressRepository.cs` (liniile 17–27, 31–41)

**Problema:** `GetEpicProgressAsync` și `GetEpicStoryProgressAsync` sunt read-only dar track-uite.

**Fix recomandat:** Adaugă `.AsNoTracking()`.

---

### STR-08: StoriesService.GetStoryByIdAsync — call-uri secvențiale

**Fișier:** `Features\Stories\Services\StoriesService.cs` (liniile 45–98)

**Problema:** Call-uri secvențiale independente: `GetStoryByIdAsync`, `GetUserStoryProgressAsync`, `GetStoryCompletionStatusAsync`, `GetAvailableLanguagesAsync`.

**Fix recomandat:** `Task.WhenAll` pentru call-urile independente.

---

### STR-09: `EF.Functions.ILike` pe StoryId — nu folosește index

**Fișier:** `Features\Story-Editor\Repositories\StoriesRepository.cs` (liniile 174–176, 201–203, 232–234, 253–255, 292–294)

**Problema:** `EF.Functions.ILike(p.StoryId, storyId)` previne utilizarea index-ului pe `StoryId`.

**Fix recomandat:** Normalizează StoryId-urile (lowercase) la insert și folosește `==` sau adaugă un functional index `LOWER(StoryId)`.

---

### EDT-03: StoryCraftsRepository — missing `AsNoTracking()` pe read queries

**Fișier:** `Features\Story-Editor\Repositories\StoryCraftsRepository.cs` (liniile 18–51, 54–90, 261–288)

**Problema:** `GetAsync`, `GetWithLanguageAsync`, `ListByOwnerAsync`, `ListAllAsync` nu folosesc `AsNoTracking()` pe query-uri read-only.

**Fix recomandat:** Adaugă `.AsNoTracking()` pe citiri. Pentru `SaveDraftAsync` unde craft-ul trebuie tracked, folosește overload-uri separate.

---

### EDT-04: SaveStoryEditEndpoint — double craft load

**Fișier:** `Features\Story-Editor\Endpoints\SaveStoryEditEndpoint.cs` (liniile 106, 138)

**Problema:** `_crafts.GetAsync` este apelat pentru validare, apoi `SaveDraftAsync` îl reîncarcă.

**Fix recomandat:** Pasează craft-ul deja încărcat în `SaveDraftAsync`.

---

### EDT-05: StoryPublishingService — duplicate craft load

**Fișier:** `Features\Story-Editor\Services\StoryPublishingService.cs` (liniile 37–64)

**Problema:** Worker-ul încarcă craft-ul complet, apoi `UpsertFromCraftAsync` îl reîncarcă cu same includes.

**Fix recomandat:** Adaugă overload care acceptă craft-ul deja încărcat.

---

### EDT-06: StoryExportService — ZIP complet în memorie

**Fișier:** `Features\Story-Editor\Services\StoryExportService.cs` (liniile 38–74, 164–204)

**Problema:** ZIP-ul este construit complet în `MemoryStream`, apoi `ms.ToArray()`. Pentru stories mari, presiune pe memorie B1.

**Fix recomandat:** Stream direct în blob storage în loc de buffering complet.

---

### EDT-07: PublishStoryEndpoint — `StoryId.ToLower()` previne index

**Fișier:** `Features\Story-Editor\Endpoints\PublishStoryEndpoint.cs` (liniile 330–332, 338)

**Problema:** `j.StoryId.ToLower() == storyIdLower` previne utilizarea indexului.

**Fix recomandat:** `EF.Functions.ILike(j.StoryId, storyId)` sau normalizare la insert.

---

## Probleme MINORE

### INF-11: RequestPerformanceMiddleware — GC stats per request

**Fișier:** `Infrastructure\Middleware\RequestPerformanceMiddleware.cs` (liniile 53–56, 70–81)

**Problema:** Fiecare request declanșează `GC.GetTotalMemory`, `GC.CollectionCount`, `Environment.WorkingSet`.

**Fix recomandat:** Gate-uiește logic behind `IsDevelopment()` sau sampling (1 din 100 requests).

---

### INF-12: DbContext — lipsa query tracking behavior global

**Fișier:** `Data\XooDbContext.cs`

**Problema:** Nu există `UseQueryTrackingBehavior(NoTracking)` global. Fiecare query trebuie marcat individual.

**Fix recomandat:** Consideră setarea globală și folosește `AsTracking()` explicit doar unde faci update-uri.

---

### INF-13: ImageCompressionService — full image load

**Fișier:** `Infrastructure\Services\Images\ImageCompressionService.cs` (liniile 59–60)

**Problema:** `Image.LoadAsync` încarcă imaginea completă în memorie.

**Fix recomandat:** Procesare în worker separat sau Azure Function. Limitează decode size cu `DecoderOptions`.

---

### INF-14: TreeOfLightRepository — GetStoryHeroesAsync fără filtrare

**Fișier:** `Features\TreeOfLight\Repositories\TreeOfLightRepository.cs` (liniile 204–210)

**Problema:** Încarcă toți eroii publicați fără paginare.

**Fix recomandat:** Adaugă paginare și/sau cache.

---

### MKT-13: Missing index pe StoryReviews.StoryId

**Fișier:** `Data\Configurations\StoryReviewConfiguration.cs`

**Problema:** Index pe `(UserId, StoryId)` dar nu pe `StoryId` singur. Query-uri `Where(r => r.StoryId == x)` nu profită.

**Fix recomandat:** `builder.HasIndex(x => new { x.StoryId, x.IsActive });`

---

### MKT-14: Missing index pe EpicReviews.EpicId

**Fișier:** `Data\Configurations\EpicReviewConfiguration.cs`

**Problema:** Lipsește index pe `EpicId` singur.

**Fix recomandat:** `builder.HasIndex(x => new { x.EpicId, x.IsActive });`

---

### MKT-15: EpicLikesRepository — ILike cu list.Any()

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\EpicLikesRepository.cs` (liniile 60–64)

**Problema:** `list.Any(id => EF.Functions.ILike(l.EpicId, id))` produce chain mare de `OR`. Ineficient.

**Fix recomandat:** Dacă ID-urile sunt exact match, folosește `list.Contains(l.EpicId)`.

---

### MKT-16: FavoritesRepository.IsFavoriteAsync — 2 round-trips

**Fișier:** `Features\TalesOfAlchimalia\Market\Repositories\FavoritesRepository.cs` (liniile 73–82)

**Problema:** Două query-uri pentru o simplă verificare de favorit.

**Fix recomandat:** Un singur query cu join/subquery.

---

### MKT-17: StoriesMarketplaceService — 5 DB calls secvențiale în purchase

**Fișier:** `Features\TalesOfAlchimalia\Market\Services\StoriesMarketplaceService.cs` (liniile 64–111)

**Problema:** 5 DB calls secvențiale: story lookup, isPurchased, price, purchase, wallet.

**Fix recomandat:** Combină story lookup cu price. Rulează isPurchased în paralel. Returnează wallet din purchase.

---

### STR-10: Verbose logging pe hot paths

**Fișiere:**
- `CompleteEvaluationEndpoint.cs` (liniile 82–86, 106–115, etc.)
- `SubmitQuizAnswerEndpoint.cs` (liniile 67–70, 83–88, etc.)

**Problema:** Multiple `LogInformation` pe fiecare request pe calea de evaluation/quiz.

**Fix recomandat:** Downgrade la `LogDebug`.

---

### STR-11: StoryDetailsMapper — file I/O sincron pe hot path

**Fișier:** `Features\TalesOfAlchimalia\Market\Mappers\StoryDetailsMapper.cs` (liniile 151–183)

**Problema:** `GetSummaryFromJson` citește de pe filesystem la fiecare request.

**Fix recomandat:** Cache summaries in memory by `(storyId, locale)`.

---

### EDT-08: StoryDraftManager.EnsureDraftAsync — double load

**Fișier:** `Features\Story-Editor\Services\Content\StoryDraftManager.cs` (liniile 25–37)

**Problema:** `GetAsync` încarcă craft-ul complet doar ca să verifice existența.

**Fix recomandat:** `AnyAsync` pentru existență:
```csharp
var exists = await _context.StoryCrafts.AnyAsync(x => x.StoryId == storyId, ct);
```

---

### EDT-09: Missing cache pentru GetAvailableLanguagesAsync

**Fișier:** `Features\Story-Editor\Repositories\StoryCraftsRepository.cs` (liniile 327–349)

**Problema:** Limbile sunt încărcate din DB la fiecare request. Se schimbă rar.

**Fix recomandat:** Cache scurt (5–10 min) per `storyId`.

---

## Recomandări pentru indecși DB

Bazat pe pattern-urile de query identificate:

| Tabel | Coloane | Motivul |
|-------|---------|---------|
| `StoryReviews` | `(StoryId, IsActive)` | Query-uri `Where(StoryId, IsActive)` frecvente |
| `EpicReviews` | `(EpicId, IsActive)` | Similar |
| `EpicStoryProgress` | `(UserId, EpicId)` | `GetEpicStoryProgressAsync` |
| `EpicProgress` | `(UserId, EpicId)` | `GetEpicProgressAsync` |
| `UserStoryReadProgress` | `(UserId, StoryId)` | Query-uri frecvente |
| `StoryCrafts` | `(OwnerUserId, UpdatedAt)` | Listare sorted |
| `StoryCrafts` | `(AssignedReviewerUserId)` | Filtrare assigned |
| `StoryCraftTranslations` | `(StoryCraftId, LanguageCode)` | `GetWithLanguageAsync` |
| `StoryPublishJobs` | `(StoryId, QueuedAtUtc)` | Latest publish job |
| `UserTokenBalances` | `(UserId, Type, Value)` | Token lookups |
| `HeroProgress` | `(UserId, HeroId)` | Hero progress lookups |
| `TreeProgress` | `(UserId, TreeConfigurationId)` | Tree progress lookups |
| `StoryProgress` | `(UserId, TreeConfigurationId, StoryId)` | Story progress lookups |

**Recomandare:** Rulează `EXPLAIN ANALYZE` pe query-urile principale înainte de a adăuga indecși noi.

---

## Prioritizare recomandată

### Fase 1 — Impact imediat, efort mic (1–2 zile)

| # | Problemă | Impact estimat |
|---|----------|----------------|
| INF-02 | Fix Auth0 cache invalidation | -1 DB call per request |
| INF-03 | Configurare connection pool | Stabilitate DB |
| INF-05 | Adaugă response compression | -60-80% payload size |
| MKT-09, STR-05, STR-07, EDT-03 | `AsNoTracking()` peste tot | -20-30% memorie EF |
| INF-12 | Global NoTracking behavior | Previne uitarea pe viitor |

### Fase 2 — Impact mare, efort mediu (3–5 zile)

| # | Problemă | Impact estimat |
|---|----------|----------------|
| MKT-03/04/05/06 | SQL aggregation pentru reviews | -90% DB load pe stats |
| MKT-01/02 | Fix favorites (load by IDs) | -95% memorie pe favorites |
| STR-01 | Fix quiz answers filter | Query 100x mai rapid |
| STR-02 | DB-level pagination | Eliminare memory bloat |
| STR-03 | Batch quiz updates | -N DB calls |
| STR-04 | Batch hero image lookups | -N DB calls per epic progress |
| EDT-01 | Batch dialog nodes load | -N DB calls per save |
| EDT-02 | Filter crafts in DB | Eliminare memory bloat |

### Fase 3 — Arhitectural, efort mare (1–2 săptămâni)

| # | Problemă | Impact estimat |
|---|----------|----------------|
| INF-01 | Separare workers în proces dedicat | -50% CPU/RAM pe B1 |
| EDT-04/05 | Eliminare double loads | -2 DB calls per save/publish |
| EDT-06 | Streaming ZIP export | Reduce memory spikes |
| INF-09/10 | Optimizare tree + profile queries | Reduce latență per request |
| DB Indexes | Adaugă indecșii recomandați | Query-uri 10-100x mai rapide |

### Fase 4 — Nice to have

| # | Problemă | Impact estimat |
|---|----------|----------------|
| INF-06 | HTTP/2 | Multiplexing, header compression |
| INF-07 | MemoryCache size limit | Previne OOM |
| INF-11 | GC stats sampling | Reduce overhead per request |
| EDT-09 | Cache languages | Minor latency reduction |
| MKT-17 | Optimizare purchase flow | Minor latency reduction |

---

## Considerații specifice Azure B1

Azure B1 oferă resurse limitate (1.75 GB RAM, 1 vCPU). Recomandări generale:

1. **CPU**: Evită calcule în memorie (aggregations, sorting) — delegă la PostgreSQL
2. **Memorie**: Evită `ToListAsync()` pe seturi mari, folosește proiecții și paginare
3. **Threads**: Evită I/O sincron (`File.ReadAllText`), folosește variante async
4. **Conexiuni DB**: Limitează pool-ul la 20, monitorizează cu `pg_stat_activity`
5. **Background workers**: 20 workers pe B1 sunt prea mulți — mută-i separat
6. **Cache**: Limitează dimensiunea cache-ului in-memory, evită cache unbounded
7. **Payload**: Activează compression (Brotli/Gzip) — reduce bandwidth cu 60-80%
8. **Scale**: Când treci pe un tier superior, prioritizează separarea workers de API

---

*Generat automat pe baza analizei codului sursă din `XooCreator.BA` — 28 Februarie 2026*
