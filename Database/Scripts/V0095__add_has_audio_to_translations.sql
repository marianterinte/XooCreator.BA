-- Adds HasAudio flag to StoryCraftTranslations and StoryDefinitionTranslations tables.
-- This indicates whether a language has audio support for a story.

ALTER TABLE IF EXISTS alchimalia_schema."StoryCraftTranslations"
  ADD COLUMN IF NOT EXISTS "HasAudio" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE IF EXISTS alchimalia_schema."StoryDefinitionTranslations"
  ADD COLUMN IF NOT EXISTS "HasAudio" boolean NOT NULL DEFAULT FALSE;
