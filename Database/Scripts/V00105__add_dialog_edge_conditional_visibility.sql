-- Conditional visibility for dialog options (hideIfBranchSet, showOnlyIfBranchesSet)
-- Draft: StoryCraftDialogEdges
-- Published: StoryDialogEdges
-- ShowOnlyIfBranchesSet: JSON array of branch IDs stored as TEXT

ALTER TABLE alchimalia_schema."StoryCraftDialogEdges"
    ADD COLUMN IF NOT EXISTS "HideIfBranchSet" VARCHAR(100) NULL;

ALTER TABLE alchimalia_schema."StoryCraftDialogEdges"
    ADD COLUMN IF NOT EXISTS "ShowOnlyIfBranchesSet" TEXT NULL;

ALTER TABLE alchimalia_schema."StoryDialogEdges"
    ADD COLUMN IF NOT EXISTS "HideIfBranchSet" VARCHAR(100) NULL;

ALTER TABLE alchimalia_schema."StoryDialogEdges"
    ADD COLUMN IF NOT EXISTS "ShowOnlyIfBranchesSet" TEXT NULL;
