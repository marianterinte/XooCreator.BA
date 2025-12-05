-- Adds story evaluation system: IsEvaluative flag, IsCorrect on answers, and evaluation tracking tables

BEGIN;

-- Add IsEvaluative flag to StoryDefinitions
ALTER TABLE "alchimalia_schema"."StoryDefinitions"
    ADD COLUMN IF NOT EXISTS "IsEvaluative" boolean NOT NULL DEFAULT false;

-- Add IsEvaluative flag to StoryCrafts (for editor drafts)
ALTER TABLE "alchimalia_schema"."StoryCrafts"
    ADD COLUMN IF NOT EXISTS "IsEvaluative" boolean NOT NULL DEFAULT false;

-- Add IsCorrect flag to StoryAnswers
ALTER TABLE "alchimalia_schema"."StoryAnswers"
    ADD COLUMN IF NOT EXISTS "IsCorrect" boolean NOT NULL DEFAULT false;

-- Add IsCorrect flag to StoryCraftAnswers (for editor drafts)
ALTER TABLE "alchimalia_schema"."StoryCraftAnswers"
    ADD COLUMN IF NOT EXISTS "IsCorrect" boolean NOT NULL DEFAULT false;

-- Create StoryQuizAnswer table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryQuizAnswers" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" text NOT NULL,
    "TileId" text NOT NULL,
    "SelectedAnswerId" text NOT NULL,
    "IsCorrect" boolean NOT NULL,
    "AnsweredAt" timestamp with time zone NOT NULL,
    "SessionId" uuid,
    CONSTRAINT "PK_StoryQuizAnswers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryQuizAnswers_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_UserStoryTileSession" 
    ON "alchimalia_schema"."StoryQuizAnswers" ("UserId", "StoryId", "TileId", "SessionId");

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_UserStory" 
    ON "alchimalia_schema"."StoryQuizAnswers" ("UserId", "StoryId");

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_Session" 
    ON "alchimalia_schema"."StoryQuizAnswers" ("SessionId");

-- Create StoryEvaluationResult table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryEvaluationResults" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" text NOT NULL,
    "SessionId" uuid NOT NULL,
    "TotalQuizzes" integer NOT NULL,
    "CorrectAnswers" integer NOT NULL,
    "ScorePercentage" integer NOT NULL,
    "CompletedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_StoryEvaluationResults" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryEvaluationResults_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryEvaluationResults_UserStorySession" 
        UNIQUE ("UserId", "StoryId", "SessionId")
);

CREATE INDEX IF NOT EXISTS "IX_StoryEvaluationResults_UserStoryCompleted" 
    ON "alchimalia_schema"."StoryEvaluationResults" ("UserId", "StoryId", "CompletedAt");

COMMIT;

