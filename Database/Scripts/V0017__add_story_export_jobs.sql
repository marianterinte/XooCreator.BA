-- Migration: Add StoryExportJobs table for background export processing
-- This table tracks export jobs for both draft and published stories

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryExportJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(255) NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(255) NOT NULL,
    "Locale" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "IsDraft" BOOLEAN NOT NULL DEFAULT FALSE,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "ZipBlobPath" TEXT NULL,
    "ZipFileName" VARCHAR(255) NULL,
    "ZipSizeBytes" BIGINT NULL,
    "MediaCount" INTEGER NULL,
    "LanguageCount" INTEGER NULL
);

-- Indexes for efficient querying
CREATE INDEX IF NOT EXISTS "IX_StoryExportJobs_StoryId_Status" ON alchimalia_schema."StoryExportJobs" ("StoryId", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryExportJobs_QueuedAtUtc" ON alchimalia_schema."StoryExportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_StoryExportJobs_OwnerUserId" ON alchimalia_schema."StoryExportJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."StoryExportJobs" IS 'Tracks background export jobs for story ZIP generation';
COMMENT ON COLUMN alchimalia_schema."StoryExportJobs"."Status" IS 'Job status: Queued, Running, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."StoryExportJobs"."ZipBlobPath" IS 'Path to the generated ZIP file in blob storage';
COMMENT ON COLUMN alchimalia_schema."StoryExportJobs"."IsDraft" IS 'True if exporting draft story, false if exporting published story';
