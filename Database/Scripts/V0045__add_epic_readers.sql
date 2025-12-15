-- Add EpicReaders table for tracking unique readers per epic
-- Run date: 2025-12-XX
-- Description: Adds EpicReaders table to track unique readers (downloads) per epic.
-- Similar to StoryReaders but for epics.

BEGIN;

-- Create EpicReaders table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicReaders" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "EpicId" character varying(100) NOT NULL,
    "AcquiredAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "AcquisitionSource" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_EpicReaders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicReaders_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicReaders_StoryEpicDefinitions_EpicId" 
        FOREIGN KEY ("EpicId") REFERENCES "alchimalia_schema"."StoryEpicDefinitions" ("Id") ON DELETE CASCADE
);

-- Create unique index to ensure one reader per user per epic
CREATE UNIQUE INDEX IF NOT EXISTS "IX_EpicReaders_UserId_EpicId" 
    ON "alchimalia_schema"."EpicReaders" ("UserId", "EpicId");

-- Create index on EpicId for count queries
CREATE INDEX IF NOT EXISTS "IX_EpicReaders_EpicId" 
    ON "alchimalia_schema"."EpicReaders" ("EpicId");

COMMIT;

