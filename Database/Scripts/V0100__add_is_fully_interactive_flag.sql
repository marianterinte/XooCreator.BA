-- Add IsFullyInteractive field to StoryDefinitions and StoryCrafts
-- Description: Adds IsFullyInteractive boolean field for stories that are fully interactive.
-- Idempotent: safe to run multiple times.

BEGIN;

-- Add IsFullyInteractive column to StoryDefinitions if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryDefinitions' 
        AND column_name = 'IsFullyInteractive'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryDefinitions"
        ADD COLUMN "IsFullyInteractive" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

-- Add IsFullyInteractive column to StoryCrafts if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryCrafts' 
        AND column_name = 'IsFullyInteractive'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryCrafts"
        ADD COLUMN "IsFullyInteractive" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

COMMIT;
