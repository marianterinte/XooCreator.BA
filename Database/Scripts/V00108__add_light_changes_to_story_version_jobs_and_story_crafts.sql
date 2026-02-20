-- Add LightChanges column to StoryVersionJobs and StoryCrafts.
-- Used by "New version -> Light changes" workflow to skip asset updates at publish time.
-- Idempotent: safe to run multiple times.

ALTER TABLE alchimalia_schema."StoryVersionJobs"
ADD COLUMN IF NOT EXISTS "LightChanges" boolean NOT NULL DEFAULT false;

ALTER TABLE alchimalia_schema."StoryCrafts"
ADD COLUMN IF NOT EXISTS "LightChanges" boolean NOT NULL DEFAULT false;
