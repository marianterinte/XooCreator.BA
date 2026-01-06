# Recomandare: Notificare Job Completion (Quick Win + SSE)

## TL;DR ✅
- **Acum (cel mai ușor, risc minim):** repară și uniformizează polling-ul (timeout peste tot + exponential backoff + cleanup).  
- **Apoi (upgrade „serios”, încă simplu):** treci la **SSE (Server-Sent Events)**: 1 conexiune deschisă per job, cu update-uri push din server, în loc de request-uri repetitive.

Documentul de analiză pe care l-am studiat indică clar problema curentă: **polling la 5–10s**, **~18 polling services**, unele **fără timeout** (risk de memory leaks), plus impact pe **Azure B1**. Pe baza acestor constrângeri, recomand un plan în două faze: **Quick Fix** (Polling îmbunătățit) → **SSE**.

---

## Contextul problemei (pe scurt)
### Ce se întâmplă acum
- UI face polling la interval fix pentru status-ul job-urilor.
- Există multe implementări „paralele” de polling (servicii separate), ceea ce duce la:
  - request-uri constante chiar când nu se schimbă nimic
  - risc de leak dacă o subscripție rămâne activă (mai ales fără timeout)
  - încărcare inutilă pe un plan restrâns (B1)

### Ce vrei, de fapt
- **UI să afle rapid** când un job s-a terminat.
- **Fără furtună de request-uri**.
- **Fără leak-uri**.
- **Cu implementare cât mai simplă**.

---

## Opțiuni (pe înțeles)
### 1) Polling îmbunătățit (cea mai ușoară variantă)
**Idee:** rămâi pe polling, dar îl faci civilizat:
- **timeout** obligatoriu pentru orice poll
- **exponential backoff** (când job-ul nu se schimbă, crești intervalul)
- **cleanup** garantat (unsubscribe / abort / destroy)

✅ Pro: rapid, risc minim, nu schimbă backend-ul  
⚠️ Contra: tot polling rămâne (latency variabilă, încă mai ai request-uri)

### 2) SSE (Server-Sent Events) (cea mai bună raportare efort/beneficiu)
**Idee:** UI deschide o conexiune HTTP (GET) care rămâne deschisă, iar serverul trimite evenimente când apar.

✅ Pro: aproape „real-time”, load mult mai mic, implementare mai simplă decât WebSockets  
⚠️ Contra: cere endpoint nou backend + integrare front

### 3) WebSockets/SignalR (doar dacă ai nevoie de bidirecțional)
✅ Pro: bidirecțional, flexibil  
⚠️ Contra: mai multă infrastructură, mai mult cod, mai multe edge-case-uri

---

# Recomandarea mea (plan concret)

## Faza 1: Quick Win (Polling îmbunătățit)
### Scop
- reduci request-urile și elimini leak-urile fără să atingi arhitectura.

### Checklist de implementare
1. **Unifică polling-ul într-un singur helper generic**, ex:
   - `pollJobStatus(jobType, jobId, { initialMs, maxMs, timeoutMs })`
2. **Timeout obligatoriu** pentru toate poll-urile (fără excepții).
3. **Exponential backoff**:
   - pornești la 1–2s, crești la 5s, 10s, 20s… până la max (ex 60s), cât timp statusul nu se schimbă.
4. **Cleanup garantat**:
   - Angular: `takeUntilDestroyed()` / `DestroyRef` / `AbortController` (în funcție de implementare)
5. **Standardizează rezultatul**:
   - `Queued | Running | Completed | Failed` (+ payload opțional: error, progress)

### Criterii de acceptanță
- Niciun poll nu rulează la infinit (timeout).
- Nu există subscripții active după ce componenta se distruge.
- Request-urile scad vizibil (ideal -50% sau mai mult).
- UI rămâne stabilă pe sesiuni lungi.

---

## Faza 2: SSE (optimizare majoră, încă simplă)
### Scop
- înlocuiești polling-ul repetitiv cu **1 conexiune SSE** per job.

## SSE, „client + server” explicat simplu

### Ce este SSE
Un **GET** normal care primește un răspuns care **nu se termină imediat**. Serverul „picură” evenimente text.

### Cum arată fluxul complet
1. UI pornește job-ul:
   - `POST /api/jobs/{jobType}/start` → răspuns cu `{ jobId }`
2. UI se abonează:
   - `GET /api/jobs/{jobType}/{jobId}/events` (SSE)
3. Serverul ține conexiunea deschisă și trimite update-uri:
   - Running, Progress (opțional), Completed/Failed
4. Când job-ul e gata:
   - serverul trimite ultimul eveniment și închide conexiunea
   - UI oprește listener-ul și actualizează ecranul

### Formatul mesajelor SSE (esențial)
Evenimentele sunt text, în format:
```
data: {"jobId":"...","status":"Running","progress":0.3}

data: {"jobId":"...","status":"Completed"}
```
Observă linia goală între evenimente (\n\n).

### Keep-alive
Ca să nu închidă proxy-urile conexiunea „tăcută”, serverul trimite periodic:
```
: keep-alive
```

### Reconnect
Browser-ul reconectează automat (în multe cazuri). Totuși, în practică, tu tratezi:
- `onerror` → UI face fallback la polling sau reîncearcă SSE o dată / de câteva ori.

### Limitare importantă: autentificare
`EventSource` **nu permite headers custom**. Ai variante:
- **Cookie-based auth** (ideal)
- token în query string (doar cu TTL scurt și fără log-uri care îl expun)
- alternativ: folosești `fetch()` cu streaming + `ReadableStream` (mai mult cod)

---

# Design recomandat SSE pentru proiectul tău

## Contract API (propunere simplă)
- `POST /api/jobs/{jobType}/start` → `{ jobId }`
- `GET  /api/jobs/{jobType}/{jobId}` → status curent (fallback/poll)
- `GET  /api/jobs/{jobType}/{jobId}/events` → SSE stream

### Payload standard
```json
{
  "jobId": "uuid",
  "jobType": "StoryPublish",
  "status": "Queued|Running|Completed|Failed",
  "progress": 0.0,
  "message": "optional",
  "resultRef": "optional",
  "timestamp": "ISO"
}
```

---

## Backend: 2 implementări posibile

### Varianta A (cea mai simplă): SSE endpoint verifică statusul în DB
- Endpoint SSE intră într-un loop:
  - citește statusul curent
  - dacă s-a schimbat, trimite event
  - delay 500ms–1s
  - keep-alive la 15s
  - când statusul final apare → trimite final + close

✅ ușor, puțin wiring  
⚠️ face DB reads pe durata conexiunii (dar tot e mult mai puțin decât polling masiv din UI)

### Varianta B (mai eficientă): Worker-ul „împinge” evenimente către SSE
- Ai un `IJobNotificationHub` (in-memory) care ține conexiuni active pentru jobId
- Worker-ul, când finalizează job-ul, cheamă `Notify(jobId, status)`
- SSE endpoint doar „ascultă” un Channel/queue

✅ foarte eficient  
⚠️ puțin mai mult cod, dar curat

**Notă:** Pe B1 (de obicei single instance), Varianta B in-memory e ok. Dacă scalezi la mai multe instanțe, ai nevoie de bus distribuit.

---

## Frontend (Angular): integrarea SSE
### Pattern recomandat
- Creezi un `JobEventsService` care expune:
  - `watchJob(jobType, jobId): Observable<JobEvent>`
- Intern:
  - construiește `EventSource`
  - mapează `onmessage` → `next(event)`
  - la `Completed/Failed` → închide EventSource și completează observable-ul
  - la `onerror` → fallback la polling (Faza 1)

### Fallback elegant
- Dacă SSE pică:
  1) încerci reconectare 1–2 ori  
  2) dacă tot pică, treci pe polling cu backoff.

---

# Faza 3 (opțional): scalare și hardening
Dacă ajungi să scalezi la multiple instanțe (sau vrei robustete mare):
- înlocuiești in-memory hub cu:
  - Azure Service Bus topic/subscription, sau
  - Redis pub/sub, sau
  - Event Grid etc.
- adaugi observabilitate:
  - număr conexiuni SSE active
  - durată medie per job
  - rate de reconectări / erori

---

# Plan de lucru pentru Agentic AI (task list)
## Sprint 1: Quick Win (Polling)
- [ ] Inventariază toate polling-urile existente și identifică cele fără timeout.
- [ ] Introdu un helper generic (un singur mecanism).
- [ ] Backoff + timeout + cleanup peste tot.
- [ ] Instrumentare: log / metric de request-uri per minut.

## Sprint 2: SSE
- [ ] Definește contractele API (start/status/events).
- [ ] Backend: implementează endpoint-ul SSE (Varianta A sau B).
- [ ] Frontend: `JobEventsService` cu EventSource + fallback.
- [ ] Migrează 1 job end-to-end (ex: StoryPublish) ca „vertical slice”.
- [ ] Migrează restul job-urilor folosind același contract.

## Teste/validare
- [ ] Test: 50 job-uri lansate, UI primește completări fără leak.
- [ ] Test: network drop, SSE reconectează sau trece pe fallback.
- [ ] Test: job timeout și job failed arată corect UI.

---

## Decizia finală (pe o frază)
**Fă întâi Polling îmbunătățit ca să stabilizezi rapid (și să reduci load-ul), apoi treci la SSE ca soluție principală pentru notificări, cu fallback la polling pentru reziliență.** 🚀
