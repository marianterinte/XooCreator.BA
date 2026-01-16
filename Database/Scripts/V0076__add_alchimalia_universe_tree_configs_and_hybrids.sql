-- Add Tree of Heroes configurations and LOI hybrid composition
-- Idempotent: creates tables first, then adds constraints separately
BEGIN;

-- Add audio per language (optional)
ALTER TABLE "alchimalia_schema"."HeroDefinitionDefinitionTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" character varying(500);
ALTER TABLE "alchimalia_schema"."HeroDefinitionCraftTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" character varying(500);
ALTER TABLE "alchimalia_schema"."HeroDefinitionTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" character varying(500);
ALTER TABLE "alchimalia_schema"."AnimalDefinitionTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" character varying(500);
ALTER TABLE "alchimalia_schema"."AnimalCraftTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" character varying(500);

-- =========================
-- Tree of Heroes Configs (Definition)
-- =========================
-- Create table without foreign key constraints first
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."TreeOfHeroesConfigDefinitions" (
    "Id" uuid NOT NULL,
    "Label" character varying(255) NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'published',
    "PublishedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "PublishedAtUtc" timestamp with time zone,
    CONSTRAINT "PK_TreeOfHeroesConfigDefinitions" PRIMARY KEY ("Id")
);

-- Add foreign key constraints separately (idempotent)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigDefinitions'
        AND c.conname = 'FK_TreeOfHeroesConfigDefinitions_AlchimaliaUsers_PublishedByUserId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitions"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigDefinitions_AlchimaliaUsers_PublishedByUserId"
            FOREIGN KEY ("PublishedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS "IX_TreeOfHeroesConfigDefinitions_Status"
    ON "alchimalia_schema"."TreeOfHeroesConfigDefinitions" ("Status");

-- Create nodes table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."TreeOfHeroesConfigDefinitionNodes" (
    "Id" uuid NOT NULL,
    "ConfigDefinitionId" uuid NOT NULL,
    "HeroDefinitionId" character varying(100) NOT NULL,
    "PositionX" double precision NOT NULL DEFAULT 0,
    "PositionY" double precision NOT NULL DEFAULT 0,
    "CourageCost" integer NOT NULL DEFAULT 0,
    "CuriosityCost" integer NOT NULL DEFAULT 0,
    "ThinkingCost" integer NOT NULL DEFAULT 0,
    "CreativityCost" integer NOT NULL DEFAULT 0,
    "SafetyCost" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TreeOfHeroesConfigDefinitionNodes" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for nodes
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigDefinitionNodes'
        AND c.conname = 'FK_TreeOfHeroesConfigDefinitionNodes_ConfigDefinitions_ConfigDefinitionId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionNodes"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigDefinitionNodes_ConfigDefinitions_ConfigDefinitionId"
            FOREIGN KEY ("ConfigDefinitionId") REFERENCES "alchimalia_schema"."TreeOfHeroesConfigDefinitions" ("Id") ON DELETE CASCADE;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigDefinitionNodes'
        AND c.conname = 'FK_TreeOfHeroesConfigDefinitionNodes_HeroDefinitionDefinitions_HeroDefinitionId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionNodes"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigDefinitionNodes_HeroDefinitionDefinitions_HeroDefinitionId"
            FOREIGN KEY ("HeroDefinitionId") REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_TreeOfHeroesConfigDefinitionNodes_Config_Hero"
    ON "alchimalia_schema"."TreeOfHeroesConfigDefinitionNodes" ("ConfigDefinitionId", "HeroDefinitionId");

-- Create edges table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."TreeOfHeroesConfigDefinitionEdges" (
    "Id" uuid NOT NULL,
    "ConfigDefinitionId" uuid NOT NULL,
    "FromHeroId" character varying(100) NOT NULL,
    "ToHeroId" character varying(100) NOT NULL,
    CONSTRAINT "PK_TreeOfHeroesConfigDefinitionEdges" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for edges
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigDefinitionEdges'
        AND c.conname = 'FK_TreeOfHeroesConfigDefinitionEdges_ConfigDefinitions_ConfigDefinitionId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionEdges"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigDefinitionEdges_ConfigDefinitions_ConfigDefinitionId"
            FOREIGN KEY ("ConfigDefinitionId") REFERENCES "alchimalia_schema"."TreeOfHeroesConfigDefinitions" ("Id") ON DELETE CASCADE;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigDefinitionEdges'
        AND c.conname = 'FK_TreeOfHeroesConfigDefinitionEdges_HeroDefinitionDefinitions_FromHeroId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionEdges"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigDefinitionEdges_HeroDefinitionDefinitions_FromHeroId"
            FOREIGN KEY ("FromHeroId") REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigDefinitionEdges'
        AND c.conname = 'FK_TreeOfHeroesConfigDefinitionEdges_HeroDefinitionDefinitions_ToHeroId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigDefinitionEdges"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigDefinitionEdges_HeroDefinitionDefinitions_ToHeroId"
            FOREIGN KEY ("ToHeroId") REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_TreeOfHeroesConfigDefinitionEdges_Config_From_To"
    ON "alchimalia_schema"."TreeOfHeroesConfigDefinitionEdges" ("ConfigDefinitionId", "FromHeroId", "ToHeroId");

-- =========================
-- Tree of Heroes Configs (Craft)
-- =========================
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."TreeOfHeroesConfigCrafts" (
    "Id" uuid NOT NULL,
    "PublishedDefinitionId" uuid,
    "Label" character varying(255) NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'draft',
    "CreatedByUserId" uuid,
    "ReviewedByUserId" uuid,
    "ReviewNotes" text,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_TreeOfHeroesConfigCrafts" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for crafts
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCrafts'
        AND c.conname = 'FK_TreeOfHeroesConfigCrafts_ConfigDefinitions_PublishedDefinitionId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCrafts"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCrafts_ConfigDefinitions_PublishedDefinitionId"
            FOREIGN KEY ("PublishedDefinitionId") REFERENCES "alchimalia_schema"."TreeOfHeroesConfigDefinitions" ("Id") ON DELETE SET NULL;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCrafts'
        AND c.conname = 'FK_TreeOfHeroesConfigCrafts_AlchimaliaUsers_CreatedByUserId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCrafts"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCrafts_AlchimaliaUsers_CreatedByUserId"
            FOREIGN KEY ("CreatedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCrafts'
        AND c.conname = 'FK_TreeOfHeroesConfigCrafts_AlchimaliaUsers_ReviewedByUserId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCrafts"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCrafts_AlchimaliaUsers_ReviewedByUserId"
            FOREIGN KEY ("ReviewedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS "IX_TreeOfHeroesConfigCrafts_Status"
    ON "alchimalia_schema"."TreeOfHeroesConfigCrafts" ("Status");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."TreeOfHeroesConfigCraftNodes" (
    "Id" uuid NOT NULL,
    "ConfigCraftId" uuid NOT NULL,
    "HeroDefinitionId" character varying(100) NOT NULL,
    "PositionX" double precision NOT NULL DEFAULT 0,
    "PositionY" double precision NOT NULL DEFAULT 0,
    "CourageCost" integer NOT NULL DEFAULT 0,
    "CuriosityCost" integer NOT NULL DEFAULT 0,
    "ThinkingCost" integer NOT NULL DEFAULT 0,
    "CreativityCost" integer NOT NULL DEFAULT 0,
    "SafetyCost" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TreeOfHeroesConfigCraftNodes" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for craft nodes
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCraftNodes'
        AND c.conname = 'FK_TreeOfHeroesConfigCraftNodes_ConfigCrafts_ConfigCraftId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftNodes"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCraftNodes_ConfigCrafts_ConfigCraftId"
            FOREIGN KEY ("ConfigCraftId") REFERENCES "alchimalia_schema"."TreeOfHeroesConfigCrafts" ("Id") ON DELETE CASCADE;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCraftNodes'
        AND c.conname = 'FK_TreeOfHeroesConfigCraftNodes_HeroDefinitionDefinitions_HeroDefinitionId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftNodes"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCraftNodes_HeroDefinitionDefinitions_HeroDefinitionId"
            FOREIGN KEY ("HeroDefinitionId") REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_TreeOfHeroesConfigCraftNodes_Config_Hero"
    ON "alchimalia_schema"."TreeOfHeroesConfigCraftNodes" ("ConfigCraftId", "HeroDefinitionId");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."TreeOfHeroesConfigCraftEdges" (
    "Id" uuid NOT NULL,
    "ConfigCraftId" uuid NOT NULL,
    "FromHeroId" character varying(100) NOT NULL,
    "ToHeroId" character varying(100) NOT NULL,
    CONSTRAINT "PK_TreeOfHeroesConfigCraftEdges" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for craft edges
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCraftEdges'
        AND c.conname = 'FK_TreeOfHeroesConfigCraftEdges_ConfigCrafts_ConfigCraftId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftEdges"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCraftEdges_ConfigCrafts_ConfigCraftId"
            FOREIGN KEY ("ConfigCraftId") REFERENCES "alchimalia_schema"."TreeOfHeroesConfigCrafts" ("Id") ON DELETE CASCADE;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCraftEdges'
        AND c.conname = 'FK_TreeOfHeroesConfigCraftEdges_HeroDefinitionDefinitions_FromHeroId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftEdges"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCraftEdges_HeroDefinitionDefinitions_FromHeroId"
            FOREIGN KEY ("FromHeroId") REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'TreeOfHeroesConfigCraftEdges'
        AND c.conname = 'FK_TreeOfHeroesConfigCraftEdges_HeroDefinitionDefinitions_ToHeroId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."TreeOfHeroesConfigCraftEdges"
            ADD CONSTRAINT "FK_TreeOfHeroesConfigCraftEdges_HeroDefinitionDefinitions_ToHeroId"
            FOREIGN KEY ("ToHeroId") REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_TreeOfHeroesConfigCraftEdges_Config_From_To"
    ON "alchimalia_schema"."TreeOfHeroesConfigCraftEdges" ("ConfigCraftId", "FromHeroId", "ToHeroId");

-- =========================
-- LOI Hybrid Composition
-- =========================
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalHybridDefinitionParts" (
    "Id" uuid NOT NULL,
    "AnimalDefinitionId" uuid NOT NULL,
    "SourceAnimalId" uuid NOT NULL,
    "BodyPartKey" character varying(50) NOT NULL,
    "OrderIndex" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_AnimalHybridDefinitionParts" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for hybrid definition parts
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'AnimalHybridDefinitionParts'
        AND c.conname = 'FK_AnimalHybridDefinitionParts_AnimalDefinitions_AnimalDefinitionId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."AnimalHybridDefinitionParts"
            ADD CONSTRAINT "FK_AnimalHybridDefinitionParts_AnimalDefinitions_AnimalDefinitionId"
            FOREIGN KEY ("AnimalDefinitionId") REFERENCES "alchimalia_schema"."AnimalDefinitions" ("Id") ON DELETE CASCADE;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'AnimalHybridDefinitionParts'
        AND c.conname = 'FK_AnimalHybridDefinitionParts_SourceAnimals_SourceAnimalId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."AnimalHybridDefinitionParts"
            ADD CONSTRAINT "FK_AnimalHybridDefinitionParts_SourceAnimals_SourceAnimalId"
            FOREIGN KEY ("SourceAnimalId") REFERENCES "alchimalia_schema"."AnimalDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'AnimalHybridDefinitionParts'
        AND c.conname = 'FK_AnimalHybridDefinitionParts_BodyParts_BodyPartKey'
    ) THEN
        ALTER TABLE "alchimalia_schema"."AnimalHybridDefinitionParts"
            ADD CONSTRAINT "FK_AnimalHybridDefinitionParts_BodyParts_BodyPartKey"
            FOREIGN KEY ("BodyPartKey") REFERENCES "alchimalia_schema"."BodyParts" ("Key") ON DELETE RESTRICT;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS "IX_AnimalHybridDefinitionParts_AnimalDefinitionId"
    ON "alchimalia_schema"."AnimalHybridDefinitionParts" ("AnimalDefinitionId");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalHybridCraftParts" (
    "Id" uuid NOT NULL,
    "AnimalCraftId" uuid NOT NULL,
    "SourceAnimalId" uuid NOT NULL,
    "BodyPartKey" character varying(50) NOT NULL,
    "OrderIndex" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_AnimalHybridCraftParts" PRIMARY KEY ("Id")
);

-- Add foreign key constraints for hybrid craft parts
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'AnimalHybridCraftParts'
        AND c.conname = 'FK_AnimalHybridCraftParts_AnimalCrafts_AnimalCraftId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."AnimalHybridCraftParts"
            ADD CONSTRAINT "FK_AnimalHybridCraftParts_AnimalCrafts_AnimalCraftId"
            FOREIGN KEY ("AnimalCraftId") REFERENCES "alchimalia_schema"."AnimalCrafts" ("Id") ON DELETE CASCADE;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'AnimalHybridCraftParts'
        AND c.conname = 'FK_AnimalHybridCraftParts_SourceAnimals_SourceAnimalId'
    ) THEN
        ALTER TABLE "alchimalia_schema"."AnimalHybridCraftParts"
            ADD CONSTRAINT "FK_AnimalHybridCraftParts_SourceAnimals_SourceAnimalId"
            FOREIGN KEY ("SourceAnimalId") REFERENCES "alchimalia_schema"."AnimalDefinitions" ("Id") ON DELETE RESTRICT;
    END IF;
    
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint c
        JOIN pg_class t ON c.conrelid = t.oid
        JOIN pg_namespace n ON t.relnamespace = n.oid
        WHERE n.nspname = 'alchimalia_schema'
        AND t.relname = 'AnimalHybridCraftParts'
        AND c.conname = 'FK_AnimalHybridCraftParts_BodyParts_BodyPartKey'
    ) THEN
        ALTER TABLE "alchimalia_schema"."AnimalHybridCraftParts"
            ADD CONSTRAINT "FK_AnimalHybridCraftParts_BodyParts_BodyPartKey"
            FOREIGN KEY ("BodyPartKey") REFERENCES "alchimalia_schema"."BodyParts" ("Key") ON DELETE RESTRICT;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS "IX_AnimalHybridCraftParts_AnimalCraftId"
    ON "alchimalia_schema"."AnimalHybridCraftParts" ("AnimalCraftId");

COMMIT;
