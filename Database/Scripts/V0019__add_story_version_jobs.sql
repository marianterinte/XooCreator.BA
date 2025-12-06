-- Story Version Jobs table for background create version worker
-- Run date: 2025-01-XX
-- Description: Adds StoryVersionJobs table used to track background create version operations per story.

BEGIN;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryVersionJobs"
(
    "Id" uuid NOT NULL,
    "StoryId" character varying(200) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_StoryVersionJobs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_StoryVersionJobs_StoryId_Status"
    ON alchimalia_schema."StoryVersionJobs" ("StoryId", "Status");

CREATE INDEX IF NOT EXISTS "IX_StoryVersionJobs_QueuedAtUtc"
    ON alchimalia_schema."StoryVersionJobs" ("QueuedAtUtc");

COMMIT;

