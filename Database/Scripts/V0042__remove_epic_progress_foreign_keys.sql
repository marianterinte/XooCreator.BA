-- Remove foreign key constraints from EpicProgress and EpicStoryProgress
-- This script is kept for backward compatibility if FK constraints exist from older migrations
-- Note: V0047 (formerly V0032) no longer creates these FK constraints, so this script is only needed for existing databases
-- Run date: 2025-12-XX
-- Description: EpicProgress and EpicStoryProgress EpicId can reference either StoryEpics (old) or StoryEpicDefinition (new)

BEGIN;

-- Drop foreign key constraint from EpicProgress (if exists from older migration)
ALTER TABLE IF EXISTS alchimalia_schema."EpicProgress"
    DROP CONSTRAINT IF EXISTS "FK_EpicProgress_StoryEpics_EpicId";

-- Drop foreign key constraint from EpicStoryProgress (if exists from older migration)
ALTER TABLE IF EXISTS alchimalia_schema."EpicStoryProgress"
    DROP CONSTRAINT IF EXISTS "FK_EpicStoryProgress_StoryEpics_EpicId";

COMMIT;

