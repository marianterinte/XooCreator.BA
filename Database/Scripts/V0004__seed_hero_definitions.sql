-- Auto-generated from Data/SeedData/SharedConfigs/hero-tree.json + BookOfHeroes/i18n/*/hero-tree.json
-- Run date: 2025-11-27 11:59:54+02:00

BEGIN;

-- Hero Definitions
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('seed', 'seed', 0, 0, 0, 0, 0,
     '[]', '[]', FALSE, 0, 0, 'images/toh/heroes/alchimalia_hero.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5bdebba5-48fe-5870-9762-bc0e08f5929c', 'seed', 'en-us', 'Heroic Seed', 'The seed from which all heroes grow. Click to discover your heroic potential.', 'In your heart beats a seed of heroism. Click on it to discover your first heroes and begin your heroic journey.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9b055714-fcd9-572a-ae9d-92bbb7c31cf0', 'seed', 'hu-hu', 'Hősies Mag', 'A mag, amelyből minden hős nő. Kattints, hogy felfedezd hősies potenciálodat.', 'A szívedben egy hősiesség magja dobog. Kattints rá, hogy felfedezd első hőseidet és kezdd el hősies utadat.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('88d05806-b3c0-53a2-8549-adbaebb58652', 'seed', 'ro-ro', 'Sămânța Eroică', 'Sămânța din care cresc toți eroii. Apasă pentru a descoperi potențialul tău eroic.', 'În inima ta bate o sămânță de eroism. Apasă pe ea pentru a descoperi primii tăi eroi și a începe călătoria ta eroică.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('alchimalia_hero', 'hero', 0, 0, 0, 0, 0,
     '[]', '[]', FALSE, 0, 0, 'images/toh/heroes/alchimalia_hero.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('44afc746-b008-529b-9dd2-1469f1d74261', 'alchimalia_hero', 'en-us', 'Alchimalia Hero', 'The root of all heroes, the source of transformation.', 'Your faithful companion, the beginning of the heroic journey.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('57fa10c3-0e0c-528a-97f4-a13b93012620', 'alchimalia_hero', 'hu-hu', 'Alchimalia Hőse', 'Minden hős gyökere, az átalakulás forrása.', 'Hűséges társad, a hősies út kezdete')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('df9e6e4e-cd85-563b-8a5a-9672e710a238', 'alchimalia_hero', 'ro-ro', 'Eroul Alchimalia', 'Rădăcina tuturor eroilor, sursa transformării.', 'Companionul tău fidel, începutul călătoriei eroice')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_puppy', 'hero', 1, 0, 0, 0, 0,
     '["alchimalia_hero"]', '[]', FALSE, -150, -200, 'images/toh/heroes/hero_brave_puppy.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('40354d20-fc35-5639-bc04-5b66fa991459', 'hero_brave_puppy', 'en-us', 'Brave Puppy', 'A small pup full of courage and determination.', 'This little pup faces challenges head-on, inspiring bravery in everyone.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c16ffb1b-4ef2-5544-a26d-27f693f482fd', 'hero_brave_puppy', 'hu-hu', 'Bátor Kiskutya', 'Egy bátor lélek, aki megadja neked az erőt, hogy szembenézz bármilyen kihívással...', 'A Bátor Kiskutya megtanít, hogy az igazi bátorság nem a félelem hiánya, hanem a cselekvés a félelem ellenére.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4f058f40-73b1-5973-95b3-b21f8974c8e1', 'hero_brave_puppy', 'ro-ro', 'Cățelul Curajos', 'Un spirit curajos care îți va da puterea să înfrunți orice provocare...', 'Cățelul Curajos te învață că adevăratul curaj nu înseamnă absența fricii, ci acțiunea în ciuda ei.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_pup_lvl2', 'hero', 2, 0, 0, 0, 0,
     '["hero_brave_puppy"]', '[]', FALSE, -150, -280, 'images/toh/heroes/hero_guardian_dog.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('e3658fbe-3e54-5024-9d96-39d693c519b4', 'hero_brave_pup_lvl2', 'en-us', 'Guardian Dog', 'A loyal protector with unwavering courage and devotion.', 'The Guardian Dog stands watch, defending those in need with brave determination.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('b5776e59-90a4-53b9-a6ad-e17da9f476c2', 'hero_brave_pup_lvl2', 'hu-hu', 'Őrző Kutya', 'Egy hűséges védelmező rendíthetetlen bátorsággal és odaadással.', 'Az Őrző Kutya őrködik, megvédve a szükségben lévőket bátor elszántsággal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('0eb6892d-4fec-50f3-bdd7-1b9331bf2102', 'hero_brave_pup_lvl2', 'ro-ro', 'Câinele Gardian', 'Un protector loial cu curaj neclintit și devotament de neclintit.', 'Câinele Gardian stă de veghe, apărându-i pe cei în nevoie cu hotărâre curajoasă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_pup_lvl3', 'hero', 3, 0, 0, 0, 0,
     '["hero_brave_pup_lvl2"]', '[]', FALSE, -150, -360, 'images/toh/heroes/hero_legendary_dog.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a892d30a-0958-5e91-94c9-b88ca8e38dce', 'hero_brave_pup_lvl3', 'en-us', 'Legendary Dog', 'A legendary hero of unmatched courage and noble spirit.', 'The Legendary Dog inspires legends, embodying the purest form of heroic courage.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('da1d7f1a-d7fc-539b-b59c-f33c9f253987', 'hero_brave_pup_lvl3', 'hu-hu', 'Legendás Kutya', 'Egy legendás hős páratlan bátorsággal és nemes szellemmel.', 'A Legendás Kutya legendákat inspirál, a hősies bátorság legtisztább formáját megtestesítve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('8ba99a1a-122d-5b5d-990e-eb4103d952ad', 'hero_brave_pup_lvl3', 'ro-ro', 'Câinele Legendar', 'Un erou legendar de curaj fără egal și spirit nobil.', 'Câinele Legendar inspiră legende, întruchipând forma cea mai pură a curajului eroic.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_cat', 'hero', 0, 1, 0, 0, 0,
     '["alchimalia_hero"]', '[]', FALSE, 150, -200, 'images/toh/heroes/hero_curious_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a6169293-6c3b-5881-88cc-84782d4be14e', 'hero_curious_cat', 'en-us', 'Curious Cat', 'A curious being that explores every corner of the world.', 'The Curious Cat guides you through mysteries and wonders, awakening your curiosity.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('06e69f63-c81a-5a98-a664-fa3cf06b208f', 'hero_curious_cat', 'hu-hu', 'Kíváncsi Macska', 'Egy kíváncsi lény, aki megnyitja a szemed a világ csodái felé...', 'A Kíváncsi Macska vezet téged a kérdések labirintusán, megmutatva, hogy minden rejtély egy csodát rejt.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ccbb2a73-313e-5d8a-909c-46f8391ae1a0', 'hero_curious_cat', 'ro-ro', 'Pisica Curioasă', 'O ființă curioasă care îți va deschide ochii către minunile lumii...', 'Pisica Curioasă te ghidează prin labirintul întrebărilor, arătându-ți că fiecare mister ascunde o minune.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_cat_lvl2', 'hero', 0, 2, 0, 0, 0,
     '["hero_curious_cat"]', '[]', FALSE, 150, -280, 'images/toh/heroes/hero_explorer_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('0bb4ee56-b9d0-5baf-af9a-45df32a574b1', 'hero_curious_cat_lvl2', 'en-us', 'Explorer Cat', 'A fearless explorer driven by endless curiosity and wonder.', 'The Explorer Cat ventures into unknown territories, discovering hidden secrets and mysteries.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ae0609b8-fb76-5089-a3a6-f00bfaa17c89', 'hero_curious_cat_lvl2', 'hu-hu', 'Felfedező Macska', 'Egy rettenthetetlen felfedező, akit végtelen kíváncsiság és csodálat vezet.', 'A Felfedező Macska ismeretlen területekre merészkedik, rejtett titkokat és rejtélyeket fedezve fel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('0369945d-21de-5fe7-b512-6c8b807ef578', 'hero_curious_cat_lvl2', 'ro-ro', 'Pisica Exploratoare', 'O exploratoare neînfricată condusă de curiozitate și minunărie nesfârșite.', 'Pisica Exploratoare se aventurează în teritorii necunoscute, descoperind secrete ascunse și mistere.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_cat_lvl3', 'hero', 0, 3, 0, 0, 0,
     '["hero_curious_cat_lvl2"]', '[]', FALSE, 150, -360, 'images/toh/heroes/hero_oracle_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('827f1eb6-fc6d-5dfe-83f8-8f6e6fce2538', 'hero_curious_cat_lvl3', 'en-us', 'Oracle Cat', 'A mystical seer with profound insight and prophetic wisdom.', 'The Oracle Cat sees beyond the veil of reality, revealing truths hidden from ordinary sight.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a17d49ae-7795-5618-ad32-f7eb6fed7d28', 'hero_curious_cat_lvl3', 'hu-hu', 'Jósló Macska', 'Egy misztikus látnok mély megértéssel és prófétai bölcsességgel.', 'A Jósló Macska a valóság fátyla mögé lát, igazságokat tárva fel, amelyek a hétköznapi szemnek rejtve maradnak.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('cd8be94c-2638-5aac-9db9-aeb1fbaf36b2', 'hero_curious_cat_lvl3', 'ro-ro', 'Pisica Oracol', 'O văzătoare mistică cu înțelegere profundă și înțelepciune profetică.', 'Pisica Oracol vede dincolo de vălul realității, dezvăluind adevăruri ascunse de ochiul obișnuit.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_owl', 'hero', 0, 0, 1, 0, 0,
     '["alchimalia_hero"]', '[]', FALSE, 0, -220, 'images/toh/heroes/hero_wise_owl.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('6e56a01a-6b13-57c7-8cea-1dc70e6a723e', 'hero_wise_owl', 'en-us', 'Wise Owl', 'A keen observer, full of knowledge and insight.', 'The Wise Owl watches over the world, sharing wisdom and guidance to seekers.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4865fde5-2c60-564e-aa4b-d0423402d5cd', 'hero_wise_owl', 'hu-hu', 'Bölcs Bagoly', 'Egy bölcs mentor, aki megosztja veled a tudás titkait...', 'A Bölcs Bagoly suttogja a éjszaka titkait és megnyitja a szemed a rejtett igazságok felé.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('fc828168-235f-5e71-8e07-a253bdb8f630', 'hero_wise_owl', 'ro-ro', 'Bufnița Înțeleaptă', 'Un mentor înțelept care îți va împărtăși secretele cunoașterii...', 'Bufnița Înțeleaptă îți șoptește secretele nopții și îți deschide ochii către adevărurile ascunse.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_owl_lvl2', 'hero', 0, 0, 2, 0, 0,
     '["hero_wise_owl"]', '[]', FALSE, 0, -300, 'images/toh/heroes/hero_scholar_owl.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('cfc85df6-3ea0-5965-a9e0-d2ccb7af00e2', 'hero_wise_owl_lvl2', 'en-us', 'Scholar Owl', 'A learned scholar with deep knowledge and analytical thinking.', 'The Scholar Owl studies the mysteries of the world, sharing wisdom through careful analysis.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('142970eb-3d34-50de-bd51-509582b6f27e', 'hero_wise_owl_lvl2', 'hu-hu', 'Tudós Bagoly', 'Egy tanult tudós mély tudással és analitikus gondolkodással.', 'A Tudós Bagoly a világ rejtélyeit tanulmányozza, bölcsességet osztva meg gondos elemzésen keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('3c20f68b-b1a6-5e65-a902-feda1927f465', 'hero_wise_owl_lvl2', 'ro-ro', 'Bufnița Savantă', 'O savantă învățată cu cunoaștere profundă și gândire analitică.', 'Bufnița Savantă studiază misterele lumii, împărtășind înțelepciunea prin analiză atentă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_owl_lvl3', 'hero', 0, 0, 3, 0, 0,
     '["hero_wise_owl_lvl2"]', '[]', FALSE, 0, -380, 'images/toh/heroes/hero_ancient_owl.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('8c931716-f8f0-5431-a2dc-fc4bf27894a8', 'hero_wise_owl_lvl3', 'en-us', 'Ancient Owl', 'An ancient sage with timeless wisdom and eternal knowledge.', 'The Ancient Owl has witnessed the ages, holding the deepest secrets of the universe.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7dc21d29-47cc-5d99-a246-2980b7c90a24', 'hero_wise_owl_lvl3', 'hu-hu', 'Ősi Bagoly', 'Egy ősi bölcs időtlen bölcsességgel és örök tudással.', 'Az Ősi Bagoly tanúja volt a koroknak, a világegyetem legmélyebb titkait őrizve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('d6ab6842-8b1e-5546-b903-155034c60b15', 'hero_wise_owl_lvl3', 'ro-ro', 'Bufnița Antică', 'O înțeleaptă antică cu înțelepciune atemporală și cunoaștere eternă.', 'Bufnița Antică a fost martoră la epoci, păstrând cele mai profunde secrete ale universului.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_playful_horse', 'hero', 0, 0, 0, 1, 0,
     '["alchimalia_hero"]', '[]', FALSE, -200, -280, 'images/toh/heroes/hero_playful_horse.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('99deac2f-0126-5cec-8a1e-0f429e1efebc', 'hero_playful_horse', 'en-us', 'Playful Horse', 'A joyful creature, bringing creativity and fun wherever it goes.', 'The Playful Horse gallops freely, inspiring imagination and playful ideas.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2805af01-06a7-51b0-bbbe-fae34c926591', 'hero_playful_horse', 'hu-hu', 'Játékos Kiscsikó', 'Egy játékos társ, aki korlátok nélkül felszabadítja a képzeletedet...', 'A Játékos Kiscsikó hozza neked a tiszta alkotás örömét, ahol a képzelet kantár nélkül vágtázik.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c38bb4c6-1750-510e-b584-5a3ed226c738', 'hero_playful_horse', 'ro-ro', 'Căluțul Jucăuș', 'Un companion jucăuș care îți va elibera imaginația fără limite...', 'Căluțul Jucăuș îți aduce bucuria creației pure, unde imaginația galopează fără frâu.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_playful_horse_lvl2', 'hero', 0, 0, 0, 2, 0,
     '["hero_playful_horse"]', '[]', FALSE, -200, -360, 'images/toh/heroes/hero_artistic_horse.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c20ca655-63cb-53b3-af1e-909871557806', 'hero_playful_horse_lvl2', 'en-us', 'Artistic Horse', 'A creative artist expressing beauty through imagination and skill.', 'The Artistic Horse creates masterpieces, bringing dreams to life through creative expression.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2580dd24-43ea-5736-8f0d-703db9e1b6ce', 'hero_playful_horse_lvl2', 'hu-hu', 'Művészi Ló', 'Egy kreatív művész, aki szépséget fejez ki képzelet és mesterség révén.', 'A Művészi Ló remekműveket alkot, álmokat életre hozva kreatív kifejezésen keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('68d86a9e-8aa6-5b10-8b2a-ce23480c0855', 'hero_playful_horse_lvl2', 'ro-ro', 'Calul Artistic', 'Un artist creativ care exprimă frumusețea prin imaginație și măiestrie.', 'Calul Artistic creează capodopere, aducând visele la viață prin expresie creativă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_playful_horse_lvl3', 'hero', 0, 0, 0, 3, 0,
     '["hero_playful_horse_lvl2"]', '[]', FALSE, -200, -440, 'images/toh/heroes/hero_genius_horse.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('6692ca23-c1eb-5134-af00-ea829b4cd38c', 'hero_playful_horse_lvl3', 'en-us', 'Genius Horse', 'A creative genius with unparalleled artistic vision and innovation.', 'The Genius Horse transcends ordinary creativity, crafting works of pure artistic brilliance.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ef919be2-c331-580e-96c4-9bbb5984ae10', 'hero_playful_horse_lvl3', 'hu-hu', 'Zseni Ló', 'Egy kreatív zseni páratlan művészi látomással és innovációval.', 'A Zseni Ló túlmutat a hétköznapi kreativitáson, tiszta művészi ragyogás műveket alkotva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c5ba222d-2f5e-53f0-8b8d-f23d1d6af0e9', 'hero_playful_horse_lvl3', 'ro-ro', 'Calul Geniu', 'Un geniu creativ cu viziune artistică și inovație fără egal.', 'Calul Geniu transcende creativitatea obișnuită, creând opere de pură strălucire artistică.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_cautious_hedgehog', 'hero', 0, 0, 0, 0, 1,
     '["alchimalia_hero"]', '[]', FALSE, 200, -280, 'images/toh/heroes/hero_cautious_hedgehog.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ae99afa3-d2a9-5704-b499-28669b7b6713', 'hero_cautious_hedgehog', 'en-us', 'Cautious Hedgehog', 'A careful protector, mindful of danger and safety.', 'The Hedgehog teaches prudence and foresight, keeping everyone safe.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('3c5cc213-9923-5f9c-b35e-757ae5124207', 'hero_cautious_hedgehog', 'hu-hu', 'Óvatos Sün', 'Egy óvatos védelmező, aki biztonságot és egyensúlyt ad neked...', 'Az Óvatos Sün megtanít, hogy a biztonság nem félelmet jelent, hanem bölcsességet.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('13ebc029-2433-5e8e-8bb4-bf6ef9244825', 'hero_cautious_hedgehog', 'ro-ro', 'Ariciul Precaut', 'Un protector precaut care îți va oferi siguranța și echilibrul...', 'Ariciul Precaut îți învață că siguranța nu înseamnă teamă, ci înțelepciune.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_cautious_hedgehog_lvl2', 'hero', 0, 0, 0, 0, 2,
     '["hero_cautious_hedgehog"]', '[]', FALSE, 200, -380, 'images/toh/heroes/hero_shieldquill.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9ae56d56-992a-5a0c-b92d-6875061c7129', 'hero_cautious_hedgehog_lvl2', 'en-us', 'Shieldquill', 'Careful protection built from prudence and experience.', 'Uses its quills as a shield to guard what matters most.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('93503004-92fe-57e9-b59a-eaf8a5bcda58', 'hero_cautious_hedgehog_lvl2', 'hu-hu', 'Pajzs-Tüskés', 'Gondos védelem, óvatosságból és tapasztalatból építve.', 'Tüskéit pajzsként használja, hogy megvédje, ami a legfontosabb.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('06ca240a-9b0e-50ea-9d49-1f34076d4fdf', 'hero_cautious_hedgehog_lvl2', 'ro-ro', 'Ariciul Scut-Spini', 'Protecție atentă, construită din prudență și experiență.', 'Își folosește spinii ca scut pentru a apăra ceea ce contează cel mai mult.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_cautious_hedgehog_lvl3', 'hero', 0, 0, 0, 0, 3,
     '["hero_cautious_hedgehog_lvl2"]', '[]', FALSE, 200, -480, 'images/toh/heroes/hero_wardenspike.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('aa473054-ad12-5287-a643-09056aa6364c', 'hero_cautious_hedgehog_lvl3', 'en-us', 'Wardenspike', 'Unyielding sentinel, determined to uphold safety.', 'Turns vigilance into an art, keeping watch at the world''s edges.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('446724a3-e204-5959-9543-4af1dc98a148', 'hero_cautious_hedgehog_lvl3', 'hu-hu', 'Őrző Tüske', 'Rendíthetetlen őrszem, elszántan a biztonság fenntartására.', 'Az őrséget művészetté alakítja, a világ határain őrködve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('bc79c486-a13f-5907-9340-98dcd880ba40', 'hero_cautious_hedgehog_lvl3', 'ro-ro', 'Paznicul Țepușă', 'Strajă de neclintit, hotărâtă să mențină siguranța.', 'Transformă vigilența în artă, stând de veghe la hotarele lumii.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_catdog_interstellar', 'hero', 1, 1, 0, 0, 0,
     '["hero_brave_puppy","hero_curious_cat"]', '[]', FALSE, 0, -350, 'images/toh/heroes/hero_catdog_interstellar.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('bfacffd3-2ec3-5564-b67c-3ac5920dee89', 'hero_catdog_interstellar', 'en-us', 'CatDog Interstellar', 'A hybrid of curiosity and courage, ready for interstellar adventures.', 'This heroic hybrid combines the best traits of cat and dog, exploring the cosmos fearlessly.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('841f6265-2965-53e6-bbce-13a6c7cd5cbd', 'hero_catdog_interstellar', 'hu-hu', 'Csillagközi Macska-Kutya', 'Egy kozmikus felfedező, aki a bátorság és kíváncsiság egyesüléséből született...', 'A Csillagközi Macska-Kutya egyesíti a végtelen kíváncsiságot a bátorsággal, hogy felfedezze az ismeretlent.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ebc81e3f-cb92-5046-8102-f35a1b04a845', 'hero_catdog_interstellar', 'ro-ro', 'CatDog Interstelar', 'Un explorator cosmic născut din uniunea curajului și curiozității...', 'CatDog Interstelar îmbină curiozitatea infinită cu curajul de a explora necunoscutul.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_sage', 'hero', 1, 0, 1, 0, 0,
     '["hero_brave_puppy","hero_wise_owl"]', '[]', FALSE, -180, -380, 'images/toh/heroes/hero_brave_sage.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5f07bec5-fe1c-5c4d-af4f-791a1dd0227a', 'hero_brave_sage', 'en-us', 'Brave Sage', 'A mix of wisdom and bravery, guiding heroes with insight.', 'The Brave Sage combines intellect and courage to lead others through challenges.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('14f1bdac-cb71-5775-8e34-fa5dbe4225c2', 'hero_brave_sage', 'hu-hu', 'Bátor Bölcs', 'Egy bölcs lélek, aki bátorságot és bölcsességet egyesít...', 'A Bátor Bölcs megmutatja, hogy az igazi erő a tudás és a bátorság harmóniájában rejlik.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('e01533c5-340e-5224-81a4-9145f905bb04', 'hero_brave_sage', 'ro-ro', 'Înțeleptul Curajos', 'Un erou ce îmbină curajul cu înțelepciunea, acționând cu o putere chibzuită.', 'Înțeleptul Curajos te învață că adevărata putere nu este doar lupta, ci a ști când și de ce să lupți.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_artist', 'hero', 1, 0, 0, 1, 0,
     '["hero_brave_puppy","hero_playful_horse"]', '[]', FALSE, -250, -420, 'images/toh/heroes/hero_brave_artist.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a2a402d3-d6e6-5a10-9b63-9de79420a8f9', 'hero_brave_artist', 'en-us', 'Brave Artist', 'A creative soul with a courageous heart.', 'The Brave Artist paints heroic tales with bravery and imagination.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('619e330e-e53a-5ce8-9984-64debc73b52a', 'hero_brave_artist', 'hu-hu', 'Bátor Művész', 'Egy hős, aki mer alkotni ott, ahol mások félnek, új horizontokat nyitva művészetével.', 'A Bátor Művész megmutatja, hogy az alkotás cselekedete a legfőbb bátorság, új világokat életre hozva a semmiből.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2f3ac07c-0c5d-5cff-b23a-497ad19c2786', 'hero_brave_artist', 'ro-ro', 'Artistul Curajos', 'Un erou ce îndrăznește să creeze acolo unde alții se tem, deschizând noi orizonturi prin arta sa.', 'Artistul Curajos arată că actul creației este curajul suprem, aducând la viață lumi noi din nimic.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_guardian', 'hero', 1, 0, 0, 0, 1,
     '["hero_brave_puppy","hero_cautious_hedgehog"]', '[]', FALSE, 0, -400, 'images/toh/heroes/hero_brave_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('16579435-c66e-58b5-83da-eac8b05f44a3', 'hero_brave_guardian', 'en-us', 'Light Wolf', 'A guardian of courage, standing firm against all threats.', 'Light Wolf protects the realm, embodying courage and vigilance.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('135f15d2-37d7-599c-816c-1c81299155c0', 'hero_brave_guardian', 'hu-hu', 'Bátor Őrző', 'Egy hős, ahol a bátorság és védelem kéz a kézben jár, védve habozás nélkül, de bölcsen.', 'A Bátor Őrző a remegtetlen pajzs, megvédve az ártatlanokat rendíthetetlen elszántsággal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2e9a94b8-1618-551e-ae16-55b2d4bedfff', 'hero_brave_guardian', 'ro-ro', 'Gardianul Curajos', 'Un erou unde curajul și protecția merg mână în mână, apărând fără ezitare, dar cu înțelepciune.', 'Gardianul Curajos este scutul ce nu tremură, protejând inocenții cu o hotărâre de neclintit.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_researcher', 'hero', 0, 1, 1, 0, 0,
     '["hero_curious_cat","hero_wise_owl"]', '[]', FALSE, 180, -380, 'images/toh/heroes/hero_wise_researcher.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('f4703d41-3e4b-501c-96b2-b415d98ee2eb', 'hero_wise_researcher', 'en-us', 'Wise Researcher', 'Curious and analytical, seeking knowledge in every corner.', 'This hero uncovers secrets and guides others with insightful discoveries.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4ad6ad68-7263-5383-9195-2cfeb92ee662', 'hero_wise_researcher', 'hu-hu', 'Bölcs Kutató', 'Egy hős, aki egyesíti az analitikus elmét a fáradhatatlan kíváncsisággal, hogy rejtett igazságokat fedezzen fel.', 'A Bölcs Kutató követi a tudás fonalát a legnagyobb labirintusokon keresztül, fényt találva a sötétségben.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('b73257a9-5e43-53e6-960e-c19104e8b77e', 'hero_wise_researcher', 'ro-ro', 'Cercetătorul Înțelept', 'Un erou ce unește mintea analitică cu curiozitatea neobosită pentru a descoperi adevăruri ascunse.', 'Cercetătorul Înțelept urmărește firul cunoașterii prin cele mai mari labirinturi, găsind lumină în întuneric.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_creative_explorer', 'hero', 0, 1, 0, 1, 0,
     '["hero_curious_cat","hero_playful_horse"]', '[]', FALSE, -80, -450, 'images/toh/heroes/hero_creative_explorer.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a1c14fa6-141e-5cac-8438-d4c5bd1b8781', 'hero_creative_explorer', 'en-us', 'Creative Explorer', 'Adventurous and inventive, always finding new paths.', 'The Creative Explorer charts unknown lands with imagination and curiosity.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9fed3e9b-03f4-5f88-a095-84155f0483ca', 'hero_creative_explorer', 'hu-hu', 'Kreatív Felfedező', 'Egy hős, aki egyesíti a képzeletet a megismerés vágyával; minden lépés egy új találmány.', 'A Kreatív Felfedező számára a térkép rajzolódik ki, ahogy bejárja, az ismeretlent művészeti alkotássá alakítva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('399f051b-7fe2-56c2-ac63-8fe4cde7a62a', 'hero_creative_explorer', 'ro-ro', 'Exploratorul Creativ', 'Un erou ce îmbină imaginația cu dorința de a cunoaște; fiecare pas e o nouă invenție.', 'Pentru Exploratorul Creativ, harta se desenează pe măsură ce este parcursă, transformând necunoscutul într-o operă de artă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_guard_cat', 'hero', 0, 1, 0, 0, 1,
     '["hero_curious_cat","hero_cautious_hedgehog"]', '[]', FALSE, 250, -420, 'images/toh/heroes/hero_guard_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('d24f5d07-9e08-546e-ab99-9a9f73581586', 'hero_guard_cat', 'en-us', 'Guard Cat', 'A protective hybrid, attentive and vigilant.', 'The Guard Cat keeps watch, blending caution and curiosity in every move.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('1c2fb32d-20d3-545a-b567-1d9f2b8087f6', 'hero_guard_cat', 'hu-hu', 'Őrző Macska', 'Egy kalandos védelmező, aki óvatosan felfedez.', 'Az Őrző Macska ismeri területének minden szögletét, biztosítva, hogy minden kíváncsiság kielégüljön a biztonság veszélyeztetése nélkül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('17b0464c-7d27-5d25-8230-aa1abeda6a39', 'hero_guard_cat', 'ro-ro', 'Pisica de Gardă', 'Un protector aventuros care explorează cu prudență.', 'Pisica de Gardă cunoaște fiecare colț al teritoriului său, asigurându-se că orice curiozitate este satisfăcută fără a compromite siguranța.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_pegasus', 'hero', 0, 0, 1, 1, 0,
     '["hero_wise_owl","hero_playful_horse"]', '[]', FALSE, 80, -450, 'images/toh/heroes/hero_wise_pegasus.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7d9f4e9d-a927-5256-9fc9-f2f4b05e2656', 'hero_wise_pegasus', 'en-us', 'Wise Pegasus', 'A majestic creature of intellect and creativity.', 'The Wise Pegasus flies through challenges, offering guidance and inspiration.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('cdec2124-822d-524d-bd0a-accc271d8329', 'hero_wise_pegasus', 'hu-hu', 'Bölcs Pegazus', 'A tudás őrzője, aki a határokon túl tud repülni.', 'A Bölcs Pegazus gondolatokat visz szárnyain, az absztrakt ötleteket repülő valóságokká alakítva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5686134d-e052-51a2-8b59-a224e54648f4', 'hero_wise_pegasus', 'ro-ro', 'Pegazul Înțelept', 'Un gardian al cunoașterii care poate zbura dincolo de limite.', 'Pegazul Înțelept poartă gândurile pe aripile sale, transformând ideile abstracte în realități zburătoare.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_thoughtful_guardian', 'hero', 0, 0, 1, 0, 1,
     '["hero_wise_owl","hero_cautious_hedgehog"]', '[]', FALSE, 150, -500, 'images/toh/heroes/hero_thoughtful_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('17c36ce9-b161-56ae-8a7a-90d9132f1bfb', 'hero_thoughtful_guardian', 'en-us', 'Thoughtful Guardian', 'A careful thinker, combining safety with wisdom.', 'This hero balances prudence and intelligence to protect the realm.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('05af3db4-4a05-511d-9959-4bb31d3ffc16', 'hero_thoughtful_guardian', 'hu-hu', 'Gondolkodó Őrző', 'Egy hős, aki cselekvés előtt tervez; a biztonság jól megfontolt stratégiákból ered.', 'A Gondolkodó Őrző nem kőből épít erődöket, hanem logikából és előrelátásból.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2646ea45-7048-5ed1-8436-ec5510e609b2', 'hero_thoughtful_guardian', 'ro-ro', 'Gardianul Gânditor', 'Un erou ce planifică înainte de a acționa; siguranța vine din strategiile bine gândite.', 'Gardianul Gânditor construiește fortărețe nu din piatră, ci din logică și prevedere.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_creative_guardian', 'hero', 0, 0, 0, 1, 1,
     '["hero_playful_horse","hero_cautious_hedgehog"]', '[]', FALSE, -150, -500, 'images/toh/heroes/hero_creative_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7686dec2-04f5-598b-a55c-e00f86641173', 'hero_creative_guardian', 'en-us', 'Creative Guardian', 'A guardian blending imagination with caution.', 'The Creative Guardian defends while inspiring inventive solutions.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('d2e9f52e-505b-51f7-b639-ee9fcb5970bf', 'hero_creative_guardian', 'hu-hu', 'Kreatív Őrző', 'Egy találékony védelmező, aki eredeti megoldásokat talál a biztonság biztosítására.', 'A Kreatív Őrző nem falakat épít, hanem egy olyan világot képzel el, ahol ezek nem szükségesek.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('fb19a7ac-dc31-5b14-bf5b-6e75e63ee0a7', 'hero_creative_guardian', 'ro-ro', 'Gardianul Creativ', 'Un protector inventiv care găsește soluții originale pentru a asigura siguranța.', 'Gardianul Creativ protejează lumea nu construind ziduri, ci imaginând o lume în care acestea nu sunt necesare.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_curious_wise', 'hero', 1, 1, 1, 0, 0,
     '["hero_brave_curious","hero_wise_owl"]', '[]', FALSE, -100, -500, 'images/toh/heroes/hero_wise_lion_sage.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('1891c898-cb01-5829-ad5a-ec2285f21251', 'hero_brave_curious_wise', 'en-us', 'Wise Lion Sage', 'A majestic lion combining courage, curiosity, and wisdom.', 'The Wise Lion Sage leads with both strength and insight, inspiring others through noble example.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('759b3051-fbdc-5566-b96f-48fec4846bc9', 'hero_brave_curious_wise', 'hu-hu', 'Bölcs Oroszlán Bölcs', 'Egy fenséges oroszlán, aki egyesíti a bátorságot, kíváncsiságot és bölcsességet.', 'A Bölcs Oroszlán Bölcs erővel és megértéssel vezet, másokat inspirálva nemes példájával.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('6d69e033-8af6-5ef0-a06e-f28c943b4c52', 'hero_brave_curious_wise', 'ro-ro', 'Leul Înțelept Savant', 'Un leu măreț care îmbină curajul, curiozitatea și înțelepciunea.', 'Leul Înțelept Savant conduce cu atât putere cât și înțelegere, inspirându-i pe alții prin exemplul nobil.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_curious_creative', 'hero', 1, 1, 0, 1, 0,
     '["hero_brave_curious","hero_playful_horse"]', '[]', FALSE, -200, -500, 'images/toh/heroes/hero_brave_lion_artist.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7c3252d6-f65c-5de3-b4ed-0fe92536616d', 'hero_brave_curious_creative', 'en-us', 'Brave Lion Artist', 'A courageous lion expressing creativity through bold artistic vision.', 'The Brave Lion Artist creates masterpieces that inspire courage and imagination in all who see them.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('213b8a6b-20a8-599d-82a4-61df82cb44ad', 'hero_brave_curious_creative', 'hu-hu', 'Bátor Oroszlán Művész', 'Egy bátor oroszlán, aki kreativitást fejez ki merész művészi látomáson keresztül.', 'A Bátor Oroszlán Művész remekműveket alkot, amelyek bátorságot és képzeletet inspirálnak mindenki számára, aki látja őket.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('37cddb67-ea65-54be-9e2b-b32940f59124', 'hero_brave_curious_creative', 'ro-ro', 'Leul Curajos Artist', 'Un leu curajos care exprimă creativitatea prin viziune artistică îndrăzneață.', 'Leul Curajos Artist creează capodopere care inspiră curaj și imaginație în toți cei care le văd.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_curious_safe', 'hero', 1, 1, 0, 0, 1,
     '["hero_brave_curious","hero_cautious_hedgehog"]', '[]', FALSE, 0, -500, 'images/toh/heroes/hero_guardian_lion.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('b74bee8f-028f-5fe6-b78d-b2d835ab7c68', 'hero_brave_curious_safe', 'en-us', 'Guardian Lion', 'A protective lion combining courage, curiosity, and safety.', 'The Guardian Lion watches over the realm, ensuring safety while encouraging exploration and discovery.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5bdb70c9-5ba3-5e01-aabd-db63514eed20', 'hero_brave_curious_safe', 'hu-hu', 'Őrző Oroszlán', 'Egy védelmező oroszlán, aki egyesíti a bátorságot, kíváncsiságot és biztonságot.', 'Az Őrző Oroszlán a birodalom felett őrködik, biztonságot biztosítva, miközben felfedezést és megismerést ösztönöz.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c9ac86f8-5337-5e48-b8d4-8beb96332b0e', 'hero_brave_curious_safe', 'ro-ro', 'Leul Gardian', 'Un leu protector care îmbină curajul, curiozitatea și siguranța.', 'Leul Gardian veghează asupra regatului, asigurând siguranța în timp ce încurajează explorarea și descoperirea.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_wise_creative', 'hero', 1, 0, 1, 1, 0,
     '["hero_brave_wise","hero_playful_horse"]', '[]', FALSE, -300, -500, 'images/toh/heroes/hero_brave_sage_artist.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('df3b2f83-c17c-58bc-9694-a0749534caa2', 'hero_brave_wise_creative', 'en-us', 'Brave Sage Artist', 'A wise artist blending courage, thinking, and creativity.', 'The Brave Sage Artist creates profound works that combine intellectual depth with artistic beauty.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4c32daf3-d28e-57c4-aca6-191eb6626580', 'hero_brave_wise_creative', 'hu-hu', 'Bátor Bölcs Művész', 'Egy bölcs művész, aki egyesíti a bátorságot, gondolkodást és kreativitást.', 'A Bátor Bölcs Művész mély műveket alkot, amelyek egyesítik az intellektuális mélységet a művészi szépséggel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5efa94e0-f834-533f-a060-a266cd492a03', 'hero_brave_wise_creative', 'ro-ro', 'Artistul Înțelept Curajos', 'Un artist înțelept care îmbină curajul, gândirea și creativitatea.', 'Artistul Înțelept Curajos creează opere profunde care combină adâncimea intelectuală cu frumusețea artistică.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_wise_safe', 'hero', 1, 0, 1, 0, 1,
     '["hero_brave_wise","hero_cautious_hedgehog"]', '[]', FALSE, -100, -550, 'images/toh/heroes/hero_brave_sage_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('0d272058-743d-5aeb-9bf2-f760ce5fc8d0', 'hero_brave_wise_safe', 'en-us', 'Brave Sage Guardian', 'A protective sage combining courage, wisdom, and safety.', 'The Brave Sage Guardian defends with both strength and wisdom, ensuring safety through thoughtful protection.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2363b36a-86a5-52e7-a6b0-70246efcda92', 'hero_brave_wise_safe', 'hu-hu', 'Bátor Bölcs Őrző', 'Egy védelmező bölcs, aki egyesíti a bátorságot, bölcsességet és biztonságot.', 'A Bátor Bölcs Őrző erővel és bölcsességgel véd, biztonságot biztosítva gondos védelemmel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9da7c402-078c-5278-a150-1c0bc8865e9c', 'hero_brave_wise_safe', 'ro-ro', 'Gardianul Înțelept Curajos', 'Un gardian protector care îmbină curajul, înțelepciunea și siguranța.', 'Gardianul Înțelept Curajos se apără cu atât putere cât și înțelepciune, asigurând siguranța prin protecție gândită.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_creative_safe', 'hero', 1, 0, 0, 1, 1,
     '["hero_brave_creative","hero_cautious_hedgehog"]', '[]', FALSE, -200, -550, 'images/toh/heroes/hero_brave_artist_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('e9e50df2-45e5-5a5b-be1b-7029dd0feb8f', 'hero_brave_creative_safe', 'en-us', 'Brave Artist Guardian', 'A creative guardian blending courage, creativity, and safety.', 'The Brave Artist Guardian protects through creative solutions, combining artistic vision with vigilant care.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('70daae84-4204-546e-addb-6353ad6db2da', 'hero_brave_creative_safe', 'hu-hu', 'Bátor Művész Őrző', 'Egy kreatív őrző, aki egyesíti a bátorságot, kreativitást és biztonságot.', 'A Bátor Művész Őrző kreatív megoldásokkal véd, művészi látomást kombinálva figyelmes gondossággal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('1d7309be-2081-5e28-aeaf-406780d1acbe', 'hero_brave_creative_safe', 'ro-ro', 'Gardianul Artist Curajos', 'Un gardian creativ care îmbină curajul, creativitatea și siguranța.', 'Gardianul Artist Curajos protejează prin soluții creative, combinând viziunea artistică cu grija vigilentă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_wise_creative', 'hero', 0, 1, 1, 1, 0,
     '["hero_curious_wise","hero_playful_horse"]', '[]', FALSE, 100, -500, 'images/toh/heroes/hero_wise_explorer_artist.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('80f00e35-79c4-5094-87ef-a55a02e42b44', 'hero_curious_wise_creative', 'en-us', 'Wise Explorer Artist', 'An intellectual artist combining curiosity, thinking, and creativity.', 'The Wise Explorer Artist discovers new artistic frontiers through thoughtful exploration and creative expression.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('cd86f6d3-0b2b-5ff9-9794-1b78402bbeca', 'hero_curious_wise_creative', 'hu-hu', 'Bölcs Felfedező Művész', 'Egy intellektuális művész, aki egyesíti a kíváncsiságot, gondolkodást és kreativitást.', 'A Bölcs Felfedező Művész új művészi határokat fedez fel gondos felfedezésen és kreatív kifejezésen keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('0febcaea-8b33-5363-8935-82db63279507', 'hero_curious_wise_creative', 'ro-ro', 'Artistul Explorator Înțelept', 'Un artist intelectual care îmbină curiozitatea, gândirea și creativitatea.', 'Artistul Explorator Înțelept descoperă noi frontiere artistice prin explorare gândită și expresie creativă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_wise_safe', 'hero', 0, 1, 1, 0, 1,
     '["hero_curious_wise","hero_cautious_hedgehog"]', '[]', FALSE, 0, -550, 'images/toh/heroes/hero_wise_guardian_explorer.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('db11c99a-16bd-52b1-a57e-c7ffdec6d773', 'hero_curious_wise_safe', 'en-us', 'Wise Guardian Explorer', 'A protective explorer combining curiosity, wisdom, and safety.', 'The Wise Guardian Explorer ventures safely into unknown territories, guided by wisdom and protected by caution.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('40683f84-deb5-5e55-9b46-69a54c420b50', 'hero_curious_wise_safe', 'hu-hu', 'Bölcs Őrző Felfedező', 'Egy védelmező felfedező, aki egyesíti a kíváncsiságot, bölcsességet és biztonságot.', 'A Bölcs Őrző Felfedező biztonságosan merészkedik ismeretlen területekre, bölcsesség vezetésével és óvatosság védelmével.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9416e2fd-2d30-5729-95ba-5513046bea61', 'hero_curious_wise_safe', 'ro-ro', 'Exploratorul Gardian Înțelept', 'Un explorator protector care îmbină curiozitatea, înțelepciunea și siguranța.', 'Exploratorul Gardian Înțelept se aventurează în siguranță în teritorii necunoscute, ghidat de înțelepciune și protejat de precauție.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_creative_safe', 'hero', 0, 1, 0, 1, 1,
     '["hero_curious_creative","hero_cautious_hedgehog"]', '[]', FALSE, 200, -500, 'images/toh/heroes/hero_creative_guardian_explorer.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('3f55d1e4-c92a-58ad-86ba-9ecfe378020f', 'hero_curious_creative_safe', 'en-us', 'Creative Guardian Explorer', 'An inventive explorer combining curiosity, creativity, and safety.', 'The Creative Guardian Explorer discovers new worlds through imaginative solutions while ensuring safe passage.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('8527c98a-16a6-54b2-b86f-01245588c1be', 'hero_curious_creative_safe', 'hu-hu', 'Kreatív Őrző Felfedező', 'Egy találékony felfedező, aki egyesíti a kíváncsiságot, kreativitást és biztonságot.', 'A Kreatív Őrző Felfedező új világokat fedez fel képzeletbeli megoldásokon keresztül, miközben biztonságos átkelést biztosít.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('6e2552c5-497f-5bc3-901c-04362dc0e008', 'hero_curious_creative_safe', 'ro-ro', 'Exploratorul Gardian Creativ', 'Un explorator inventiv care îmbină curiozitatea, creativitatea și siguranța.', 'Exploratorul Gardian Creativ descoperă lumi noi prin soluții imaginare în timp ce asigură trecerea în siguranță.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_creative_safe', 'hero', 0, 0, 1, 1, 1,
     '["hero_wise_creative","hero_cautious_hedgehog"]', '[]', FALSE, 100, -550, 'images/toh/heroes/hero_wise_creative_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('05c00517-58ff-5e4d-b233-49b6f1a7290e', 'hero_wise_creative_safe', 'en-us', 'Wise Creative Guardian', 'A thoughtful guardian combining wisdom, creativity, and safety.', 'The Wise Creative Guardian protects through innovative solutions, blending intellectual insight with artistic vision.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('defe07f0-5b32-5c47-87e3-a60d6db2fdde', 'hero_wise_creative_safe', 'hu-hu', 'Bölcs Kreatív Őrző', 'Egy gondolkodó őrző, aki egyesíti a bölcsességet, kreativitást és biztonságot.', 'A Bölcs Kreatív Őrző innovatív megoldásokkal véd, intellektuális megértést kombinálva művészi látomással.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4e7dd8ac-04d3-5527-b9ba-493cdb72dc65', 'hero_wise_creative_safe', 'ro-ro', 'Gardianul Creativ Înțelept', 'Un gardian gânditor care îmbină înțelepciunea, creativitatea și siguranța.', 'Gardianul Creativ Înțelept protejează prin soluții inovatoare, îmbinând înțelegerea intelectuală cu viziunea artistică.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_curious_wise_creative', 'hero', 1, 1, 1, 1, 0,
     '["hero_brave_curious_wise","hero_playful_horse"]', '[]', FALSE, -150, -600, 'images/toh/heroes/hero_master_lion_artist.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('300e56fa-0018-52c5-abc2-a2491d3d851e', 'hero_brave_curious_wise_creative', 'en-us', 'Master Lion Artist', 'The ultimate artistic lion, master of courage, curiosity, wisdom, and creativity.', 'The Master Lion Artist creates legendary works that inspire all who witness their artistic genius and noble spirit.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4841b149-bd26-595d-ab10-26444df994fa', 'hero_brave_curious_wise_creative', 'hu-hu', 'Mester Oroszlán Művész', 'A végső művészi oroszlán, a bátorság, kíváncsiság, bölcsesség és kreativitás mestere.', 'A Mester Oroszlán Művész legendás műveket alkot, amelyek mindenkit inspirálnak, aki tanúja művészi zsenialitásának és nemes szellemének.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('13bdcd4b-dd28-578e-8827-45c7d60e2008', 'hero_brave_curious_wise_creative', 'ro-ro', 'Maestrul Leul Artist', 'Leul artistic suprem, maestru al curajului, curiozității, înțelepciunii și creativității.', 'Maestrul Leul Artist creează opere legendare care inspiră toți cei care asistă la geniul său artistic și spiritul nobil.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_curious_wise_safe', 'hero', 1, 1, 1, 0, 1,
     '["hero_brave_curious_wise","hero_cautious_hedgehog"]', '[]', FALSE, -50, -600, 'images/toh/heroes/hero_master_guardian_lion.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a8b838d7-81a4-520c-8f9c-a2f2dbd2b855', 'hero_brave_curious_wise_safe', 'en-us', 'Master Guardian Lion', 'The ultimate protective lion, master of courage, curiosity, wisdom, and safety.', 'The Master Guardian Lion stands as the ultimate protector, ensuring safety while encouraging exploration and growth.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5ab42d21-0ae6-5968-9202-beadc6cefdf1', 'hero_brave_curious_wise_safe', 'hu-hu', 'Mester Őrző Oroszlán', 'A végső védelmező oroszlán, a bátorság, kíváncsiság, bölcsesség és biztonság mestere.', 'A Mester Őrző Oroszlán a végső védelmezőként áll, biztonságot biztosítva, miközben felfedezést és növekedést ösztönöz.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4b699322-72cb-5894-9926-7202aca7a78d', 'hero_brave_curious_wise_safe', 'ro-ro', 'Maestrul Leul Gardian', 'Leul protector suprem, maestru al curajului, curiozității, înțelepciunii și siguranței.', 'Maestrul Leul Gardian stă ca protectorul suprem, asigurând siguranța în timp ce încurajează explorarea și creșterea.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_curious_creative_safe', 'hero', 1, 1, 0, 1, 1,
     '["hero_brave_curious_creative","hero_cautious_hedgehog"]', '[]', FALSE, -100, -600, 'images/toh/heroes/hero_master_explorer_lion.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5213d756-c257-52b4-b58a-3f8197e9612d', 'hero_brave_curious_creative_safe', 'en-us', 'Master Explorer Lion', 'The ultimate exploring lion, master of courage, curiosity, creativity, and safety.', 'The Master Explorer Lion ventures into the most challenging territories, discovering wonders while ensuring safe passage.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a2d2df18-2202-52da-bd78-3929e3577555', 'hero_brave_curious_creative_safe', 'hu-hu', 'Mester Felfedező Oroszlán', 'A végső felfedező oroszlán, a bátorság, kíváncsiság, kreativitás és biztonság mestere.', 'A Mester Felfedező Oroszlán a legkihívóbb területekre merészkedik, csodákat fedezve fel, miközben biztonságos átkelést biztosít.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('41eff715-6a0c-5cea-9596-f788f47ad36b', 'hero_brave_curious_creative_safe', 'ro-ro', 'Maestrul Leul Explorator', 'Leul explorator suprem, maestru al curajului, curiozității, creativității și siguranței.', 'Maestrul Leul Explorator se aventurează în cele mai provocatoare teritorii, descoperind minunății în timp ce asigură trecerea în siguranță.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_wise_creative_safe', 'hero', 1, 0, 1, 1, 1,
     '["hero_brave_wise_creative","hero_cautious_hedgehog"]', '[]', FALSE, -200, -600, 'images/toh/heroes/hero_master_sage_guardian.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('d856b1cd-8d7a-5963-a853-1334822f0d2c', 'hero_brave_wise_creative_safe', 'en-us', 'Master Sage Guardian', 'The ultimate wise guardian, master of courage, thinking, creativity, and safety.', 'The Master Sage Guardian combines profound wisdom with creative protection, ensuring safety through enlightened guidance.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7b48b0bc-9556-5f08-9a14-89b88e238b8f', 'hero_brave_wise_creative_safe', 'hu-hu', 'Mester Bölcs Őrző', 'A végső bölcs őrző, a bátorság, gondolkodás, kreativitás és biztonság mestere.', 'A Mester Bölcs Őrző mély bölcsességet kombinál kreatív védelemmel, biztonságot biztosítva megvilágosodott vezetéssel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a7784d4e-c936-58d5-a6a0-98a9f7acf5f6', 'hero_brave_wise_creative_safe', 'ro-ro', 'Maestrul Gardian Înțelept', 'Gardianul înțelept suprem, maestru al curajului, gândirii, creativității și siguranței.', 'Maestrul Gardian Înțelept combină înțelepciunea profundă cu protecția creativă, asigurând siguranța prin ghidare iluminată.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_wise_creative_safe', 'hero', 0, 1, 1, 1, 1,
     '["hero_curious_wise_creative","hero_cautious_hedgehog"]', '[]', FALSE, 50, -600, 'images/toh/heroes/hero_master_creative_explorer.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('6125b658-9072-5231-9aaf-77f727d95460', 'hero_curious_wise_creative_safe', 'en-us', 'Master Creative Explorer', 'The ultimate creative explorer, master of curiosity, thinking, creativity, and safety.', 'The Master Creative Explorer discovers new realms of possibility through innovative thinking and artistic vision.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9f63395d-0288-517c-8f77-68f84442b2ef', 'hero_curious_wise_creative_safe', 'hu-hu', 'Mester Kreatív Felfedező', 'A végső kreatív felfedező, a kíváncsiság, gondolkodás, kreativitás és biztonság mestere.', 'A Mester Kreatív Felfedező új lehetőségek birodalmait fedez fel innovatív gondolkodáson és művészi látomáson keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ea57b41f-e089-5cc3-85bf-075f64abc644', 'hero_curious_wise_creative_safe', 'ro-ro', 'Maestrul Explorator Creativ', 'Exploratorul creativ suprem, maestru al curiozității, gândirii, creativității și siguranței.', 'Maestrul Explorator Creativ descoperă noi regnuri ale posibilității prin gândire inovatoare și viziune artistică.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_alchimalian_dragon', 'hero', 5, 5, 5, 5, 5,
     '["hero_brave_puppy","hero_curious_cat","hero_wise_owl","hero_playful_horse","hero_cautious_hedgehog"]', '[]', FALSE, 0, -600, 'images/toh/heroes/hero_alchimalian_dragon.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "CourageCost" = EXCLUDED."CourageCost",
    "CuriosityCost" = EXCLUDED."CuriosityCost",
    "ThinkingCost" = EXCLUDED."ThinkingCost",
    "CreativityCost" = EXCLUDED."CreativityCost",
    "SafetyCost" = EXCLUDED."SafetyCost",
    "PrerequisitesJson" = EXCLUDED."PrerequisitesJson",
    "RewardsJson" = EXCLUDED."RewardsJson",
    "IsUnlocked" = EXCLUDED."IsUnlocked",
    "PositionX" = EXCLUDED."PositionX",
    "PositionY" = EXCLUDED."PositionY",
    "Image" = EXCLUDED."Image",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5788020d-59bc-5451-9c6d-c771a0ae54cd', 'hero_alchimalian_dragon', 'en-us', 'Alchimalian Dragon', 'The ultimate hero, embodying all traits to perfection.', 'This mighty dragon represents the pinnacle of courage, curiosity, thinking, creativity, and safety.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c1f2bb07-3cbb-5e15-8186-66bb2d5a4fea', 'hero_alchimalian_dragon', 'hu-hu', 'Legendás Alchimalia Sárkány', 'A legendás sárkány, aki a hősies tulajdonságok tökéletességét testesíti meg...', 'A Legendás Alchimalia Sárkány a kozmikus egyensúly tökéletességét testesíti meg.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c6288b3a-a0b9-5117-87c4-9ebba40d197e', 'hero_alchimalian_dragon', 'ro-ro', 'Legendarul Dragon Alchimalian', 'Legendarul dragon care încorporează perfecțiunea tuturor trăsăturilor eroice...', 'Legendarul Dragon Alchimalian încorporează perfecțiunea echilibrului cosmic.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
COMMIT;
