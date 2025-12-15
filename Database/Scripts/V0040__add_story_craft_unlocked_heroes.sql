-- Adds StoryCraftUnlockedHeroes table
-- This table stores the many-to-many relationship between StoryCraft and unlocked heroes (stored as string IDs)
-- Run date: 2025-12-XX
-- Description: Allows stories to unlock heroes when completed

BEGIN;

-- Create StoryCraftUnlockedHeroes table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCraftUnlockedHeroes" (
    "StoryCraftId" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_StoryCraftUnlockedHeroes" PRIMARY KEY ("StoryCraftId", "HeroId"),
    CONSTRAINT "FK_StoryCraftUnlockedHeroes_StoryCrafts_StoryCraftId" 
        FOREIGN KEY ("StoryCraftId") REFERENCES "alchimalia_schema"."StoryCrafts" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_StoryCraftUnlockedHeroes_StoryCraftId" 
    ON "alchimalia_schema"."StoryCraftUnlockedHeroes" ("StoryCraftId");
CREATE INDEX IF NOT EXISTS "IX_StoryCraftUnlockedHeroes_HeroId" 
    ON "alchimalia_schema"."StoryCraftUnlockedHeroes" ("HeroId");

COMMIT;
