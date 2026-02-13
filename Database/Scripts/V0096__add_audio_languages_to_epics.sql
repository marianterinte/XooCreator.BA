-- Add AudioLanguages column to StoryEpicDefinitions and StoryEpicCrafts.
-- Stores audio-supported language codes per epic (same pattern as stories).

ALTER TABLE alchimalia_schema."StoryEpicDefinitions"
ADD COLUMN IF NOT EXISTS "AudioLanguages" text[] NOT NULL DEFAULT '{}';

ALTER TABLE alchimalia_schema."StoryEpicCrafts"
ADD COLUMN IF NOT EXISTS "AudioLanguages" text[] NOT NULL DEFAULT '{}';
