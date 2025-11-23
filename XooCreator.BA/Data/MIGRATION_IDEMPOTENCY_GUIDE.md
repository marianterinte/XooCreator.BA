# Migration Idempotency Guide

## Problems
When running migrations, you may encounter errors like:

1. **Table already exists:**
```
Npgsql.PostgresException: '42P07: relation "TableName" already exists'
```

2. **Duplicate key violations:**
```
Npgsql.PostgresException: '23505: duplicate key value violates unique constraint "IX_TableName_ColumnName"'
```

These happen when:
- Tables were created manually or through a different migration path, but the migration still tries to create them
- Seed data is inserted multiple times, violating unique constraints

## Solution: Make Migrations Idempotent

Migrations should be **idempotent** - they can be run multiple times safely without errors.

### Approach 1: Use SQL `IF NOT EXISTS` (Recommended)

Convert `CreateTable` calls to SQL raw with `CREATE TABLE IF NOT EXISTS`:

**Before:**
```csharp
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
```

**After:**
```csharp
migrationBuilder.Sql(@"
    CREATE TABLE IF NOT EXISTS ""MyTable"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_MyTable"" PRIMARY KEY (""Id"")
    );
");
```

### Approach 2: Use MigrationHelpers (For New Migrations)

For new migrations, use the `MigrationHelpers` class:

```csharp
using XooCreator.BA.Data;

MigrationHelpers.CreateTableIfNotExists(
    migrationBuilder,
    "MyTable",
    @"""Id"" uuid NOT NULL,
     ""Name"" character varying(100) NOT NULL,
     CONSTRAINT ""PK_MyTable"" PRIMARY KEY (""Id"")"
);
```

### Approach 3: Add Columns Idempotently

When adding columns in future migrations:

```csharp
MigrationHelpers.AddColumnIfNotExists(
    migrationBuilder,
    "MyTable",
    "NewColumn",
    "character varying(100) NOT NULL DEFAULT ''"
);
```

### Approach 4: Insert Data Idempotently

When inserting seed data, use `ON CONFLICT DO NOTHING` to prevent duplicate key errors:

**Before:**
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

**After (Single Column Unique Constraint):**
```csharp
// For primary key or single-column unique index
migrationBuilder.Sql(@"
    INSERT INTO ""MyTable"" (""Id"", ""Name"")
    VALUES 
        ('...', 'Value1'),
        ('...', 'Value2')
    ON CONFLICT (""Id"") DO NOTHING;
");
```

**After (Composite Unique Constraint):**
```csharp
// For composite unique index (e.g., HeroId + RegionId)
migrationBuilder.Sql(@"
    INSERT INTO ""HeroMessages"" (""Id"", ""HeroId"", ""RegionId"", ""MessageKey"")
    VALUES 
        ('...', 'hero1', 'region1', 'message1'),
        ('...', 'hero2', 'region2', 'message2')
    ON CONFLICT (""HeroId"", ""RegionId"") DO NOTHING;
");
```

**Using MigrationHelpers:**
```csharp
// Single column
MigrationHelpers.InsertDataIfNotExists(
    migrationBuilder,
    "MyTable",
    "Id",  // conflict column
    @"(""Id"", ""Name"")
     VALUES 
         ('...', 'Value1'),
         ('...', 'Value2')"
);

// Composite columns
MigrationHelpers.InsertDataIfNotExists(
    migrationBuilder,
    "HeroMessages",
    new[] { "HeroId", "RegionId" },  // conflict columns
    @"(""Id"", ""HeroId"", ""RegionId"", ""MessageKey"")
     VALUES 
         ('...', 'hero1', 'region1', 'message1'),
         ('...', 'hero2', 'region2', 'message2')"
);
```

**Important:** Always check which unique constraint is being violated:
- If it's a primary key, use the primary key column(s)
- If it's a unique index, use the index column(s) as shown in the error message
- For composite indexes, use all columns in the same order as defined in the index

## Current Status

The migration `20251122223257_authors.cs` has been partially converted:
- ✅ `AlchimaliaUsers` - converted to `CREATE TABLE IF NOT EXISTS`
- ✅ `BestiaryItems` - converted to `CREATE TABLE IF NOT EXISTS`
- ⏳ Remaining 61 tables - need conversion

## Converting Remaining Tables

To convert the remaining tables, you can:

1. **Manual conversion** (recommended for critical tables):
   - Find each `CreateTable` call
   - Convert to SQL `CREATE TABLE IF NOT EXISTS`
   - Test the migration

2. **Batch conversion** (for all tables):
   - Use a script or tool to parse and convert all `CreateTable` calls
   - Review the generated SQL
   - Test thoroughly

## Best Practices for Future Migrations

1. **Always use idempotent operations**:
   - `CREATE TABLE IF NOT EXISTS`
   - `CREATE INDEX IF NOT EXISTS`
   - `ALTER TABLE ... ADD COLUMN IF NOT EXISTS` (via MigrationHelpers)

2. **Test migrations multiple times**:
   - Run migration
   - Rollback (if needed)
   - Run again - should not error

3. **Use MigrationHelpers for new migrations**:
   ```csharp
   MigrationHelpers.CreateTableIfNotExists(...)
   MigrationHelpers.CreateIndexIfNotExists(...)
   MigrationHelpers.AddColumnIfNotExists(...)
   MigrationHelpers.InsertDataIfNotExists(...)  // For seed data
   ```

4. **For InsertData operations**:
   - Always use `ON CONFLICT DO NOTHING` with the appropriate unique constraint
   - Check the error message to identify which constraint is violated
   - Use the exact column names from the unique index/constraint
   - For composite constraints, include all columns in the same order

## Notes

- Entity Framework Core's `MigrateAsync()` tracks applied migrations in `__EFMigrationsHistory`
- If a migration fails partway through, you may need to manually mark it as applied or fix the issue
- Idempotent migrations allow you to re-run them safely, even if some objects already exist

