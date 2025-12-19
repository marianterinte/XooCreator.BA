-- V0062: Add IsActive column to StoryEpicDefinitions for soft delete / unpublish
-- Run date: 2025-12-17
-- Description: Adds IsActive flag to support unpublishing story epics while preserving player progress

BEGIN;

-- Add IsActive to StoryEpicDefinitions
ALTER TABLE alchimalia_schema."StoryEpicDefinitions"
ADD COLUMN "IsActive" boolean NOT NULL DEFAULT true;

-- Create index for IsActive queries (for filtering published/active epics)
CREATE INDEX "IX_StoryEpicDefinitions_IsActive" 
    ON alchimalia_schema."StoryEpicDefinitions" ("IsActive");

-- Create composite index for Status + IsActive (common query pattern)
CREATE INDEX "IX_StoryEpicDefinitions_Status_IsActive" 
    ON alchimalia_schema."StoryEpicDefinitions" ("Status", "IsActive");

COMMIT;

