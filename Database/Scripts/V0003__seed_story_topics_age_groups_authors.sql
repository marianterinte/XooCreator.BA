-- Auto-generated from Data/SeedData/Story-Editor (topics, age groups, authors)
-- Run date: 2025-11-27 11:59:42+02:00

BEGIN;

-- Story Topics
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d8933b73-6f53-5fe8-8de8-bfcc099fc8c4', 'int_puzzles', 'interactivity', 40, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c3d192a3-c5cc-5213-a0be-59b79435b739', 'd8933b73-6f53-5fe8-8de8-bfcc099fc8c4', 'en-us', 'Puzzles and riddles')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('b5dd9d22-28b5-508e-8ce5-3560cc8bc440', 'd8933b73-6f53-5fe8-8de8-bfcc099fc8c4', 'hu-hu', 'Rejtvények és találós kérdések')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('17285054-a46b-5559-935a-55e1371ff837', 'd8933b73-6f53-5fe8-8de8-bfcc099fc8c4', 'ro-ro', 'Puzzle-uri și ghicitori')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d85dd066-4b4f-5c3d-b616-0fba92036b59', 'val_friendship_team', 'values_and_morals', 45, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a00d0e7a-037b-562c-bd7a-642267fa899f', 'd85dd066-4b4f-5c3d-b616-0fba92036b59', 'en-us', 'Friendship and teamwork')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('472ef69b-bb97-529d-ad72-da609b7c5aa3', 'd85dd066-4b4f-5c3d-b616-0fba92036b59', 'hu-hu', 'Barátság és csapatmunka')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('6547990d-fd48-563d-9fa1-ffcd0cdce3a1', 'd85dd066-4b4f-5c3d-b616-0fba92036b59', 'ro-ro', 'Prietenie și lucru în echipă')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('50e90f6f-1c99-5d9a-8c09-426f45728652', 'edu_environment', 'educational', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d586978a-8a55-51d6-826f-45679bd5be82', '50e90f6f-1c99-5d9a-8c09-426f45728652', 'en-us', 'Environment')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('2561f26d-af03-5d5c-8837-63702447a241', '50e90f6f-1c99-5d9a-8c09-426f45728652', 'hu-hu', 'Környezet')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('49036d1e-9bb2-5225-97ce-3f6bd9bbfa6b', '50e90f6f-1c99-5d9a-8c09-426f45728652', 'ro-ro', 'Mediu și natură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('cda808ae-b6f2-5a6a-8107-044ff1a3aac3', 'cx_twist_ending', 'complexity', 34, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('f80c7f9d-fdeb-5e69-a39d-252548cdd9e7', 'cda808ae-b6f2-5a6a-8107-044ff1a3aac3', 'en-us', 'Twist ending')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('2dfa0e8f-90e9-5510-9294-536e935d3fb3', 'cda808ae-b6f2-5a6a-8107-044ff1a3aac3', 'hu-hu', 'Meglepetés vég')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('245bb364-8827-5e09-84e7-7d34f6cccfc5', 'cda808ae-b6f2-5a6a-8107-044ff1a3aac3', 'ro-ro', 'Final surpriză')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('e58a4d43-daf3-57e0-af89-035ca7a516c5', 'edu_critical_thinking', 'educational', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ed0a375a-4a38-50ef-bf84-1afac9c50a23', 'e58a4d43-daf3-57e0-af89-035ca7a516c5', 'en-us', 'Critical thinking')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('900cc105-84e8-5ba3-b28d-a7da5abea639', 'e58a4d43-daf3-57e0-af89-035ca7a516c5', 'hu-hu', 'Kritikus gondolkodás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('59e47081-83bb-5b10-a67e-987f2837cbe7', 'e58a4d43-daf3-57e0-af89-035ca7a516c5', 'ro-ro', 'Gândire critică')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('ac018b49-5115-54f5-aeee-502b4b208ee7', 'emo_self_acceptance', 'emotional_depth', 21, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('87984b20-bc38-5d14-8112-30bd1455d6d7', 'ac018b49-5115-54f5-aeee-502b4b208ee7', 'en-us', 'Self-acceptance')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('b38652b3-3d25-5b76-94cf-cafe0c5d5bfe', 'ac018b49-5115-54f5-aeee-502b4b208ee7', 'hu-hu', 'Önelfogadás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ed05af49-3ceb-5a00-a821-0f67faaa9fd0', 'ac018b49-5115-54f5-aeee-502b4b208ee7', 'ro-ro', 'Auto-acceptare')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('8f357f22-a231-5ba5-8f61-919ac5f2ab51', 'cx_multiple_threads', 'complexity', 33, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('6e70d79c-dc4d-5109-bc26-a7e27c08bf82', '8f357f22-a231-5ba5-8f61-919ac5f2ab51', 'en-us', 'Multiple plot threads')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('32f6d18c-7c7c-5029-a423-afbf69403543', '8f357f22-a231-5ba5-8f61-919ac5f2ab51', 'hu-hu', 'Több cselekményszál')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c03d254e-6010-5ece-8b4f-fec26d82e919', '8f357f22-a231-5ba5-8f61-919ac5f2ab51', 'ro-ro', 'Mai multe fire narative')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('641f7293-8ff2-567c-a7a1-dfa736eb4e62', 'pace_slow_cozy', 'pace_and_action', 25, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('3d0c4ee1-411e-5fad-a53a-cc67462fdf38', '641f7293-8ff2-567c-a7a1-dfa736eb4e62', 'en-us', 'Slow and cozy')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('8520319a-689a-5963-8bcf-f5c15288cc81', '641f7293-8ff2-567c-a7a1-dfa736eb4e62', 'hu-hu', 'Lassú és kényelmes')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c5438019-a519-5cd2-9f62-ba5cdcd4b339', '641f7293-8ff2-567c-a7a1-dfa736eb4e62', 'ro-ro', 'Lent și liniștit')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('515bee87-5213-5c56-83ba-235d9dd6bd54', 'pace_exploration', 'pace_and_action', 29, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('56df94fa-f3d3-5ea7-aa8e-b7f6dc571ce9', '515bee87-5213-5c56-83ba-235d9dd6bd54', 'en-us', 'Exploration-focused')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('81a8ebb6-00ad-53a2-a652-f9e6d6f98568', '515bee87-5213-5c56-83ba-235d9dd6bd54', 'hu-hu', 'Felfedezés-központú')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('afad46f4-566a-5c2a-b918-df8d79f454ce', '515bee87-5213-5c56-83ba-235d9dd6bd54', 'ro-ro', 'Explorare')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('be984289-7651-5629-ab39-ddcf86529f79', 'emo_joy', 'emotional_depth', 22, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('7e38748d-9afb-515e-a1b6-c2e35ea98b54', 'be984289-7651-5629-ab39-ddcf86529f79', 'en-us', 'Joy and celebration')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0d974380-d3d9-5953-9664-6d91cc6afed5', 'be984289-7651-5629-ab39-ddcf86529f79', 'hu-hu', 'Öröm és ünneplés')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('952ddbb7-d6e0-5f97-8b14-a8f874e82184', 'be984289-7651-5629-ab39-ddcf86529f79', 'ro-ro', 'Bucurie și celebrare')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('4c92de8f-e004-5040-9727-9fc3bd98a021', 'fun_mystery', 'fun', 10, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('348364a5-6a71-5829-b9a5-62627fe6c910', '4c92de8f-e004-5040-9727-9fc3bd98a021', 'en-us', 'Mystery fun')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('de0deec1-6aba-5ac9-81f1-3212972827ed', '4c92de8f-e004-5040-9727-9fc3bd98a021', 'hu-hu', 'Rejtély')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0ce73ef1-eb82-52f5-9edd-3d3867bd8705', '4c92de8f-e004-5040-9727-9fc3bd98a021', 'ro-ro', 'Mister')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('6156190e-86bc-5840-ae4a-8170b68628ed', 'val_perseverance', 'values_and_morals', 48, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('f09fc27d-d99f-51c2-b8f6-691a4d6ed4cd', '6156190e-86bc-5840-ae4a-8170b68628ed', 'en-us', 'Perseverance')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('38f05840-2d3e-59bc-a6aa-e8ffff6b86d7', '6156190e-86bc-5840-ae4a-8170b68628ed', 'hu-hu', 'Kitartás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('fa8d4607-8fa2-5e51-ba82-bd5f3b8001a6', '6156190e-86bc-5840-ae4a-8170b68628ed', 'ro-ro', 'Perseverență')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d69a0a3d-c18e-5d41-9bfa-7ab89a07adff', 'fun_popular_story', 'fun', 16, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('331ef8de-90c4-558c-bc54-808ca719689a', 'd69a0a3d-c18e-5d41-9bfa-7ab89a07adff', 'en-us', 'Popular story')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('582dfbed-9c1e-59f8-b679-d7ce91b94876', 'd69a0a3d-c18e-5d41-9bfa-7ab89a07adff', 'hu-hu', 'Népszerű történet')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('089bae3e-985c-5efd-8bee-b2ec6a739c53', 'd69a0a3d-c18e-5d41-9bfa-7ab89a07adff', 'ro-ro', 'Poveste populară')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('dad5768a-e954-56ea-a129-5cda781e0253', 'fun_cute_animals', 'fun', 14, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('18e6c035-362d-5d08-a96b-16631884e7ad', 'dad5768a-e954-56ea-a129-5cda781e0253', 'en-us', 'Cute animals')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('62b1dda6-c599-5741-8a3f-f0323f39d123', 'dad5768a-e954-56ea-a129-5cda781e0253', 'hu-hu', 'Aranyos állatok')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('532830f9-2780-555b-aa7f-d13b18a49677', 'dad5768a-e954-56ea-a129-5cda781e0253', 'ro-ro', 'Animale drăguțe')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('c347c854-cc7e-51ae-840b-ff6edb4e8808', 'edu_reading', 'educational', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('dfc2e3c5-23a8-5b52-a953-5b790564b282', 'c347c854-cc7e-51ae-840b-ff6edb4e8808', 'en-us', 'Reading')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e75eb980-214c-5d17-bc72-e813e2ca82ac', 'c347c854-cc7e-51ae-840b-ff6edb4e8808', 'hu-hu', 'Olvasás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ba61bd7d-a44c-5a4f-81c7-51b25e26d643', 'c347c854-cc7e-51ae-840b-ff6edb4e8808', 'ro-ro', 'Citire')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('8f9861fc-01b4-5c82-b967-e62f598d0c95', 'fun_funny_dialogues', 'fun', 15, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('41905ba4-4445-51be-90c4-eaf162cf93d0', '8f9861fc-01b4-5c82-b967-e62f598d0c95', 'en-us', 'Funny dialogues')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('88df5de0-6676-5f25-a8a3-edbbb86d2e90', '8f9861fc-01b4-5c82-b967-e62f598d0c95', 'hu-hu', 'Vicces párbeszédek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('1ffdbfef-8bfc-59de-835d-00f34d12df17', '8f9861fc-01b4-5c82-b967-e62f598d0c95', 'ro-ro', 'Dialoguri haioase')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d2203126-cdfc-5876-8252-84e91eb82d48', 'cx_open_ending', 'complexity', 36, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('b087480c-6e0c-5e3f-9afe-bb44907096a4', 'd2203126-cdfc-5876-8252-84e91eb82d48', 'en-us', 'Open ending')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('7a97dce0-f256-5487-b94c-60dc080c35a4', 'd2203126-cdfc-5876-8252-84e91eb82d48', 'hu-hu', 'Nyitott vég')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('cc5570cc-c932-5918-a8f8-de1088f7582a', 'd2203126-cdfc-5876-8252-84e91eb82d48', 'ro-ro', 'Final deschis')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('6a69faf5-70c5-5e98-b1fa-8d5338f0ed52', 'edu_social_skills', 'educational', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e23b8f22-faf5-52e6-81e9-56a7aab894f8', '6a69faf5-70c5-5e98-b1fa-8d5338f0ed52', 'en-us', 'Social skills')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('5c447286-3d64-510a-b436-54d2a627c168', '6a69faf5-70c5-5e98-b1fa-8d5338f0ed52', 'hu-hu', 'Szociális készségek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('cb5b06ef-55d6-5fbe-ae64-0551176b2de8', '6a69faf5-70c5-5e98-b1fa-8d5338f0ed52', 'ro-ro', 'Abilități sociale')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('77eb01d0-18f3-512a-b78e-3cd56579e9f4', 'edu_math', 'educational', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c646d135-154f-5b32-a46f-749d7b5ec043', '77eb01d0-18f3-512a-b78e-3cd56579e9f4', 'en-us', 'Math')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('b7e25278-0d2b-54e8-9b24-bd3c4ac3f2cb', '77eb01d0-18f3-512a-b78e-3cd56579e9f4', 'hu-hu', 'Matematika')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('522c26e9-8c96-5660-99ea-47ca6828f47a', '77eb01d0-18f3-512a-b78e-3cd56579e9f4', 'ro-ro', 'Matematică')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d1bd3908-2385-5c02-ae43-64285112d984', 'pace_fast_dynamic', 'pace_and_action', 27, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e4ed85ca-e097-5ca2-b098-fb5bb61bad26', 'd1bd3908-2385-5c02-ae43-64285112d984', 'en-us', 'Fast and dynamic')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('87444112-e3ac-597c-9154-49f468cca520', 'd1bd3908-2385-5c02-ae43-64285112d984', 'hu-hu', 'Gyors és dinamikus')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('2f27215d-8aa0-519c-a726-fecfddd2092d', 'd1bd3908-2385-5c02-ae43-64285112d984', 'ro-ro', 'Rapid și dinamic')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('f8938d2a-d970-5c8b-966b-9fb39da253bf', 'int_questions', 'interactivity', 37, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('62ba0292-a45a-5dbc-9140-20ae49fbc686', 'f8938d2a-d970-5c8b-966b-9fb39da253bf', 'en-us', 'Questions for reader')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('8476e8f5-93c5-5e44-a9d5-d249b42fe4f0', 'f8938d2a-d970-5c8b-966b-9fb39da253bf', 'hu-hu', 'Kérdések az olvasónak')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('aa0ec629-e7e8-50fb-a4a7-d44244bd36e9', 'f8938d2a-d970-5c8b-966b-9fb39da253bf', 'ro-ro', 'Întrebări pentru cititor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('7c235a73-7c57-578b-8c36-f472dd15f9af', 'emo_family_bonds', 'emotional_depth', 24, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('383f16b2-5f4f-5468-b746-aca5ecaa99fd', '7c235a73-7c57-578b-8c36-f472dd15f9af', 'en-us', 'Family bonds')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('25969796-7fec-5412-9ca6-7055e048ab5b', '7c235a73-7c57-578b-8c36-f472dd15f9af', 'hu-hu', 'Családi kötelékek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ed8b4a46-325b-5a55-b84f-4d599ff83b2a', '7c235a73-7c57-578b-8c36-f472dd15f9af', 'ro-ro', 'Legături de familie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('83368a10-fb38-56a9-bb2e-0f49c49dd9a6', 'val_eco_awareness', 'values_and_morals', 51, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('f13be9ec-72c1-527b-a30f-299b0ee99c50', '83368a10-fb38-56a9-bb2e-0f49c49dd9a6', 'en-us', 'Eco awareness')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c5016f95-79e2-559a-9956-31db5a0b95c4', '83368a10-fb38-56a9-bb2e-0f49c49dd9a6', 'hu-hu', 'Környezettudatosság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('350857a2-e0fc-5ebb-a883-6cb8956f4295', '83368a10-fb38-56a9-bb2e-0f49c49dd9a6', 'ro-ro', 'Conștientizare ecologică')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('4797f875-546d-50f2-a756-6c251593d99d', 'cx_simple_with_subplot', 'complexity', 32, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0bfbce56-3649-527e-8187-43c2e3d6c7ad', '4797f875-546d-50f2-a756-6c251593d99d', 'en-us', 'Simple with small subplot')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a94936c8-acc2-545b-9d0d-40afa480f6f5', '4797f875-546d-50f2-a756-6c251593d99d', 'hu-hu', 'Egyszerű kis mellékszálakkal')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('2bf1abf7-cdaa-51d4-9a0f-548205dca859', '4797f875-546d-50f2-a756-6c251593d99d', 'ro-ro', 'Simplă cu mic subplot')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('ca4af6a1-9abd-5be8-8f31-93523009e394', 'val_curiosity', 'values_and_morals', 49, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a95100e1-7448-5bdf-b8b0-e406f2046682', 'ca4af6a1-9abd-5be8-8f31-93523009e394', 'en-us', 'Curiosity')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d06b9217-4204-57ef-a1b3-1476cb9f3100', 'ca4af6a1-9abd-5be8-8f31-93523009e394', 'hu-hu', 'Kíváncsiság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('49f55281-3649-5161-b063-2f0e50a4e13d', 'ca4af6a1-9abd-5be8-8f31-93523009e394', 'ro-ro', 'Curiozitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('9b47a3e8-3538-5aef-a003-97fde072039b', 'fun_adventure', 'fun', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e2202276-a58d-53e8-a23a-51e9ac4f8f58', '9b47a3e8-3538-5aef-a003-97fde072039b', 'en-us', 'Adventure fun')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a8b38c07-d8bd-50c1-a444-078c40cac056', '9b47a3e8-3538-5aef-a003-97fde072039b', 'hu-hu', 'Kaland')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('f5bb6813-a9f8-5a21-a8de-4ab468b881d8', '9b47a3e8-3538-5aef-a003-97fde072039b', 'ro-ro', 'Aventură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('e4697a57-3de4-5565-96a4-113f3d2ac20c', 'edu_history', 'educational', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ee8e7312-a02a-54df-afcf-dee26b6c0675', 'e4697a57-3de4-5565-96a4-113f3d2ac20c', 'en-us', 'History')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ca03cf12-bac8-501f-a6ad-ead5b75a37bb', 'e4697a57-3de4-5565-96a4-113f3d2ac20c', 'hu-hu', 'Történelem')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('460d7a12-c096-51b6-8dd1-8c7a81e5a4c4', 'e4697a57-3de4-5565-96a4-113f3d2ac20c', 'ro-ro', 'Istorie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d257c414-389e-58d2-8bac-3bf76bccc251', 'cx_flashbacks', 'complexity', 35, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('2106992e-d851-545f-84ab-8d77614a80ca', 'd257c414-389e-58d2-8bac-3bf76bccc251', 'en-us', 'Flashbacks')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e8d2a601-e241-5112-9e96-53a4cb8f5880', 'd257c414-389e-58d2-8bac-3bf76bccc251', 'hu-hu', 'Visszaemlékezések')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('67f27d41-4510-5514-9f50-c4f44988e889', 'd257c414-389e-58d2-8bac-3bf76bccc251', 'ro-ro', 'Flashback-uri')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('0ac7132a-7c10-58e6-96e5-669647db0fed', 'emo_friendship', 'emotional_depth', 17, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('035f43db-44b5-52dd-b0f3-25277f5c8f08', '0ac7132a-7c10-58e6-96e5-669647db0fed', 'en-us', 'Friendship')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('1b0bd18e-0ec0-553e-9c4e-8dbd35d8ca9f', '0ac7132a-7c10-58e6-96e5-669647db0fed', 'hu-hu', 'Barátság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('21e5cc61-c144-57a8-a14b-42c86e2d829c', '0ac7132a-7c10-58e6-96e5-669647db0fed', 'ro-ro', 'Prietenie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('30cc4769-3784-5dbb-8a76-2f79f6a8ef04', 'emo_sadness_loss', 'emotional_depth', 23, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c3cf4d07-28ff-5eb6-a579-ff1b71814ee7', '30cc4769-3784-5dbb-8a76-2f79f6a8ef04', 'en-us', 'Sadness and loss')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('dfd8d55c-e2ee-502e-9e8f-910e619f0245', '30cc4769-3784-5dbb-8a76-2f79f6a8ef04', 'hu-hu', 'Szomorúság és veszteség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('06cb89e7-d771-5387-adde-f8acbfb26cda', '30cc4769-3784-5dbb-8a76-2f79f6a8ef04', 'ro-ro', 'Tristețe și pierdere')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('e26d39e5-d3c5-51bd-8ced-1d6b15311671', 'int_choose_path', 'interactivity', 39, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('668f26af-f584-5865-8b95-35aa6346857c', 'e26d39e5-d3c5-51bd-8ced-1d6b15311671', 'en-us', 'Choose-your-path')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d3c2dfc4-97b6-5686-9ceb-3997eda41b59', 'e26d39e5-d3c5-51bd-8ced-1d6b15311671', 'hu-hu', 'Válaszd ki az utat')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('4d83a6b3-17cf-5448-b48b-088a6871a78b', 'e26d39e5-d3c5-51bd-8ced-1d6b15311671', 'ro-ro', 'Alege-ți drumul')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('42011449-5046-59e5-b815-0bba78e01cb3', 'pace_high_suspense', 'pace_and_action', 28, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('b327308a-5b47-5982-b483-869e3f7fe0b5', '42011449-5046-59e5-b815-0bba78e01cb3', 'en-us', 'High suspense')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('27030a2a-b71c-5477-ae51-824316c728fc', '42011449-5046-59e5-b815-0bba78e01cb3', 'hu-hu', 'Nagy feszültség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d4215011-b094-53a7-a517-baead1356007', '42011449-5046-59e5-b815-0bba78e01cb3', 'ro-ro', 'Mult suspans')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('3e82fd52-d712-5412-a0dd-7c114c74ad90', 'fun_slapstick', 'fun', 12, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('f61b7bc5-78c7-5488-bc56-f131cab1736a', '3e82fd52-d712-5412-a0dd-7c114c74ad90', 'en-us', 'Slapstick humor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('967cd719-9c0b-5e9c-8315-7a95a7cefdc0', '3e82fd52-d712-5412-a0dd-7c114c74ad90', 'hu-hu', 'Fizikai humor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('af0f228d-f82d-5a4d-93d5-76f591fee5f4', '3e82fd52-d712-5412-a0dd-7c114c74ad90', 'ro-ro', 'Umor fizic')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('f583b083-7f84-5d48-940e-612ee0cfeb21', 'val_honesty', 'values_and_morals', 44, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('fe30d3c5-7fe5-5cff-9d45-20c225aa346d', 'f583b083-7f84-5d48-940e-612ee0cfeb21', 'en-us', 'Honesty')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a5950490-31fb-501c-ae62-b81f4ba47bda', 'f583b083-7f84-5d48-940e-612ee0cfeb21', 'hu-hu', 'Őszinteség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ec0374f1-7cf2-5f63-bee8-25dc4edee517', 'f583b083-7f84-5d48-940e-612ee0cfeb21', 'ro-ro', 'Onestitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('bc1ddf7a-8b6b-5adc-a094-5012b1577975', 'pace_balanced', 'pace_and_action', 26, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('1e810799-ac4f-5a23-859e-934d16eddc2b', 'bc1ddf7a-8b6b-5adc-a094-5012b1577975', 'en-us', 'Balanced pace')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('cbdb893b-3aec-5d2b-9339-bccfc202cc80', 'bc1ddf7a-8b6b-5adc-a094-5012b1577975', 'hu-hu', 'Kiegyensúlyozott')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e80677ab-1e8d-5eda-af0d-5fde98c7acc4', 'bc1ddf7a-8b6b-5adc-a094-5012b1577975', 'ro-ro', 'Echilibrat')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('1b4a0d84-4148-53ce-baa6-fc82331d101e', 'fun_wordplay', 'fun', 11, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('8b0c87ac-2420-59c7-9830-c4e7573b7469', '1b4a0d84-4148-53ce-baa6-fc82331d101e', 'en-us', 'Wordplay humor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('7bd11e25-80f0-5aec-8a12-d11c52aa1540', '1b4a0d84-4148-53ce-baa6-fc82331d101e', 'hu-hu', 'Szójáték')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('6ec3bc61-4120-599e-9980-7b7090ac7386', '1b4a0d84-4148-53ce-baa6-fc82331d101e', 'ro-ro', 'Jocuri de cuvinte')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('3f6a2bdd-4a02-5de7-bb9a-883e38ac2813', 'emo_conflict', 'emotional_depth', 20, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('6541dfe6-3ad9-55c5-9dc8-e0f3c0b89888', '3f6a2bdd-4a02-5de7-bb9a-883e38ac2813', 'en-us', 'Dealing with conflict')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d97dc1cf-7bba-568d-884a-a35e11bbe267', '3f6a2bdd-4a02-5de7-bb9a-883e38ac2813', 'hu-hu', 'Konfliktuskezelés')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('f0bf433e-afbc-54d0-add6-755401cb7452', '3f6a2bdd-4a02-5de7-bb9a-883e38ac2813', 'ro-ro', 'Gestionarea conflictului')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('cdb2465e-322e-52cc-9c7c-8d7ecc89b492', 'val_respect_others', 'values_and_morals', 46, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('63cdb0df-4860-5fb5-b365-5d86deac9b4b', 'cdb2465e-322e-52cc-9c7c-8d7ecc89b492', 'en-us', 'Respect for others')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('64633724-2e49-5685-9b37-bd7de1fe9b71', 'cdb2465e-322e-52cc-9c7c-8d7ecc89b492', 'hu-hu', 'Tisztelet mások iránt')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('df247c0a-be9a-5cb6-aa4f-1b319dcf094b', 'cdb2465e-322e-52cc-9c7c-8d7ecc89b492', 'ro-ro', 'Respect pentru ceilalți')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('8cb5ff9e-9c3f-5ff3-a0d0-c8eae0d74170', 'edu_creativity', 'educational', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('998095da-88fe-5d19-8535-3bcdd19b998d', '8cb5ff9e-9c3f-5ff3-a0d0-c8eae0d74170', 'en-us', 'Creativity')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ced4adb8-0cd5-53a3-9465-c5b206384eb4', '8cb5ff9e-9c3f-5ff3-a0d0-c8eae0d74170', 'hu-hu', 'Kreativitás')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('108b2030-39d7-54aa-bbfb-0c5f9962c3b8', '8cb5ff9e-9c3f-5ff3-a0d0-c8eae0d74170', 'ro-ro', 'Creativitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('48a4660b-92b1-523a-ae2e-f861b250b09c', 'int_draw_scene', 'interactivity', 41, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('68e7188c-612e-5d14-94a9-57f315a6262b', '48a4660b-92b1-523a-ae2e-f861b250b09c', 'en-us', 'Draw your own scene')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('21813623-6398-5684-a6ac-3e2f342f4713', '48a4660b-92b1-523a-ae2e-f861b250b09c', 'hu-hu', 'Rajzold meg a saját jeleneted')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('9f86706d-cfdf-5756-8903-17f380e63452', '48a4660b-92b1-523a-ae2e-f861b250b09c', 'ro-ro', 'Desenează scena ta')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('c1820a49-c728-5596-b7d1-d094f83e6879', 'int_physical_actions', 'interactivity', 43, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a6d96bb5-beb3-50e0-938f-4cb52a2049c4', 'c1820a49-c728-5596-b7d1-d094f83e6879', 'en-us', 'Physical actions for kids')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ccea07ce-accb-5f2f-ab3e-5e768c2e77b4', 'c1820a49-c728-5596-b7d1-d094f83e6879', 'hu-hu', 'Fizikai akciók gyerekeknek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('35501338-c353-50d6-80a7-445506e491c7', 'c1820a49-c728-5596-b7d1-d094f83e6879', 'ro-ro', 'Acțiuni fizice (ridică-te, sari)')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('7dbc4fa5-7faa-55a5-95b1-b36df7306ebd', 'pace_dialogue', 'pace_and_action', 30, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d99bbd48-b294-5382-8271-7aa162ffecb6', '7dbc4fa5-7faa-55a5-95b1-b36df7306ebd', 'en-us', 'Dialogue-focused')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('87f0880d-49c2-5808-8d6a-c8d2ad288cea', '7dbc4fa5-7faa-55a5-95b1-b36df7306ebd', 'hu-hu', 'Párbeszéd-központú')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('52571325-b168-51a5-ae32-fe47db418b72', '7dbc4fa5-7faa-55a5-95b1-b36df7306ebd', 'ro-ro', 'Dialog centric')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('9b208e41-166f-5a3a-bec4-db9952a9720f', 'val_learn_from_mistakes', 'values_and_morals', 52, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('8fcc2670-951b-599a-a4d5-f3f73395a6a0', '9b208e41-166f-5a3a-bec4-db9952a9720f', 'en-us', 'Learning from mistakes')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0b286f76-ebf4-5266-993c-03ea8bd21d6c', '9b208e41-166f-5a3a-bec4-db9952a9720f', 'hu-hu', 'Tanulás a hibákból')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('280c95e0-d8c2-5778-bccd-07ab30d9518c', '9b208e41-166f-5a3a-bec4-db9952a9720f', 'ro-ro', 'Învățare din greșeli')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('0903b140-b847-5033-98ab-9c9f7b25f904', 'cx_very_simple_linear', 'complexity', 31, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('1c40bcb9-9be1-5cbd-a5fb-fec9aa2dedf9', '0903b140-b847-5033-98ab-9c9f7b25f904', 'en-us', 'Very simple linear')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('974d80a1-1d4f-5125-98d8-af460c672f9d', '0903b140-b847-5033-98ab-9c9f7b25f904', 'hu-hu', 'Nagyon egyszerű, lineáris')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0cdeea8b-6f48-5b72-89a1-74585bbcba9a', '0903b140-b847-5033-98ab-9c9f7b25f904', 'ro-ro', 'Foarte simplă, liniară')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('b4175b85-110b-56a9-8f00-46ccdeadc76b', 'int_write_ending', 'interactivity', 42, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('6cb9d572-4349-5fe3-9caf-1c3031ec48bc', 'b4175b85-110b-56a9-8f00-46ccdeadc76b', 'en-us', 'Write your own ending')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('9b00f6d1-16d9-5be7-8a15-516cc330ab17', 'b4175b85-110b-56a9-8f00-46ccdeadc76b', 'hu-hu', 'Írd meg a saját végét')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('40d0f5e0-55b8-55a4-8d8f-4dd62af30291', 'b4175b85-110b-56a9-8f00-46ccdeadc76b', 'ro-ro', 'Scrie-ți propriul final')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('dc76e651-b50f-52b8-99ff-21055ed0d4d6', 'emo_courage', 'emotional_depth', 19, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e6ccb9e7-b8fb-5683-b443-41fe92305dc5', 'dc76e651-b50f-52b8-99ff-21055ed0d4d6', 'en-us', 'Courage')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0cc7d4f0-7f40-55ee-99f8-3d65cf2e2086', 'dc76e651-b50f-52b8-99ff-21055ed0d4d6', 'hu-hu', 'Bátorság')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('3b8ff8b7-8bc7-55a6-877c-9731259e699f', 'dc76e651-b50f-52b8-99ff-21055ed0d4d6', 'ro-ro', 'Curaj')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('bf5ebead-eb30-5094-bcaf-1c2aa192972c', 'val_respect_nature', 'values_and_morals', 47, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('7e332a32-aed9-5e1b-bb5f-304e1f3294a4', 'bf5ebead-eb30-5094-bcaf-1c2aa192972c', 'en-us', 'Respect for nature')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('560e79e8-bb06-5006-8027-091fde2d2bac', 'bf5ebead-eb30-5094-bcaf-1c2aa192972c', 'hu-hu', 'Tisztelet a természet iránt')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a95ad711-41c7-58ac-84d3-0d12c77ef666', 'bf5ebead-eb30-5094-bcaf-1c2aa192972c', 'ro-ro', 'Respect pentru natură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('7532525c-052b-514f-9c21-fb4f847297fd', 'edu_science', 'educational', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a311fdbe-b4c1-5185-a181-3a0f4577d568', '7532525c-052b-514f-9c21-fb4f847297fd', 'en-us', 'Science (bio/chem/phys)')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('b4e099d6-38b5-5574-ad72-a4bb361d1edf', '7532525c-052b-514f-9c21-fb4f847297fd', 'hu-hu', 'Tudományok')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a9e693fc-cf2c-51a3-bf65-a9694554404a', '7532525c-052b-514f-9c21-fb4f847297fd', 'ro-ro', 'Științe')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('e0de3f3f-31a7-5efa-81b5-dc7e20339293', 'fun_silly_chars', 'fun', 13, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('3fbc82aa-16db-5759-a6b1-cb7510a95714', 'e0de3f3f-31a7-5efa-81b5-dc7e20339293', 'en-us', 'Silly characters')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('c20df560-8951-5c07-99ef-8aa53a1ff78e', 'e0de3f3f-31a7-5efa-81b5-dc7e20339293', 'hu-hu', 'Vicces karakterek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('d8f38919-ba4b-5399-aaf1-5641b4a513a7', 'e0de3f3f-31a7-5efa-81b5-dc7e20339293', 'ro-ro', 'Personaje trăznite')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('02075f0a-61de-5757-af93-57ff77562677', 'edu_literature', 'educational', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('51bcfbda-0c9c-5cca-a7c5-837fe78db0d0', '02075f0a-61de-5757-af93-57ff77562677', 'en-us', 'Literature')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('e0c2a58b-66c1-5be5-b54f-678dc296a034', '02075f0a-61de-5757-af93-57ff77562677', 'hu-hu', 'Irodalom')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a18b89e3-6ad1-5d79-894c-e17620549d54', '02075f0a-61de-5757-af93-57ff77562677', 'ro-ro', 'Literatură')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('518a803b-6b99-5f9b-ad36-d58bcebe259b', 'int_quizzes', 'interactivity', 38, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('0dd89b7a-a4b8-50a7-ba8c-cf4cac6e3d74', '518a803b-6b99-5f9b-ad36-d58bcebe259b', 'en-us', 'Quizzes')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('410e0615-4163-5ae7-99e7-889394b07961', '518a803b-6b99-5f9b-ad36-d58bcebe259b', 'hu-hu', 'Kvízek')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('95cfcb05-ba0e-50bf-9923-17c41cd2c0cf', '518a803b-6b99-5f9b-ad36-d58bcebe259b', 'ro-ro', 'Quiz-uri')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('98a4f7c3-2512-5c1e-aaa2-34504d6fb099', 'classic_author', 'classic', 53, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('ad5b33cc-7e69-5e42-b5d8-ff06a1b08fbd', '98a4f7c3-2512-5c1e-aaa2-34504d6fb099', 'en-us', 'Author')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('1e1b9ded-d447-5e9e-9fea-5d07b7356a50', '98a4f7c3-2512-5c1e-aaa2-34504d6fb099', 'hu-hu', 'Író')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('8d2a905d-5b16-5262-9375-d7408c0b7635', '98a4f7c3-2512-5c1e-aaa2-34504d6fb099', 'ro-ro', 'Scriitor')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('62ddce28-3a17-55c5-b1bc-1635f87a1e97', 'val_inclusivity', 'values_and_morals', 50, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('2673b50d-055e-5d5e-9ea5-974448bb2a12', '62ddce28-3a17-55c5-b1bc-1635f87a1e97', 'en-us', 'Inclusivity and diversity')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('9836e629-2d67-52ec-acbe-c09b45e1136e', '62ddce28-3a17-55c5-b1bc-1635f87a1e97', 'hu-hu', 'Befogadás és sokszínűség')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('197d35d2-9b80-52fd-b88b-d3517113b63f', '62ddce28-3a17-55c5-b1bc-1635f87a1e97', 'ro-ro', 'Incluziune și diversitate')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopics"
    ("Id", "TopicId", "DimensionId", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('31a1685c-b7cd-566d-951a-7028478edf6c', 'emo_empathy', 'emotional_depth', 18, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("TopicId") DO UPDATE
SET "DimensionId" = EXCLUDED."DimensionId",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('08b1f6a2-f281-536a-9451-833748205efd', '31a1685c-b7cd-566d-951a-7028478edf6c', 'en-us', 'Empathy')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('a1f8ffe9-56b9-53db-8d17-2aeb08f5417d', '31a1685c-b7cd-566d-951a-7028478edf6c', 'hu-hu', 'Empátia')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."StoryTopicTranslations"
    ("Id", "StoryTopicId", "LanguageCode", "Label")
VALUES
    ('4253cd43-71a2-5e7c-907c-cf8b848ac7b1', '31a1685c-b7cd-566d-951a-7028478edf6c', 'ro-ro', 'Empatie')
ON CONFLICT ("StoryTopicId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
-- Story Age Groups
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('a9ed4050-16db-52dd-a49c-5910f7f3bd84', 'middle_school_9_12', 9, 12, 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('02b2252a-fbd9-5516-a33f-d80eaee9e3e6', 'a9ed4050-16db-52dd-a49c-5910f7f3bd84', 'en-us', 'Middle school', 'Stories with more complex conflict, subtler humor, first moral dilemmas and richer science-fantasy.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('4e5ac7ba-b818-5c80-922d-cb7bcf113170', 'a9ed4050-16db-52dd-a49c-5910f7f3bd84', 'hu-hu', 'Nagyiskolás', 'Történetek összetettebb konfliktussal, finomabb humorral, első erkölcsi dilemmákkal és gazdagabb sci-fi fantasy elemekkel.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('a74b06bf-d0e6-5bd0-bdda-a881474eb0fc', 'a9ed4050-16db-52dd-a49c-5910f7f3bd84', 'ro-ro', 'Copii mari', 'Povești cu conflict mai complex, umor mai subtil, primele dileme morale și science-fantasy mai bogat.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('f5b97f49-1f40-569d-bf00-186f8a9b0c32', 'teens_13_16', 13, 16, 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('344511c8-678a-521e-b233-fcd07ffb4a26', 'f5b97f49-1f40-569d-bf00-186f8a9b0c32', 'en-us', 'Teens', 'Stories with identity themes, nuanced morality, symbolism and increased narrative complexity.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('813096a9-b5b6-545d-93d5-f1059bde52ce', 'f5b97f49-1f40-569d-bf00-186f8a9b0c32', 'hu-hu', 'Serdülő', 'Történetek identitási témákkal, árnyalt erkölcsösséggel, szimbolizmussal és megnövelt narratív komplexitással.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('994a238f-5c88-592f-b1d0-ff0b6229cc4c', 'f5b97f49-1f40-569d-bf00-186f8a9b0c32', 'ro-ro', 'Adolescenți', 'Povești cu teme identitare, moralitate nuanțată, simbolism și complexitate narativă crescută.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('0ecda95c-08ba-597a-a9d5-6e3b999df4db', 'early_school_6_8', 6, 8, 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('186d2605-5728-5b2b-83c2-5ab6e8b50c0c', '0ecda95c-08ba-597a-a9d5-6e3b999df4db', 'en-us', 'Early school', 'Stories with a clear narrative thread, memorable characters, first explicit educational concepts.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('70adeb77-c296-5aa9-9c8b-d867a3fb54c8', '0ecda95c-08ba-597a-a9d5-6e3b999df4db', 'hu-hu', 'Kisiskolás', 'Történetek világos narratív szállal, emlékezetes karakterekkel, első explicit oktatási fogalmakkal.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('37908dee-62aa-5217-a1f6-b7e18e5b270e', '0ecda95c-08ba-597a-a9d5-6e3b999df4db', 'ro-ro', 'Școlari mici', 'Povești cu un fir narativ clar, personaje memorabile, prime noțiuni educaționale explicite.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroups"
    ("Id", "AgeGroupId", "MinAge", "MaxAge", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('70075176-c3e9-5053-933c-6406b55813bd', 'preschool_3_5', 3, 5, 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AgeGroupId") DO UPDATE
SET "MinAge" = EXCLUDED."MinAge",
    "MaxAge" = EXCLUDED."MaxAge",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('cdb129b1-9c57-5fa3-8d0c-01a68924a9dd', '70075176-c3e9-5053-933c-6406b55813bd', 'en-us', 'Preschool', 'Very simple stories, lots of images, short sentences, physical interactivity and clear emotions.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('d46445fb-acaf-5f56-937a-c0ea29a7a31a', '70075176-c3e9-5053-933c-6406b55813bd', 'hu-hu', 'Óvodás', 'Nagyon egyszerű történetek, sok kép, rövid mondatok, fizikai interaktivitás és világos érzelmek.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
INSERT INTO alchimalia_schema."StoryAgeGroupTranslations"
    ("Id", "StoryAgeGroupId", "LanguageCode", "Label", "Description")
VALUES
    ('aaaa96c3-6e55-59bf-8084-f402a6b6f446', '70075176-c3e9-5053-933c-6406b55813bd', 'ro-ro', 'Preșcolari', 'Povești foarte simple, multe imagini, propoziții scurte, interactivitate fizică și emoții clare.')
ON CONFLICT ("StoryAgeGroupId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description";
-- Classic Authors
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('f6974222-d106-569b-b9de-d404689a45dd', 'ion-creanga', 'Ion Creangă', 'ro-ro', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('f4232262-0d05-50fa-88bc-288f9f251cc7', 'petre-ispirescu', 'Petre Ispirescu', 'ro-ro', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('85394f23-0ba4-5131-8f37-079f650dfba9', 'ioan-slavici', 'Ioan Slavici', 'ro-ro', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('96d94d9a-6d0b-5f7a-b6de-f26ef96d721c', 'mihai-eminescu', 'Mihai Eminescu', 'ro-ro', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('ae7d9d3b-84cd-595e-9a0d-a0184afee044', 'barbu-stefanescu-delavrancea', 'Barbu Ștefănescu Delavrancea', 'ro-ro', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('ddeb7270-6ffe-5c28-8dc5-9c27e22fccf8', 'emil-garleanu', 'Emil Gârleanu', 'ro-ro', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('8ba6b3f9-b32f-5b9b-a1ef-a394ad784e3e', 'i-al-bratescu-voinesti', 'I. Al. Brătescu-Voinești', 'ro-ro', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('a4b400cc-f72b-5b85-97e6-ba05f61587d5', 'calin-gruia', 'Călin Gruia', 'ro-ro', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('662b1fa6-edc2-524e-9858-23397279d1bf', 'otilia-cazimir', 'Otilia Cazimir', 'ro-ro', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('a7bef6f5-2519-5b4c-818b-3b69b0f3cf03', 'dumitru-almas', 'Dumitru Almaș', 'ro-ro', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('719be274-864a-51ee-9d26-144135b9a2ca', 'cezar-petrescu', 'Cezar Petrescu', 'ro-ro', 10, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('a5aaed81-af48-5a5a-8736-98e350e324b9', 'gellu-naum', 'Gellu Naum', 'ro-ro', 11, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('4b4ca94f-fe3e-514a-a440-dfd6450aeed1', 'ana-blandiana', 'Ana Blandiana', 'ro-ro', 12, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('dff68880-6d69-5b1d-ab14-fcb5b4186f0a', 'mircea-santimbreanu', 'Mircea Sântimbreanu', 'ro-ro', 13, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('da30d9d4-9c88-5598-960d-3b0de94dc5d9', 'octav-pancu-iasi', 'Octav Pancu-Iași', 'ro-ro', 14, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('32e72ffd-112c-569b-9934-96a3735f3b2f', 'lucia-olteanu', 'Lucia Olteanu', 'ro-ro', 15, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('e0271e0d-ad5a-5e82-a227-b997a8d83199', 'alexandru-mitru', 'Alexandru Mitru', 'ro-ro', 16, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('39256a3f-2fa7-5f4b-9e10-e4a70a184e68', 'mihail-sadoveanu', 'Mihail Sadoveanu', 'ro-ro', 17, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('60b14af3-c559-5d24-86e9-4509aa8103e4', 'gala-galaction', 'Gala Galaction', 'ro-ro', 18, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('f72c0160-08cd-5fb4-baf0-07952be4cdf8', 'grigore-bajenaru', 'Grigore Băjenaru', 'ro-ro', 19, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('b9da83e5-9ce3-534c-82b0-2e4f262da9e6', 'benedek-elek', 'Benedek Elek', 'hu-hu', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('b6804511-4c27-5427-9226-8d6f133b0c7f', 'istvan-fekete', 'István Fekete', 'hu-hu', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('9e2ca344-c407-5458-a526-287eb6c73351', 'ervin-lazar', 'Ervin Lázár', 'hu-hu', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('2ceffb2e-04e5-5435-8af3-a42c5df11730', 'mihaly-fazekas', 'Mihály Fazekas', 'hu-hu', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('3069599e-0c27-5730-ae77-833e5b7f8c2e', 'magda-szabo', 'Magda Szabó', 'hu-hu', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('d222c790-b2f8-5f76-b4b5-3e18508de740', 'zsigmond-moricz', 'Zsigmond Móricz', 'hu-hu', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('756ea1e0-1823-5f6c-a3ad-b3704a8ded57', 'sandor-weores', 'Sándor Weöres', 'hu-hu', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('b8ba84c2-5632-591c-ab79-4574ec9a7d7a', 'sandor-petofi', 'Sándor Petőfi', 'hu-hu', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('274cc740-404f-5f29-b399-6a4d2e774c13', 'kalman-mikszath', 'Kálmán Mikszáth', 'hu-hu', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('fb861a69-f9be-5dbf-a209-08ad4d6e85f1', 'jozsef-attila', 'József Attila', 'hu-hu', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('29555553-29be-53fb-9f64-339ab57c74ae', 'lewis-carroll', 'Lewis Carroll', 'en-us', 0, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('e343a0dd-90b0-509a-aa51-030f75ae49af', 'beatrix-potter', 'Beatrix Potter', 'en-us', 1, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('36a6ad98-4d4d-5d13-8a5c-44c393fa1f33', 'a-a-milne', 'A. A. Milne', 'en-us', 2, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('3a5832d9-5936-5426-9257-f50b43c92efc', 'j-m-barrie', 'J. M. Barrie', 'en-us', 3, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('be41ec70-94a0-575d-bb0d-4bfd2b6e8b8c', 'l-frank-baum', 'L. Frank Baum', 'en-us', 4, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('512ea5e5-c573-5f83-8d28-1fd69645dd17', 'dr-seuss', 'Dr. Seuss', 'en-us', 5, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('36d46910-3741-5019-aa99-44938e1b48e1', 'roald-dahl', 'Roald Dahl', 'en-us', 6, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('411f7948-da44-5340-8729-a4a3282d9ce3', 'c-s-lewis', 'C. S. Lewis', 'en-us', 7, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('25ca35d9-fcd8-51f7-b3e0-29c326013f10', 'e-b-white', 'E. B. White', 'en-us', 8, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('8cabedae-65c8-5ec0-93f2-7bed6e54b149', 'shel-silverstein', 'Shel Silverstein', 'en-us', 9, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('df17c4e0-cef2-52da-a87a-6771fce14fe4', 'maurice-sendak', 'Maurice Sendak', 'en-us', 10, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('921bf25d-5a7d-5f12-bc11-abfcbf1c7e73', 'j-k-rowling', 'J. K. Rowling', 'en-us', 11, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."ClassicAuthors"
    ("Id", "AuthorId", "Name", "LanguageCode", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    ('bea64c6f-53b3-5b37-bfde-da375b802953', 'enid-blyton', 'Enid Blyton', 'en-us', 12, TRUE, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("AuthorId") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "LanguageCode" = EXCLUDED."LanguageCode",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
COMMIT;
