# Alternative pentru Notificare Job Completion - Analiză Comparativă

## 📋 Context

**Situația actuală:**
- Frontend face **polling** la fiecare 5-10 secunde pentru status job
- **18 polling services** identificate în frontend:
  1. `StoryPublishPollingService` - Polling la fiecare 10s, timeout 20 minute
  2. `StoryVersionPollingService` - Polling la fiecare 5s, **FĂRĂ TIMEOUT** ⚠️
  3. `StoryImportJobPollingService` - Polling pentru import jobs
  4. `StoryExportJobPollingService` - Polling pentru export jobs
  5. `StoryForkJobPollingService` - Polling pentru fork jobs
  6. `StoryDocumentExportJobPollingService` - Polling pentru document export
  7. `EpicPublishPollingService` - Polling pentru epic publish
  8. `EpicVersionPollingService` - Polling pentru epic version
  9. `RegionVersionPollingService` - Polling pentru region version
  10. `HeroVersionPollingService` - Polling pentru hero version
  11. + alte servicii similare

- Fiecare polling continuă până când job-ul se termină (sau timeout)
- **Probleme identificate:**
  - **Network overhead**: 12-120 request-uri per job (5-10s interval × 1-10 minute)
  - **Latency**: până la 5-10 secunde între finalizare și notificare
  - **Server load**: request-uri constante chiar și când nu se întâmplă nimic
  - **Memory leaks**: subscription-uri care pot rămâne active la infinit (ex: `StoryVersionPollingService` fără timeout)
  - **Azure B1 impact**: Request-uri constante consumă CPU și memorie pe planul limitat

**Arhitectura actuală:**

**Backend:**
- **6 Background Workers** rulează 24/7:
  1. `StoryPublishQueueWorker` - Procesează publish jobs
  2. `StoryVersionQueueWorker` - Procesează version jobs
  3. `StoryImportQueueWorker` - Procesează import jobs
  4. `StoryExportQueueWorker` - Procesează export jobs
  5. `StoryForkQueueWorker` - Procesează fork jobs
  6. `StoryForkAssetsQueueWorker` - Procesează fork assets jobs

- **Job Entities** stocate în DB:
  - `StoryPublishJob` - Status: Queued, Running, Completed, Failed, Superseded
  - `StoryVersionJob` - Status: Queued, Running, Completed, Failed, Superseded
  - `StoryExportJob` - Status: Queued, Running, Completed, Failed
  - `StoryImportJob` - Status: Queued, Running, Completed, Failed
  - `StoryForkJob` - Status: Queued, Running, Completed, Failed
  - `EpicPublishJob` - Status: Queued, Running, Completed, Failed, Superseded
  - `EpicVersionJob` - Status: Queued, Running, Completed, Failed, Superseded

- **Endpoints de status:**
  - `GET /api/stories/{storyId}/publish-jobs/{jobId}` - Returnează status publish job
  - `GET /api/stories/{storyId}/version-jobs/{jobId}` - Returnează status version job
  - `GET /api/stories/{storyId}/export-jobs/{jobId}` - Returnează status export job
  - Similar pentru import, fork, epic, etc.

- **Workers actualizează job status în DB:**
  - Când job începe: `Status = Running`, `StartedAtUtc = DateTime.UtcNow`
  - Când job se termină: `Status = Completed/Failed`, `CompletedAtUtc = DateTime.UtcNow`

**Frontend:**
- Fiecare polling service face request la endpoint-ul de status
- Polling continuă până când `status === 'Completed' || status === 'Failed'`
- Unele servicii au timeout (ex: `StoryPublishPollingService` - 20 minute)
- Altele **NU au timeout** (ex: `StoryVersionPollingService`) ⚠️

**Obiectiv:**
- Notificare instantanee când job-ul se termină (< 1s latency)
- Reducere network overhead cu 90% (1 conexiune vs 12-120 request-uri)
- Reducere server load (eliminare request-uri constante)
- Scalabilitate pentru Azure B1 (1 CPU, 1.75 GB RAM)
- Eliminare memory leaks (timeout garantat pentru toate polling services)

---

## 🔄 Alternative Analizate

### 1. **Server-Sent Events (SSE)** ⭐ RECOMANDAT

## 📚 Ce este SSE și Cum Funcționează?

### **Definiție:**
**Server-Sent Events (SSE)** este un standard web (HTML5) care permite serverului să trimită evenimente către client printr-o conexiune HTTP persistentă unidirecțională. Este parte din specifiicația HTML5 și este suportat nativ de toate browserele moderne.

### **Cum Funcționează la Nivel Tehnic:**

**1. Protocol HTTP:**
- SSE folosește **HTTP/1.1 standard** (nu necesită upgrade la HTTP/2 sau WebSocket)
- Conexiunea este o **request HTTP GET normală** care rămâne deschisă
- Serverul trimite răspunsul cu header-ul `Content-Type: text/event-stream`
- Răspunsul este un **stream continuu** (nu se închide imediat)

**2. Format Mesaje:**
Serverul trimite mesaje în format text simplu:
```
data: {"jobId":"123","status":"Running"}

data: {"jobId":"123","status":"Completed"}

```
- Fiecare mesaj începe cu `data: ` urmat de conținut
- Liniile goale (`\n\n`) separă mesajele
- Browser-ul parsează automat și declanșează evenimentul `message`

**3. Flux de Comunicare:**
```
Client (Browser)                    Server (ASP.NET Core)
     |                                    |
     |--- GET /api/jobs/123/events ----->|
     |                                    |
     |<-- HTTP 200 OK                    |
     |<-- Content-Type: text/event-stream|
     |<-- Connection: keep-alive          |
     |                                    |
     |<-- data: {"status":"Running"}      |
     |                                    |
     |<-- data: {"status":"Completed"}    |
     |                                    |
     |--- (Connection closes)             |
```

**4. Caracteristici:**
- ✅ **Unidirecțional**: Doar server → client (perfect pentru notificări)
- ✅ **Auto-reconnect**: Browser-ul reîncearcă automat dacă conexiunea se pierde
- ✅ **HTTP standard**: Funcționează prin proxy-uri, firewall-uri, CDN-uri
- ✅ **Simplicitate**: Nu necesită protocol special (ca WebSocket)

---

### **Configurații Necesare:**

#### **1. Azure App Service - Configurații:**

**✅ NU sunt necesare configurații speciale!**

SSE funcționează out-of-the-box pe Azure App Service pentru că:
- Folosește HTTP/1.1 standard
- Nu necesită WebSocket support
- Nu necesită upgrade la plan mai mare
- Funcționează pe **Basic B1** (planul tău actual)

**⚠️ Considerații Azure:**
- **Request Timeout**: Default 230 secunde - OK pentru SSE (job-urile se termină în < 5 minute)
- **Connection Limits**: B1 suportă ~100-200 conexiuni simultane - OK pentru use case-ul tău
- **Load Balancer**: Azure Load Balancer suportă SSE fără configurații speciale

**Configurare opțională (dacă ai probleme):**
```json
// appsettings.json - Nu este necesar, dar poți seta:
{
  "Kestrel": {
    "Limits": {
      "KeepAliveTimeout": "00:10:00",  // 10 minute keep-alive
      "RequestHeadersTimeout": "00:00:30"
    }
  }
}
```

#### **2. Backend (ASP.NET Core) - Configurații:**

**✅ Configurații minime necesare:**

**a) Nu necesită package-uri suplimentare:**
- SSE este suportat nativ în ASP.NET Core
- Nu necesită `Microsoft.AspNetCore.SignalR` sau alte package-uri

**b) Headers necesare în răspuns:**
```csharp
response.Headers.Add("Content-Type", "text/event-stream");
response.Headers.Add("Cache-Control", "no-cache");
response.Headers.Add("Connection", "keep-alive");
```

**c) CORS (dacă frontend-ul este pe alt domeniu):**
```csharp
// În Program.cs - dacă ai nevoie de CORS pentru SSE
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSSE", policy =>
    {
        policy.WithOrigins("https://your-frontend.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Important pentru SSE cu auth
    });
});

// În endpoint SSE:
app.UseCors("AllowSSE");
```

**d) Authentication/Authorization:**
```csharp
// SSE endpoint-ul poate folosi același auth ca restul API-ului
[Authorize] // Funcționează normal cu JWT/Auth0
public static async Task HandleSSE(...)
{
    // Auth check se face înainte de deschiderea stream-ului
}
```

#### **3. Frontend (Browser) - Configurații:**

**✅ NU sunt necesare configurații speciale!**

**a) Browser Support:**
- ✅ Chrome/Edge: Suport complet
- ✅ Firefox: Suport complet
- ✅ Safari: Suport complet
- ✅ Opera: Suport complet
- ❌ Internet Explorer: Nu suportă (dar nu mai este relevant)

**b) JavaScript API:**
```typescript
// Browser-ul oferă API nativ - nu necesită biblioteci externe
const eventSource = new EventSource('/api/jobs/123/events');

eventSource.onmessage = (event) => {
  const data = JSON.parse(event.data);
  console.log('Status update:', data);
};

eventSource.onerror = (error) => {
  console.error('SSE error:', error);
  // Browser-ul reîncearcă automat
};
```

**c) Authentication cu Cookies:**
```typescript
// Dacă folosești cookies pentru auth (ex: Auth0)
const eventSource = new EventSource('/api/jobs/123/events', {
  withCredentials: true // Include cookies în request
});
```

**d) Authentication cu Headers (limitat):**
```typescript
// ⚠️ EventSource NU suportă custom headers direct!
// Soluție: Folosește query parameter sau cookie pentru auth

// Opțiunea 1: Token în query parameter
const eventSource = new EventSource(`/api/jobs/123/events?token=${authToken}`);

// Opțiunea 2: Token în cookie (recomandat)
// Setează cookie înainte de crearea EventSource
document.cookie = `authToken=${authToken}; path=/`;
const eventSource = new EventSource('/api/jobs/123/events', {
  withCredentials: true
});
```

---

### **Cum Funcționează în Detalii:**

#### **1. Inițializare Conexiune:**

**Client (Browser):**
```typescript
// Browser-ul face un request HTTP GET normal
const eventSource = new EventSource('/api/jobs/123/events');
```

**Server (ASP.NET Core):**
```csharp
// Serverul primește request-ul ca un request HTTP normal
[Route("/api/jobs/{jobId}/events")]
public static async Task HandleSSE(string jobId, HttpContext context, CancellationToken ct)
{
    var response = context.Response;
    
    // Setează headers pentru SSE
    response.ContentType = "text/event-stream";
    response.Headers.Add("Cache-Control", "no-cache");
    response.Headers.Add("Connection", "keep-alive");
    
    // Request-ul rămâne deschis - serverul nu trimite răspuns final
    // Stream-ul rămâne activ până când:
    // - Client-ul închide conexiunea
    // - Serverul închide conexiunea
    // - Timeout (default 230s în Azure)
}
```

#### **2. Trimitere Mesaje:**

**Server:**
```csharp
// Serverul trimite mesaje în format SSE
await response.WriteAsync("data: {\"status\":\"Running\"}\n\n");
await response.Body.FlushAsync(); // Important: flush pentru a trimite imediat

// Browser-ul primește mesajul și declanșează eventSource.onmessage
```

**Client:**
```typescript
eventSource.onmessage = (event) => {
  // event.data conține mesajul trimis de server
  const status = JSON.parse(event.data);
  console.log('Status:', status.status); // "Running"
};
```

#### **3. Keep-Alive (Prevenire Timeout):**

**Problema:** Unele proxy-uri/firewall-uri închid conexiunile inactive după 30-60s

**Soluție:** Trimite comentarii periodice pentru a menține conexiunea activă:
```csharp
// La fiecare 30 secunde, trimite un comentariu (nu declanșează onmessage)
while (!ct.IsCancellationRequested)
{
    // Trimite mesaj de date sau comentariu keep-alive
    await response.WriteAsync(": keep-alive\n\n"); // Comentariu (nu declanșează event)
    await response.Body.FlushAsync();
    
    await Task.Delay(30000, ct); // La fiecare 30s
}
```

#### **4. Închidere Conexiune:**

**Server:**
```csharp
// Serverul închide conexiunea când:
// - Job-ul se termină
if (job.Status == "Completed")
{
    await response.WriteAsync("data: {\"status\":\"Completed\"}\n\n");
    await response.Body.FlushAsync();
    // Response se închide automat când metoda se termină
}

// - Sau când client-ul se deconectează (CancellationToken)
```

**Client:**
```typescript
// Client-ul închide conexiunea:
eventSource.close();

// Sau automat când:
// - Pagina se închide
// - Browser-ul se închide
// - Navigare la altă pagină
```

#### **5. Auto-Reconnect:**

**Browser-ul reîncearcă automat dacă:**
- Conexiunea se pierde (network error)
- Serverul închide conexiunea
- Timeout

**Comportament:**
```typescript
eventSource.onerror = (error) => {
  // Browser-ul reîncearcă automat după ~3 secunde
  // Nu trebuie să faci nimic - este gestionat automat
};

// Dacă vrei să controlezi reconnect-ul:
let reconnectAttempts = 0;
const maxReconnectAttempts = 5;

eventSource.onerror = (error) => {
  reconnectAttempts++;
  if (reconnectAttempts > maxReconnectAttempts) {
    eventSource.close();
    // Handle max reconnects exceeded
  }
};
```

---

### **Limitări și Considerații:**

#### **1. Limitări Browser:**
- **Conexiuni simultane per domeniu**: ~6 conexiuni (limita HTTP/1.1)
  - ✅ OK pentru use case-ul tău (1-2 job-uri active per user)
  - ⚠️ Dacă ai nevoie de > 6 conexiuni → folosește HTTP/2 sau WebSocket

#### **2. Limitări Azure:**
- **Request Timeout**: 230 secunde (default)
  - ✅ OK pentru job-uri care durează < 5 minute
  - ⚠️ Dacă job-urile durează > 5 minute → implementează reconnect logic

#### **3. Limitări Proxy/Firewall:**
- Unele proxy-uri închid conexiunile inactive după 30-60s
  - ✅ Soluție: Keep-alive cu comentarii periodice
  - ✅ Azure App Service nu are această problemă

#### **4. Limitări CORS:**
- Dacă frontend-ul este pe alt domeniu, necesită CORS configurat
  - ✅ Soluție: Configurează CORS în `Program.cs`

---

### **Comparație cu Alte Tehnologii:**

| Aspect | SSE | WebSocket | Long Polling | Short Polling |
|-------|-----|-----------|--------------|---------------|
| **Protocol** | HTTP/1.1 | WS/WSS | HTTP/1.1 | HTTP/1.1 |
| **Direcție** | Unidirecțional | Bidirecțional | Unidirecțional | Unidirecțional |
| **Conexiune** | Persistentă | Persistentă | Temporară | Temporară |
| **Overhead** | Minimal | Mediu | Mediu | Mare |
| **Complexitate** | Simplu | Mediu | Simplu | Simplu |
| **Browser Support** | Excelent | Excelent | Excelent | Excelent |
| **Azure Config** | Nu necesită | Poate necesita | Nu necesită | Nu necesită |
| **Cost** | $0 | $0-$55/lună | $0 | $0 |

---

### **Exemplu Complet de Implementare:**

**Backend (ASP.NET Core):**
```csharp
[Route("/api/jobs/{jobType}/{jobId}/events")]
[Authorize]
public static async Task HandleSSE(
    string jobType,
    Guid jobId,
    HttpContext context,
    XooDbContext db,
    CancellationToken ct)
{
    var response = context.Response;
    
    // Setează headers SSE
    response.ContentType = "text/event-stream";
    response.Headers.Add("Cache-Control", "no-cache");
    response.Headers.Add("Connection", "keep-alive");
    
    // Trimite mesaj de conectare
    await response.WriteAsync("data: {\"type\":\"connected\"}\n\n", ct);
    await response.Body.FlushAsync(ct);
    
    var lastStatus = "";
    var lastKeepAlive = DateTime.UtcNow;
    
    while (!ct.IsCancellationRequested)
    {
        // Verifică status job din DB
        var job = await GetJobByTypeAsync(db, jobType, jobId, ct);
        
        if (job == null)
        {
            await response.WriteAsync("data: {\"type\":\"error\",\"message\":\"Job not found\"}\n\n", ct);
            break;
        }
        
        // Trimite update doar dacă status-ul s-a schimbat
        if (job.Status != lastStatus)
        {
            var statusJson = JsonSerializer.Serialize(new {
                jobId = job.Id,
                status = job.Status,
                completedAt = job.CompletedAtUtc
            });
            
            await response.WriteAsync($"data: {statusJson}\n\n", ct);
            await response.Body.FlushAsync(ct);
            
            lastStatus = job.Status;
            
            // Închide dacă job-ul s-a terminat
            if (job.Status == "Completed" || job.Status == "Failed")
            {
                break;
            }
        }
        
        // Keep-alive la fiecare 30s
        if (DateTime.UtcNow - lastKeepAlive > TimeSpan.FromSeconds(30))
        {
            await response.WriteAsync(": keep-alive\n\n", ct);
            await response.Body.FlushAsync(ct);
            lastKeepAlive = DateTime.UtcNow;
        }
        
        // Așteaptă 1 secundă înainte de următoarea verificare
        await Task.Delay(1000, ct);
    }
}
```

**Frontend (TypeScript/Angular):**
```typescript
@Injectable({ providedIn: 'root' })
export class JobNotificationService {
  subscribeToJob(
    jobType: string,
    jobId: string,
    onStatusUpdate: (status: any) => void
  ): () => void {
    const url = `/api/jobs/${jobType}/${jobId}/events`;
    const eventSource = new EventSource(url, {
      withCredentials: true // Include auth cookies
    });

    eventSource.onmessage = (event) => {
      const data = JSON.parse(event.data);
      
      if (data.type === 'connected') {
        console.log('SSE connected');
        return;
      }
      
      onStatusUpdate(data);
      
      if (data.status === 'Completed' || data.status === 'Failed') {
        eventSource.close();
      }
    };

    eventSource.onerror = (error) => {
      console.error('SSE error:', error);
      eventSource.close();
    };

    // Return cleanup function
    return () => eventSource.close();
  }
}
```

---

### **Concluzie:**

**SSE este perfect pentru use case-ul tău pentru că:**
- ✅ **Nu necesită configurații speciale în Azure** - Funcționează out-of-the-box
- ✅ **Nu necesită package-uri suplimentare** - Suportat nativ în ASP.NET Core
- ✅ **Nu necesită biblioteci externe în frontend** - API nativ în browser
- ✅ **Simplu de implementat** - Doar headers HTTP și format text simplu
- ✅ **Compatibil cu Azure B1** - Funcționează perfect pe planul tău actual
- ✅ **Performanță excelentă** - Latency < 1s, overhead minimal

**Singurele configurații necesare:**
1. Headers SSE în răspuns (Content-Type, Cache-Control, Connection)
2. CORS (dacă frontend-ul este pe alt domeniu)
3. Authentication (folosește același mecanism ca restul API-ului)

**Cum funcționează:**
- Conexiune HTTP persistentă unidirecțională
- Serverul trimite mesaje în format text simplu
- Browser-ul parsează automat și declanșează evenimente
- Auto-reconnect în caz de eroare
- Funcționează prin proxy-uri, firewall-uri, CDN-uri

---

**Cum funcționează:**
- Conexiune HTTP persistentă unidirecțională (server → client)
- Server trimite evenimente când job-ul se termină
- Client ascultă pe un endpoint `/api/jobs/{jobId}/events`

**Implementare Backend:**

```csharp
// 1. Service pentru gestionare conexiuni SSE active
public interface IJobNotificationService
{
    void RegisterConnection(string jobId, string connectionId, HttpResponse response);
    void UnregisterConnection(string jobId, string connectionId);
    Task NotifyJobCompleted(string jobId, JobStatus status);
}

public class JobNotificationService : IJobNotificationService
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, HttpResponse>> _connections = new();
    private readonly ILogger<JobNotificationService> _logger;

    public void RegisterConnection(string jobId, string connectionId, HttpResponse response)
    {
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");
        
        var connections = _connections.GetOrAdd(jobId, _ => new ConcurrentDictionary<string, HttpResponse>());
        connections.TryAdd(connectionId, response);
        
        _logger.LogInformation("SSE connection registered: jobId={JobId}, connectionId={ConnectionId}", jobId, connectionId);
    }

    public void UnregisterConnection(string jobId, string connectionId)
    {
        if (_connections.TryGetValue(jobId, out var connections))
        {
            connections.TryRemove(connectionId, out _);
            if (connections.IsEmpty)
            {
                _connections.TryRemove(jobId, out _);
            }
        }
    }

    public async Task NotifyJobCompleted(string jobId, JobStatus status)
    {
        if (_connections.TryGetValue(jobId, out var connections))
        {
            var message = $"data: {JsonSerializer.Serialize(status)}\n\n";
            foreach (var (connectionId, response) in connections)
            {
                try
                {
                    await response.WriteAsync(message);
                    await response.Body.FlushAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send SSE message: jobId={JobId}, connectionId={ConnectionId}", jobId, connectionId);
                    UnregisterConnection(jobId, connectionId);
                }
            }
        }
    }
}

// 2. Endpoint SSE
[Route("/api/jobs/{jobType}/{jobId}/events")]
[Authorize]
public static async Task HandleSSE(
    [FromRoute] string jobType, // "publish", "version", "export", etc.
    [FromRoute] Guid jobId,
    [FromServices] IJobNotificationService notificationService,
    [FromServices] XooDbContext db,
    HttpContext context,
    CancellationToken ct)
{
    var connectionId = Guid.NewGuid().ToString();
    var response = context.Response;
    
    notificationService.RegisterConnection($"{jobType}-{jobId}", connectionId, response);
    
    try
    {
        // Send initial connection message
        await response.WriteAsync("data: {\"type\":\"connected\"}\n\n", ct);
        await response.Body.FlushAsync(ct);
        
        // Keep connection alive with periodic heartbeats
        while (!ct.IsCancellationRequested)
        {
            // Check job status from DB
            var job = await GetJobByTypeAsync(db, jobType, jobId, ct);
            
            if (job == null)
            {
                await response.WriteAsync("data: {\"type\":\"error\",\"message\":\"Job not found\"}\n\n", ct);
                break;
            }
            
            var status = new { 
                jobId = job.Id, 
                status = job.Status,
                completedAt = job.CompletedAtUtc 
            };
            
            await response.WriteAsync($"data: {JsonSerializer.Serialize(status)}\n\n", ct);
            await response.Body.FlushAsync(ct);
            
            // If job completed, close connection
            if (job.Status == "Completed" || job.Status == "Failed" || job.Status == "Superseded")
            {
                break;
            }
            
            // Wait 1 second before next check
            await Task.Delay(1000, ct);
        }
    }
    finally
    {
        notificationService.UnregisterConnection($"{jobType}-{jobId}", connectionId);
    }
}

// 3. Worker integrare - Notificare când job se termină
// În StoryPublishQueueWorker, după ce actualizează job.Status = Completed:
await _jobNotificationService.NotifyJobCompleted($"publish-{job.Id}", new JobStatus 
{ 
    JobId = job.Id, 
    Status = job.Status,
    CompletedAtUtc = job.CompletedAtUtc 
});
```

**Implementare Frontend:**

```typescript
// 1. Service generic pentru SSE
@Injectable({ providedIn: 'root' })
export class JobNotificationService {
  private eventSources = new Map<string, EventSource>();

  /**
   * Subscribe to job status updates via SSE
   * @param jobType Type of job (publish, version, export, etc.)
   * @param jobId Job ID
   * @param onStatusUpdate Callback when status updates
   * @returns Function to unsubscribe
   */
  subscribeToJob(
    jobType: string,
    jobId: string,
    onStatusUpdate: (status: JobStatus) => void
  ): () => void {
    const key = `${jobType}-${jobId}`;
    
    // Close existing connection if any
    if (this.eventSources.has(key)) {
      this.eventSources.get(key)?.close();
    }

    const url = `/api/jobs/${jobType}/${jobId}/events`;
    const eventSource = new EventSource(url, {
      withCredentials: true // Include auth cookies
    });

    eventSource.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        
        if (data.type === 'connected') {
          console.log(`[SSE] Connected to job: ${jobType}-${jobId}`);
          return;
        }

        const status: JobStatus = {
          jobId: data.jobId,
          status: data.status,
          completedAt: data.completedAt ? new Date(data.completedAt) : undefined
        };

        onStatusUpdate(status);

        // Close connection if job completed
        if (status.status === 'Completed' || status.status === 'Failed' || status.status === 'Superseded') {
          eventSource.close();
          this.eventSources.delete(key);
        }
      } catch (error) {
        console.error('[SSE] Failed to parse message:', error);
      }
    };

    eventSource.onerror = (error) => {
      console.error('[SSE] Connection error:', error);
      eventSource.close();
      this.eventSources.delete(key);
    };

    this.eventSources.set(key, eventSource);

    // Return unsubscribe function
    return () => {
      eventSource.close();
      this.eventSources.delete(key);
    };
  }
}

// 2. Înlocuire polling service cu SSE
// În StoryVersionPollingService (sau orice alt polling service):
@Injectable({ providedIn: 'root' })
export class StoryVersionPollingService {
  constructor(
    private editorService: StoryEditorService,
    private jobNotificationService: JobNotificationService
  ) {}

  /**
   * Subscribe to version job status via SSE (new implementation)
   */
  subscribeToJobStatus(
    storyId: string,
    jobId: string
  ): Observable<VersionJobStatus> {
    return new Observable(observer => {
      const unsubscribe = this.jobNotificationService.subscribeToJob(
        'version',
        jobId,
        (status) => {
          // Map to VersionJobStatus format
          const versionStatus: VersionJobStatus = {
            jobId: status.jobId,
            storyId: storyId,
            status: status.status as VersionJobStatusType,
            queuedAtUtc: status.queuedAtUtc || new Date().toISOString(),
            completedAtUtc: status.completedAt?.toISOString(),
            errorMessage: status.errorMessage,
            dequeueCount: status.dequeueCount || 0,
            baseVersion: status.baseVersion || 0
          };
          
          observer.next(versionStatus);

          // Complete observable when job finished
          if (status.status === 'Completed' || status.status === 'Failed' || status.status === 'Superseded') {
            observer.complete();
          }
        }
      );

      // Cleanup on unsubscribe
      return () => unsubscribe();
    });
  }

  /**
   * Legacy polling method (kept for backward compatibility during migration)
   */
  pollJobStatus(storyId: string, jobId: string): Observable<VersionJobStatus> {
    // ... existing polling implementation
  }
}
```

**Avantaje:**
- ✅ **Performanță excelentă**: Latency < 1s (vs 5-10s polling)
- ✅ **Network overhead redus**: 1 conexiune persistentă vs 12-120 request-uri
- ✅ **Server load redus**: 1 conexiune vs polling constant
- ✅ **Implementare simplă**: Nu necesită biblioteci externe
- ✅ **Compatibil Azure B1**: Funcționează pe App Service standard
- ✅ **Auto-reconnect**: Browser-ul reîncearcă automat
- ✅ **HTTP/1.1 standard**: Nu necesită upgrade infrastructură

**Dezavantaje:**
- ⚠️ **Unidirecțional**: Doar server → client (OK pentru notificări)
- ⚠️ **Conexiuni limitate**: ~6 per browser (OK pentru use case-ul nostru)
- ⚠️ **Timeout proxy**: Unele proxy-uri timeout după 30-60s (necesită keep-alive)

**Cost:**
- **$0** - Folosește conexiuni HTTP existente
- **Impact Azure B1**: Minimal (1 conexiune per job activ)

**Efort implementare:**
- Backend: 1-2 zile (endpoint SSE + integrare cu workers)
- Frontend: 1 zi (înlocuire polling services)
- **Total: 2-3 zile**

**Scalabilitate:**
- ✅ Suportă 100+ conexiuni simultane pe B1
- ✅ Fiecare conexiune consumă ~1-2 MB RAM
- ✅ Ideal pentru 10-50 job-uri active simultan

---

### 2. **WebSockets** 

**Cum funcționează:**
- Conexiune bidirecțională persistentă
- Protocol WebSocket (ws:// sau wss://)
- Server poate trimite mesaje oricând

**Implementare:**
```csharp
// Backend - SignalR Hub
public class JobNotificationHub : Hub
{
    public async Task SubscribeToJob(string jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"job-{jobId}");
    }
}

// Worker - Notificare când job se termină
await _hubContext.Clients.Group($"job-{jobId}").SendAsync("JobCompleted", status);
```

```typescript
// Frontend - SignalR Client
const connection = new signalR.HubConnectionBuilder()
  .withUrl('/jobNotifications')
  .build();

await connection.invoke('SubscribeToJob', jobId);
connection.on('JobCompleted', (status) => {
  // Handle completion
});
```

**Avantaje:**
- ✅ **Performanță excelentă**: Latency < 100ms
- ✅ **Bidirecțional**: Poate trimite și primi mesaje
- ✅ **Real-time**: Notificare instantanee
- ✅ **Scalabil**: SignalR gestionează conexiunile automat

**Dezavantaje:**
- ⚠️ **Complexitate**: Necesită SignalR sau WebSocket server
- ⚠️ **Azure SignalR Service**: Cost suplimentar ($0.55/lună pentru Free tier, $55/lună pentru Standard)
- ⚠️ **Overhead**: Protocol mai complex decât SSE
- ⚠️ **Azure B1**: Poate necesita upgrade la plan mai mare pentru WebSocket support

**Cost:**
- **Opțiunea 1 - SignalR local**: $0 (dar consumă resurse B1)
- **Opțiunea 2 - Azure SignalR Service**: $0.55-$55/lună (după tier)
- **Impact Azure B1**: Mediu (WebSocket connections consumă mai mult decât SSE)

**Efort implementare:**
- Backend: 2-3 zile (SignalR setup + hub + integrare workers)
- Frontend: 1-2 zile (SignalR client)
- **Total: 3-5 zile**

**Scalabilitate:**
- ✅ Excelent cu Azure SignalR Service
- ⚠️ Limită pe B1 fără Azure SignalR (50-100 conexiuni)

---

### 3. **Long Polling**

**Cum funcționează:**
- Client face request care rămâne deschis până când job se termină
- Server ține request-ul deschis și răspunde când job se termină
- Client face un nou request imediat după primirea răspunsului

**Implementare:**
```csharp
// Backend - Long Polling Endpoint
app.MapGet("/api/jobs/{jobId}/wait", async (string jobId) =>
{
    var timeout = TimeSpan.FromSeconds(30);
    var cts = new CancellationTokenSource(timeout);
    
    // Wait for job completion or timeout
    while (!cts.IsCancellationRequested)
    {
        var status = await GetJobStatus(jobId);
        if (status.IsCompleted)
        {
            return Results.Ok(status);
        }
        await Task.Delay(1000, cts.Token);
    }
    
    return Results.Ok(new { status: "Running", timeout: true });
});
```

```typescript
// Frontend - Long Polling Client
async function waitForJob(jobId: string) {
  while (true) {
    const response = await fetch(`/api/jobs/${jobId}/wait`);
    const status = await response.json();
    
    if (status.status === 'Completed' || status.status === 'Failed') {
      return status;
    }
    // Continue waiting
  }
}
```

**Avantaje:**
- ✅ **Simplu**: Doar HTTP standard
- ✅ **Compatibil**: Funcționează peste tot
- ✅ **Fără upgrade**: Nu necesită infrastructură nouă

**Dezavantaje:**
- ⚠️ **Network overhead**: Request-uri repetate (deși mai puține decât short polling)
- ⚠️ **Server load**: Request-uri blocate pe server
- ⚠️ **Timeout handling**: Complex pentru timeout-uri și reconectare
- ⚠️ **Latency**: Până la 30s (timeout interval)

**Cost:**
- **$0** - Folosește HTTP existent
- **Impact Azure B1**: Mediu (request-uri blocate consumă thread-uri)

**Efort implementare:**
- Backend: 1 zi (endpoint long polling)
- Frontend: 1 zi (client long polling)
- **Total: 2 zile**

**Scalabilitate:**
- ⚠️ Limită: Request-uri blocate consumă thread-uri (problema pe B1 cu 1 CPU)

---

### 4. **Azure Service Bus Topics/Subscriptions**

**Cum funcționează:**
- Worker publică eveniment când job se termină pe Service Bus Topic
- Frontend face polling la Service Bus Subscription (sau folosește WebHook)
- Alternativ: Frontend se abonează la Service Bus prin backend proxy

**Implementare:**
```csharp
// Worker - Publică eveniment
await _serviceBusClient.CreateSender("job-completed-topic")
    .SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(status)));

// Backend - Proxy endpoint care face polling la Service Bus
app.MapGet("/api/jobs/{jobId}/status", async (string jobId) =>
{
    // Check Service Bus subscription for job completion event
    var receiver = await _serviceBusClient.CreateReceiver("job-completed-subscription");
    var message = await receiver.ReceiveMessageAsync();
    // Process and return
});
```

**Avantaje:**
- ✅ **Scalabil**: Service Bus gestionează milioane de mesaje
- ✅ **Reliable**: Delivery garantat
- ✅ **Decoupled**: Workers și frontend complet decuplați

**Dezavantaje:**
- ⚠️ **Cost**: $0.05 per milion mesaje + $0.05 per milion operations
- ⚠️ **Complexitate**: Necesită setup Service Bus + subscriptions
- ⚠️ **Latency**: Poate fi mai mare (Service Bus overhead)
- ⚠️ **Overkill**: Prea complex pentru use case-ul nostru

**Cost:**
- **$0.05-$5/lună** (după volum mesaje)
- **Impact Azure B1**: Minimal (doar pentru workers)

**Efort implementare:**
- Backend: 3-5 zile (Service Bus setup + integrare)
- Frontend: 1-2 zile (polling sau WebHook)
- **Total: 4-7 zile**

**Scalabilitate:**
- ✅ Excelent pentru volume mari
- ⚠️ Overkill pentru 10-50 job-uri/lună

---

### 5. **Azure Event Grid + WebHooks**

**Cum funcționează:**
- Worker publică eveniment pe Event Grid când job se termină
- Event Grid trimite WebHook către frontend (sau backend proxy)
- Frontend primește notificare prin WebHook

**Implementare:**
```csharp
// Worker - Publică eveniment
await _eventGridClient.PublishEventsAsync(new EventGridEvent[]
{
    new EventGridEvent("job-completed", "JobCompleted", "1.0", status)
});

// Backend - WebHook endpoint
app.MapPost("/api/webhooks/job-completed", async (JobStatus status) =>
{
    // Notify connected clients via SSE/WebSocket
    await NotifyClients(status);
});
```

**Avantaje:**
- ✅ **Scalabil**: Event Grid gestionează milioane de evenimente
- ✅ **Reliable**: Delivery garantat
- ✅ **Decoupled**: Workers și frontend complet decuplați

**Dezavantaje:**
- ⚠️ **Cost**: $0.60 per milion evenimente
- ⚠️ **Complexitate**: Necesită setup Event Grid + WebHook handling
- ⚠️ **Latency**: Poate fi mai mare (Event Grid overhead)
- ⚠️ **Overkill**: Prea complex pentru use case-ul nostru

**Cost:**
- **$0.60-$6/lună** (după volum evenimente)
- **Impact Azure B1**: Minimal

**Efort implementare:**
- Backend: 3-5 zile (Event Grid setup + WebHook handling)
- Frontend: 1-2 zile (WebHook receiver sau polling)
- **Total: 4-7 zile**

**Scalabilitate:**
- ✅ Excelent pentru volume mari
- ⚠️ Overkill pentru 10-50 job-uri/lună

---

### 6. **Polling Îmbunătățit (Exponential Backoff)**

**Cum funcționează:**
- Polling cu interval care crește exponențial (5s → 10s → 20s → 40s)
- Reducere overhead când job-ul durează mult
- Timeout garantat

**Implementare:**
```typescript
// Frontend - Exponential Backoff Polling
let interval = 5000; // Start with 5s
const maxInterval = 60000; // Max 60s

const poll = async () => {
  const status = await getJobStatus(jobId);
  if (status.status === 'Completed' || status.status === 'Failed') {
    return status;
  }
  
  interval = Math.min(interval * 1.5, maxInterval); // Exponential backoff
  setTimeout(poll, interval);
};
```

**Avantaje:**
- ✅ **Simplu**: Nu necesită modificări backend
- ✅ **Îmbunătățire**: Reducere 50-70% network overhead vs polling fix
- ✅ **Fără upgrade**: Funcționează cu infrastructura existentă

**Dezavantaje:**
- ⚠️ **Încă polling**: Network overhead rămâne (doar redus)
- ⚠️ **Latency**: Poate fi până la 60s între finalizare și notificare
- ⚠️ **Server load**: Request-uri continue (doar mai puține)

**Cost:**
- **$0** - Fără costuri suplimentare
- **Impact Azure B1**: Redus (mai puține request-uri)

**Efort implementare:**
- Backend: 0 zile (fără modificări)
- Frontend: 0.5 zi (modificare polling services)
- **Total: 0.5 zi**

**Scalabilitate:**
- ✅ OK pentru 10-50 job-uri active
- ⚠️ Limită: Request-uri continue (doar mai puține)

---

## 📊 Comparație Finală

| Soluție | Latency | Network Overhead | Server Load | Cost | Efort | Scalabilitate | Recomandare |
|---------|---------|------------------|-------------|------|-------|---------------|-------------|
| **SSE** | < 1s | 1 conexiune | Minimal | $0 | 2-3 zile | ⭐⭐⭐⭐⭐ | ⭐ **RECOMANDAT** |
| **WebSockets/SignalR** | < 100ms | 1 conexiune | Mediu | $0-$55/lună | 3-5 zile | ⭐⭐⭐⭐ | ⭐⭐ Dacă ai nevoie de bidirecțional |
| **Long Polling** | 1-30s | Request-uri repetate | Mediu | $0 | 2 zile | ⭐⭐⭐ | ⚠️ OK dar nu ideal |
| **Service Bus** | 1-5s | Minimal | Minimal | $0.05-$5/lună | 4-7 zile | ⭐⭐⭐⭐⭐ | ⚠️ Overkill |
| **Event Grid** | 1-5s | Minimal | Minimal | $0.60-$6/lună | 4-7 zile | ⭐⭐⭐⭐⭐ | ⚠️ Overkill |
| **Polling Îmbunătățit** | 5-60s | Redus 50-70% | Redus | $0 | 0.5 zi | ⭐⭐⭐ | ⚠️ Quick fix |

---

## 🎯 Recomandare Finală

### **Opțiunea 1: Server-Sent Events (SSE)** ⭐ RECOMANDAT

**De ce:**
- ✅ **Performanță excelentă**: Latency < 1s
- ✅ **Network overhead minim**: 1 conexiune vs 12-120 request-uri
- ✅ **Server load minim**: 1 conexiune persistentă
- ✅ **Cost $0**: Fără servicii suplimentare
- ✅ **Implementare simplă**: 2-3 zile
- ✅ **Compatibil Azure B1**: Funcționează perfect pe planul actual
- ✅ **Scalabil**: Suportă 100+ conexiuni simultane

**Implementare:**
1. Backend: Endpoint SSE `/api/jobs/{jobId}/events` care trimite update-uri
2. Worker: Când job se termină, verifică conexiunile active și trimite eveniment
3. Frontend: Înlocuire polling services cu `EventSource`

**Efort:** 2-3 zile
**ROI:** Reducere 90% network overhead + latency < 1s

---

### **Opțiunea 2: Polling Îmbunătățit (Quick Fix)**

**De ce:**
- ✅ **Implementare rapidă**: 0.5 zi
- ✅ **Îmbunătățire imediată**: Reducere 50-70% overhead
- ✅ **Fără risc**: Nu schimbă arhitectura

**Implementare:**
1. Frontend: Modificare polling services cu exponential backoff
2. Timeout garantat pentru toate polling services

**Efort:** 0.5 zi
**ROI:** Reducere 50-70% network overhead (dar încă polling)

---

### **Opțiunea 3: WebSockets/SignalR (Dacă ai nevoie de bidirecțional)**

**De ce:**
- ✅ **Performanță excelentă**: Latency < 100ms
- ✅ **Bidirecțional**: Poate trimite și primi mesaje
- ⚠️ **Complexitate**: Mai complex decât SSE
- ⚠️ **Cost**: Poate necesita Azure SignalR Service ($0.55-$55/lună)

**Când să folosești:**
- Dacă ai nevoie de comunicare bidirecțională (ex: chat, colaborare real-time)
- Dacă ai volume foarte mari (1000+ conexiuni simultane)

**Efort:** 3-5 zile
**ROI:** Performanță maximă, dar overhead mai mare

---

## 🚀 Plan de Implementare Recomandat

### **Faza 1: Quick Win - Polling Îmbunătățit (0.5 zi)**

**Obiectiv:** Reducere imediată a network overhead cu 50-70%

**Pași:**
1. **Adăugare timeout pentru toate polling services:**
   - `StoryVersionPollingService` - Adăugare `MAX_POLL_ATTEMPTS = 240` (20 minute la 5s interval)
   - `EpicVersionPollingService` - Adăugare timeout similar
   - `RegionVersionPollingService` - Adăugare timeout similar
   - `HeroVersionPollingService` - Adăugare timeout similar
   - Verificare toate celelalte polling services pentru timeout

2. **Implementare exponential backoff:**
   ```typescript
   // Exemplu pentru StoryVersionPollingService
   private readonly INITIAL_POLL_INTERVAL_MS = 5000;
   private readonly MAX_POLL_INTERVAL_MS = 60000;
   private readonly BACKOFF_MULTIPLIER = 1.5;
   private readonly MAX_POLL_ATTEMPTS = 240; // 20 minute max

   pollJobStatus(storyId: string, jobId: string): Observable<VersionJobStatus> {
     let attemptCount = 0;
     let currentInterval = this.INITIAL_POLL_INTERVAL_MS;

     return timer(0, currentInterval).pipe(
       switchMap(() => {
         attemptCount++;
         
         // Timeout check
         if (attemptCount > this.MAX_POLL_ATTEMPTS) {
           return of({
             jobId,
             storyId,
             status: 'Failed' as VersionJobStatusType,
             queuedAtUtc: new Date().toISOString(),
             errorMessage: 'Polling timeout: Job did not complete within expected time',
             dequeueCount: 0,
             baseVersion: 0
           });
         }

         // Exponential backoff
         currentInterval = Math.min(
           currentInterval * this.BACKOFF_MULTIPLIER,
           this.MAX_POLL_INTERVAL_MS
         );

         return this.editorService.getVersionJobStatus(storyId, jobId).pipe(
           catchError(err => {
             // ... error handling
           })
         );
       }),
       takeWhile(status => {
         return status.status === 'Queued' || status.status === 'Running';
       }, true)
     );
   }
   ```

3. **Testare:**
   - Verificare că timeout funcționează corect
   - Verificare că exponential backoff reduce numărul de request-uri
   - Monitorizare network overhead în DevTools

**Impact:**
- ✅ Reducere 50-70% network overhead
- ✅ Eliminare memory leaks (timeout garantat)
- ✅ Fără modificări backend necesare

---

### **Faza 2: Optimizare - Server-Sent Events (2-3 zile)**

**Obiectiv:** Reducere 90% network overhead + latency < 1s

**Pași Backend (1-2 zile):**

1. **Creare `IJobNotificationService`:**
   - Service pentru gestionare conexiuni SSE active
   - Metode: `RegisterConnection`, `UnregisterConnection`, `NotifyJobCompleted`
   - Storage: `ConcurrentDictionary<string, ConcurrentDictionary<string, HttpResponse>>`

2. **Creare endpoint SSE generic:**
   - `GET /api/jobs/{jobType}/{jobId}/events`
   - Suport pentru: publish, version, export, import, fork, epic-publish, epic-version
   - Authorization check
   - Keep-alive cu heartbeats
   - Auto-close când job se termină

3. **Integrare în workers:**
   - În `StoryPublishQueueWorker`: Notificare când `Status = Completed/Failed`
   - În `StoryVersionQueueWorker`: Notificare când `Status = Completed/Failed`
   - Similar pentru toți workers

4. **Testare backend:**
   - Test endpoint SSE cu `curl` sau Postman
   - Verificare că notificările sunt trimise corect
   - Verificare cleanup conexiuni

**Pași Frontend (1 zi):**

1. **Creare `JobNotificationService`:**
   - Service generic pentru SSE
   - Metodă `subscribeToJob(jobType, jobId, callback)`
   - Gestionare auto-reconnect
   - Cleanup conexiuni

2. **Migrare polling services:**
   - `StoryVersionPollingService` - Adăugare metodă `subscribeToJobStatus()` cu SSE
   - `StoryPublishPollingService` - Adăugare metodă `subscribeToJobStatus()` cu SSE
   - Similar pentru toate polling services
   - Păstrare metode legacy pentru backward compatibility

3. **Actualizare componente:**
   - Înlocuire `pollJobStatus()` cu `subscribeToJobStatus()` în componente
   - Testare fiecare tip de job (publish, version, export, etc.)

4. **Testare frontend:**
   - Verificare că SSE funcționează corect
   - Verificare că notificările sunt primite instant
   - Verificare cleanup conexiuni

**Impact:**
- ✅ Reducere 90% network overhead (1 conexiune vs 12-120 request-uri)
- ✅ Latency < 1s (vs 5-10s polling)
- ✅ Server load minim (1 conexiune persistentă)

---

### **Faza 3 (Opțional): Scalare**

**Când să implementezi:**
- Dacă ai nevoie de 1000+ conexiuni simultane
- Dacă ai nevoie de comunicare bidirecțională
- Dacă Azure B1 nu mai suportă numărul de conexiuni

**Opțiuni:**
1. **Azure SignalR Service:**
   - Migrare de la SSE la SignalR
   - Setup Azure SignalR Service (Free tier: $0.55/lună, Standard: $55/lună)
   - Implementare SignalR Hub
   - Migrare frontend la SignalR client

2. **Upgrade Azure App Service:**
   - Upgrade de la B1 la S1 sau P1V2
   - Mai multe resurse pentru conexiuni SSE
   - Cost: $55-$150/lună

---

## 📊 Metrici și Măsurători

### **Metrici de Performanță**

**Înainte de optimizare (Polling):**
- Network requests per job: 12-120 (5-10s interval × 1-10 minute)
- Latency: 5-10 secunde (până la următorul poll)
- Server load: Request-uri constante chiar și când nu se întâmplă nimic
- Memory: Subscription-uri care pot rămâne active la infinit

**După Faza 1 (Polling Îmbunătățit):**
- Network requests per job: 6-40 (exponential backoff)
- Latency: 5-60 secunde (exponential backoff)
- Server load: Redus 50-70%
- Memory: Timeout garantat, fără memory leaks

**După Faza 2 (SSE):**
- Network requests per job: 1 conexiune persistentă
- Latency: < 1 secundă (notificare instantanee)
- Server load: Minimal (1 conexiune per job activ)
- Memory: Cleanup automat când job se termină

### **Metrici de Monitorizare**

**Azure App Service Metrics:**
- **HTTP Requests/sec**: Ar trebui să scadă cu 50-90% după implementare
- **Response Time**: Ar trebui să scadă (mai puține request-uri = mai puțin load)
- **CPU Usage**: Ar trebui să scadă (mai puține request-uri = mai puțin CPU)
- **Memory Usage**: Ar trebui să scadă (eliminare memory leaks)

**Application Insights:**
- **Custom Metric**: Număr de conexiuni SSE active
- **Custom Metric**: Timp mediu între job completion și notificare
- **Custom Metric**: Număr de request-uri polling vs SSE connections

**Frontend Metrics:**
- **Network Tab**: Număr de request-uri per job (înainte vs după)
- **Performance Tab**: Latency între job completion și UI update
- **Memory Tab**: Verificare memory leaks (subscription-uri care rămân active)

---

## ⚠️ Considerații Azure B1

**Azure App Service B1 Limite:**
- **CPU**: 1 core
- **RAM**: 1.75 GB
- **Concurrent Connections**: ~100-200 (depinde de utilizare)
- **Request Timeout**: 230 secunde (default)

**Impact SSE pe B1:**
- ✅ **1 conexiune SSE** consumă ~1-2 MB RAM
- ✅ **100 conexiuni simultane** = ~100-200 MB RAM (OK pentru B1)
- ✅ **CPU overhead**: Minimal (doar pentru keep-alive)
- ✅ **Network**: 1 conexiune persistentă vs 12-120 request-uri

**Recomandări:**
- ✅ SSE este perfect pentru B1 (suportă 100+ conexiuni)
- ⚠️ Monitorizare conexiuni active (alertă dacă > 150)
- ⚠️ Implementare cleanup agresiv pentru conexiuni inactive
- ⚠️ Considerare upgrade la S1 dacă ai nevoie de > 200 conexiuni simultane

---

## 🔧 Troubleshooting

### **Probleme comune SSE:**

1. **Conexiune se închide prematur:**
   - **Cauză**: Proxy timeout (30-60s)
   - **Soluție**: Implementare keep-alive cu heartbeats la fiecare 30s

2. **Notificări nu sunt primite:**
   - **Cauză**: Worker nu notifică service-ul
   - **Soluție**: Verificare integrare worker cu `IJobNotificationService`

3. **Memory leak (conexiuni nu se închid):**
   - **Cauză**: Cleanup nu este apelat
   - **Soluție**: Verificare `finally` block în endpoint SSE

4. **CORS issues:**
   - **Cauză**: Headers CORS lipsă
   - **Soluție**: Adăugare CORS headers în endpoint SSE

---

## 📝 Checklist Implementare

### **Faza 1 - Polling Îmbunătățit:**
- [ ] Adăugare timeout pentru `StoryVersionPollingService`
- [ ] Adăugare timeout pentru `EpicVersionPollingService`
- [ ] Adăugare timeout pentru `RegionVersionPollingService`
- [ ] Adăugare timeout pentru `HeroVersionPollingService`
- [ ] Verificare toate celelalte polling services
- [ ] Implementare exponential backoff pentru toate
- [ ] Testare timeout funcționează
- [ ] Monitorizare network overhead

### **Faza 2 - SSE:**
- [ ] Creare `IJobNotificationService`
- [ ] Implementare `JobNotificationService`
- [ ] Creare endpoint SSE generic
- [ ] Integrare în `StoryPublishQueueWorker`
- [ ] Integrare în `StoryVersionQueueWorker`
- [ ] Integrare în toți workers
- [ ] Creare `JobNotificationService` în frontend
- [ ] Migrare `StoryVersionPollingService` la SSE
- [ ] Migrare `StoryPublishPollingService` la SSE
- [ ] Migrare toate polling services la SSE
- [ ] Actualizare componente să folosească SSE
- [ ] Testare end-to-end
- [ ] Monitorizare metrici

---

## 📚 Resurse

- **Server-Sent Events MDN**: https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events
- **ASP.NET Core SSE**: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/request-response?view=aspnetcore-8.0#streaming
- **Azure App Service Limits**: https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans
- **SignalR vs SSE**: https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-8.0

---

## 📝 Concluzie

**Pentru use case-ul tău (10-50 job-uri active, Azure B1):**

1. **SSE este cea mai bună opțiune** - Performanță excelentă, cost $0, implementare simplă
2. **Polling îmbunătățit** - Quick fix dacă vrei ceva rapid
3. **WebSockets/SignalR** - Doar dacă ai nevoie de bidirecțional sau volume foarte mari

**Recomandare:** Implementează SSE pentru notificări job completion. Este perfect pentru use case-ul tău și oferă cel mai bun ROI.

