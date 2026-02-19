-- Adds staging metadata for batch direct-to-blob media imports (audio/image).
-- ZIP path remains supported for backward compatibility.

ALTER TABLE alchimalia_schema."StoryAudioImportJobs"
    ADD COLUMN IF NOT EXISTS "StagingPrefix" VARCHAR(512) NULL,
    ADD COLUMN IF NOT EXISTS "BatchMappingJson" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryAudioImportJobs"
    ALTER COLUMN "ZipBlobPath" DROP NOT NULL;

ALTER TABLE alchimalia_schema."StoryImageImportJobs"
    ADD COLUMN IF NOT EXISTS "StagingPrefix" VARCHAR(512) NULL,
    ADD COLUMN IF NOT EXISTS "BatchMappingJson" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryImageImportJobs"
    ALTER COLUMN "ZipBlobPath" DROP NOT NULL;
