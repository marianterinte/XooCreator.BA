-- Update Epic references to use Craft/Definition tables instead of old EpicHero and StoryRegion tables
-- Run date: 2025-12-17
-- Description: Recreates foreign key constraints to point to EpicHeroCraft/EpicHeroDefinition and StoryRegionCraft/StoryRegionDefinition
-- Note: Old foreign keys were automatically removed by DROP CASCADE in previous migration

BEGIN;

-- Remove any existing references that cannot be satisfied after dropping the old Hero/Region tables.
DELETE FROM alchimalia_schema."StoryEpicCraftHeroReferences"
WHERE "HeroId" NOT IN (SELECT "Id" FROM alchimalia_schema."EpicHeroCrafts");

DELETE FROM alchimalia_schema."StoryEpicHeroReferences"
WHERE "HeroId" NOT IN (SELECT "Id" FROM alchimalia_schema."EpicHeroDefinitions");

DELETE FROM alchimalia_schema."StoryEpicCraftRegions"
WHERE "RegionId" NOT IN (SELECT "Id" FROM alchimalia_schema."StoryRegionCrafts");

DELETE FROM alchimalia_schema."StoryEpicDefinitionRegions"
WHERE "RegionId" NOT IN (SELECT "Id" FROM alchimalia_schema."StoryRegionDefinitions");


-- Update StoryEpicCraftHeroReferences to reference EpicHeroCrafts (for draft epics)
-- Note: Foreign key uses composite key (Id, OwnerUserId) - need to handle this carefully
-- For now, we'll reference just Id since HeroId in reference table is just the Id
ALTER TABLE alchimalia_schema."StoryEpicCraftHeroReferences"
    ADD CONSTRAINT "FK_StoryEpicCraftHeroReferences_EpicHeroCrafts_HeroId"
    FOREIGN KEY ("HeroId") 
    REFERENCES alchimalia_schema."EpicHeroCrafts" ("Id") 
    ON DELETE RESTRICT;

-- Update StoryEpicHeroReferences to reference EpicHeroDefinitions (for published epics)
ALTER TABLE alchimalia_schema."StoryEpicHeroReferences"
    ADD CONSTRAINT "FK_StoryEpicHeroReferences_EpicHeroDefinitions_HeroId"
    FOREIGN KEY ("HeroId") 
    REFERENCES alchimalia_schema."EpicHeroDefinitions" ("Id") 
    ON DELETE RESTRICT;

-- Update StoryEpicCraftRegions to reference StoryRegionCrafts (for draft epics)
-- Note: StoryEpicCraftRegions uses RegionId as string, not composite key
ALTER TABLE alchimalia_schema."StoryEpicCraftRegions"
    ADD CONSTRAINT "FK_StoryEpicCraftRegions_StoryRegionCrafts_RegionId"
    FOREIGN KEY ("RegionId") 
    REFERENCES alchimalia_schema."StoryRegionCrafts" ("Id") 
    ON DELETE RESTRICT;

-- Update StoryEpicDefinitionRegions to reference StoryRegionDefinitions (for published epics)
-- Note: StoryEpicDefinitionRegions uses RegionId as string, not composite key
ALTER TABLE alchimalia_schema."StoryEpicDefinitionRegions"
    ADD CONSTRAINT "FK_StoryEpicDefinitionRegions_StoryRegionDefinitions_RegionId"
    FOREIGN KEY ("RegionId") 
    REFERENCES alchimalia_schema."StoryRegionDefinitions" ("Id") 
    ON DELETE RESTRICT;

COMMIT;

