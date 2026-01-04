-- Adds EpicExportJobs table for background epic export processing
-- This table tracks export jobs for both draft and published epics

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicExportJobs"
(
    "Id" UUID PRIMARY KEY,
    "EpicId" VARCHAR(256) NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(256) NOT NULL,
    "Locale" VARCHAR(16) NOT NULL DEFAULT 'ro-ro',
    "IsDraft" BOOLEAN NOT NULL DEFAULT FALSE,
    "Status" VARCHAR(32) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    
    -- Export options
    "IncludeStories" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeHeroes" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeRegions" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeImages" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeAudio" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeVideo" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeProgress" BOOLEAN NOT NULL DEFAULT FALSE,
    "LanguageFilter" VARCHAR(16) NULL,
    
    -- Progress tracking
    "CurrentPhase" VARCHAR(32) NULL,
    "StoriesExported" INTEGER NOT NULL DEFAULT 0,
    "TotalStories" INTEGER NOT NULL DEFAULT 0,
    "HeroesExported" INTEGER NOT NULL DEFAULT 0,
    "TotalHeroes" INTEGER NOT NULL DEFAULT 0,
    "RegionsExported" INTEGER NOT NULL DEFAULT 0,
    "TotalRegions" INTEGER NOT NULL DEFAULT 0,
    "AssetsExported" INTEGER NOT NULL DEFAULT 0,
    "TotalAssets" INTEGER NOT NULL DEFAULT 0,
    
    -- Results
    "ErrorMessage" TEXT NULL,
    "ZipBlobPath" VARCHAR(512) NULL,
    "ZipFileName" VARCHAR(256) NULL,
    "ZipSizeBytes" BIGINT NULL
);

-- Indexes for efficient querying
CREATE INDEX IF NOT EXISTS "IX_EpicExportJobs_EpicId_Status" ON alchimalia_schema."EpicExportJobs" ("EpicId", "Status");
CREATE INDEX IF NOT EXISTS "IX_EpicExportJobs_QueuedAtUtc" ON alchimalia_schema."EpicExportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_EpicExportJobs_OwnerUserId" ON alchimalia_schema."EpicExportJobs" ("OwnerUserId");

COMMENT ON TABLE alchimalia_schema."EpicExportJobs" IS 'Tracks background export jobs for epic ZIP generation';
COMMENT ON COLUMN alchimalia_schema."EpicExportJobs"."Status" IS 'Job status: Queued, Preparing, PackagingStories, PackagingHeroes, PackagingRegions, Finalizing, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."EpicExportJobs"."ZipBlobPath" IS 'Path to the generated ZIP file in blob storage';
COMMENT ON COLUMN alchimalia_schema."EpicExportJobs"."IsDraft" IS 'True if exporting draft epic, false if exporting published epic';

