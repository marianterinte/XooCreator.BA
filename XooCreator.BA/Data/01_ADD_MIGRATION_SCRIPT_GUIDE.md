# Add Migration Script Guide

> **How to add new SQL migration scripts to the database**

## Overview

This guide explains how to add new SQL migration scripts that will be automatically executed by the `XooCreator.DbScriptRunner` when you push to `DEV` or `main` branches.

## Automatic Execution

SQL scripts in `Database/Scripts/` are automatically executed by the CI/CD pipeline:

- **On push to `DEV`**: Scripts run on DEV database
- **On push to `main`**: Scripts run on PROD database
- **Workflow**: `.github/workflows/db-scripts.yml`

## Script Naming Convention

Scripts must follow the naming pattern: `V####__description.sql`

- **`V####`**: Sequential version number (e.g., `V0001`, `V0002`, `V0009`)
- **`__`**: Double underscore separator
- **`description`**: Descriptive name in snake_case (e.g., `add_missing_translations`)
- **`.sql`**: SQL file extension

**Examples:**
- `V0001__initial_full_schema.sql`
- `V0002__seed_bestiary_items.sql`
- `V0009__add_missing_hungarian_hero_translations.sql`

## Idempotency Requirement

**⚠️ IMPORTANT:** All migration scripts MUST be idempotent (safe to run multiple times).

### How to Make Scripts Idempotent

Use PostgreSQL's `ON CONFLICT` clause for INSERT statements:

```sql
INSERT INTO alchimalia_schema."TableName"
    ("Id", "Column1", "Column2")
VALUES
    ('guid-value', 'value1', 'value2')
ON CONFLICT ("Id") DO UPDATE
SET "Column1" = EXCLUDED."Column1",
    "Column2" = EXCLUDED."Column2";
```

For unique constraints on multiple columns:

```sql
INSERT INTO alchimalia_schema."TableName"
    ("Id", "Column1", "Column2")
VALUES
    ('guid-value', 'value1', 'value2')
ON CONFLICT ("Column1", "Column2") DO UPDATE
SET "Id" = EXCLUDED."Id";
```

### Other Idempotent Patterns

1. **CREATE TABLE**: Use `IF NOT EXISTS` (handled automatically by interceptor)
2. **ALTER TABLE ADD COLUMN**: Use `IF NOT EXISTS` (handled automatically by interceptor)
3. **CREATE INDEX**: Use `IF NOT EXISTS` (handled automatically by interceptor)
4. **INSERT**: Always use `ON CONFLICT DO UPDATE` or `ON CONFLICT DO NOTHING`

## Step-by-Step: Adding a New Migration Script

### Step 1: Create the SQL Script

Create a new file in `Database/Scripts/` with the next sequential version number:

```sql
-- Auto-generated from [source] or Manual migration
-- Run date: YYYY-MM-DD HH:mm:ss+02:00
-- Description: What this script does

BEGIN;

-- Your SQL statements here
-- Make sure all INSERT statements use ON CONFLICT

COMMIT;
```

### Step 2: Test Locally (Optional but Recommended)

Test the script locally before committing:

```bash
# Connect to your local database and run the script
psql -h localhost -U your_user -d your_database -f Database/Scripts/V####__your_script.sql
```

### Step 3: Commit and Push

```bash
git add Database/Scripts/V####__your_script.sql
git commit -m "Add migration script: V####__your_script"
git push origin DEV
```

### Step 4: Verify Execution

Check the GitHub Actions workflow run:
1. Go to your repository on GitHub
2. Click on "Actions" tab
3. Find the "Database Scripts Runner" workflow run
4. Verify the script executed successfully

## Example: Adding Translation Data

### Scenario: Add Hungarian translations for Tree of Heroes

1. **Create PowerShell generator** (optional, for data-driven scripts):
   - Location: `Database/Scripts/Generators/Generate-HungarianHeroTranslationsSql.ps1`
   - Purpose: Generate SQL from JSON translation files

2. **Run generator** (if applicable):
   ```powershell
   cd Database/Scripts/Generators
   .\Generate-HungarianHeroTranslationsSql.ps1
   ```
   This generates: `Database/Scripts/V0009__add_missing_hungarian_hero_translations.sql`

3. **Verify generated SQL**:
   - Check that all INSERT statements use `ON CONFLICT DO UPDATE`
   - Verify GUIDs are deterministic (using `GuidUtils.ps1`)

4. **Commit and push**:
   ```bash
   git add Database/Scripts/V0009__add_missing_hungarian_hero_translations.sql
   git commit -m "Add Hungarian hero translations migration"
   git push origin DEV
   ```

5. **Workflow automatically executes**:
   - `db-scripts.yml` detects the new script
   - `XooCreator.DbScriptRunner` checks if script was already applied (by checksum)
   - If not applied, executes the script
   - Records execution in `__SchemaVersions` table

## Script Execution Tracking

The `XooCreator.DbScriptRunner` tracks executed scripts in the `__SchemaVersions` table:

- **Script Name**: File name (e.g., `V0009__add_missing_hungarian_hero_translations.sql`)
- **Checksum**: Hash of script content
- **Applied At**: Timestamp when script was executed
- **Status**: `Succeeded` or `RolledBack`

If a script's content changes (checksum changes), it will be re-executed.

## Best Practices

1. **Always use transactions**: Wrap your script in `BEGIN;` and `COMMIT;`
2. **Make scripts idempotent**: Use `ON CONFLICT` for all INSERT statements
3. **Use deterministic GUIDs**: For reproducible data, use `GuidUtils.ps1` in generators
4. **Add comments**: Document what the script does and its source
5. **Test locally first**: Verify the script works before pushing
6. **Incremental changes**: Each script should represent a single logical change
7. **Version sequentially**: Don't skip version numbers

## Troubleshooting

### Script Not Executing

- **Check file name**: Must match pattern `V####__*.sql`
- **Check file location**: Must be in `Database/Scripts/` (not in subdirectories)
- **Check workflow logs**: Look for errors in GitHub Actions

### Script Execution Fails

- **Check idempotency**: Ensure all INSERT statements use `ON CONFLICT`
- **Check syntax**: Verify SQL syntax is correct
- **Check dependencies**: Ensure referenced tables/columns exist
- **Check constraints**: Verify unique constraints match `ON CONFLICT` clauses

### Script Executed Multiple Times

- **This is normal**: Scripts are designed to be idempotent
- **Check `__SchemaVersions`**: Verify script is tracked correctly
- **Check checksum**: If script content changed, it will re-execute

## Related Documentation

- **Migration Idempotency**: See `MIGRATION_IDEMPOTENCY_GUIDE.md` for EF Core migrations
- **Database Scripts Runner**: See `XooCreator.DbScriptRunner` project for implementation details
- **Workflow Configuration**: See `.github/workflows/db-scripts.yml` for CI/CD setup

## Quick Reference

| Action | Command/Location |
|--------|------------------|
| Create new script | `Database/Scripts/V####__description.sql` |
| Test locally | `psql -f Database/Scripts/V####__description.sql` |
| Generator scripts | `Database/Scripts/Generators/*.ps1` |
| Workflow config | `.github/workflows/db-scripts.yml` |
| Execution tracking | `__SchemaVersions` table in database |

