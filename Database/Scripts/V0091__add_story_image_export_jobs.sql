-- Migration: Add StoryImageExportJobs table for background image export processing
-- Same pattern as StoryAudioExportJobs: job is enqueued, worker generates images per page, ZIP uploaded to blob

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryImageExportJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(255) NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(255) NOT NULL,
    "Locale" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "Provider" VARCHAR(50) NOT NULL DEFAULT 'Google',
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "ZipBlobPath" TEXT NULL,
    "ZipFileName" VARCHAR(255) NULL,
    "ZipSizeBytes" BIGINT NULL,
    "ImageCount" INTEGER NULL,
    "SelectedTileIdsJson" TEXT NULL,
    "ApiKeyOverride" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryImageExportJobs_StoryId_Status" ON alchimalia_schema."StoryImageExportJobs" ("StoryId", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryImageExportJobs_QueuedAtUtc" ON alchimalia_schema."StoryImageExportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_StoryImageExportJobs_OwnerUserId" ON alchimalia_schema."StoryImageExportJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."StoryImageExportJobs" IS 'Tracks background image export jobs for story illustration ZIP generation';
COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."Status" IS 'Job status: Queued, Running, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."Provider" IS 'Image provider: Google (Nano Banana), OpenAI (future)';
COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."ZipBlobPath" IS 'Path to the generated ZIP file in blob storage (exports/images/{jobId}/)';
COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."SelectedTileIdsJson" IS 'JSON array of selected tile GUIDs, null = all pages';
COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."ApiKeyOverride" IS 'Google API key from UI for image generation';
