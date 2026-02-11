-- Dialog branching + option rewards (idempotent)
-- Adds:
-- 1) BranchId on StoryCraftTiles / StoryTiles
-- 2) JumpToTileId + SetBranchId on StoryCraftDialogEdges / StoryDialogEdges
-- 3) Token reward tables for dialog options (craft + published)

-- 1) Tile branch tagging (draft + published)
ALTER TABLE alchimalia_schema."StoryCraftTiles"
    ADD COLUMN IF NOT EXISTS "BranchId" VARCHAR(100) NULL;

ALTER TABLE alchimalia_schema."StoryTiles"
    ADD COLUMN IF NOT EXISTS "BranchId" VARCHAR(100) NULL;

-- 2) Option-level jump/branch actions (draft + published)
ALTER TABLE alchimalia_schema."StoryCraftDialogEdges"
    ADD COLUMN IF NOT EXISTS "JumpToTileId" VARCHAR(100) NULL;

ALTER TABLE alchimalia_schema."StoryCraftDialogEdges"
    ADD COLUMN IF NOT EXISTS "SetBranchId" VARCHAR(100) NULL;

ALTER TABLE alchimalia_schema."StoryDialogEdges"
    ADD COLUMN IF NOT EXISTS "JumpToTileId" VARCHAR(100) NULL;

ALTER TABLE alchimalia_schema."StoryDialogEdges"
    ADD COLUMN IF NOT EXISTS "SetBranchId" VARCHAR(100) NULL;

-- 3) Draft dialog option tokens
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftDialogEdgeTokens"
(
    "Id" UUID PRIMARY KEY,
    "StoryCraftDialogEdgeId" UUID NOT NULL,
    "Type" VARCHAR(100) NOT NULL,
    "Value" VARCHAR(200) NOT NULL,
    "Quantity" INTEGER NOT NULL DEFAULT 1
);

CREATE INDEX IF NOT EXISTS "IX_StoryCraftDialogEdgeTokens_StoryCraftDialogEdgeId"
    ON alchimalia_schema."StoryCraftDialogEdgeTokens" ("StoryCraftDialogEdgeId");

ALTER TABLE alchimalia_schema."StoryCraftDialogEdgeTokens"
    DROP CONSTRAINT IF EXISTS "FK_StoryCraftDialogEdgeTokens_StoryCraftDialogEdges_StoryCraftDialogEdgeId";
ALTER TABLE alchimalia_schema."StoryCraftDialogEdgeTokens"
    ADD CONSTRAINT "FK_StoryCraftDialogEdgeTokens_StoryCraftDialogEdges_StoryCraftDialogEdgeId"
    FOREIGN KEY ("StoryCraftDialogEdgeId")
    REFERENCES alchimalia_schema."StoryCraftDialogEdges" ("Id")
    ON DELETE CASCADE;

-- 4) Published dialog option tokens
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDialogEdgeTokens"
(
    "Id" UUID PRIMARY KEY,
    "StoryDialogEdgeId" UUID NOT NULL,
    "Type" VARCHAR(100) NOT NULL,
    "Value" VARCHAR(200) NOT NULL,
    "Quantity" INTEGER NOT NULL DEFAULT 1
);

CREATE INDEX IF NOT EXISTS "IX_StoryDialogEdgeTokens_StoryDialogEdgeId"
    ON alchimalia_schema."StoryDialogEdgeTokens" ("StoryDialogEdgeId");

ALTER TABLE alchimalia_schema."StoryDialogEdgeTokens"
    DROP CONSTRAINT IF EXISTS "FK_StoryDialogEdgeTokens_StoryDialogEdges_StoryDialogEdgeId";
ALTER TABLE alchimalia_schema."StoryDialogEdgeTokens"
    ADD CONSTRAINT "FK_StoryDialogEdgeTokens_StoryDialogEdges_StoryDialogEdgeId"
    FOREIGN KEY ("StoryDialogEdgeId")
    REFERENCES alchimalia_schema."StoryDialogEdges" ("Id")
    ON DELETE CASCADE;
