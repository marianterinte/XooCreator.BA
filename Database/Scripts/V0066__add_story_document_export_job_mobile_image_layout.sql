-- Migration: Add mobile image layout flag for story document export jobs
-- When true, PDF renderer will center images at ~60% width to mimic mobile layout and leave space for text.

ALTER TABLE IF EXISTS alchimalia_schema."StoryDocumentExportJobs"
    ADD COLUMN IF NOT EXISTS "UseMobileImageLayout" BOOLEAN NOT NULL DEFAULT TRUE;

COMMENT ON COLUMN alchimalia_schema."StoryDocumentExportJobs"."UseMobileImageLayout"
    IS 'If true, render images centered at ~60% width to mimic mobile layout and leave space for text';


