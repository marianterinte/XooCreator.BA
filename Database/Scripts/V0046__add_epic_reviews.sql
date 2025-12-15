-- Add EpicReviews table for user reviews on epics
-- Run date: 2025-12-XX
-- Description: Adds EpicReviews table to allow users to review epics.
-- Similar to StoryReviews but for epics.

BEGIN;

-- Create EpicReviews table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicReviews" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "EpicId" character varying(100) NOT NULL,
    "Rating" integer NOT NULL,
    "Comment" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "IsActive" boolean NOT NULL DEFAULT true,
    CONSTRAINT "PK_EpicReviews" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicReviews_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicReviews_StoryEpicDefinitions_EpicId" 
        FOREIGN KEY ("EpicId") REFERENCES "alchimalia_schema"."StoryEpicDefinitions" ("Id") ON DELETE CASCADE
);

-- Create unique index to ensure one review per user per epic
CREATE UNIQUE INDEX IF NOT EXISTS "IX_EpicReviews_UserId_EpicId" 
    ON "alchimalia_schema"."EpicReviews" ("UserId", "EpicId");

-- Create index on EpicId for queries
CREATE INDEX IF NOT EXISTS "IX_EpicReviews_EpicId" 
    ON "alchimalia_schema"."EpicReviews" ("EpicId");

-- Create index on IsActive for filtering active reviews
CREATE INDEX IF NOT EXISTS "IX_EpicReviews_IsActive" 
    ON "alchimalia_schema"."EpicReviews" ("IsActive");

COMMIT;

