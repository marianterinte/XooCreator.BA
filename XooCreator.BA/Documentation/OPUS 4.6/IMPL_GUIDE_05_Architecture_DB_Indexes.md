# Ghid de Implementare 05 — Arhitectură & Indecși DB

**Referință:** `PROBLEMS_SCAN_28.feb.2026.md`  
**Probleme acoperite:** INF-01, INF-06, INF-09, INF-10, INF-13, INF-14, DB Indexes  
**Efort estimat:** 1–2 săptămâni  
**Impact:** Structural — îmbunătățiri pe termen lung

**Implementat:** Task 2 (HTTP/2), Task 3 (UserProfileService combine queries), Task 4 (TreeOfHeroes config cache), Task 5 (Database indexes — V00113), Task 6 (TreeOfLight GetStoryHeroesAsync cache). **Exclus / decis mai târziu:** Task 1 (Separare workers — feature flags sau Worker App), Task 7 (ImageCompressionService — Azure Function / worker separat).

---

## Task 1: Separare Background Workers de API (INF-01)

### Problema
20 HostedServices în același proces cu API-ul pe B1. Asta e probabil cauza principală a încetinirilor.

### Opțiunea A — Feature Flag per Worker (Rapid, 1 zi)

Adaugă o configurare care permite dezactivarea selectivă a workerilor.

**Fișier: `appsettings.json`**

Adaugă secțiune nouă:
```json
{
  "Workers": {
    "EnableStoryPublish": true,
    "EnableStoryVersion": true,
    "EnableStoryImport": false,
    "EnableStoryFork": false,
    "EnableStoryForkAssets": false,
    "EnableStoryExport": false,
    "EnableStoryDocumentExport": false,
    "EnableStoryAudioExport": false,
    "EnableStoryAudioImport": false,
    "EnableStoryTranslation": false,
    "EnableStoryImageImport": false,
    "EnableStoryImageExport": false,
    "EnableExportCleanup": true,
    "EnableEpicPublish": true,
    "EnableEpicVersion": true,
    "EnableEpicAggregates": true,
    "EnableHeroDefinitionVersion": false,
    "EnableAnimalVersion": false,
    "EnableHeroPublish": false,
    "EnableAnimalPublish": false
  }
}
```

**Fișier: `Program.cs`**

**Înlocuiește** înregistrarea directă a workerilor (liniile 58–77) cu:

```csharp
var workersConfig = builder.Configuration.GetSection("Workers");

void AddWorkerIf<T>(string key) where T : class, IHostedService
{
    if (workersConfig.GetValue<bool>(key, true))
        builder.Services.AddHostedService<T>();
}

AddWorkerIf<StoryPublishQueueWorker>("EnableStoryPublish");
AddWorkerIf<StoryVersionQueueWorker>("EnableStoryVersion");
AddWorkerIf<StoryImportQueueWorker>("EnableStoryImport");
AddWorkerIf<StoryForkQueueWorker>("EnableStoryFork");
AddWorkerIf<StoryForkAssetsQueueWorker>("EnableStoryForkAssets");
AddWorkerIf<StoryExportQueueWorker>("EnableStoryExport");
AddWorkerIf<StoryDocumentExportQueueWorker>("EnableStoryDocumentExport");
AddWorkerIf<StoryAudioExportQueueWorker>("EnableStoryAudioExport");
AddWorkerIf<StoryAudioImportQueueWorker>("EnableStoryAudioImport");
AddWorkerIf<StoryTranslationQueueWorker>("EnableStoryTranslation");
AddWorkerIf<StoryImageImportQueueWorker>("EnableStoryImageImport");
AddWorkerIf<StoryImageExportQueueWorker>("EnableStoryImageExport");
AddWorkerIf<ExportCleanupService>("EnableExportCleanup");
AddWorkerIf<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicPublishQueueJob>("EnableEpicPublish");
AddWorkerIf<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicVersionQueueJob>("EnableEpicVersion");
AddWorkerIf<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicAggregatesQueueJob>("EnableEpicAggregates");
AddWorkerIf<HeroDefinitionVersionQueueWorker>("EnableHeroDefinitionVersion");
AddWorkerIf<AnimalVersionQueueWorker>("EnableAnimalVersion");
AddWorkerIf<HeroPublishQueueWorker>("EnableHeroPublish");
AddWorkerIf<AnimalPublishQueueWorker>("EnableAnimalPublish");
```

### Recomandare pentru B1:
Activează doar workerii esențiali:
- `EnableStoryPublish: true` — fără asta nu se publică stories
- `EnableStoryVersion: true` — versioning
- `EnableEpicPublish: true` — publicare epics
- `EnableEpicVersion: true` — versioning epics
- `EnableEpicAggregates: true` — agregate
- `EnableExportCleanup: true` — cleanup
- Restul: `false` (activează manual când e nevoie de import/export/fork etc.)

### Opțiunea B — Worker App Separat (Pe termen lung, 1 săptămână)

Creează un proiect separat `XooCreator.Workers` care referențiază aceleași servicii dar rulează doar workerii. Deploy pe un App Service Plan separat sau ca Azure Function.

Asta e soluția ideală dar necesită mai mult efort de setup.

---

## Task 2: Configurare HTTP/2 (INF-06) ✅ Implementat

### Fișier: `Program.cs`

**Înlocuiește** (liniile 82–85):
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024L * 1024 * 1024; // 1GB
});
```

**Cu:**
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024L * 1024 * 1024; // 1GB

    var portStr = Environment.GetEnvironmentVariable("PORT");
    if (int.TryParse(portStr, out var port))
    {
        options.ListenAnyIP(port, listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
        });
    }
});
```

**NOTĂ:** HTTP/2 over plaintext (h2c) e suportat de Kestrel. Dacă Azure App Service termină TLS, h2c funcționează intern.

Adaugă using:
```csharp
using Microsoft.AspNetCore.Server.Kestrel.Core;
```

---

## Task 3: Optimizare UserProfileService (INF-10) ✅ Implementat

### Fișier: `Features/User/Services/UserProfileService.cs`

### Problema
6+ query-uri secvențiale pentru profil.

### Modificare — Combine queries unde posibil:

```csharp
public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, CancellationToken ct)
{
    // Combine user + wallet into a single query using projection
    var userInfo = await _context.AlchimaliaUsers
        .AsNoTracking()
        .Where(u => u.Id == userId)
        .Select(u => new
        {
            User = u,
            Wallet = _context.CreditWallets
                .FirstOrDefault(w => w.UserId == userId),
            HasEverPurchased = _context.StoryPurchases
                .Any(p => p.UserId == userId),
            LastPurchase = _context.StoryPurchases
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchasedAt)
                .FirstOrDefault(),
            LastTransaction = _context.CreditTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault()
        })
        .FirstOrDefaultAsync(ct);

    if (userInfo == null) return null;
    
    // BuilderConfig, BodyParts, AnimalDefinitions — cache sau query separat
    // ...
}
```

**NOTĂ:** Verifică că EF Core 8 traduce corect subquery-urile de mai sus. Unele pot necesita `AsSplitQuery()`.

**Alternativă simplă** — rulează query-urile independente cu `Task.WhenAll` dacă fiecare are propriul scope. Dar cu un singur `DbContext` NU poți rula query-uri paralele, deci această abordare necesită `IServiceScopeFactory`.

---

## Task 4: TreeOfHeroes Config Cache (INF-09) ✅ Implementat

### Fișier: `Features/TreeOfHeroes/Repositories/TreeOfHeroesRepository.cs`

### Problema
`GetTreeOfHeroesConfigAsync` încarcă toate `HeroDefinitionDefinitions` cu Include-uri.

### Modificare

Adaugă cache pe config (se schimbă rar):

```csharp
private static readonly string TreeConfigCacheKey = "tree_of_heroes_config";
private readonly IMemoryCache _cache;

public async Task<TreeOfHeroesConfigDto?> GetTreeOfHeroesConfigAsync(CancellationToken ct)
{
    if (_cache.TryGetValue(TreeConfigCacheKey, out TreeOfHeroesConfigDto? cached))
        return cached;

    // ... query existent ...
    var config = /* rezultatul query-ului */;

    _cache.Set(TreeConfigCacheKey, config, new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
        Size = 10
    });

    return config;
}
```

Adaugă invalidare când config-ul e modificat (din admin endpoints).

---

## Task 5: Database Indexes ✅ Implementat

### Creează un nou script SQL

**Fișier:** `Database/Scripts/V00113__add_performance_indexes.sql` (creat).

Verifică numerotarea ultimului script și incrementează.

```sql
-- Performance indexes based on query analysis (28 Feb 2026)

-- StoryReviews: frequently queried by StoryId + IsActive
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_reviews_storyid_isactive
    ON "StoryReviews" ("StoryId", "IsActive");

-- EpicReviews: frequently queried by EpicId + IsActive
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_epic_reviews_epicid_isactive
    ON "EpicReviews" ("EpicId", "IsActive");

-- EpicStoryProgress: queried by (UserId, EpicId)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_epic_story_progress_userid_epicid
    ON "EpicStoryProgress" ("UserId", "EpicId");

-- EpicProgress: queried by (UserId, EpicId)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_epic_progress_userid_epicid
    ON "EpicProgress" ("UserId", "EpicId");

-- UserStoryReadProgress: queried by (UserId, StoryId) frequently
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_user_story_read_progress_userid_storyid
    ON "UserStoryReadProgress" ("UserId", "StoryId");

-- StoryCrafts: listed by OwnerUserId, sorted by UpdatedAt
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_crafts_owneruserid_updatedat
    ON "StoryCrafts" ("OwnerUserId", "UpdatedAt" DESC);

-- StoryCrafts: filtered by AssignedReviewerUserId
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_crafts_assignedreviewer
    ON "StoryCrafts" ("AssignedReviewerUserId")
    WHERE "AssignedReviewerUserId" IS NOT NULL;

-- StoryCraftTranslations: queried by (StoryCraftId, LanguageCode)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_craft_translations_craftid_lang
    ON "StoryCraftTranslations" ("StoryCraftId", "LanguageCode");

-- StoryPublishJobs: queried by (StoryId, Status)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_publish_jobs_storyid_status
    ON "StoryPublishJobs" ("StoryId", "Status");

-- UserTokenBalances: queried by (UserId, Type, Value)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_user_token_balances_userid_type_value
    ON "UserTokenBalances" ("UserId", "Type", "Value");

-- HeroProgress: queried by (UserId, HeroId)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_hero_progress_userid_heroid
    ON "HeroProgress" ("UserId", "HeroId");

-- TreeProgress: queried by (UserId, TreeConfigurationId)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_tree_progress_userid_treeconfigid
    ON "TreeProgress" ("UserId", "TreeConfigurationId");

-- StoryProgress: queried by (UserId, TreeConfigurationId, StoryId)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_progress_userid_treeconfigid_storyid
    ON "StoryProgress" ("UserId", "TreeConfigurationId", "StoryId");

-- StoryQuizAnswers: queried by (UserId, StoryId)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_quiz_answers_userid_storyid
    ON "StoryQuizAnswers" ("UserId", "StoryId");

-- UserFavoriteStories: queried by UserId
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_user_favorite_stories_userid
    ON "UserFavoriteStories" ("UserId");

-- UserFavoriteEpics: queried by UserId
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_user_favorite_epics_userid
    ON "UserFavoriteEpics" ("UserId");

-- StoryReaders: queried by StoryId (for reader counts)
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_readers_storyid
    ON "StoryReaders" ("StoryId");

-- EpicReaders: queried by EpicId
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_epic_readers_epicid
    ON "EpicReaders" ("EpicId");

-- StoryLikes: queried by StoryId
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_story_likes_storyid
    ON "StoryLikes" ("StoryId");

-- EpicLikes: queried by EpicId
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_epic_likes_epicid
    ON "EpicLikes" ("EpicId");
```

### NOTĂ IMPORTANTĂ
- `CREATE INDEX CONCURRENTLY` nu blochează tabelul dar necesită mai mult timp
- Verifică că schema corectă e folosită (poate fi `alchimalia_schema`)
- Prefixează cu schema: `ON "alchimalia_schema"."StoryReviews"` dacă e necesar
- Rulează `EXPLAIN ANALYZE` pe query-urile principale ÎNAINTE de a adăuga indecși pentru a confirma că lipsesc
- Unele indecși pot exista deja — `IF NOT EXISTS` previne erori

### Cum verifici dacă un index e necesar:
```sql
EXPLAIN ANALYZE
SELECT * FROM "alchimalia_schema"."StoryReviews"
WHERE "StoryId" = 'test-story-id' AND "IsActive" = true;
```

Dacă vezi `Seq Scan` → indexul e necesar.  
Dacă vezi `Index Scan` → indexul există deja.

---

## Task 6: TreeOfLight GetStoryHeroesAsync (INF-14) ✅ Implementat

### Fișier: `Features/TreeOfLight/TreeOfLightRepository.cs`

### Problema
`GetStoryHeroesAsync` încarcă toți eroii publicați fără paginare.

### Modificare

Adaugă cache (datele se schimbă rar):

```csharp
public async Task<List<StoryHeroDto>> GetStoryHeroesAsync(CancellationToken ct)
{
    var cacheKey = "story_heroes_published";
    if (_cache.TryGetValue(cacheKey, out List<StoryHeroDto>? cached))
        return cached ?? new();

    var heroes = await _context.StoryHeroes
        .AsNoTracking()
        .Where(h => h.Status == "published")
        .Include(h => h.Translations)
        .Select(h => new StoryHeroDto { /* projection */ })
        .ToListAsync(ct);

    _cache.Set(cacheKey, heroes, new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
        Size = 5
    });

    return heroes;
}
```

---

## Task 7: ImageCompressionService (INF-13)

### Fișier: `Infrastructure/Services/Images/ImageCompressionService.cs`

### Recomandare pe termen lung
Mută procesarea imaginilor într-o Azure Function sau într-un worker separat. Pe B1, image processing poate bloca thread-uri și consuma memorie excesivă.

### Fix rapid — limitează dimensiunea de procesare:

```csharp
// Limitează decode size
var decoderOptions = new DecoderOptions
{
    TargetSize = new Size(2048, 2048) // Max 2048x2048
};
using var image = await Image.LoadAsync(decoderOptions, inputStream, ct);
```

---

## Ordinea de implementare recomandată

### Săptămâna 1:
1. **Task 1 Opțiunea A** (Feature flags workers) — cel mai mare impact
2. **Task 5** (Database indexes) — rulează scriptul
3. **Task 4** (TreeOfHeroes cache) — simplu

### Săptămâna 2:
4. **Task 2** (HTTP/2) — simplu
5. **Task 3** (UserProfileService) — mediu
6. **Task 6** (StoryHeroes cache) — simplu
7. **Task 7** (Image compression) — simplu

### Pe termen lung (post-B1):
- **Task 1 Opțiunea B** — Worker App separat
- Migrare workers la Azure Functions

---

## Checklist post-implementare

- [ ] Verifică că aplicația pornește corect după modificări
- [ ] Monitorizează Application Insights pentru erori noi
- [ ] Verifică memoria procesului după dezactivarea workerilor
- [ ] Rulează `EXPLAIN ANALYZE` pe query-urile principale după indecși
- [ ] Testează marketplace, story details, epic details, editor
- [ ] Monitorizează latența request-urilor timp de 24h
- [ ] Verifică GC pressure în Application Insights

---

*Ghid generat pe baza analizei din PROBLEMS_SCAN_28.feb.2026.md*
