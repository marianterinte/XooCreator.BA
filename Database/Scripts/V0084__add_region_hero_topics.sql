-- Adds topic (tagging) support for Regions and Heroes - many-to-many with StoryTopics
-- 2 junction tables for Crafts (draft), 2 for Definitions (published)

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryRegionCraftTopic" (
    "StoryRegionCraftId" text NOT NULL,
    "StoryTopicId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
    CONSTRAINT "PK_StoryRegionCraftTopic" PRIMARY KEY ("StoryRegionCraftId", "StoryTopicId"),
    CONSTRAINT "FK_StoryRegionCraftTopic_StoryRegionCrafts_StoryRegionCraftId" FOREIGN KEY ("StoryRegionCraftId") REFERENCES alchimalia_schema."StoryRegionCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryRegionCraftTopic_StoryTopics_StoryTopicId" FOREIGN KEY ("StoryTopicId") REFERENCES alchimalia_schema."StoryTopics" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryRegionDefinitionTopic" (
    "StoryRegionDefinitionId" text NOT NULL,
    "StoryTopicId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
    CONSTRAINT "PK_StoryRegionDefinitionTopic" PRIMARY KEY ("StoryRegionDefinitionId", "StoryTopicId"),
    CONSTRAINT "FK_StoryRegionDefinitionTopic_StoryRegionDefinitions_StoryRegionDefinitionId" FOREIGN KEY ("StoryRegionDefinitionId") REFERENCES alchimalia_schema."StoryRegionDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryRegionDefinitionTopic_StoryTopics_StoryTopicId" FOREIGN KEY ("StoryTopicId") REFERENCES alchimalia_schema."StoryTopics" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicHeroCraftTopic" (
    "EpicHeroCraftId" text NOT NULL,
    "StoryTopicId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
    CONSTRAINT "PK_EpicHeroCraftTopic" PRIMARY KEY ("EpicHeroCraftId", "StoryTopicId"),
    CONSTRAINT "FK_EpicHeroCraftTopic_EpicHeroCrafts_EpicHeroCraftId" FOREIGN KEY ("EpicHeroCraftId") REFERENCES alchimalia_schema."EpicHeroCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicHeroCraftTopic_StoryTopics_StoryTopicId" FOREIGN KEY ("StoryTopicId") REFERENCES alchimalia_schema."StoryTopics" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicHeroDefinitionTopic" (
    "EpicHeroDefinitionId" text NOT NULL,
    "StoryTopicId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
    CONSTRAINT "PK_EpicHeroDefinitionTopic" PRIMARY KEY ("EpicHeroDefinitionId", "StoryTopicId"),
    CONSTRAINT "FK_EpicHeroDefinitionTopic_EpicHeroDefinitions_EpicHeroDefinitionId" FOREIGN KEY ("EpicHeroDefinitionId") REFERENCES alchimalia_schema."EpicHeroDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicHeroDefinitionTopic_StoryTopics_StoryTopicId" FOREIGN KEY ("StoryTopicId") REFERENCES alchimalia_schema."StoryTopics" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_StoryRegionCraftTopic_StoryTopicId" ON alchimalia_schema."StoryRegionCraftTopic" ("StoryTopicId");
CREATE INDEX IF NOT EXISTS "IX_StoryRegionDefinitionTopic_StoryTopicId" ON alchimalia_schema."StoryRegionDefinitionTopic" ("StoryTopicId");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroCraftTopic_StoryTopicId" ON alchimalia_schema."EpicHeroCraftTopic" ("StoryTopicId");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroDefinitionTopic_StoryTopicId" ON alchimalia_schema."EpicHeroDefinitionTopic" ("StoryTopicId");
