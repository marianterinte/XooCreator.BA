-- V0061: Add IsActive column to EpicHeroDefinitions and StoryRegionDefinitions for soft delete / unpublish

-- Add IsActive to EpicHeroDefinitions
ALTER TABLE alchimalia_schema."EpicHeroDefinitions"
ADD COLUMN "IsActive" boolean NOT NULL DEFAULT true;

-- Add IsActive to StoryRegionDefinitions
ALTER TABLE alchimalia_schema."StoryRegionDefinitions"
ADD COLUMN "IsActive" boolean NOT NULL DEFAULT true;

-- Create index for IsActive queries (for filtering published/active entities)
CREATE INDEX "IX_EpicHeroDefinitions_IsActive" ON alchimalia_schema."EpicHeroDefinitions" ("IsActive");
CREATE INDEX "IX_StoryRegionDefinitions_IsActive" ON alchimalia_schema."StoryRegionDefinitions" ("IsActive");

