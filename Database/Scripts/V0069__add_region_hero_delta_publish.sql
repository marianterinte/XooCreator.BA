-- Delta publish scaffolding for Regions and Heroes (change logs + asset links)
-- Run date: 2025-01-XX
-- Description: Adds RegionPublishChangeLogs, HeroPublishChangeLogs, RegionAssetLinks, and HeroAssetLinks tables for incremental publish.

BEGIN;

-- ===========================================
-- REGION PUBLISH CHANGE LOGS
-- ===========================================

-- Use pluralized name to match EF Core conventions: RegionPublishChangeLogs
CREATE TABLE IF NOT EXISTS alchimalia_schema."RegionPublishChangeLogs"
(
    "Id" uuid NOT NULL,
    "RegionId" character varying(100) NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "EntityType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "ChangeType" character varying(32) NOT NULL,
    "Hash" character varying(128),
    "PayloadJson" jsonb,
    "AssetDraftPath" character varying(1024),
    "AssetPublishedPath" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "CreatedBy" uuid,
    CONSTRAINT "PK_RegionPublishChangeLogs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_RegionPublishChangeLogs_RegionId_DraftVersion"
    ON alchimalia_schema."RegionPublishChangeLogs" ("RegionId", "DraftVersion");

CREATE INDEX IF NOT EXISTS "IX_RegionPublishChangeLogs_RegionId"
    ON alchimalia_schema."RegionPublishChangeLogs" ("RegionId");

-- ===========================================
-- HERO PUBLISH CHANGE LOGS
-- ===========================================

-- Use pluralized name to match EF Core conventions: HeroPublishChangeLogs
CREATE TABLE IF NOT EXISTS alchimalia_schema."HeroPublishChangeLogs"
(
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
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "CreatedBy" uuid,
    CONSTRAINT "PK_HeroPublishChangeLogs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_HeroPublishChangeLogs_HeroId_DraftVersion"
    ON alchimalia_schema."HeroPublishChangeLogs" ("HeroId", "DraftVersion");

CREATE INDEX IF NOT EXISTS "IX_HeroPublishChangeLogs_HeroId"
    ON alchimalia_schema."HeroPublishChangeLogs" ("HeroId");

-- ===========================================
-- REGION ASSET LINKS
-- ===========================================

-- Links draft assets to their published counterparts to support rename/copy tracking.
CREATE TABLE IF NOT EXISTS alchimalia_schema."RegionAssetLinks"
(
    "Id" uuid NOT NULL,
    "RegionId" character varying(100) NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10),
    "AssetType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "DraftPath" character varying(1024) NOT NULL,
    "PublishedPath" character varying(1024),
    "ContentHash" character varying(128),
    "LastSyncedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    CONSTRAINT "PK_RegionAssetLinks" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_RegionAssetLinks_RegionId_DraftVersion"
    ON alchimalia_schema."RegionAssetLinks" ("RegionId", "DraftVersion");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_RegionAssetLinks_DraftPath"
    ON alchimalia_schema."RegionAssetLinks" ("DraftPath");

CREATE INDEX IF NOT EXISTS "IX_RegionAssetLinks_RegionId"
    ON alchimalia_schema."RegionAssetLinks" ("RegionId");

-- ===========================================
-- HERO ASSET LINKS
-- ===========================================

-- Links draft assets to their published counterparts to support rename/copy tracking.
CREATE TABLE IF NOT EXISTS alchimalia_schema."HeroAssetLinks"
(
    "Id" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10),
    "AssetType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "DraftPath" character varying(1024) NOT NULL,
    "PublishedPath" character varying(1024),
    "ContentHash" character varying(128),
    "LastSyncedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    CONSTRAINT "PK_HeroAssetLinks" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_HeroAssetLinks_HeroId_DraftVersion"
    ON alchimalia_schema."HeroAssetLinks" ("HeroId", "DraftVersion");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_HeroAssetLinks_DraftPath"
    ON alchimalia_schema."HeroAssetLinks" ("DraftPath");

CREATE INDEX IF NOT EXISTS "IX_HeroAssetLinks_HeroId"
    ON alchimalia_schema."HeroAssetLinks" ("HeroId");

-- Note: LastDraftVersion and LastPublishedVersion columns already exist in:
-- - StoryRegionCrafts.LastDraftVersion (added in V0056)
-- - StoryRegionDefinitions.LastPublishedVersion (added in V0056)
-- - EpicHeroCrafts.LastDraftVersion (added in V0055)
-- - EpicHeroDefinitions.LastPublishedVersion (added in V0055)
-- No need to add them here.

COMMIT;

