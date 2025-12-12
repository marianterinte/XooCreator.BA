-- Adds versioning and review workflow fields to StoryEpics
-- Similar to StoryCraft: Version, BaseVersion, LastDraftVersion, and review workflow fields

BEGIN;

-- Add versioning fields
ALTER TABLE "alchimalia_schema"."StoryEpics"
    ADD COLUMN IF NOT EXISTS "Version" integer NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS "BaseVersion" integer NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS "LastDraftVersion" integer NOT NULL DEFAULT 0;

-- Add review workflow fields
ALTER TABLE "alchimalia_schema"."StoryEpics"
    ADD COLUMN IF NOT EXISTS "AssignedReviewerUserId" uuid NULL,
    ADD COLUMN IF NOT EXISTS "ReviewNotes" text NULL,
    ADD COLUMN IF NOT EXISTS "ReviewStartedAt" timestamp with time zone NULL,
    ADD COLUMN IF NOT EXISTS "ReviewEndedAt" timestamp with time zone NULL,
    ADD COLUMN IF NOT EXISTS "ReviewedByUserId" uuid NULL,
    ADD COLUMN IF NOT EXISTS "ApprovedByUserId" uuid NULL;

-- Add foreign key constraints for reviewer fields
ALTER TABLE "alchimalia_schema"."StoryEpics"
    ADD CONSTRAINT IF NOT EXISTS "FK_StoryEpics_AlchimaliaUsers_AssignedReviewerUserId"
        FOREIGN KEY ("AssignedReviewerUserId") 
        REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") 
        ON DELETE SET NULL;

ALTER TABLE "alchimalia_schema"."StoryEpics"
    ADD CONSTRAINT IF NOT EXISTS "FK_StoryEpics_AlchimaliaUsers_ReviewedByUserId"
        FOREIGN KEY ("ReviewedByUserId") 
        REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") 
        ON DELETE SET NULL;

ALTER TABLE "alchimalia_schema"."StoryEpics"
    ADD CONSTRAINT IF NOT EXISTS "FK_StoryEpics_AlchimaliaUsers_ApprovedByUserId"
        FOREIGN KEY ("ApprovedByUserId") 
        REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") 
        ON DELETE SET NULL;

-- Add indexes for reviewer lookups
CREATE INDEX IF NOT EXISTS "IX_StoryEpics_AssignedReviewerUserId"
    ON "alchimalia_schema"."StoryEpics" ("AssignedReviewerUserId")
    WHERE "AssignedReviewerUserId" IS NOT NULL;

CREATE INDEX IF NOT EXISTS "IX_StoryEpics_Status"
    ON "alchimalia_schema"."StoryEpics" ("Status");

COMMIT;

