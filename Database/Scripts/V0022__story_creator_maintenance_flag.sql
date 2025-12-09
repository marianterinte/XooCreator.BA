-- Adds a global platform setting used to disable Story Creator access for all creators.
-- Safe to run multiple times (idempotent).

CREATE TABLE IF NOT EXISTS alchimalia_schema."PlatformSettings" (
    "Key" text PRIMARY KEY,
    "BoolValue" boolean NOT NULL DEFAULT false,
    "StringValue" text NULL,
    "UpdatedAt" timestamptz NOT NULL DEFAULT NOW(),
    "UpdatedBy" text NULL
);

-- Seed default value (disabled = false)
INSERT INTO alchimalia_schema."PlatformSettings" ("Key", "BoolValue", "StringValue")
VALUES ('story-creator-disabled', false, NULL)
ON CONFLICT ("Key") DO NOTHING;

