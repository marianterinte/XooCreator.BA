-- Adds support for Epic Topics and Age Groups (Many-to-Many relationships)

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEpicCraftTopic" (
    "StoryEpicCraftId" text NOT NULL,
    "StoryTopicId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_StoryEpicCraftTopic" PRIMARY KEY ("StoryEpicCraftId", "StoryTopicId"),
    CONSTRAINT "FK_StoryEpicCraftTopic_StoryEpicCrafts_StoryEpicCraftId" FOREIGN KEY ("StoryEpicCraftId") REFERENCES alchimalia_schema."StoryEpicCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryEpicCraftTopic_StoryTopics_StoryTopicId" FOREIGN KEY ("StoryTopicId") REFERENCES alchimalia_schema."StoryTopics" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEpicCraftAgeGroup" (
    "StoryEpicCraftId" text NOT NULL,
    "StoryAgeGroupId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_StoryEpicCraftAgeGroup" PRIMARY KEY ("StoryEpicCraftId", "StoryAgeGroupId"),
    CONSTRAINT "FK_StoryEpicCraftAgeGroup_StoryEpicCrafts_StoryEpicCraftId" FOREIGN KEY ("StoryEpicCraftId") REFERENCES alchimalia_schema."StoryEpicCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryEpicCraftAgeGroup_StoryAgeGroups_StoryAgeGroupId" FOREIGN KEY ("StoryAgeGroupId") REFERENCES alchimalia_schema."StoryAgeGroups" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEpicDefinitionTopic" (
    "StoryEpicDefinitionId" text NOT NULL,
    "StoryTopicId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_StoryEpicDefinitionTopic" PRIMARY KEY ("StoryEpicDefinitionId", "StoryTopicId"),
    CONSTRAINT "FK_StoryEpicDefinitionTopic_StoryEpicDefinitions_StoryEpicDefinitionId" FOREIGN KEY ("StoryEpicDefinitionId") REFERENCES alchimalia_schema."StoryEpicDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryEpicDefinitionTopic_StoryTopics_StoryTopicId" FOREIGN KEY ("StoryTopicId") REFERENCES alchimalia_schema."StoryTopics" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEpicDefinitionAgeGroup" (
    "StoryEpicDefinitionId" text NOT NULL,
    "StoryAgeGroupId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_StoryEpicDefinitionAgeGroup" PRIMARY KEY ("StoryEpicDefinitionId", "StoryAgeGroupId"),
    CONSTRAINT "FK_StoryEpicDefinitionAgeGroup_StoryEpicDefinitions_StoryEpicDefinitionId" FOREIGN KEY ("StoryEpicDefinitionId") REFERENCES alchimalia_schema."StoryEpicDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryEpicDefinitionAgeGroup_StoryAgeGroups_StoryAgeGroupId" FOREIGN KEY ("StoryAgeGroupId") REFERENCES alchimalia_schema."StoryAgeGroups" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_StoryEpicCraftTopic_StoryTopicId" ON alchimalia_schema."StoryEpicCraftTopic" ("StoryTopicId");
CREATE INDEX IF NOT EXISTS "IX_StoryEpicCraftAgeGroup_StoryAgeGroupId" ON alchimalia_schema."StoryEpicCraftAgeGroup" ("StoryAgeGroupId");
CREATE INDEX IF NOT EXISTS "IX_StoryEpicDefinitionTopic_StoryTopicId" ON alchimalia_schema."StoryEpicDefinitionTopic" ("StoryTopicId");
CREATE INDEX IF NOT EXISTS "IX_StoryEpicDefinitionAgeGroup_StoryAgeGroupId" ON alchimalia_schema."StoryEpicDefinitionAgeGroup" ("StoryAgeGroupId");
