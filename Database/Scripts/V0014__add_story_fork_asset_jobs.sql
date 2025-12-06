-- Adds StoryForkAssetJobs table for asynchronous fork asset copy

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryForkAssetJobs"
(
    "Id" UUID PRIMARY KEY,
    "SourceStoryId" VARCHAR(256) NOT NULL,
    "SourceType" VARCHAR(32) NOT NULL,
    "SourceOwnerUserId" UUID NULL,
    "SourceOwnerEmail" VARCHAR(256) NULL,
    "TargetStoryId" VARCHAR(256) NOT NULL,
    "TargetOwnerUserId" UUID NOT NULL,
    "TargetOwnerEmail" VARCHAR(256) NOT NULL,
    "RequestedByEmail" VARCHAR(256) NOT NULL,
    "Status" VARCHAR(32) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INT NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITHOUT TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITHOUT TIME ZONE NULL,
    "AttemptedAssets" INT NOT NULL DEFAULT 0,
    "CopiedAssets" INT NOT NULL DEFAULT 0,
    "ErrorMessage" TEXT NULL,
    "WarningSummary" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryForkAssetJobs_TargetStoryId_Status"
    ON "alchimalia_schema"."StoryForkAssetJobs" ("TargetStoryId", "Status");

CREATE INDEX IF NOT EXISTS "IX_StoryForkAssetJobs_SourceStoryId"
    ON "alchimalia_schema"."StoryForkAssetJobs" ("SourceStoryId");

