-- Add VoiceName column to StoryAudioExportJobs table
-- Optional Gemini TTS voice override (e.g. Sulafat, Zephyr). When null, uses server default from config.

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'alchimalia_schema'
        AND table_name = 'StoryAudioExportJobs'
        AND column_name = 'VoiceName'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryAudioExportJobs"
        ADD COLUMN "VoiceName" VARCHAR(64) NULL;
    END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."StoryAudioExportJobs"."VoiceName" IS 'Optional Gemini TTS voice name override (e.g. Sulafat, Zephyr). When null, uses server default from config';
