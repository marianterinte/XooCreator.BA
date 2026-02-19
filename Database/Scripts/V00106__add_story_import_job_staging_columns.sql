-- Adds StagingPrefix and ManifestBlobPath for client-side full story import (assets uploaded to staging, no ZIP on server).
-- When set, worker reads manifest and assets from these paths instead of ZipBlobPath.

ALTER TABLE alchimalia_schema."StoryImportJobs"
    ADD COLUMN IF NOT EXISTS "StagingPrefix" VARCHAR(512) NULL,
    ADD COLUMN IF NOT EXISTS "ManifestBlobPath" VARCHAR(512) NULL;

-- ZipBlobPath can be null when using staging (assets-from-browser flow).
ALTER TABLE alchimalia_schema."StoryImportJobs"
    ALTER COLUMN "ZipBlobPath" DROP NOT NULL;
