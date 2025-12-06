-- Mark learn-math-s1 story as evaluative and set correct answers
-- This script is idempotent and can be run multiple times safely
-- 
-- IMPORTANT: If you get "current transaction is aborted" error:
-- 1. Run: ROLLBACK;
-- 2. Then run this script in a NEW session/connection

-- Step 1: Update IsEvaluative flag (simple, no dependencies)
UPDATE "alchimalia_schema"."StoryDefinitions"
SET "IsEvaluative" = true,
    "UpdatedAt" = CURRENT_TIMESTAMP
WHERE "StoryId" = 'learn-math-s1';

-- Step 2: Reset ALL answers for quiz tiles to false, then set correct ones to true
-- This approach uses CASE WHEN to explicitly set IsCorrect based on TileId and AnswerId
UPDATE "alchimalia_schema"."StoryAnswers" sa
SET "IsCorrect" = CASE
    -- q1: answerId "b" is correct
    WHEN st."TileId" = 'q1' AND sa."AnswerId" = 'b' THEN true
    -- q2: answerId "b" is correct
    WHEN st."TileId" = 'q2' AND sa."AnswerId" = 'b' THEN true
    -- q3: answerId "c" is correct
    WHEN st."TileId" = 'q3' AND sa."AnswerId" = 'c' THEN true
    -- q4: answerId "b" is correct
    WHEN st."TileId" = 'q4' AND sa."AnswerId" = 'b' THEN true
    -- All other answers are false
    ELSE false
END
FROM "alchimalia_schema"."StoryTiles" st
JOIN "alchimalia_schema"."StoryDefinitions" sd ON st."StoryDefinitionId" = sd."Id"
WHERE sd."StoryId" = 'learn-math-s1'
  AND st."TileId" IN ('q1', 'q2', 'q3', 'q4')
  AND sa."StoryTileId" = st."Id";
