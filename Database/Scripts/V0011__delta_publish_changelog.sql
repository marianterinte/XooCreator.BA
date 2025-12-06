-- Delta publish scaffolding (change log + asset links + metadata columns)
-- Run date: 2025-12-03 10:00:00+02:00
-- Description: Adds StoryPublishChangeLog / StoryAssetLinks tables and the supporting columns for incremental publish.

BEGIN;

-- Use pluralized name to match EF Core conventions: StoryPublishChangeLogs
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryPublishChangeLogs"
(
    "Id" uuid NOT NULL,
    "StoryId" character varying(200) NOT NULL,
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
    CONSTRAINT "PK_StoryPublishChangeLogs" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_StoryPublishChangeLogs_StoryId_DraftVersion"
    ON alchimalia_schema."StoryPublishChangeLogs" ("StoryId", "DraftVersion");

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryAssetLinks"
(
    "Id" uuid NOT NULL,
    "StoryId" character varying(200) NOT NULL,
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
    CONSTRAINT "PK_StoryAssetLinks" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_StoryAssetLinks_StoryId_DraftVersion"
    ON alchimalia_schema."StoryAssetLinks" ("StoryId", "DraftVersion");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryAssetLinks_DraftPath"
    ON alchimalia_schema."StoryAssetLinks" ("DraftPath");

ALTER TABLE alchimalia_schema."StoryCrafts"
    ADD COLUMN IF NOT EXISTS "LastDraftVersion" integer NOT NULL DEFAULT 0;

ALTER TABLE alchimalia_schema."StoryDefinitions"
    ADD COLUMN IF NOT EXISTS "LastPublishedVersion" integer NOT NULL DEFAULT 0;

ALTER TABLE alchimalia_schema."StoryTiles"
    ADD COLUMN IF NOT EXISTS "ContentHash" character varying(128);

ALTER TABLE alchimalia_schema."StoryTileTranslations"
    ADD COLUMN IF NOT EXISTS "ContentHash" character varying(128);

COMMIT;

