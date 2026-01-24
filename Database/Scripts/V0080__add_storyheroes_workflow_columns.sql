-- Add workflow and review columns to StoryHeroes to align with EF model and
-- migration 20260119130658_AddEpicTopicsAndAgeGroups.
-- Fixes: column s.CreatedByUserId does not exist (42703) when calling
-- GET /api/{locale}/bestiary?bestiaryType=discovery.
--
-- The C# entity StoryHero and EF migration add these columns; raw SQL scripts
-- (V0001, etc.) never did, so DBs deployed only via DbScriptRunner are missing them.

BEGIN;

-- Rename old column if it exists (V0001 used UnlockConditionJson; EF model uses UnlockConditionsJson)
DO $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'StoryHeroes' AND column_name = 'UnlockConditionJson'
  ) AND NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'StoryHeroes' AND column_name = 'UnlockConditionsJson'
  ) THEN
    ALTER TABLE alchimalia_schema."StoryHeroes" RENAME COLUMN "UnlockConditionJson" TO "UnlockConditionsJson";
  END IF;
END $$;

-- Drop column removed by EF migration (model no longer has IsActive)
ALTER TABLE alchimalia_schema."StoryHeroes" DROP COLUMN IF EXISTS "IsActive";

-- Add workflow/review columns expected by StoryHero entity
ALTER TABLE alchimalia_schema."StoryHeroes"
  ADD COLUMN IF NOT EXISTS "CreatedByUserId" uuid,
  ADD COLUMN IF NOT EXISTS "ReviewedByUserId" uuid,
  ADD COLUMN IF NOT EXISTS "ReviewNotes" character varying(2000),
  ADD COLUMN IF NOT EXISTS "Status" character varying(50) NOT NULL DEFAULT 'draft',
  ADD COLUMN IF NOT EXISTS "Version" integer NOT NULL DEFAULT 1;

-- Indexes used by EF (Id+Status, Status) – ignore if already present
CREATE INDEX IF NOT EXISTS "IX_StoryHeroes_Status"
  ON alchimalia_schema."StoryHeroes" ("Status");
CREATE INDEX IF NOT EXISTS "IX_StoryHeroes_Id_Status"
  ON alchimalia_schema."StoryHeroes" ("Id", "Status");

COMMIT;
