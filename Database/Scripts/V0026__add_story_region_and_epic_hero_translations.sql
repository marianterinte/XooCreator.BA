-- Migration: Add translations tables for StoryRegion and EpicHero
-- Following the same pattern as StoryCraftTranslation and StoryDefinitionTranslation

-- Create StoryRegionTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryRegionTranslations" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "StoryRegionId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL DEFAULT 'ro-ro',
    "Name" character varying(200) NOT NULL,
    "Description" text,
    CONSTRAINT "PK_StoryRegionTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryRegionTranslations_StoryRegions_StoryRegionId"
        FOREIGN KEY ("StoryRegionId") REFERENCES "alchimalia_schema"."StoryRegions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryRegionTranslations_StoryRegionId_LanguageCode"
        UNIQUE ("StoryRegionId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_StoryRegionTranslations_StoryRegionId" 
    ON "alchimalia_schema"."StoryRegionTranslations" ("StoryRegionId");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionTranslations_LanguageCode" 
    ON "alchimalia_schema"."StoryRegionTranslations" ("LanguageCode");

-- Create EpicHeroTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicHeroTranslations" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "EpicHeroId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL DEFAULT 'ro-ro',
    "Name" character varying(200) NOT NULL,
    "GreetingText" character varying(1000),
    CONSTRAINT "PK_EpicHeroTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicHeroTranslations_EpicHeroes_EpicHeroId"
        FOREIGN KEY ("EpicHeroId") REFERENCES "alchimalia_schema"."EpicHeroes" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_EpicHeroTranslations_EpicHeroId_LanguageCode"
        UNIQUE ("EpicHeroId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_EpicHeroTranslations_EpicHeroId" 
    ON "alchimalia_schema"."EpicHeroTranslations" ("EpicHeroId");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroTranslations_LanguageCode" 
    ON "alchimalia_schema"."EpicHeroTranslations" ("LanguageCode");

