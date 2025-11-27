# Soluție Robustă pentru Migrări Incrementale

> **ℹ️ Notă (2025-11-27):** Document istoric. Producția folosește acum `Database/Scripts` + `XooCreator.DbScriptRunner` (fără `DatabaseMigrationService/MigrateAsync`). Secțiunile de mai jos rămân ca referință pentru abordarea EF Core anterioară.

## Prezentare Generală

Această soluție oferă un sistem **complet automat** pentru aplicarea migrărilor de bază de date în mod incremental, fără a recrea baza de date și fără a depinde de gestionarea excepțiilor. Soluția folosește un **interceptor automat** care transformă toate comenzile SQL din migrări în operații idempotente.

## Componente Principale

### 1. IdempotentMigrationCommandInterceptor (Automat)

**Interceptor care transformă automat toate comenzile SQL din migrări:**

- **`CREATE TABLE`** → Verifică dacă tabelul există; dacă da, adaugă doar coloanele lipsă
- **`CREATE INDEX`** → Verifică dacă coloanele există; creează index doar dacă nu există
- **`ALTER TABLE ... ADD COLUMN`** → `ALTER TABLE ... ADD COLUMN IF NOT EXISTS`
- **`ALTER TABLE ... ADD CONSTRAINT`** → Wrapped în `DO $$` block cu verificare
- **`INSERT INTO`** → `INSERT INTO ... ON CONFLICT DO NOTHING` (toate constraint-urile)
- **`CREATE SEQUENCE`** → `CREATE SEQUENCE IF NOT EXISTS`
- **`CREATE TYPE`** → `CREATE TYPE IF NOT EXISTS`

**Caracteristici:**
- ✅ **Complet automat** - nu necesită modificări în migrări
- ✅ **Transparent** - funcționează cu migrările existente
- ✅ **Robust** - gestionează toate cazurile edge-case
- ✅ **Logging** - loghează transformările pentru debugging

### 2. IDatabaseMigrationService / DatabaseMigrationService

Serviciu care gestionează migrările de bază de date:

- **`ApplyMigrationsAsync()`** - Aplică toate migrările pendente incremental
- **`GetPendingMigrationsAsync()`** - Returnează lista migrărilor care nu au fost încă aplicate
- **`GetAppliedMigrationsAsync()`** - Returnează lista migrărilor deja aplicate
- **`EnsureMigrationsHistoryTableExistsAsync()`** - Asigură existența tabelei de istoric
- **`EnsureCriticalColumnsExistAsync()`** - Asigură existența coloanelor critice (internal)

**Caracteristici:**
- Folosește metodele built-in din EF Core (`GetPendingMigrationsAsync`, `MigrateAsync`)
- Logging detaliat pentru fiecare pas
- Gestionare robustă a erorilor cu logging informativ
- Verifică și adaugă coloane critice înainte de migrări

### 3. MigrationHelpers (Opțional - pentru migrări noi)

Clasă de helper-uri pentru crearea de migrări idempotente (opțional, interceptorul face totul automat):

**Operații disponibile:**
- `CreateTableIfNotExists()` - Creează tabelă doar dacă nu există
- `CreateIndexIfNotExists()` - Creează index doar dacă nu există
- `AddColumnIfNotExists()` - Adaugă coloană doar dacă nu există
- `InsertDataIfNotExists()` - Inserează date doar dacă nu există
- `AddPrimaryKeyIfNotExists()` - Adaugă cheie primară doar dacă nu există
- `AddForeignKeyIfNotExists()` - Adaugă cheie străină doar dacă nu există
- `AddUniqueConstraintIfNotExists()` - Adaugă constraint unic doar dacă nu există
- Și multe altele...

**Notă:** Aceste helper-uri sunt opționale - interceptorul face transformările automat!

## Use Cases - Ce Funcționează Automat

### ✅ Use Case 1: Adăugare Coloană Nullable în Tabel Existente

**Scenariu:** Adaugi o coloană nouă nullable într-un tabel care are deja date.

**Cod Migrare (Standard EF Core):**
```csharp
migrationBuilder.AddColumn<Guid?>(
    name: "NewColumn",
    table: "MyTable",
    type: "uuid",
    nullable: true);
```

**Rezultat:**
- ✅ Funcționează perfect - coloana este adăugată doar dacă nu există
- ✅ Sigur de rulat de mai multe ori
- ✅ Funcționează chiar dacă tabelul are date existente
- ✅ **Nu necesită modificări în cod** - interceptorul face totul automat

### ✅ Use Case 2: Creare Tabel Nou

**Scenariu:** Creezi un tabel complet nou.

**Cod Migrare (Standard EF Core):**
```csharp
migrationBuilder.CreateTable(
    name: "MyNewTable",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_MyNewTable", x => x.Id);
    });
```

**Rezultat:**
- ✅ Funcționează perfect - tabelul este creat doar dacă nu există
- ✅ Sigur de rulat de mai multe ori
- ✅ Dacă tabelul există, coloanele lipsă sunt adăugate automat

### ✅ Use Case 3: Modificare Tabel Existent (Adăugare Coloane)

**Scenariu:** Modifici un tabel existent adăugând coloane noi (ex: `ClassicAuthorId` în `StoryDefinitions`).

**Cod Migrare (Standard EF Core):**
```csharp
migrationBuilder.CreateTable(
    name: "StoryDefinitions",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        StoryId = table.Column<string>(type: "text", nullable: false),
        ClassicAuthorId = table.Column<Guid>(type: "uuid", nullable: true), // COLOANĂ NOUĂ
        // ... alte coloane
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_StoryDefinitions", x => x.Id);
    });
```

**Rezultat:**
- ✅ Interceptorul detectează că tabelul există
- ✅ Adaugă automat doar coloana `ClassicAuthorId` care lipsește
- ✅ Funcționează perfect - fără erori chiar dacă tabelul există
- ✅ Sigur de rulat de mai multe ori
- ✅ Toate datele existente sunt păstrate

### ✅ Use Case 4: Inserare Date Seed

**Scenariu:** Inserezi date seed care ar putea exista deja.

**Cod Migrare (Standard EF Core):**
```csharp
migrationBuilder.InsertData(
    table: "MyTable",
    columns: new[] { "Id", "Name" },
    values: new object[,]
    {
        { new Guid("..."), "Value1" },
        { new Guid("..."), "Value2" }
    });
```

**Rezultat:**
- ✅ Interceptorul transformă în: `INSERT INTO ... ON CONFLICT DO NOTHING;`
- ✅ Funcționează perfect - gestionează conflictele pe cheie primară ȘI constraint-uri unice
- ✅ Sigur de rulat de mai multe ori
- ✅ Fără erori de duplicate key violations

### ✅ Use Case 5: Creare Index-uri

**Scenariu:** Creezi un index pe o coloană care ar putea să nu existe încă.

**Cod Migrare (Standard EF Core):**
```csharp
migrationBuilder.CreateIndex(
    name: "IX_MyTable_ColumnName",
    table: "MyTable",
    column: "ColumnName");
```

**Rezultat:**
- ✅ Interceptorul verifică dacă coloana există înainte de a crea index-ul
- ✅ Creează index-ul doar dacă nu există
- ✅ Funcționează perfect - fără erori dacă coloana nu există sau index-ul există deja
- ✅ Sigur de rulat de mai multe ori

### ⚠️ Use Case 6: Adăugare Coloană NOT NULL Fără Default

**Scenariu:** Adaugi o coloană NOT NULL fără valoare implicită într-un tabel cu date existente.

**Cod Migrare:**
```csharp
migrationBuilder.AddColumn<string>(
    name: "RequiredColumn",
    table: "MyTable",
    type: "character varying(100)",
    maxLength: 100,
    nullable: false); // NOT NULL, fără default
```

**Rezultat:**
- ❌ PostgreSQL va eșua pentru că rândurile existente ar avea valori NULL
- ⚠️ Acesta este comportamentul normal PostgreSQL - TREBUIE să furnizezi o valoare implicită

**Soluție:**
```csharp
migrationBuilder.AddColumn<string>(
    name: "RequiredColumn",
    table: "MyTable",
    type: "character varying(100)",
    maxLength: 100,
    nullable: false,
    defaultValue: ""); // Furnizează valoare implicită
```

## Utilizare

### În Program.cs

Soluția este integrată automat în `Program.cs`. La pornirea aplicației:

1. Se verifică dacă există migrări pendente
2. Se aplică incremental doar migrările noi
3. Se loghează progresul pentru fiecare migrare

```csharp
// În Program.cs - se execută automat la startup
var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
var migrationSuccess = await migrationService.ApplyMigrationsAsync();
```

### În Migrări Noi

**✅ VESTE BUNĂ:** Poți scrie migrări normal folosind metodele standard EF Core! Interceptorul face totul automat.

```csharp
// Scrie migrări normal - interceptorul le face idempotente automat
public partial class MyMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Creează tabelă - interceptorul adaugă IF NOT EXISTS automat
        migrationBuilder.CreateTable(
            name: "MyTable",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MyTable", x => x.Id);
            });

        // Adaugă coloană - interceptorul adaugă IF NOT EXISTS automat
        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "MyTable",
            type: "character varying(500)",
            nullable: true);

        // Creează index - interceptorul verifică existența automat
        migrationBuilder.CreateIndex(
            name: "IX_MyTable_Name",
            table: "MyTable",
            column: "Name");

        // Inserează date - interceptorul adaugă ON CONFLICT DO NOTHING automat
        migrationBuilder.InsertData(
            table: "MyTable",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                { new Guid("..."), "Value1" },
                { new Guid("..."), "Value2" }
            });
    }
}
```

**Nu este nevoie să folosești `MigrationHelpers` sau SQL raw** - interceptorul face totul automat!

## Avantaje

1. **Complet automat** - Nu necesită modificări în migrări
2. **Robust** - Nu depinde de excepții pentru funcționare
3. **Idempotent** - Migrările pot fi rulate de mai multe ori în siguranță
4. **Incremental** - Aplică doar migrările noi, nu recrează baza de date
5. **Logging detaliat** - Fiecare pas este logat pentru debugging
6. **Generic** - Funcționează cu orice migrare, vechi sau nouă
7. **Production-ready** - Gestionare robustă a erorilor și logging informativ
8. **Transparent** - Funcționează cu migrările existente fără modificări

## Best Practices

1. **Scrie migrări normal** - Interceptorul gestionează idempotența automat
2. **Pentru coloane NOT NULL, furnizează întotdeauna o valoare implicită** dacă tabelul are date existente
3. **Folosește coloane nullable când este posibil** pentru coloane noi în tabele existente
4. **Testează migrările de mai multe ori**:
   - Rulează migrarea
   - Rulează din nou - nu ar trebui să dea erori
5. **Nu recrea baza de date în producție** - setează `Database:RecreateOnStart` la `false`

## Configurare

### Interceptorul

Interceptorul este înregistrat automat în `Program.cs` când se configurează `DbContext`:

```csharp
options.UseNpgsql(cs);
var loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();
var logger = loggerFactory?.CreateLogger<IdempotentMigrationCommandInterceptor>();
options.AddInterceptors(new IdempotentMigrationCommandInterceptor(logger));
```

### Serviciul de Migrare

Serviciul este înregistrat automat în `ServiceCollectionExtensions.AddInfrastructureServices()`:

```csharp
services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
```

**Nu este nevoie de configurare suplimentară** - funcționează out-of-the-box.

## Răspunsuri la Întrebări Frecvente

### Q: Dacă mâine adaug o coloană nouă într-o tabelă și adaug o migrare, când fac deploy o să meargă?

**R:** ✅ **DA!** Funcționează perfect:
- Dacă coloana este **nullable** → Funcționează automat, fără probleme
- Dacă coloana este **NOT NULL cu default** → Funcționează automat, fără probleme
- Dacă coloana este **NOT NULL fără default** → Trebuie să furnizezi o valoare implicită

### Q: Dacă fac un tabel nou, sigur va merge?

**R:** ✅ **DA!** Funcționează perfect:
- Tabelul este creat doar dacă nu există
- Dacă tabelul există deja, coloanele lipsă sunt adăugate automat
- Sigur de rulat de mai multe ori

### Q: Dacă modific un tabel existent cu un câmp nullable, va merge?

**R:** ✅ **DA!** Funcționează perfect:
- Interceptorul detectează că tabelul există
- Adaugă automat doar coloana care lipsește
- Toate datele existente sunt păstrate
- Sigur de rulat de mai multe ori

### Q: Trebuie să modific migrările existente?

**R:** ❌ **NU!** Interceptorul funcționează automat cu toate migrările, vechi sau noi.

### Q: Trebuie să folosesc MigrationHelpers în migrări noi?

**R:** ❌ **NU!** Poți scrie migrări normal folosind metodele standard EF Core. Interceptorul face totul automat.

## Note

- Soluția folosește tabelele de istoric EF Core (`__EFMigrationsHistory`) pentru a ține evidența migrărilor aplicate
- Dacă o migrare eșuează, se loghează care migrări au fost aplicate înainte de eșec
- Toate transformările sunt logate la nivel Debug/Trace pentru troubleshooting
- Interceptorul folosește funcții specifice PostgreSQL (`IF NOT EXISTS`, `ON CONFLICT DO NOTHING`)
- Soluția este complet automată și nu necesită modificări în codul migrărilor
