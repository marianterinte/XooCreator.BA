## Obiectiv

Asigurăm un flux determinist de provisionare și migrare pentru baza de date `alchimalia_schema`, astfel încât:

- mediul `dev` din Azure App Service să poată fi recreat complet (drop + migrate) atunci când `Database:RecreateOnStart = true`;
- același mecanism să aplice incremental migrațiile ulterioare atunci când flag-ul este dezactivat;
- pipeline-ul GitHub Actions/azure să fie capabil să ruleze fără intervenții manuale, folosind conexiunea setată în secrets.

## Observații cheie

- `ConnectionStrings__Postgres` trebuie să conțină `SearchPath=alchimalia_schema` și este populat în App Service din secretul `AZURE_PG_CONNSTRING_DEV_EUROPA_EUROPA / AZURE_PG_CONNSTRING_PROD_EUROPA`.
- `Program.cs` nu mai rulează `DatabaseMigrationService` / `MigrateAsync`; aplicația doar testează conectivitatea și afișează o pagină de eroare dacă DB-ul nu este pregătit.
- Structura DB este controlată 100% de scripturile incremental numerotate (`Database/Scripts/V0001__*.sql`) rulate prin `XooCreator.DbScriptRunner` și urmărite în `alchimalia_schema.schema_versions`.

## Plan de acțiune recomandat

1. **Config App Service (Dev)**
   - Setează explicit `ASPNETCORE_ENVIRONMENT=Development`.
   - Stochează conexiunea Azure PostgreSQL în secretul `AZURE_PG_CONNSTRING_DEV_EUROPA` (folosit de pipeline) sau direct în `ConnectionStrings__Postgres` și include `SearchPath=alchimalia_schema`.
2. **Cod**
   - Schema implicită rămâne `alchimalia_schema`; aplicația doar verifică conectivitatea la startup, iar migrarea este responsabilitatea scripturilor SQL.
   - `DatabaseConfiguration` continue să seteze `SearchPath` și să folosească `alchimalia_schema` drept schemă implicită.
3. **Pipeline**
   - YAML-ul de deploy pe `dev` rulează `dotnet publish` + deploy și setează connection string-ul Azure; nu mai sunt necesare flag-uri suplimentare.
4. **Testare**
   - Test manual local: rulează aplicația cu setările dev (folosind același connection string, eventual printr-un tunnel) și confirmă că migrarea + seeding-ul se execută.
   - Test în Azure: monitorizează Log Stream pentru mesajele de migrare și confirmă apariția tabelelor în `alchimalia_schema`.
5. **Stabilizare**
   - Migrarea rulează incremental prin `Database Scripts Runner` (local + GitHub Actions); aplicația trebuie lansată doar după ce `schema_versions` confirmă ultimul script.
   - Monitorizează joburile `Database Scripts Runner` și folosește endpoint-ul `/debug/db-state` (doar în Development) pentru a inspecta înregistrările din `schema_versions`.
   - Pentru situații în care schema trebuie resetată complet, rulează `dotnet run --project XooCreator.DbScriptRunner -- --connection "...conn..." --rollback V0001` (execută `R0001__drop_schema.sql`, recreează schema și `schema_versions`, apoi rerulează scripturile forward).

## Checklist la fiecare deploy

- [ ] Environment-ul dev are connection string-ul corect (`AZURE_PG_CONNSTRING_DEV_EUROPA` / `ConnectionStrings__Postgres`) și include `SearchPath=alchimalia_schema`.
- [ ] Pentru schimbări de schemă s-au generat scripturi noi `VXXXX__*.sql` + eventual generator PowerShell.
- [ ] Pipeline-ul finalizează fără erori și App Service pornește fără `startupException`.
- [ ] În schema `alchimalia_schema` apar tabelele/coloanele noi.

