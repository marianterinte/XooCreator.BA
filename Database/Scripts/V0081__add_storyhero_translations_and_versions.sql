-- Create StoryHeroTranslations and StoryHeroVersions tables to align with EF model
-- and migration 20260119130658_AddEpicTopicsAndAgeGroups.
-- Fixes: 42P01 relation "alchimalia_schema.StoryHeroTranslations" does not exist
-- when calling GET /api/{locale}/bestiary (e.g. bestiaryType=discovery).
--
-- These tables exist only in the EF migration; raw SQL scripts (V0001, etc.) never
-- created them, so DBs deployed only via DbScriptRunner are missing them.

BEGIN;

-- StoryHeroTranslations: translated Name, Description, GreetingText, GreetingAudioUrl per language
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryHeroTranslations" (
    "Id" uuid NOT NULL,
    "StoryHeroId" uuid NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Description" text,
    "GreetingText" text,
    "GreetingAudioUrl" character varying(500),
    CONSTRAINT "PK_StoryHeroTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryHeroTranslations_StoryHeroes_StoryHeroId"
        FOREIGN KEY ("StoryHeroId") REFERENCES alchimalia_schema."StoryHeroes" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryHeroTranslations_StoryHeroId_LanguageCode"
    ON alchimalia_schema."StoryHeroTranslations" ("StoryHeroId", "LanguageCode");

-- StoryHeroVersions: version snapshots for StoryHero workflow
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryHeroVersions" (
    "Id" uuid NOT NULL,
    "StoryHeroId" uuid NOT NULL,
    "Version" integer NOT NULL,
    "Status" character varying(50) NOT NULL,
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "SnapshotJson" text,
    CONSTRAINT "PK_StoryHeroVersions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryHeroVersions_StoryHeroes_StoryHeroId"
        FOREIGN KEY ("StoryHeroId") REFERENCES alchimalia_schema."StoryHeroes" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryHeroVersions_StoryHeroId_Version"
    ON alchimalia_schema."StoryHeroVersions" ("StoryHeroId", "Version");

COMMIT;
