# Check differences between Discovery translation files

param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path
)

function Get-FullPath {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $RelativePath))
}

$enPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/en-us/discover-bestiary.json"
$huPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/hu-hu/discover-bestiary.json"
$roPath = Get-FullPath "XooCreator.BA/Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json"

$en = Get-Content $enPath -Raw | ConvertFrom-Json
$hu = Get-Content $huPath -Raw | ConvertFrom-Json
$ro = Get-Content $roPath -Raw | ConvertFrom-Json

Write-Host "Discovery Translation File Comparison" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "English: $($en.Count) entries" -ForegroundColor White
Write-Host "Hungarian: $($hu.Count) entries" -ForegroundColor White
Write-Host "Romanian: $($ro.Count) entries" -ForegroundColor White
Write-Host ""

# Get combinations
$enCombos = $en | ForEach-Object { $_.Combination } | Sort-Object
$huCombos = $hu | ForEach-Object { $_.Combination } | Sort-Object
$roCombos = $ro | ForEach-Object { $_.Combination } | Sort-Object

# Find extra in Romanian
$extraInRo = @()
foreach ($combo in $roCombos) {
    if ($enCombos -notcontains $combo) {
        $extraInRo += $combo
    }
}
if ($extraInRo.Count -gt 0) {
    Write-Host "EXTRA combinations in Romanian (not in English): $($extraInRo.Count)" -ForegroundColor Yellow
    $extraInRo | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    Write-Host ""
}

# Find missing in Romanian
$missingInRo = $enCombos | Where-Object { $roCombos -notcontains $_ }
if ($missingInRo.Count -gt 0) {
    Write-Host "MISSING combinations in Romanian (in English but not in Romanian):" -ForegroundColor Red
    $missingInRo | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    Write-Host ""
}

# Find missing in Hungarian
$missingInHu = $enCombos | Where-Object { $huCombos -notcontains $_ }
if ($missingInHu.Count -gt 0) {
    Write-Host "MISSING combinations in Hungarian (in English but not in Hungarian):" -ForegroundColor Red
    $missingInHu | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    Write-Host ""
}

# Find extra in Hungarian
$extraInHu = $huCombos | Where-Object { $enCombos -notcontains $_ }
if ($extraInHu.Count -gt 0) {
    Write-Host "EXTRA combinations in Hungarian (not in English):" -ForegroundColor Yellow
    $extraInHu | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    Write-Host ""
}

Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  English should be the source of truth with $($en.Count) entries" -ForegroundColor White
if ($extraInRo.Count -gt 0) {
    Write-Host "  Romanian has $($extraInRo.Count) extra entries that should be removed" -ForegroundColor Yellow
}
if ($missingInRo.Count -gt 0) {
    Write-Host "  Romanian is missing $($missingInRo.Count) entries" -ForegroundColor Red
}

