-- Audio URL per language for dialog replica (node) and option (edge)

ALTER TABLE alchimalia_schema."StoryCraftDialogNodeTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" VARCHAR(500) NULL;

ALTER TABLE alchimalia_schema."StoryCraftDialogEdgeTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" VARCHAR(500) NULL;

ALTER TABLE alchimalia_schema."StoryDialogNodeTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" VARCHAR(500) NULL;

ALTER TABLE alchimalia_schema."StoryDialogEdgeTranslations"
    ADD COLUMN IF NOT EXISTS "AudioUrl" VARCHAR(500) NULL;
