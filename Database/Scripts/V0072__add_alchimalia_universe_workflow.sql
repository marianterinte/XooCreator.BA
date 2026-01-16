-- Add Alchimalia Universe Editor Workflow Support
-- Run date: 2025-01-16
-- Description: Adds workflow fields (status, versioning, review) to Animals, HeroDefinitions, and StoryHeroes
--              Creates versioning tables and StoryHeroTranslations table for multi-language support with audio

BEGIN;

-- ============================================================================
-- 1. Extend Animals table with workflow fields
-- ============================================================================

-- Add Status column (default: 'draft')
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'Status'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "Status" character varying(50) NOT NULL DEFAULT 'draft';
    END IF;
END $$;

-- Add CreatedByUserId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'CreatedByUserId'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "CreatedByUserId" uuid;
    END IF;
END $$;

-- Add ReviewedByUserId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'ReviewedByUserId'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "ReviewedByUserId" uuid;
    END IF;
END $$;

-- Add ReviewNotes column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'ReviewNotes'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "ReviewNotes" character varying(2000);
    END IF;
END $$;

-- Add Version column (default: 1)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'Version'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "Version" integer NOT NULL DEFAULT 1;
    END IF;
END $$;

-- Add ParentVersionId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'ParentVersionId'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "ParentVersionId" uuid;
    END IF;
END $$;

-- Add CreatedAt column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'CreatedAt'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "CreatedAt" timestamp with time zone;
    END IF;
END $$;

-- Add UpdatedAt column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'Animals' 
        AND column_name = 'UpdatedAt'
    ) THEN
        ALTER TABLE alchimalia_schema."Animals" 
        ADD COLUMN "UpdatedAt" timestamp with time zone;
    END IF;
END $$;

-- Create indexes on Animals
CREATE INDEX IF NOT EXISTS "IX_Animals_Status"
    ON alchimalia_schema."Animals" ("Status");

CREATE INDEX IF NOT EXISTS "IX_Animals_Id_Status"
    ON alchimalia_schema."Animals" ("Id", "Status");

-- ============================================================================
-- 2. Extend HeroDefinitions table with workflow fields
-- ============================================================================

-- Add Status column (default: 'draft')
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'Status'
    ) THEN
        ALTER TABLE alchimalia_schema."HeroDefinitions" 
        ADD COLUMN "Status" character varying(50) NOT NULL DEFAULT 'draft';
    END IF;
END $$;

-- Add CreatedByUserId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'CreatedByUserId'
    ) THEN
        ALTER TABLE alchimalia_schema."HeroDefinitions" 
        ADD COLUMN "CreatedByUserId" uuid;
    END IF;
END $$;

-- Add ReviewedByUserId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'ReviewedByUserId'
    ) THEN
        ALTER TABLE alchimalia_schema."HeroDefinitions" 
        ADD COLUMN "ReviewedByUserId" uuid;
    END IF;
END $$;

-- Add ReviewNotes column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'ReviewNotes'
    ) THEN
        ALTER TABLE alchimalia_schema."HeroDefinitions" 
        ADD COLUMN "ReviewNotes" character varying(2000);
    END IF;
END $$;

-- Add Version column (default: 1)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'Version'
    ) THEN
        ALTER TABLE alchimalia_schema."HeroDefinitions" 
        ADD COLUMN "Version" integer NOT NULL DEFAULT 1;
    END IF;
END $$;

-- Add ParentVersionId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'ParentVersionId'
    ) THEN
        ALTER TABLE alchimalia_schema."HeroDefinitions" 
        ADD COLUMN "ParentVersionId" character varying(100);
    END IF;
END $$;

-- Create indexes on HeroDefinitions
CREATE INDEX IF NOT EXISTS "IX_HeroDefinitions_Status"
    ON alchimalia_schema."HeroDefinitions" ("Status");

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitions_Id_Status"
    ON alchimalia_schema."HeroDefinitions" ("Id", "Status");

-- ============================================================================
-- 3. Extend StoryHeroes table with workflow fields
-- ============================================================================

-- Rename UnlockConditionJson to UnlockConditionsJson if needed
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'UnlockConditionJson'
    ) AND NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'UnlockConditionsJson'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryHeroes" 
        RENAME COLUMN "UnlockConditionJson" TO "UnlockConditionsJson";
    END IF;
END $$;

-- Add Status column (default: 'draft')
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'Status'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryHeroes" 
        ADD COLUMN "Status" character varying(50) NOT NULL DEFAULT 'draft';
    END IF;
END $$;

-- Add CreatedByUserId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'CreatedByUserId'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryHeroes" 
        ADD COLUMN "CreatedByUserId" uuid;
    END IF;
END $$;

-- Add ReviewedByUserId column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'ReviewedByUserId'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryHeroes" 
        ADD COLUMN "ReviewedByUserId" uuid;
    END IF;
END $$;

-- Add ReviewNotes column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'ReviewNotes'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryHeroes" 
        ADD COLUMN "ReviewNotes" character varying(2000);
    END IF;
END $$;

-- Add Version column (default: 1)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'StoryHeroes' 
        AND column_name = 'Version'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryHeroes" 
        ADD COLUMN "Version" integer NOT NULL DEFAULT 1;
    END IF;
END $$;

-- Create indexes on StoryHeroes
CREATE INDEX IF NOT EXISTS "IX_StoryHeroes_Status"
    ON alchimalia_schema."StoryHeroes" ("Status");

CREATE INDEX IF NOT EXISTS "IX_StoryHeroes_Id_Status"
    ON alchimalia_schema."StoryHeroes" ("Id", "Status");

-- ============================================================================
-- 4. Create StoryHeroTranslations table (if not exists)
-- ============================================================================

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryHeroTranslations"
(
    "Id" uuid NOT NULL,
    "StoryHeroId" uuid NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Description" text,
    "GreetingText" text,
    "GreetingAudioUrl" character varying(500),
    CONSTRAINT "PK_StoryHeroTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryHeroTranslations_StoryHeroes_StoryHeroId" 
        FOREIGN KEY ("StoryHeroId") REFERENCES alchimalia_schema."StoryHeroes" ("Id") ON DELETE CASCADE
);

-- Create unique index on (StoryHeroId, LanguageCode)
CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryHeroTranslations_StoryHeroId_LanguageCode"
    ON alchimalia_schema."StoryHeroTranslations" ("StoryHeroId", "LanguageCode");

-- ============================================================================
-- 5. Create AnimalVersions table
-- ============================================================================

CREATE TABLE IF NOT EXISTS alchimalia_schema."AnimalVersions"
(
    "Id" uuid NOT NULL,
    "AnimalId" uuid NOT NULL,
    "Version" integer NOT NULL,
    "Status" character varying(50) NOT NULL,
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "SnapshotJson" text,
    CONSTRAINT "PK_AnimalVersions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AnimalVersions_Animals_AnimalId" 
        FOREIGN KEY ("AnimalId") REFERENCES alchimalia_schema."Animals" ("Id") ON DELETE CASCADE
);

-- Create unique index on (AnimalId, Version)
CREATE UNIQUE INDEX IF NOT EXISTS "IX_AnimalVersions_AnimalId_Version"
    ON alchimalia_schema."AnimalVersions" ("AnimalId", "Version");

-- ============================================================================
-- 6. Create HeroDefinitionVersions table
-- ============================================================================

CREATE TABLE IF NOT EXISTS alchimalia_schema."HeroDefinitionVersions"
(
    "Id" uuid NOT NULL,
    "HeroDefinitionId" character varying(100) NOT NULL,
    "Version" integer NOT NULL,
    "Status" character varying(50) NOT NULL,
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "SnapshotJson" text,
    CONSTRAINT "PK_HeroDefinitionVersions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HeroDefinitionVersions_HeroDefinitions_HeroDefinitionId" 
        FOREIGN KEY ("HeroDefinitionId") REFERENCES alchimalia_schema."HeroDefinitions" ("Id") ON DELETE CASCADE
);

-- Create unique index on (HeroDefinitionId, Version)
CREATE UNIQUE INDEX IF NOT EXISTS "IX_HeroDefinitionVersions_HeroDefinitionId_Version"
    ON alchimalia_schema."HeroDefinitionVersions" ("HeroDefinitionId", "Version");

-- ============================================================================
-- 7. Create StoryHeroVersions table
-- ============================================================================

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryHeroVersions"
(
    "Id" uuid NOT NULL,
    "StoryHeroId" uuid NOT NULL,
    "Version" integer NOT NULL,
    "Status" character varying(50) NOT NULL,
    "CreatedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "SnapshotJson" text,
    CONSTRAINT "PK_StoryHeroVersions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryHeroVersions_StoryHeroes_StoryHeroId" 
        FOREIGN KEY ("StoryHeroId") REFERENCES alchimalia_schema."StoryHeroes" ("Id") ON DELETE CASCADE
);

-- Create unique index on (StoryHeroId, Version)
CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryHeroVersions_StoryHeroId_Version"
    ON alchimalia_schema."StoryHeroVersions" ("StoryHeroId", "Version");

COMMIT;
