param(
    [string]$OutputPath = "../V00120__seed_bestiary_item_translations.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")

$i18nRoot = Join-Path $repoRoot "XooCreator.BA" "Data" "SeedData" "Discovery" "i18n"
$namespaceGuid = "00000000-0000-0000-0000-000000000000"
. (Join-Path $scriptRoot "GuidUtils.ps1")

$locales = @("en-us", "ro-ro", "hu-hu")
$fileName = "discover-bestiary.json"

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
}

function Load-LocaleCreatures {
    param([string]$Locale)

    $path = Join-Path (Join-Path $i18nRoot $Locale) $fileName
    if (-not (Test-Path $path)) {
        throw "Seed data file not found for locale '$Locale': $path"
    }

    $json = Get-Content $path -Raw -Encoding UTF8 | ConvertFrom-Json
    if ($null -eq $json) {
        throw "Seed data file for locale '$Locale' is empty or invalid JSON: $path"
    }

    $map = @{}
    foreach ($item in $json) {
        if ($null -eq $item) { continue }
        $combination = [string]$item.Combination
        if ([string]::IsNullOrWhiteSpace($combination)) { continue }
        $map[$combination] = $item
    }

    return @{
        Locale = $Locale
        Path = $path
        Map = $map
    }
}

$tokens = @("Bunny", "Giraffe", "Hippo", "None")

function Split-Combination {
    param([string]$Combination)

    $result = @()
    $remaining = $Combination
    for ($i = 0; $i -lt 3; $i++) {
        $match = $null
        foreach ($token in $tokens) {
            if ($remaining.StartsWith($token, $true, [System.Globalization.CultureInfo]::InvariantCulture)) {
                $match = $token
                $remaining = $remaining.Substring($token.Length)
                break
            }
        }

        if (-not $match) {
            throw "Could not split combination '$Combination'"
        }

        if ($match -eq "None") {
            $match = "—"
        }

        $result += $match
    }

    return $result
}

function Escape-SqlLiteral {
    param([string]$Value, [switch]$AllowNull)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        if ($AllowNull) { return "NULL" }
        return "''"
    }

    $escaped = $Value.Replace("'", "''")
    return "'" + $escaped + "'"
}

if (-not (Test-Path $i18nRoot)) {
    throw "Discovery i18n directory not found: $i18nRoot"
}

Write-Host "Generating BestiaryItemTranslations SQL..." -ForegroundColor Cyan
Write-Host ""

# Load creatures per locale
$localeData = @{}
foreach ($locale in $locales) {
    $localeInfo = Load-LocaleCreatures -Locale $locale
    $localeData[$locale] = $localeInfo
    Write-Host "Loaded $($localeInfo.Map.Count) creatures for locale $locale from $($localeInfo.Path)" -ForegroundColor Gray
}

$enMap = $localeData["en-us"].Map

$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
$runDate = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("-- Auto-generated from Data/SeedData/Discovery/i18n/*/discover-bestiary.json")
$lines.Add("-- Locales: en-us, ro-ro, hu-hu")
$lines.Add("-- Run date: $runDate")
$lines.Add("-- This script seeds BestiaryItemTranslations for all discovery bestiary combinations.")
$lines.Add("-- It is idempotent: safe to run multiple times.")
$lines.Add("")
$lines.Add("BEGIN;")
$lines.Add("")

$insertCount = 0

# Use English as source of truth for the set of combinations
$sortedCombinations = $enMap.Keys | Sort-Object

foreach ($combination in $sortedCombinations) {
    $parts = Split-Combination -Combination $combination
    $arms = $parts[0]
    $body = $parts[1]
    $head = $parts[2]

    $comboKey = "$arms|$body|$head"

    # Bestiary item Id must match V0002__seed_bestiary_items.sql
    $bestiaryIdLiteral = Get-GuidLiteral -NamespaceGuid $namespaceGuid -Name ("bestiary:{0}" -f $comboKey)

    foreach ($locale in $locales) {
        $map = $localeData[$locale].Map
        if (-not $map.ContainsKey($combination)) {
            Write-Host ("WARNING: Locale '{0}' missing combination '{1}'" -f $locale, $combination) -ForegroundColor Yellow
            continue
        }

        $item = $map[$combination]
        $name = [string]$item.Name
        $story = [string]$item.Story

        $nameLiteral = Escape-SqlLiteral -Value $name
        $storyLiteral = Escape-SqlLiteral -Value $story

        # Deterministic Id per (BestiaryItem, Locale)
        $translationKey = "bestiary-tr:{0}|{1}" -f $comboKey, $locale
        $translationIdLiteral = Get-GuidLiteral -NamespaceGuid $namespaceGuid -Name $translationKey

        $sql = (
            "INSERT INTO alchimalia_schema.""BestiaryItemTranslations""`n" +
            "    (""Id"", ""BestiaryItemId"", ""LanguageCode"", ""Name"", ""Story"")`n" +
            "VALUES`n" +
            "    ({0}, {1}, '{2}', {3}, {4})`n" +
            "ON CONFLICT (""BestiaryItemId"", ""LanguageCode"") DO UPDATE`n" +
            "SET ""Name"" = EXCLUDED.""Name"",`n" +
            "    ""Story"" = EXCLUDED.""Story"";`n"
        ) -f $translationIdLiteral, $bestiaryIdLiteral, $locale, $nameLiteral, $storyLiteral

        $lines.Add($sql.Trim())
        $lines.Add("")
        $insertCount++
    }
}

$lines.Add("COMMIT;")
$lines.Add("")

[System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.Encoding]::UTF8)

Write-Host ""
Write-Host "SUCCESS: Generated BestiaryItemTranslations SQL" -ForegroundColor Green
Write-Host "  File: $outputFullPath" -ForegroundColor Cyan
Write-Host "  Insert/Upsert statements: $insertCount" -ForegroundColor Cyan
Write-Host ""
