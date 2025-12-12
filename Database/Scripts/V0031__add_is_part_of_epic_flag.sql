-- Add IsPartOfEpic field to StoryDefinitions and StoryCrafts
-- Run date: 2025-01-XX
-- Description: Adds IsPartOfEpic boolean field to mark stories that are part of an epic (draft or published).
-- Stories with IsPartOfEpic = true should not appear as independent stories in marketplace.

BEGIN;

-- Add IsPartOfEpic column to StoryDefinitions if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryDefinitions' 
        AND column_name = 'IsPartOfEpic'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryDefinitions"
        ADD COLUMN "IsPartOfEpic" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

-- Add IsPartOfEpic column to StoryCrafts if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryCrafts' 
        AND column_name = 'IsPartOfEpic'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryCrafts"
        ADD COLUMN "IsPartOfEpic" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

-- Update existing StoryDefinitions that are in epics
UPDATE alchimalia_schema."StoryDefinitions" sd
SET "IsPartOfEpic" = true
WHERE EXISTS (
    SELECT 1 
    FROM alchimalia_schema."StoryEpicStoryNodes" sn
    WHERE sn."StoryId" = sd."StoryId"
);

-- Update existing StoryCrafts that are in epics
UPDATE alchimalia_schema."StoryCrafts" sc
SET "IsPartOfEpic" = true
WHERE EXISTS (
    SELECT 1 
    FROM alchimalia_schema."StoryEpicStoryNodes" sn
    WHERE sn."StoryId" = sc."StoryId"
);

COMMIT;

