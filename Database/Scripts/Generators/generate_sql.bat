@echo off
setlocal enabledelayedexpansion

echo Generating BestiaryItemTranslations SQL...

set "scriptRoot=%~dp0"
set "repoRoot=%scriptRoot%..\..\.."
set "i18nRoot=%repoRoot%XooCreator.BA\XooCreator.BA\Data\SeedData\Discovery\i18n"
set "outputPath=%scriptRoot%..\V00120__seed_bestiary_item_translations.sql"

echo Script root: %scriptRoot%
echo Repo root: %repoRoot%
echo i18n root: %i18nRoot%
echo Output path: %outputPath%

REM Create header
echo -- Auto-generated from Data/SeedData/Discovery/i18n/*/discover-bestiary.json > "%outputPath%"
echo -- Locales: en-us, ro-ro, hu-hu >> "%outputPath%"
echo -- Run date: %date% %time% >> "%outputPath%"
echo -- This script seeds BestiaryItemTranslations for all discovery bestiary combinations. >> "%outputPath%"
echo -- It is idempotent: safe to run multiple times. >> "%outputPath%"
echo. >> "%outputPath%"
echo BEGIN; >> "%outputPath%"
echo. >> "%outputPath%"

REM Process English combinations first
set "enFile=%i18nRoot%\en-us\discover-bestiary.json"

echo Processing English combinations from: %enFile%

REM Use PowerShell to parse JSON and generate SQL
powershell -Command ^
"$json = Get-Content '%enFile%' -Raw | ConvertFrom-Json; ^
$combinations = $json | Where-Object { $_.Combination } | Sort-Object Combination; ^
foreach ($item in $combinations) { ^
  $comb = $item.Combination; ^
  Write-Host \"Processing combination: $comb\"; ^
  $name = $item.Name -replace \"'\", \"''\"; ^
  $story = $item.Story -replace \"'\", \"''\"; ^
  $guid = [System.Guid]::NewGuid().ToString(); ^
  echo \"INSERT INTO alchimalia_schema.\"\"BestiaryItemTranslations\"\"\" >> \"%outputPath%\"; ^
  echo \"    (\"\"Id\"\", \"\"BestiaryItemId\"\", \"\"LanguageCode\"\", \"\"Name\"\", \"\"Story\"\")\" >> \"%outputPath%\"; ^
  echo \"VALUES\" >> \"%outputPath%\"; ^
  echo \"    ('$guid', 'PLACEHOLDER_BESTIARY_ID', 'en-us', '$name', '$story')\" >> \"%outputPath%\"; ^
  echo \"ON CONFLICT (\"\"BestiaryItemId\"\", \"\"LanguageCode\"\") DO UPDATE\" >> \"%outputPath%\"; ^
  echo \"SET \"\"Name\"\" = EXCLUDED.\"\"Name\"\",\" >> \"%outputPath%\"; ^
  echo \"    \"\"Story\"\" = EXCLUDED.\"\"Story\"\";\" >> \"%outputPath%\"; ^
  echo \"\" >> \"%outputPath%\"; ^
}"

echo COMMIT; >> "%outputPath%"
echo. >> "%outputPath%"

echo SQL generation completed!
echo Output file: %outputPath%

pause
