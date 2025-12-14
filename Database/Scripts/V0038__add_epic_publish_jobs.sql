-- Epic Publish Jobs table for background publish worker
-- Run date: 2025-12-XX
-- Description: Adds EpicPublishJobs table used to track background publish operations per epic.

BEGIN;

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicPublishJobs"
(
    "Id" uuid NOT NULL,
    "EpicId" character varying(200) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "LangTag" character varying(10) NOT NULL DEFAULT 'ro-ro',
    "DraftVersion" integer NOT NULL,
    "ForceFull" boolean NOT NULL DEFAULT false,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_EpicPublishJobs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_EpicPublishJobs_EpicId_Status"
    ON alchimalia_schema."EpicPublishJobs" ("EpicId", "Status");

CREATE INDEX IF NOT EXISTS "IX_EpicPublishJobs_QueuedAtUtc"
    ON alchimalia_schema."EpicPublishJobs" ("QueuedAtUtc");

COMMIT;

