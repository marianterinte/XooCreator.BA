-- Add StoryLikes table for tracking story likes per user
-- Run date: 2025-01-XX
-- Description: Adds StoryLikes table to track story likes. Only users who have read the story can like it.
--              Similar to StoryReaders but for likes instead of downloads.

BEGIN;

-- Create StoryLikes table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryLikes"
(
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" character varying(200) NOT NULL,
    "LikedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_StoryLikes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryLikes_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE
);

-- Create unique index on (UserId, StoryId) - one like per user per story
CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryLikes_UserId_StoryId"
    ON "alchimalia_schema"."StoryLikes" ("UserId", "StoryId");

-- Create index on StoryId for count queries
CREATE INDEX IF NOT EXISTS "IX_StoryLikes_StoryId"
    ON "alchimalia_schema"."StoryLikes" ("StoryId");

COMMIT;

