-- Co-authors for Epic (Craft + Definition) and Story (Craft + Definition)
-- Co-author: either UserId (Alchimalia user) or DisplayName (free text). Id is PK for ordering and multiple free-text entries.

-- Epic Craft CoAuthors
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEpicCraftCoAuthors" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "StoryEpicCraftId" text NOT NULL,
    "UserId" uuid NULL,
    "DisplayName" character varying(500) NULL,
    "SortOrder" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_StoryEpicCraftCoAuthors" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryEpicCraftCoAuthors_StoryEpicCrafts_StoryEpicCraftId" FOREIGN KEY ("StoryEpicCraftId") REFERENCES alchimalia_schema."StoryEpicCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryEpicCraftCoAuthors_AlchimaliaUsers_UserId" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryEpicCraftCoAuthors_StoryEpicCraftId" ON alchimalia_schema."StoryEpicCraftCoAuthors" ("StoryEpicCraftId");
CREATE INDEX IF NOT EXISTS "IX_StoryEpicCraftCoAuthors_UserId" ON alchimalia_schema."StoryEpicCraftCoAuthors" ("UserId") WHERE "UserId" IS NOT NULL;

-- Epic Definition CoAuthors (published)
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEpicDefinitionCoAuthors" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "StoryEpicDefinitionId" text NOT NULL,
    "UserId" uuid NULL,
    "DisplayName" character varying(500) NULL,
    "SortOrder" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_StoryEpicDefinitionCoAuthors" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryEpicDefinitionCoAuthors_StoryEpicDefinitions_StoryEpicDefinitionId" FOREIGN KEY ("StoryEpicDefinitionId") REFERENCES alchimalia_schema."StoryEpicDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryEpicDefinitionCoAuthors_AlchimaliaUsers_UserId" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryEpicDefinitionCoAuthors_StoryEpicDefinitionId" ON alchimalia_schema."StoryEpicDefinitionCoAuthors" ("StoryEpicDefinitionId");
CREATE INDEX IF NOT EXISTS "IX_StoryEpicDefinitionCoAuthors_UserId" ON alchimalia_schema."StoryEpicDefinitionCoAuthors" ("UserId") WHERE "UserId" IS NOT NULL;

-- Story Craft CoAuthors
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryCraftCoAuthors" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "StoryCraftId" uuid NOT NULL,
    "UserId" uuid NULL,
    "DisplayName" character varying(500) NULL,
    "SortOrder" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_StoryCraftCoAuthors" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCraftCoAuthors_StoryCrafts_StoryCraftId" FOREIGN KEY ("StoryCraftId") REFERENCES alchimalia_schema."StoryCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryCraftCoAuthors_AlchimaliaUsers_UserId" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryCraftCoAuthors_StoryCraftId" ON alchimalia_schema."StoryCraftCoAuthors" ("StoryCraftId");
CREATE INDEX IF NOT EXISTS "IX_StoryCraftCoAuthors_UserId" ON alchimalia_schema."StoryCraftCoAuthors" ("UserId") WHERE "UserId" IS NOT NULL;

-- Story Definition CoAuthors (published)
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryDefinitionCoAuthors" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "StoryDefinitionId" uuid NOT NULL,
    "UserId" uuid NULL,
    "DisplayName" character varying(500) NULL,
    "SortOrder" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_StoryDefinitionCoAuthors" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryDefinitionCoAuthors_StoryDefinitions_StoryDefinitionId" FOREIGN KEY ("StoryDefinitionId") REFERENCES alchimalia_schema."StoryDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryDefinitionCoAuthors_AlchimaliaUsers_UserId" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_StoryDefinitionCoAuthors_StoryDefinitionId" ON alchimalia_schema."StoryDefinitionCoAuthors" ("StoryDefinitionId");
CREATE INDEX IF NOT EXISTS "IX_StoryDefinitionCoAuthors_UserId" ON alchimalia_schema."StoryDefinitionCoAuthors" ("UserId") WHERE "UserId" IS NOT NULL;
