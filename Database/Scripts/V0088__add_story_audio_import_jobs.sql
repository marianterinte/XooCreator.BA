-- Migration: Add StoryAudioImportJobs table for background audio import (full audio upload)
-- Jobs are enqueued when user uploads a ZIP; worker processes and updates tiles with audio URLs

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryAudioImportJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(255) NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(255) NOT NULL,
    "OwnerEmail" VARCHAR(255) NOT NULL,
    "Locale" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "ZipBlobPath" TEXT NOT NULL,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "Success" BOOLEAN NOT NULL DEFAULT FALSE,
    "ImportedCount" INTEGER NOT NULL DEFAULT 0,
    "TotalPages" INTEGER NOT NULL DEFAULT 0,
    "ErrorsJson" TEXT NULL,
    "WarningsJson" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryAudioImportJobs_StoryId_Status" ON alchimalia_schema."StoryAudioImportJobs" ("StoryId", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryAudioImportJobs_QueuedAtUtc" ON alchimalia_schema."StoryAudioImportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_StoryAudioImportJobs_OwnerUserId" ON alchimalia_schema."StoryAudioImportJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."StoryAudioImportJobs" IS 'Tracks background audio import jobs (ZIP upload per story/locale)';
COMMENT ON COLUMN alchimalia_schema."StoryAudioImportJobs"."ZipBlobPath" IS 'Temp blob path where uploaded ZIP is stored until processed';
COMMENT ON COLUMN alchimalia_schema."StoryAudioImportJobs"."OwnerEmail" IS 'Email used for draft blob path (owner or requester)';
