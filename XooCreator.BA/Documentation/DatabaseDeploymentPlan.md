## Obiectiv

Asigurăm un flux determinist de provisionare și migrare pentru baza de date `alchimalia_schema`, astfel încât:

- mediul `dev` din Azure App Service să poată fi recreat complet (drop + migrate) atunci când `Database:RecreateOnStart = true`;
- același mecanism să aplice incremental migrațiile ulterioare atunci când flag-ul este dezactivat;
- pipeline-ul GitHub Actions/azure să fie capabil să ruleze fără intervenții manuale, folosind conexiunea setată în secrets.

## Observații cheie

- `appsettings.Development.json` citește connection string-ul din `AZURE_PG_CONNSTRING_DEV`. Dacă App Service rulează cu `ASPNETCORE_ENVIRONMENT=Production`, se folosește `AZURE_PG_URL_PROD`; în lipsa acesteia, fallback-ul este un connection string local (`localhost`).  
- `Program.cs` dropează schema când flag-ul `RecreateOnStart` este `true`, apoi folosește `DatabaseMigrationService` pentru a rula `MigrateAsync`.
- Interceptorul de migrare avea verificări hardcodate pe `table_schema = 'public'`, ceea ce împiedica unele operații (indexuri, coloane) să fie aplicate pe schema personalizată.

## Plan de acțiune recomandat

1. **Config App Service (Dev)**
   - Setează explicit `ASPNETCORE_ENVIRONMENT=Development`.
   - Stochează conexiunea Azure PostgreSQL în secretul `AZURE_POSTGRES_CONNSTRING_DEV` (folosit de pipeline) sau direct în `ConnectionStrings__Postgres` și include `SearchPath=alchimalia_schema`.
2. **Cod**
   - Schema implicită rămâne `alchimalia_schema`; aplicația nu mai face drop automat, ci rulează `ApplyMigrationsAsync()` la fiecare startup.
   - `DatabaseConfiguration` configurează `SearchPath` + `__EFMigrationsHistory` pe schema din configurare.
3. **Pipeline**
   - YAML-ul de deploy pe `dev` rulează `dotnet publish` + deploy și setează connection string-ul Azure; nu mai sunt necesare flag-uri suplimentare.
4. **Testare**
   - Test manual local: rulează aplicația cu setările dev (folosind același connection string, eventual printr-un tunnel) și confirmă că migrarea + seeding-ul se execută.
   - Test în Azure: monitorizează Log Stream pentru mesajele de migrare și confirmă apariția tabelelor în `alchimalia_schema`.
5. **Stabilizare**
   - Migrarea rulează incremental la fiecare pornire; pentru schimbări mai mari poți rula scripturi SQL manual.
   - Monitorizează `DatabaseMigrationService` pentru erori și folosește `/debug/db-state` pentru diagnostic rapid.

## Checklist la fiecare deploy

- [ ] Environment-ul dev are connection string-ul corect (`AZURE_POSTGRES_CONNSTRING_DEV` / `ConnectionStrings__Postgres`) și include `SearchPath=alchimalia_schema`.
- [ ] `dotnet ef migrations add` a fost rulat local pentru schimbările noi (dacă este cazul).
- [ ] Pipeline-ul finalizează fără erori și App Service pornește fără `startupException`.
- [ ] În schema `alchimalia_schema` apar tabelele/coloanele noi.

