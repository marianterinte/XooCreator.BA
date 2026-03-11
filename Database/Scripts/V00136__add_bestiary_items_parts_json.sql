-- BestiaryItems: optional PartsJson for full generative combination (all body parts)
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'BestiaryItems' AND column_name = 'PartsJson'
  ) THEN
    ALTER TABLE alchimalia_schema."BestiaryItems" ADD COLUMN "PartsJson" character varying(2000) NULL;
  END IF;
END $$;
