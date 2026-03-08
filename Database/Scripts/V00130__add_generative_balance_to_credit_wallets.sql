-- Add GenerativeBalance for Supporter Pack LOI credits (consumed per generative animal).
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'CreditWallets' AND column_name = 'GenerativeBalance'
  ) THEN
    ALTER TABLE alchimalia_schema."CreditWallets" ADD COLUMN "GenerativeBalance" double precision NOT NULL DEFAULT 0;
  END IF;
END $$;

COMMENT ON COLUMN alchimalia_schema."CreditWallets"."GenerativeBalance" IS 'Supporter Pack generative LOI credits; increased on grant, consumed per generation';
