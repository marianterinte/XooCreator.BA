# Migration Idempotency Guide

> **⚠️ TEMPORARILY DISABLED:** This solution is temporarily disabled for debugging. Currently using simple logic with `recreate=true` and direct `MigrateAsync()`.

## Problems
When running migrations, you may encounter errors like:

1. **Table already exists:**
```
Npgsql.PostgresException: '42P07: relation "TableName" already exists'
```

2. **Column already exists:**
```
Npgsql.PostgresException: '42710: column "ColumnName" of relation "TableName" already exists'
```

3. **Duplicate key violations:**
```
Npgsql.PostgresException: '23505: duplicate key value violates unique constraint "IX_TableName_ColumnName"'
```

4. **Column does not exist (when querying):**
```
Npgsql.PostgresException: '42703: column s.ColumnName does not exist'
```

These happen when:
- Tables were created manually or through a different migration path, but the migration still tries to create them
- Columns were added in a newer migration but the table already exists from an older migration
- Seed data is inserted multiple times, violating unique constraints
- Migrations are run multiple times or in different orders

## Solution: Automatic Idempotent Migrations

**✅ GOOD NEWS:** We have an **automatic interceptor** that makes ALL migrations idempotent without requiring any code changes!

### Automatic Transformation (IdempotentMigrationCommandInterceptor)

The `IdempotentMigrationCommandInterceptor` automatically transforms SQL commands to be idempotent:

1. **`CREATE TABLE`** → Wrapped in `DO $$` block that:
   - Creates table if it doesn't exist
   - Adds missing columns if table exists

2. **`CREATE INDEX`** → Wrapped in `DO $$` block that:
   - Checks if all columns exist before creating index
   - Creates index only if it doesn't exist

3. **`ALTER TABLE ... ADD COLUMN`** → `ALTER TABLE ... ADD COLUMN IF NOT EXISTS`

4. **`ALTER TABLE ... ADD CONSTRAINT`** → Wrapped in `DO $$` block that checks if constraint exists

5. **`INSERT INTO`** → `INSERT INTO ... ON CONFLICT DO NOTHING` (handles all unique constraints)

6. **`CREATE SEQUENCE`** → `CREATE SEQUENCE IF NOT EXISTS`

7. **`CREATE TYPE`** → `CREATE TYPE IF NOT EXISTS`

**This means you can write migrations normally using EF Core's standard methods, and they will automatically be idempotent!**

## Use Cases - What Works Automatically

### ✅ Use Case 1: Adding a New Nullable Column to Existing Table

**Scenario:** You add a new nullable column to an existing table that already has data.

**Migration Code (Standard EF Core):**
```csharp
migrationBuilder.AddColumn<Guid?>(
    name: "NewColumn",
    table: "MyTable",
    type: "uuid",
    nullable: true);
```

**What Happens:**
- Interceptor transforms to: `ALTER TABLE "MyTable" ADD COLUMN IF NOT EXISTS "NewColumn" uuid NULL;`
- ✅ Works perfectly - column is added only if it doesn't exist
- ✅ Safe to run multiple times
- ✅ Works even if table has existing data

### ✅ Use Case 2: Creating a New Table

**Scenario:** You create a completely new table.

**Migration Code (Standard EF Core):**
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

**What Happens:**
- Interceptor transforms to: `CREATE TABLE IF NOT EXISTS "MyNewTable" ...`
- ✅ Works perfectly - table is created only if it doesn't exist
- ✅ Safe to run multiple times
- ✅ If table exists, missing columns are automatically added

### ✅ Use Case 3: Modifying Existing Table (Adding Columns)

**Scenario:** You modify an existing table by adding new columns (e.g., `ClassicAuthorId` to `StoryDefinitions`).

**Migration Code (Standard EF Core):**
```csharp
migrationBuilder.CreateTable(
    name: "StoryDefinitions",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        StoryId = table.Column<string>(type: "text", nullable: false),
        ClassicAuthorId = table.Column<Guid>(type: "uuid", nullable: true), // NEW COLUMN
        // ... other columns
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_StoryDefinitions", x => x.Id);
    });
```

**What Happens:**
- Interceptor detects table exists
- Automatically adds only the missing `ClassicAuthorId` column
- ✅ Works perfectly - no errors even if table exists
- ✅ Safe to run multiple times
- ✅ All existing data is preserved

### ✅ Use Case 4: Inserting Seed Data

**Scenario:** You insert seed data that might already exist.

**Migration Code (Standard EF Core):**
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

**What Happens:**
- Interceptor transforms to: `INSERT INTO "MyTable" ... ON CONFLICT DO NOTHING;`
- ✅ Works perfectly - handles conflicts on primary key AND unique constraints
- ✅ Safe to run multiple times
- ✅ No duplicate key violations

### ✅ Use Case 5: Creating Indexes

**Scenario:** You create an index on a column that might not exist yet.

**Migration Code (Standard EF Core):**
```csharp
migrationBuilder.CreateIndex(
    name: "IX_MyTable_ColumnName",
    table: "MyTable",
    column: "ColumnName");
```

**What Happens:**
- Interceptor checks if column exists before creating index
- Creates index only if it doesn't exist
- ✅ Works perfectly - no errors if column doesn't exist or index already exists
- ✅ Safe to run multiple times

### ⚠️ Use Case 6: Adding NOT NULL Column Without Default

**Scenario:** You add a NOT NULL column without a default value to a table with existing data.

**Migration Code:**
```csharp
migrationBuilder.AddColumn<string>(
    name: "RequiredColumn",
    table: "MyTable",
    type: "character varying(100)",
    maxLength: 100,
    nullable: false); // NOT NULL, no default
```

**What Happens:**
- PostgreSQL will fail because existing rows would have NULL values
- ❌ This is expected PostgreSQL behavior - you MUST provide a default value

**Solution:**
```csharp
migrationBuilder.AddColumn<string>(
    name: "RequiredColumn",
    table: "MyTable",
    type: "character varying(100)",
    maxLength: 100,
    nullable: false,
    defaultValue: ""); // Provide default value
```

## Manual Approach (For Reference)

If you want to manually create idempotent migrations (not required with interceptor):

### Approach 1: Use MigrationHelpers

For new migrations, you can use the `MigrationHelpers` class:

```csharp
using XooCreator.BA.Data;

MigrationHelpers.CreateTableIfNotExists(
    migrationBuilder,
    "MyTable",
    @"""Id"" uuid NOT NULL,
     ""Name"" character varying(100) NOT NULL,
     CONSTRAINT ""PK_MyTable"" PRIMARY KEY (""Id"")"
);

MigrationHelpers.AddColumnIfNotExists(
    migrationBuilder,
    "MyTable",
    "NewColumn",
    "character varying(100) NOT NULL DEFAULT ''"
);

MigrationHelpers.InsertDataIfNotExists(
    migrationBuilder,
    "MyTable",
    "Id",
    @"(""Id"", ""Name"")
     VALUES 
         ('...', 'Value1'),
         ('...', 'Value2')"
);
```

### Approach 2: Use Raw SQL with IF NOT EXISTS

```csharp
migrationBuilder.Sql(@"
    CREATE TABLE IF NOT EXISTS ""MyTable"" (
        ""Id"" uuid NOT NULL,
        ""Name"" character varying(100) NOT NULL,
        CONSTRAINT ""PK_MyTable"" PRIMARY KEY (""Id"")
    );
");

migrationBuilder.Sql(@"
    ALTER TABLE ""MyTable"" 
    ADD COLUMN IF NOT EXISTS ""NewColumn"" character varying(100) NOT NULL DEFAULT '';
");

migrationBuilder.Sql(@"
    INSERT INTO ""MyTable"" (""Id"", ""Name"")
    VALUES 
        ('...', 'Value1'),
        ('...', 'Value2')
    ON CONFLICT DO NOTHING;
");
```

## Best Practices

1. **Write migrations normally** - The interceptor handles idempotency automatically
2. **For NOT NULL columns, always provide a default value** if the table has existing data
3. **Test migrations multiple times**:
   - Run migration
   - Run again - should not error
4. **Use nullable columns when possible** for new columns in existing tables
5. **The interceptor is automatic** - no code changes needed in migrations

## Current Status

✅ **All migrations are automatically idempotent** via `IdempotentMigrationCommandInterceptor`

✅ **No manual conversion needed** - write migrations using standard EF Core methods

✅ **Works for all use cases:**
- Creating new tables
- Adding columns to existing tables (nullable or with defaults)
- Creating indexes
- Inserting seed data
- Adding constraints

## Notes

- Entity Framework Core's `MigrateAsync()` tracks applied migrations in `__EFMigrationsHistory`
- The interceptor is registered automatically in `Program.cs` when configuring `DbContext`
- If a migration fails partway through, check the logs for detailed error information
- The interceptor uses PostgreSQL-specific features (`IF NOT EXISTS`, `ON CONFLICT DO NOTHING`)
- All transformations are logged at Debug/Trace level for troubleshooting
