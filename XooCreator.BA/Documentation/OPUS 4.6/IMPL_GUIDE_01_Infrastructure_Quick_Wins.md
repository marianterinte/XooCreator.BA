# Ghid de Implementare 01 — Infrastructure Quick Wins

**Referință:** `PROBLEMS_SCAN_28.feb.2026.md`  
**Probleme acoperite:** INF-01 → INF-12  
**Efort estimat:** 1–2 zile  
**Impact:** Mare — aceste fix-uri afectează TOATE request-urile

---

## Task 1: Fix Auth0 Cache Invalidation (INF-02) ✅

### Fișier: `Infrastructure/Services/Auth0UserService.cs`

### Problema
La linia 72, `InvalidateUserCache(auth0Id)` este apelat ÎNAINTE de `EnsureAsync`. Asta face cache-ul inutil — la fiecare request autentificat se face un DB call.

### Modificare

**Înlocuiește** (liniile 70–73):
```csharp
// Ensure user exists and sync profile data
// Note: EnsureAsync may update user data, so we invalidate cache first to ensure fresh data
InvalidateUserCache(auth0Id);
_cachedUser = await _userRepository.EnsureAsync(auth0Id, name, email, picture, ct);
```

**Cu:**
```csharp
_cachedUser = await _userRepository.EnsureAsync(auth0Id, name, email, picture, ct);
```

### Explicație
`EnsureAsync` deja face upsert corect. Cache-ul ar trebui invalidat doar când user-ul e modificat din altă parte (ex: admin endpoint). Prin eliminarea `InvalidateUserCache`, cache-ul de 5 minute va funcționa corect, reducând DB calls cu ~80% pentru useri deja autentificați.

### Verificare
După modificare, monitorizează cu Application Insights că numărul de query-uri pe `AlchimaliaUsers` scade semnificativ.

---

## Task 2: Configurare Connection Pool PostgreSQL (INF-03)

### Fișier: `Infrastructure/Configuration/DatabaseConfiguration.cs`

### Problema
Connection string-ul este folosit direct fără `MinPoolSize`, `MaxPoolSize`, `CommandTimeout`. Default Npgsql pool = 100, prea mult pentru B1.

### Modificare

**Înlocuiește** metoda `BuildConnectionString` (liniile 87–111) cu:
```csharp
private static string BuildConnectionString(string value)
{
    NpgsqlConnectionStringBuilder npg;

    if (!value.Contains("://", StringComparison.Ordinal))
    {
        npg = new NpgsqlConnectionStringBuilder(value);
    }
    else if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
    {
        var userInfo = uri.UserInfo.Split(':', 2);
        npg = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = userInfo.ElementAtOrDefault(0) ?? "postgres",
            Password = userInfo.ElementAtOrDefault(1) ?? string.Empty,
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require
        };
    }
    else
    {
        return value;
    }

    // Performance settings for Azure B1
    if (npg.MinPoolSize == 0) npg.MinPoolSize = 2;
    if (npg.MaxPoolSize == 100) npg.MaxPoolSize = 20; // B1: keep low
    if (npg.CommandTimeout == 30) npg.CommandTimeout = 30; // explicit default
    if (npg.Timeout == 15) npg.Timeout = 15; // connection timeout
    npg.Pooling = true;

    return npg.ConnectionString;
}
```

### Explicație
Pe B1 cu 1 vCPU, 100 conexiuni simultane sunt imposibil de servit. 20 e un maxim realist. `MinPoolSize = 2` menține conexiuni calde pentru latență minimă.

---

## Task 3: Elimină `BuildServiceProvider()` din configurare (INF-04) ✅

### Fișier: `Infrastructure/Configuration/DatabaseConfiguration.cs`

### Problema
La linia 41, `services.BuildServiceProvider()` creează un service provider intermediar. Asta cauzează duplicate instances și warnings la compilare.

### Modificare

**Înlocuiește** (liniile 41–43):
```csharp
var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
var logger = loggerFactory?.CreateLogger<IdempotentMigrationCommandInterceptor>();
options.AddInterceptors(new IdempotentMigrationCommandInterceptor(dbSchema, logger));
```

**Cu:**
```csharp
options.AddInterceptors(new IdempotentMigrationCommandInterceptor(dbSchema, null));
```

### Explicație
Interceptor-ul poate funcționa fără logger. Logger-ul era doar pentru debug. Asta elimină service provider-ul duplicat și warning-urile asociate.

---

## Task 4: Adaugă Response Compression (INF-05) ✅

### Fișier: `Program.cs`

### Modificare 1 — Adaugă using-uri la începutul fișierului:
```csharp
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
```

### Modificare 2 — După `builder.Services.AddApplicationInsightsTelemetry();` (linia 22), adaugă:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json",
        "text/plain",
        "text/html",
        "application/javascript",
        "text/css"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
```

### Modificare 3 — Imediat după `var app = builder.Build();` (linia 97), adaugă:
```csharp
app.UseResponseCompression();
```

**IMPORTANT:** `UseResponseCompression()` trebuie apelat ÎNAINTE de orice alt middleware care scrie response body. Deci ÎNAINTE de `UseCors`, `UseRequestPerformanceTracking`, etc.

### Explicație
JSON responses pot fi comprimate cu 60-80%. Pe un B1 cu bandwidth limitat, asta reduce latența percepută semnificativ.

---

## Task 5: MemoryCache cu limită (INF-07)

### Fișier: `Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`

### Modificare

**Înlocuiește** (linia 47):
```csharp
services.AddMemoryCache(); // Shared in-process cache (users, marketplace, universe, etc.)
```

**Cu:**
```csharp
services.AddMemoryCache(options =>
{
    options.SizeLimit = 2048;
    options.CompactionPercentage = 0.25;
});
```

### ATENȚIE — Side effect
După această modificare, TOATE adăugările în cache TREBUIE să specifice `Size`. Caută în codebase toate locurile unde se apelează `_cache.Set(...)` sau `_cache.GetOrCreate(...)` și asigură-te că fiecare `MemoryCacheEntryOptions` are `Size = 1` (sau o valoare proporțională cu dimensiunea obiectului).

**Locuri de verificat:**
- `Auth0UserService.cs` linia 78 — adaugă `Size = 1`
- `MemoryAppCache.cs` — verifică `Set` method
- `MarketplaceCatalogCache.cs` — verifică cache entries
- Orice alt loc care folosește `IMemoryCache` direct

### Exemplu de adăugare `Size`:
```csharp
var cacheOptions = new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = CacheExpiration,
    SlidingExpiration = TimeSpan.FromMinutes(2),
    Priority = CacheItemPriority.Normal,
    Size = 1 // <-- ADAUGĂ ASTA
};
```

---

## Task 6: BlobSasService — Elimină `CreateIfNotExistsAsync` repetat (INF-08) ✅

### Fișier: `Infrastructure/Services/Blob/BlobSasService.cs`

### Modificare

Adaugă un `HashSet` pentru container-uri deja verificate și verifică doar o dată:

**Înlocuiește** clasa completă cu:
```csharp
public class BlobSasService : IBlobSasService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly HashSet<string> _verifiedContainers = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _containerLock = new(1, 1);
    public string DraftContainer { get; }
    public string PublishedContainer { get; }

    public BlobSasService(IConfiguration configuration)
    {
        var section = configuration.GetSection("AzureStorage");
        var conn = ResolveConfiguredValue(section["ConnectionString"])
                   ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
        if (string.IsNullOrWhiteSpace(conn))
        {
            throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
        }

        DraftContainer = ResolveConfiguredValue(section["DraftContainer"])
                         ?? Environment.GetEnvironmentVariable("AzureStorage__DraftContainer")
                         ?? "alchimalia-drafts";
        PublishedContainer = ResolveConfiguredValue(section["PublishedContainer"])
                             ?? Environment.GetEnvironmentVariable("AzureStorage__PublishedContainer")
                             ?? "alchimaliacontent";

        _blobServiceClient = new BlobServiceClient(conn);
    }

    public BlobClient GetBlobClient(string container, string blobPath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        return containerClient.GetBlobClient(blobPath);
    }

    public BlobContainerClient GetContainerClient(string container)
        => _blobServiceClient.GetBlobContainerClient(container);

    private async Task EnsureContainerAsync(string container, CancellationToken ct)
    {
        if (_verifiedContainers.Contains(container)) return;

        await _containerLock.WaitAsync(ct);
        try
        {
            if (_verifiedContainers.Contains(container)) return;
            await _blobServiceClient.GetBlobContainerClient(container)
                .CreateIfNotExistsAsync(cancellationToken: ct);
            _verifiedContainers.Add(container);
        }
        finally
        {
            _containerLock.Release();
        }
    }

    public async Task<Uri> GetPutSasAsync(string container, string blobPath, string contentType, TimeSpan ttl, CancellationToken ct = default)
    {
        await EnsureContainerAsync(container, ct);
        var blobClient = GetBlobClient(container, blobPath);

        var permissions = BlobSasPermissions.Create | BlobSasPermissions.Write;
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sasBuilder.ContentType = contentType;
        sasBuilder.SetPermissions(permissions);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri;
    }

    public Task<Uri> GetReadSasAsync(string container, string blobPath, TimeSpan ttl, CancellationToken ct = default)
    {
        var blobClient = GetBlobClient(container, blobPath);
        var permissions = BlobSasPermissions.Read;
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sasBuilder.SetPermissions(permissions);
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return Task.FromResult(sasUri);
    }

    private static string? ResolveConfiguredValue(string? configured)
    {
        if (string.IsNullOrWhiteSpace(configured)) return null;
        if (configured.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
        {
            var envKey = configured[4..].Trim();
            return string.IsNullOrWhiteSpace(envKey) ? null : Environment.GetEnvironmentVariable(envKey);
        }
        return configured;
    }
}
```

---

## Task 7: RequestPerformanceMiddleware — Sampling pentru GC stats (INF-11) ✅

### Fișier: `Infrastructure/Middleware/RequestPerformanceMiddleware.cs`

### Modificare

**Înlocuiește** (liniile 53–58):
```csharp
// Capture initial memory and GC stats
var initialMemory = GC.GetTotalMemory(false);
var initialGen0 = GC.CollectionCount(0);
var initialGen1 = GC.CollectionCount(1);
var initialGen2 = GC.CollectionCount(2);
var initialWorkingSet = Environment.WorkingSet;
```

**Cu:**
```csharp
var shouldTrackMemory = _isDevelopment || Random.Shared.Next(100) == 0; // 1% sampling in production
long initialMemory = 0, initialWorkingSet = 0;
int initialGen0 = 0, initialGen1 = 0, initialGen2 = 0;
if (shouldTrackMemory)
{
    initialMemory = GC.GetTotalMemory(false);
    initialGen0 = GC.CollectionCount(0);
    initialGen1 = GC.CollectionCount(1);
    initialGen2 = GC.CollectionCount(2);
    initialWorkingSet = Environment.WorkingSet;
}
```

Și **înlocuiește** (liniile 71–82):
```csharp
// Capture final memory and GC stats
var finalMemory = GC.GetTotalMemory(false);
var finalGen0 = GC.CollectionCount(0);
var finalGen1 = GC.CollectionCount(1);
var finalGen2 = GC.CollectionCount(2);
var finalWorkingSet = Environment.WorkingSet;

var memoryDelta = finalMemory - initialMemory;
var gen0Collections = finalGen0 - initialGen0;
var gen1Collections = finalGen1 - initialGen1;
var gen2Collections = finalGen2 - initialGen2;
var workingSetDelta = finalWorkingSet - initialWorkingSet;
```

**Cu:**
```csharp
long memoryDelta = 0, workingSetDelta = 0;
int gen0Collections = 0, gen1Collections = 0, gen2Collections = 0;
if (shouldTrackMemory)
{
    var finalMemory = GC.GetTotalMemory(false);
    var finalGen0 = GC.CollectionCount(0);
    var finalGen1 = GC.CollectionCount(1);
    var finalGen2 = GC.CollectionCount(2);
    var finalWorkingSet = Environment.WorkingSet;
    memoryDelta = finalMemory - initialMemory;
    gen0Collections = finalGen0 - initialGen0;
    gen1Collections = finalGen1 - initialGen1;
    gen2Collections = finalGen2 - initialGen2;
    workingSetDelta = finalWorkingSet - initialWorkingSet;
}
```

---

## Task 8: Global NoTracking Behavior (INF-12) ✅

### Fișier: `Infrastructure/Configuration/DatabaseConfiguration.cs`

### Modificare

**Adaugă** după `options.EnableDetailedErrors();` (linia 36):
```csharp
options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
```

### ATENȚIE — Side effect
Asta face ca TOATE query-urile să fie NoTracking by default. Toate locurile care fac update/insert/delete trebuie să folosească `.AsTracking()` explicit.

**Pattern de căutat:** Toate metodele care apelează `SaveChanges()` sau `SaveChangesAsync()` — entity-urile loadate în acele metode trebuie să aibă `.AsTracking()` sau să fie atașate cu `_context.Attach(entity)`.

**RECOMANDARE:** Implementează acest task DOAR după ce ai verificat toate repository-urile care fac update. Alternativ, poți sări acest task și adăuga `.AsNoTracking()` manual pe fiecare query read-only (mai sigur, dar mai laborios).

---

## Ordinea de implementare recomandată

1. **Task 1** (Auth0 cache) — cel mai simplu, cel mai mare impact
2. **Task 2** (Connection pool) — simplu, stabilitate
3. **Task 3** (BuildServiceProvider) — simplu, cleanup
4. **Task 4** (Response compression) — impact vizibil pentru useri
5. **Task 6** (BlobSas) — simplu, reduce calls
6. **Task 7** (GC sampling) — simplu, reduce overhead
7. **Task 5** (MemoryCache limit) — necesită verificare în mai multe fișiere
8. **Task 8** (Global NoTracking) — cel mai riscant, necesită audit complet

---

*Ghid generat pe baza analizei din PROBLEMS_SCAN_28.feb.2026.md*
