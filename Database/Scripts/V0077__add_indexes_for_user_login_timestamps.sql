-- Migration: Add indexes on user login and creation timestamps
-- Run date: 2026-01-21
-- Description: Adds performance indexes on LastLoginAt and CreatedAt columns in AlchimaliaUsers table
--              to improve query performance when sorting/filtering users by activity or registration date.
--              These indexes support Admin Dashboard user management features.

BEGIN;

-- Add index on LastLoginAt (DESC order for "most recent login" queries)
CREATE INDEX IF NOT EXISTS "IX_AlchimaliaUsers_LastLoginAt"
    ON alchimalia_schema."AlchimaliaUsers" ("LastLoginAt" DESC);

COMMENT ON INDEX alchimalia_schema."IX_AlchimaliaUsers_LastLoginAt" IS 
    'Improves performance for queries sorting/filtering users by last login date (most recent first)';

-- Add index on CreatedAt (DESC order for "newest users" queries)
CREATE INDEX IF NOT EXISTS "IX_AlchimaliaUsers_CreatedAt"
    ON alchimalia_schema."AlchimaliaUsers" ("CreatedAt" DESC);

COMMENT ON INDEX alchimalia_schema."IX_AlchimaliaUsers_CreatedAt" IS 
    'Improves performance for queries sorting/filtering users by registration date (newest first)';

COMMIT;
