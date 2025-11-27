param(
    [string]$OutputPath = "../V0004__seed_hero_definitions.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$timestamp = "2025-01-01T00:00:00Z"
$namespaceGuid = "00000000-0000-0000-0000-000000000000"
$invariant = [System.Globalization.CultureInfo]::InvariantCulture

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
}

function Load-JsonFile {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        throw "Missing JSON file: $Path"
    }
    return Get-Content $Path -Raw | ConvertFrom-Json
}

function Escape-Sql {
    param([string]$Value)
    if ($null -eq $Value) { return "" }
    return $Value.Replace("'", "''")
}

function Get-GuidExpr {
    param([string]$Key)
    return "uuid_generate_v5('$namespaceGuid', '$Key')"
}

function To-JsonLiteral {
    param([object]$Value)
    if ($null -eq $Value) { return "[]" }
    $json = ConvertTo-Json -InputObject $Value -Compress
    if ($json -eq "null") { return "[]" }
    if ([string]::IsNullOrWhiteSpace($json)) { return "[]" }
    return $json
}

$structurePath = Get-FullPath "XooCreator.BA/Data/SeedData/SharedConfigs/hero-tree.json"
$structure = Load-JsonFile $structurePath
if ($null -eq $structure.nodes) {
    throw "Hero tree structure does not contain nodes."
}

$translationsBase = Get-FullPath "XooCreator.BA/Data/SeedData/BookOfHeroes/i18n"
$localeDirs = Get-ChildItem $translationsBase -Directory
if ($localeDirs.Count -eq 0) {
    throw "No locale directories found under $translationsBase"
}

$translations = @{}
foreach ($dir in $localeDirs) {
    $locale = $dir.Name
    $file = Join-Path $dir.FullName "hero-tree.json"
    if (Test-Path $file) {
        $translations[$locale] = Get-Content $file -Raw | ConvertFrom-Json -AsHashtable
    }
    else {
        $translations[$locale] = @{}
    }
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

function Get-PropValue {
    param($Object, [string]$Name)
    if ($Object -is [System.Collections.IDictionary]) {
        if ($Object.Contains($Name)) {
            return $Object[$Name]
        }
    }
    else {
        $prop = $Object.PSObject.Properties[$Name]
        if ($prop) { return $prop.Value }
    }
    return $null
}

function Get-CostValue {
    param($Costs, [string]$Name)
    $value = Get-PropValue $Costs $Name
    if ($null -eq $value) { return 0 }
    return [int]$value
}

$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
$lines = New-Object System.Collections.Generic.List[string]
$runStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"
$lines.Add("-- Auto-generated from Data/SeedData/SharedConfigs/hero-tree.json + BookOfHeroes/i18n/*/hero-tree.json")
$lines.Add("-- Run date: $runStamp")
$lines.Add('')
$lines.Add('DO $$')
$lines.Add('BEGIN')
$lines.Add("    IF NOT EXISTS (")
$lines.Add("        SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp'")
$lines.Add("    ) THEN")
$lines.Add("        CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";")
$lines.Add("    END IF;")
$lines.Add('END $$;')
$lines.Add('')
$lines.Add('BEGIN;')
$lines.Add('')
$lines.Add('-- Hero Definitions')

foreach ($node in $structure.nodes) {
    $heroId = $node.id
    if ([string]::IsNullOrWhiteSpace($heroId)) { continue }

    $costs = Get-PropValue $node 'costs'
    $courage = Get-CostValue $costs 'courage'
    $curiosity = Get-CostValue $costs 'curiosity'
    $thinking = Get-CostValue $costs 'thinking'
    $creativity = Get-CostValue $costs 'creativity'
    $safety = Get-CostValue $costs 'safety'

    $prerequisites = Get-PropValue $node 'prerequisites'
    $rewards = Get-PropValue $node 'rewards'
    $prereqJson = To-JsonLiteral ($prerequisites ?? @())
    $rewardsJson = To-JsonLiteral ($rewards ?? @())

    $posXValue = Get-PropValue $node 'positionX'
    if ($null -eq $posXValue) { $posXValue = 0 }
    $posYValue = Get-PropValue $node 'positionY'
    if ($null -eq $posYValue) { $posYValue = 0 }
    $posX = [System.Convert]::ToDouble($posXValue, $invariant)
    $posY = [System.Convert]::ToDouble($posYValue, $invariant)
    $posXStr = $posX.ToString("0.###", $invariant)
    $posYStr = $posY.ToString("0.###", $invariant)

    $image = Escape-Sql ($node.image ?? "")
    $type = Escape-Sql ($node.type ?? "hero")
    $prereqLiteral = Escape-Sql $prereqJson
    $rewardsLiteral = Escape-Sql $rewardsJson
    $isUnlocked = if ((Get-PropValue $node 'isUnlocked')) { "TRUE" } else { "FALSE" }

    $sql = @"
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('$heroId', '$type', $courage, $curiosity, $thinking, $creativity, $safety,
     '$prereqLiteral', '$rewardsLiteral', $isUnlocked, $posXStr, $posYStr, '$image', '$timestamp', '$timestamp')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
    $lines.Add($sql.Trim())

    foreach ($locale in ($translations.Keys | Sort-Object)) {
        $dict = $translations[$locale]
        $name = Escape-Sql (Get-TranslationValue $dict $node.nameKey) 
        if ([string]::IsNullOrWhiteSpace($name)) { $name = Escape-Sql $node.nameKey }
        $description = Escape-Sql (Get-TranslationValue $dict $node.descriptionKey)
        if ([string]::IsNullOrWhiteSpace($description)) { $description = Escape-Sql $node.descriptionKey }
        $story = Escape-Sql (Get-TranslationValue $dict $node.storyKey)
        if ([string]::IsNullOrWhiteSpace($story)) { $story = Escape-Sql $node.storyKey }

        $translationId = Get-GuidExpr "hero-def-tr:$heroId|$locale"
        $translationSql = @"
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ($translationId, '$heroId', '$locale', '$name', '$description', '$story')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
"@
        $lines.Add($translationSql.Trim())
    }
}

$lines.Add('COMMIT;')

[System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.Encoding]::UTF8)
Write-Host "Generated hero definitions SQL at $outputFullPath"

