-- Adds StoryImportJobs table for background import processing

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryImportJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(256) NOT NULL,
    "OriginalStoryId" VARCHAR(256) NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(256) NOT NULL,
    "Locale" VARCHAR(16) NOT NULL,
    "ZipBlobPath" VARCHAR(512) NOT NULL,
    "ZipFileName" VARCHAR(256) NOT NULL,
    "ZipSizeBytes" BIGINT NOT NULL DEFAULT 0,
    "Status" VARCHAR(32) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INT NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITHOUT TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITHOUT TIME ZONE NULL,
    "ImportedAssets" INT NOT NULL DEFAULT 0,
    "TotalAssets" INT NOT NULL DEFAULT 0,
    "ImportedLanguagesCount" INT NOT NULL DEFAULT 0,
    "ErrorMessage" TEXT NULL,
    "WarningSummary" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryImportJobs_StoryId_Status"
    ON "alchimalia_schema"."StoryImportJobs" ("StoryId", "Status");

CREATE INDEX IF NOT EXISTS "IX_StoryImportJobs_OwnerUserId"
    ON "alchimalia_schema"."StoryImportJobs" ("OwnerUserId");

