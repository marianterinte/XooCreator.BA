-- Story Publish Jobs table for background publish worker
-- Run date: 2025-12-04 10:30:00+02:00
-- Description: Adds StoryPublishJobs table used to track background publish operations per story.

BEGIN;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryPublishJobs"
(
    "Id" uuid NOT NULL,
    "StoryId" character varying(200) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "LangTag" character varying(10) NOT NULL,
    "DraftVersion" integer NOT NULL,
    "ForceFull" boolean NOT NULL DEFAULT FALSE,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_StoryPublishJobs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_StoryPublishJobs_StoryId_Status"
    ON alchimalia_schema."StoryPublishJobs" ("StoryId", "Status");

CREATE INDEX IF NOT EXISTS "IX_StoryPublishJobs_QueuedAtUtc"
    ON alchimalia_schema."StoryPublishJobs" ("QueuedAtUtc");

COMMIT;


