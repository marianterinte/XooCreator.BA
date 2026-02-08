-- Add reference image and prompt columns to StoryImageExportJobs (same schema as V0091)

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD "ReferenceImageBase64" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD "ReferenceImageMimeType" VARCHAR(128) NULL;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD "ExtraInstructions" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryImageExportJobs"
    ADD "ImageModel" VARCHAR(128) NULL;
