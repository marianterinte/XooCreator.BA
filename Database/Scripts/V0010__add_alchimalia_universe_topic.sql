-- Add Alchimalia Universe dimension and topic
-- Run date: 2025-01-XX XX:XX:XX+02:00
-- Description: Adds new "Alchimalia Universe" dimension with a single checkbox topic for stories featuring Alchimalia heroes and universe

BEGIN;

-- Story Topic: alchimalia_universe
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('767b1e15-7a51-552c-9fd3-b716b1c6a6a1', 'alchimalia_universe', 'alchimalia_universe', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";

-- Translations for alchimalia_universe topic
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('13e94b94-e58e-5ec4-8367-2d69a5991d9c', '767b1e15-7a51-552c-9fd3-b716b1c6a6a1', 'ro-ro', 'Universul Alchimalia')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";

INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('cb46cd63-6f74-5ed2-bd78-d76314bc5217', '767b1e15-7a51-552c-9fd3-b716b1c6a6a1', 'en-us', 'Alchimalia Universe')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";

INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c364cfb7-986e-5bc8-a86b-13fa5daf578a', '767b1e15-7a51-552c-9fd3-b716b1c6a6a1', 'hu-hu', 'Alchimalia Univerzum')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";

COMMIT;

