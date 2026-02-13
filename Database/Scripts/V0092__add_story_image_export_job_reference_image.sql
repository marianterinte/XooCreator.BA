-- Add reference image and prompt columns to StoryImageExportJobs (idempotent)

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD COLUMN IF NOT EXISTS "ReferenceImageBase64" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD COLUMN IF NOT EXISTS "ReferenceImageMimeType" VARCHAR(128) NULL;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD COLUMN IF NOT EXISTS "ExtraInstructions" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD COLUMN IF NOT EXISTS "ImageModel" VARCHAR(128) NULL;
