-- Add TtsModel column to StoryAudioExportJobs table
-- Optional TTS model override (e.g. gemini-2.5-flash-preview-tts, gemini-2.5-pro-tts). When null, uses server default.

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'alchimalia_schema'
        AND table_name = 'StoryAudioExportJobs'
        AND column_name = 'TtsModel'
    ) THEN
        ALTER TABLE alchimalia_schema."StoryAudioExportJobs"
        ADD COLUMN "TtsModel" VARCHAR(128) NULL;
    END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."StoryAudioExportJobs"."TtsModel" IS 'Optional TTS model override (e.g. gemini-2.5-flash-preview-tts, gemini-2.5-pro-tts). When null, uses server default from config';
