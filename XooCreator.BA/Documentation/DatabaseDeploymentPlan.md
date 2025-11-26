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
   - Păstrează `AZURE_PG_CONNSTRING_DEV` în Application Settings și, opțional, setează `DATABASE_URL` cu aceeași valoare pentru fallback rapid.
2. **Cod**
   - Interceptorul (`IdempotentMigrationCommandInterceptor`) să folosească schema din config (`alchimalia_schema`) pentru toate verificările din `information_schema`/`pg_indexes`.
   - `DatabaseConfiguration` transmite schema către interceptor și configurează `SearchPath` + `__EFMigrationsHistory` pe aceeași schema.
3. **Pipeline**
   - YAML-ul de deploy pe `dev` să ruleze `dotnet publish` + deploy (deja existent).  
   - După implementarea schimbărilor de mai sus, un restart cu `RecreateOnStart=true` va droppa schema și va recrea complet baza. Convertirea la `false` se face după ce baza e sincronizată.
4. **Testare**
   - Test manual local: rulează aplicația cu setările dev (folosind același connection string, eventual printr-un tunnel) și confirmă că migrarea + seeding-ul se execută.
   - Test în Azure: monitorizează Log Stream pentru mesajele de migrare și confirmă apariția tabelelor în `alchimalia_schema`.
5. **Stabilizare**
   - După primul deploy reușit pe dev, schimbă `Database:RecreateOnStart` în secrets/config la `false` pentru a evita ștergerea datelor la restarte ulterioare.
   - Pentru migrări ulterioare, menține fluxul incremental și monitorizează `DatabaseMigrationService` pentru erori.

## Checklist la fiecare deploy

- [ ] Environment-ul dev are toate connection string-urile + flag-urile corecte.
- [ ] `dotnet ef migrations add` a fost rulat local pentru schimbările noi (dacă este cazul).
- [ ] Pipeline-ul finalizează fără erori și App Service pornește fără `startupException`.
- [ ] În schema `alchimalia_schema` apar tabelele/coloanele noi.
- [ ] `RecreateOnStart` este readus la `false` când nu se mai dorește drop complet.

