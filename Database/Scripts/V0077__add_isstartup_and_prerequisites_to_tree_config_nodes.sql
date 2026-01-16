-- Migration: Add IsStartup and PrerequisitesJson to Tree Config Nodes
-- Description: Adds support for startup nodes and prerequisites (linked heroes) in tree configurations

-- Add columns to TreeOfHeroesConfigCraftNodes
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'TreeOfHeroesConfigCraftNodes' 
        AND column_name = 'IsStartup'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftNodes"
            ADD COLUMN "IsStartup" BOOLEAN NOT NULL DEFAULT FALSE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'TreeOfHeroesConfigCraftNodes' 
        AND column_name = 'PrerequisitesJson'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftNodes"
            ADD COLUMN "PrerequisitesJson" TEXT NOT NULL DEFAULT '[]';
    END IF;
END $$;

-- Add columns to TreeOfHeroesConfigDefinitionNodes
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'TreeOfHeroesConfigDefinitionNodes' 
        AND column_name = 'IsStartup'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionNodes"
            ADD COLUMN "IsStartup" BOOLEAN NOT NULL DEFAULT FALSE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'TreeOfHeroesConfigDefinitionNodes' 
        AND column_name = 'PrerequisitesJson'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionNodes"
            ADD COLUMN "PrerequisitesJson" TEXT NOT NULL DEFAULT '[]';
    END IF;
END $$;
