-- Adds StoryDefinitionUnlockedHeroes table
-- This table stores the many-to-many relationship between StoryDefinition and unlocked heroes (stored as string IDs)
-- Similar to StoryCraftUnlockedHeroes but for published stories
-- Run date: 2025-12-XX
-- Description: Allows published stories to unlock heroes when completed

BEGIN;

-- Create StoryDefinitionUnlockedHeroes table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryDefinitionUnlockedHeroes" (
    "StoryDefinitionId" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_StoryDefinitionUnlockedHeroes" PRIMARY KEY ("StoryDefinitionId", "HeroId"),
    CONSTRAINT "FK_StoryDefinitionUnlockedHeroes_StoryDefinitions_StoryDefinitionId" 
        FOREIGN KEY ("StoryDefinitionId") REFERENCES "alchimalia_schema"."StoryDefinitions" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_StoryDefinitionUnlockedHeroes_StoryDefinitionId" 
    ON "alchimalia_schema"."StoryDefinitionUnlockedHeroes" ("StoryDefinitionId");
CREATE INDEX IF NOT EXISTS "IX_StoryDefinitionUnlockedHeroes_HeroId" 
    ON "alchimalia_schema"."StoryDefinitionUnlockedHeroes" ("HeroId");

COMMIT;
