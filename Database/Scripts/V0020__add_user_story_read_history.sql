-- User Story Read History table for permanent reading history tracking
-- Run date: 2025-01-07
-- Description: Adds UserStoryReadHistory table to preserve reading history even after progress is reset.
--              Stores aggregate progress (total tiles read, percentage) per user per story.
--              This allows parent dashboard to show read stories even after completion and progress reset.

BEGIN;

-- Create UserStoryReadHistory table
CREATE TABLE IF NOT EXISTS alchimalia_schema."UserStoryReadHistory"
(
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" character varying(200) NOT NULL,
    "TotalTilesRead" integer NOT NULL,
    "TotalTiles" integer NOT NULL,
    "LastReadAt" timestamp with time zone NOT NULL,
    "CompletedAt" timestamp with time zone,
    CONSTRAINT "PK_UserStoryReadHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserStoryReadHistory_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE
);

-- Create unique index on (UserId, StoryId) - one history record per user per story
CREATE UNIQUE INDEX IF NOT EXISTS "IX_UserStoryReadHistory_UserId_StoryId"
    ON alchimalia_schema."UserStoryReadHistory" ("UserId", "StoryId");

-- Create index on CompletedAt for filtering completed stories
CREATE INDEX IF NOT EXISTS "IX_UserStoryReadHistory_CompletedAt"
    ON alchimalia_schema."UserStoryReadHistory" ("CompletedAt");

COMMIT;

