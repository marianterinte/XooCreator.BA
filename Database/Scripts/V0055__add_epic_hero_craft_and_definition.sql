-- Adds EpicHeroCraft and EpicHeroDefinition tables (similar to StoryEpicCraft and StoryEpicDefinition)
-- EpicHeroCraft = draft heroes (for editing)
-- EpicHeroDefinition = published heroes (for marketplace/epics)
-- Run date: 2025-12-17
-- Description: Separates hero drafts from published heroes, similar to epics architecture

BEGIN;

-- Create EpicHeroCrafts table (draft heroes)
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicHeroCrafts" (
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
    CONSTRAINT "PK_EpicHeroCrafts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicHeroCrafts_AlchimaliaUsers_OwnerUserId" 
        FOREIGN KEY ("OwnerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicHeroCrafts_AlchimaliaUsers_AssignedReviewerUserId" 
        FOREIGN KEY ("AssignedReviewerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_EpicHeroCrafts_AlchimaliaUsers_ReviewedByUserId" 
        FOREIGN KEY ("ReviewedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_EpicHeroCrafts_AlchimaliaUsers_ApprovedByUserId" 
        FOREIGN KEY ("ApprovedByUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "UQ_EpicHeroCrafts_OwnerUserId_Id" 
        UNIQUE ("OwnerUserId", "Id")
);

CREATE INDEX IF NOT EXISTS "IX_EpicHeroCrafts_OwnerUserId_Id" 
    ON "alchimalia_schema"."EpicHeroCrafts" ("OwnerUserId", "Id");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroCrafts_Id_Status" 
    ON "alchimalia_schema"."EpicHeroCrafts" ("Id", "Status");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroCrafts_Status" 
    ON "alchimalia_schema"."EpicHeroCrafts" ("Status");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroCrafts_AssignedReviewerUserId" 
    ON "alchimalia_schema"."EpicHeroCrafts" ("AssignedReviewerUserId") 
    WHERE "AssignedReviewerUserId" IS NOT NULL;

-- Create EpicHeroCraftTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicHeroCraftTranslations" (
    "Id" uuid NOT NULL,
    "EpicHeroCraftId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" text,
    "GreetingText" text,
    "GreetingAudioUrl" character varying(500),
    CONSTRAINT "PK_EpicHeroCraftTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicHeroCraftTranslations_EpicHeroCrafts_EpicHeroCraftId" 
        FOREIGN KEY ("EpicHeroCraftId") 
        REFERENCES "alchimalia_schema"."EpicHeroCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_EpicHeroCraftTranslations_EpicHeroCraftId_LanguageCode" 
        UNIQUE ("EpicHeroCraftId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_EpicHeroCraftTranslations_EpicHeroCraftId" 
    ON "alchimalia_schema"."EpicHeroCraftTranslations" ("EpicHeroCraftId");

-- Create EpicHeroDefinitions table (published heroes)
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicHeroDefinitions" (
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
    CONSTRAINT "PK_EpicHeroDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicHeroDefinitions_AlchimaliaUsers_OwnerUserId" 
        FOREIGN KEY ("OwnerUserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_EpicHeroDefinitions_OwnerUserId_Id" 
        UNIQUE ("OwnerUserId", "Id")
);

CREATE INDEX IF NOT EXISTS "IX_EpicHeroDefinitions_OwnerUserId_Id" 
    ON "alchimalia_schema"."EpicHeroDefinitions" ("OwnerUserId", "Id");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroDefinitions_Id_Version" 
    ON "alchimalia_schema"."EpicHeroDefinitions" ("Id", "Version");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroDefinitions_Status" 
    ON "alchimalia_schema"."EpicHeroDefinitions" ("Status");

-- Create EpicHeroDefinitionTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicHeroDefinitionTranslations" (
    "Id" uuid NOT NULL,
    "EpicHeroDefinitionId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" text,
    "GreetingText" text,
    "GreetingAudioUrl" character varying(500),
    CONSTRAINT "PK_EpicHeroDefinitionTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicHeroDefinitionTranslations_EpicHeroDefinitions_EpicHeroDefinitionId" 
        FOREIGN KEY ("EpicHeroDefinitionId") 
        REFERENCES "alchimalia_schema"."EpicHeroDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_EpicHeroDefinitionTranslations_EpicHeroDefinitionId_LanguageCode" 
        UNIQUE ("EpicHeroDefinitionId", "LanguageCode")
);

CREATE INDEX IF NOT EXISTS "IX_EpicHeroDefinitionTranslations_EpicHeroDefinitionId" 
    ON "alchimalia_schema"."EpicHeroDefinitionTranslations" ("EpicHeroDefinitionId");

COMMIT;

