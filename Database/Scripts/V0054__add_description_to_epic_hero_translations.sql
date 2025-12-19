-- Migration: Add Description and GreetingAudioUrl columns to EpicHeroTranslations table
-- This adds support for hero descriptions and audio URLs per language, similar to StoryRegionTranslations

-- Add Description column to EpicHeroTranslations table
ALTER TABLE "alchimalia_schema"."EpicHeroTranslations"
    ADD COLUMN IF NOT EXISTS "Description" text;

-- Add comment to document the column
COMMENT ON COLUMN "alchimalia_schema"."EpicHeroTranslations"."Description" IS 'Optional description for the hero in this language';

-- Add GreetingAudioUrl column to EpicHeroTranslations table (per language)
ALTER TABLE "alchimalia_schema"."EpicHeroTranslations"
    ADD COLUMN IF NOT EXISTS "GreetingAudioUrl" character varying(1000);

-- Add comment to document the column
COMMENT ON COLUMN "alchimalia_schema"."EpicHeroTranslations"."GreetingAudioUrl" IS 'Audio URL for greeting in this language';

