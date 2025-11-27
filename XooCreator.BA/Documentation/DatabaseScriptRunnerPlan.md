## Plan Runner Scripturi SQL

### Obiectiv
Implementăm un mecanism determinist de aplicare a scripturilor SQL (fără EF Migrations), cu versiuni, tranzacții și rollback opțional, astfel încât orice mediu să poată fi adus în starea dorită doar prin rularea scripturilor și a runner-ului dedicat.

---

### 1. Structura Scripturilor
1.1 Foldere dedicate:
   - `Database/Scripts` pentru scripturile forward.
   - `Database/Scripts/Rollbacks` (opțional) pentru scripturile de rollback asociate.

1.2 Convenție de nume:
   - `V0001__create_schema.sql`, `V0002__init_tables.sql`, `V0003__seed_core_data.sql` etc.
   - Prefix numeric incremental + dublu underscore + descriere scurtă.
   - Pentru rollback: `R0001__drop_schema.sql` etc. (prefixul trebuie să corespundă scriptului forward).

1.3 Conținut scripturi:
   - Idempotente (`CREATE SCHEMA IF NOT EXISTS`, `ALTER TABLE ... ADD COLUMN IF NOT EXISTS`, `INSERT ... ON CONFLICT DO NOTHING`).
   - Comentarii în capul fișierului (ce face, dependențe, eventual rollback asociat).

---

### 2. Tabela de Versiuni
2.1 Nume: `alchimalia_schema.schema_versions` (sau altă schemă dedicată).

2.2 Coloane:
   - `script_name` (PK),
   - `checksum` (hash SHA256/MD5 al conținutului),
   - `executed_at` (timestamp),
   - `execution_time_ms` (opțional),
   - `status` (Succeeded/Failed/Others, opțional).

2.3 Comportament:
   - Runner-ul verifică înainte de execuție dacă `script_name` există și checksum-ul coincide.
     - Dacă există și checksum-ul e identic → scriptul se ignoră.
     - Dacă lipsește sau checksum-ul diferă → scriptul se rulează (și se actualizează/inserează în tabel).

---

### 3. Runner-ul (Console App)
3.1 Proiect nou: `XooCreator.DbScriptRunner` (Console).

3.2 Input-uri:
   - Connection string (linie de comandă sau config),
   - Calea către folderul cu scripturi (default `../../Database/Scripts`),
   - Opțiune `--dry-run` (listează scripturile fără să le execute),
   - Opțiune `--rollback` (execută scripturile de rollback pentru un anumit interval).

3.3 Pași execuție (forward):
   1. Conectare la DB.
   2. Creare tabel `schema_versions` dacă nu există.
   3. Enumerare fișiere `V*.sql`, sortare după prefix.
   4. Pentru fiecare:
      - Calculează checksum.
      - Verifică în `schema_versions`.
      - Dacă este nou sau checksum diferit:
         - Deschide tranzacție.
         - Execută scriptul (comenzi separate, se poate face split pe `;` cu atenție).
         - Commit; în caz de excepție → rollback + log + runner returnează cod != 0.
         - Scrie în `schema_versions`.
      - Dacă scriptul a rulat deja (checksum identic) → log explicit „skip”.

3.4 Logging:
   - Console (stdout) cu format consistent: „Applying V0002__init_tables.sql ... OK (00:00:02.345)”.
   - În caz de eroare, afișează mesajul SQL, codul Postgres etc.

3.5 Comportament tranzacțional:
   - Fiecare script se execută într-o tranzacție separată.
   - Dacă scriptul e prea mare și nu trebuie totul într-o singură tranzacție, acceptăm scripturi `BEGIN; ... COMMIT;` explicite (runner-ul detectează și nu mai închide tranzacția).

---

### 4. Rollback
4.1 Manual:
   - Pentru fiecare `V` se poate crea `R` corespunzător.
   - Runner-ul primește parametru `--rollback V0003` și execută `R0003__...` într-o tranzacție. Nu se rulează automat (decât dacă decidem la stepul de pipeline).

4.2 Automat (optional):
   - Pipeline-ul poate avea un job „manual intervention” în caz de eșec → rulează `dotnet run -- --rollback Vxxxx`.

---

### 5. Pipeline/Deployment
5.1 În GitHub Actions (dev și prod):
   - După `dotnet publish`, înainte de restart App Service:
     ```yaml
     - name: Run database scripts
       run: dotnet run --project XooCreator.DbScriptRunner \
             --connection "${{ secrets.AZURE_POSTGRES_CONNSTRING_DEV }}" \
             --scripts-path Database/Scripts
     ```
   - Dacă step-ul eșuează, pipeline-ul se oprește (nu se face deploy).

5.2 În App Service:
   - Runner-ul nu se execută la fiecare pornire (doar în pipeline). Aplicația `Program.cs` rămâne simplă (nu mai face drop/migrate).

---

### 6. Tranziția de la EF Migrations
6.1 Export migrare inițială:
   - Convertim `20251126184048_InitialFullSchema` în script SQL `V0001__initial_full_schema.sql`.
   - Inserăm manual `INSERT INTO schema_versions` pentru scripturile aplicate deja (dacă baza a fost migrată cu EF).

6.2 Dezactivare EF:
   - `Program.cs` doar verifică schema și rulează seeder-urile necesare (fără `MigrateAsync`).
   - Documentație actualizată pentru echipă (folosesc runner-ul pentru schimbări viitoare).

---

### 7. Testare & QA
7.1 Local:
   - rulează runner-ul pe un Postgres local (sau docker) pentru a verifica ordinea scripturilor.

7.2 Dev/QA:
   - Pipeline rulează scripturile; verifică `schema_versions`, logurile și schema finală.

7.3 Prod:
   - Stabilește proces: rulezi pipeline-ul → db scripts → deploy → monitorizare.
   - Optionally, păstrezi un snapshot/backup înainte de a rula scripturile.

---

### 8. Next steps (pentru sesiunea viitoare)
1. Inițializează proiectul `XooCreator.DbScriptRunner` (console).
2. Creează tabela `schema_versions` și scriptul `V0001__initial_full_schema.sql`.
3. Adaugă utilitarul în pipeline (dev) cu dry-run + execuție.
4. Testează pe baza Azure înainte de a merge în producție.
5. Documentează convenția de scriere a scripturilor pentru restul echipei.

---

### 9. Implementare curentă (2025-11-27)
- ✅ Proiectul `XooCreator.DbScriptRunner` există în soluție și expune CLI:
  ```bash
  dotnet run --project XooCreator.DbScriptRunner \
      --connection "Host=...;Username=...;Password=...;Database=...;SearchPath=alchimalia_schema" \
      --scripts-path Database/Scripts \
      --dry-run
  ```
  Opțiuni suportate: `--connection`, `--schema`, `--scripts-path`, `--rollbacks-path`, `--dry-run`, `--rollback V0003`. Valorile pot veni și din `ConnectionStrings__Postgres` sau `DB_RUNNER_CONNECTIONSTRING`.
- ✅ Runner-ul creează/folosește tabela `alchimalia_schema.schema_versions`, verifică checksum-ul (SHA256) și scrie status `Succeeded` sau `RolledBack` împreună cu durata execuției.
- ✅ Structura de directoare `Database/Scripts` + `Database/Scripts/Rollbacks` este prezentă în repo. Primul script (`V0001__initial_full_schema.sql`) a fost exportat din migrarea `20251126184048_InitialFullSchema` folosind `dotnet tool run dotnet-ef migrations script ...`.
- ✅ În rădăcina backend-ului există un manifest local `.config/dotnet-tools.json` care fixează `dotnet-ef` la versiunea 8.0.11 (evităm conflictul cu instalația globală 10.x).
- ✅ `V0002__seed_bestiary_items.sql` înserează toate combinațiile Bestiary generate din `Data/SeedData/Discovery/i18n/<locale>/discover-bestiary.json`. Scriptul este produs determinist cu `Database/Scripts/Generators/Generate-BestiarySql.ps1` (folosește `uuid_generate_v5` pentru chei stabile și este idempotent via `ON CONFLICT ("Id")`).
- ✅ `V0003__seed_story_topics_age_groups_authors.sql` aduce în DB toate topic-urile + traducerile, grupele de vârstă + descrierile și lista de autori clasici din `Data/SeedData/Story-Editor/**`. Generator:  
  ```powershell
  cd BA/XooCreator.BA/Database/Scripts/Generators
  pwsh ./Generate-StoryTopicsSql.ps1
  ```
- ℹ️ Pentru a regenera `V0002`:  
  ```powershell
  cd BA/XooCreator.BA/Database/Scripts/Generators
  pwsh ./Generate-BestiarySql.ps1
  ```
- ℹ️ Toate scripturile noi trebuie să fie reproductibile din sursele din `Data/SeedData/**`. Vom adăuga generatoare similare pentru Topics/AgeGroups, Hero Tree, Stories, Tree Model, etc., astfel încât viitoare actualizări de conținut să se facă prin „source of truth” în JSON + regenerare SQL.
- ℹ️ Fiecare script se execută în tranzacție separată; dacă fișierul conține manual `BEGIN; ... COMMIT;`, runner-ul detectează și nu mai deschide tranzacție proprie.
- ℹ️ Modele de rulare:
  - Execuție reală: `dotnet run --project XooCreator.DbScriptRunner -- --connection "%ConnectionStrings__Postgres%"`.
  - Numai raport (fără modificări): adaugă `--dry-run` (nu se creează tabela `schema_versions` dacă lipsește).
  - Rollback manual: `dotnet run --project XooCreator.DbScriptRunner -- --connection "...conn..." --rollback V0003` (caută `R0003__*.sql` în `Database/Scripts/Rollbacks`).
- 🔜 Următorii pași rămași din plan: integrarea în pipeline (dry-run + execuție), teste pe Azure/Postgres local și actualizarea documentației aplicației pentru a elimina EF Migrations din `Program.cs`.

