-- Adds StoryForkJobs table for asynchronous fork processing

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryForkJobs"
(
    "Id" UUID PRIMARY KEY,
    "SourceStoryId" VARCHAR(256) NOT NULL,
    "SourceType" VARCHAR(32) NOT NULL,
    "CopyAssets" BOOLEAN NOT NULL DEFAULT FALSE,
    "RequestedByUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(256) NOT NULL,
    "TargetOwnerUserId" UUID NOT NULL,
    "TargetOwnerEmail" VARCHAR(256) NOT NULL,
    "TargetStoryId" VARCHAR(256) NOT NULL,
    "Status" VARCHAR(32) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INT NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITHOUT TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITHOUT TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,
    "WarningSummary" TEXT NULL,
    "AssetJobId" UUID NULL,
    "AssetJobStatus" VARCHAR(32) NULL,
    "SourceTranslations" INT NOT NULL DEFAULT 0,
    "SourceTiles" INT NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS "IX_StoryForkJobs_TargetStoryId_Status"
    ON "alchimalia_schema"."StoryForkJobs" ("TargetStoryId", "Status");

CREATE INDEX IF NOT EXISTS "IX_StoryForkJobs_RequestedByUserId"
    ON "alchimalia_schema"."StoryForkJobs" ("RequestedByUserId");


