# Script to check Hungarian translations for Book of Heroes
# Checks tree of heroes, story heroes, and discovery translations

param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $RelativePath))
}

Write-Host "Checking Hungarian translations for Book of Heroes..." -ForegroundColor Cyan
Write-Host ""

# 1. Check Tree of Heroes translations
Write-Host "=== Tree of Heroes ===" -ForegroundColor Yellow
$heroTreePath = Get-FullPath "XooCreator.BA/Data/SeedData/SharedConfigs/hero-tree.json"
$huHeroTreePath = Get-FullPath "XooCreator.BA/Data/SeedData/BookOfHeroes/i18n/hu-hu/hero-tree.json"

if (-not (Test-Path $heroTreePath)) {
    Write-Host "ERROR: hero-tree.json not found at $heroTreePath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $huHeroTreePath)) {
    Write-Host "ERROR: Hungarian hero-tree.json not found at $huHeroTreePath" -ForegroundColor Red
    exit 1
}

$heroTree = Get-Content $heroTreePath -Raw | ConvertFrom-Json
$huTranslations = Get-Content $huHeroTreePath -Raw | ConvertFrom-Json

$missingTree = @()
foreach ($node in $heroTree.nodes) {
    $nameKey = $node.nameKey
    $descKey = $node.descriptionKey
    $storyKey = $node.storyKey
    
    $huProps = $huTranslations.PSObject.Properties.Name
    
    if ($nameKey -notin $huProps) {
        $missingTree += "Missing name: $nameKey for hero $($node.id)"
    }
    if ($descKey -notin $huProps) {
        $missingTree += "Missing desc: $descKey for hero $($node.id)"
    }
    if ($storyKey -notin $huProps) {
        $missingTree += "Missing story: $storyKey for hero $($node.id)"
    }
}

if ($missingTree.Count -eq 0) {
    $heroCount = $heroTree.nodes.Count
    Write-Host "OK: All Tree of Heroes translations found ($heroCount heroes)" -ForegroundColor Green
} else {
    Write-Host "ERROR: Missing $($missingTree.Count) Tree of Heroes translations:" -ForegroundColor Red
    $missingTree | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}

Write-Host ""

# 2. Check Story Heroes translations
Write-Host "=== Story Heroes ===" -ForegroundColor Yellow
$storyHeroesPath = Get-FullPath "XooCreator.BA/Data/SeedData/SharedConfigs/story-heroes.json"
$huStoryHeroesPath = Get-FullPath "XooCreator.BA/Data/SeedData/Translations/hu-HU/story-heroes.json"

if (-not (Test-Path $storyHeroesPath)) {
    Write-Host "WARNING: story-heroes.json not found at $storyHeroesPath" -ForegroundColor Yellow
} else {
    if (-not (Test-Path $huStoryHeroesPath)) {
        Write-Host "ERROR: Hungarian story-heroes.json not found at $huStoryHeroesPath" -ForegroundColor Red
    } else {
        $storyHeroes = Get-Content $storyHeroesPath -Raw | ConvertFrom-Json
        $huStoryHeroes = Get-Content $huStoryHeroesPath -Raw | ConvertFrom-Json
        
        $missingStory = @()
        foreach ($hero in $storyHeroes.storyHeroes) {
            $heroId = $hero.heroId
            $nameKey = "story_hero_${heroId}_name"
            $descKey = "story_hero_${heroId}_description"
            $storyKey = "story_hero_${heroId}_story"
            
            $huProps = $huStoryHeroes.PSObject.Properties.Name
            
            if ($nameKey -notin $huProps) {
                $missingStory += "Missing name: $nameKey for hero $heroId"
            }
            if ($descKey -notin $huProps) {
                $missingStory += "Missing description: $descKey for hero $heroId"
            }
            if ($storyKey -notin $huProps) {
                $missingStory += "Missing story: $storyKey for hero $heroId"
            }
        }
        
        if ($missingStory.Count -eq 0) {
            $storyHeroCount = $storyHeroes.storyHeroes.Count
            Write-Host "OK: All Story Heroes translations found ($storyHeroCount heroes)" -ForegroundColor Green
        } else {
            Write-Host "ERROR: Missing $($missingStory.Count) Story Heroes translations:" -ForegroundColor Red
            $missingStory | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        }
    }
}

Write-Host ""

# 3. Check Discovery translations
Write-Host "=== Discovery ===" -ForegroundColor Yellow
$discoveryPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/en-us/discover-bestiary.json"
$huDiscoveryPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/hu-hu/discover-bestiary.json"

if (-not (Test-Path $discoveryPath)) {
    Write-Host "WARNING: English discover-bestiary.json not found at $discoveryPath" -ForegroundColor Yellow
} else {
    if (-not (Test-Path $huDiscoveryPath)) {
        Write-Host "ERROR: Hungarian discover-bestiary.json not found at $huDiscoveryPath" -ForegroundColor Red
    } else {
        $enDiscovery = Get-Content $discoveryPath -Raw | ConvertFrom-Json
        $huDiscovery = Get-Content $huDiscoveryPath -Raw | ConvertFrom-Json
        
        $missingDiscovery = @()
        foreach ($creature in $enDiscovery) {
            $combination = $creature.Combination
            $huCreature = $huDiscovery | Where-Object { $_.Combination -eq $combination }
            
            if ($null -eq $huCreature) {
                $missingDiscovery += "Missing creature: $combination"
            } else {
                if ([string]::IsNullOrWhiteSpace($huCreature.Name)) {
                    $missingDiscovery += "Missing name for: $combination"
                }
                if ([string]::IsNullOrWhiteSpace($huCreature.Story)) {
                    $missingDiscovery += "Missing story for: $combination"
                }
            }
        }
        
        if ($missingDiscovery.Count -eq 0) {
            $creatureCount = $enDiscovery.Count
            Write-Host "OK: All Discovery translations found ($creatureCount creatures)" -ForegroundColor Green
        } else {
            Write-Host "ERROR: Missing $($missingDiscovery.Count) Discovery translations:" -ForegroundColor Red
            $missingDiscovery | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        }
    }
}

Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
$totalMissing = $missingTree.Count + $missingStory.Count + $missingDiscovery.Count
if ($totalMissing -eq 0) {
    Write-Host "OK: All Hungarian translations are complete!" -ForegroundColor Green
} else {
    Write-Host "ERROR: Total missing translations: $totalMissing" -ForegroundColor Red
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Review the missing translations above" -ForegroundColor White
    Write-Host "2. Add missing translations to the JSON files" -ForegroundColor White
    Write-Host "3. Run this script again to verify" -ForegroundColor White
}

