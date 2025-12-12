-- Add IsStartupRegion field to StoryEpicRegions
-- Run date: 2025-12-XX
-- Description: Adds IsStartupRegion boolean field to mark a region as the starting region for an epic.
-- Only one region per epic can be marked as startup region.

BEGIN;

-- Add IsStartupRegion column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryEpicRegions' 
        AND column_name = 'IsStartupRegion'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryEpicRegions"
        ADD COLUMN "IsStartupRegion" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

-- Create unique partial index to ensure only one startup region per epic
-- This index allows multiple false values but only one true value per EpicId
CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryEpicRegions_EpicId_IsStartupRegion_Unique"
    ON alchimalia_schema."StoryEpicRegions" ("EpicId")
    WHERE "IsStartupRegion" = true;

COMMIT;

