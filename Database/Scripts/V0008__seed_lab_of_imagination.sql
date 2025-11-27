-- Auto-generated from Data/SeedData/LaboratoryOfImagination
-- Run date: 2025-11-27 12:00:45+02:00

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
    ('34aad965-979f-522f-b373-0bec09d41c02', 'arms', 'en-us', 'Arms')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('ebe2e344-7cc4-5734-91ce-16837f6a3a60', 'body', 'en-us', 'Body')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('749c09f3-37c7-5ca1-bb0b-604e662099ed', 'head', 'en-us', 'Head')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('001043f6-e38d-5ed1-8af1-f8bb4df77345', 'horn', 'en-us', 'Horn')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('fafe417f-7bde-5702-b108-5fa2bc9ff274', 'horns', 'en-us', 'Horns')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('9c86d4a1-9508-5a39-bd0c-49f70afcb21c', 'legs', 'en-us', 'Legs')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('ac24dbf4-3476-58aa-949a-39b349284471', 'tail', 'en-us', 'Tail')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('10523761-eb5b-5801-9b75-0422acc7bcbe', 'wings', 'en-us', 'Wings')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('88bcd6fd-f1d3-5dde-a73e-76cb87c23bcc', 'arms', 'hu-hu', 'Karok')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('6a147f99-7576-5dae-bd65-47cc51bc7335', 'body', 'hu-hu', 'Test')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('300e7872-dc00-5277-b17b-33c77334ec31', 'head', 'hu-hu', 'Fej')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('80cb25bf-35d6-5726-884d-5012089e8e24', 'legs', 'hu-hu', 'Lábak')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('1469b33d-d555-5836-a7cf-7e590b9e7561', 'tail', 'hu-hu', 'Farok')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('a2c1e64c-0681-51cd-adf7-38d7bc418f2b', 'arms', 'ro-ro', 'Arms')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('87d1d6d6-885b-552c-9134-a7b3e617e725', 'body', 'ro-ro', 'Body')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('df3deb66-b952-5ae1-a6ce-02c162633191', 'head', 'ro-ro', 'Head')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('9687305d-ab33-5e17-979e-09c481e6cdf0', 'horn', 'ro-ro', 'Horn')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('54c0b975-3498-5471-94b0-10d0e6521cbd', 'horns', 'ro-ro', 'Horns')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('069d9165-9e43-56a0-830f-24bc2be7ad26', 'legs', 'ro-ro', 'Legs')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('b4b926f9-6a03-52d2-8e85-735e932844db', 'tail', 'ro-ro', 'Tail')
ON CONFLICT ("BodyPartKey", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name";
INSERT INTO alchimalia_schema."BodyPartTranslations"
    ("Id", "BodyPartKey", "LanguageCode", "Name")
VALUES
    ('6235a0d7-d1b5-5aac-8727-29f5bac6be39', 'wings', 'ro-ro', 'Wings')
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
    ('2b664536-ac22-53dc-bd9a-0c38b2a306e8', '00000000-0000-0000-0000-000000000001', 'en-us', 'Bunny')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('2270be93-6826-5569-8956-7ca4d1b58327', '00000000-0000-0000-0000-000000000002', 'en-us', 'Hippo')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('94db2f44-9672-552e-bd40-e5d9d30a5df1', '00000000-0000-0000-0000-000000000003', 'en-us', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('6d1da6a9-e1d5-5ee6-b603-07f70a401a78', '00000000-0000-0000-0000-000000000004', 'en-us', 'Dog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('31606018-f93c-5c48-b78a-c854c8f2165a', '00000000-0000-0000-0000-000000000005', 'en-us', 'Fox')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('f5565cdb-f363-5768-be90-e3a8366fdefb', '00000000-0000-0000-0000-000000000006', 'en-us', 'Cat')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('6d7cefc4-a150-57b3-a685-89f12ef10cc2', '00000000-0000-0000-0000-000000000007', 'en-us', 'Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('4b643445-6f15-5fcb-bc99-77675b5d5020', '00000000-0000-0000-0000-000000000008', 'en-us', 'Camel')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('8c655700-4c81-5786-bf0f-b77525889dde', '00000000-0000-0000-0000-000000000009', 'en-us', 'Deer')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('972ab141-0e7d-5766-9a2a-c7b83c097dfe', '00000000-0000-0000-0000-00000000000a', 'en-us', 'Duck')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('353825d8-adb8-57da-8573-8622f0cdae32', '00000000-0000-0000-0000-00000000000b', 'en-us', 'Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('94dc10e5-d30f-55c8-a8d8-8515b2eab0c3', '00000000-0000-0000-0000-00000000000c', 'en-us', 'Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('c2fc3673-fd7b-5e51-a323-a48def77f9a6', '00000000-0000-0000-0000-00000000000d', 'en-us', 'Ostrich')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('2f94f559-1847-5a9f-9228-78c31526baeb', '00000000-0000-0000-0000-00000000000e', 'en-us', 'Parrot')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('5815c432-b35a-50b2-8b05-ffac28ba8241', '00000000-0000-0000-0000-00000000000f', 'en-us', 'Jaguar')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('54a16dcc-04db-5d8a-bb6d-f20344610eaf', '00000000-0000-0000-0000-000000000010', 'en-us', 'Toucan')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('317144e5-a3f9-5ae7-8bcc-03e5ac1eeeba', '00000000-0000-0000-0000-000000000011', 'en-us', 'Anaconda')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('596d096a-4dba-525d-ba58-02d63621ec38', '00000000-0000-0000-0000-000000000012', 'en-us', 'Capuchin Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('8424883e-c570-5154-bea7-aaa6620ce616', '00000000-0000-0000-0000-000000000013', 'en-us', 'Poison Dart Frog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('9e06031b-08a8-530e-92af-d9d7d539c7ac', '00000000-0000-0000-0000-000000000014', 'en-us', 'Lion')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('87f7fdea-5869-5458-8d8a-e21f160a5c60', '00000000-0000-0000-0000-000000000015', 'en-us', 'African Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('097b6812-8baa-5dc6-adcc-bdd57a25f9e5', '00000000-0000-0000-0000-000000000016', 'en-us', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('95a174be-8ae1-5419-9c96-6ed0bdd6ee43', '00000000-0000-0000-0000-000000000017', 'en-us', 'Zebra')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('ffe40398-3891-5dc6-b392-475c1002df9c', '00000000-0000-0000-0000-000000000018', 'en-us', 'Rhinoceros')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('7e11a357-1bf3-5059-8cbf-47557d087f24', '00000000-0000-0000-0000-000000000019', 'en-us', 'Bison')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('647ae114-ca39-5e07-b702-3fb9f12dd8c7', '00000000-0000-0000-0000-00000000001a', 'en-us', 'Saiga Antelope')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('b3c952e5-6f40-510f-a3b0-68bb59e0c16e', '00000000-0000-0000-0000-00000000001b', 'en-us', 'Gray Wolf')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('27914744-227c-5844-abda-7bcc94d63053', '00000000-0000-0000-0000-00000000001c', 'en-us', 'Przewalski''s Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('1afb8381-c55e-59e8-a959-15384d0cb33f', '00000000-0000-0000-0000-00000000001d', 'en-us', 'Steppe Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('123e71ac-1dbb-5b0b-837a-c02d6c6e3422', '00000000-0000-0000-0000-00000000001e', 'en-us', 'Cow')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('7de47e4c-de41-5449-9d88-4febca089fa7', '00000000-0000-0000-0000-00000000001f', 'en-us', 'Sheep')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('7fb8135a-7ffd-5cdd-be81-d4a5549e3d4c', '00000000-0000-0000-0000-000000000020', 'en-us', 'Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('adfd861a-13be-5755-988f-2a2f78c97e38', '00000000-0000-0000-0000-000000000021', 'en-us', 'Chicken')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('ddc5383c-878d-55e9-a4c2-864187bb1179', '00000000-0000-0000-0000-000000000022', 'en-us', 'Pig')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('5b3c555e-8c3b-5bbf-b97d-79a7621548d6', '00000000-0000-0000-0000-000000000001', 'hu-hu', 'Nyuszi')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('935f40c6-bb78-5823-9e9f-f77fbf892943', '00000000-0000-0000-0000-000000000002', 'hu-hu', 'Víziló')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('45394247-643a-5fcd-9374-779a7077b9ec', '00000000-0000-0000-0000-000000000003', 'hu-hu', 'Zsiráf')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('6939ed5f-2c9b-5a11-8602-792ff5532b2d', '00000000-0000-0000-0000-000000000004', 'hu-hu', 'Kutya')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('299c8d62-9a16-58f8-b20f-a588b97a3cd1', '00000000-0000-0000-0000-000000000005', 'hu-hu', 'Róka')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('710a67ba-d791-5d6f-b360-069908bc9b74', '00000000-0000-0000-0000-000000000006', 'hu-hu', 'Macska')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('17ca02f5-6cf8-57ba-b474-60633490e218', '00000000-0000-0000-0000-000000000007', 'hu-hu', 'Majom')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('c00fa41f-381d-500c-b21a-9ba18703220d', '00000000-0000-0000-0000-000000000001', 'ro-ro', 'Bunny')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('a4cb20a4-092b-5b86-a597-f48a5dc9ce2e', '00000000-0000-0000-0000-000000000002', 'ro-ro', 'Hippo')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('3660beb3-c21c-5f8a-85f9-5e2a2f47de72', '00000000-0000-0000-0000-000000000003', 'ro-ro', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('d75203b6-4a43-50a3-a3a2-0ba5a9ce3e7b', '00000000-0000-0000-0000-000000000004', 'ro-ro', 'Dog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('93708ccd-6e47-5574-8740-6e171490d5b8', '00000000-0000-0000-0000-000000000005', 'ro-ro', 'Fox')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('f15ede48-6676-53c7-bdfd-5e9a4a715c2b', '00000000-0000-0000-0000-000000000006', 'ro-ro', 'Cat')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('0679b9a1-2c61-5fef-abc4-1cb08836bf51', '00000000-0000-0000-0000-000000000007', 'ro-ro', 'Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('de8d0ab8-8d25-594e-b56f-5a7b01f749a3', '00000000-0000-0000-0000-000000000008', 'ro-ro', 'Camel')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('93bea7e2-d23c-5c2b-b7a2-8de1d00e25aa', '00000000-0000-0000-0000-000000000009', 'ro-ro', 'Deer')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('87852245-b2f1-549a-83c4-a051e055ca8b', '00000000-0000-0000-0000-00000000000a', 'ro-ro', 'Duck')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('f8942454-9d07-57a8-a0e3-827fc5557add', '00000000-0000-0000-0000-00000000000b', 'ro-ro', 'Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('3207d833-c0c8-5641-90a9-2858305e8167', '00000000-0000-0000-0000-00000000000c', 'ro-ro', 'Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('b8c6e8db-b18d-5168-824e-7f75fb25c022', '00000000-0000-0000-0000-00000000000d', 'ro-ro', 'Ostrich')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('7d53135c-2403-5eb6-8d8a-61c9e7268c00', '00000000-0000-0000-0000-00000000000e', 'ro-ro', 'Parrot')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('1fd51c72-5c14-5463-96e2-4156801605b6', '00000000-0000-0000-0000-00000000000f', 'ro-ro', 'Jaguar')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('23330dd9-4f71-5be1-a7f9-730d03c225ef', '00000000-0000-0000-0000-000000000010', 'ro-ro', 'Toucan')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('f01fd99c-a653-5981-8765-807305d38ef0', '00000000-0000-0000-0000-000000000011', 'ro-ro', 'Anaconda')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('e53d3962-2ee9-53aa-85a5-57a97becfdfb', '00000000-0000-0000-0000-000000000012', 'ro-ro', 'Capuchin Monkey')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('1b63ae8b-2149-5a4c-b505-19f045d7548a', '00000000-0000-0000-0000-000000000013', 'ro-ro', 'Poison Dart Frog')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('6fed117d-9097-59a4-84a2-7d678cfc759e', '00000000-0000-0000-0000-000000000014', 'ro-ro', 'Lion')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('268372d8-e222-5eee-b2d3-1ff4907bcafa', '00000000-0000-0000-0000-000000000015', 'ro-ro', 'African Elephant')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('b81c84a8-04f2-5636-8533-fb1c41c89e3c', '00000000-0000-0000-0000-000000000016', 'ro-ro', 'Giraffe')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('3a317a49-f988-5118-b3da-34b56ce08cbf', '00000000-0000-0000-0000-000000000017', 'ro-ro', 'Zebra')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('65518c28-c65a-5db8-9f80-b798ff7c3bb6', '00000000-0000-0000-0000-000000000018', 'ro-ro', 'Rhinoceros')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('b7cf31a4-8e6b-53a4-a9ec-31b4d3237921', '00000000-0000-0000-0000-000000000019', 'ro-ro', 'Bison')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('43c766f7-5e1c-50f2-97f1-16c8244e4a91', '00000000-0000-0000-0000-00000000001a', 'ro-ro', 'Saiga Antelope')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('ea1da95d-6cdd-52a1-b17c-b0f2afb147d9', '00000000-0000-0000-0000-00000000001b', 'ro-ro', 'Gray Wolf')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('ea50c5bd-3d51-5f6f-8cfe-9f76d64e5212', '00000000-0000-0000-0000-00000000001c', 'ro-ro', 'Przewalski''s Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('526b1da0-b6d5-5a46-b2cf-1adeb29af29e', '00000000-0000-0000-0000-00000000001d', 'ro-ro', 'Steppe Eagle')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('08d653f3-1d38-5f7e-8ba4-ef035f6aa558', '00000000-0000-0000-0000-00000000001e', 'ro-ro', 'Cow')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('9b8a1f35-5c93-51b4-ba98-7e7992e87913', '00000000-0000-0000-0000-00000000001f', 'ro-ro', 'Sheep')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('8e9e8b4d-abd3-5a4c-8cf9-22a94c40a88f', '00000000-0000-0000-0000-000000000020', 'ro-ro', 'Horse')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('6a0d324d-944b-52ba-8390-856466bbf903', '00000000-0000-0000-0000-000000000021', 'ro-ro', 'Chicken')
ON CONFLICT ("AnimalId", "LanguageCode") DO UPDATE
SET "Label" = EXCLUDED."Label";
INSERT INTO alchimalia_schema."AnimalTranslations"
    ("Id", "AnimalId", "LanguageCode", "Label")
VALUES
    ('79ddd38b-864d-52df-a13c-38187febebe5', '00000000-0000-0000-0000-000000000022', 'ro-ro', 'Pig')
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
