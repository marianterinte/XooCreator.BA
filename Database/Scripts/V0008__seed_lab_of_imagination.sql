-- Auto-generated from Data/SeedData/LaboratoryOfImagination
-- Run date: 2025-11-27 10:07:23+02:00

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp'
    ) THEN
        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    END IF;
END $$;

BEGIN;

-- Body parts (base locale)
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('arms', 'Arms', 'images/bodyparts/hands.webp', FALSE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('body', 'Body', 'images/bodyparts/body.webp', FALSE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('head', 'Head', 'images/bodyparts/face.webp', FALSE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('horn', 'Horn', 'images/bodyparts/horn.webp', TRUE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('horns', 'Horns', 'images/bodyparts/horns.webp', TRUE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('legs', 'Legs', 'images/bodyparts/legs.webp', TRUE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('tail', 'Tail', 'images/bodyparts/tail.webp', TRUE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";
INSERT INTO alchimalia_schema."BodyParts"
    ("Key", "Name", "Image", "IsBaseLocked")
VALUES
    ('wings', 'Wings', 'images/bodyparts/wings.webp', TRUE)
ON CONFLICT ("Key") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Image" = EXCLUDED."Image",
    "IsBaseLocked" = EXCLUDED."IsBaseLocked";

-- Body part translations per locale
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:arms:en-us'), 'arms', 'en-us', 'Arms')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:body:en-us'), 'body', 'en-us', 'Body')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:head:en-us'), 'head', 'en-us', 'Head')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:horn:en-us'), 'horn', 'en-us', 'Horn')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:horns:en-us'), 'horns', 'en-us', 'Horns')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:legs:en-us'), 'legs', 'en-us', 'Legs')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:tail:en-us'), 'tail', 'en-us', 'Tail')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:wings:en-us'), 'wings', 'en-us', 'Wings')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:arms:hu-hu'), 'arms', 'hu-hu', 'Karok')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:body:hu-hu'), 'body', 'hu-hu', 'Test')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:head:hu-hu'), 'head', 'hu-hu', 'Fej')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:legs:hu-hu'), 'legs', 'hu-hu', 'Lábak')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:tail:hu-hu'), 'tail', 'hu-hu', 'Farok')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:arms:ro-ro'), 'arms', 'ro-ro', 'Arms')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:body:ro-ro'), 'body', 'ro-ro', 'Body')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:head:ro-ro'), 'head', 'ro-ro', 'Head')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:horn:ro-ro'), 'horn', 'ro-ro', 'Horn')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:horns:ro-ro'), 'horns', 'ro-ro', 'Horns')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:legs:ro-ro'), 'legs', 'ro-ro', 'Legs')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:tail:ro-ro'), 'tail', 'ro-ro', 'Tail')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'bodypart:wings:ro-ro'), 'wings', 'ro-ro', 'Wings')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";

-- Regions (base locale)
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Sahara')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000002', 'Jungle')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000003', 'Farm')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000004', 'Savanna')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000005', 'Forest')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000006', 'Wetlands')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."Regions"
    ("Id", "Name")
VALUES
    ('10000000-0000-0000-0000-000000000007', 'Mountains')
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name";

-- Animals (base locale)
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000001', 'Bunny', 'images/animals/base/bunny.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000002', 'Hippo', 'images/animals/base/hippo.jpg', FALSE, '10000000-0000-0000-0000-000000000006')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'Giraffe', 'images/animals/base/giraffe.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000004', 'Dog', 'images/animals/base/dog.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000005', 'Fox', 'images/animals/base/fox.jpg', FALSE, '10000000-0000-0000-0000-000000000005')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000006', 'Cat', 'images/animals/base/cat.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000007', 'Monkey', 'images/animals/base/monkey.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000008', 'Camel', 'images/animals/base/camel.jpg', FALSE, '10000000-0000-0000-0000-000000000001')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'Deer', 'images/animals/base/deer.jpg', FALSE, '10000000-0000-0000-0000-000000000005')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'Duck', 'images/animals/base/duck.jpg', FALSE, '10000000-0000-0000-0000-000000000006')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'Eagle', 'images/animals/base/eagle.jpg', FALSE, '10000000-0000-0000-0000-000000000007')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000000c', 'Elephant', 'images/animals/base/elephant.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'Ostrich', 'images/animals/base/ostrich.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'Parrot', 'images/animals/base/parrot.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000000f', 'Jaguar', 'images/animals/base/jaguar.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'Toucan', 'images/animals/base/toucan.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000011', 'Anaconda', 'images/animals/base/anaconda.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000012', 'Capuchin Monkey', 'images/animals/base/capuchin_monkey.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000013', 'Poison Dart Frog', 'images/animals/base/poison_dart_frog.jpg', FALSE, '10000000-0000-0000-0000-000000000002')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000014', 'Lion', 'images/animals/base/lion.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000015', 'African Elephant', 'images/animals/base/african_elephant.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000016', 'Giraffe', 'images/animals/base/giraffe.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000017', 'Zebra', 'images/animals/base/zebra.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'Rhinoceros', 'images/animals/base/rhinoceros.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000019', 'Bison', 'images/animals/base/bison.jpg', FALSE, '10000000-0000-0000-0000-000000000007')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'Saiga Antelope', 'images/animals/base/saiga_antelope.jpg', FALSE, '10000000-0000-0000-0000-000000000004')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000001b', 'Gray Wolf', 'images/animals/base/gray_wolf.jpg', FALSE, '10000000-0000-0000-0000-000000000005')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000001c', 'Przewalski''s Horse', 'images/animals/base/przewalski_horse.jpg', FALSE, '10000000-0000-0000-0000-000000000007')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'Steppe Eagle', 'images/animals/base/steppe_eagle.jpg', FALSE, '10000000-0000-0000-0000-000000000007')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000001e', 'Cow', 'images/animals/base/cow.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-00000000001f', 'Sheep', 'images/animals/base/sheep.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000020', 'Horse', 'images/animals/base/horse.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'Chicken', 'images/animals/base/chicken.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";
INSERT INTO alchimalia_schema."Animals"
    ("Id", "Label", "Src", "IsHybrid", "RegionId")
VALUES
    ('00000000-0000-0000-0000-000000000022', 'Pig', 'images/animals/base/pig.jpg', FALSE, '10000000-0000-0000-0000-000000000003')
ON CONFLICT ("Id") DO UPDATE
SET "Label" = EXCLUDED."Label",
    "Src" = EXCLUDED."Src",
    "IsHybrid" = EXCLUDED."IsHybrid",
    "RegionId" = EXCLUDED."RegionId";

-- Animal translations per locale
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000001:en-us'), '00000000-0000-0000-0000-000000000001', 'en-us', 'Bunny')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000002:en-us'), '00000000-0000-0000-0000-000000000002', 'en-us', 'Hippo')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000003:en-us'), '00000000-0000-0000-0000-000000000003', 'en-us', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000004:en-us'), '00000000-0000-0000-0000-000000000004', 'en-us', 'Dog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000005:en-us'), '00000000-0000-0000-0000-000000000005', 'en-us', 'Fox')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000006:en-us'), '00000000-0000-0000-0000-000000000006', 'en-us', 'Cat')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000007:en-us'), '00000000-0000-0000-0000-000000000007', 'en-us', 'Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000008:en-us'), '00000000-0000-0000-0000-000000000008', 'en-us', 'Camel')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000009:en-us'), '00000000-0000-0000-0000-000000000009', 'en-us', 'Deer')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000a:en-us'), '00000000-0000-0000-0000-00000000000a', 'en-us', 'Duck')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000b:en-us'), '00000000-0000-0000-0000-00000000000b', 'en-us', 'Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000c:en-us'), '00000000-0000-0000-0000-00000000000c', 'en-us', 'Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000d:en-us'), '00000000-0000-0000-0000-00000000000d', 'en-us', 'Ostrich')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000e:en-us'), '00000000-0000-0000-0000-00000000000e', 'en-us', 'Parrot')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000f:en-us'), '00000000-0000-0000-0000-00000000000f', 'en-us', 'Jaguar')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000010:en-us'), '00000000-0000-0000-0000-000000000010', 'en-us', 'Toucan')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000011:en-us'), '00000000-0000-0000-0000-000000000011', 'en-us', 'Anaconda')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000012:en-us'), '00000000-0000-0000-0000-000000000012', 'en-us', 'Capuchin Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000013:en-us'), '00000000-0000-0000-0000-000000000013', 'en-us', 'Poison Dart Frog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000014:en-us'), '00000000-0000-0000-0000-000000000014', 'en-us', 'Lion')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000015:en-us'), '00000000-0000-0000-0000-000000000015', 'en-us', 'African Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000016:en-us'), '00000000-0000-0000-0000-000000000016', 'en-us', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000017:en-us'), '00000000-0000-0000-0000-000000000017', 'en-us', 'Zebra')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000018:en-us'), '00000000-0000-0000-0000-000000000018', 'en-us', 'Rhinoceros')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000019:en-us'), '00000000-0000-0000-0000-000000000019', 'en-us', 'Bison')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001a:en-us'), '00000000-0000-0000-0000-00000000001a', 'en-us', 'Saiga Antelope')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001b:en-us'), '00000000-0000-0000-0000-00000000001b', 'en-us', 'Gray Wolf')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001c:en-us'), '00000000-0000-0000-0000-00000000001c', 'en-us', 'Przewalski''s Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001d:en-us'), '00000000-0000-0000-0000-00000000001d', 'en-us', 'Steppe Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001e:en-us'), '00000000-0000-0000-0000-00000000001e', 'en-us', 'Cow')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001f:en-us'), '00000000-0000-0000-0000-00000000001f', 'en-us', 'Sheep')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000020:en-us'), '00000000-0000-0000-0000-000000000020', 'en-us', 'Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000021:en-us'), '00000000-0000-0000-0000-000000000021', 'en-us', 'Chicken')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000022:en-us'), '00000000-0000-0000-0000-000000000022', 'en-us', 'Pig')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000001:hu-hu'), '00000000-0000-0000-0000-000000000001', 'hu-hu', 'Nyuszi')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000002:hu-hu'), '00000000-0000-0000-0000-000000000002', 'hu-hu', 'Víziló')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000003:hu-hu'), '00000000-0000-0000-0000-000000000003', 'hu-hu', 'Zsiráf')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000004:hu-hu'), '00000000-0000-0000-0000-000000000004', 'hu-hu', 'Kutya')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000005:hu-hu'), '00000000-0000-0000-0000-000000000005', 'hu-hu', 'Róka')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000006:hu-hu'), '00000000-0000-0000-0000-000000000006', 'hu-hu', 'Macska')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000007:hu-hu'), '00000000-0000-0000-0000-000000000007', 'hu-hu', 'Majom')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000001:ro-ro'), '00000000-0000-0000-0000-000000000001', 'ro-ro', 'Bunny')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000002:ro-ro'), '00000000-0000-0000-0000-000000000002', 'ro-ro', 'Hippo')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000003:ro-ro'), '00000000-0000-0000-0000-000000000003', 'ro-ro', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000004:ro-ro'), '00000000-0000-0000-0000-000000000004', 'ro-ro', 'Dog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000005:ro-ro'), '00000000-0000-0000-0000-000000000005', 'ro-ro', 'Fox')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000006:ro-ro'), '00000000-0000-0000-0000-000000000006', 'ro-ro', 'Cat')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000007:ro-ro'), '00000000-0000-0000-0000-000000000007', 'ro-ro', 'Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000008:ro-ro'), '00000000-0000-0000-0000-000000000008', 'ro-ro', 'Camel')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000009:ro-ro'), '00000000-0000-0000-0000-000000000009', 'ro-ro', 'Deer')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000a:ro-ro'), '00000000-0000-0000-0000-00000000000a', 'ro-ro', 'Duck')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000b:ro-ro'), '00000000-0000-0000-0000-00000000000b', 'ro-ro', 'Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000c:ro-ro'), '00000000-0000-0000-0000-00000000000c', 'ro-ro', 'Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000d:ro-ro'), '00000000-0000-0000-0000-00000000000d', 'ro-ro', 'Ostrich')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000e:ro-ro'), '00000000-0000-0000-0000-00000000000e', 'ro-ro', 'Parrot')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000000f:ro-ro'), '00000000-0000-0000-0000-00000000000f', 'ro-ro', 'Jaguar')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000010:ro-ro'), '00000000-0000-0000-0000-000000000010', 'ro-ro', 'Toucan')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000011:ro-ro'), '00000000-0000-0000-0000-000000000011', 'ro-ro', 'Anaconda')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000012:ro-ro'), '00000000-0000-0000-0000-000000000012', 'ro-ro', 'Capuchin Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000013:ro-ro'), '00000000-0000-0000-0000-000000000013', 'ro-ro', 'Poison Dart Frog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000014:ro-ro'), '00000000-0000-0000-0000-000000000014', 'ro-ro', 'Lion')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000015:ro-ro'), '00000000-0000-0000-0000-000000000015', 'ro-ro', 'African Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000016:ro-ro'), '00000000-0000-0000-0000-000000000016', 'ro-ro', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000017:ro-ro'), '00000000-0000-0000-0000-000000000017', 'ro-ro', 'Zebra')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000018:ro-ro'), '00000000-0000-0000-0000-000000000018', 'ro-ro', 'Rhinoceros')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000019:ro-ro'), '00000000-0000-0000-0000-000000000019', 'ro-ro', 'Bison')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001a:ro-ro'), '00000000-0000-0000-0000-00000000001a', 'ro-ro', 'Saiga Antelope')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001b:ro-ro'), '00000000-0000-0000-0000-00000000001b', 'ro-ro', 'Gray Wolf')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001c:ro-ro'), '00000000-0000-0000-0000-00000000001c', 'ro-ro', 'Przewalski''s Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001d:ro-ro'), '00000000-0000-0000-0000-00000000001d', 'ro-ro', 'Steppe Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001e:ro-ro'), '00000000-0000-0000-0000-00000000001e', 'ro-ro', 'Cow')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-00000000001f:ro-ro'), '00000000-0000-0000-0000-00000000001f', 'ro-ro', 'Sheep')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000020:ro-ro'), '00000000-0000-0000-0000-000000000020', 'ro-ro', 'Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000021:ro-ro'), '00000000-0000-0000-0000-000000000021', 'ro-ro', 'Chicken')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000'::uuid, 'animal:00000000-0000-0000-0000-000000000022:ro-ro'), '00000000-0000-0000-0000-000000000022', 'ro-ro', 'Pig')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";

-- Animal part supports (base locale)
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000001', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000001', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000001', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000002', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000002', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000002', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'horn')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'horns')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000003', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000004', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000004', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000004', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000005', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000005', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000005', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000006', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000006', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000006', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000007', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000007', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000007', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000008', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000008', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000008', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'horn')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'horns')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000009', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000a', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000b', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000c', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000c', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000c', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000d', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000e', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000f', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000f', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000000f', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000010', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000011', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000011', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000011', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000012', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000012', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000012', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000013', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000013', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000013', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000013', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000014', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000014', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000014', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000015', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000015', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000015', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000016', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000016', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000016', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000016', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000016', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000017', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000017', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000017', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000017', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000017', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'horn')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000018', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000019', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000019', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000019', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'horns')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001a', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001b', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001b', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001b', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001c', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001c', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001c', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001c', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001c', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001d', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001e', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001e', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001e', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001f', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001f', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-00000000001f', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000020', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000020', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000020', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000020', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000020', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'legs')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'tail')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000021', 'wings')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000022', 'arms')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000022', 'body')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;
INSERT INTO alchimalia_schema."AnimalPartSupports"
    ("AnimalId", "PartKey")
VALUES
    ('00000000-0000-0000-0000-000000000022', 'head')
ON CONFLICT ("AnimalId", "PartKey") DO NOTHING;

COMMIT;
