-- Migration: Add Story Creators Challenge Subscriptions and Submissions tables

BEGIN;

-- Create StoryCreatorsChallengeSubscriptions table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallengeSubscriptions" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ChallengeId" character varying(100) NOT NULL,
    "UserId" uuid NOT NULL,
    "SubscribedAt" timestamp NOT NULL DEFAULT now(),
    CONSTRAINT "PK_StoryCreatorsChallengeSubscriptions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCreatorsChallengeSubscriptions_Challenges_ChallengeId"
        FOREIGN KEY ("ChallengeId") REFERENCES "alchimalia_schema"."StoryCreatorsChallenges" ("ChallengeId") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryCreatorsChallengeSubscriptions_Users_UserId"
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryCreatorsChallengeSubscriptions_ChallengeId_UserId"
        UNIQUE ("ChallengeId", "UserId")
);

CREATE INDEX IF NOT EXISTS "IX_StoryCreatorsChallengeSubscriptions_ChallengeId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeSubscriptions" ("ChallengeId");
CREATE INDEX IF NOT EXISTS "IX_StoryCreatorsChallengeSubscriptions_UserId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeSubscriptions" ("UserId");


-- Create StoryCreatorsChallengeSubmissions table
-- Add CoverImageUrl and CoverImageRelPath columns to StoryCreatorsChallenges table
ALTER TABLE "alchimalia_schema"."StoryCreatorsChallenges"
    ADD COLUMN IF NOT EXISTS "CoverImageUrl" character varying(500),
    ADD COLUMN IF NOT EXISTS "CoverImageRelPath" character varying(500);

-- Create StoryCreatorsChallengeSubscriptions table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryCreatorsChallengeSubscriptions" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "ChallengeId" character varying(100) NOT NULL,
    "StoryId" character varying(200) NOT NULL,
    "UserId" uuid NOT NULL,
    "SubmittedAt" timestamp NOT NULL DEFAULT now(),
    "LikesCount" integer NOT NULL DEFAULT 0,
    "IsWinner" boolean NOT NULL DEFAULT false,
    CONSTRAINT "PK_StoryCreatorsChallengeSubmissions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryCreatorsChallengeSubmissions_Challenges_ChallengeId"
        FOREIGN KEY ("ChallengeId") REFERENCES "alchimalia_schema"."StoryCreatorsChallenges" ("ChallengeId") ON DELETE CASCADE,
    CONSTRAINT "FK_StoryCreatorsChallengeSubmissions_Users_UserId"
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryCreatorsChallengeSubmissions_ChallengeId_StoryId"
        UNIQUE ("ChallengeId", "StoryId")
);

CREATE INDEX IF NOT EXISTS "IX_StoryCreatorsChallengeSubmissions_ChallengeId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeSubmissions" ("ChallengeId");
CREATE INDEX IF NOT EXISTS "IX_StoryCreatorsChallengeSubmissions_StoryId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeSubmissions" ("StoryId");
CREATE INDEX IF NOT EXISTS "IX_StoryCreatorsChallengeSubmissions_UserId" 
    ON "alchimalia_schema"."StoryCreatorsChallengeSubmissions" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_StoryCreatorsChallengeSubmissions_LikesCount" 
    ON "alchimalia_schema"."StoryCreatorsChallengeSubmissions" ("LikesCount");

COMMIT;
