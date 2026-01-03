-- Migration: Add RowVersion columns for optimistic concurrency control
-- Date: 2024-01-03
-- Purpose: Prevent lost updates when multiple users edit the same entity concurrently

-- Enable pgcrypto extension if not already enabled (required for gen_random_bytes)
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Add RowVersion to StoryCraft table
ALTER TABLE "alchimalia_schema"."StoryCrafts"
ADD COLUMN IF NOT EXISTS "RowVersion" bytea;

-- Add RowVersion to StoryEpicCraft table
ALTER TABLE "alchimalia_schema"."StoryEpicCrafts"
ADD COLUMN IF NOT EXISTS "RowVersion" bytea;

-- Add RowVersion to StoryRegionCraft table
ALTER TABLE "alchimalia_schema"."StoryRegionCrafts"
ADD COLUMN IF NOT EXISTS "RowVersion" bytea;

-- Add RowVersion to EpicHeroCraft table
ALTER TABLE "alchimalia_schema"."EpicHeroCrafts"
ADD COLUMN IF NOT EXISTS "RowVersion" bytea;

-- Create function to update RowVersion on row update
CREATE OR REPLACE FUNCTION "alchimalia_schema".update_row_version()
RETURNS TRIGGER AS $$
BEGIN
    NEW."RowVersion" = gen_random_bytes(8);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create triggers for StoryCraft
DROP TRIGGER IF EXISTS update_storycrafts_rowversion ON "alchimalia_schema"."StoryCrafts";
CREATE TRIGGER update_storycrafts_rowversion
BEFORE INSERT OR UPDATE ON "alchimalia_schema"."StoryCrafts"
FOR EACH ROW
EXECUTE FUNCTION "alchimalia_schema".update_row_version();

-- Create triggers for StoryEpicCraft
DROP TRIGGER IF EXISTS update_storyepiccrafts_rowversion ON "alchimalia_schema"."StoryEpicCrafts";
CREATE TRIGGER update_storyepiccrafts_rowversion
BEFORE INSERT OR UPDATE ON "alchimalia_schema"."StoryEpicCrafts"
FOR EACH ROW
EXECUTE FUNCTION "alchimalia_schema".update_row_version();

-- Create triggers for StoryRegionCraft
DROP TRIGGER IF EXISTS update_storyregioncrafts_rowversion ON "alchimalia_schema"."StoryRegionCrafts";
CREATE TRIGGER update_storyregioncrafts_rowversion
BEFORE INSERT OR UPDATE ON "alchimalia_schema"."StoryRegionCrafts"
FOR EACH ROW
EXECUTE FUNCTION "alchimalia_schema".update_row_version();

-- Create triggers for EpicHeroCraft
DROP TRIGGER IF EXISTS update_epicherocrafts_rowversion ON "alchimalia_schema"."EpicHeroCrafts";
CREATE TRIGGER update_epicherocrafts_rowversion
BEFORE INSERT OR UPDATE ON "alchimalia_schema"."EpicHeroCrafts"
FOR EACH ROW
EXECUTE FUNCTION "alchimalia_schema".update_row_version();

-- Update existing rows with initial RowVersion values
UPDATE "alchimalia_schema"."StoryCrafts" SET "RowVersion" = gen_random_bytes(8) WHERE "RowVersion" IS NULL;
UPDATE "alchimalia_schema"."StoryEpicCrafts" SET "RowVersion" = gen_random_bytes(8) WHERE "RowVersion" IS NULL;
UPDATE "alchimalia_schema"."StoryRegionCrafts" SET "RowVersion" = gen_random_bytes(8) WHERE "RowVersion" IS NULL;
UPDATE "alchimalia_schema"."EpicHeroCrafts" SET "RowVersion" = gen_random_bytes(8) WHERE "RowVersion" IS NULL;

