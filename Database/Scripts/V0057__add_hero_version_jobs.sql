-- Hero Version Jobs table for background create version worker
-- Run date: 2025-12-17
-- Description: Adds HeroVersionJobs table used to track background create version operations per hero.

BEGIN;

CREATE TABLE IF NOT EXISTS alchimalia_schema."HeroVersionJobs"
(
    "Id" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_HeroVersionJobs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_HeroVersionJobs_HeroId_Status"
    ON alchimalia_schema."HeroVersionJobs" ("HeroId", "Status");

CREATE INDEX IF NOT EXISTS "IX_HeroVersionJobs_QueuedAtUtc"
    ON alchimalia_schema."HeroVersionJobs" ("QueuedAtUtc");

COMMIT;

