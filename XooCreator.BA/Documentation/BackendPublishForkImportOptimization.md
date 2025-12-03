# Plan de îmbunătățire pentru fluxurile Publish / Fork / Import / Export

## 0. Context și obiectiv
- Planul vizează endpointurile cele mai lente: `ImportFullStoryEndpoint`, `PublishStoryEndpoint`, `ForkStoryEndpoint`, `ExportDraftStoryEndpoint`, `ExportPublishedStoryEndpoint`.
- Platforma rulează pe Azure App Service B1 (1 vCPU, 1.75 GB RAM) cu baza Postgres găzduită în altă regiune; trebuie să minimizăm memoria, CPU-ul și round-trip-urile EF.
- Scopul este să aplicăm îmbunătățirile incremental (“baby steps”), validând fiecare pas prin metrici din Application Insights.

## 1. Monitorizare & diagnostic (înainte de cod)
1. Activează Application Insights (dacă nu este deja):
   - Rulează `az monitor app-insights component connect-webapp` pentru site-ul curent.
   - În `appsettings.Production.json` setează connection string-ul la `ApplicationInsights`.
2. Adaugă pachetele:
   - `Microsoft.ApplicationInsights.AspNetCore`
   - `Microsoft.EntityFrameworkCore.Diagnostics` pentru event-logging.
3. Pornește logging pentru EF/Postgres:
   - În `Program.cs`, configurează `builder.Services.AddApplicationInsightsTelemetry();`.
   - Adaugă `builder.Services.AddLogging(o => o.AddApplicationInsights());`.
   - Setează `EnableSensitiveDataLogging` doar în staging pentru analize temporare.
4. Creează vizualizări în Azure:
   - Dashboards pentru `Server response time`, `Requests in application queue`, `Memory working set`, `Exceptions`.
   - Grafuri dedicate pentru endpointurile `import-full`, `publish`, `fork`, `export`.
5. Rulează manual scenariile lente, colectează timeline-uri (Application Insights Profiler).

## 2. Import Full Story – streaming & limitări
1. Refactorizează `UploadAssetsFromZipAsync`:
   - Înlocuiește `MemoryStream` cu streaming direct (`await entryStream.CopyToAsync(destinationStream)`).
   - Folosește `BlobClient.OpenWriteAsync` cu `overwrite: true` pentru upload fără buffer intermediar.
2. Adaugă un prag de timeout per asset (ex. `CancellationTokenSource` pe 2 minute) pentru a evita blocări infinite.
3. Procesează activele în batch-uri (ex. `Parallel.ForEachAsync` cu `MaxDegreeOfParallelism = 3`) pentru a reduce durata totală fără a satura CPU.
4. Tratează fallback-ul assetelor lipsă prin coadă de avertizări, fără a arunca excepție integrală.
5. Introdu un parametru de preview:
   - Opțiune `?validateOnly=true` care doar parcurge manifestul și raportează erorile, fără upload (util pentru testare).
6. Loghează dimensional: dimensiune ZIP, număr assets, timp total, memorie curentă (`GC.GetTotalMemory`).

## 3. Publish Story – optimizare EF și copiere asset-uri
1. Înlocuiește ștergerea+inserția completă cu update-uri incremental:
   - Compară tile-urile existente cu cele noi (folosește `Dictionary` după `TileId`).
   - Actualizează doar ce s-a schimbat; șterge elementele care lipsesc.
   - Folosește `ExecuteDeleteAsync`/`ExecuteUpdateAsync` (EF Core 7+) pentru loturi mari.
2. Mută `StoryPublishingService.UpsertFromCraftAsync` într-un `TransactionScope` explicit pentru a reduce round-trip-urile.
3. Proiectează datele cu `AsNoTracking()` pentru citiri și atașează manual obiectele care se modifică.
4. Optimizează copierea assetelor:
   - Folosește `BlobBatchClient` sau `CopyFromUriAsync` fără polling manual; ascultă evenimente `CopyStatus` prin `WaitForCompletionAsync`.
   - Rulează copiile în paralel controlat (`SemaphoreSlim`).
5. Mută finalizarea publicării într-un job de fundal:
   - Endpoint-ul lansează o comandă (de exemplu, în Azure Storage Queue).
   - Un worker consumă job-ul și finalizează publish-ul; răspunsul HTTP revine imediat cu status `202 Accepted`.
6. După publicare, actualizează statisticile în memorie (`IMemoryCache`) pentru a evita reinterogarea DB în marketplace.

## 4. Fork Story – reutilizarea assetelor
1. Ajustează `ForkStoryEndpoint` să încarce doar câmpurile necesare:
   - Creează metode `GetSummaryAsync` în `StoryCraftsRepository` care proiectează doar metadatele.
   - Încarcă restul doar dacă se cere `CopyAssets`.
2. Extrage logica de copiere asset într-un serviciu asincron:
   - Endpoint-ul răspunde imediat (202) și stochează task-ul într-o coadă.
   - Worker-ul rulează `CopyDraftToDraftAsync` cu retry (de ex. 3 încercări, backoff).
3. Evită `FirstOrDefaultAsync` cu `.Include` multiple – folosește `Select` pentru a obține direct `ownerEmail`.
4. Normalizează email-urile (lowercase) înainte de a forma căile de blob pentru a reduce duplicarea assetelor.

## 5. Export Draft & Published – streaming la client
1. Utilizează `BlobClient.OpenReadAsync` cu `ZipArchiveEntry.Open()` direct, fără `MemoryStream` global.
2. Setează `Response.Headers.Content-Disposition` și scrie ZIP-ul prin `FileCallbackResult` (streaming).
3. Adaugă caching:
   - Pentru export published, memorează ZIP-ul generat 15 minute într-un cont de storage “exports”.
   - Pentru draft, generează un SAS (valabil 10 minute) și returnează linkul, nu fișierul.
4. Journalizare:
   - Adaugă loguri cu durata totală și dimensiunea răspunsului la fiecare export.

## 6. Reducerea încărcărilor EF
1. În `StoryCraftsRepository`, introdu metode specializate:
   - `GetForPublishAsync` (include doar ce folosește publish).
   - `GetForForkAsync`.
   - `GetForEditorAsync` cu proiecții DTO.
2. Folosește `Select` + `ToListAsync` pentru colecțiile mari; doar când chiar este nevoie atașezi entitățile complete.
3. Activează `UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)` doar când este sigur; altfel utilizează `SplitQuery` pentru a evita cartesian explosion.

## 7. Optimizări infrastructură (în paralel cu refactor)
1. Mută Postgres în aceeași regiune Azure ca App Service (reduce latența 80–120 ms/request).
2. Activează PgBouncer sau `Max Pool Size` mai mare în connection string (ex. `Maximum Pool Size=200`).
3. Scale out temporar App Service la plan P0v3 sau instanțe multiple pentru a amortiza spike-urile până când codul este optimizat.

## 8. Validare incrementală (baby steps)
1. După fiecare bloc de modificări:
   - Rulează testele automate și adaugă scenarii dedicate (import de 100 MB, publish cu 50 de asset-uri).
   - Monitorizează grafurile AI pentru scăderea timpilor (<5 s pentru publish, <10 s pentru import).
2. Documentează rezultatele în `ApplicationInsightsLoggingEvaluation.md`.
3. Comunicarea către echipă: checklist-ul fiecărui pas și metricile înainte/după.

---
*Acest document servește drept roadmap incremental. Aplică pașii în ordinea priorităților (secțiunile 1–3 sunt critice).*

