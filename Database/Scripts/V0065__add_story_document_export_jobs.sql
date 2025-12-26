-- Migration: Add StoryDocumentExportJobs table for background PDF/DOCX generation
-- This table tracks document export jobs for both draft and published stories.

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDocumentExportJobs"
(
    "Id" UUID PRIMARY KEY,
    "StoryId" VARCHAR(255) NOT NULL,

    -- Story owner (creator) and requestor (who triggered the export)
    "StoryOwnerUserId" UUID NOT NULL,
    "RequestedByUserId" UUID NOT NULL,
    "RequestedByEmail" VARCHAR(255) NOT NULL,

    "Locale" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "IsDraft" BOOLEAN NOT NULL DEFAULT FALSE,

    -- Output format and rendering options
    "Format" VARCHAR(10) NOT NULL DEFAULT 'pdf', -- pdf|docx
    "PaperSize" VARCHAR(10) NOT NULL DEFAULT 'A4', -- A4|Letter
    "IncludeCover" BOOLEAN NOT NULL DEFAULT TRUE,
    "IncludeQuizAnswers" BOOLEAN NOT NULL DEFAULT FALSE,

    "Status" VARCHAR(50) NOT NULL DEFAULT 'Queued',
    "DequeueCount" INTEGER NOT NULL DEFAULT 0,
    "QueuedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "StartedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "CompletedAtUtc" TIMESTAMP WITH TIME ZONE NULL,
    "ErrorMessage" TEXT NULL,

    -- Output location (stored in blob storage; SAS generated at status endpoint)
    "OutputBlobPath" TEXT NULL,
    "OutputFileName" VARCHAR(255) NULL,
    "OutputSizeBytes" BIGINT NULL
);

-- Indexes for efficient querying
CREATE INDEX IF NOT EXISTS "IX_StoryDocumentExportJobs_StoryId_Status" ON alchimalia_schema."StoryDocumentExportJobs" ("StoryId", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryDocumentExportJobs_QueuedAtUtc" ON alchimalia_schema."StoryDocumentExportJobs" ("QueuedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_StoryDocumentExportJobs_RequestedByUserId" ON alchimalia_schema."StoryDocumentExportJobs" ("RequestedByUserId");
CREATE INDEX IF NOT EXISTS "IX_StoryDocumentExportJobs_StoryOwnerUserId" ON alchimalia_schema."StoryDocumentExportJobs" ("StoryOwnerUserId");

COMMENT ON TABLE alchimalia_schema."StoryDocumentExportJobs" IS 'Tracks background document export jobs for story PDF/DOCX generation';
COMMENT ON COLUMN alchimalia_schema."StoryDocumentExportJobs"."Status" IS 'Job status: Queued, Running, Completed, Failed';
COMMENT ON COLUMN alchimalia_schema."StoryDocumentExportJobs"."OutputBlobPath" IS 'Path to the generated document in blob storage';
COMMENT ON COLUMN alchimalia_schema."StoryDocumentExportJobs"."IsDraft" IS 'True if exporting draft story, false if exporting published story';

