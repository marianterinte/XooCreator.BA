# Simple generator for BestiaryItemTranslations
$ErrorActionPreference = "Stop"

# Paths
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$i18nRoot = Join-Path $repoRoot "XooCreator.BA" "Data" "SeedData" "Discovery" "i18n"
$outputPath = Join-Path $scriptRoot ".." "V00120__seed_bestiary_item_translations.sql"

# Load locales
$locales = @("en-us", "ro-ro", "hu-hu")
$fileName = "discover-bestiary.json"

Write-Host "Loading JSON files..." -ForegroundColor Cyan

$localeData = @{}
foreach ($locale in $locales) {
    $path = Join-Path (Join-Path $i18nRoot $locale) $fileName
    $json = Get-Content $path -Raw -Encoding UTF8 | ConvertFrom-Json
    $map = @{}
    foreach ($item in $json) {
        $combination = [string]$item.Combination
        if ([string]::IsNullOrWhiteSpace($combination)) { continue }
        $map[$combination] = $item
    }
    $localeData[$locale] = $map
    Write-Host "Loaded $($map.Count) items for $locale" -ForegroundColor Gray
}

# Load V0002 to get BestiaryItem IDs
$v0002Path = Join-Path $scriptRoot ".." "V0002__seed_bestiary_items.sql"
$v0002Content = Get-Content $v0002Path -Raw -Encoding UTF8

# Parse BestiaryItem IDs
$bestiaryIds = @{}
$lines = $v0002Content -split "`n"
foreach ($line in $lines) {
    if ($line -match "VALUES\s*\('([^']+)',\s*'([^']+)',\s*'([^']+)',\s*'([^']+)") {
        $id = $matches[1]
        $arms = $matches[2]
        $body = $matches[3]
        $head = $matches[4]
        $comboKey = "$arms|$body|$head"
        $bestiaryIds[$comboKey] = $id
    }
}

Write-Host "Found $($bestiaryIds.Count) BestiaryItem IDs" -ForegroundColor Gray

# Generate SQL
$lines = @()
$lines += "-- Auto-generated from Data/SeedData/Discovery/i18n/*/discover-bestiary.json"
$lines += "-- Locales: en-us, ro-ro, hu-hu"
$lines += "-- Run date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ssK')"
$lines += "-- This script seeds BestiaryItemTranslations for all discovery bestiary combinations."
$lines += "-- It is idempotent: safe to run multiple times."
$lines += ""
$lines += "BEGIN;"
$lines += ""

$insertCount = 0
$enCombinations = $localeData["en-us"].Keys | Sort-Object

foreach ($combination in $enCombinations) {
    # Parse combination
    $parts = @()
    $remaining = $combination
    $tokens = @("Bunny", "Giraffe", "Hippo", "None")
    
    for ($i = 0; $i -lt 3; $i++) {
        foreach ($token in $tokens) {
            if ($remaining.StartsWith($token)) {
                if ($token -eq "None") {
                    $parts += "—"
                } else {
                    $parts += $token
                }
                $remaining = $remaining.Substring($token.Length)
                break
            }
        }
    }
    
    if ($parts.Count -ne 3) {
        Write-Host "Warning: Could not parse combination $combination" -ForegroundColor Yellow
        continue
    }
    
    $arms = $parts[0]
    $body = $parts[1]
    $head = $parts[2]
    $comboKey = "$arms|$body|$head"
    
    $bestiaryId = $bestiaryIds[$comboKey]
    if (-not $bestiaryId) {
        Write-Host "Warning: No BestiaryItem ID found for $comboKey" -ForegroundColor Yellow
        continue
    }
    
    foreach ($locale in $locales) {
        $map = $localeData[$locale]
        if (-not $map.ContainsKey($combination)) {
            Write-Host "Warning: Locale $locale missing combination $combination" -ForegroundColor Yellow
            continue
        }
        
        $item = $map[$combination]
        $name = [string]$item.Name
        $story = [string]$item.Story
        
        # Escape SQL literals
        $nameEscaped = $name.Replace("'", "''")
        $storyEscaped = $story.Replace("'", "''")
        
        # Generate UUID
        $translationId = [System.Guid]::NewGuid().ToString()
        
        $lines += "INSERT INTO alchimalia_schema.""BestiaryItemTranslations"""
        $lines += "    (""Id"", ""BestiaryItemId"", ""LanguageCode"", ""Name"", ""Story"")"
        $lines += "VALUES"
        $lines += "    ('$translationId', '$bestiaryId', '$locale', '$nameEscaped', '$storyEscaped')"
        $lines += "ON CONFLICT (""BestiaryItemId"", ""LanguageCode"") DO UPDATE"
        $lines += "SET ""Name"" = EXCLUDED.""Name"","
        $lines += "    ""Story"" = EXCLUDED.""Story"";"
        $lines += ""
        $insertCount++
    }
}

$lines += "COMMIT;"
$lines += ""

# Write file
[System.IO.File]::WriteAllLines($outputPath, $lines, [System.Text.Encoding]::UTF8)

Write-Host ""
Write-Host "SUCCESS: Generated BestiaryItemTranslations SQL" -ForegroundColor Green
Write-Host "  File: $outputPath" -ForegroundColor Cyan
Write-Host "  Insert/Upsert statements: $insertCount" -ForegroundColor Cyan
Write-Host ""
