-- Add AlwaysShowInStoriesList flag to StoryDefinitions and StoryCrafts
-- Description: Allows specific epic stories to still appear in standalone marketplace story lists.
-- Idempotent: safe to run multiple times.

BEGIN;

-- Add AlwaysShowInStoriesList column to StoryDefinitions if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'alchimalia_schema'
          AND table_name = 'StoryDefinitions'
          AND column_name = 'AlwaysShowInStoriesList'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryDefinitions"
            ADD COLUMN "AlwaysShowInStoriesList" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

-- Add AlwaysShowInStoriesList column to StoryCrafts if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'alchimalia_schema'
          AND table_name = 'StoryCrafts'
          AND column_name = 'AlwaysShowInStoriesList'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryCrafts"
            ADD COLUMN "AlwaysShowInStoriesList" boolean NOT NULL DEFAULT false;
    END IF;
END $$;

COMMIT;

