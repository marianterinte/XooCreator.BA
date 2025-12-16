-- Add UserFavoriteEpics table for tracking user favorite epics
-- Run date: 2025-12-XX
-- Description: Adds UserFavoriteEpics table to allow users to mark epics as favorites.
-- Similar to UserFavoriteStories but for epics.

BEGIN;

-- Create UserFavoriteEpics table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."UserFavoriteEpics" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "EpicId" character varying(100) NOT NULL,
    "AddedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_UserFavoriteEpics" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserFavoriteEpics_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserFavoriteEpics_StoryEpicDefinitions_EpicId" 
        FOREIGN KEY ("EpicId") REFERENCES "alchimalia_schema"."StoryEpicDefinitions" ("Id") ON DELETE CASCADE
);

-- Create unique index to ensure one favorite per user per epic
CREATE UNIQUE INDEX IF NOT EXISTS "IX_UserFavoriteEpics_UserId_EpicId" 
    ON "alchimalia_schema"."UserFavoriteEpics" ("UserId", "EpicId");

-- Create index on EpicId for queries
CREATE INDEX IF NOT EXISTS "IX_UserFavoriteEpics_EpicId" 
    ON "alchimalia_schema"."UserFavoriteEpics" ("EpicId");

COMMIT;

