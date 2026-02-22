-- Migration: Add IncludeVideo, IncludeAudio, IncludeImages to StoryExportJobs
-- When all are false, export contains only JSON (story + dialogs). Default true for backward compatibility.

ALTER TABLE alchimalia_schema."StoryExportJobs"
    ADD COLUMN IF NOT EXISTS "IncludeVideo" BOOLEAN NULL DEFAULT TRUE,
    ADD COLUMN IF NOT EXISTS "IncludeAudio" BOOLEAN NULL DEFAULT TRUE,
    ADD COLUMN IF NOT EXISTS "IncludeImages" BOOLEAN NULL DEFAULT TRUE;

COMMENT ON COLUMN alchimalia_schema."StoryExportJobs"."IncludeVideo" IS 'When true (default), include video assets in export ZIP';
COMMENT ON COLUMN alchimalia_schema."StoryExportJobs"."IncludeAudio" IS 'When true (default), include audio assets in export ZIP';
COMMENT ON COLUMN alchimalia_schema."StoryExportJobs"."IncludeImages" IS 'When true (default), include image assets in export ZIP';
