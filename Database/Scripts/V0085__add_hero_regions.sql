-- Hero–Region many-to-many: associate heroes with published regions.
-- Junction tables: draft (EpicHeroCraftRegion) and published (EpicHeroDefinitionRegion).

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicHeroCraftRegion" (
    "EpicHeroCraftId" text NOT NULL,
    "RegionId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
    CONSTRAINT "PK_EpicHeroCraftRegion" PRIMARY KEY ("EpicHeroCraftId", "RegionId"),
    CONSTRAINT "FK_EpicHeroCraftRegion_EpicHeroCrafts_EpicHeroCraftId" FOREIGN KEY ("EpicHeroCraftId") REFERENCES alchimalia_schema."EpicHeroCrafts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicHeroCraftRegion_StoryRegionDefinitions_RegionId" FOREIGN KEY ("RegionId") REFERENCES alchimalia_schema."StoryRegionDefinitions" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS alchimalia_schema."EpicHeroDefinitionRegion" (
    "EpicHeroDefinitionId" text NOT NULL,
    "RegionId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
    CONSTRAINT "PK_EpicHeroDefinitionRegion" PRIMARY KEY ("EpicHeroDefinitionId", "RegionId"),
    CONSTRAINT "FK_EpicHeroDefinitionRegion_EpicHeroDefinitions_EpicHeroDefinitionId" FOREIGN KEY ("EpicHeroDefinitionId") REFERENCES alchimalia_schema."EpicHeroDefinitions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EpicHeroDefinitionRegion_StoryRegionDefinitions_RegionId" FOREIGN KEY ("RegionId") REFERENCES alchimalia_schema."StoryRegionDefinitions" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_EpicHeroCraftRegion_RegionId" ON alchimalia_schema."EpicHeroCraftRegion" ("RegionId");
CREATE INDEX IF NOT EXISTS "IX_EpicHeroDefinitionRegion_RegionId" ON alchimalia_schema."EpicHeroDefinitionRegion" ("RegionId");
