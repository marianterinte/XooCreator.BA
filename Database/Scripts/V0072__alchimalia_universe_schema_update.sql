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
-- Migrates existing HeroDefinitions/Animals into new Craft/Definition tables
-- Idempotent inserts; does not modify legacy tables.
-- Story Heroes JSON -> DB handled separately (not possible in pure SQL).
-- Run date: 2026-01-16
-- Updated: 2025-01-28 - Removed Type column, added versioning fields

BEGIN;

-- =========================
-- Tree of Heroes: Definitions
-- =========================
INSERT INTO "alchimalia_schema"."HeroDefinitionDefinitions" (
    "Id", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
    "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image",
    "Status", "Version", "BaseVersion", "LastPublishedVersion", "PublishedByUserId", "CreatedAt", "UpdatedAt", "PublishedAtUtc"
)
SELECT
    hd."Id", hd."CourageCost", hd."CuriosityCost", hd."ThinkingCost", hd."CreativityCost", hd."SafetyCost",
    hd."PrerequisitesJson", hd."RewardsJson", hd."IsUnlocked", hd."PositionX", hd."PositionY", hd."Image",
    'published', 1, 0, 0, NULL, COALESCE(hd."CreatedAt", now() at time zone 'utc'), COALESCE(hd."UpdatedAt", now() at time zone 'utc'), now() at time zone 'utc'
FROM "alchimalia_schema"."HeroDefinitions" hd
WHERE hd."Id" NOT IN ('puf-puf', 'linkaro', 'grubot')
AND NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionDefinitions" d WHERE d."Id" = hd."Id"
);

INSERT INTO "alchimalia_schema"."HeroDefinitionDefinitionTranslations" (
    "Id", "HeroDefinitionDefinitionId", "LanguageCode", "Name", "Description", "Story"
)
SELECT
    hdt."Id", hdt."HeroDefinitionId", hdt."LanguageCode", hdt."Name", hdt."Description", hdt."Story"
FROM "alchimalia_schema"."HeroDefinitionTranslations" hdt
WHERE hdt."HeroDefinitionId" NOT IN ('puf-puf', 'linkaro', 'grubot')
AND NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionDefinitionTranslations" dt
    WHERE dt."HeroDefinitionDefinitionId" = hdt."HeroDefinitionId" AND dt."LanguageCode" = hdt."LanguageCode"
);

-- =========================
-- Tree of Heroes: Crafts (published mirror)
-- =========================
INSERT INTO "alchimalia_schema"."HeroDefinitionCrafts" (
    "Id", "PublishedDefinitionId", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
    "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image",
    "Status", "BaseVersion", "LastDraftVersion", "CreatedByUserId", "AssignedReviewerUserId", "ReviewedByUserId", "ApprovedByUserId", "ReviewNotes", "ReviewStartedAt", "ReviewEndedAt", "CreatedAt", "UpdatedAt"
)
SELECT
    hd."Id", hd."Id", hd."CourageCost", hd."CuriosityCost", hd."ThinkingCost", hd."CreativityCost", hd."SafetyCost",
    hd."PrerequisitesJson", hd."RewardsJson", hd."IsUnlocked", hd."PositionX", hd."PositionY", hd."Image",
    'published', 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, COALESCE(hd."CreatedAt", now() at time zone 'utc'), COALESCE(hd."UpdatedAt", now() at time zone 'utc')
FROM "alchimalia_schema"."HeroDefinitions" hd
WHERE hd."Id" NOT IN ('puf-puf', 'linkaro', 'grubot')
AND NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionCrafts" c WHERE c."Id" = hd."Id"
);

INSERT INTO "alchimalia_schema"."HeroDefinitionCraftTranslations" (
    "Id", "HeroDefinitionCraftId", "LanguageCode", "Name", "Description", "Story"
)
SELECT
    hdt."Id", hdt."HeroDefinitionId", hdt."LanguageCode", hdt."Name", hdt."Description", hdt."Story"
FROM "alchimalia_schema"."HeroDefinitionTranslations" hdt
WHERE hdt."HeroDefinitionId" NOT IN ('puf-puf', 'linkaro', 'grubot')
AND NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionCraftTranslations" ct
    WHERE ct."HeroDefinitionCraftId" = hdt."HeroDefinitionId" AND ct."LanguageCode" = hdt."LanguageCode"
);

-- =========================
-- LOI Animals: Definitions
-- =========================
INSERT INTO "alchimalia_schema"."AnimalDefinitions" (
    "Id", "Label", "Src", "IsHybrid", "RegionId", "Status", "Version", "BaseVersion", "LastPublishedVersion", "PublishedByUserId", "CreatedAt", "UpdatedAt", "PublishedAtUtc"
)
SELECT
    a."Id", a."Label", a."Src", a."IsHybrid", a."RegionId", 'published', 1, 0, 0, NULL,
    now() at time zone 'utc', now() at time zone 'utc', now() at time zone 'utc'
FROM "alchimalia_schema"."Animals" a
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalDefinitions" d WHERE d."Id" = a."Id"
);

INSERT INTO "alchimalia_schema"."AnimalDefinitionTranslations" (
    "Id", "AnimalDefinitionId", "LanguageCode", "Label"
)
SELECT
    at."Id", at."AnimalId", at."LanguageCode", at."Label"
FROM "alchimalia_schema"."AnimalTranslations" at
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalDefinitionTranslations" dt
    WHERE dt."AnimalDefinitionId" = at."AnimalId" AND dt."LanguageCode" = at."LanguageCode"
);

INSERT INTO "alchimalia_schema"."AnimalDefinitionPartSupports" (
    "AnimalDefinitionId", "BodyPartKey"
)
SELECT
    aps."AnimalId", aps."PartKey"
FROM "alchimalia_schema"."AnimalPartSupports" aps
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalDefinitionPartSupports" dps
    WHERE dps."AnimalDefinitionId" = aps."AnimalId" AND dps."BodyPartKey" = aps."PartKey"
);

-- =========================
-- LOI Animals: Crafts (published mirror)
-- =========================
INSERT INTO "alchimalia_schema"."AnimalCrafts" (
    "Id", "PublishedDefinitionId", "Label", "Src", "IsHybrid", "RegionId",
    "Status", "BaseVersion", "LastDraftVersion", "CreatedByUserId", "AssignedReviewerUserId", "ReviewedByUserId", "ApprovedByUserId", "ReviewNotes", "ReviewStartedAt", "ReviewEndedAt", "CreatedAt", "UpdatedAt"
)
SELECT
    a."Id", a."Id", a."Label", a."Src", a."IsHybrid", a."RegionId",
    'published', 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, now() at time zone 'utc', now() at time zone 'utc'
FROM "alchimalia_schema"."Animals" a
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalCrafts" c WHERE c."Id" = a."Id"
);

INSERT INTO "alchimalia_schema"."AnimalCraftTranslations" (
    "Id", "AnimalCraftId", "LanguageCode", "Label"
)
SELECT
    at."Id", at."AnimalId", at."LanguageCode", at."Label"
FROM "alchimalia_schema"."AnimalTranslations" at
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalCraftTranslations" ct
    WHERE ct."AnimalCraftId" = at."AnimalId" AND ct."LanguageCode" = at."LanguageCode"
);

INSERT INTO "alchimalia_schema"."AnimalCraftPartSupports" (
    "AnimalCraftId", "BodyPartKey"
)
SELECT
    aps."AnimalId", aps."PartKey"
FROM "alchimalia_schema"."AnimalPartSupports" aps
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalCraftPartSupports" cps
    WHERE cps."AnimalCraftId" = aps."AnimalId" AND cps."BodyPartKey" = aps."PartKey"
);

COMMIT;
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
-- Make RegionId optional (nullable) in AnimalCrafts and AnimalDefinitions tables
-- This allows animals to exist without being associated with a specific region

-- AnimalCrafts table
ALTER TABLE "alchimalia_schema"."AnimalCrafts" 
    ALTER COLUMN "RegionId" DROP NOT NULL;

-- AnimalDefinitions table  
ALTER TABLE "alchimalia_schema"."AnimalDefinitions"
    ALTER COLUMN "RegionId" DROP NOT NULL;

-- Note: Foreign key constraints remain, but now allow NULL values
-- This means animals can optionally be associated with a region, but if a region is specified,
-- it must be a valid region ID from the Regions table
CREATE TABLE alchimalia_schema."HeroPublishJobs" (
    "Id" uuid NOT NULL,
    "HeroId" text NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" text NOT NULL,
    "LangTag" text NOT NULL DEFAULT 'ro-ro',
    "ForceFull" boolean NOT NULL DEFAULT FALSE,
    "Status" text NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone NULL,
    "CompletedAtUtc" timestamp with time zone NULL,
    "ErrorMessage" text NULL,
    CONSTRAINT "PK_HeroPublishJobs" PRIMARY KEY ("Id")
);

CREATE TABLE alchimalia_schema."AnimalPublishJobs" (
    "Id" uuid NOT NULL,
    "AnimalId" text NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" text NOT NULL,
    "LangTag" text NOT NULL DEFAULT 'ro-ro',
    "ForceFull" boolean NOT NULL DEFAULT FALSE,
    "Status" text NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone NULL,
    "CompletedAtUtc" timestamp with time zone NULL,
    "ErrorMessage" text NULL,
    CONSTRAINT "PK_AnimalPublishJobs" PRIMARY KEY ("Id")
);
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
