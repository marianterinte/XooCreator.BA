-- Adds story-to-story unlock support for Story Epic unlock rules
-- Run date: 2025-12-23
-- Description: Adds nullable ToStoryId to unlock rules tables (craft + definition).

BEGIN;

ALTER TABLE IF EXISTS "alchimalia_schema"."StoryEpicCraftUnlockRules"
    ADD COLUMN IF NOT EXISTS "ToStoryId" character varying(100);

ALTER TABLE IF EXISTS "alchimalia_schema"."StoryEpicDefinitionUnlockRules"
    ADD COLUMN IF NOT EXISTS "ToStoryId" character varying(100);

-- Optional indexes to speed up targeting lookups
CREATE INDEX IF NOT EXISTS "IX_StoryEpicCraftUnlockRules_EpicId_ToStoryId"
    ON "alchimalia_schema"."StoryEpicCraftUnlockRules" ("EpicId", "ToStoryId")
    WHERE "ToStoryId" IS NOT NULL;

CREATE INDEX IF NOT EXISTS "IX_StoryEpicDefinitionUnlockRules_EpicId_ToStoryId"
    ON "alchimalia_schema"."StoryEpicDefinitionUnlockRules" ("EpicId", "ToStoryId")
    WHERE "ToStoryId" IS NOT NULL;

COMMIT;


