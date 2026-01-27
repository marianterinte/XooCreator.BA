-- Add newsletter subscription fields to AlchimaliaUsers table
-- Run date: 2025-01-25
-- Description: Adds columns for newsletter subscription functionality:
--              - IsNewsletterSubscribed: Boolean flag indicating if user is subscribed (default: true)
--              - NewsletterSubscribedAt: Timestamp when user subscribed (nullable)
--              - NewsletterUnsubscribedAt: Timestamp when user unsubscribed (nullable)

BEGIN;

-- Add IsNewsletterSubscribed column (boolean, default true - users are automatically subscribed)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'AlchimaliaUsers' 
        AND column_name = 'IsNewsletterSubscribed'
    ) THEN
        ALTER TABLE alchimalia_schema."AlchimaliaUsers" 
        ADD COLUMN "IsNewsletterSubscribed" boolean NOT NULL DEFAULT true;
        
        -- Add comment for documentation
        COMMENT ON COLUMN alchimalia_schema."AlchimaliaUsers"."IsNewsletterSubscribed" IS 
            'Newsletter subscription status - users are automatically subscribed by default';
    END IF;
END $$;

-- Add NewsletterSubscribedAt column (nullable timestamp)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'AlchimaliaUsers' 
        AND column_name = 'NewsletterSubscribedAt'
    ) THEN
        ALTER TABLE alchimalia_schema."AlchimaliaUsers" 
        ADD COLUMN "NewsletterSubscribedAt" timestamp with time zone NULL;
        
        -- Add comment for documentation
        COMMENT ON COLUMN alchimalia_schema."AlchimaliaUsers"."NewsletterSubscribedAt" IS 
            'Date when user subscribed to newsletter (null if never subscribed)';
    END IF;
END $$;

-- Add NewsletterUnsubscribedAt column (nullable timestamp)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'AlchimaliaUsers' 
        AND column_name = 'NewsletterUnsubscribedAt'
    ) THEN
        ALTER TABLE alchimalia_schema."AlchimaliaUsers" 
        ADD COLUMN "NewsletterUnsubscribedAt" timestamp with time zone NULL;
        
        -- Add comment for documentation
        COMMENT ON COLUMN alchimalia_schema."AlchimaliaUsers"."NewsletterUnsubscribedAt" IS 
            'Date when user unsubscribed from newsletter (null if currently subscribed)';
    END IF;
END $$;

-- Set NewsletterSubscribedAt for existing users who are subscribed (default true)
-- This ensures existing users have a subscription date
UPDATE alchimalia_schema."AlchimaliaUsers"
SET "NewsletterSubscribedAt" = "CreatedAt"
WHERE "IsNewsletterSubscribed" = true 
  AND "NewsletterSubscribedAt" IS NULL;

COMMIT;
