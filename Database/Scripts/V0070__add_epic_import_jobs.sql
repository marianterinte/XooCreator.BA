-- Adds EpicImportJobs table for background epic import processing
-- This table tracks import jobs for epic ZIP files

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicImportJobs"
(
    "Id" UUID PRIMARY KEY,
    "EpicId" VARCHAR(256) NOT NULL,
    "OriginalEpicId" VARCHAR(256) NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(256) NOT NULL,
    "Locale" VARCHAR(16) NOT NULL DEFAULT 'ro-ro',
    "ZipBlobPath" VARCHAR(512) NOT NULL,
    "ZipFileName" VARCHAR(256) NOT NULL,
    "ZipSizeBytes" BIGINT NOT NULL DEFAULT 0,
    "Status" VARCHAR(32) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    
    -- Import options
    "ConflictStrategy" VARCHAR(16) NOT NULL DEFAULT 'skip',
    "IncludeStories" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeHeroes" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeRegions" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeImages" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeAudio" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeVideo" BOOLEAN NOT NULL DEFAULT TRUE,
    "GenerateNewIds" BOOLEAN NOT NULL DEFAULT FALSE,
    "IdPrefix" VARCHAR(32) NULL,
    
    -- Progress tracking (stored as JSONB)
    "PhasesJson" JSONB NULL,
    "IdMappingsJson" JSONB NULL,
    
    -- Results
    "ErrorMessage" TEXT NULL,
    "WarningsJson" JSONB NULL
);

-- Indexes for efficient querying
CREATE INDEX IF NOT EXISTS "IX_EpicImportJobs_EpicId_Status" ON alchimalia_schema."EpicImportJobs" ("EpicId", "Status");
CREATE INDEX IF NOT EXISTS "IX_EpicImportJobs_QueuedAtUtc" ON alchimalia_schema."EpicImportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_EpicImportJobs_OwnerUserId" ON alchimalia_schema."EpicImportJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."EpicImportJobs" IS 'Tracks background import jobs for epic ZIP files';
COMMENT ON COLUMN alchimalia_schema."EpicImportJobs"."Status" IS 'Job status: Queued, Analyzing, ImportingRegions, ImportingHeroes, ImportingStories, EstablishingRelationships, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."EpicImportJobs"."ConflictStrategy" IS 'Conflict resolution strategy: skip, replace, new_ids';
COMMENT ON COLUMN alchimalia_schema."EpicImportJobs"."PhasesJson" IS 'JSONB structure tracking import phases and progress';
COMMENT ON COLUMN alchimalia_schema."EpicImportJobs"."IdMappingsJson" IS 'JSONB structure mapping original IDs to new IDs (if GenerateNewIds=true)';
COMMENT ON COLUMN alchimalia_schema."EpicImportJobs"."WarningsJson" IS 'JSONB array of warning messages during import';

