-- Persist per-tile selected heroes for character-selection tiles
-- Stores JSON array of hero IDs (e.g. ["hero-a","hero-b"])

ALTER TABLE alchimalia_schema."StoryCraftTiles"
    ADD COLUMN IF NOT EXISTS "AvailableHeroIdsJson" text NULL;

ALTER TABLE alchimalia_schema."StoryTiles"
    ADD COLUMN IF NOT EXISTS "AvailableHeroIdsJson" text NULL;
