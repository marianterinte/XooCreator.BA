param(
    [string]$OutputPath = "../V0003__seed_story_topics_age_groups_authors.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$timestamp = "2025-01-01T00:00:00Z"
$namespaceGuid = "00000000-0000-0000-0000-000000000000"
. (Join-Path $scriptRoot "GuidUtils.ps1")

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
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

function Get-AuthorId {
    param([string]$Name)
    $id = $Name.ToLowerInvariant()
    $replacements = @{
        "ă"="a"; "â"="a"; "î"="i"; "ș"="s"; "ş"="s"; "ț"="t"; "ţ"="t";
        "á"="a"; "é"="e"; "í"="i"; "ó"="o"; "ö"="o"; "ő"="o"; "ú"="u"; "ü"="u"; "ű"="u";
        "ã"="a"; "ê"="e"; "õ"="o"
    }
    foreach ($entry in $replacements.GetEnumerator()) {
        $id = $id.Replace($entry.Key, $entry.Value)
    }
    $id = ($id -replace "[\s]+", "-")
    $id = ($id -replace "[\.,'""\(\)]", "")
    while ($id.Contains("--")) { $id = $id.Replace("--", "-") }
    return $id.Trim("-")
}

function Load-Json {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        throw "Missing JSON seed file: $Path"
    }
    return Get-Content $Path -Raw | ConvertFrom-Json
}

$topicsBasePath = Get-FullPath "XooCreator.BA/Data/SeedData/Story-Editor/Topic/i18n"
$authorsPath = Get-FullPath "XooCreator.BA/Data/SeedData/Story-Editor/authors.json"
$locales = @("ro-ro", "en-us", "hu-hu")

# Load topics (structure anchored in ro-ro)
$primaryTopics = Load-Json (Join-Path $topicsBasePath "ro-ro/topics.json")
$topicMap = @{}
$topicOrder = 0
foreach ($dimension in $primaryTopics.story_dimensions) {
    foreach ($topic in ($dimension.allowed_values | Where-Object { $_ -ne $null })) {
        $topicMap[$topic.id] = [ordered]@{
            TopicId     = $topic.id
            DimensionId = $dimension.id
            SortOrder   = $topicOrder++
            Labels      = @{}
        }
        $topicMap[$topic.id].Labels["ro-ro"] = $topic.label ?? ""
    }
}

foreach ($locale in $locales) {
    $topicFile = Join-Path (Join-Path $topicsBasePath $locale) "topics.json"
    if (-not (Test-Path $topicFile)) { continue }
    $localeJson = Load-Json $topicFile
    foreach ($dimension in $localeJson.story_dimensions) {
        foreach ($topic in ($dimension.allowed_values | Where-Object { $_ -ne $null })) {
            if ($topicMap.ContainsKey($topic.id)) {
                $topicMap[$topic.id].Labels[$locale] = $topic.label ?? ""
            }
        }
    }
}

# Load age groups
$ageGroupMap = @{}
$agePrimary = Load-Json (Join-Path $topicsBasePath "ro-ro/age-groups.json")
$ageOrder = 0
foreach ($group in $agePrimary.age_groups) {
    $ageGroupMap[$group.id] = [ordered]@{
        AgeGroupId = $group.id
        MinAge     = [int]$group.min_age
        MaxAge     = [int]$group.max_age
        SortOrder  = $ageOrder++
        Labels     = @{}
        Descriptions = @{}
    }
    $ageGroupMap[$group.id].Labels["ro-ro"] = $group.label ?? ""
    $ageGroupMap[$group.id].Descriptions["ro-ro"] = $group.description ?? ""
}

foreach ($locale in $locales) {
    $ageFile = Join-Path (Join-Path $topicsBasePath $locale) "age-groups.json"
    if (-not (Test-Path $ageFile)) { continue }
    $ageJson = Load-Json $ageFile
    foreach ($group in $ageJson.age_groups) {
        if ($ageGroupMap.ContainsKey($group.id)) {
            $ageGroupMap[$group.id].Labels[$locale] = $group.label ?? ""
            $ageGroupMap[$group.id].Descriptions[$locale] = $group.description ?? ""
        }
    }
}

# Load authors
$authorsJson = Load-Json $authorsPath
$authors = New-Object System.Collections.Generic.List[object]
function Add-Authors {
    param([string[]]$Names, [string]$LanguageCode)
    $sort = 0
    foreach ($name in $Names) {
        if ([string]::IsNullOrWhiteSpace($name)) { continue }
        $authors.Add([pscustomobject]@{
            AuthorId = Get-AuthorId $name;
            Name = $name;
            LanguageCode = $LanguageCode;
            SortOrder = $sort++
        })
    }
}

Add-Authors -Names $authorsJson.romanian_authors -LanguageCode "ro-ro"
Add-Authors -Names $authorsJson.hungarian_authors -LanguageCode "hu-hu"
Add-Authors -Names $authorsJson.english_american_authors -LanguageCode "en-us"

# Build SQL
$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
$lines = New-Object System.Collections.Generic.List[string]
$runStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"
$lines.Add("-- Auto-generated from Data/SeedData/Story-Editor (topics, age groups, authors)")
$lines.Add("-- Run date: $runStamp")
$lines.Add("")
$lines.Add("BEGIN;")
$lines.Add("")

$lines.Add("-- Story Topics")
foreach ($topic in ($topicMap.Values | Sort-Object SortOrder)) {
    $topicIdExpr = New-GuidLiteral "story-topic:$($topic.TopicId)"
    $sql = @"
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ($topicIdExpr, '$($topic.TopicId)', '$($topic.DimensionId)', $($topic.SortOrder), TRUE, '$timestamp', '$timestamp')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
    $lines.Add($sql.Trim())

    foreach ($locale in ($topic.Labels.Keys | Sort-Object)) {
        $label = Escape-Sql $topic.Labels[$locale]
        $translationId = New-GuidLiteral "story-topic-tr:$($topic.TopicId)|$locale"
        $translationSql = @"
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ($translationId, $topicIdExpr, '$locale', '$label')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
"@
        $lines.Add($translationSql.Trim())
    }
}

$lines.Add("-- Story Age Groups")
foreach ($group in ($ageGroupMap.Values | Sort-Object SortOrder)) {
    $groupIdExpr = New-GuidLiteral "story-age-group:$($group.AgeGroupId)"
    $sql = @"
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ($groupIdExpr, '$($group.AgeGroupId)', $($group.MinAge), $($group.MaxAge), $($group.SortOrder), TRUE, '$timestamp', '$timestamp')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
    $lines.Add($sql.Trim())

    foreach ($locale in ($group.Labels.Keys | Sort-Object)) {
        $label = Escape-Sql $group.Labels[$locale]
        $description = Escape-Sql ($group.Descriptions[$locale])
        $translationId = New-GuidLiteral "story-age-group-tr:$($group.AgeGroupId)|$locale"
        $translationSql = @"
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ($translationId, $groupIdExpr, '$locale', '$label', '$description')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
"@
        $lines.Add($translationSql.Trim())
    }
}

$lines.Add("-- Classic Authors")
foreach ($author in $authors) {
    $authorIdExpr = New-GuidLiteral "classic-author:$($author.AuthorId)"
    $name = Escape-Sql $author.Name
    $sql = @"
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ($authorIdExpr, '$($author.AuthorId)', '$name', '$($author.LanguageCode)', $($author.SortOrder), TRUE, '$timestamp', '$timestamp')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
    $lines.Add($sql.Trim())
}

$lines.Add("COMMIT;")

[System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.Encoding]::UTF8)
Write-Host "Generated story topics/age groups/authors SQL at $outputFullPath"

