-- Add ShowInUniverse flag to StoryRegionCrafts and StoryRegionDefinitions.
-- When true and region is published, the region appears on Explore Alchimalia Universe page.
-- Idempotent: safe to run multiple times.

BEGIN;

-- Add ShowInUniverse column to StoryRegionCrafts if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'alchimalia_schema'
          AND table_name = 'StoryRegionCrafts'
          AND column_name = 'ShowInUniverse'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryRegionCrafts"
            ADD COLUMN "ShowInUniverse" boolean NULL DEFAULT false;
    END IF;
END $$;

-- Add ShowInUniverse column to StoryRegionDefinitions if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'alchimalia_schema'
          AND table_name = 'StoryRegionDefinitions'
          AND column_name = 'ShowInUniverse'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryRegionDefinitions"
            ADD COLUMN "ShowInUniverse" boolean NULL DEFAULT false;
    END IF;
END $$;

COMMIT;
