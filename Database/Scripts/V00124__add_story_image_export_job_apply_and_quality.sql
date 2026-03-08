-- Add ApplyToStoryWhenDone and ImageQuality to StoryImageExportJobs (idempotent)

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD COLUMN IF NOT EXISTS "ApplyToStoryWhenDone" BOOLEAN NOT NULL DEFAULT true;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD COLUMN IF NOT EXISTS "ImageQuality" VARCHAR(20) NULL;

COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."ApplyToStoryWhenDone" IS 'When true, client should apply generated images to the story after job completes';
COMMENT ON COLUMN alchimalia_schema."StoryImageExportJobs"."ImageQuality" IS 'Image quality: light | medium | heavy; used where supported (e.g. OpenAI size)';
