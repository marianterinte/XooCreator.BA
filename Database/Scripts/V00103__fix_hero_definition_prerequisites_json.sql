-- Normalize PrerequisitesJson from single-quoted string to JSON array for SafeParsePrerequisites.
-- Seed uses '"alchimalia_hero"' and '"hero_wise_owl"'; backend expects ["alchimalia_hero"] and ["hero_wise_owl"].

BEGIN;

UPDATE "alchimalia_schema"."HeroDefinitionDefinitions"
SET "PrerequisitesJson" = '["' || REPLACE(TRIM(BOTH '"' FROM "PrerequisitesJson"), '"', '') || '"]'
WHERE "PrerequisitesJson" IS NOT NULL
  AND "PrerequisitesJson" != ''
  AND "PrerequisitesJson" NOT LIKE '[%'
  AND "PrerequisitesJson" LIKE '"%"'
  AND (LENGTH("PrerequisitesJson") - LENGTH(REPLACE("PrerequisitesJson", ',', '')) = 0);

UPDATE "alchimalia_schema"."HeroDefinitionCrafts"
SET "PrerequisitesJson" = '["' || REPLACE(TRIM(BOTH '"' FROM "PrerequisitesJson"), '"', '') || '"]'
WHERE "PrerequisitesJson" IS NOT NULL
  AND "PrerequisitesJson" != ''
  AND "PrerequisitesJson" NOT LIKE '[%'
  AND "PrerequisitesJson" LIKE '"%"'
  AND (LENGTH("PrerequisitesJson") - LENGTH(REPLACE("PrerequisitesJson", ',', '')) = 0);

COMMIT;
