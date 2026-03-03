# Bestiary Translations — Seed Summary

## Ce trebuie făcut

**Seed la datele respective din JSON-uri** pentru BestiaryItemTranslations.

- **Sursă:** `Data/SeedData/Discovery/i18n/*/discover-bestiary.json` (en-us, ro-ro, hu-hu)
- **Destinație:** tabela `BestiaryItemTranslations` (63 combinații × 3 limbi = 189 rânduri)
- **Generator:** `Database/Scripts/Generators/Generate-BestiaryItemTranslationsSql.ps1` → scrie `V00120__seed_bestiary_item_translations.sql`

## Pași

1. Rulare generator: `powershell -ExecutionPolicy Bypass -File Generate-BestiaryItemTranslationsSql.ps1` din folderul `Database/Scripts/Generators`
2. Aplicare migrații SQL (V00119 CREATE TABLE + V00120 seed)
