# Generate SQL from JSON Guide

> **How to create PowerShell generators for data-driven SQL migration scripts**

## Overview

When you need to populate database tables from JSON files (translations, seed data, etc.), you can create PowerShell generator scripts that automatically generate SQL migration scripts.

## When to Use Generators

Use generators when:
- ✅ Data comes from JSON files (translations, configurations)
- ✅ You need to regenerate SQL when JSON files change
- ✅ You want to ensure consistency between JSON and SQL
- ✅ You have multiple locales/languages to process

**Don't use generators for:**
- ❌ One-time manual migrations
- ❌ Schema changes (use EF Core migrations)
- ❌ Simple data that doesn't change often

## Generator Script Location

All generator scripts are located in:
```
Database/Scripts/Generators/
```

## Generator Script Structure

### Basic Template

```powershell
param(
    [string]$OutputPath = "../V####__description.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path (Join-Path (Join-Path $scriptRoot "..") "..") "..")
. (Join-Path $scriptRoot "GuidUtils.ps1")

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
}

function Load-JsonFile {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        throw "Missing JSON file: $Path"
    }
    return Get-Content $Path -Raw -Encoding UTF8 | ConvertFrom-Json
}

function Escape-Sql {
    param([string]$Value)
    if ($null -eq $Value) { return "" }
    return $Value.Replace("'", "''")
}

function New-GuidLiteral {
    param([string]$Key)
    $namespaceGuid = "00000000-0000-0000-0000-000000000000"
    return Get-GuidLiteral -NamespaceGuid $namespaceGuid -Name $Key
}

# Load your JSON files
$jsonPath = Get-FullPath "XooCreator.BA/Data/SeedData/YourPath/your-file.json"
$jsonData = Load-JsonFile $jsonPath

# Generate SQL
$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
$runDate = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("-- Auto-generated from [source]")
$lines.Add("-- Run date: $runDate")
$lines.Add("-- Description")
$lines.Add('')
$lines.Add('BEGIN;')
$lines.Add('')

# Process data and generate INSERT statements
foreach ($item in $jsonData) {
    $id = New-GuidLiteral "your-namespace:$($item.id)"
    $name = Escape-Sql $item.name
    
    $sql = @"
INSERT INTO alchimalia_schema."YourTable"
    ("Id", "Name")
VALUES
    ($id, '$name')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";

"@
    $lines.Add($sql.Trim())
}

$lines.Add('COMMIT;')
$lines.Add('')

[System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.Encoding]::UTF8)

Write-Host "SUCCESS: Generated SQL at $outputFullPath" -ForegroundColor Green
```

## Key Components

### 1. Path Resolution

Always use `Get-FullPath` helper to resolve paths relative to repository root:

```powershell
function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
}

$jsonPath = Get-FullPath "XooCreator.BA/Data/SeedData/YourPath/file.json"
```

### 2. JSON Loading with UTF-8

Always specify UTF-8 encoding for JSON files (especially for translations):

```powershell
function Load-JsonFile {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        throw "Missing JSON file: $Path"
    }
    return Get-Content $Path -Raw -Encoding UTF8 | ConvertFrom-Json
}
```

### 3. SQL Escaping

Always escape single quotes in SQL strings:

```powershell
function Escape-Sql {
    param([string]$Value)
    if ($null -eq $Value) { return "" }
    return $Value.Replace("'", "''")
}

$nameEscaped = Escape-Sql $item.name
```

### 4. Deterministic GUIDs

Use `GuidUtils.ps1` for reproducible GUIDs:

```powershell
. (Join-Path $scriptRoot "GuidUtils.ps1")

function New-GuidLiteral {
    param([string]$Key)
    $namespaceGuid = "00000000-0000-0000-0000-000000000000"
    return Get-GuidLiteral -NamespaceGuid $namespaceGuid -Name $Key
}

$id = New-GuidLiteral "table-name:$($item.id)"
```

### 5. Idempotent INSERT Statements

Always use `ON CONFLICT DO UPDATE`:

```powershell
$sql = @"
INSERT INTO alchimalia_schema."TableName"
    ("Id", "Column1", "Column2")
VALUES
    ($id, '$value1', '$value2')
ON CONFLICT ("Id") DO UPDATE
SET "Column1" = EXCLUDED."Column1",
    "Column2" = EXCLUDED."Column2";

"@
```

For unique constraints on multiple columns:

```powershell
ON CONFLICT ("Column1", "Column2") DO UPDATE
SET "Id" = EXCLUDED."Id",
    "Column3" = EXCLUDED."Column3";
```

## Example: Translation Generator

See `Generate-HungarianHeroTranslationsSql.ps1` for a complete example:

1. **Loads structure** from `hero-tree.json` to get hero IDs
2. **Loads translations** from `hu-HU/hero-tree.json`
3. **Generates SQL** with idempotent INSERT statements
4. **Uses deterministic GUIDs** for translation records

## Running Generators

### Manual Execution

```powershell
cd Database/Scripts/Generators
.\Generate-YourScript.ps1
```

### With Custom Output Path

```powershell
.\Generate-YourScript.ps1 -OutputPath "../V0010__custom_name.sql"
```

## Generated SQL Format

The generated SQL should:

1. ✅ Start with comments describing source and date
2. ✅ Wrap everything in `BEGIN;` / `COMMIT;` transaction
3. ✅ Use `ON CONFLICT DO UPDATE` for all INSERTs
4. ✅ Use deterministic GUIDs for reproducible results
5. ✅ Escape all string values properly
6. ✅ Handle NULL values gracefully

## Best Practices

1. **Test generators locally** before committing
2. **Verify generated SQL** manually the first time
3. **Use consistent naming** for GUID namespaces
4. **Handle missing data** gracefully (skip or use defaults)
5. **Add helpful error messages** when files are missing
6. **Document the generator** with comments explaining the data structure
7. **Version the output** - update version number when regenerating

## Common Patterns

### Processing Multiple Locales

```powershell
$locales = @("en-us", "hu-hu", "ro-ro")
foreach ($locale in $locales) {
    $translationPath = Get-FullPath "XooCreator.BA/Data/SeedData/i18n/$locale/translations.json"
    $translations = Load-JsonFile $translationPath
    
    foreach ($key in $translations.PSObject.Properties.Name) {
        $value = $translations.$key
        # Generate SQL for this locale
    }
}
```

### Handling Nested JSON

```powershell
foreach ($node in $structure.nodes) {
    $heroId = $node.id
    $nameKey = $node.nameKey
    $descriptionKey = $node.descriptionKey
    
    $name = Get-TranslationValue $translations $nameKey
    $description = Get-TranslationValue $translations $descriptionKey
}
```

### Processing Arrays

```powershell
foreach ($item in $jsonData.items) {
    $id = New-GuidLiteral "namespace:$($item.id)"
    # Process item
}
```

## Troubleshooting

### Encoding Issues

If special characters appear corrupted:
- ✅ Ensure `-Encoding UTF8` when reading JSON
- ✅ Ensure `[System.Text.Encoding]::UTF8` when writing SQL
- ✅ PostgreSQL will read UTF-8 correctly even if editor shows corruption

### Path Resolution Errors

If paths don't resolve correctly:
- ✅ Use `Get-FullPath` helper function
- ✅ Verify repository root is correct
- ✅ Use forward slashes in paths (PowerShell handles them)

### GUID Generation Issues

If GUIDs are not deterministic:
- ✅ Use `GuidUtils.ps1` functions
- ✅ Use consistent namespace GUID
- ✅ Use consistent key format

## Related Files

- **GuidUtils.ps1**: Helper functions for deterministic GUID generation
- **Generate-HeroDefinitionsSql.ps1**: Example generator for hero definitions
- **Generate-HungarianHeroTranslationsSql.ps1**: Example generator for translations
- **Generate-BestiarySql.ps1**: Example generator for bestiary items

## Quick Reference

| Task | Code |
|------|------|
| Load JSON | `Load-JsonFile $path` |
| Escape SQL | `Escape-Sql $value` |
| Generate GUID | `New-GuidLiteral "namespace:key"` |
| Write SQL file | `[System.IO.File]::WriteAllLines($path, $lines, [System.Text.Encoding]::UTF8)` |
| Get repo root | `Resolve-Path (Join-Path (Join-Path (Join-Path $scriptRoot "..") "..") "..")` |

