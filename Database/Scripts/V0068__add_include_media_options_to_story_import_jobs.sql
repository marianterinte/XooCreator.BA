-- Adds IncludeImages, IncludeAudio, and IncludeVideo columns to StoryImportJobs table
-- These columns control which media types are imported during full story import

ALTER TABLE "alchimalia_schema"."StoryImportJobs"
    ADD COLUMN IF NOT EXISTS "IncludeImages" BOOLEAN NOT NULL DEFAULT TRUE,
    ADD COLUMN IF NOT EXISTS "IncludeAudio" BOOLEAN NOT NULL DEFAULT TRUE,
    ADD COLUMN IF NOT EXISTS "IncludeVideo" BOOLEAN NOT NULL DEFAULT TRUE;

