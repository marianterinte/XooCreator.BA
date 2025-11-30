# Script to add missing Hungarian translations for Discovery creatures
# Reads English file, checks what's missing in Hungarian, and adds placeholders

param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path,
    [switch]$DryRun = $false
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $RelativePath))
}

Write-Host "Adding missing Hungarian translations for Discovery creatures..." -ForegroundColor Cyan
Write-Host ""

$enPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/en-us/discover-bestiary.json"
$huPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/hu-hu/discover-bestiary.json"
$roPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json"

if (-not (Test-Path $enPath)) {
    Write-Host "ERROR: English file not found at $enPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $roPath)) {
    Write-Host "WARNING: Romanian file not found at $roPath" -ForegroundColor Yellow
    Write-Host "Will use English as fallback for translations" -ForegroundColor Yellow
}

# Load files
$enCreatures = Get-Content $enPath -Raw | ConvertFrom-Json
$huCreatures = @()
if (Test-Path $huPath) {
    $huCreatures = Get-Content $huPath -Raw | ConvertFrom-Json
}
$roCreatures = @()
if (Test-Path $roPath) {
    $roCreatures = Get-Content $roPath -Raw | ConvertFrom-Json
}

# Create lookup dictionaries
$huLookup = @{}
foreach ($creature in $huCreatures) {
    $huLookup[$creature.Combination] = $creature
}

$roLookup = @{}
foreach ($creature in $roCreatures) {
    $roLookup[$creature.Combination] = $creature
}

# Find missing creatures
$missing = @()
foreach ($enCreature in $enCreatures) {
    $combination = $enCreature.Combination
    if (-not $huLookup.ContainsKey($combination)) {
        $missing += @{
            Combination = $combination
            English = $enCreature
            Romanian = if ($roLookup.ContainsKey($combination)) { $roLookup[$combination] } else { $null }
        }
    }
}

if ($missing.Count -eq 0) {
    Write-Host "OK: All creatures already have Hungarian translations!" -ForegroundColor Green
    exit 0
}

Write-Host "Found $($missing.Count) missing translations:" -ForegroundColor Yellow
$missing | ForEach-Object { Write-Host "  - $($_.Combination)" -ForegroundColor White }
Write-Host ""

if ($DryRun) {
    Write-Host "DRY RUN: Would add $($missing.Count) translations" -ForegroundColor Cyan
    Write-Host "Run without -DryRun to actually add them" -ForegroundColor Cyan
    exit 0
}

# Add missing creatures to Hungarian file
$newCreatures = @()
foreach ($item in $missing) {
    $newCreature = [PSCustomObject]@{
        Combination = $item.Combination
        Name = ""
        ImagePrompt = $item.English.ImagePrompt
        Story = ""
        ImageFileName = $item.English.ImageFileName
    }
    
    # Try to get translation from Romanian, otherwise use English with note
    if ($null -ne $item.Romanian) {
        $newCreature.Name = $item.Romanian.Name
        $newCreature.Story = $item.Romanian.Story
        Write-Host "Using Romanian translation for $($item.Combination)" -ForegroundColor Green
    } else {
        # Use English as placeholder - needs manual translation
        $newCreature.Name = "[TRANSLATE] $($item.English.Name)"
        $newCreature.Story = "[TRANSLATE] $($item.English.Story)"
        Write-Host "Using English placeholder for $($item.Combination) - NEEDS MANUAL TRANSLATION" -ForegroundColor Yellow
    }
    
    $newCreatures += $newCreature
}

# Add to existing Hungarian creatures
$allHuCreatures = $huCreatures + $newCreatures

# Sort by Combination for consistency
$allHuCreatures = $allHuCreatures | Sort-Object -Property Combination

# Convert to JSON with proper formatting
$jsonOptions = @{
    Depth = 10
    Compress = $false
}
$json = $allHuCreatures | ConvertTo-Json @jsonOptions

# Write to file
$json | Set-Content -Path $huPath -Encoding UTF8

Write-Host ""
Write-Host "SUCCESS: Added $($newCreatures.Count) translations to Hungarian file" -ForegroundColor Green
Write-Host "File updated: $huPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "NOTE: If any translations show '[TRANSLATE]', they need manual translation from English" -ForegroundColor Yellow
Write-Host "Review the file and translate those entries to Hungarian" -ForegroundColor Yellow

