-- Adds PublishedAtUtc column to StoryEpics to track publish timestamp

BEGIN;

ALTER TABLE "alchimalia_schema"."StoryEpics"
    ADD COLUMN IF NOT EXISTS "PublishedAtUtc" timestamp with time zone NULL;

COMMIT;


