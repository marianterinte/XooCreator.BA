-- Delta publish scaffolding for Epic
-- Run date: 2025-01-29
-- Description: Adds EpicPublishChangeLogs and EpicAssetLinks tables for incremental publish.

BEGIN;

-- ===========================================
-- EPIC PUBLISH CHANGE LOGS
-- ===========================================

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicPublishChangeLogs"
(
    "Id" uuid NOT NULL,
    "EpicId" character varying(100) NOT NULL,
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
    CONSTRAINT "PK_EpicPublishChangeLogs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_EpicPublishChangeLogs_EpicId_DraftVersion"
    ON alchimalia_schema."EpicPublishChangeLogs" ("EpicId", "DraftVersion");

-- ===========================================
-- EPIC ASSET LINKS
-- ===========================================

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicAssetLinks"
(
    "Id" uuid NOT NULL,
    "EpicId" character varying(100) NOT NULL,
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
    CONSTRAINT "PK_EpicAssetLinks" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_EpicAssetLinks_EpicId_DraftVersion"
    ON alchimalia_schema."EpicAssetLinks" ("EpicId", "DraftVersion");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_EpicAssetLinks_DraftPath"
    ON alchimalia_schema."EpicAssetLinks" ("DraftPath");

COMMIT;

