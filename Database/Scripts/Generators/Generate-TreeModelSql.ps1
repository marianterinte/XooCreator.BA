param(
    [string]$OutputPath = "../V0007__seed_tree_model.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$timestamp = "2025-01-01T00:00:00Z"

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

function Normalize-ImagePath {
    param([string]$Path)
    if ([string]::IsNullOrWhiteSpace($Path)) { return $null }
    return $Path.TrimStart('/')
}

function Get-DeterministicInt {
    param([string]$Key)
    $md5 = [System.Security.Cryptography.MD5]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Key)
        $hash = $md5.ComputeHash($bytes)
        $value = [System.BitConverter]::ToInt32($hash, 0)
        if ($value -eq [int]::MinValue) { return 0 }
        return [Math]::Abs($value)
    }
    finally {
        $md5.Dispose()
    }
}

function Get-PropValue {
    param($Object, [string]$Name)
    if ($null -eq $Object) { return $null }
    if ($Object -is [System.Collections.IDictionary]) {
        if ($Object.Contains($Name)) { return $Object[$Name] }
        return $null
    }
    $prop = $Object.PSObject.Properties[$Name]
    if ($prop) { return $prop.Value }
    return $null
}

function Load-TreeSeeds {
    $seedsDir = Get-FullPath "XooCreator.BA/Data/SeedData/TreeOfLight"
    if (-not (Test-Path $seedsDir)) {
        throw "Tree model seed directory not found: $seedsDir"
    }
    $files = @(Get-ChildItem -Path $seedsDir -Filter *.json | Sort-Object Name)
    if ($files.Count -eq 0) {
        throw "No tree model seed files found under $seedsDir"
    }
    $seeds = @()
    foreach ($file in $files) {
        $data = Get-Content $file.FullName -Raw | ConvertFrom-Json -Depth 6
        if ($null -eq $data) {
            throw "Failed to deserialize tree model seed data from $($file.Name)."
        }
        $seeds += [pscustomobject]@{
            FileName = $file.Name
            Configuration = $data.configuration
            Regions = $data.regions
            StoryNodes = $data.storyNodes
            UnlockRules = $data.unlockRules
        }
    }
    return $seeds
}

function Build-TreeSql {
    param($Seeds)
    $lines = New-Object System.Collections.Generic.List[string]
    $runStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"
    $lines.Add("-- Auto-generated from Data/SeedData/TreeOfLight/*.json")
    $lines.Add("-- Run date: $runStamp")
    $lines.Add("")
    $lines.Add('DO $$')
    $lines.Add("BEGIN")
    $lines.Add("    IF NOT EXISTS (")
    $lines.Add("        SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp'")
    $lines.Add("    ) THEN")
    $lines.Add("        CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";")
    $lines.Add("    END IF;")
    $lines.Add('END $$;')
    $lines.Add("")
    $lines.Add("BEGIN;")
    $lines.Add("")

    foreach ($seed in $Seeds) {
        $configId = (Get-PropValue $seed.Configuration 'id')
        if ([string]::IsNullOrWhiteSpace($configId)) { $configId = (Get-PropValue $seed.Configuration 'Id') }
        if ([string]::IsNullOrWhiteSpace($configId)) { $configId = $seed.FileName.Replace(".json","") }
        if ([string]::IsNullOrWhiteSpace($configId)) {
            throw "Tree configuration missing id (file: $($seed.FileName))."
        }
        $configName = (Get-PropValue $seed.Configuration 'name')
        if ([string]::IsNullOrWhiteSpace($configName)) { $configName = "Tree Model" }
        $isDefault = [bool]((Get-PropValue $seed.Configuration 'isDefault') ?? $false)
        $lines.Add("-- Tree configuration: $configId ($($seed.FileName))")
        $configSql = @"
INSERT INTO alchimalia_schema."TreeConfigurations"
    ("Id", "Name", "IsDefault")
VALUES
    ('$configId', '$(($configName).Replace("'", "''"))', $(Format-Bool $isDefault))
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "IsDefault" = EXCLUDED."IsDefault";
"@
        $lines.Add($configSql.Trim())

        foreach ($region in @($seed.Regions)) {
            if ($null -eq $region) { continue }
            $regionId = (Get-PropValue $region 'id')
            if ([string]::IsNullOrWhiteSpace($regionId)) { continue }
            $label = (Get-PropValue $region 'label')
            if ([string]::IsNullOrWhiteSpace($label)) { $label = $regionId }
            $imageUrl = Normalize-ImagePath (Get-PropValue $region 'imageUrl')
            $pufMsg = Get-PropValue $region 'pufpufMessage'
            $sortOrder = [int]((Get-PropValue $region 'sortOrder') ?? 0)
            $isLocked = [bool]((Get-PropValue $region 'isLocked') ?? $false)
            $regionSql = @"
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('$regionId', $(Sql-Literal $label), $(Sql-Literal $imageUrl -AllowNull), $(Sql-Literal $pufMsg -AllowNull), $sortOrder, $(Format-Bool $isLocked), '$configId', '$timestamp', '$timestamp')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
            $lines.Add($regionSql.Trim())
        }

        foreach ($node in @($seed.StoryNodes)) {
            if ($null -eq $node) { continue }
            $storyId = Get-PropValue $node 'storyId'
            $regionId = Get-PropValue $node 'regionId'
            if ([string]::IsNullOrWhiteSpace($storyId) -or [string]::IsNullOrWhiteSpace($regionId)) {
                throw "Story node entry missing storyId or regionId in $($seed.FileName)."
            }
            $rewardImage = Normalize-ImagePath (Get-PropValue $node 'rewardImageUrl')
            $sortOrder = [int]((Get-PropValue $node 'sortOrder') ?? 0)
            $nodeKey = "treestory:${configId}:${regionId}:${storyId}"
            $nodeId = Get-DeterministicInt $nodeKey
            $nodeSql = @"
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ($nodeId, '$storyId', '$regionId', $(Sql-Literal $rewardImage -AllowNull), $sortOrder, '$configId', '$timestamp', '$timestamp')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
            $lines.Add($nodeSql.Trim())
        }

        foreach ($rule in @($seed.UnlockRules)) {
            if ($null -eq $rule) { continue }
            $ruleType = Get-PropValue $rule 'type'
            $fromId = Get-PropValue $rule 'fromId'
            $toRegionId = Get-PropValue $rule 'toRegionId'
            if ([string]::IsNullOrWhiteSpace($ruleType) -or [string]::IsNullOrWhiteSpace($fromId) -or [string]::IsNullOrWhiteSpace($toRegionId)) {
                throw "Unlock rule missing mandatory fields in $($seed.FileName)."
            }
            $requiredCsv = Get-PropValue $rule 'requiredStoriesCsv'
            $minCount = Get-PropValue $rule 'minCount'
            $storyId = Get-PropValue $rule 'storyId'
            $sortOrder = [int]((Get-PropValue $rule 'sortOrder') ?? 0)
            $ruleKey = "treeunlock:${configId}:${ruleType}:${fromId}:${toRegionId}:" + ($storyId ?? $requiredCsv ?? "")
            $ruleId = Get-DeterministicInt $ruleKey
            $minCountLit = if ($null -ne $minCount) { $minCount } else { "NULL" }
            $ruleSql = @"
INSERT INTO alchimalia_schema."TreeUnlockRules"
    ("Id", "Type", "FromId", "ToRegionId", "RequiredStoriesCsv", "MinCount", "StoryId", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ($ruleId, '$ruleType', '$fromId', '$toRegionId', $(Sql-Literal $requiredCsv -AllowNull), $minCountLit, $(Sql-Literal $storyId -AllowNull), $sortOrder, '$configId', '$timestamp', '$timestamp')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "FromId" = EXCLUDED."FromId",
    "ToRegionId" = EXCLUDED."ToRegionId",
    "RequiredStoriesCsv" = EXCLUDED."RequiredStoriesCsv",
    "MinCount" = EXCLUDED."MinCount",
    "StoryId" = EXCLUDED."StoryId",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
            $lines.Add($ruleSql.Trim())
        }

        $lines.Add("")
    }

    $lines.Add("COMMIT;")
    return $lines
}

$seeds = Load-TreeSeeds
$sqlLines = Build-TreeSql -Seeds $seeds
$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
[System.IO.File]::WriteAllLines($outputFullPath, $sqlLines, [System.Text.Encoding]::UTF8)
Write-Host "Generated tree model SQL at $outputFullPath"

