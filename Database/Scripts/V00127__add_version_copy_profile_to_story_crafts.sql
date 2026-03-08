-- Add version copy profile to StoryCrafts for publish-time language merge.
-- When VersionCopyLanguageMode='selected', publish updates only selected languages; unselected remain untouched.
-- Idempotent: safe to run multiple times.

ALTER TABLE alchimalia_schema."StoryCrafts"
    ADD COLUMN IF NOT EXISTS "VersionCopyLanguageMode" VARCHAR(16) NULL;

ALTER TABLE alchimalia_schema."StoryCrafts"
    ADD COLUMN IF NOT EXISTS "VersionCopySelectedLanguagesJson" VARCHAR(2000) NULL;

COMMENT ON COLUMN alchimalia_schema."StoryCrafts"."VersionCopyLanguageMode" IS 'all or selected. When selected, publish updates only selected languages';
COMMENT ON COLUMN alchimalia_schema."StoryCrafts"."VersionCopySelectedLanguagesJson" IS 'JSON array of lang codes when VersionCopyLanguageMode=selected';
