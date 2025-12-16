-- Add unique indexes for IsStartupRegion constraint on StoryEpicDefinitionRegions and StoryEpicCraftRegions
-- Note: IsStartupRegion column is already created in V0037__add_story_epic_craft_and_definition.sql
-- This script only adds the unique constraint indexes to ensure only one startup region per epic
-- Run date: 2025-12-XX
-- Description: Creates unique partial indexes to ensure only one startup region per epic.

BEGIN;

-- Create unique partial index on StoryEpicDefinitionRegions to ensure only one startup region per epic
-- This index allows multiple false values but only one true value per EpicId
CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryEpicDefinitionRegions_EpicId_IsStartupRegion_Unique"
    ON alchimalia_schema."StoryEpicDefinitionRegions" ("EpicId")
    WHERE "IsStartupRegion" = true;

-- Create unique partial index on StoryEpicCraftRegions to ensure only one startup region per epic
-- This index allows multiple false values but only one true value per EpicId
CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryEpicCraftRegions_EpicId_IsStartupRegion_Unique"
    ON alchimalia_schema."StoryEpicCraftRegions" ("EpicId")
    WHERE "IsStartupRegion" = true;

COMMIT;

