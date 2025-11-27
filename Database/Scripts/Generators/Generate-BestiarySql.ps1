param(
    [string]$OutputPath = "../V0002__seed_bestiary_items.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptRoot ".." ".." "..")
$seedFile = Join-Path $repoRoot "XooCreator.BA" "Data" "SeedData" "Discovery" "i18n" "ro-ro" "discover-bestiary.json"

if (-not (Test-Path $seedFile)) {
    throw "Seed file not found: $seedFile"
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

$seedData = Get-Content $seedFile -Raw | ConvertFrom-Json
if (-not $seedData) {
    throw "Seed file $seedFile does not contain data."
}

$outputFullPath = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $OutputPath))
$runDate = Get-Date -Format "yyyy-MM-dd HH:mm:ssK"

$headerLines = @(
    "-- Auto-generated from Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json",
    "-- Run date: $runDate",
    "",
    'DO $$',
    'BEGIN',
    '    IF NOT EXISTS (',
    '        SELECT 1',
    '        FROM pg_extension',
    '        WHERE extname = ''uuid-ossp''',
    '    ) THEN',
    '        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";',
    '    END IF;',
    'END $$;',
    "",
    'BEGIN;',
    ""
)

[System.IO.File]::WriteAllText($outputFullPath, ($headerLines -join [Environment]::NewLine), [System.Text.Encoding]::UTF8)

foreach ($item in $seedData) {
    $combo = $item.Combination
    $parts = Split-Combination -Combination $combo
    $arms = $parts[0]
    $body = $parts[1]
    $head = $parts[2]

    $comboKey = "$arms|$body|$head"
    $idExpression = "uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:$comboKey')"
    $name = $item.Name.Replace("'", "''")
    $story = ($item.Story ?? "").Replace("'", "''")

    $sql = @"
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ($idExpression, '$arms', '$body', '$head', '$name', '$story', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

"@

    [System.IO.File]::AppendAllText($outputFullPath, $sql, [System.Text.Encoding]::UTF8)
}

[System.IO.File]::AppendAllText($outputFullPath, "COMMIT;" + [Environment]::NewLine, [System.Text.Encoding]::UTF8)

Write-Host "Generated bestiary SQL at $outputFullPath"

