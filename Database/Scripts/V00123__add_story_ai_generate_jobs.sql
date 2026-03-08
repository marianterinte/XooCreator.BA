-- Migration: Add StoryAIGenerateJobs table for async "Generate Full AI Story" flow
-- Worker processes job by deserializing RequestJson and calling the draft handler.

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryAIGenerateJobs"
(
    "Id" UUID PRIMARY KEY,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(255) NOT NULL,
    "OwnerFirstName" VARCHAR(255) NULL,
    "OwnerLastName" VARCHAR(255) NULL,
    "Locale" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "ErrorCode" VARCHAR(100) NULL,
    "StoryId" VARCHAR(255) NULL,
    "RequestJson" TEXT NOT NULL,
    "ProgressMessage" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryAIGenerateJobs_OwnerUserId" ON alchimalia_schema."StoryAIGenerateJobs" ("OwnerUserId");
CREATE INDEX IF NOT EXISTS "IX_StoryAIGenerateJobs_Status_QueuedAtUtc" ON alchimalia_schema."StoryAIGenerateJobs" ("Status", "QueuedAtUtc");

COMMENT ON TABLE alchimalia_schema."StoryAIGenerateJobs" IS 'Background jobs for Generate Full AI Story from list (async flow)';
COMMENT ON COLUMN alchimalia_schema."StoryAIGenerateJobs"."Status" IS 'Queued, Running, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."StoryAIGenerateJobs"."RequestJson" IS 'JSON-serialized GenerateFullStoryDraftRequest';
COMMENT ON COLUMN alchimalia_schema."StoryAIGenerateJobs"."ErrorCode" IS 'e.g. RateLimitExceeded for 429';
