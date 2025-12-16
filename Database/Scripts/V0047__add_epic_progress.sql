-- Migration: Add Epic Progress tables for tracking user progress in Story Epics
-- Similar to TreeProgress and StoryProgress for Tree of Light

-- Create EpicProgress table
CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicProgress" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL,
    "RegionId" character varying(100) NOT NULL,
    "IsUnlocked" boolean NOT NULL,
    "EpicId" character varying(100) NOT NULL,
    "UnlockedAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_EpicProgress" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicProgress_AlchimaliaUsers_UserId" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE
    -- Note: EpicId foreign key constraint removed - EpicId can reference either StoryEpics (old) or StoryEpicDefinition (new)
    -- See V0042__remove_epic_progress_foreign_keys.sql
);

-- Create unique index for EpicProgress
CREATE UNIQUE INDEX IF NOT EXISTS "IX_EpicProgress_UserId_RegionId_EpicId" 
ON alchimalia_schema."EpicProgress" ("UserId", "RegionId", "EpicId");

-- Create EpicStoryProgress table
CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicStoryProgress" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL,
    "StoryId" character varying(100) NOT NULL,
    "SelectedAnswer" text,
    "TokensJson" text,
    "CompletedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "EpicId" character varying(100) NOT NULL,
    CONSTRAINT "PK_EpicStoryProgress" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicStoryProgress_AlchimaliaUsers_UserId" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE
    -- Note: EpicId foreign key constraint removed - EpicId can reference either StoryEpics (old) or StoryEpicDefinition (new)
    -- See V0042__remove_epic_progress_foreign_keys.sql
);

-- Create unique index for EpicStoryProgress
CREATE UNIQUE INDEX IF NOT EXISTS "IX_EpicStoryProgress_UserId_StoryId_EpicId" 
ON alchimalia_schema."EpicStoryProgress" ("UserId", "StoryId", "EpicId");

