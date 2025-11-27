-- Auto-generated from Data/SeedData/Story-Editor (topics, age groups, authors)
-- Run date: 2025-11-27 07:50:59+02:00

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp'
    ) THEN
        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    END IF;
END $$;

BEGIN;

-- Story Topics
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_wordplay'), 'fun_wordplay', 'fun', 11, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_wordplay|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_wordplay'), 'en-us', 'Wordplay humor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_wordplay|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_wordplay'), 'hu-hu', 'Szójáték')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_wordplay|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_wordplay'), 'ro-ro', 'Jocuri de cuvinte')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_inclusivity'), 'val_inclusivity', 'values_and_morals', 50, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_inclusivity|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_inclusivity'), 'en-us', 'Inclusivity and diversity')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_inclusivity|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_inclusivity'), 'hu-hu', 'Befogadás és sokszínűség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_inclusivity|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_inclusivity'), 'ro-ro', 'Incluziune și diversitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_critical_thinking'), 'edu_critical_thinking', 'educational', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_critical_thinking|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_critical_thinking'), 'en-us', 'Critical thinking')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_critical_thinking|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_critical_thinking'), 'hu-hu', 'Kritikus gondolkodás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_critical_thinking|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_critical_thinking'), 'ro-ro', 'Gândire critică')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_funny_dialogues'), 'fun_funny_dialogues', 'fun', 15, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_funny_dialogues|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_funny_dialogues'), 'en-us', 'Funny dialogues')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_funny_dialogues|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_funny_dialogues'), 'hu-hu', 'Vicces párbeszédek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_funny_dialogues|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_funny_dialogues'), 'ro-ro', 'Dialoguri haioase')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_joy'), 'emo_joy', 'emotional_depth', 22, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_joy|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_joy'), 'en-us', 'Joy and celebration')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_joy|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_joy'), 'hu-hu', 'Öröm és ünneplés')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_joy|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_joy'), 'ro-ro', 'Bucurie și celebrare')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_mystery'), 'fun_mystery', 'fun', 10, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_mystery|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_mystery'), 'en-us', 'Mystery fun')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_mystery|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_mystery'), 'hu-hu', 'Rejtély')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_mystery|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_mystery'), 'ro-ro', 'Mister')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_flashbacks'), 'cx_flashbacks', 'complexity', 35, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_flashbacks|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_flashbacks'), 'en-us', 'Flashbacks')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_flashbacks|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_flashbacks'), 'hu-hu', 'Visszaemlékezések')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_flashbacks|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_flashbacks'), 'ro-ro', 'Flashback-uri')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_friendship'), 'emo_friendship', 'emotional_depth', 17, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_friendship|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_friendship'), 'en-us', 'Friendship')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_friendship|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_friendship'), 'hu-hu', 'Barátság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_friendship|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_friendship'), 'ro-ro', 'Prietenie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_adventure'), 'fun_adventure', 'fun', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_adventure|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_adventure'), 'en-us', 'Adventure fun')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_adventure|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_adventure'), 'hu-hu', 'Kaland')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_adventure|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_adventure'), 'ro-ro', 'Aventură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_fast_dynamic'), 'pace_fast_dynamic', 'pace_and_action', 27, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_fast_dynamic|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_fast_dynamic'), 'en-us', 'Fast and dynamic')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_fast_dynamic|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_fast_dynamic'), 'hu-hu', 'Gyors és dinamikus')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_fast_dynamic|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_fast_dynamic'), 'ro-ro', 'Rapid și dinamic')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_reading'), 'edu_reading', 'educational', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_reading|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_reading'), 'en-us', 'Reading')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_reading|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_reading'), 'hu-hu', 'Olvasás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_reading|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_reading'), 'ro-ro', 'Citire')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_honesty'), 'val_honesty', 'values_and_morals', 44, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_honesty|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_honesty'), 'en-us', 'Honesty')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_honesty|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_honesty'), 'hu-hu', 'Őszinteség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_honesty|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_honesty'), 'ro-ro', 'Onestitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_silly_chars'), 'fun_silly_chars', 'fun', 13, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_silly_chars|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_silly_chars'), 'en-us', 'Silly characters')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_silly_chars|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_silly_chars'), 'hu-hu', 'Vicces karakterek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_silly_chars|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_silly_chars'), 'ro-ro', 'Personaje trăznite')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_quizzes'), 'int_quizzes', 'interactivity', 38, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_quizzes|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_quizzes'), 'en-us', 'Quizzes')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_quizzes|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_quizzes'), 'hu-hu', 'Kvízek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_quizzes|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_quizzes'), 'ro-ro', 'Quiz-uri')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_write_ending'), 'int_write_ending', 'interactivity', 42, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_write_ending|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_write_ending'), 'en-us', 'Write your own ending')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_write_ending|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_write_ending'), 'hu-hu', 'Írd meg a saját végét')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_write_ending|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_write_ending'), 'ro-ro', 'Scrie-ți propriul final')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_eco_awareness'), 'val_eco_awareness', 'values_and_morals', 51, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_eco_awareness|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_eco_awareness'), 'en-us', 'Eco awareness')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_eco_awareness|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_eco_awareness'), 'hu-hu', 'Környezettudatosság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_eco_awareness|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_eco_awareness'), 'ro-ro', 'Conștientizare ecologică')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_very_simple_linear'), 'cx_very_simple_linear', 'complexity', 31, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_very_simple_linear|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_very_simple_linear'), 'en-us', 'Very simple linear')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_very_simple_linear|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_very_simple_linear'), 'hu-hu', 'Nagyon egyszerű, lineáris')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_very_simple_linear|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_very_simple_linear'), 'ro-ro', 'Foarte simplă, liniară')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_questions'), 'int_questions', 'interactivity', 37, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_questions|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_questions'), 'en-us', 'Questions for reader')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_questions|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_questions'), 'hu-hu', 'Kérdések az olvasónak')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_questions|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_questions'), 'ro-ro', 'Întrebări pentru cititor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_science'), 'edu_science', 'educational', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_science|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_science'), 'en-us', 'Science (bio/chem/phys)')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_science|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_science'), 'hu-hu', 'Tudományok')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_science|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_science'), 'ro-ro', 'Științe')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_open_ending'), 'cx_open_ending', 'complexity', 36, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_open_ending|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_open_ending'), 'en-us', 'Open ending')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_open_ending|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_open_ending'), 'hu-hu', 'Nyitott vég')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_open_ending|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_open_ending'), 'ro-ro', 'Final deschis')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_literature'), 'edu_literature', 'educational', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_literature|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_literature'), 'en-us', 'Literature')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_literature|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_literature'), 'hu-hu', 'Irodalom')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_literature|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_literature'), 'ro-ro', 'Literatură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_social_skills'), 'edu_social_skills', 'educational', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_social_skills|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_social_skills'), 'en-us', 'Social skills')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_social_skills|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_social_skills'), 'hu-hu', 'Szociális készségek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_social_skills|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_social_skills'), 'ro-ro', 'Abilități sociale')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:classic_author'), 'classic_author', 'classic', 53, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:classic_author|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:classic_author'), 'en-us', 'Author')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:classic_author|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:classic_author'), 'hu-hu', 'Író')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:classic_author|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:classic_author'), 'ro-ro', 'Scriitor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_choose_path'), 'int_choose_path', 'interactivity', 39, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_choose_path|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_choose_path'), 'en-us', 'Choose-your-path')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_choose_path|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_choose_path'), 'hu-hu', 'Válaszd ki az utat')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_choose_path|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_choose_path'), 'ro-ro', 'Alege-ți drumul')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_conflict'), 'emo_conflict', 'emotional_depth', 20, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_conflict|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_conflict'), 'en-us', 'Dealing with conflict')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_conflict|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_conflict'), 'hu-hu', 'Konfliktuskezelés')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_conflict|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_conflict'), 'ro-ro', 'Gestionarea conflictului')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_slow_cozy'), 'pace_slow_cozy', 'pace_and_action', 25, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_slow_cozy|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_slow_cozy'), 'en-us', 'Slow and cozy')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_slow_cozy|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_slow_cozy'), 'hu-hu', 'Lassú és kényelmes')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_slow_cozy|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_slow_cozy'), 'ro-ro', 'Lent și liniștit')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_cute_animals'), 'fun_cute_animals', 'fun', 14, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_cute_animals|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_cute_animals'), 'en-us', 'Cute animals')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_cute_animals|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_cute_animals'), 'hu-hu', 'Aranyos állatok')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_cute_animals|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_cute_animals'), 'ro-ro', 'Animale drăguțe')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_family_bonds'), 'emo_family_bonds', 'emotional_depth', 24, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_family_bonds|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_family_bonds'), 'en-us', 'Family bonds')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_family_bonds|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_family_bonds'), 'hu-hu', 'Családi kötelékek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_family_bonds|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_family_bonds'), 'ro-ro', 'Legături de familie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_simple_with_subplot'), 'cx_simple_with_subplot', 'complexity', 32, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_simple_with_subplot|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_simple_with_subplot'), 'en-us', 'Simple with small subplot')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_simple_with_subplot|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_simple_with_subplot'), 'hu-hu', 'Egyszerű kis mellékszálakkal')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_simple_with_subplot|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_simple_with_subplot'), 'ro-ro', 'Simplă cu mic subplot')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_draw_scene'), 'int_draw_scene', 'interactivity', 41, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_draw_scene|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_draw_scene'), 'en-us', 'Draw your own scene')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_draw_scene|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_draw_scene'), 'hu-hu', 'Rajzold meg a saját jeleneted')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_draw_scene|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_draw_scene'), 'ro-ro', 'Desenează scena ta')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_perseverance'), 'val_perseverance', 'values_and_morals', 48, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_perseverance|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_perseverance'), 'en-us', 'Perseverance')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_perseverance|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_perseverance'), 'hu-hu', 'Kitartás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_perseverance|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_perseverance'), 'ro-ro', 'Perseverență')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_environment'), 'edu_environment', 'educational', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_environment|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_environment'), 'en-us', 'Environment')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_environment|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_environment'), 'hu-hu', 'Környezet')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_environment|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_environment'), 'ro-ro', 'Mediu și natură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_puzzles'), 'int_puzzles', 'interactivity', 40, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_puzzles|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_puzzles'), 'en-us', 'Puzzles and riddles')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_puzzles|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_puzzles'), 'hu-hu', 'Rejtvények és találós kérdések')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_puzzles|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_puzzles'), 'ro-ro', 'Puzzle-uri și ghicitori')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_creativity'), 'edu_creativity', 'educational', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_creativity|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_creativity'), 'en-us', 'Creativity')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_creativity|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_creativity'), 'hu-hu', 'Kreativitás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_creativity|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_creativity'), 'ro-ro', 'Creativitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_learn_from_mistakes'), 'val_learn_from_mistakes', 'values_and_morals', 52, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_learn_from_mistakes|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_learn_from_mistakes'), 'en-us', 'Learning from mistakes')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_learn_from_mistakes|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_learn_from_mistakes'), 'hu-hu', 'Tanulás a hibákból')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_learn_from_mistakes|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_learn_from_mistakes'), 'ro-ro', 'Învățare din greșeli')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_exploration'), 'pace_exploration', 'pace_and_action', 29, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_exploration|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_exploration'), 'en-us', 'Exploration-focused')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_exploration|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_exploration'), 'hu-hu', 'Felfedezés-központú')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_exploration|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_exploration'), 'ro-ro', 'Explorare')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_sadness_loss'), 'emo_sadness_loss', 'emotional_depth', 23, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_sadness_loss|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_sadness_loss'), 'en-us', 'Sadness and loss')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_sadness_loss|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_sadness_loss'), 'hu-hu', 'Szomorúság és veszteség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_sadness_loss|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_sadness_loss'), 'ro-ro', 'Tristețe și pierdere')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_nature'), 'val_respect_nature', 'values_and_morals', 47, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_respect_nature|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_nature'), 'en-us', 'Respect for nature')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_respect_nature|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_nature'), 'hu-hu', 'Tisztelet a természet iránt')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_respect_nature|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_nature'), 'ro-ro', 'Respect pentru natură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_others'), 'val_respect_others', 'values_and_morals', 46, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_respect_others|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_others'), 'en-us', 'Respect for others')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_respect_others|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_others'), 'hu-hu', 'Tisztelet mások iránt')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_respect_others|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_respect_others'), 'ro-ro', 'Respect pentru ceilalți')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_empathy'), 'emo_empathy', 'emotional_depth', 18, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_empathy|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_empathy'), 'en-us', 'Empathy')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_empathy|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_empathy'), 'hu-hu', 'Empátia')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_empathy|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_empathy'), 'ro-ro', 'Empatie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_physical_actions'), 'int_physical_actions', 'interactivity', 43, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_physical_actions|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_physical_actions'), 'en-us', 'Physical actions for kids')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_physical_actions|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_physical_actions'), 'hu-hu', 'Fizikai akciók gyerekeknek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:int_physical_actions|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:int_physical_actions'), 'ro-ro', 'Acțiuni fizice (ridică-te, sari)')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_curiosity'), 'val_curiosity', 'values_and_morals', 49, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_curiosity|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_curiosity'), 'en-us', 'Curiosity')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_curiosity|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_curiosity'), 'hu-hu', 'Kíváncsiság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_curiosity|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_curiosity'), 'ro-ro', 'Curiozitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_self_acceptance'), 'emo_self_acceptance', 'emotional_depth', 21, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_self_acceptance|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_self_acceptance'), 'en-us', 'Self-acceptance')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_self_acceptance|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_self_acceptance'), 'hu-hu', 'Önelfogadás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_self_acceptance|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_self_acceptance'), 'ro-ro', 'Auto-acceptare')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_multiple_threads'), 'cx_multiple_threads', 'complexity', 33, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_multiple_threads|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_multiple_threads'), 'en-us', 'Multiple plot threads')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_multiple_threads|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_multiple_threads'), 'hu-hu', 'Több cselekményszál')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_multiple_threads|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_multiple_threads'), 'ro-ro', 'Mai multe fire narative')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_balanced'), 'pace_balanced', 'pace_and_action', 26, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_balanced|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_balanced'), 'en-us', 'Balanced pace')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_balanced|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_balanced'), 'hu-hu', 'Kiegyensúlyozott')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_balanced|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_balanced'), 'ro-ro', 'Echilibrat')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_twist_ending'), 'cx_twist_ending', 'complexity', 34, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_twist_ending|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_twist_ending'), 'en-us', 'Twist ending')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_twist_ending|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_twist_ending'), 'hu-hu', 'Meglepetés vég')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:cx_twist_ending|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:cx_twist_ending'), 'ro-ro', 'Final surpriză')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_high_suspense'), 'pace_high_suspense', 'pace_and_action', 28, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_high_suspense|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_high_suspense'), 'en-us', 'High suspense')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_high_suspense|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_high_suspense'), 'hu-hu', 'Nagy feszültség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_high_suspense|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_high_suspense'), 'ro-ro', 'Mult suspans')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_history'), 'edu_history', 'educational', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_history|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_history'), 'en-us', 'History')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_history|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_history'), 'hu-hu', 'Történelem')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_history|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_history'), 'ro-ro', 'Istorie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_dialogue'), 'pace_dialogue', 'pace_and_action', 30, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_dialogue|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_dialogue'), 'en-us', 'Dialogue-focused')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_dialogue|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_dialogue'), 'hu-hu', 'Párbeszéd-központú')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:pace_dialogue|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:pace_dialogue'), 'ro-ro', 'Dialog centric')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_popular_story'), 'fun_popular_story', 'fun', 16, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_popular_story|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_popular_story'), 'en-us', 'Popular story')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_popular_story|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_popular_story'), 'hu-hu', 'Népszerű történet')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_popular_story|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_popular_story'), 'ro-ro', 'Poveste populară')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_friendship_team'), 'val_friendship_team', 'values_and_morals', 45, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_friendship_team|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_friendship_team'), 'en-us', 'Friendship and teamwork')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_friendship_team|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_friendship_team'), 'hu-hu', 'Barátság és csapatmunka')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:val_friendship_team|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:val_friendship_team'), 'ro-ro', 'Prietenie și lucru în echipă')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_slapstick'), 'fun_slapstick', 'fun', 12, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_slapstick|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_slapstick'), 'en-us', 'Slapstick humor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_slapstick|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_slapstick'), 'hu-hu', 'Fizikai humor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:fun_slapstick|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:fun_slapstick'), 'ro-ro', 'Umor fizic')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_courage'), 'emo_courage', 'emotional_depth', 19, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_courage|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_courage'), 'en-us', 'Courage')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_courage|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_courage'), 'hu-hu', 'Bátorság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:emo_courage|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:emo_courage'), 'ro-ro', 'Curaj')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_math'), 'edu_math', 'educational', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_math|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_math'), 'en-us', 'Math')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_math|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_math'), 'hu-hu', 'Matematika')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic-tr:edu_math|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-topic:edu_math'), 'ro-ro', 'Matematică')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
-- Story Age Groups
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:teens_13_16'), 'teens_13_16', 13, 16, 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:teens_13_16|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:teens_13_16'), 'en-us', 'Teens', 'Stories with identity themes, nuanced morality, symbolism and increased narrative complexity.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:teens_13_16|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:teens_13_16'), 'hu-hu', 'Serdülő', 'Történetek identitási témákkal, árnyalt erkölcsösséggel, szimbolizmussal és megnövelt narratív komplexitással.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:teens_13_16|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:teens_13_16'), 'ro-ro', 'Adolescenți', 'Povești cu teme identitare, moralitate nuanțată, simbolism și complexitate narativă crescută.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:early_school_6_8'), 'early_school_6_8', 6, 8, 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:early_school_6_8|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:early_school_6_8'), 'en-us', 'Early school', 'Stories with a clear narrative thread, memorable characters, first explicit educational concepts.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:early_school_6_8|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:early_school_6_8'), 'hu-hu', 'Kisiskolás', 'Történetek világos narratív szállal, emlékezetes karakterekkel, első explicit oktatási fogalmakkal.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:early_school_6_8|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:early_school_6_8'), 'ro-ro', 'Școlari mici', 'Povești cu un fir narativ clar, personaje memorabile, prime noțiuni educaționale explicite.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:middle_school_9_12'), 'middle_school_9_12', 9, 12, 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:middle_school_9_12|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:middle_school_9_12'), 'en-us', 'Middle school', 'Stories with more complex conflict, subtler humor, first moral dilemmas and richer science-fantasy.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:middle_school_9_12|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:middle_school_9_12'), 'hu-hu', 'Nagyiskolás', 'Történetek összetettebb konfliktussal, finomabb humorral, első erkölcsi dilemmákkal és gazdagabb sci-fi fantasy elemekkel.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:middle_school_9_12|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:middle_school_9_12'), 'ro-ro', 'Copii mari', 'Povești cu conflict mai complex, umor mai subtil, primele dileme morale și science-fantasy mai bogat.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:preschool_3_5'), 'preschool_3_5', 3, 5, 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:preschool_3_5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:preschool_3_5'), 'en-us', 'Preschool', 'Very simple stories, lots of images, short sentences, physical interactivity and clear emotions.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:preschool_3_5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:preschool_3_5'), 'hu-hu', 'Óvodás', 'Nagyon egyszerű történetek, sok kép, rövid mondatok, fizikai interaktivitás és világos érzelmek.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group-tr:preschool_3_5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'story-age-group:preschool_3_5'), 'ro-ro', 'Preșcolari', 'Povești foarte simple, multe imagini, propoziții scurte, interactivitate fizică și emoții clare.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
-- Classic Authors
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:ion-creanga'), 'ion-creanga', 'Ion Creangă', 'ro-ro', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:petre-ispirescu'), 'petre-ispirescu', 'Petre Ispirescu', 'ro-ro', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:ioan-slavici'), 'ioan-slavici', 'Ioan Slavici', 'ro-ro', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:mihai-eminescu'), 'mihai-eminescu', 'Mihai Eminescu', 'ro-ro', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:barbu-stefanescu-delavrancea'), 'barbu-stefanescu-delavrancea', 'Barbu Ștefănescu Delavrancea', 'ro-ro', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:emil-garleanu'), 'emil-garleanu', 'Emil Gârleanu', 'ro-ro', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:i-al-bratescu-voinesti'), 'i-al-bratescu-voinesti', 'I. Al. Brătescu-Voinești', 'ro-ro', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:calin-gruia'), 'calin-gruia', 'Călin Gruia', 'ro-ro', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:otilia-cazimir'), 'otilia-cazimir', 'Otilia Cazimir', 'ro-ro', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:dumitru-almas'), 'dumitru-almas', 'Dumitru Almaș', 'ro-ro', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:cezar-petrescu'), 'cezar-petrescu', 'Cezar Petrescu', 'ro-ro', 10, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:gellu-naum'), 'gellu-naum', 'Gellu Naum', 'ro-ro', 11, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:ana-blandiana'), 'ana-blandiana', 'Ana Blandiana', 'ro-ro', 12, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:mircea-santimbreanu'), 'mircea-santimbreanu', 'Mircea Sântimbreanu', 'ro-ro', 13, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:octav-pancu-iasi'), 'octav-pancu-iasi', 'Octav Pancu-Iași', 'ro-ro', 14, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:lucia-olteanu'), 'lucia-olteanu', 'Lucia Olteanu', 'ro-ro', 15, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:alexandru-mitru'), 'alexandru-mitru', 'Alexandru Mitru', 'ro-ro', 16, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:mihail-sadoveanu'), 'mihail-sadoveanu', 'Mihail Sadoveanu', 'ro-ro', 17, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:gala-galaction'), 'gala-galaction', 'Gala Galaction', 'ro-ro', 18, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:grigore-bajenaru'), 'grigore-bajenaru', 'Grigore Băjenaru', 'ro-ro', 19, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:benedek-elek'), 'benedek-elek', 'Benedek Elek', 'hu-hu', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:istvan-fekete'), 'istvan-fekete', 'István Fekete', 'hu-hu', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:ervin-lazar'), 'ervin-lazar', 'Ervin Lázár', 'hu-hu', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:mihaly-fazekas'), 'mihaly-fazekas', 'Mihály Fazekas', 'hu-hu', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:magda-szabo'), 'magda-szabo', 'Magda Szabó', 'hu-hu', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:zsigmond-moricz'), 'zsigmond-moricz', 'Zsigmond Móricz', 'hu-hu', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:sandor-weores'), 'sandor-weores', 'Sándor Weöres', 'hu-hu', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:sandor-petofi'), 'sandor-petofi', 'Sándor Petőfi', 'hu-hu', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:kalman-mikszath'), 'kalman-mikszath', 'Kálmán Mikszáth', 'hu-hu', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:jozsef-attila'), 'jozsef-attila', 'József Attila', 'hu-hu', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:lewis-carroll'), 'lewis-carroll', 'Lewis Carroll', 'en-us', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:beatrix-potter'), 'beatrix-potter', 'Beatrix Potter', 'en-us', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:a-a-milne'), 'a-a-milne', 'A. A. Milne', 'en-us', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:j-m-barrie'), 'j-m-barrie', 'J. M. Barrie', 'en-us', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:l-frank-baum'), 'l-frank-baum', 'L. Frank Baum', 'en-us', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:dr-seuss'), 'dr-seuss', 'Dr. Seuss', 'en-us', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:roald-dahl'), 'roald-dahl', 'Roald Dahl', 'en-us', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:c-s-lewis'), 'c-s-lewis', 'C. S. Lewis', 'en-us', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:e-b-white'), 'e-b-white', 'E. B. White', 'en-us', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:shel-silverstein'), 'shel-silverstein', 'Shel Silverstein', 'en-us', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:maurice-sendak'), 'maurice-sendak', 'Maurice Sendak', 'en-us', 10, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:j-k-rowling'), 'j-k-rowling', 'J. K. Rowling', 'en-us', 11, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'classic-author:enid-blyton'), 'enid-blyton', 'Enid Blyton', 'en-us', 12, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
COMMIT;
