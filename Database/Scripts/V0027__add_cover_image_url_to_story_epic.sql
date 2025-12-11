-- Migration: Add CoverImageUrl column to StoryEpics table
-- This column stores the background image URL for the Tree Logic view
-- Date: 2025-12-11

BEGIN;

-- Add CoverImageUrl column to StoryEpics table (nullable, as it's optional)
ALTER TABLE "alchimalia_schema"."StoryEpics" 
ADD COLUMN IF NOT EXISTS "CoverImageUrl" text NULL;

COMMIT;

