-- Add granular version copy profile to StoryVersionJobs.
-- When LightChanges is false: CopyImages, CopyAudio, CopyVideo control which asset types to copy;
-- LanguageMode + SelectedLanguagesJson filter by language.
-- Idempotent: safe to run multiple times.

ALTER TABLE alchimalia_schema."StoryVersionJobs"
    ADD COLUMN IF NOT EXISTS "CopyImages" BOOLEAN NULL;

ALTER TABLE alchimalia_schema."StoryVersionJobs"
    ADD COLUMN IF NOT EXISTS "CopyAudio" BOOLEAN NULL;

ALTER TABLE alchimalia_schema."StoryVersionJobs"
    ADD COLUMN IF NOT EXISTS "CopyVideo" BOOLEAN NULL;

ALTER TABLE alchimalia_schema."StoryVersionJobs"
    ADD COLUMN IF NOT EXISTS "LanguageMode" VARCHAR(16) NULL;

ALTER TABLE alchimalia_schema."StoryVersionJobs"
    ADD COLUMN IF NOT EXISTS "SelectedLanguagesJson" VARCHAR(2000) NULL;

COMMENT ON COLUMN alchimalia_schema."StoryVersionJobs"."CopyImages" IS 'Copy cover+tile images when LightChanges is false. Null=legacy (treat as true)';
COMMENT ON COLUMN alchimalia_schema."StoryVersionJobs"."CopyAudio" IS 'Copy audio when LightChanges is false. Null=legacy (treat as true)';
COMMENT ON COLUMN alchimalia_schema."StoryVersionJobs"."CopyVideo" IS 'Copy video when LightChanges is false. Null=legacy (treat as true)';
COMMENT ON COLUMN alchimalia_schema."StoryVersionJobs"."LanguageMode" IS 'all or selected. Null=legacy (treat as all)';
COMMENT ON COLUMN alchimalia_schema."StoryVersionJobs"."SelectedLanguagesJson" IS 'JSON array of lang codes when LanguageMode=selected. Null/empty=none';
