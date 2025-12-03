# Îmbunătățiri Logging și Monitoring

## Rezumat

Acest document descrie toate modificările de logging și monitoring implementate pentru a oferi vizibilitate completă asupra performanței aplicației și pentru a identifica bottleneck-uri în Azure Application Insights.

---

## 1. Configurare Application Insights

### 1.1. Adăugare Connection String

**Fișiere modificate:**
- `appsettings.Production.json`
- `appsettings.Development.json`

**Modificări:**
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=...;IngestionEndpoint=...;LiveEndpoint=...;ApplicationId=..."
  }
}
```

**Scop:** Permite trimiterea telemetriei către Azure Application Insights.

---

## 2. Îmbunătățire GlobalExceptionMiddleware

### 2.1. Tracking Explicit de Excepții

**Fișier:** `Infrastructure/Errors/GlobalExceptionMiddleware.cs`

**Modificări:**
- Adăugat `TelemetryClient` pentru tracking explicit de excepții
- Implementat `TrackException()` cu metadata detaliată:
  - TraceId
  - Layer (Repository, Service, Endpoint, etc.)
  - Operation
  - StatusCode
  - Path, Method, QueryString
- Adăugat `Flush()` pentru trimiterea imediată a excepțiilor

**Cod adăugat:**
```csharp
private readonly TelemetryClient? _telemetryClient;

// În HandleExceptionAsync:
if (_telemetryClient != null)
{
    var exceptionTelemetry = new ExceptionTelemetry(ex)
    {
        SeverityLevel = status >= 500 ? SeverityLevel.Error : SeverityLevel.Warning,
        Properties = { /* metadata detaliată */ }
    };
    _telemetryClient.TrackException(exceptionTelemetry);
    _telemetryClient.Flush();
}
```

**Beneficii:**
- Excepțiile sunt loggate explicit în Application Insights
- Metadata detaliată pentru debugging
- Severity level corect bazat pe status code

---

## 3. Logging Durata Query-uri la Baza de Date

### 3.1. Tracking în StoriesMarketplaceRepository

**Fișier:** `Features/TalesOfAlchimalia/Market/StoriesMarketplaceRepository.cs`

**Modificări:**
- Adăugat `ILogger` și `TelemetryClient` în constructor
- Implementat măsurare durată cu `Stopwatch` în `GetMarketplaceStoriesWithPaginationAsync()`
- Tracking ca Dependency Telemetry și Custom Metric

**Cod adăugat:**
```csharp
private readonly ILogger<StoriesMarketplaceRepository>? _logger;
private readonly TelemetryClient? _telemetryClient;

public async Task<(List<StoryMarketplaceItemDto> Stories, int TotalCount, bool HasMore)> 
    GetMarketplaceStoriesWithPaginationAsync(...)
{
    var stopwatch = Stopwatch.StartNew();
    try
    {
        // ... operațiuni DB ...
    }
    finally
    {
        stopwatch.Stop();
        var durationMs = stopwatch.ElapsedMilliseconds;
        
        // Logging ILogger
        _logger?.LogInformation("GetMarketplaceStoriesWithPaginationAsync completed | Duration={DurationMs}ms | ...");
        
        // Tracking Application Insights
        if (_telemetryClient != null)
        {
            // Dependency Telemetry
            var dependencyTelemetry = new DependencyTelemetry { /* ... */ };
            _telemetryClient.TrackDependency(dependencyTelemetry);
            
            // Custom Metric
            _telemetryClient.TrackMetric("MarketplaceStoriesQueryDuration", durationMs, properties);
        }
    }
}
```

**Metrici trackate:**
- `MarketplaceStoriesQueryDuration` - durata totală a query-urilor (ms)
- Dependency Telemetry cu tip "Database" și target "PostgreSQL"

**Metadata inclusă:**
- Page, PageSize
- Locale
- SortBy, SortOrder
- UserId

---

## 4. Configurare Npgsql Logging

### 4.1. Logging pentru Query-uri SQL

**Fișiere modificate:**
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`
- `Infrastructure/Configuration/DatabaseConfiguration.cs`

**Modificări în appsettings:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Npgsql": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**Modificări în DatabaseConfiguration.cs:**
```csharp
options.UseNpgsql(cs, npgsqlOptions =>
{
    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", dbSchema);
});

// Enable detailed EF Core command logging (includes SQL queries and execution time)
options.EnableSensitiveDataLogging(false);
options.EnableDetailedErrors();
```

**Beneficii:**
- Vezi exact ce query-uri SQL se execută
- Durata fiecărui query
- Erori detaliate din PostgreSQL

**Unde vezi logurile:**
- Application Insights → Logs → Traces
- Caută: `"Executing DbCommand"` sau `"Npgsql"`

---

## 5. Request Performance Middleware

### 5.1. Middleware pentru Tracking Performance

**Fișier nou:** `Infrastructure/Middleware/RequestPerformanceMiddleware.cs`

**Funcționalități:**
- Măsoară durata totală a fiecărui request
- Trackează memoria (Working Set, Memory Delta)
- Trackează colecțiile GC (Gen0, Gen1, Gen2)
- Trimite metrici în Application Insights

**Metrici trackate:**
1. **RequestDuration** - durata totală a request-ului (ms)
2. **ServerResponseTime** - timpul de procesare pe server (ms)
3. **WorkingSet** - memoria curentă folosită de proces (bytes)
4. **WorkingSetDelta** - diferența de memorie în timpul request-ului (bytes)
5. **MemoryDelta** - diferența de memorie GC (bytes)
6. **GC_Gen0_Collections** - numărul de colecții GC Gen0
7. **GC_Gen1_Collections** - numărul de colecții GC Gen1
8. **GC_Gen2_Collections** - numărul de colecții GC Gen2

**Implementare:**
```csharp
public class RequestPerformanceMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Capture initial stats
        var initialMemory = GC.GetTotalMemory(false);
        var initialGen0 = GC.CollectionCount(0);
        var initialGen1 = GC.CollectionCount(1);
        var initialGen2 = GC.CollectionCount(2);
        var initialWorkingSet = Environment.WorkingSet;
        
        try
        {
            await _next(context);
        }
        finally
        {
            // Calculate deltas and track metrics
            // ...
        }
    }
}
```

**Integrare în Program.cs:**
```csharp
app.UseRequestPerformanceTracking(); // Adăugat înainte de UseGlobalExceptionHandling
```

**Logging:**
- Toate metricile sunt loggate prin `ILogger` cu format structurat
- Trimise în Application Insights ca Custom Metrics

---

## 6. Endpoint-Specific Tracking

### 6.1. Tracking în GetMarketplaceStoriesEndpoint

**Fișier:** `Features/TalesOfAlchimalia/Market/Endpoints/GetMarketplaceStoriesEndpoint.cs`

**Modificări:**
- Adăugat `ILogger` și `TelemetryClient` în constructor
- Implementat măsurare durată cu `Stopwatch` în `HandleGetMarketplace()`
- Tracking ca Custom Metric

**Cod adăugat:**
```csharp
private readonly ILogger<GetMarketplaceStoriesEndpoint>? _logger;
private readonly TelemetryClient? _telemetryClient;

public static async Task<Ok<GetMarketplaceStoriesResponse>> HandleGetMarketplace(...)
{
    var endpointStopwatch = Stopwatch.StartNew();
    try
    {
        // ... logică endpoint ...
    }
    finally
    {
        endpointStopwatch.Stop();
        var endpointDuration = endpointStopwatch.ElapsedMilliseconds;
        
        // Logging și tracking
        ep._logger?.LogInformation("GetMarketplaceStoriesEndpoint completed | Duration={Duration}ms | ...");
        
        if (ep._telemetryClient != null)
        {
            ep._telemetryClient.TrackMetric("GetMarketplaceStoriesEndpoint_Duration", endpointDuration, properties);
        }
    }
}
```

**Metric trackat:**
- `GetMarketplaceStoriesEndpoint_Duration` - durata totală a endpoint-ului (ms)

**Metadata inclusă:**
- Endpoint name
- Locale
- Page, PageSize
- SortBy, SortOrder

---

## 7. Rezumat Metrici Trackate

### 7.1. Custom Metrics în Application Insights

| Metric Name | Descriere | Unitate |
|------------|-----------|---------|
| `RequestDuration` | Durata totală a request-ului | ms |
| `ServerResponseTime` | Timpul de procesare pe server | ms |
| `WorkingSet` | Memoria curentă folosită | bytes |
| `WorkingSetDelta` | Diferența de memorie în timpul request-ului | bytes |
| `MemoryDelta` | Diferența de memorie GC | bytes |
| `GC_Gen0_Collections` | Număr colecții GC Gen0 | count |
| `GC_Gen1_Collections` | Număr colecții GC Gen1 | count |
| `GC_Gen2_Collections` | Număr colecții GC Gen2 | count |
| `MarketplaceStoriesQueryDuration` | Durata query-urilor DB pentru marketplace | ms |
| `GetMarketplaceStoriesEndpoint_Duration` | Durata totală a endpoint-ului marketplace | ms |

### 7.2. Dependency Telemetry

- **Tip:** Database
- **Name:** GetMarketplaceStoriesWithPagination
- **Target:** PostgreSQL
- **Durata:** măsurată în milisecunde

### 7.3. Exception Telemetry

- Tracking explicit prin `TelemetryClient.TrackException()`
- Metadata completă: TraceId, Layer, Operation, StatusCode, Path, Method, QueryString
- Severity level bazat pe status code

---

## 8. Query-uri Utile în Application Insights

### 8.1. Durata Request-urilor

```kusto
// Durata medie a request-urilor pe endpoint
customMetrics
| where name == "RequestDuration"
| where customDimensions.Path contains "/tales-of-alchimalia/market"
| summarize avg(value), p95(value), p99(value) by bin(timestamp, 1h)
```

### 8.2. Durata Query-urilor DB

```kusto
// Durata query-urilor la baza de date
customMetrics
| where name == "MarketplaceStoriesQueryDuration"
| summarize avg(value), max(value), p95(value) by bin(timestamp, 1h)
```

### 8.3. Comparație Request vs DB Duration

```kusto
// Comparație între durata totală a request-ului și durata query-urilor DB
customMetrics
| where name in ("RequestDuration", "MarketplaceStoriesQueryDuration")
| where customDimensions.Path contains "/tales-of-alchimalia/market"
| summarize avg(value) by name, bin(timestamp, 1h)
```

### 8.4. Memorie și GC

```kusto
// Memoria folosită
customMetrics
| where name == "WorkingSet"
| summarize avg(value) by bin(timestamp, 1h)

// GC Collections
customMetrics
| where name startswith "GC_Gen"
| summarize sum(value) by name, bin(timestamp, 1h)
```

### 8.5. Query-uri SQL Loggate

```kusto
// Vezi query-urile SQL executate
traces
| where message contains "Executing DbCommand"
| project timestamp, message, customDimensions
| order by timestamp desc
```

### 8.6. Excepții

```kusto
// Excepții trackate
exceptions
| where customDimensions contains "Layer"
| project timestamp, type, message, customDimensions
| order by timestamp desc
```

---

## 9. Identificare Bottleneck-uri

### 9.1. Analiză Performanță

**Pași pentru identificare bottleneck-uri:**

1. **Compară RequestDuration cu MarketplaceStoriesQueryDuration**
   - Dacă `RequestDuration` >> `MarketplaceStoriesQueryDuration` → bottleneck în business logic
   - Dacă `MarketplaceStoriesQueryDuration` >> `RequestDuration` → bottleneck în DB

2. **Verifică query-urile SQL lente**
   - Caută în logs Npgsql query-uri cu durată mare
   - Verifică dacă există query-uri N+1 sau lipsă de indexuri

3. **Monitorizează memoria**
   - `WorkingSetDelta` mare → posibilă problemă de memorie
   - `GC_Gen2_Collections` frecvente → presiune pe GC, posibil memory leak

4. **Verifică endpoint duration**
   - Compară `GetMarketplaceStoriesEndpoint_Duration` cu `RequestDuration`
   - Diferență mare → overhead în middleware sau alte componente

### 9.2. Alerte Recomandate

1. **Alert pentru query-uri lente:**
   - `MarketplaceStoriesQueryDuration > 1000ms` (1 secundă)

2. **Alert pentru request-uri lente:**
   - `RequestDuration > 2000ms` (2 secunde)

3. **Alert pentru memorie:**
   - `WorkingSet > 500MB` (ajustabil în funcție de resurse)

4. **Alert pentru GC frecvent:**
   - `GC_Gen2_Collections > 0` în interval de 5 minute

---

## 10. Fișiere Modificate

### 10.1. Fișiere Noi Create

- `Infrastructure/Middleware/RequestPerformanceMiddleware.cs`
- `Documentation/LoggingImprovements.md` (acest fișier)

### 10.2. Fișiere Modificate

- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`
- `Program.cs`
- `Infrastructure/Configuration/DatabaseConfiguration.cs`
- `Infrastructure/Errors/GlobalExceptionMiddleware.cs`
- `Features/TalesOfAlchimalia/Market/StoriesMarketplaceRepository.cs`
- `Features/TalesOfAlchimalia/Market/Endpoints/GetMarketplaceStoriesEndpoint.cs`

---

## 11. Pași pentru Verificare

### 11.1. După Deploy

1. **Verifică Application Insights Connection String**
   - Asigură-te că connection string-ul este setat corect în Azure App Service

2. **Generează un request de test**
   - Fă un request la `/api/{locale}/tales-of-alchimalia/market`
   - Așteaptă 2-3 minute pentru propagarea datelor

3. **Verifică în Application Insights:**
   - **Metrics** → Custom Metrics → vezi toate metricile trackate
   - **Logs** → Traces → vezi logurile structurate
   - **Logs** → Dependencies → vezi dependency telemetry pentru DB
   - **Logs** → Exceptions → vezi excepțiile trackate

4. **Rulează query-urile KQL**
   - Folosește query-urile din secțiunea 8 pentru analiză

---

## 12. Note Tehnice

### 12.1. Performance Overhead

- Middleware-ul adaugă overhead minim (~1-2ms per request)
- Logging-ul Npgsql poate genera multe loguri - ajustează log level dacă e necesar
- `Flush()` pentru excepții asigură trimiterea imediată dar poate afecta performanța

### 12.2. Best Practices

- Nu loga date sensibile (passwords, tokens) - `EnableSensitiveDataLogging(false)`
- Folosește log levels corespunzătoare (Information pentru production, Debug pentru development)
- Monitorizează volumul de telemetrie trimis pentru a evita costuri mari

### 12.3. Optimizări Viitoare

- Adaugă sampling pentru a reduce volumul de telemetrie dacă e necesar
- Implementează custom TelemetryInitializer pentru adăugare metadata globală
- Configurează alerts în Azure pentru metrici critice
- Folosește Application Insights Profiler pentru analiză detaliată de performanță

---

## 13. Concluzie

Toate modificările de logging și monitoring au fost implementate cu succes. Aplicația acum oferă:

✅ **Vizibilitate completă** asupra performanței request-urilor  
✅ **Tracking detaliat** pentru query-uri DB  
✅ **Monitorizare memorie și GC**  
✅ **Logging SQL queries** pentru debugging  
✅ **Exception tracking** îmbunătățit  
✅ **Metrici custom** pentru analiză în Application Insights  

Aceste îmbunătățiri permit identificarea rapidă a bottleneck-urilor și optimizarea performanței aplicației.

