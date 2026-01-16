-- Migrates existing HeroDefinitions/Animals into new Craft/Definition tables
-- Idempotent inserts; does not modify legacy tables.
-- Story Heroes JSON -> DB handled separately (not possible in pure SQL).
-- Run date: 2026-01-16

BEGIN;

-- =========================
-- Tree of Heroes: Definitions
-- =========================
INSERT INTO "alchimalia_schema"."HeroDefinitionDefinitions" (
    "Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
    "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image",
    "Status", "PublishedByUserId", "CreatedAt", "UpdatedAt", "PublishedAtUtc"
)
SELECT
    hd."Id", hd."Type", hd."CourageCost", hd."CuriosityCost", hd."ThinkingCost", hd."CreativityCost", hd."SafetyCost",
    hd."PrerequisitesJson", hd."RewardsJson", hd."IsUnlocked", hd."PositionX", hd."PositionY", hd."Image",
    'published', NULL, COALESCE(hd."CreatedAt", now() at time zone 'utc'), COALESCE(hd."UpdatedAt", now() at time zone 'utc'), now() at time zone 'utc'
FROM "alchimalia_schema"."HeroDefinitions" hd
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionDefinitions" d WHERE d."Id" = hd."Id"
);

INSERT INTO "alchimalia_schema"."HeroDefinitionDefinitionTranslations" (
    "Id", "HeroDefinitionDefinitionId", "LanguageCode", "Name", "Description", "Story"
)
SELECT
    hdt."Id", hdt."HeroDefinitionId", hdt."LanguageCode", hdt."Name", hdt."Description", hdt."Story"
FROM "alchimalia_schema"."HeroDefinitionTranslations" hdt
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionDefinitionTranslations" dt
    WHERE dt."HeroDefinitionDefinitionId" = hdt."HeroDefinitionId" AND dt."LanguageCode" = hdt."LanguageCode"
);

-- =========================
-- Tree of Heroes: Crafts (published mirror)
-- =========================
INSERT INTO "alchimalia_schema"."HeroDefinitionCrafts" (
    "Id", "PublishedDefinitionId", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
    "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image",
    "Status", "CreatedByUserId", "ReviewedByUserId", "ReviewNotes", "CreatedAt", "UpdatedAt"
)
SELECT
    hd."Id", hd."Id", hd."Type", hd."CourageCost", hd."CuriosityCost", hd."ThinkingCost", hd."CreativityCost", hd."SafetyCost",
    hd."PrerequisitesJson", hd."RewardsJson", hd."IsUnlocked", hd."PositionX", hd."PositionY", hd."Image",
    'published', NULL, NULL, NULL, COALESCE(hd."CreatedAt", now() at time zone 'utc'), COALESCE(hd."UpdatedAt", now() at time zone 'utc')
FROM "alchimalia_schema"."HeroDefinitions" hd
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionCrafts" c WHERE c."Id" = hd."Id"
);

INSERT INTO "alchimalia_schema"."HeroDefinitionCraftTranslations" (
    "Id", "HeroDefinitionCraftId", "LanguageCode", "Name", "Description", "Story"
)
SELECT
    hdt."Id", hdt."HeroDefinitionId", hdt."LanguageCode", hdt."Name", hdt."Description", hdt."Story"
FROM "alchimalia_schema"."HeroDefinitionTranslations" hdt
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."HeroDefinitionCraftTranslations" ct
    WHERE ct."HeroDefinitionCraftId" = hdt."HeroDefinitionId" AND ct."LanguageCode" = hdt."LanguageCode"
);

-- =========================
-- LOI Animals: Definitions
-- =========================
INSERT INTO "alchimalia_schema"."AnimalDefinitions" (
    "Id", "Label", "Src", "IsHybrid", "RegionId", "Status", "PublishedByUserId", "CreatedAt", "UpdatedAt", "PublishedAtUtc"
)
SELECT
    a."Id", a."Label", a."Src", a."IsHybrid", a."RegionId", 'published', NULL,
    COALESCE(a."CreatedAt", now() at time zone 'utc'), COALESCE(a."UpdatedAt", now() at time zone 'utc'), now() at time zone 'utc'
FROM "alchimalia_schema"."Animals" a
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalDefinitions" d WHERE d."Id" = a."Id"
);

INSERT INTO "alchimalia_schema"."AnimalDefinitionTranslations" (
    "Id", "AnimalDefinitionId", "LanguageCode", "Label"
)
SELECT
    at."Id", at."AnimalId", at."LanguageCode", at."Label"
FROM "alchimalia_schema"."AnimalTranslations" at
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalDefinitionTranslations" dt
    WHERE dt."AnimalDefinitionId" = at."AnimalId" AND dt."LanguageCode" = at."LanguageCode"
);

INSERT INTO "alchimalia_schema"."AnimalDefinitionPartSupports" (
    "AnimalDefinitionId", "BodyPartKey"
)
SELECT
    aps."AnimalId", aps."PartKey"
FROM "alchimalia_schema"."AnimalPartSupports" aps
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalDefinitionPartSupports" dps
    WHERE dps."AnimalDefinitionId" = aps."AnimalId" AND dps."BodyPartKey" = aps."PartKey"
);

-- =========================
-- LOI Animals: Crafts (published mirror)
-- =========================
INSERT INTO "alchimalia_schema"."AnimalCrafts" (
    "Id", "PublishedDefinitionId", "Label", "Src", "IsHybrid", "RegionId",
    "Status", "CreatedByUserId", "ReviewedByUserId", "ReviewNotes", "CreatedAt", "UpdatedAt"
)
SELECT
    a."Id", a."Id", a."Label", a."Src", a."IsHybrid", a."RegionId",
    'published', NULL, NULL, NULL, COALESCE(a."CreatedAt", now() at time zone 'utc'), COALESCE(a."UpdatedAt", now() at time zone 'utc')
FROM "alchimalia_schema"."Animals" a
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalCrafts" c WHERE c."Id" = a."Id"
);

INSERT INTO "alchimalia_schema"."AnimalCraftTranslations" (
    "Id", "AnimalCraftId", "LanguageCode", "Label"
)
SELECT
    at."Id", at."AnimalId", at."LanguageCode", at."Label"
FROM "alchimalia_schema"."AnimalTranslations" at
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalCraftTranslations" ct
    WHERE ct."AnimalCraftId" = at."AnimalId" AND ct."LanguageCode" = at."LanguageCode"
);

INSERT INTO "alchimalia_schema"."AnimalCraftPartSupports" (
    "AnimalCraftId", "BodyPartKey"
)
SELECT
    aps."AnimalId", aps."PartKey"
FROM "alchimalia_schema"."AnimalPartSupports" aps
WHERE NOT EXISTS (
    SELECT 1 FROM "alchimalia_schema"."AnimalCraftPartSupports" cps
    WHERE cps."AnimalCraftId" = aps."AnimalId" AND cps."BodyPartKey" = aps."PartKey"
);

COMMIT;
