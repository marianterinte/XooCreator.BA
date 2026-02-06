-- Migration: Add ApiKeyOverride column to StoryAudioExportJobs table
-- This column allows users to provide an optional API key from the UI

-- Add ApiKeyOverride column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryAudioExportJobs' 
        AND column_name = 'ApiKeyOverride'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryAudioExportJobs" 
        ADD COLUMN "ApiKeyOverride" TEXT NULL;
    END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."StoryAudioExportJobs"."ApiKeyOverride" IS 'Optional API key from UI, null = use default from server configuration';
