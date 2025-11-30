param(
    [string]$OutputPath = "../V0009__add_missing_hungarian_hero_translations.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path (Join-Path (Join-Path $scriptRoot "..") "..") "..")
$namespaceGuid = "00000000-0000-0000-0000-000000000000"
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
    return Get-GuidLiteral -NamespaceGuid $namespaceGuid -Name $Key
}

# Load hero tree structure
$structurePath = Get-FullPath "XooCreator.BA/Data/SeedData/SharedConfigs/hero-tree.json"
$structure = Load-JsonFile $structurePath
if ($null -eq $structure.nodes) {
    throw "Hero tree structure does not contain nodes."
}

# Load Hungarian translations
$huTranslationsPath = Get-FullPath "XooCreator.BA/Data/SeedData/BookOfHeroes/i18n/hu-HU/hero-tree.json"
$huTranslations = Load-JsonFile $huTranslationsPath
if ($huTranslations -isnot [System.Collections.IDictionary]) {
    # Convert to hashtable if it's a PSCustomObject
    $huDict = @{}
    $huTranslations.PSObject.Properties | ForEach-Object {
        $huDict[$_.Name] = $_.Value
    }
    $huTranslations = $huDict
}

function Get-TranslationValue {
    param($Dictionary, [string]$Key)
    if ($Dictionary -is [System.Collections.IDictionary]) {
        if ($Dictionary.Contains($Key)) {
            return $Dictionary[$Key]
        }
    }
    return $null
}

$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
$runDate = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("-- Auto-generated from Data/SeedData/BookOfHeroes/i18n/hu-HU/hero-tree.json")
$lines.Add("-- Run date: $runDate")
$lines.Add("-- This script adds/updates Hungarian translations for Tree of Heroes")
$lines.Add("-- It is idempotent: safe to run multiple times")
$lines.Add('')
$lines.Add('BEGIN;')
$lines.Add('')

$locale = "hu-hu"
$addedCount = 0
$skippedCount = 0

foreach ($node in $structure.nodes) {
    $heroId = $node.id
    if ([string]::IsNullOrWhiteSpace($heroId)) { continue }

    # Get translation keys from structure
    $nameKey = $node.nameKey
    $descriptionKey = $node.descriptionKey
    $storyKey = $node.storyKey

    # Get Hungarian translations
    $name = Get-TranslationValue $huTranslations $nameKey
    $description = Get-TranslationValue $huTranslations $descriptionKey
    $story = Get-TranslationValue $huTranslations $storyKey

    # Skip if no translations found
    if ([string]::IsNullOrWhiteSpace($name) -and 
        [string]::IsNullOrWhiteSpace($description) -and 
        [string]::IsNullOrWhiteSpace($story)) {
        Write-Host "WARNING: No Hungarian translations found for hero: $heroId" -ForegroundColor Yellow
        $skippedCount++
        continue
    }

    # Use fallback to key if translation is missing
    if ([string]::IsNullOrWhiteSpace($name)) { $name = $nameKey }
    if ([string]::IsNullOrWhiteSpace($description)) { $description = $descriptionKey }
    if ([string]::IsNullOrWhiteSpace($story)) { $story = $storyKey }

    # Escape SQL
    $nameEscaped = Escape-Sql $name
    $descriptionEscaped = Escape-Sql $description
    $storyEscaped = Escape-Sql $story

    # Generate deterministic GUID for translation
    $translationId = New-GuidLiteral "hero-def-tr:$heroId|$locale"

    # Generate INSERT ... ON CONFLICT DO UPDATE (idempotent)
    $translationSql = @"
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ($translationId, '$heroId', '$locale', '$nameEscaped', '$descriptionEscaped', '$storyEscaped')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";

"@
    $lines.Add($translationSql.Trim())
    $addedCount++
}

$lines.Add('COMMIT;')
$lines.Add('')

[System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.Encoding]::UTF8)

Write-Host ""
Write-Host "SUCCESS: Generated Hungarian hero translations SQL" -ForegroundColor Green
Write-Host "  File: $outputFullPath" -ForegroundColor Cyan
Write-Host "  Heroes processed: $addedCount" -ForegroundColor Cyan
if ($skippedCount -gt 0) {
    Write-Host "  Heroes skipped (no translations): $skippedCount" -ForegroundColor Yellow
}
Write-Host ""

