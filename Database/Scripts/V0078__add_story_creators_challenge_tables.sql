-- Create StoryCreatorsChallenges table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallenges" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ChallengeId" character varying(100) NOT NULL,
    "Status" character varying(20) NOT NULL DEFAULT 'active',
    "SortOrder" integer NOT NULL DEFAULT 0,
    "EndDate" timestamp,
    "CreatedAt" timestamp NOT NULL DEFAULT now(),
    "UpdatedAt" timestamp NOT NULL DEFAULT now(),
    "CreatedByUserId" uuid,
    "UpdatedByUserId" uuid,
    CONSTRAINT "PK_StoryCreatorsChallenges" PRIMARY KEY ("Id"),
    CONSTRAINT "UQ_StoryCreatorsChallenges_ChallengeId" UNIQUE ("ChallengeId")
);

CREATE INDEX "IX_StoryCreatorsChallenges_Status" 
    ON "alchimalia_schema"."StoryCreatorsChallenges" ("Status");
CREATE INDEX "IX_StoryCreatorsChallenges_SortOrder" 
    ON "alchimalia_schema"."StoryCreatorsChallenges" ("SortOrder");
CREATE INDEX "IX_StoryCreatorsChallenges_EndDate" 
    ON "alchimalia_schema"."StoryCreatorsChallenges" ("EndDate");

-- Create StoryCreatorsChallengeTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallengeTranslations" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ChallengeId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL DEFAULT 'ro-ro',
    "Topic" character varying(500) NOT NULL,
    "Description" text,
    CONSTRAINT "PK_StoryCreatorsChallengeTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCreatorsChallengeTranslations_Challenges_ChallengeId"
        FOREIGN KEY ("ChallengeId") REFERENCES "alchimalia_schema"."StoryCreatorsChallenges" ("ChallengeId") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryCreatorsChallengeTranslations_ChallengeId_LanguageCode"
        UNIQUE ("ChallengeId", "LanguageCode")
);

CREATE INDEX "IX_StoryCreatorsChallengeTranslations_ChallengeId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeTranslations" ("ChallengeId");
CREATE INDEX "IX_StoryCreatorsChallengeTranslations_LanguageCode" 
    ON "alchimalia_schema"."StoryCreatorsChallengeTranslations" ("LanguageCode");

-- Create StoryCreatorsChallengeItems table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallengeItems" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ChallengeId" character varying(100) NOT NULL,
    "ItemId" character varying(100) NOT NULL,
    "SortOrder" integer NOT NULL DEFAULT 0,
    "CreatedAt" timestamp NOT NULL DEFAULT now(),
    "UpdatedAt" timestamp NOT NULL DEFAULT now(),
    CONSTRAINT "PK_StoryCreatorsChallengeItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCreatorsChallengeItems_Challenges_ChallengeId"
        FOREIGN KEY ("ChallengeId") REFERENCES "alchimalia_schema"."StoryCreatorsChallenges" ("ChallengeId") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryCreatorsChallengeItems_ChallengeId_ItemId"
        UNIQUE ("ChallengeId", "ItemId"),
    CONSTRAINT "UQ_StoryCreatorsChallengeItems_ItemId"
        UNIQUE ("ItemId")
);

CREATE INDEX "IX_StoryCreatorsChallengeItems_ChallengeId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeItems" ("ChallengeId");
CREATE INDEX "IX_StoryCreatorsChallengeItems_SortOrder" 
    ON "alchimalia_schema"."StoryCreatorsChallengeItems" ("SortOrder");

-- Create StoryCreatorsChallengeItemTranslations table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallengeItemTranslations" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ItemId" character varying(100) NOT NULL,
    "LanguageCode" character varying(10) NOT NULL DEFAULT 'ro-ro',
    "Title" character varying(500) NOT NULL,
    "Description" text,
    CONSTRAINT "PK_StoryCreatorsChallengeItemTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCreatorsChallengeItemTranslations_Items_ItemId"
        FOREIGN KEY ("ItemId") REFERENCES "alchimalia_schema"."StoryCreatorsChallengeItems" ("ItemId") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryCreatorsChallengeItemTranslations_ItemId_LanguageCode"
        UNIQUE ("ItemId", "LanguageCode")
);

CREATE INDEX "IX_StoryCreatorsChallengeItemTranslations_ItemId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeItemTranslations" ("ItemId");
CREATE INDEX "IX_StoryCreatorsChallengeItemTranslations_LanguageCode" 
    ON "alchimalia_schema"."StoryCreatorsChallengeItemTranslations" ("LanguageCode");

-- Create StoryCreatorsChallengeItemRewards table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallengeItemRewards" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ItemId" character varying(100) NOT NULL,
    "TokenType" character varying(50) NOT NULL,
    "TokenValue" character varying(200) NOT NULL,
    "Quantity" integer NOT NULL DEFAULT 1,
    "SortOrder" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_StoryCreatorsChallengeItemRewards" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCreatorsChallengeItemRewards_Items_ItemId"
        FOREIGN KEY ("ItemId") REFERENCES "alchimalia_schema"."StoryCreatorsChallengeItems" ("ItemId") ON DELETE CASCADE
);

CREATE INDEX "IX_StoryCreatorsChallengeItemRewards_ItemId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeItemRewards" ("ItemId");
CREATE INDEX "IX_StoryCreatorsChallengeItemRewards_TokenType" 
    ON "alchimalia_schema"."StoryCreatorsChallengeItemRewards" ("TokenType");
