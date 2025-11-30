# Script to translate Discovery creatures from English to Hungarian
# This script reads the English file and translates all entries to Hungarian

param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $RelativePath))
}

Write-Host "Translating Discovery creatures to Hungarian..." -ForegroundColor Cyan
Write-Host ""

$enPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/en-us/discover-bestiary.json"
$huPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/hu-hu/discover-bestiary.json"

if (-not (Test-Path $enPath)) {
    Write-Host "ERROR: English file not found at $enPath" -ForegroundColor Red
    exit 1
}

# Load English file
$enCreatures = Get-Content $enPath -Raw | ConvertFrom-Json

# Translation dictionary based on Combination (not Name, since same name can appear for different combinations)
# Maps Combination -> Hungarian Name
$nameTranslations = @{
    "BunnyGiraffeNone" = "Nyuszi-zsiráf"
    "BunnyHippoNone" = "Nyuszi-vizi"
    "GiraffeHippoNone" = "Zsiráf-vizi"
    "BunnyGiraffeHippo" = "Nyuszi-zsiráf-vizi"
    "GiraffeBunnyNone" = "Zsiráf-nyuszi"
    "GiraffeHippoBunny" = "Zsiráf-vizi-nyuszi"
    "HippoBunnyNone" = "Vizi-nyuszi"
    "HippoGiraffeNone" = "Vizi-zsiráf"
    "HippoBunnyGiraffe" = "Vizi-nyuszi-zsiráf"
    "BunnyHippoGiraffe" = "Nyuszi-vizi-zsiráf"
    "GiraffeNoneHippo" = "Zsiráf-vizi"
    "BunnyNoneHippo" = "Nyuszi-vizi"
    "HippoNoneBunny" = "Vizi-nyuszi"
    "NoneBunnyHippo" = "Nyuszi-vizi"
    "NoneGiraffeHippo" = "Zsiráf-vizi"
    "NoneHippoGiraffe" = "Vizi-zsiráf"
    "BunnyNoneGiraffe" = "Nyuszi-zsiráf"
    "GiraffeNoneBunny" = "Zsiráf-nyuszi"
    "HippoNoneGiraffe" = "Vizi-zsiráf"
    "HippoGiraffeBunny" = "Vizi-zsiráf-nyuszi"
    "GiraffeBunnyHippo" = "Zsiráf-nyuszi-vizi"
    "NoneGiraffeBunny" = "Zsiráf-nyuszi"
    "NoneHippoBunny" = "Vizi-nyuszi"
    "NoneBunnyGiraffe" = "Nyuszi-zsiráf"
    "GiraffeGiraffeHippo" = "Kettős-zsiráf-vizi"
    "GiraffeGiraffeBunny" = "Zsiráf-zsiráf-nyuszi"
    "BunnyBunnyGiraffe" = "Nyuszi-nyuszi-zsiráf"
    "BunnyBunnyHippo" = "Nyuszi-nyuszi-vizi"
    "HippoHippoBunny" = "Vizi-vizi-nyuszi"
    "HippoHippoGiraffe" = "Vizi-vizi-zsiráf"
    "GiraffeHippoHippo" = "Zsiráf-vizi-vizi"
    "BunnyHippoHippo" = "Nyuszi-vizi-vizi"
    "BunnyGiraffeGiraffe" = "Nyuszi-zsiráf-zsiráf"
    "HippoGiraffeGiraffe" = "Vizi-zsiráf-zsiráf"
    "HippoBunnyBunny" = "Vizi-nyuszi-nyuszi"
    "GiraffeBunnyBunny" = "Zsiráf-nyuszi-nyuszi"
    "BunnyHippoBunny" = "Nyuszi-vizi-nyuszi"
    "BunnyGiraffeBunny" = "Nyuszi-zsiráf-nyuszi"
    "GiraffeHippoGiraffe" = "Zsiráf-vizi-zsiráf"
    "GiraffeBunnyGiraffe" = "Zsiráf-nyuszi-zsiráf"
    "HippoBunnyHippo" = "Vizi-nyuszi-vizi"
    "HippoGiraffeHippo" = "Vizi-zsiráf-vizi"
}

# Function to mark story for translation
# Note: Actual translation should be done by a professional translator or translation service
function Get-StoryPlaceholder {
    param([string]$englishText)
    
    # This is a placeholder - in a real scenario, you would use a translation API
    # The actual translation should be done by a professional translator or translation service
    return "[FORDÍTÁS SZÜKSÉGES] $englishText"
}

# Create Hungarian creatures array
$huCreatures = @()

foreach ($enCreature in $enCreatures) {
    $huCreature = [PSCustomObject]@{
        Combination = $enCreature.Combination
        Name = ""
        ImagePrompt = $enCreature.ImagePrompt
        Story = ""
        ImageFileName = $enCreature.ImageFileName
    }
    
    # Translate name based on Combination (not Name, since same name can appear for different combinations)
    if ($nameTranslations.ContainsKey($enCreature.Combination)) {
        $huCreature.Name = $nameTranslations[$enCreature.Combination]
    } else {
        # Keep original name if no translation found
        Write-Host "WARNING: No translation found for Combination: $($enCreature.Combination), using original name: $($enCreature.Name)" -ForegroundColor Yellow
        $huCreature.Name = $enCreature.Name
    }
    
    # Translate story - this needs proper translation
    # For now, we'll mark it for translation
    $huCreature.Story = Get-StoryPlaceholder $enCreature.Story
    
    $huCreatures += $huCreature
}

# Sort by Combination
$huCreatures = $huCreatures | Sort-Object -Property Combination

# Convert to JSON
$json = $huCreatures | ConvertTo-Json -Depth 10 -Compress:$false

# Write to file
$json | Set-Content -Path $huPath -Encoding UTF8

Write-Host "SUCCESS: Created Hungarian translation file" -ForegroundColor Green
Write-Host "NOTE: Stories need proper translation - currently marked with [FORDÍTÁS SZÜKSÉGES]" -ForegroundColor Yellow
Write-Host "File: $huPath" -ForegroundColor Cyan

