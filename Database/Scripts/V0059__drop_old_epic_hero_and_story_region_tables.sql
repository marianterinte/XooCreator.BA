-- Drop old EpicHero and StoryRegion tables with CASCADE
-- Run date: 2025-12-17
-- Description: Eliminates old EpicHero and StoryRegion tables (not in production yet).
-- CASCADE will automatically remove all foreign key constraints and dependencies.

BEGIN;

-- Drop old tables cu CASCADE pentru a elimina automat toate constraint-urile și referințele
-- CASCADE va elimina automat:
-- - Foreign key constraints care referă aceste tabele
-- - Indexuri asociate
-- - Orice alte dependențe

-- Drop EpicHero tables (CASCADE va elimina automat foreign keys din StoryEpicHeroReferences și StoryEpicCraftHeroReferences)
DROP TABLE IF EXISTS alchimalia_schema."EpicHeroTranslations" CASCADE;
DROP TABLE IF EXISTS alchimalia_schema."EpicHeroes" CASCADE;

-- Drop StoryRegion tables (CASCADE va elimina automat foreign keys din StoryEpicRegionReferences și StoryEpicCraftRegions)
DROP TABLE IF EXISTS alchimalia_schema."StoryRegionTranslations" CASCADE;
DROP TABLE IF EXISTS alchimalia_schema."StoryRegions" CASCADE;

COMMIT;

