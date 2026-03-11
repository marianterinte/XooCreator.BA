-- Add IsPrivateStoryGeneration flag to StoryAIGenerateJobs (your-story flow; different request payload and credit consumption).
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'StoryAIGenerateJobs' AND column_name = 'IsPrivateStoryGeneration'
  ) THEN
    ALTER TABLE alchimalia_schema."StoryAIGenerateJobs" ADD COLUMN "IsPrivateStoryGeneration" boolean NOT NULL DEFAULT false;
  END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."StoryAIGenerateJobs"."IsPrivateStoryGeneration" IS 'When true, job uses GeneratePrivateStoryRequest and persists to StoryDefinition with IsPrivate; consumes full-story credits';
