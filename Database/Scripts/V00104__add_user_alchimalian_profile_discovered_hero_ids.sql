-- Add DiscoveredHeroIds (JSON array of hero definition IDs) to User Alchimalian Profile.
-- Users "discover" a hero when they tap Descoperă / Vezi ce erou poți deveni; discovered heroes stay visible.

BEGIN;

ALTER TABLE "alchimalia_schema"."UserAlchimalianProfiles"
ADD COLUMN IF NOT EXISTS "DiscoveredHeroIds" jsonb NOT NULL DEFAULT '[]';

COMMIT;
