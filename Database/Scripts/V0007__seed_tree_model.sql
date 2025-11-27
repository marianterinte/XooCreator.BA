-- Auto-generated from Data/SeedData/TreeOfLight/*.json
-- Run date: 2025-11-27 12:11:42+02:00

BEGIN;

-- Tree configuration: puf-puf-journey-v1 (puf-puf-journey-v1.json)
INSERT INTO alchimalia_schema."TreeConfigurations"
    ("Id", "Name", "IsDefault")
VALUES
    ('puf-puf-journey-v1', 'Puf-Puf''s First Journey', TRUE)
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "IsDefault" = EXCLUDED."IsDefault";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('gateway', 'gateway', 'images/worlds/gateway.jpg', NULL, 0, FALSE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('terra', 'terra', 'images/worlds/terra.jpg', NULL, 1, FALSE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('lunaria', 'lunaria', 'images/worlds/lunaria.jpg', NULL, 2, FALSE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('mechanika', 'mechanika', 'images/worlds/pyron.jpg', NULL, 3, FALSE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('oceanica', 'oceanica', 'images/worlds/oceanica.jpg', NULL, 4, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('sylvaria', 'sylvaria', 'images/worlds/sylvaria.jpg', NULL, 5, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('crystalia', 'crystalia', 'images/worlds/crystalia.jpg', NULL, 6, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('zephyra', 'zephyra', 'images/worlds/zephyra.jpg', NULL, 7, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('pyron', 'pyron', 'images/worlds/pyron.jpg', NULL, 8, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('neptunia', 'neptunia', 'images/worlds/neptunia.jpg', NULL, 9, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('aetherion', 'aetherion', 'images/worlds/aetherion.jpg', NULL, 10, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeRegions"
    ("Id", "Label", "ImageUrl", "PufpufMessage", "SortOrder", "IsLocked", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    ('kelo-ketis', 'kelo-ketis', 'images/worlds/kelo-ketis.jpg', NULL, 11, TRUE, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "PufpufMessage" = EXCLUDED."PufpufMessage",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsLocked" = EXCLUDED."IsLocked",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (447496500, 'intro-pufpuf', 'gateway', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (964238639, 'terra-s1', 'terra', 'images/tol/stories/seed@alchimalia.com/terra-s1/0.Cover.png', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (261332114, 'terra-s2', 'terra', 'images/tol/stories/seed@alchimalia.com/terra-s2/0.Cover.png', 2, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (641828863, 'lunaria-s1', 'lunaria', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/0.Cover.png', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (1729976598, 'lunaria-s2', 'lunaria', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/0.Cover.png', 2, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (1338088729, 'mechanika-s1', 'mechanika', 'images/worlds/pyron.jpg', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (425715225, 'sylvaria-s1', 'sylvaria', 'images/worlds/sylvaria.jpg', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeStoryNodes"
    ("Id", "StoryId", "RegionId", "RewardImageUrl", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (1034585746, 'crystalia-s1', 'crystalia', 'images/worlds/crystalia.jpg', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("StoryId", "RegionId", "TreeConfigurationId") DO UPDATE
SET "RewardImageUrl" = EXCLUDED."RewardImageUrl",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeUnlockRules"
    ("Id", "Type", "FromId", "ToRegionId", "RequiredStoriesCsv", "MinCount", "StoryId", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (1790783080, 'story', 'gateway', 'terra', NULL, NULL, 'intro-pufpuf', 1, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "FromId" = EXCLUDED."FromId",
    "ToRegionId" = EXCLUDED."ToRegionId",
    "RequiredStoriesCsv" = EXCLUDED."RequiredStoriesCsv",
    "MinCount" = EXCLUDED."MinCount",
    "StoryId" = EXCLUDED."StoryId",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeUnlockRules"
    ("Id", "Type", "FromId", "ToRegionId", "RequiredStoriesCsv", "MinCount", "StoryId", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (1897462065, 'all', 'terra', 'lunaria', 'terra-s1,terra-s2', NULL, NULL, 2, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "FromId" = EXCLUDED."FromId",
    "ToRegionId" = EXCLUDED."ToRegionId",
    "RequiredStoriesCsv" = EXCLUDED."RequiredStoriesCsv",
    "MinCount" = EXCLUDED."MinCount",
    "StoryId" = EXCLUDED."StoryId",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeUnlockRules"
    ("Id", "Type", "FromId", "ToRegionId", "RequiredStoriesCsv", "MinCount", "StoryId", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (422392001, 'all', 'lunaria', 'mechanika', 'lunaria-s1,lunaria-s2', NULL, NULL, 3, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "FromId" = EXCLUDED."FromId",
    "ToRegionId" = EXCLUDED."ToRegionId",
    "RequiredStoriesCsv" = EXCLUDED."RequiredStoriesCsv",
    "MinCount" = EXCLUDED."MinCount",
    "StoryId" = EXCLUDED."StoryId",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeUnlockRules"
    ("Id", "Type", "FromId", "ToRegionId", "RequiredStoriesCsv", "MinCount", "StoryId", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (1708708753, 'story', 'mechanika', 'sylvaria', NULL, NULL, 'mechanika-s1', 4, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "FromId" = EXCLUDED."FromId",
    "ToRegionId" = EXCLUDED."ToRegionId",
    "RequiredStoriesCsv" = EXCLUDED."RequiredStoriesCsv",
    "MinCount" = EXCLUDED."MinCount",
    "StoryId" = EXCLUDED."StoryId",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."TreeUnlockRules"
    ("Id", "Type", "FromId", "ToRegionId", "RequiredStoriesCsv", "MinCount", "StoryId", "SortOrder", "TreeConfigurationId", "CreatedAt", "UpdatedAt")
VALUES
    (338712849, 'story', 'mechanika', 'crystalia', NULL, NULL, 'mechanika-s1', 5, 'puf-puf-journey-v1', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "FromId" = EXCLUDED."FromId",
    "ToRegionId" = EXCLUDED."ToRegionId",
    "RequiredStoriesCsv" = EXCLUDED."RequiredStoriesCsv",
    "MinCount" = EXCLUDED."MinCount",
    "StoryId" = EXCLUDED."StoryId",
    "SortOrder" = EXCLUDED."SortOrder",
    "UpdatedAt" = EXCLUDED."UpdatedAt";

COMMIT;
