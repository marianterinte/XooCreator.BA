-- Add child age preferences to AlchimaliaUsers table
-- Run date: 2025-01-XX
-- Description: Adds columns for parent dashboard functionality:
--              - SelectedAgeGroupIds: Array of age group IDs selected by parent (e.g., ["preschool_3_5", "early_school_6_8"])
--              - AutoFilterStoriesByAge: Boolean flag to enable automatic filtering of stories by selected age groups in marketplace

BEGIN;

-- Add SelectedAgeGroupIds column (nullable text array, stores selected age group IDs)
-- Using IF NOT EXISTS pattern for idempotency
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'AlchimaliaUsers' 
        AND column_name = 'SelectedAgeGroupIds'
    ) THEN
        ALTER TABLE alchimalia_schema."AlchimaliaUsers" 
        ADD COLUMN "SelectedAgeGroupIds" text[] NULL;
        
        -- Add comment for documentation
        COMMENT ON COLUMN alchimalia_schema."AlchimaliaUsers"."SelectedAgeGroupIds" IS 
            'Array of age group IDs selected by parent for filtering stories. Nullable, can be set in parent dashboard.';
    END IF;
END $$;

-- Add AutoFilterStoriesByAge column (boolean, default false)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'alchimalia_schema' 
        AND table_name = 'AlchimaliaUsers' 
        AND column_name = 'AutoFilterStoriesByAge'
    ) THEN
        ALTER TABLE alchimalia_schema."AlchimaliaUsers" 
        ADD COLUMN "AutoFilterStoriesByAge" boolean NOT NULL DEFAULT false;
        
        -- Add comment for documentation
        COMMENT ON COLUMN alchimalia_schema."AlchimaliaUsers"."AutoFilterStoriesByAge" IS 
            'If true, marketplace will automatically filter stories to show only those matching selected age groups.';
    END IF;
END $$;

COMMIT;

