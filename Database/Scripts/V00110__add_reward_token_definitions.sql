-- Migration: Add RewardTokenDefinitions table for admin-configurable reward tokens
-- Used by Story Creator to populate comboboxes when adding tokens to quiz/dialog answers

CREATE TABLE IF NOT EXISTS alchimalia_schema."RewardTokenDefinitions" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "Type" character varying(64) NOT NULL,
    "Value" character varying(128) NOT NULL,
    "DisplayNameKey" character varying(128) NOT NULL,
    "Icon" character varying(32) NULL,
    "SortOrder" integer NOT NULL DEFAULT 0,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    CONSTRAINT "PK_RewardTokenDefinitions" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_RewardTokenDefinitions_Type_Value" 
    ON alchimalia_schema."RewardTokenDefinitions" ("Type", "Value");

-- Seed initial reward tokens from StoryTokenService
INSERT INTO alchimalia_schema."RewardTokenDefinitions" ("Id", "Type", "Value", "DisplayNameKey", "Icon", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (gen_random_uuid(), 'Personality', 'courage', 'token_courage', '🦁', 1, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Personality', 'curiosity', 'token_curiosity', '🔍', 2, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Personality', 'thinking', 'token_thinking', '🧠', 3, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Personality', 'creativity', 'token_creativity', '🎨', 4, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Personality', 'safety', 'token_safety', '🛡️', 5, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'discovery', 'token_discovery', '💎', 6, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'discovery credit', 'token_discovery_credit', '💎', 7, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'math_addition', 'token_math_addition', '➕', 8, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'math_subtraction', 'token_math_subtraction', '➖', 9, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'math_multiplication', 'token_math_multiplication', '✖️', 10, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'math_division', 'token_math_division', '➗', 11, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC'),
    (gen_random_uuid(), 'Discovery', 'math', 'token_math_addition', '➕', 12, true, now() AT TIME ZONE 'UTC', now() AT TIME ZONE 'UTC')
ON CONFLICT ("Type", "Value") DO NOTHING;
