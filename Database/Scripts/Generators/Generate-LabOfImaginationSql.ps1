param(
    [string]$BaseLocale = "ro-ro",
    [string]$OutputPath = "../V0008__seed_lab_of_imagination.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$labRoot = Join-Path $repoRoot "XooCreator.BA" "Data" "SeedData" "LaboratoryOfImagination" "i18n"
$timestamp = "2025-01-01T00:00:00Z"
$uuidNamespace = "00000000-0000-0000-0000-000000000000"
. (Join-Path $scriptRoot "GuidUtils.ps1")

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
}

function Sql-Literal {
    param([string]$Value, [switch]$AllowNull)
    if ([string]::IsNullOrWhiteSpace($Value)) {
        if ($AllowNull) { return "NULL" }
        return "''"
    }
    return "'" + $Value.Replace("'", "''") + "'"
}

function Format-Bool {
    param([bool]$Value)
    if ($Value) { return "TRUE" }
    return "FALSE"
}

function Load-JsonArray {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        throw "Seed data file not found: $Path"
    }
    return Get-Content $Path -Raw | ConvertFrom-Json
}

function Normalize-AssetPath {
    param([string]$Value)
    if ([string]::IsNullOrWhiteSpace($Value)) { return $null }
    return $Value.Trim()
}

function New-GuidLiteral {
    param([string]$Key)
    return Get-GuidLiteral -NamespaceGuid $uuidNamespace -Name $Key
}

if (-not (Test-Path $labRoot)) {
    throw "LaboratoryOfImagination seed directory not found: $labRoot"
}

$baseDir = Join-Path $labRoot $BaseLocale
if (-not (Test-Path $baseDir)) {
    throw "Base locale directory not found: $baseDir"
}

$bodyPartsBase = Load-JsonArray (Join-Path $baseDir "bodyparts.json")
$regionsBase = Load-JsonArray (Join-Path $baseDir "regions.json")
$animalsBase = Load-JsonArray (Join-Path $baseDir "animals.json")
$supportsBase = Load-JsonArray (Join-Path $baseDir "animal-part-supports.json")

$localeDirs = Get-ChildItem -Path $labRoot -Directory
$bodyPartTranslationsByLocale = @{}
$animalTranslationsByLocale = @{}

foreach ($dir in $localeDirs) {
    $locale = $dir.Name

    $bodypartsPath = Join-Path $dir.FullName "bodyparts.json"
    if (Test-Path $bodypartsPath) {
        $parts = Load-JsonArray $bodypartsPath
        $map = @{}
        foreach ($part in $parts) {
            if ($null -eq $part) { continue }
            $key = [string]$part.key
            if ([string]::IsNullOrWhiteSpace($key)) { continue }
            $map[$key] = [string]$part.name
        }
        if ($map.Count -gt 0) {
            $bodyPartTranslationsByLocale[$locale] = $map
        }
    }

    $animalsPath = Join-Path $dir.FullName "animals.json"
    if (Test-Path $animalsPath) {
        $animals = Load-JsonArray $animalsPath
        $map = @{}
        foreach ($animal in $animals) {
            if ($null -eq $animal) { continue }
            $id = [string]$animal.id
            if ([string]::IsNullOrWhiteSpace($id)) { continue }
            $map[$id] = [string]$animal.label
        }
        if ($map.Count -gt 0) {
            $animalTranslationsByLocale[$locale] = $map
        }
    }
}

$lines = New-Object System.Collections.Generic.List[string]
$runStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"
$lines.Add("-- Auto-generated from Data/SeedData/LaboratoryOfImagination")
$lines.Add("-- Run date: $runStamp")
$lines.Add("")
$lines.Add("BEGIN;")
$lines.Add("")

# BodyParts
$lines.Add("-- Body parts (base locale)")
foreach ($part in ($bodyPartsBase | Sort-Object { $_.key })) {
    if ($null -eq $part) { continue }
    $key = [string]$part.key
    if ([string]::IsNullOrWhiteSpace($key)) { continue }
    $name = [string]$part.name
    $image = Normalize-AssetPath $part.image
    $isLocked = [bool]($part.isBaseLocked -eq $true)
    $lines.Add(@"
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('$key', $(Sql-Literal $name), $(Sql-Literal $image), $(Format-Bool $isLocked))
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
"@.Trim())
}
$lines.Add("")

# BodyPart translations
$lines.Add("-- Body part translations per locale")
$sortedBodyLocales = $bodyPartTranslationsByLocale.Keys | Sort-Object
foreach ($locale in $sortedBodyLocales) {
    $map = $bodyPartTranslationsByLocale[$locale]
    foreach ($key in ($map.Keys | Sort-Object)) {
        $name = $map[$key]
        $uuidExpr = New-GuidLiteral "bodypart:${key}:${locale}"
        $lines.Add(@"
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ($uuidExpr, '$key', '$locale', $(Sql-Literal $name))
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
"@.Trim())
    }
}
$lines.Add("")

# Regions
$lines.Add("-- Regions (base locale)")
foreach ($region in ($regionsBase | Sort-Object { $_.id })) {
    if ($null -eq $region) { continue }
    $id = [string]$region.id
    if ([string]::IsNullOrWhiteSpace($id)) { continue }
    $name = [string]$region.name
    $lines.Add(@"
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('$id', $(Sql-Literal $name))
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
"@.Trim())
}
$lines.Add("")

# Animals
$lines.Add("-- Animals (base locale)")
foreach ($animal in ($animalsBase | Sort-Object { $_.id })) {
    if ($null -eq $animal) { continue }
    $id = [string]$animal.id
    if ([string]::IsNullOrWhiteSpace($id)) { continue }
    $label = [string]$animal.label
    $src = Normalize-AssetPath $animal.src
    $regionId = [string]$animal.regionId
    if ([string]::IsNullOrWhiteSpace($regionId)) {
        throw "Animal '$id' missing regionId"
    }
    $isHybrid = [bool]($animal.isHybrid -eq $true)
    $lines.Add(@"
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('$id', $(Sql-Literal $label), $(Sql-Literal $src), $(Format-Bool $isHybrid), '$regionId')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
"@.Trim())
}
$lines.Add("")

# Animal translations
$lines.Add("-- Animal translations per locale")
$sortedAnimalLocales = $animalTranslationsByLocale.Keys | Sort-Object
foreach ($locale in $sortedAnimalLocales) {
    $map = $animalTranslationsByLocale[$locale]
    foreach ($animalId in ($map.Keys | Sort-Object)) {
        $label = $map[$animalId]
        $uuidExpr = New-GuidLiteral "animal:${animalId}:${locale}"
        $lines.Add(@"
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ($uuidExpr, '$animalId', '$locale', $(Sql-Literal $label))
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
"@.Trim())
    }
}
$lines.Add("")

# Animal part supports
$lines.Add("-- Animal part supports (base locale)")
foreach ($support in ($supportsBase | Sort-Object @{Expression = { $_.animalId }}, @{Expression = { $_.partKey }})) {
    if ($null -eq $support) { continue }
    $animalId = [string]$support.animalId
    $partKey = [string]$support.partKey
    if ([string]::IsNullOrWhiteSpace($animalId) -or [string]::IsNullOrWhiteSpace($partKey)) { continue }
    $lines.Add(@"
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('$animalId', '$partKey')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
"@.Trim())
}
$lines.Add("")

$lines.Add("COMMIT;")

$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
[System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.Encoding]::UTF8)
Write-Host "Generated Laboratory of Imagination SQL at $outputFullPath"

