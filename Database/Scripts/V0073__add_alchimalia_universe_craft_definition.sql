-- Adds Craft/Definition tables for Alchimalia Universe (Tree of Heroes + LOI Animals)
-- Includes versioning, change logs, and version jobs for complete workflow support.
-- Story Heroes remain on existing tables; JSON removed elsewhere in code.
-- Run date: 2026-01-16
-- Updated: 2025-01-28 - Added versioning fields, change logs, and version jobs

BEGIN;

-- =========================
-- Tree of Heroes (Craft)
-- =========================
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionCrafts" (
    "Id" character varying(100) NOT NULL,
    "PublishedDefinitionId" character varying(100),
    "CourageCost" integer NOT NULL DEFAULT 0,
    "CuriosityCost" integer NOT NULL DEFAULT 0,
    "ThinkingCost" integer NOT NULL DEFAULT 0,
    "CreativityCost" integer NOT NULL DEFAULT 0,
    "SafetyCost" integer NOT NULL DEFAULT 0,
    "PrerequisitesJson" text NOT NULL DEFAULT '[]',
    "RewardsJson" text NOT NULL DEFAULT '[]',
    "IsUnlocked" boolean NOT NULL DEFAULT false,
    "PositionX" double precision NOT NULL DEFAULT 0,
    "PositionY" double precision NOT NULL DEFAULT 0,
    "Image" character varying(500) NOT NULL DEFAULT '',
    "Status" character varying(20) NOT NULL DEFAULT 'draft',
    "BaseVersion" integer NOT NULL DEFAULT 0,
    "LastDraftVersion" integer NOT NULL DEFAULT 0,
    "CreatedByUserId" uuid,
    "AssignedReviewerUserId" uuid,
    "ReviewedByUserId" uuid,
    "ApprovedByUserId" uuid,
    "ReviewNotes" text,
    "ReviewStartedAt" timestamp with time zone,
    "ReviewEndedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_HeroDefinitionCrafts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HeroDefinitionCrafts_AlchimaliaUsers_CreatedByUserId"
        FOREIGN KEY ("CreatedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_HeroDefinitionCrafts_AlchimaliaUsers_AssignedReviewerUserId"
        FOREIGN KEY ("AssignedReviewerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_HeroDefinitionCrafts_AlchimaliaUsers_ReviewedByUserId"
        FOREIGN KEY ("ReviewedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_HeroDefinitionCrafts_AlchimaliaUsers_ApprovedByUserId"
        FOREIGN KEY ("ApprovedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionCrafts_Status"
    ON "alchimalia_schema"."HeroDefinitionCrafts" ("Status");
CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionCrafts_CreatedByUserId"
    ON "alchimalia_schema"."HeroDefinitionCrafts" ("CreatedByUserId")
    WHERE "CreatedByUserId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionCrafts_AssignedReviewerUserId"
    ON "alchimalia_schema"."HeroDefinitionCrafts" ("AssignedReviewerUserId")
    WHERE "AssignedReviewerUserId" IS NOT NULL;

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionCraftTranslations" (
    "Id" uuid NOT NULL,
    "HeroDefinitionCraftId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Description" text NOT NULL,
    "Story" text NOT NULL,
    CONSTRAINT "PK_HeroDefinitionCraftTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HeroDefinitionCraftTranslations_HeroDefinitionCrafts_HeroDefinitionCraftId"
        FOREIGN KEY ("HeroDefinitionCraftId")
        REFERENCES "alchimalia_schema"."HeroDefinitionCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_HeroDefinitionCraftTranslations_Craft_Language"
        UNIQUE ("HeroDefinitionCraftId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionCraftTranslations_CraftId"
    ON "alchimalia_schema"."HeroDefinitionCraftTranslations" ("HeroDefinitionCraftId");

-- =========================
-- Tree of Heroes (Definition)
-- =========================
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionDefinitions" (
    "Id" character varying(100) NOT NULL,
    "CourageCost" integer NOT NULL DEFAULT 0,
    "CuriosityCost" integer NOT NULL DEFAULT 0,
    "ThinkingCost" integer NOT NULL DEFAULT 0,
    "CreativityCost" integer NOT NULL DEFAULT 0,
    "SafetyCost" integer NOT NULL DEFAULT 0,
    "PrerequisitesJson" text NOT NULL DEFAULT '[]',
    "RewardsJson" text NOT NULL DEFAULT '[]',
    "IsUnlocked" boolean NOT NULL DEFAULT false,
    "PositionX" double precision NOT NULL DEFAULT 0,
    "PositionY" double precision NOT NULL DEFAULT 0,
    "Image" character varying(500) NOT NULL DEFAULT '',
    "Status" character varying(20) NOT NULL DEFAULT 'published',
    "Version" integer NOT NULL DEFAULT 1,
    "BaseVersion" integer NOT NULL DEFAULT 0,
    "LastPublishedVersion" integer NOT NULL DEFAULT 0,
    "PublishedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "PublishedAtUtc" timestamp with time zone,
    CONSTRAINT "PK_HeroDefinitionDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HeroDefinitionDefinitions_AlchimaliaUsers_PublishedByUserId"
        FOREIGN KEY ("PublishedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionDefinitions_Status"
    ON "alchimalia_schema"."HeroDefinitionDefinitions" ("Status");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionDefinitionTranslations" (
    "Id" uuid NOT NULL,
    "HeroDefinitionDefinitionId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Description" text NOT NULL,
    "Story" text NOT NULL,
    CONSTRAINT "PK_HeroDefinitionDefinitionTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HeroDefinitionDefinitionTranslations_HeroDefinitionDefinitions_HeroDefinitionDefinitionId"
        FOREIGN KEY ("HeroDefinitionDefinitionId")
        REFERENCES "alchimalia_schema"."HeroDefinitionDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_HeroDefinitionDefinitionTranslations_Definition_Language"
        UNIQUE ("HeroDefinitionDefinitionId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionDefinitionTranslations_DefinitionId"
    ON "alchimalia_schema"."HeroDefinitionDefinitionTranslations" ("HeroDefinitionDefinitionId");

-- =========================
-- LOI Animals (Craft)
-- =========================
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalCrafts" (
    "Id" uuid NOT NULL,
    "PublishedDefinitionId" uuid,
    "Label" character varying(255) NOT NULL,
    "Src" character varying(500) NOT NULL,
    "IsHybrid" boolean NOT NULL DEFAULT false,
    "RegionId" uuid NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'draft',
    "BaseVersion" integer NOT NULL DEFAULT 0,
    "LastDraftVersion" integer NOT NULL DEFAULT 0,
    "CreatedByUserId" uuid,
    "AssignedReviewerUserId" uuid,
    "ReviewedByUserId" uuid,
    "ApprovedByUserId" uuid,
    "ReviewNotes" text,
    "ReviewStartedAt" timestamp with time zone,
    "ReviewEndedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_AnimalCrafts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AnimalCrafts_Regions_RegionId"
        FOREIGN KEY ("RegionId") REFERENCES "alchimalia_schema"."Regions" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_AnimalCrafts_AlchimaliaUsers_CreatedByUserId"
        FOREIGN KEY ("CreatedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_AnimalCrafts_AlchimaliaUsers_AssignedReviewerUserId"
        FOREIGN KEY ("AssignedReviewerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_AnimalCrafts_AlchimaliaUsers_ReviewedByUserId"
        FOREIGN KEY ("ReviewedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_AnimalCrafts_AlchimaliaUsers_ApprovedByUserId"
        FOREIGN KEY ("ApprovedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_AnimalCrafts_Status"
    ON "alchimalia_schema"."AnimalCrafts" ("Status");
CREATE INDEX IF NOT EXISTS "IX_AnimalCrafts_RegionId"
    ON "alchimalia_schema"."AnimalCrafts" ("RegionId");
CREATE INDEX IF NOT EXISTS "IX_AnimalCrafts_AssignedReviewerUserId"
    ON "alchimalia_schema"."AnimalCrafts" ("AssignedReviewerUserId")
    WHERE "AssignedReviewerUserId" IS NOT NULL;

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalCraftTranslations" (
    "Id" uuid NOT NULL,
    "AnimalCraftId" uuid NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Label" character varying(255) NOT NULL,
    CONSTRAINT "PK_AnimalCraftTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AnimalCraftTranslations_AnimalCrafts_AnimalCraftId"
        FOREIGN KEY ("AnimalCraftId")
        REFERENCES "alchimalia_schema"."AnimalCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_AnimalCraftTranslations_Craft_Language"
        UNIQUE ("AnimalCraftId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_AnimalCraftTranslations_CraftId"
    ON "alchimalia_schema"."AnimalCraftTranslations" ("AnimalCraftId");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalCraftPartSupports" (
    "AnimalCraftId" uuid NOT NULL,
    "BodyPartKey" character varying(50) NOT NULL,
    CONSTRAINT "PK_AnimalCraftPartSupports" PRIMARY KEY ("AnimalCraftId", "BodyPartKey"),
    CONSTRAINT "FK_AnimalCraftPartSupports_AnimalCrafts_AnimalCraftId"
        FOREIGN KEY ("AnimalCraftId")
        REFERENCES "alchimalia_schema"."AnimalCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AnimalCraftPartSupports_BodyParts_BodyPartKey"
        FOREIGN KEY ("BodyPartKey")
        REFERENCES "alchimalia_schema"."BodyParts" ("Key") ON DELETE RESTRICT
);

-- =========================
-- LOI Animals (Definition)
-- =========================
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalDefinitions" (
    "Id" uuid NOT NULL,
    "Label" character varying(255) NOT NULL,
    "Src" character varying(500) NOT NULL,
    "IsHybrid" boolean NOT NULL DEFAULT false,
    "RegionId" uuid NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'published',
    "Version" integer NOT NULL DEFAULT 1,
    "BaseVersion" integer NOT NULL DEFAULT 0,
    "LastPublishedVersion" integer NOT NULL DEFAULT 0,
    "PublishedByUserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "PublishedAtUtc" timestamp with time zone,
    CONSTRAINT "PK_AnimalDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AnimalDefinitions_Regions_RegionId"
        FOREIGN KEY ("RegionId") REFERENCES "alchimalia_schema"."Regions" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_AnimalDefinitions_AlchimaliaUsers_PublishedByUserId"
        FOREIGN KEY ("PublishedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_AnimalDefinitions_Status"
    ON "alchimalia_schema"."AnimalDefinitions" ("Status");
CREATE INDEX IF NOT EXISTS "IX_AnimalDefinitions_RegionId"
    ON "alchimalia_schema"."AnimalDefinitions" ("RegionId");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalDefinitionTranslations" (
    "Id" uuid NOT NULL,
    "AnimalDefinitionId" uuid NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Label" character varying(255) NOT NULL,
    CONSTRAINT "PK_AnimalDefinitionTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AnimalDefinitionTranslations_AnimalDefinitions_AnimalDefinitionId"
        FOREIGN KEY ("AnimalDefinitionId")
        REFERENCES "alchimalia_schema"."AnimalDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_AnimalDefinitionTranslations_Definition_Language"
        UNIQUE ("AnimalDefinitionId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_AnimalDefinitionTranslations_DefinitionId"
    ON "alchimalia_schema"."AnimalDefinitionTranslations" ("AnimalDefinitionId");

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalDefinitionPartSupports" (
    "AnimalDefinitionId" uuid NOT NULL,
    "BodyPartKey" character varying(50) NOT NULL,
    CONSTRAINT "PK_AnimalDefinitionPartSupports" PRIMARY KEY ("AnimalDefinitionId", "BodyPartKey"),
    CONSTRAINT "FK_AnimalDefinitionPartSupports_AnimalDefinitions_AnimalDefinitionId"
        FOREIGN KEY ("AnimalDefinitionId")
        REFERENCES "alchimalia_schema"."AnimalDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AnimalDefinitionPartSupports_BodyParts_BodyPartKey"
        FOREIGN KEY ("BodyPartKey")
        REFERENCES "alchimalia_schema"."BodyParts" ("Key") ON DELETE RESTRICT
);

-- =========================
-- Change Log Tables (Delta Publish Support)
-- =========================

-- HeroDefinitionPublishChangeLogs
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionPublishChangeLogs" (
    "Id" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "EntityType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "ChangeType" character varying(32) NOT NULL,
    "Hash" character varying(128),
    "PayloadJson" jsonb,
    "AssetDraftPath" character varying(1024),
    "AssetPublishedPath" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreatedBy" uuid,
    CONSTRAINT "PK_HeroDefinitionPublishChangeLogs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionPublishChangeLogs_HeroId_DraftVersion"
    ON "alchimalia_schema"."HeroDefinitionPublishChangeLogs" ("HeroId", "DraftVersion");
CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionPublishChangeLogs_HeroId"
    ON "alchimalia_schema"."HeroDefinitionPublishChangeLogs" ("HeroId");

-- AnimalPublishChangeLogs
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalPublishChangeLogs" (
    "Id" uuid NOT NULL,
    "AnimalId" uuid NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "EntityType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "ChangeType" character varying(32) NOT NULL,
    "Hash" character varying(128),
    "PayloadJson" jsonb,
    "AssetDraftPath" character varying(1024),
    "AssetPublishedPath" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreatedBy" uuid,
    CONSTRAINT "PK_AnimalPublishChangeLogs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_AnimalPublishChangeLogs_AnimalId_DraftVersion"
    ON "alchimalia_schema"."AnimalPublishChangeLogs" ("AnimalId", "DraftVersion");
CREATE INDEX IF NOT EXISTS "IX_AnimalPublishChangeLogs_AnimalId"
    ON "alchimalia_schema"."AnimalPublishChangeLogs" ("AnimalId");

-- =========================
-- Version Job Tables (Create Version Support)
-- =========================

-- HeroDefinitionVersionJobs
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionVersionJobs" (
    "Id" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_HeroDefinitionVersionJobs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HeroDefinitionVersionJobs_AlchimaliaUsers_OwnerUserId"
        FOREIGN KEY ("OwnerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionVersionJobs_HeroId_Status"
    ON "alchimalia_schema"."HeroDefinitionVersionJobs" ("HeroId", "Status");
CREATE INDEX IF NOT EXISTS "IX_HeroDefinitionVersionJobs_QueuedAtUtc"
    ON "alchimalia_schema"."HeroDefinitionVersionJobs" ("QueuedAtUtc");

-- AnimalVersionJobs
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalVersionJobs" (
    "Id" uuid NOT NULL,
    "AnimalId" uuid NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_AnimalVersionJobs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AnimalVersionJobs_AlchimaliaUsers_OwnerUserId"
        FOREIGN KEY ("OwnerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_AnimalVersionJobs_AnimalId_Status"
    ON "alchimalia_schema"."AnimalVersionJobs" ("AnimalId", "Status");
CREATE INDEX IF NOT EXISTS "IX_AnimalVersionJobs_QueuedAtUtc"
    ON "alchimalia_schema"."AnimalVersionJobs" ("QueuedAtUtc");

COMMIT;
