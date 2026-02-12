-- Dialog tile support (craft + published definition)

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogParticipants"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftId" UUID NOT NULL,
    "HeroId" VARCHAR(100) NOT NULL,
    "SortOrder" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryCraftDialogParticipants_StoryCraftId_HeroId"
    ON alchimalia_schema."StoryCraftDialogParticipants" ("StoryCraftId", "HeroId");

ALTER TABLE alchimalia_schema."StoryCraftDialogParticipants"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogParticipants_StoryCrafts_StoryCraftId";
ALTER TABLE alchimalia_schema."StoryCraftDialogParticipants"
    ADD CONSTRAINT "FK_StoryCraftDialogParticipants_StoryCrafts_StoryCraftId"
    FOREIGN KEY ("StoryCraftId") REFERENCES alchimalia_schema."StoryCrafts" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogTiles"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftId" UUID NOT NULL,
    "StoryCraftTileId" UUID NOT NULL,
    "RootNodeId" VARCHAR(100) NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryCraftDialogTiles_StoryCraftTileId"
    ON alchimalia_schema."StoryCraftDialogTiles" ("StoryCraftTileId");

ALTER TABLE alchimalia_schema."StoryCraftDialogTiles"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogTiles_StoryCrafts_StoryCraftId";
ALTER TABLE alchimalia_schema."StoryCraftDialogTiles"
    ADD CONSTRAINT "FK_StoryCraftDialogTiles_StoryCrafts_StoryCraftId"
    FOREIGN KEY ("StoryCraftId") REFERENCES alchimalia_schema."StoryCrafts" ("Id") ON DELETE CASCADE;

ALTER TABLE alchimalia_schema."StoryCraftDialogTiles"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogTiles_StoryCraftTiles_StoryCraftTileId";
ALTER TABLE alchimalia_schema."StoryCraftDialogTiles"
    ADD CONSTRAINT "FK_StoryCraftDialogTiles_StoryCraftTiles_StoryCraftTileId"
    FOREIGN KEY ("StoryCraftTileId") REFERENCES alchimalia_schema."StoryCraftTiles" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogNodes"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftDialogTileId" UUID NOT NULL,
    "NodeId" VARCHAR(100) NOT NULL,
    "SpeakerType" VARCHAR(20) NOT NULL DEFAULT 'reader',
    "SpeakerHeroId" VARCHAR(100) NULL,
    "SortOrder" INTEGER NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryCraftDialogNodes_StoryCraftDialogTileId_NodeId"
    ON alchimalia_schema."StoryCraftDialogNodes" ("StoryCraftDialogTileId", "NodeId");

ALTER TABLE alchimalia_schema."StoryCraftDialogNodes"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogNodes_StoryCraftDialogTiles_StoryCraftDialogTileId";
ALTER TABLE alchimalia_schema."StoryCraftDialogNodes"
    ADD CONSTRAINT "FK_StoryCraftDialogNodes_StoryCraftDialogTiles_StoryCraftDialogTileId"
    FOREIGN KEY ("StoryCraftDialogTileId") REFERENCES alchimalia_schema."StoryCraftDialogTiles" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogNodeTranslations"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftDialogNodeId" UUID NOT NULL,
    "LanguageCode" VARCHAR(10) NOT NULL,
    "Text" TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryCraftDialogNodeTranslations_Node_Language"
    ON alchimalia_schema."StoryCraftDialogNodeTranslations" ("StoryCraftDialogNodeId", "LanguageCode");

ALTER TABLE alchimalia_schema."StoryCraftDialogNodeTranslations"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogNodeTranslations_StoryCraftDialogNodes_StoryCraftDialogNodeId";
ALTER TABLE alchimalia_schema."StoryCraftDialogNodeTranslations"
    ADD CONSTRAINT "FK_StoryCraftDialogNodeTranslations_StoryCraftDialogNodes_StoryCraftDialogNodeId"
    FOREIGN KEY ("StoryCraftDialogNodeId") REFERENCES alchimalia_schema."StoryCraftDialogNodes" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogEdges"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftDialogNodeId" UUID NOT NULL,
    "EdgeId" VARCHAR(100) NOT NULL,
    "ToNodeId" VARCHAR(100) NOT NULL,
    "OptionOrder" INTEGER NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryCraftDialogEdges_Node_EdgeId"
    ON alchimalia_schema."StoryCraftDialogEdges" ("StoryCraftDialogNodeId", "EdgeId");

ALTER TABLE alchimalia_schema."StoryCraftDialogEdges"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogEdges_StoryCraftDialogNodes_StoryCraftDialogNodeId";
ALTER TABLE alchimalia_schema."StoryCraftDialogEdges"
    ADD CONSTRAINT "FK_StoryCraftDialogEdges_StoryCraftDialogNodes_StoryCraftDialogNodeId"
    FOREIGN KEY ("StoryCraftDialogNodeId") REFERENCES alchimalia_schema."StoryCraftDialogNodes" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogEdgeTranslations"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftDialogEdgeId" UUID NOT NULL,
    "LanguageCode" VARCHAR(10) NOT NULL,
    "OptionText" TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryCraftDialogEdgeTranslations_Edge_Language"
    ON alchimalia_schema."StoryCraftDialogEdgeTranslations" ("StoryCraftDialogEdgeId", "LanguageCode");

ALTER TABLE alchimalia_schema."StoryCraftDialogEdgeTranslations"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogEdgeTranslations_StoryCraftDialogEdges_StoryCraftDialogEdgeId";
ALTER TABLE alchimalia_schema."StoryCraftDialogEdgeTranslations"
    ADD CONSTRAINT "FK_StoryCraftDialogEdgeTranslations_StoryCraftDialogEdges_StoryCraftDialogEdgeId"
    FOREIGN KEY ("StoryCraftDialogEdgeId") REFERENCES alchimalia_schema."StoryCraftDialogEdges" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogParticipants"
(
    "Id" UUID PRIMARY KEY,
    "StoryDefinitionId" UUID NOT NULL,
    "HeroId" VARCHAR(100) NOT NULL,
    "SortOrder" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryDialogParticipants_StoryDefinitionId_HeroId"
    ON alchimalia_schema."StoryDialogParticipants" ("StoryDefinitionId", "HeroId");

ALTER TABLE alchimalia_schema."StoryDialogParticipants"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogParticipants_StoryDefinitions_StoryDefinitionId";
ALTER TABLE alchimalia_schema."StoryDialogParticipants"
    ADD CONSTRAINT "FK_StoryDialogParticipants_StoryDefinitions_StoryDefinitionId"
    FOREIGN KEY ("StoryDefinitionId") REFERENCES alchimalia_schema."StoryDefinitions" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogTiles"
(
    "Id" UUID PRIMARY KEY,
    "StoryDefinitionId" UUID NOT NULL,
    "StoryTileId" UUID NOT NULL,
    "RootNodeId" VARCHAR(100) NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryDialogTiles_StoryTileId"
    ON alchimalia_schema."StoryDialogTiles" ("StoryTileId");

ALTER TABLE alchimalia_schema."StoryDialogTiles"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogTiles_StoryDefinitions_StoryDefinitionId";
ALTER TABLE alchimalia_schema."StoryDialogTiles"
    ADD CONSTRAINT "FK_StoryDialogTiles_StoryDefinitions_StoryDefinitionId"
    FOREIGN KEY ("StoryDefinitionId") REFERENCES alchimalia_schema."StoryDefinitions" ("Id") ON DELETE CASCADE;

ALTER TABLE alchimalia_schema."StoryDialogTiles"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogTiles_StoryTiles_StoryTileId";
ALTER TABLE alchimalia_schema."StoryDialogTiles"
    ADD CONSTRAINT "FK_StoryDialogTiles_StoryTiles_StoryTileId"
    FOREIGN KEY ("StoryTileId") REFERENCES alchimalia_schema."StoryTiles" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogNodes"
(
    "Id" UUID PRIMARY KEY,
    "StoryDialogTileId" UUID NOT NULL,
    "NodeId" VARCHAR(100) NOT NULL,
    "SpeakerType" VARCHAR(20) NOT NULL DEFAULT 'reader',
    "SpeakerHeroId" VARCHAR(100) NULL,
    "SortOrder" INTEGER NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryDialogNodes_StoryDialogTileId_NodeId"
    ON alchimalia_schema."StoryDialogNodes" ("StoryDialogTileId", "NodeId");

ALTER TABLE alchimalia_schema."StoryDialogNodes"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogNodes_StoryDialogTiles_StoryDialogTileId";
ALTER TABLE alchimalia_schema."StoryDialogNodes"
    ADD CONSTRAINT "FK_StoryDialogNodes_StoryDialogTiles_StoryDialogTileId"
    FOREIGN KEY ("StoryDialogTileId") REFERENCES alchimalia_schema."StoryDialogTiles" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogNodeTranslations"
(
    "Id" UUID PRIMARY KEY,
    "StoryDialogNodeId" UUID NOT NULL,
    "LanguageCode" VARCHAR(10) NOT NULL,
    "Text" TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryDialogNodeTranslations_Node_Language"
    ON alchimalia_schema."StoryDialogNodeTranslations" ("StoryDialogNodeId", "LanguageCode");

ALTER TABLE alchimalia_schema."StoryDialogNodeTranslations"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogNodeTranslations_StoryDialogNodes_StoryDialogNodeId";
ALTER TABLE alchimalia_schema."StoryDialogNodeTranslations"
    ADD CONSTRAINT "FK_StoryDialogNodeTranslations_StoryDialogNodes_StoryDialogNodeId"
    FOREIGN KEY ("StoryDialogNodeId") REFERENCES alchimalia_schema."StoryDialogNodes" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogEdges"
(
    "Id" UUID PRIMARY KEY,
    "StoryDialogNodeId" UUID NOT NULL,
    "EdgeId" VARCHAR(100) NOT NULL,
    "ToNodeId" VARCHAR(100) NOT NULL,
    "OptionOrder" INTEGER NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryDialogEdges_Node_EdgeId"
    ON alchimalia_schema."StoryDialogEdges" ("StoryDialogNodeId", "EdgeId");

ALTER TABLE alchimalia_schema."StoryDialogEdges"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogEdges_StoryDialogNodes_StoryDialogNodeId";
ALTER TABLE alchimalia_schema."StoryDialogEdges"
    ADD CONSTRAINT "FK_StoryDialogEdges_StoryDialogNodes_StoryDialogNodeId"
    FOREIGN KEY ("StoryDialogNodeId") REFERENCES alchimalia_schema."StoryDialogNodes" ("Id") ON DELETE CASCADE;

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogEdgeTranslations"
(
    "Id" UUID PRIMARY KEY,
    "StoryDialogEdgeId" UUID NOT NULL,
    "LanguageCode" VARCHAR(10) NOT NULL,
    "OptionText" TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryDialogEdgeTranslations_Edge_Language"
    ON alchimalia_schema."StoryDialogEdgeTranslations" ("StoryDialogEdgeId", "LanguageCode");

ALTER TABLE alchimalia_schema."StoryDialogEdgeTranslations"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogEdgeTranslations_StoryDialogEdges_StoryDialogEdgeId";
ALTER TABLE alchimalia_schema."StoryDialogEdgeTranslations"
    ADD CONSTRAINT "FK_StoryDialogEdgeTranslations_StoryDialogEdges_StoryDialogEdgeId"
    FOREIGN KEY ("StoryDialogEdgeId") REFERENCES alchimalia_schema."StoryDialogEdges" ("Id") ON DELETE CASCADE;
