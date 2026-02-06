-- Migration: Add StoryAudioExportJobs table for background audio export processing
-- This table tracks audio export jobs for draft stories

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryAudioExportJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(255) NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(255) NOT NULL,
    "Locale" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "ZipBlobPath" TEXT NULL,
    "ZipFileName" VARCHAR(255) NULL,
    "ZipSizeBytes" BIGINT NULL,
    "AudioCount" INTEGER NULL,
    "SelectedTileIdsJson" TEXT NULL -- JSON array of selected tile GUIDs, null = all pages
);

-- Add SelectedTileIdsJson column if it doesn't exist (for existing tables)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryAudioExportJobs' 
        AND column_name = 'SelectedTileIdsJson'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryAudioExportJobs" 
        ADD COLUMN "SelectedTileIdsJson" TEXT NULL;
    END IF;
END $$;

-- Indexes for efficient querying
CREATE INDEX IF NOT EXISTS "IX_StoryAudioExportJobs_StoryId_Status" ON alchimalia_schema."StoryAudioExportJobs" ("StoryId", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryAudioExportJobs_QueuedAtUtc" ON alchimalia_schema."StoryAudioExportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_StoryAudioExportJobs_OwnerUserId" ON alchimalia_schema."StoryAudioExportJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."StoryAudioExportJobs" IS 'Tracks background audio export jobs for story audio ZIP generation';
COMMENT ON COLUMN alchimalia_schema."StoryAudioExportJobs"."Status" IS 'Job status: Queued, Running, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."StoryAudioExportJobs"."ZipBlobPath" IS 'Path to the generated ZIP file in blob storage';
COMMENT ON COLUMN alchimalia_schema."StoryAudioExportJobs"."SelectedTileIdsJson" IS 'JSON array of selected tile GUIDs for audio generation, null = all pages';
