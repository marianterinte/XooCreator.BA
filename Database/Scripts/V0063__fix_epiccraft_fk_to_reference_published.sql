-- Change FK constraints from StoryEpicCraft* to reference *Definitions (published)
-- instead of *Crafts (drafts).
-- Reason: Epic drafts should only reference PUBLISHED regions and heroes.
-- When a region/hero is published, the craft is deleted, so the FK constraint to crafts would fail.
-- Run date: 2025-12-18

BEGIN;

-- ===========================================
-- FIX REGIONS: StoryEpicCraftRegions
-- ===========================================

-- Drop the old FK constraint that references StoryRegionCrafts
ALTER TABLE alchimalia_schema."StoryEpicCraftRegions"
    DROP CONSTRAINT IF EXISTS "FK_StoryEpicCraftRegions_StoryRegionCrafts_RegionId";

-- Drop if new constraint already exists (idempotent)
ALTER TABLE alchimalia_schema."StoryEpicCraftRegions"
    DROP CONSTRAINT IF EXISTS "FK_StoryEpicCraftRegions_StoryRegionDefinitions_RegionId";

-- Add new FK constraint that references StoryRegionDefinitions (published regions)
ALTER TABLE alchimalia_schema."StoryEpicCraftRegions"
    ADD CONSTRAINT "FK_StoryEpicCraftRegions_StoryRegionDefinitions_RegionId"
    FOREIGN KEY ("RegionId") 
    REFERENCES alchimalia_schema."StoryRegionDefinitions" ("Id") 
    ON DELETE RESTRICT;

-- ===========================================
-- FIX HEROES: StoryEpicCraftHeroReferences
-- ===========================================

-- Drop the old FK constraint that references EpicHeroCrafts
ALTER TABLE alchimalia_schema."StoryEpicCraftHeroReferences"
    DROP CONSTRAINT IF EXISTS "FK_StoryEpicCraftHeroReferences_EpicHeroCrafts_HeroId";

-- Drop if new constraint already exists (idempotent)
ALTER TABLE alchimalia_schema."StoryEpicCraftHeroReferences"
    DROP CONSTRAINT IF EXISTS "FK_StoryEpicCraftHeroReferences_EpicHeroDefinitions_HeroId";

-- Add new FK constraint that references EpicHeroDefinitions (published heroes)
ALTER TABLE alchimalia_schema."StoryEpicCraftHeroReferences"
    ADD CONSTRAINT "FK_StoryEpicCraftHeroReferences_EpicHeroDefinitions_HeroId"
    FOREIGN KEY ("HeroId") 
    REFERENCES alchimalia_schema."EpicHeroDefinitions" ("Id") 
    ON DELETE RESTRICT;

COMMIT;

