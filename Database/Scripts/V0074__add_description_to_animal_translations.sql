-- Adds per-language description fields for LOI animals.
-- Required by Laboratory of Imagination editor (name + description per language).

ALTER TABLE IF EXISTS alchimalia_schema."AnimalCraftTranslations"
  ADD COLUMN IF NOT EXISTS "Description" character varying(4000);

ALTER TABLE IF EXISTS alchimalia_schema."AnimalDefinitionTranslations"
  ADD COLUMN IF NOT EXISTS "Description" character varying(4000);

