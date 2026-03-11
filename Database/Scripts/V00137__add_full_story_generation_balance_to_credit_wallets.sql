-- Add FullStoryGenerationBalance for Supporter Pack full story generation credits (consumed per private story).
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'CreditWallets' AND column_name = 'FullStoryGenerationBalance'
  ) THEN
    ALTER TABLE alchimalia_schema."CreditWallets" ADD COLUMN "FullStoryGenerationBalance" double precision NOT NULL DEFAULT 0;
  END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."CreditWallets"."FullStoryGenerationBalance" IS 'Supporter Pack full story generation credits; increased on grant, consumed per private story generation';
