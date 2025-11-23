# Soluție Robustă pentru Migrări Incrementale

## Prezentare Generală

Această soluție oferă un sistem robust pentru aplicarea migrărilor de bază de date în mod incremental, fără a recrea baza de date și fără a depinde de gestionarea excepțiilor. Soluția folosește operații SQL idempotente (`IF NOT EXISTS`) pentru a asigura că migrările pot fi rulate în siguranță de mai multe ori.

## Componente

### 1. IDatabaseMigrationService / DatabaseMigrationService

Serviciu care gestionează migrările de bază de date:

- **`ApplyMigrationsAsync()`** - Aplică toate migrările pendente incremental
- **`GetPendingMigrationsAsync()`** - Returnează lista migrărilor care nu au fost încă aplicate
- **`GetAppliedMigrationsAsync()`** - Returnează lista migrărilor deja aplicate
- **`EnsureMigrationsHistoryTableExistsAsync()`** - Asigură existența tabelei de istoric (idempotent)

**Caracteristici:**
- Folosește metodele built-in din EF Core (`GetPendingMigrationsAsync`, `MigrateAsync`)
- Logging detaliat pentru fiecare pas
- Gestionare robustă a erorilor cu logging informativ
- Nu depinde de excepții pentru funcționare

### 2. MigrationHelpers (Îmbunătățit)

Clasă de helper-uri pentru crearea de migrări idempotente:

**Operații disponibile:**
- `CreateTableIfNotExists()` - Creează tabelă doar dacă nu există
- `CreateIndexIfNotExists()` - Creează index doar dacă nu există
- `AddColumnIfNotExists()` - Adaugă coloană doar dacă nu există
- `DropTableIfExists()` - Șterge tabelă doar dacă există
- `DropIndexIfExists()` - Șterge index doar dacă există
- `InsertDataIfNotExists()` - Inserează date doar dacă nu există (ON CONFLICT DO NOTHING)
- `AddPrimaryKeyIfNotExists()` - Adaugă cheie primară doar dacă nu există
- `AddForeignKeyIfNotExists()` - Adaugă cheie străină doar dacă nu există
- `AddUniqueConstraintIfNotExists()` - Adaugă constraint unic doar dacă nu există
- `AddCheckConstraintIfNotExists()` - Adaugă constraint de verificare doar dacă nu există
- `DropConstraintIfExists()` - Șterge constraint doar dacă există
- `DropColumnIfExists()` - Șterge coloană doar dacă există
- `RenameColumnIfExists()` - Redenumește coloană doar dacă există
- `AlterColumnTypeIfExists()` - Modifică tipul coloanei doar dacă există
- `SetColumnDefaultIfExists()` - Setează valoare implicită doar dacă coloana există
- `DropColumnDefaultIfExists()` - Șterge valoare implicită doar dacă coloana există

**Toate operațiile sunt idempotente** - pot fi rulate de mai multe ori fără erori.

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

Pentru migrări noi, folosiți `MigrationHelpers` pentru operații idempotente:

```csharp
using XooCreator.BA.Data;

public partial class MyMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Creează tabelă idempotent
        MigrationHelpers.CreateTableIfNotExists(
            migrationBuilder,
            "MyTable",
            @"""Id"" uuid NOT NULL,
             ""Name"" character varying(100) NOT NULL,
             CONSTRAINT ""PK_MyTable"" PRIMARY KEY (""Id"")"
        );

        // Creează index idempotent
        MigrationHelpers.CreateIndexIfNotExists(
            migrationBuilder,
            "IX_MyTable_Name",
            "MyTable",
            @"""Name"""
        );

        // Adaugă coloană idempotent
        MigrationHelpers.AddColumnIfNotExists(
            migrationBuilder,
            "MyTable",
            "Description",
            "character varying(500)"
        );

        // Inserează date idempotent
        MigrationHelpers.InsertDataIfNotExists(
            migrationBuilder,
            "MyTable",
            "Id",
            @"(""Id"", ""Name"")
             VALUES 
                 ('...', 'Value1'),
                 ('...', 'Value2')"
        );
    }
}
```

## Avantaje

1. **Robust** - Nu depinde de excepții pentru funcționare
2. **Idempotent** - Migrările pot fi rulate de mai multe ori în siguranță
3. **Incremental** - Aplică doar migrările noi, nu recrează baza de date
4. **Logging detaliat** - Fiecare pas este logat pentru debugging
5. **Generic** - Funcționează cu orice migrare care folosește operații idempotente
6. **Production-ready** - Gestionare robustă a erorilor și logging informativ

## Best Practices

1. **Folosiți întotdeauna operații idempotente** în migrări noi:
   - `CREATE TABLE IF NOT EXISTS`
   - `CREATE INDEX IF NOT EXISTS`
   - `ALTER TABLE ... ADD COLUMN IF NOT EXISTS`
   - `INSERT ... ON CONFLICT DO NOTHING`

2. **Testați migrările de mai multe ori**:
   - Rulați migrarea
   - Rulați din nou - nu ar trebui să dea erori

3. **Folosiți MigrationHelpers** pentru toate operațiile în migrări noi

4. **Nu recreați baza de date în producție** - setați `Database:RecreateOnStart` la `false`

## Configurare

Serviciul este înregistrat automat în `ServiceCollectionExtensions.AddInfrastructureServices()`:

```csharp
services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
```

Nu este nevoie de configurare suplimentară - funcționează out-of-the-box.

## Note

- Soluția folosește tabelele de istoric EF Core (`__EFMigrationsHistory`) pentru a ține evidența migrărilor aplicate
- Dacă o migrare eșuează, se loghează care migrări au fost aplicate înainte de eșec
- Migrările vechi care nu folosesc operații idempotente pot cauza erori dacă sunt rulate de mai multe ori - convertiți-le folosind `MigrationHelpers`

