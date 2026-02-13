-- Add AudioLanguages column to StoryDefinitions and StoryCrafts.
-- Stores audio-supported language codes directly on the story entity for performance 
-- (avoids joining to translations just to check audio support).

ALTER TABLE alchimalia_schema."StoryDefinitions"
ADD COLUMN IF NOT EXISTS "AudioLanguages" text[] NOT NULL DEFAULT '{}';

ALTER TABLE alchimalia_schema."StoryCrafts"
ADD COLUMN IF NOT EXISTS "AudioLanguages" text[] NOT NULL DEFAULT '{}';
