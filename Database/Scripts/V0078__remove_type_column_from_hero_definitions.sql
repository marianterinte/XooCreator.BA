-- Remove Type column from Hero Definition tables
-- The Type field (hero/seed) is no longer needed
-- Run date: 2026-01-16

BEGIN;

-- Remove Type column from HeroDefinitionCrafts
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitionCrafts' 
        AND column_name = 'Type'
    ) THEN
        ALTER TABLE "alchimalia_schema"."HeroDefinitionCrafts"
            DROP COLUMN "Type";
    END IF;
END $$;

-- Remove Type column from HeroDefinitionDefinitions
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitionDefinitions' 
        AND column_name = 'Type'
    ) THEN
        ALTER TABLE "alchimalia_schema"."HeroDefinitionDefinitions"
            DROP COLUMN "Type";
    END IF;
END $$;

-- Remove Type column from HeroDefinitions (legacy table)
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'HeroDefinitions' 
        AND column_name = 'Type'
    ) THEN
        ALTER TABLE "alchimalia_schema"."HeroDefinitions"
            DROP COLUMN "Type";
    END IF;
END $$;

COMMIT;
