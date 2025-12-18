-- Epic Version Jobs table for background create version worker
-- Run date: 2025-12-XX
-- Description: Adds EpicVersionJobs table used to track background create version operations per epic.

BEGIN;

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicVersionJobs"
(
    "Id" uuid NOT NULL,
    "EpicId" character varying(200) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_EpicVersionJobs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_EpicVersionJobs_EpicId_Status"
    ON alchimalia_schema."EpicVersionJobs" ("EpicId", "Status");

CREATE INDEX IF NOT EXISTS "IX_EpicVersionJobs_QueuedAtUtc"
    ON alchimalia_schema."EpicVersionJobs" ("QueuedAtUtc");

COMMIT;





