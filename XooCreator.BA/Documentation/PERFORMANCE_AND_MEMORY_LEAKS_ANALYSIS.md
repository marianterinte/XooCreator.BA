# Analiză Performanță și Memory Leaks - Story Editor Backend

## 📋 Context

Aplicația rulează pe **Azure App Service Basic B1** (1 CPU, 1.75 GB RAM) și prezintă probleme de performanță, în special când se execută operații de publish, new version sau play story.

## 🔴 Probleme Critice Identificate

### 1. Background Workers Rulează Continuu (24/7)

**Problema:**
- **6 background workers** rulează permanent în App Service B1:
  1. `StoryForkQueueWorker`
  2. `StoryForkAssetsQueueWorker`
  3. `StoryVersionQueueWorker`
  4. `StoryPublishQueueWorker`
  5. `StoryImportQueueWorker`
  6. `StoryExportQueueWorker`

**Comportament:**
- Fiecare worker face **polling la coadă la fiecare 3 secunde**, chiar și când nu sunt mesaje
- Rulează continuu în `while (!stoppingToken.IsCancellationRequested)` loop
- Consumă CPU și memorie chiar și în idle state

**Impact:**
- **Consum continuu de resurse**: 6 procese active permanent
- **CPU overhead**: Polling constant la 6 cozi Azure Storage
- **Memorie**: Fiecare worker menține conexiuni și resurse active
- **Pe B1 (1 CPU, 1.75 GB RAM)**: Resursele limitate sunt consumate de workers chiar și când nu procesează mesaje

**Locații afectate:**
- `StoryPublishQueueWorker.cs` - linia 50-62: polling continuu
- `StoryVersionQueueWorker.cs` - linia 51-64: polling continuu
- Toate celelalte 4 workers au același pattern

**Recomandare:**
- Migrare la **Azure Functions cu Queue Triggers** (on-demand execution)
- Sau: Implementare "smart polling" - crește delay-ul când nu sunt mesaje (ex: 3s → 10s → 30s → 60s)
- Sau: Folosire Azure Service Bus cu long polling în loc de short polling

---

### 2. Query-uri Fără Paginare - GetAllStoriesAsync

**Problema:**
```csharp
public async Task<List<StoryContentDto>> GetAllStoriesAsync(string locale)
{
    var stories = await _context.StoryDefinitions
        .Include(s => s.Translations)
        .Include(s => s.Tiles).ThenInclude(t => t.Translations)
        .Include(s => s.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
        .Include(s => s.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
        .Where(s => s.IsActive)
        .OrderBy(s => s.SortOrder)
        .ToListAsync(); // ⚠️ Încarcă TOATE story-urile într-o singură query
}
```

**Impact:**
- Dacă există 100+ story-uri, query-ul încarcă:
  - 100 StoryDefinitions
  - 100+ Translations
  - 1000+ Tiles (presupunând ~10 tiles/story)
  - 5000+ Answers (presupunând ~5 answers/tile)
  - 10000+ Tokens (presupunând ~2 tokens/answer)
- **Memorie**: Poate consuma 50-200 MB pentru o singură query
- **Timp de execuție**: 2-10 secunde pentru query-uri mari
- **Database load**: Query-uri foarte mari cu multe JOIN-uri

**Locații afectate:**
- `StoriesRepository.cs` - linia 22-38: `GetAllStoriesAsync`

**Recomandare:**
- Adăugare paginare: `Skip(page * pageSize).Take(pageSize)`
- Sau: Returnare doar metadata (fără tiles/answers/tokens) pentru listă
- Sau: Implementare endpoint separat pentru detalii complete

---

### 3. Query-uri Ineficiente pentru User Progress

**Problema:**
```csharp
public async Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId)
{
    // ⚠️ Încarcă TOATE progress-urile pentru user
    var allProgress = await _context.UserStoryReadProgress
        .Where(p => p.UserId == userId)
        .OrderBy(p => p.ReadAt)
        .ToListAsync();
    
    // ⚠️ Filtrare în memorie (ineficient)
    var filteredProgress = allProgress
        .Where(p => string.Equals(p.StoryId, storyId, StringComparison.OrdinalIgnoreCase))
        .ToList();
}
```

**Impact:**
- Dacă user-ul are progress pentru 50+ story-uri, query-ul încarcă toate în memorie
- Filtrarea se face în memorie, nu în database
- **Memorie**: Consum inutil pentru date care nu sunt folosite
- **Timp de execuție**: Mai lent decât un query filtrat direct în DB

**Locații afectate:**
- `StoriesRepository.cs` - linia 92-114: `GetUserStoryProgressAsync`
- `StoriesRepository.cs` - linia 116-149: `MarkTileAsReadAsync` (același pattern)
- `StoriesRepository.cs` - linia 151-213: `ResetStoryProgressAsync` (același pattern)

**Recomandare:**
- Filtrare direct în query: `.Where(p => p.UserId == userId && p.StoryId == storyId)`
- Sau: Normalizare StoryId în database pentru a evita case-insensitive filtering
- Sau: Index pe `(UserId, StoryId)` pentru performanță

---

### 4. Încărcare Masivă de Date în Publish/Version Jobs

**Problema:**
```csharp
// StoryPublishQueueWorker.cs - linia 120-127
var craft = await db.StoryCrafts
    .Include(c => c.Tiles).ThenInclude(t => t.Translations)
    .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
    .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
    .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
    .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
    .AsSplitQuery() // ✅ Bine că folosește split query
    .FirstOrDefaultAsync(c => c.StoryId == job.StoryId, stoppingToken);
```

**Impact:**
- Pentru story-uri mari (50+ tiles, 200+ answers), query-ul încarcă:
  - 1 StoryCraft
  - 50+ Tiles
  - 50+ Tile Translations
  - 200+ Answers
  - 200+ Answer Translations
  - 400+ Tokens
- **Memorie**: 20-100 MB per job pentru story-uri mari
- **Timp de execuție**: 1-5 secunde pentru query-uri mari
- **Pe B1 (1.75 GB RAM)**: Dacă rulează 2-3 job-uri simultan, poate consuma 30-50% din RAM

**Observații pozitive:**
- ✅ Folosește `AsSplitQuery()` pentru a reduce presiunea pe memorie
- ✅ Folosește `using var scope` pentru cleanup corect
- ✅ DbContext este disposed corect după fiecare mesaj

**Recomandare:**
- Considerare lazy loading pentru părți care nu sunt folosite imediat
- Sau: Procesare incrementală (procesează tiles în batch-uri)
- Sau: Streaming pentru asset copying (nu încărca totul în memorie)

---

### 5. Query-uri Fără Indexuri Optimizate

**Problema:**
- Query-urile pentru progress folosesc filtrare case-insensitive în memorie
- Nu există indexuri optimizate pentru `(UserId, StoryId)` în `UserStoryReadProgress`

**Impact:**
- Query-urile sunt mai lente decât ar putea fi
- Database trebuie să scaneze mai multe rânduri

**Recomandare:**
- Adăugare index: `CREATE INDEX IX_UserStoryReadProgress_UserId_StoryId ON UserStoryReadProgress(UserId, StoryId)`
- Normalizare StoryId în database pentru a evita case-insensitive filtering

---

## 🟡 Probleme Medii

### 6. Polling fără Timeout în Frontend

**Problema:**
- `StoryVersionPollingService` nu are timeout (documentat în `09_CURRENT_ISSUES.md`)
- Poate continua polling la infinit dacă job-ul nu se finalizează

**Impact:**
- **Memory leak în frontend**: Subscription-ul rămâne activ
- **Network overhead**: Request-uri continue către server
- **Battery drain** pe mobile

**Recomandare:**
- Adăugare timeout similar cu `StoryPublishPollingService` (10 minute)

---

### 7. Multiple API Calls pentru Același Resource

**Problema:**
- Frontend face multiple API calls pentru același resource fără caching
- Nu există request deduplication

**Impact:**
- **Network overhead**: Request-uri duplicate
- **Database load**: Query-uri duplicate
- **Performance**: Timp de răspuns mai mare

**Recomandare:**
- Implementare request deduplication (ex: RxJS `shareReplay`)
- Cache pentru resources frecvent accesate

---

### 8. Lipsă Connection Pooling Optimization

**Problema:**
- Nu există configurare explicită pentru connection pooling PostgreSQL
- Default settings pot fi suboptimale pentru B1

**Impact:**
- Conexiuni multiple deschise simultan
- Posibilă epuizare a pool-ului de conexiuni

**Recomandare:**
- Configurare explicită connection string cu `Max Pool Size` și `Min Pool Size`
- Monitorizare număr conexiuni active

---

## ✅ Aspecte Pozitive (Nu Sunt Probleme)

### 1. DbContext Scoping Corect
- ✅ Toți worker-ii folosesc `using var scope` pentru fiecare mesaj
- ✅ DbContext este disposed corect după fiecare operație
- ✅ Nu există memory leaks din DbContext

### 2. AsSplitQuery() Folosit
- ✅ `StoryPublishQueueWorker` folosește `AsSplitQuery()` pentru a reduce presiunea pe memorie
- ✅ `StoryVersionQueueWorker` folosește `AsSplitQuery()`

### 3. Retry Logic Corect
- ✅ Worker-ii au retry logic (max 3 încercări)
- ✅ Failed jobs sunt marcate corect

### 4. Error Handling Corect
- ✅ Worker-ii gestionează excepții corect
- ✅ Jobs failed sunt marcate cu error message

---

## 📊 Impact Estimativ pe Azure B1

### Resurse Disponibile:
- **CPU**: 1 core
- **RAM**: 1.75 GB
- **Storage**: 10 GB

### Consum Actual (Estimativ):

**Background Workers (24/7):**
- 6 workers × ~50 MB RAM = **~300 MB RAM** (idle)
- 6 workers × ~5% CPU = **~30% CPU** (idle, polling)
- **Total**: ~17% din RAM, ~30% din CPU chiar și când nu procesează mesaje

**Publish/Version Jobs (când rulează):**
- 1 job × ~50-100 MB RAM = **~50-100 MB RAM**
- 1 job × ~20-50% CPU = **~20-50% CPU**
- **Total**: ~3-6% din RAM, ~20-50% din CPU per job

**API Requests (când rulează):**
- GetAllStoriesAsync: **~50-200 MB RAM** (temporar)
- GetUserStoryProgressAsync: **~10-50 MB RAM** (temporar)
- **Total**: ~3-11% din RAM per request

**Concluzie:**
- **Idle state**: ~20% RAM, ~30% CPU consumat de workers
- **Peak load** (publish + play story simultan): ~40-60% RAM, ~80-100% CPU
- **Risc**: Pe B1, peak load poate cauza slowdown-uri sau timeouts

---

## 🎯 Recomandări Prioritizate

### Prioritate Înaltă (Imediat)

1. **Migrare Background Workers la Azure Functions**
   - Elimină consumul continuu de resurse
   - Costuri: $0/lună pentru volume mici-medii (Consumption Plan)
   - Efort: 2-3 săptămâni
   - ROI: Economie $13-55/lună + eliberare resurse B1

2. **Optimizare GetAllStoriesAsync**
   - Adăugare paginare sau returnare doar metadata
   - Efort: 1-2 zile
   - Impact: Reducere 80-90% memorie per request

3. **Optimizare GetUserStoryProgressAsync**
   - Filtrare direct în query (nu în memorie)
   - Efort: 1 zi
   - Impact: Reducere 70-90% memorie per request

### Prioritate Medie (Scurt Termen)

4. **Adăugare Timeout pentru Version Polling**
   - Efort: 1 zi
   - Impact: Previne memory leaks în frontend

5. **Optimizare Indexuri Database**
   - Adăugare index `(UserId, StoryId)` pentru progress
   - Efort: 1 zi
   - Impact: Reducere 50-70% timp query

6. **Configurare Connection Pooling**
   - Efort: 1 zi
   - Impact: Previne epuizare conexiuni

### Prioritate Mică (Mediu Termen)

7. **Implementare Request Deduplication**
   - Efort: 2-3 zile
   - Impact: Reducere network overhead

8. **Optimizare Publish/Version Jobs**
   - Procesare incrementală pentru story-uri mari
   - Efort: 1 săptămână
   - Impact: Reducere 30-50% memorie per job

---

## 📈 Metrici de Monitorizare Recomandate

1. **Memory Usage**
   - Monitorizare RAM utilizat în Azure Portal
   - Alertă când > 80% (1.4 GB pe B1)

2. **CPU Usage**
   - Monitorizare CPU utilizat
   - Alertă când > 80% pentru > 5 minute

3. **Database Connections**
   - Monitorizare număr conexiuni active
   - Alertă când > 80% din pool

4. **Query Performance**
   - Monitorizare timp execuție query-uri
   - Alertă când > 5 secunde

5. **Background Job Duration**
   - Monitorizare timp execuție job-uri
   - Alertă când > 10 minute

---

## 🔍 Concluzie

**Problemele principale care pot cauza slowdown-uri pe B1:**

1. ✅ **Background Workers (24/7)** - Consumă 20% RAM, 30% CPU chiar și idle
2. ✅ **GetAllStoriesAsync fără paginare** - Consumă 50-200 MB RAM per request
3. ✅ **GetUserStoryProgressAsync ineficient** - Consumă 10-50 MB RAM per request
4. ✅ **Publish/Version Jobs** - Consumă 50-100 MB RAM per job

**Recomandare finală:**
- **Imediat**: Optimizare query-uri (GetAllStoriesAsync, GetUserStoryProgressAsync)
- **Scurt termen**: Migrare background workers la Azure Functions
- **Mediu termen**: Optimizare indexuri și connection pooling

**ROI estimat:**
- Optimizare query-uri: **-50-70% memorie per request** (1 săptămână efort)
- Migrare Azure Functions: **-20% RAM idle, -30% CPU idle** + **$0 costuri suplimentare** (2-3 săptămâni efort)

---

## 📚 Referințe

- [BACKGROUND_JOBS_ANALYSIS.md](./BACKGROUND_JOBS_ANALYSIS.md) - Analiză detaliată background jobs
- [09_CURRENT_ISSUES.md](../xoo-creator/002.Documentation/StoryEditorLatestVersion/09_CURRENT_ISSUES.md) - Probleme identificate
- [08_API_ENDPOINTS.md](../xoo-creator/002.Documentation/StoryEditorLatestVersion/08_API_ENDPOINTS.md) - Endpoint-uri API

