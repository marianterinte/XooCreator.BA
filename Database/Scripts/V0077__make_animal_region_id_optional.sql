-- Make RegionId optional (nullable) in AnimalCrafts and AnimalDefinitions tables
-- This allows animals to exist without being associated with a specific region

-- AnimalCrafts table
ALTER TABLE "alchimalia_schema"."AnimalCrafts" 
    ALTER COLUMN "RegionId" DROP NOT NULL;

-- AnimalDefinitions table  
ALTER TABLE "alchimalia_schema"."AnimalDefinitions"
    ALTER COLUMN "RegionId" DROP NOT NULL;

-- Note: Foreign key constraints remain, but now allow NULL values
-- This means animals can optionally be associated with a region, but if a region is specified,
-- it must be a valid region ID from the Regions table
