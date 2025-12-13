-- Migration: Add translations table for StoryEpic
-- Following the same pattern as StoryRegionTranslation and EpicHeroTranslation

-- Create StoryEpicTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryEpicTranslations" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "StoryEpicId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL DEFAULT 'ro-ro',
    "Name" character varying(200) NOT NULL,
    "Description" character varying(1000),
    CONSTRAINT "PK_StoryEpicTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryEpicTranslations_StoryEpics_StoryEpicId"
        FOREIGN KEY ("StoryEpicId") REFERENCES "alchimalia_schema"."StoryEpics" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryEpicTranslations_StoryEpicId_LanguageCode"
        UNIQUE ("StoryEpicId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_StoryEpicTranslations_StoryEpicId" 
    ON "alchimalia_schema"."StoryEpicTranslations" ("StoryEpicId");
CREATE INDEX IF NOT EXISTS "IX_StoryEpicTranslations_LanguageCode" 
    ON "alchimalia_schema"."StoryEpicTranslations" ("LanguageCode");

