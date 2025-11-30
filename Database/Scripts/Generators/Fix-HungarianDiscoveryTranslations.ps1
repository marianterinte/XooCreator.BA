# Script to fix Hungarian Discovery translations
# Removes Romanian text that was incorrectly added and replaces with placeholders

param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $RelativePath))
}

Write-Host "Fixing Hungarian Discovery translations..." -ForegroundColor Cyan
Write-Host ""

$enPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/en-us/discover-bestiary.json"
$huPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/hu-hu/discover-bestiary.json"
$roPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json"

if (-not (Test-Path $enPath)) {
    Write-Host "ERROR: English file not found at $enPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $huPath)) {
    Write-Host "ERROR: Hungarian file not found at $huPath" -ForegroundColor Red
    exit 1
}

# Load files
$enCreatures = Get-Content $enPath -Raw | ConvertFrom-Json
$huCreatures = Get-Content $huPath -Raw | ConvertFrom-Json

# Create lookup for English creatures (this is the source of truth)
$enLookup = @{}
foreach ($creature in $enCreatures) {
    $enLookup[$creature.Combination] = $creature
}

# Check which Hungarian entries have Romanian text
# Romanian text typically contains: "s-a născut", "a apărut", "Copacul Luminii", etc.
$romanianPatterns = @("s-a născut", "a apărut", "Copacul Luminii", "împreună", "spirit", "vegheat")
$fixedCount = 0
$needsTranslation = @()

foreach ($huCreature in $huCreatures) {
    $isRomanian = $false
    
    # Check if Name or Story contains Romanian patterns
    foreach ($pattern in $romanianPatterns) {
        if ($huCreature.Story -like "*$pattern*" -or $huCreature.Name -like "*$pattern*") {
            $isRomanian = $true
            break
        }
    }
    
    # Also check for Romanian diacritics in name (ă, â, î, ș, ț)
    if ($huCreature.Name -match "[ăâîșțĂÂÎȘȚ]") {
        $isRomanian = $true
    }
    
    if ($isRomanian) {
        # Find English equivalent
        $enCreature = $enLookup[$huCreature.Combination]
        if ($null -ne $enCreature) {
            # Replace with placeholder
            $huCreature.Name = "[TRANSLATE TO HU] $($enCreature.Name)"
            $huCreature.Story = "[TRANSLATE TO HU] $($enCreature.Story)"
            $fixedCount++
            $needsTranslation += $huCreature.Combination
            Write-Host "Fixed: $($huCreature.Combination) - replaced Romanian with placeholder" -ForegroundColor Yellow
        }
    }
}

# Keep only creatures that exist in English (remove extras)
$validCombinations = $enLookup.Keys
$filteredCreatures = $huCreatures | Where-Object { $validCombinations -contains $_.Combination }

# Add missing creatures with placeholders
$huLookup = @{}
foreach ($creature in $filteredCreatures) {
    $huLookup[$creature.Combination] = $creature
}

$missing = @()
foreach ($enCreature in $enCreatures) {
    if (-not $huLookup.ContainsKey($enCreature.Combination)) {
        $missing += $enCreature
    }
}

foreach ($enCreature in $missing) {
    $newCreature = [PSCustomObject]@{
        Combination = $enCreature.Combination
        Name = "[TRANSLATE TO HU] $($enCreature.Name)"
        ImagePrompt = $enCreature.ImagePrompt
        Story = "[TRANSLATE TO HU] $($enCreature.Story)"
        ImageFileName = $enCreature.ImageFileName
    }
    $filteredCreatures += $newCreature
    Write-Host "Added placeholder for: $($enCreature.Combination)" -ForegroundColor Cyan
}

# Sort by Combination
$filteredCreatures = $filteredCreatures | Sort-Object -Property Combination

# Convert to JSON
$json = $filteredCreatures | ConvertTo-Json -Depth 10 -Compress:$false

# Write to file
$json | Set-Content -Path $huPath -Encoding UTF8

Write-Host ""
Write-Host "SUCCESS: Fixed Hungarian file" -ForegroundColor Green
Write-Host "  - Fixed $fixedCount entries with Romanian text" -ForegroundColor Yellow
Write-Host "  - Added $($missing.Count) missing entries with placeholders" -ForegroundColor Cyan
Write-Host "  - Total entries: $($filteredCreatures.Count) (should match English: $($enCreatures.Count))" -ForegroundColor White
Write-Host ""
Write-Host "IMPORTANT: All entries marked with '[TRANSLATE TO HU]' need manual translation from English to Hungarian" -ForegroundColor Red
Write-Host "File: $huPath" -ForegroundColor Cyan

