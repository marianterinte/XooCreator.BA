-- Migration: Add index on Auth0Id for AlchimaliaUsers
-- Run date: 2025-01-XX
-- Description: Adds index on Auth0Id column to dramatically improve query performance for user lookups.
-- This fixes OperationCanceledException errors caused by slow queries (800ms-5s) during authentication.

BEGIN;

-- Add index on Auth0Id (most queries filter by this column)
CREATE INDEX IF NOT EXISTS "IX_AlchimaliaUsers_Auth0Id"
    ON alchimalia_schema."AlchimaliaUsers" ("Auth0Id");

-- Optional: Add unique constraint if Auth0Id should be unique per user
-- Note: Uncomment only if Auth0Id is guaranteed to be unique
-- ALTER TABLE alchimalia_schema."AlchimaliaUsers" 
-- ADD CONSTRAINT "UQ_AlchimaliaUsers_Auth0Id" UNIQUE ("Auth0Id");

COMMIT;

