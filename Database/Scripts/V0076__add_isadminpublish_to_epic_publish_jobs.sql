-- Add IsAdminPublish column to EpicPublishJobs table
-- This allows tracking whether a publish job was initiated by an admin,
-- which enables publishing from draft/changes_requested status
-- Run date: 2025-01-XX

BEGIN;

ALTER TABLE alchimalia_schema."EpicPublishJobs"
ADD COLUMN IF NOT EXISTS "IsAdminPublish" boolean NOT NULL DEFAULT false;

COMMENT ON COLUMN alchimalia_schema."EpicPublishJobs"."IsAdminPublish" IS 'True if this publish job was initiated by an admin, allowing publish from draft/changes_requested status';

COMMIT;
