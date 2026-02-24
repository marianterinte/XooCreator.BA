-- Add MarketplaceCoverImageUrl to StoryEpicDefinitions and StoryEpicCrafts
-- Description: Separate cover image for marketplace (distinct from Tree Logic background).
-- Idempotent: ADD COLUMN IF NOT EXISTS.

BEGIN;

ALTER TABLE alchimalia_schema."StoryEpicDefinitions"
    ADD COLUMN IF NOT EXISTS "MarketplaceCoverImageUrl" character varying(500) NULL;

ALTER TABLE alchimalia_schema."StoryEpicCrafts"
    ADD COLUMN IF NOT EXISTS "MarketplaceCoverImageUrl" character varying(500) NULL;

COMMIT;
