-- Adds StoryRegionCraft and StoryRegionDefinition tables (similar to StoryEpicCraft and StoryEpicDefinition)
-- StoryRegionCraft = draft regions (for editing)
-- StoryRegionDefinition = published regions (for marketplace/epics)
-- Run date: 2025-12-17
-- Description: Separates region drafts from published regions, similar to epics architecture

BEGIN;

-- Create StoryRegionCrafts table (draft regions)
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryRegionCrafts" (
    "Id" character varying(100) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "ImageUrl" character varying(500),
    "OwnerUserId" uuid NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'draft',
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "BaseVersion" integer NOT NULL DEFAULT 0,
    "LastDraftVersion" integer NOT NULL DEFAULT 0,
    "AssignedReviewerUserId" uuid,
    "ReviewNotes" text,
    "ReviewStartedAt" timestamp with time zone,
    "ReviewEndedAt" timestamp with time zone,
    "ReviewedByUserId" uuid,
    "ApprovedByUserId" uuid,
    CONSTRAINT "PK_StoryRegionCrafts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryRegionCrafts_AlchimaliaUsers_OwnerUserId" 
        FOREIGN KEY ("OwnerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryRegionCrafts_AlchimaliaUsers_AssignedReviewerUserId" 
        FOREIGN KEY ("AssignedReviewerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_StoryRegionCrafts_AlchimaliaUsers_ReviewedByUserId" 
        FOREIGN KEY ("ReviewedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_StoryRegionCrafts_AlchimaliaUsers_ApprovedByUserId" 
        FOREIGN KEY ("ApprovedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "UQ_StoryRegionCrafts_OwnerUserId_Id" 
        UNIQUE ("OwnerUserId", "Id")
);

CREATE INDEX IF NOT EXISTS "IX_StoryRegionCrafts_OwnerUserId_Id" 
    ON "alchimalia_schema"."StoryRegionCrafts" ("OwnerUserId", "Id");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionCrafts_Id_Status" 
    ON "alchimalia_schema"."StoryRegionCrafts" ("Id", "Status");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionCrafts_Status" 
    ON "alchimalia_schema"."StoryRegionCrafts" ("Status");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionCrafts_AssignedReviewerUserId" 
    ON "alchimalia_schema"."StoryRegionCrafts" ("AssignedReviewerUserId") 
    WHERE "AssignedReviewerUserId" IS NOT NULL;

-- Create StoryRegionCraftTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryRegionCraftTranslations" (
    "Id" uuid NOT NULL,
    "StoryRegionCraftId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" text,
    CONSTRAINT "PK_StoryRegionCraftTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryRegionCraftTranslations_StoryRegionCrafts_StoryRegionCraftId" 
        FOREIGN KEY ("StoryRegionCraftId") 
        REFERENCES "alchimalia_schema"."StoryRegionCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryRegionCraftTranslations_StoryRegionCraftId_LanguageCode" 
        UNIQUE ("StoryRegionCraftId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_StoryRegionCraftTranslations_StoryRegionCraftId" 
    ON "alchimalia_schema"."StoryRegionCraftTranslations" ("StoryRegionCraftId");

-- Create StoryRegionDefinitions table (published regions)
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryRegionDefinitions" (
    "Id" character varying(100) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "ImageUrl" character varying(500),
    "OwnerUserId" uuid NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'published',
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "PublishedAtUtc" timestamp with time zone,
    "Version" integer NOT NULL DEFAULT 1,
    "BaseVersion" integer NOT NULL DEFAULT 0,
    "LastPublishedVersion" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_StoryRegionDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryRegionDefinitions_AlchimaliaUsers_OwnerUserId" 
        FOREIGN KEY ("OwnerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryRegionDefinitions_OwnerUserId_Id" 
        UNIQUE ("OwnerUserId", "Id")
);

CREATE INDEX IF NOT EXISTS "IX_StoryRegionDefinitions_OwnerUserId_Id" 
    ON "alchimalia_schema"."StoryRegionDefinitions" ("OwnerUserId", "Id");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionDefinitions_Id_Version" 
    ON "alchimalia_schema"."StoryRegionDefinitions" ("Id", "Version");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionDefinitions_Status" 
    ON "alchimalia_schema"."StoryRegionDefinitions" ("Status");

-- Create StoryRegionDefinitionTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryRegionDefinitionTranslations" (
    "Id" uuid NOT NULL,
    "StoryRegionDefinitionId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" text,
    CONSTRAINT "PK_StoryRegionDefinitionTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryRegionDefinitionTranslations_StoryRegionDefinitions_StoryRegionDefinitionId" 
        FOREIGN KEY ("StoryRegionDefinitionId") 
        REFERENCES "alchimalia_schema"."StoryRegionDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryRegionDefinitionTranslations_StoryRegionDefinitionId_LanguageCode" 
        UNIQUE ("StoryRegionDefinitionId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_StoryRegionDefinitionTranslations_StoryRegionDefinitionId" 
    ON "alchimalia_schema"."StoryRegionDefinitionTranslations" ("StoryRegionDefinitionId");

COMMIT;

