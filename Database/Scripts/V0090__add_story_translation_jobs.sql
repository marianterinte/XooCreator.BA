-- Migration: Add StoryTranslationJobs table for background story translation
-- Same pattern as StoryAudioExportJobs: job is enqueued, worker processes it

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryTranslationJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(255) NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    "ReferenceLanguage" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "TargetLanguagesJson" TEXT NOT NULL DEFAULT '[]',
    "SelectedTileIdsJson" TEXT NULL,
    "ApiKeyOverride" TEXT NULL,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "FieldsTranslated" INTEGER NULL,
    "UpdatedLanguagesJson" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryTranslationJobs_StoryId_Status" ON alchimalia_schema."StoryTranslationJobs" ("StoryId", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryTranslationJobs_QueuedAtUtc" ON alchimalia_schema."StoryTranslationJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_StoryTranslationJobs_OwnerUserId" ON alchimalia_schema."StoryTranslationJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."StoryTranslationJobs" IS 'Tracks background story translation jobs (reference + target languages, optional selected tiles)';
COMMENT ON COLUMN alchimalia_schema."StoryTranslationJobs"."Status" IS 'Job status: Queued, Running, Completed, Failed';
