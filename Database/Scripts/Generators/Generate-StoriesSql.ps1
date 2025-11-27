param(
    [ValidateSet("main", "indie")]
    [string]$Mode = "main",
    [string]$OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$namespaceGuid = "00000000-0000-0000-0000-000000000000"
$timestamp = "2025-01-01T00:00:00Z"
$locales = @("ro-ro", "en-us", "hu-hu")
$seedOwnerEmail = "seed@alchimalia.com"
$indieOwnerId = "33333333-3333-3333-3333-333333333333"
. (Join-Path $scriptRoot "GuidUtils.ps1")

if (-not $OutputPath) {
    $OutputPath = if ($Mode -eq "main") { "../V0005__seed_main_stories.sql" } else { "../V0006__seed_indie_stories.sql" }
}

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RelativePath))
}

function Escape-Sql {
    param([string]$Value)
    if ($null -eq $Value) { return "" }
    return $Value.Replace("'", "''")
}

function Sql-Literal {
    param([string]$Value, [switch]$AllowNull)
    if ([string]::IsNullOrWhiteSpace($Value)) {
        if ($AllowNull) { return "NULL" }
        return "''"
    }
    return "'" + (Escape-Sql $Value) + "'"
}

function Format-Double {
    param([double]$Value)
    return $Value.ToString("0.###############", [System.Globalization.CultureInfo]::InvariantCulture)
}

function New-GuidLiteral {
    param([string]$Key)
    return Get-GuidLiteral -NamespaceGuid $namespaceGuid -Name $Key
}

function Map-TokenFamily {
    param([string]$Type)
    switch (($Type ?? "Personality").Trim()) {
        "TreeOfHeroes" { return "Personality" }
        "Personality" { return "Personality" }
        "Alchemy" { return "Alchemy" }
        "Discovery" { return "Discovery" }
        "Generative" { return "Generative" }
        "Learning" { return "Discovery" }
        default { return "Personality" }
    }
}

function Sanitize-EmailFolder {
    param([string]$Email)
    if ([string]::IsNullOrWhiteSpace($Email)) { return "unknown" }
    $trimmed = $Email.Trim().ToLowerInvariant()
    $chars = $trimmed.ToCharArray() | ForEach-Object {
        if (($_ -ge 'a' -and $_ -le 'z') -or ($_ -ge '0' -and $_ -le '9') -or $_ -in @('.', '_', '-', '@')) { $_ } else { '-' }
    }
    $cleaned = -join $chars
    while ($cleaned.Contains("--")) { $cleaned = $cleaned.Replace("--", "-") }
    return $cleaned.Trim('-')
}

function Normalize-CoverPath {
    param([string]$StoryId, [string]$CoverPath)
    if ([string]::IsNullOrWhiteSpace($CoverPath)) { return "" }
    $normalized = $CoverPath.TrimStart('/')
    if ($normalized.StartsWith("images/tales-of-alchimalia/stories/", [StringComparison]::OrdinalIgnoreCase) -or
        $normalized.StartsWith("images/tol/stories/", [StringComparison]::OrdinalIgnoreCase)) {
        return $normalized
    }
    $fileName = [System.IO.Path]::GetFileName($normalized)
    if ([string]::IsNullOrWhiteSpace($fileName)) { return "" }
    if ($fileName -notmatch "\.") {
        $ext = [System.IO.Path]::GetExtension($normalized)
        if ([string]::IsNullOrWhiteSpace($ext)) { $ext = ".png" }
        $fileName = "$fileName$ext"
    }
    $owner = Sanitize-EmailFolder $seedOwnerEmail
    return "images/tales-of-alchimalia/stories/$owner/$StoryId/$fileName"
}

function Normalize-TileImagePath {
    param([string]$StoryId, [string]$TileId, [string]$ImagePath)
    if ([string]::IsNullOrWhiteSpace($ImagePath)) { return $ImagePath }
    $normalized = $ImagePath.TrimStart('/')
    if ($normalized.StartsWith("images/tales-of-alchimalia/stories/", [StringComparison]::OrdinalIgnoreCase) -or
        $normalized.StartsWith("images/tol/stories/", [StringComparison]::OrdinalIgnoreCase)) {
        return $normalized
    }
    $fileName = [System.IO.Path]::GetFileName($normalized)
    if ([string]::IsNullOrWhiteSpace($fileName)) {
        $fileName = "$TileId.png"
    }
    elseif ($fileName -notmatch "\.") {
        $ext = [System.IO.Path]::GetExtension($normalized)
        if ([string]::IsNullOrWhiteSpace($ext)) { $ext = ".png" }
        $fileName = "$fileName$ext"
    }
    $owner = Sanitize-EmailFolder $seedOwnerEmail
    return "images/tales-of-alchimalia/stories/$owner/$StoryId/$fileName"
}

function Normalize-MediaPath {
    param([string]$Value, [string]$Locale, [ValidateSet("audio","video")][string]$Type)
    if ([string]::IsNullOrWhiteSpace($Value)) { return $null }
    $prefix = "$Type/tol/"
    if ($Value.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) {
        $suffix = $Value.Substring($prefix.Length)
        return "$Type/$Locale/tol/$suffix"
    }
    return $Value
}

function Get-JsonValue {
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

function Load-Stories {
    param([string]$BaseDir, [int]$StoryType, [bool]$NormalizeImages)
    $stories = @{}
    if (-not (Test-Path $BaseDir)) { return $stories }
    $files = Get-ChildItem -Path $BaseDir -Filter *.json | Sort-Object Name
        foreach ($file in $files) {
        $data = Get-Content $file.FullName -Raw | ConvertFrom-Json -Depth 200
        if (-not $data) { continue }
        $storyId = (Get-JsonValue $data 'storyId')
        if (-not $storyId) { continue }
        $storyId = $storyId.Trim()
        if (-not $storyId) { continue }
        $cover = Get-JsonValue $data 'coverImageUrl'
        if ($NormalizeImages) {
            $cover = Normalize-CoverPath $storyId $cover
        }
        $priceVal = Get-JsonValue $data 'price'
        if ($null -eq $priceVal) { $priceVal = Get-JsonValue $data 'priceInCredits' }
        $story = [ordered]@{
            StoryId = $storyId
            Title = (Get-JsonValue $data 'title') ?? ""
            CoverImageUrl = $cover ?? ""
            Summary = (Get-JsonValue $data 'summary') ?? ""
            StoryTopic = (Get-JsonValue $data 'storyTopic') ?? ""
            SortOrder = [int]((Get-JsonValue $data 'sortOrder') ?? 0)
            Price = if ($null -ne $priceVal) { [double]$priceVal } else { 0 }
            StoryType = $StoryType
            Tiles = @()
        }
        $tileList = (Get-JsonValue $data 'tiles')
        if ($null -eq $tileList) { $tileList = @() }
            foreach ($tile in $tileList) {
            $tileId = (Get-JsonValue $tile 'tileId')
            if (-not $tileId) { continue }
            $tileId = $tileId.Trim()
            if (-not $tileId) { continue }
            $imageUrl = Get-JsonValue $tile 'imageUrl'
            if ($NormalizeImages) {
                $imageUrl = Normalize-TileImagePath $storyId $tileId $imageUrl
            }
            $tileObj = [ordered]@{
                TileId = $tileId
                Type = (Get-JsonValue $tile 'type') ?? "page"
                SortOrder = [int]((Get-JsonValue $tile 'sortOrder') ?? 0)
                Caption = Get-JsonValue $tile 'caption'
                Text = Get-JsonValue $tile 'text'
                ImageUrl = $imageUrl
                Question = Get-JsonValue $tile 'question'
                VideoUrl = Get-JsonValue $tile 'videoUrl'
                AudioUrl = Get-JsonValue $tile 'audioUrl'
                Answers = @()
            }
            if ($tileObj.Type -eq "video" -and -not [string]::IsNullOrWhiteSpace($tileObj.VideoUrl)) {
                if ([string]::IsNullOrWhiteSpace($tileObj.ImageUrl) -or $tileObj.ImageUrl.StartsWith("video/", [StringComparison]::OrdinalIgnoreCase)) {
                    $tileObj.ImageUrl = $null
                }
            }
            $answerList = (Get-JsonValue $tile 'answers')
            if ($null -eq $answerList) { $answerList = @() }
            foreach ($answer in $answerList) {
                $answerId = (Get-JsonValue $answer 'answerId')
                if (-not $answerId) { continue }
                $answerId = $answerId.Trim()
                if (-not $answerId) { continue }
                $answerObj = [ordered]@{
                    AnswerId = $answerId
                    Text = (Get-JsonValue $answer 'text') ?? ""
                    SortOrder = [int]((Get-JsonValue $answer 'sortOrder') ?? 0)
                    Tokens = @()
                }
                $tokenList = (Get-JsonValue $answer 'tokens')
                if ($null -eq $tokenList) { $tokenList = @() }
                foreach ($token in $tokenList) {
                    $family = Map-TokenFamily (Get-JsonValue $token 'type')
                    $answerObj.Tokens += [ordered]@{
                        Type = $family
                        Value = (Get-JsonValue $token 'value') ?? ""
                        Quantity = [int]((Get-JsonValue $token 'quantity') ?? 0)
                    }
                }
                $tileObj.Answers += $answerObj
            }
            $story.Tiles += $tileObj
        }
        $stories[$storyId] = $story
    }
    return $stories
}

function Load-Translations {
    param([string]$BaseDir)
    $result = @{}
    if (-not (Test-Path $BaseDir)) { return $result }
    $files = Get-ChildItem -Path $BaseDir -Filter *.json | Sort-Object Name
    foreach ($file in $files) {
        $data = Get-Content $file.FullName -Raw | ConvertFrom-Json -Depth 200
        if (-not $data) { continue }
        $storyId = (Get-JsonValue $data 'storyId')
        if (-not $storyId) { continue }
        $storyId = $storyId.Trim()
        if (-not $storyId) { continue }
        $translation = [ordered]@{
            Title = Get-JsonValue $data 'title'
            Tiles = @{}
        }
        $tileList = (Get-JsonValue $data 'tiles')
        if ($null -eq $tileList) { $tileList = @() }
        foreach ($tile in $tileList) {
            $tileId = (Get-JsonValue $tile 'tileId')
            if (-not $tileId) { continue }
            $tileId = $tileId.Trim()
            if (-not $tileId) { continue }
            $tileTrans = [ordered]@{
                Caption = Get-JsonValue $tile 'caption'
                Text = Get-JsonValue $tile 'text'
                Question = Get-JsonValue $tile 'question'
                AudioUrl = Get-JsonValue $tile 'audioUrl'
                VideoUrl = Get-JsonValue $tile 'videoUrl'
                Answers = @{}
            }
            $answerList = (Get-JsonValue $tile 'answers')
            if ($null -eq $answerList) { $answerList = @() }
            foreach ($answer in $answerList) {
                $ansId = (Get-JsonValue $answer 'answerId')
                if (-not $ansId) { continue }
                $ansId = $ansId.Trim()
                if (-not $ansId) { continue }
                $tileTrans.Answers[$ansId] = (Get-JsonValue $answer 'text')
            }
            $translation.Tiles[$tileId] = $tileTrans
        }
        $result[$storyId] = $translation
    }
    return $result
}

function Get-TranslationForStory {
    param($Translations, [string]$StoryId, [string]$Locale)
    if (-not $Translations.ContainsKey($Locale)) { return $null }
    $stories = $Translations[$Locale]
    if ($stories.ContainsKey($StoryId)) { return $stories[$StoryId] }
    return $null
}

function Get-TileTranslation {
    param($StoryTranslation, [string]$TileId)
    if ($null -eq $StoryTranslation) { return $null }
    if ($StoryTranslation.Tiles.ContainsKey($TileId)) { return $StoryTranslation.Tiles[$TileId] }
    return $null
}

function Get-AnswerTranslation {
    param($TileTranslation, [string]$AnswerId)
    if ($null -eq $TileTranslation) { return $null }
    if ($TileTranslation.Answers.ContainsKey($AnswerId)) { return $TileTranslation.Answers[$AnswerId] }
    return $null
}

function Normalize-MediaValue {
    param($TileBase, $TileTranslation, [string]$Locale, [string]$Type)
    $value = $null
    if ($null -ne $TileTranslation) {
        if ($Type -eq "audio") { $value = $TileTranslation.AudioUrl }
        else { $value = $TileTranslation.VideoUrl }
    }
    if ([string]::IsNullOrWhiteSpace($value) -and $null -ne $TileBase) {
        if ($Type -eq "audio") { $value = $TileBase.AudioUrl }
        else { $value = $TileBase.VideoUrl }
    }
    return Normalize-MediaPath $value $Locale $Type
}

function Build-StoriesSql {
    param($Stories, $TranslationsByLocale, [int]$StoryType, [bool]$IsIndie)
    $lines = New-Object System.Collections.Generic.List[string]
    $runStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"
    $lines.Add("-- Auto-generated from Data/SeedData/Stories/seed@alchimalia.com (mode: $Mode)")
    $lines.Add("-- Run date: $runStamp")
    $lines.Add("")
    $lines.Add("BEGIN;")
    $lines.Add("")
    $lines.Add("-- Story Definitions")

    $orderedStories = $Stories.Values | Sort-Object @{Expression = { $_.SortOrder }}, @{Expression = { $_.StoryId }}

    foreach ($story in $orderedStories) {
        $storyKey = "storydef:$($story.StoryId)"
        $storyIdExpr = New-GuidLiteral $storyKey
        $titleLit = Sql-Literal $story.Title
        $coverLit = Sql-Literal $story.CoverImageUrl -AllowNull
        $summaryLit = Sql-Literal $story.Summary -AllowNull
        $topicLit = Sql-Literal $story.StoryTopic -AllowNull
        $priceLit = Format-Double ($story.Price ?? 0)
        $createdByLit = if ($IsIndie) { "'$indieOwnerId'" } else { "NULL" }
        $storySql = @"
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    ($storyIdExpr, '$($story.StoryId)', $titleLit, $coverLit, $summaryLit, $topicLit, NULL, NULL,
     $StoryType, 5, $($story.SortOrder), TRUE, 1, $priceLit,
     '$timestamp', '$timestamp', $createdByLit, $createdByLit)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
"@
        $lines.Add($storySql.Trim())

        foreach ($locale in $locales) {
            $storyTranslation = Get-TranslationForStory $TranslationsByLocale $story.StoryId $locale
            if ($null -eq $storyTranslation -and $locale -ne "ro-ro") { continue }
            $title = if ($storyTranslation) { $storyTranslation.Title } else { $story.Title }
            $titleLiteral = Sql-Literal ($title ?? $story.Title)
            $translationIdExpr = New-GuidLiteral "storydeftr:$($story.StoryId)|$locale"
            $defTrSql = @"
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ($translationIdExpr, $storyIdExpr, '$locale', $titleLiteral)
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
"@
            $lines.Add($defTrSql.Trim())
        }

        $lines.Add("")
        $lines.Add("-- Story Tiles for $($story.StoryId)")

        foreach ($tile in ($story.Tiles | Sort-Object @{Expression = { $_.SortOrder }}, @{Expression = { $_.TileId }})) {
            $tileKey = "storytile:$($story.StoryId):$($tile.TileId)"
            $tileIdExpr = New-GuidLiteral $tileKey
            $captionLit = Sql-Literal $tile.Caption -AllowNull
            $textLit = Sql-Literal $tile.Text -AllowNull
            $questionLit = Sql-Literal $tile.Question -AllowNull
            $imageLit = Sql-Literal $tile.ImageUrl -AllowNull
            $tileSql = @"
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ($tileIdExpr, $storyIdExpr, '$($tile.TileId)', '$($tile.Type)', $($tile.SortOrder), $captionLit, $textLit, $imageLit, $questionLit, '$timestamp', '$timestamp')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
"@
            $lines.Add($tileSql.Trim())

            foreach ($locale in $locales) {
                $storyTranslation = Get-TranslationForStory $TranslationsByLocale $story.StoryId $locale
                if ($null -eq $storyTranslation -and $locale -ne "ro-ro") { continue }
                $tileTranslation = Get-TileTranslation $storyTranslation $tile.TileId
                if ($null -eq $tileTranslation -and $locale -ne "ro-ro") { continue }
                $caption = if ($tileTranslation) { $tileTranslation.Caption } else { $tile.Caption }
                $textValue = if ($tileTranslation) { $tileTranslation.Text } else { $tile.Text }
                $question = if ($tileTranslation) { $tileTranslation.Question } else { $tile.Question }
                $audio = Normalize-MediaValue $tile $tileTranslation $locale "audio"
                $video = Normalize-MediaValue $tile $tileTranslation $locale "video"
                $captionLitLocale = Sql-Literal $caption -AllowNull
                $textLitLocale = Sql-Literal $textValue -AllowNull
                $questionLitLocale = Sql-Literal $question -AllowNull
                $audioLit = Sql-Literal $audio -AllowNull
                $videoLit = Sql-Literal $video -AllowNull
                $tileTrId = New-GuidLiteral "storytiletr:$($story.StoryId):$($tile.TileId)|$locale"
                $tileTrSql = @"
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ($tileTrId, $tileIdExpr, '$locale', $captionLitLocale, $textLitLocale, $questionLitLocale, $audioLit, $videoLit)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
"@
                $lines.Add($tileTrSql.Trim())
            }

            foreach ($answer in ($tile.Answers | Sort-Object @{Expression = { $_.SortOrder }}, @{Expression = { $_.AnswerId }})) {
                $answerKey = "storyanswer:$($story.StoryId):$($tile.TileId):$($answer.AnswerId)"
                $answerIdExpr = New-GuidLiteral $answerKey
                $answerTextLit = Sql-Literal $answer.Text
                $answerSql = @"
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ($answerIdExpr, $tileIdExpr, '$($answer.AnswerId)', $answerTextLit, NULL, $($answer.SortOrder), '$timestamp')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
"@
                $lines.Add($answerSql.Trim())

                $tokenIndex = 0
                foreach ($token in $answer.Tokens) {
                    $tokenKey = "storytoken:$($story.StoryId):$($tile.TileId):$($answer.AnswerId):$($token.Type):$($token.Value):$tokenIndex"
                    $tokenIdExpr = New-GuidLiteral $tokenKey
                    $tokenSql = @"
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ($tokenIdExpr, $answerIdExpr, '$($token.Type)', '$($token.Value)', $($token.Quantity))
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
"@
                    $lines.Add($tokenSql.Trim())
                    $tokenIndex++
                }

                foreach ($locale in $locales) {
                    $storyTranslation = Get-TranslationForStory $TranslationsByLocale $story.StoryId $locale
                    if ($null -eq $storyTranslation -and $locale -ne "ro-ro") { continue }
                    $tileTranslation = Get-TileTranslation $storyTranslation $tile.TileId
                    if ($null -eq $tileTranslation -and $locale -ne "ro-ro") { continue }
                    $answerTextLocale = Get-AnswerTranslation $tileTranslation $answer.AnswerId
                    if ([string]::IsNullOrWhiteSpace($answerTextLocale) -and $locale -ne "ro-ro") { continue }
                    if ([string]::IsNullOrWhiteSpace($answerTextLocale)) {
                        $answerTextLocale = $answer.Text
                    }
                    $answerTrId = New-GuidLiteral "storyanswertr:$($story.StoryId):$($tile.TileId):$($answer.AnswerId)|$locale"
                    $answerTextLiteral = Sql-Literal $answerTextLocale
                    $answerTrSql = @"
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ($answerTrId, $answerIdExpr, '$locale', $answerTextLiteral)
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
"@
                    $lines.Add($answerTrSql.Trim())
                }
            }
        }
    }

    $lines.Add("COMMIT;")
    return $lines
}

# Load data
switch ($Mode) {
    "main" {
        $baseDir = Get-FullPath "XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/i18n/ro-ro"
        $stories = Load-Stories -BaseDir $baseDir -StoryType 0 -NormalizeImages:$true
        $translations = @{}
        foreach ($locale in $locales) {
            $path = Get-FullPath "XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/i18n/$locale"
            $translations[$locale] = Load-Translations $path
        }
        $sqlLines = Build-StoriesSql -Stories $stories -TranslationsByLocale $translations -StoryType 0 -IsIndie:$false
    }
    "indie" {
        $baseDir = Get-FullPath "XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/independent/i18n/ro-ro"
        $stories = Load-Stories -BaseDir $baseDir -StoryType 1 -NormalizeImages:$true
        $translations = @{}
        foreach ($locale in $locales) {
            $path = Get-FullPath "XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/independent/i18n/$locale"
            $translations[$locale] = Load-Translations $path
        }
        $sqlLines = Build-StoriesSql -Stories $stories -TranslationsByLocale $translations -StoryType 1 -IsIndie:$true
    }
}

$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
[System.IO.File]::WriteAllLines($outputFullPath, $sqlLines, [System.Text.Encoding]::UTF8)
Write-Host "Generated stories SQL ($Mode) at $outputFullPath"

