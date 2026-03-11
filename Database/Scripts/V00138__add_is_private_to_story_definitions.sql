-- Add IsPrivate flag to StoryDefinitions (your-story private stories; never shown in marketplace).
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'StoryDefinitions' AND column_name = 'IsPrivate'
  ) THEN
    ALTER TABLE alchimalia_schema."StoryDefinitions" ADD COLUMN "IsPrivate" boolean NOT NULL DEFAULT false;
  END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."StoryDefinitions"."IsPrivate" IS 'When true, story is private to owner (your-story); excluded from marketplace';
