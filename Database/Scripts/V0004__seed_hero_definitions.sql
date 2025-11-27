-- Auto-generated from Data/SeedData/SharedConfigs/hero-tree.json + BookOfHeroes/i18n/*/hero-tree.json
-- Run date: 2025-11-27 08:13:24+02:00

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp'
    ) THEN
        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    END IF;
END $$;

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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:seed|en-us'), 'seed', 'en-us', 'Heroic Seed', 'The seed from which all heroes grow. Click to discover your heroic potential.', 'In your heart beats a seed of heroism. Click on it to discover your first heroes and begin your heroic journey.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:seed|hu-hu'), 'seed', 'hu-hu', 'Hősies Mag', 'A mag, amelyből minden hős nő. Kattints, hogy felfedezd hősies potenciálodat.', 'A szívedben egy hősiesség magja dobog. Kattints rá, hogy felfedezd első hőseidet és kezdd el hősies utadat.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:seed|ro-ro'), 'seed', 'ro-ro', 'Sămânța Eroică', 'Sămânța din care cresc toți eroii. Apasă pentru a descoperi potențialul tău eroic.', 'În inima ta bate o sămânță de eroism. Apasă pe ea pentru a descoperi primii tăi eroi și a începe călătoria ta eroică.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:alchimalia_hero|en-us'), 'alchimalia_hero', 'en-us', 'Alchimalia Hero', 'The root of all heroes, the source of transformation.', 'Your faithful companion, the beginning of the heroic journey.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:alchimalia_hero|hu-hu'), 'alchimalia_hero', 'hu-hu', 'Alchimalia Hőse', 'Minden hős gyökere, az átalakulás forrása.', 'Hűséges társad, a hősies út kezdete')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:alchimalia_hero|ro-ro'), 'alchimalia_hero', 'ro-ro', 'Eroul Alchimalia', 'Rădăcina tuturor eroilor, sursa transformării.', 'Companionul tău fidel, începutul călătoriei eroice')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_puppy', 'hero', 1, 0, 0, 0, 0,
     '"alchimalia_hero"', '[]', FALSE, -150, -200, 'images/toh/heroes/hero_brave_puppy.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_puppy|en-us'), 'hero_brave_puppy', 'en-us', 'Brave Puppy', 'A small pup full of courage and determination.', 'This little pup faces challenges head-on, inspiring bravery in everyone.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_puppy|hu-hu'), 'hero_brave_puppy', 'hu-hu', 'Bátor Kiskutya', 'Egy bátor lélek, aki megadja neked az erőt, hogy szembenézz bármilyen kihívással...', 'A Bátor Kiskutya megtanít, hogy az igazi bátorság nem a félelem hiánya, hanem a cselekvés a félelem ellenére.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_puppy|ro-ro'), 'hero_brave_puppy', 'ro-ro', 'Cățelul Curajos', 'Un spirit curajos care îți va da puterea să înfrunți orice provocare...', 'Cățelul Curajos te învață că adevăratul curaj nu înseamnă absența fricii, ci acțiunea în ciuda ei.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_pup_lvl2', 'hero', 2, 0, 0, 0, 0,
     '"hero_brave_puppy"', '[]', FALSE, -150, -280, 'images/toh/heroes/hero_guardian_dog.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_pup_lvl2|en-us'), 'hero_brave_pup_lvl2', 'en-us', 'Guardian Dog', 'A loyal protector with unwavering courage and devotion.', 'The Guardian Dog stands watch, defending those in need with brave determination.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_pup_lvl2|hu-hu'), 'hero_brave_pup_lvl2', 'hu-hu', 'Őrző Kutya', 'Egy hűséges védelmező rendíthetetlen bátorsággal és odaadással.', 'Az Őrző Kutya őrködik, megvédve a szükségben lévőket bátor elszántsággal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_pup_lvl2|ro-ro'), 'hero_brave_pup_lvl2', 'ro-ro', 'Câinele Gardian', 'Un protector loial cu curaj neclintit și devotament de neclintit.', 'Câinele Gardian stă de veghe, apărându-i pe cei în nevoie cu hotărâre curajoasă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_brave_pup_lvl3', 'hero', 3, 0, 0, 0, 0,
     '"hero_brave_pup_lvl2"', '[]', FALSE, -150, -360, 'images/toh/heroes/hero_legendary_dog.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_pup_lvl3|en-us'), 'hero_brave_pup_lvl3', 'en-us', 'Legendary Dog', 'A legendary hero of unmatched courage and noble spirit.', 'The Legendary Dog inspires legends, embodying the purest form of heroic courage.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_pup_lvl3|hu-hu'), 'hero_brave_pup_lvl3', 'hu-hu', 'Legendás Kutya', 'Egy legendás hős páratlan bátorsággal és nemes szellemmel.', 'A Legendás Kutya legendákat inspirál, a hősies bátorság legtisztább formáját megtestesítve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_pup_lvl3|ro-ro'), 'hero_brave_pup_lvl3', 'ro-ro', 'Câinele Legendar', 'Un erou legendar de curaj fără egal și spirit nobil.', 'Câinele Legendar inspiră legende, întruchipând forma cea mai pură a curajului eroic.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_cat', 'hero', 0, 1, 0, 0, 0,
     '"alchimalia_hero"', '[]', FALSE, 150, -200, 'images/toh/heroes/hero_curious_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat|en-us'), 'hero_curious_cat', 'en-us', 'Curious Cat', 'A curious being that explores every corner of the world.', 'The Curious Cat guides you through mysteries and wonders, awakening your curiosity.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat|hu-hu'), 'hero_curious_cat', 'hu-hu', 'Kíváncsi Macska', 'Egy kíváncsi lény, aki megnyitja a szemed a világ csodái felé...', 'A Kíváncsi Macska vezet téged a kérdések labirintusán, megmutatva, hogy minden rejtély egy csodát rejt.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat|ro-ro'), 'hero_curious_cat', 'ro-ro', 'Pisica Curioasă', 'O ființă curioasă care îți va deschide ochii către minunile lumii...', 'Pisica Curioasă te ghidează prin labirintul întrebărilor, arătându-ți că fiecare mister ascunde o minune.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_cat_lvl2', 'hero', 0, 2, 0, 0, 0,
     '"hero_curious_cat"', '[]', FALSE, 150, -280, 'images/toh/heroes/hero_explorer_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat_lvl2|en-us'), 'hero_curious_cat_lvl2', 'en-us', 'Explorer Cat', 'A fearless explorer driven by endless curiosity and wonder.', 'The Explorer Cat ventures into unknown territories, discovering hidden secrets and mysteries.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat_lvl2|hu-hu'), 'hero_curious_cat_lvl2', 'hu-hu', 'Felfedező Macska', 'Egy rettenthetetlen felfedező, akit végtelen kíváncsiság és csodálat vezet.', 'A Felfedező Macska ismeretlen területekre merészkedik, rejtett titkokat és rejtélyeket fedezve fel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat_lvl2|ro-ro'), 'hero_curious_cat_lvl2', 'ro-ro', 'Pisica Exploratoare', 'O exploratoare neînfricată condusă de curiozitate și minunărie nesfârșite.', 'Pisica Exploratoare se aventurează în teritorii necunoscute, descoperind secrete ascunse și mistere.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_curious_cat_lvl3', 'hero', 0, 3, 0, 0, 0,
     '"hero_curious_cat_lvl2"', '[]', FALSE, 150, -360, 'images/toh/heroes/hero_oracle_cat.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat_lvl3|en-us'), 'hero_curious_cat_lvl3', 'en-us', 'Oracle Cat', 'A mystical seer with profound insight and prophetic wisdom.', 'The Oracle Cat sees beyond the veil of reality, revealing truths hidden from ordinary sight.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat_lvl3|hu-hu'), 'hero_curious_cat_lvl3', 'hu-hu', 'Jósló Macska', 'Egy misztikus látnok mély megértéssel és prófétai bölcsességgel.', 'A Jósló Macska a valóság fátyla mögé lát, igazságokat tárva fel, amelyek a hétköznapi szemnek rejtve maradnak.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_cat_lvl3|ro-ro'), 'hero_curious_cat_lvl3', 'ro-ro', 'Pisica Oracol', 'O văzătoare mistică cu înțelegere profundă și înțelepciune profetică.', 'Pisica Oracol vede dincolo de vălul realității, dezvăluind adevăruri ascunse de ochiul obișnuit.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_owl', 'hero', 0, 0, 1, 0, 0,
     '"alchimalia_hero"', '[]', FALSE, 0, -220, 'images/toh/heroes/hero_wise_owl.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl|en-us'), 'hero_wise_owl', 'en-us', 'Wise Owl', 'A keen observer, full of knowledge and insight.', 'The Wise Owl watches over the world, sharing wisdom and guidance to seekers.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl|hu-hu'), 'hero_wise_owl', 'hu-hu', 'Bölcs Bagoly', 'Egy bölcs mentor, aki megosztja veled a tudás titkait...', 'A Bölcs Bagoly suttogja a éjszaka titkait és megnyitja a szemed a rejtett igazságok felé.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl|ro-ro'), 'hero_wise_owl', 'ro-ro', 'Bufnița Înțeleaptă', 'Un mentor înțelept care îți va împărtăși secretele cunoașterii...', 'Bufnița Înțeleaptă îți șoptește secretele nopții și îți deschide ochii către adevărurile ascunse.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_owl_lvl2', 'hero', 0, 0, 2, 0, 0,
     '"hero_wise_owl"', '[]', FALSE, 0, -300, 'images/toh/heroes/hero_scholar_owl.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl_lvl2|en-us'), 'hero_wise_owl_lvl2', 'en-us', 'Scholar Owl', 'A learned scholar with deep knowledge and analytical thinking.', 'The Scholar Owl studies the mysteries of the world, sharing wisdom through careful analysis.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl_lvl2|hu-hu'), 'hero_wise_owl_lvl2', 'hu-hu', 'Tudós Bagoly', 'Egy tanult tudós mély tudással és analitikus gondolkodással.', 'A Tudós Bagoly a világ rejtélyeit tanulmányozza, bölcsességet osztva meg gondos elemzésen keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl_lvl2|ro-ro'), 'hero_wise_owl_lvl2', 'ro-ro', 'Bufnița Savantă', 'O savantă învățată cu cunoaștere profundă și gândire analitică.', 'Bufnița Savantă studiază misterele lumii, împărtășind înțelepciunea prin analiză atentă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_wise_owl_lvl3', 'hero', 0, 0, 3, 0, 0,
     '"hero_wise_owl_lvl2"', '[]', FALSE, 0, -380, 'images/toh/heroes/hero_ancient_owl.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl_lvl3|en-us'), 'hero_wise_owl_lvl3', 'en-us', 'Ancient Owl', 'An ancient sage with timeless wisdom and eternal knowledge.', 'The Ancient Owl has witnessed the ages, holding the deepest secrets of the universe.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl_lvl3|hu-hu'), 'hero_wise_owl_lvl3', 'hu-hu', 'Ősi Bagoly', 'Egy ősi bölcs időtlen bölcsességgel és örök tudással.', 'Az Ősi Bagoly tanúja volt a koroknak, a világegyetem legmélyebb titkait őrizve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_owl_lvl3|ro-ro'), 'hero_wise_owl_lvl3', 'ro-ro', 'Bufnița Antică', 'O înțeleaptă antică cu înțelepciune atemporală și cunoaștere eternă.', 'Bufnița Antică a fost martoră la epoci, păstrând cele mai profunde secrete ale universului.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_playful_horse', 'hero', 0, 0, 0, 1, 0,
     '"alchimalia_hero"', '[]', FALSE, -200, -280, 'images/toh/heroes/hero_playful_horse.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse|en-us'), 'hero_playful_horse', 'en-us', 'Playful Horse', 'A joyful creature, bringing creativity and fun wherever it goes.', 'The Playful Horse gallops freely, inspiring imagination and playful ideas.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse|hu-hu'), 'hero_playful_horse', 'hu-hu', 'Játékos Kiscsikó', 'Egy játékos társ, aki korlátok nélkül felszabadítja a képzeletedet...', 'A Játékos Kiscsikó hozza neked a tiszta alkotás örömét, ahol a képzelet kantár nélkül vágtázik.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse|ro-ro'), 'hero_playful_horse', 'ro-ro', 'Căluțul Jucăuș', 'Un companion jucăuș care îți va elibera imaginația fără limite...', 'Căluțul Jucăuș îți aduce bucuria creației pure, unde imaginația galopează fără frâu.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_playful_horse_lvl2', 'hero', 0, 0, 0, 2, 0,
     '"hero_playful_horse"', '[]', FALSE, -200, -360, 'images/toh/heroes/hero_artistic_horse.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse_lvl2|en-us'), 'hero_playful_horse_lvl2', 'en-us', 'Artistic Horse', 'A creative artist expressing beauty through imagination and skill.', 'The Artistic Horse creates masterpieces, bringing dreams to life through creative expression.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse_lvl2|hu-hu'), 'hero_playful_horse_lvl2', 'hu-hu', 'Művészi Ló', 'Egy kreatív művész, aki szépséget fejez ki képzelet és mesterség révén.', 'A Művészi Ló remekműveket alkot, álmokat életre hozva kreatív kifejezésen keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse_lvl2|ro-ro'), 'hero_playful_horse_lvl2', 'ro-ro', 'Calul Artistic', 'Un artist creativ care exprimă frumusețea prin imaginație și măiestrie.', 'Calul Artistic creează capodopere, aducând visele la viață prin expresie creativă.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_playful_horse_lvl3', 'hero', 0, 0, 0, 3, 0,
     '"hero_playful_horse_lvl2"', '[]', FALSE, -200, -440, 'images/toh/heroes/hero_genius_horse.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse_lvl3|en-us'), 'hero_playful_horse_lvl3', 'en-us', 'Genius Horse', 'A creative genius with unparalleled artistic vision and innovation.', 'The Genius Horse transcends ordinary creativity, crafting works of pure artistic brilliance.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse_lvl3|hu-hu'), 'hero_playful_horse_lvl3', 'hu-hu', 'Zseni Ló', 'Egy kreatív zseni páratlan művészi látomással és innovációval.', 'A Zseni Ló túlmutat a hétköznapi kreativitáson, tiszta művészi ragyogás műveket alkotva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_playful_horse_lvl3|ro-ro'), 'hero_playful_horse_lvl3', 'ro-ro', 'Calul Geniu', 'Un geniu creativ cu viziune artistică și inovație fără egal.', 'Calul Geniu transcende creativitatea obișnuită, creând opere de pură strălucire artistică.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_cautious_hedgehog', 'hero', 0, 0, 0, 0, 1,
     '"alchimalia_hero"', '[]', FALSE, 200, -280, 'images/toh/heroes/hero_cautious_hedgehog.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog|en-us'), 'hero_cautious_hedgehog', 'en-us', 'Cautious Hedgehog', 'A careful protector, mindful of danger and safety.', 'The Hedgehog teaches prudence and foresight, keeping everyone safe.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog|hu-hu'), 'hero_cautious_hedgehog', 'hu-hu', 'Óvatos Sün', 'Egy óvatos védelmező, aki biztonságot és egyensúlyt ad neked...', 'Az Óvatos Sün megtanít, hogy a biztonság nem félelmet jelent, hanem bölcsességet.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog|ro-ro'), 'hero_cautious_hedgehog', 'ro-ro', 'Ariciul Precaut', 'Un protector precaut care îți va oferi siguranța și echilibrul...', 'Ariciul Precaut îți învață că siguranța nu înseamnă teamă, ci înțelepciune.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_cautious_hedgehog_lvl2', 'hero', 0, 0, 0, 0, 2,
     '"hero_cautious_hedgehog"', '[]', FALSE, 200, -380, 'images/toh/heroes/hero_shieldquill.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog_lvl2|en-us'), 'hero_cautious_hedgehog_lvl2', 'en-us', 'Shieldquill', 'Careful protection built from prudence and experience.', 'Uses its quills as a shield to guard what matters most.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog_lvl2|hu-hu'), 'hero_cautious_hedgehog_lvl2', 'hu-hu', 'Pajzs-Tüskés', 'Gondos védelem, óvatosságból és tapasztalatból építve.', 'Tüskéit pajzsként használja, hogy megvédje, ami a legfontosabb.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog_lvl2|ro-ro'), 'hero_cautious_hedgehog_lvl2', 'ro-ro', 'Ariciul Scut-Spini', 'Protecție atentă, construită din prudență și experiență.', 'Își folosește spinii ca scut pentru a apăra ceea ce contează cel mai mult.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitions"
    ("Id", "Type", "CourageCost", "CuriosityCost", "ThinkingCost", "CreativityCost", "SafetyCost",
     "PrerequisitesJson", "RewardsJson", "IsUnlocked", "PositionX", "PositionY", "Image", "CreatedAt", "UpdatedAt")
VALUES
    ('hero_cautious_hedgehog_lvl3', 'hero', 0, 0, 0, 0, 3,
     '"hero_cautious_hedgehog_lvl2"', '[]', FALSE, 200, -480, 'images/toh/heroes/hero_wardenspike.jpg', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog_lvl3|en-us'), 'hero_cautious_hedgehog_lvl3', 'en-us', 'Wardenspike', 'Unyielding sentinel, determined to uphold safety.', 'Turns vigilance into an art, keeping watch at the world''s edges.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog_lvl3|hu-hu'), 'hero_cautious_hedgehog_lvl3', 'hu-hu', 'Őrző Tüske', 'Rendíthetetlen őrszem, elszántan a biztonság fenntartására.', 'Az őrséget művészetté alakítja, a világ határain őrködve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_cautious_hedgehog_lvl3|ro-ro'), 'hero_cautious_hedgehog_lvl3', 'ro-ro', 'Paznicul Țepușă', 'Strajă de neclintit, hotărâtă să mențină siguranța.', 'Transformă vigilența în artă, stând de veghe la hotarele lumii.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_catdog_interstellar|en-us'), 'hero_catdog_interstellar', 'en-us', 'CatDog Interstellar', 'A hybrid of curiosity and courage, ready for interstellar adventures.', 'This heroic hybrid combines the best traits of cat and dog, exploring the cosmos fearlessly.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_catdog_interstellar|hu-hu'), 'hero_catdog_interstellar', 'hu-hu', 'Csillagközi Macska-Kutya', 'Egy kozmikus felfedező, aki a bátorság és kíváncsiság egyesüléséből született...', 'A Csillagközi Macska-Kutya egyesíti a végtelen kíváncsiságot a bátorsággal, hogy felfedezze az ismeretlent.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_catdog_interstellar|ro-ro'), 'hero_catdog_interstellar', 'ro-ro', 'CatDog Interstelar', 'Un explorator cosmic născut din uniunea curajului și curiozității...', 'CatDog Interstelar îmbină curiozitatea infinită cu curajul de a explora necunoscutul.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_sage|en-us'), 'hero_brave_sage', 'en-us', 'Brave Sage', 'A mix of wisdom and bravery, guiding heroes with insight.', 'The Brave Sage combines intellect and courage to lead others through challenges.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_sage|hu-hu'), 'hero_brave_sage', 'hu-hu', 'Bátor Bölcs', 'Egy bölcs lélek, aki bátorságot és bölcsességet egyesít...', 'A Bátor Bölcs megmutatja, hogy az igazi erő a tudás és a bátorság harmóniájában rejlik.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_sage|ro-ro'), 'hero_brave_sage', 'ro-ro', 'Înțeleptul Curajos', 'Un erou ce îmbină curajul cu înțelepciunea, acționând cu o putere chibzuită.', 'Înțeleptul Curajos te învață că adevărata putere nu este doar lupta, ci a ști când și de ce să lupți.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_artist|en-us'), 'hero_brave_artist', 'en-us', 'Brave Artist', 'A creative soul with a courageous heart.', 'The Brave Artist paints heroic tales with bravery and imagination.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_artist|hu-hu'), 'hero_brave_artist', 'hu-hu', 'Bátor Művész', 'Egy hős, aki mer alkotni ott, ahol mások félnek, új horizontokat nyitva művészetével.', 'A Bátor Művész megmutatja, hogy az alkotás cselekedete a legfőbb bátorság, új világokat életre hozva a semmiből.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_artist|ro-ro'), 'hero_brave_artist', 'ro-ro', 'Artistul Curajos', 'Un erou ce îndrăznește să creeze acolo unde alții se tem, deschizând noi orizonturi prin arta sa.', 'Artistul Curajos arată că actul creației este curajul suprem, aducând la viață lumi noi din nimic.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_guardian|en-us'), 'hero_brave_guardian', 'en-us', 'Light Wolf', 'A guardian of courage, standing firm against all threats.', 'Light Wolf protects the realm, embodying courage and vigilance.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_guardian|hu-hu'), 'hero_brave_guardian', 'hu-hu', 'Bátor Őrző', 'Egy hős, ahol a bátorság és védelem kéz a kézben jár, védve habozás nélkül, de bölcsen.', 'A Bátor Őrző a remegtetlen pajzs, megvédve az ártatlanokat rendíthetetlen elszántsággal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_guardian|ro-ro'), 'hero_brave_guardian', 'ro-ro', 'Gardianul Curajos', 'Un erou unde curajul și protecția merg mână în mână, apărând fără ezitare, dar cu înțelepciune.', 'Gardianul Curajos este scutul ce nu tremură, protejând inocenții cu o hotărâre de neclintit.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_researcher|en-us'), 'hero_wise_researcher', 'en-us', 'Wise Researcher', 'Curious and analytical, seeking knowledge in every corner.', 'This hero uncovers secrets and guides others with insightful discoveries.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_researcher|hu-hu'), 'hero_wise_researcher', 'hu-hu', 'Bölcs Kutató', 'Egy hős, aki egyesíti az analitikus elmét a fáradhatatlan kíváncsisággal, hogy rejtett igazságokat fedezzen fel.', 'A Bölcs Kutató követi a tudás fonalát a legnagyobb labirintusokon keresztül, fényt találva a sötétségben.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_researcher|ro-ro'), 'hero_wise_researcher', 'ro-ro', 'Cercetătorul Înțelept', 'Un erou ce unește mintea analitică cu curiozitatea neobosită pentru a descoperi adevăruri ascunse.', 'Cercetătorul Înțelept urmărește firul cunoașterii prin cele mai mari labirinturi, găsind lumină în întuneric.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_creative_explorer|en-us'), 'hero_creative_explorer', 'en-us', 'Creative Explorer', 'Adventurous and inventive, always finding new paths.', 'The Creative Explorer charts unknown lands with imagination and curiosity.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_creative_explorer|hu-hu'), 'hero_creative_explorer', 'hu-hu', 'Kreatív Felfedező', 'Egy hős, aki egyesíti a képzeletet a megismerés vágyával; minden lépés egy új találmány.', 'A Kreatív Felfedező számára a térkép rajzolódik ki, ahogy bejárja, az ismeretlent művészeti alkotássá alakítva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_creative_explorer|ro-ro'), 'hero_creative_explorer', 'ro-ro', 'Exploratorul Creativ', 'Un erou ce îmbină imaginația cu dorința de a cunoaște; fiecare pas e o nouă invenție.', 'Pentru Exploratorul Creativ, harta se desenează pe măsură ce este parcursă, transformând necunoscutul într-o operă de artă.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_guard_cat|en-us'), 'hero_guard_cat', 'en-us', 'Guard Cat', 'A protective hybrid, attentive and vigilant.', 'The Guard Cat keeps watch, blending caution and curiosity in every move.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_guard_cat|hu-hu'), 'hero_guard_cat', 'hu-hu', 'Őrző Macska', 'Egy kalandos védelmező, aki óvatosan felfedez.', 'Az Őrző Macska ismeri területének minden szögletét, biztosítva, hogy minden kíváncsiság kielégüljön a biztonság veszélyeztetése nélkül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_guard_cat|ro-ro'), 'hero_guard_cat', 'ro-ro', 'Pisica de Gardă', 'Un protector aventuros care explorează cu prudență.', 'Pisica de Gardă cunoaște fiecare colț al teritoriului său, asigurându-se că orice curiozitate este satisfăcută fără a compromite siguranța.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_pegasus|en-us'), 'hero_wise_pegasus', 'en-us', 'Wise Pegasus', 'A majestic creature of intellect and creativity.', 'The Wise Pegasus flies through challenges, offering guidance and inspiration.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_pegasus|hu-hu'), 'hero_wise_pegasus', 'hu-hu', 'Bölcs Pegazus', 'A tudás őrzője, aki a határokon túl tud repülni.', 'A Bölcs Pegazus gondolatokat visz szárnyain, az absztrakt ötleteket repülő valóságokká alakítva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_pegasus|ro-ro'), 'hero_wise_pegasus', 'ro-ro', 'Pegazul Înțelept', 'Un gardian al cunoașterii care poate zbura dincolo de limite.', 'Pegazul Înțelept poartă gândurile pe aripile sale, transformând ideile abstracte în realități zburătoare.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_thoughtful_guardian|en-us'), 'hero_thoughtful_guardian', 'en-us', 'Thoughtful Guardian', 'A careful thinker, combining safety with wisdom.', 'This hero balances prudence and intelligence to protect the realm.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_thoughtful_guardian|hu-hu'), 'hero_thoughtful_guardian', 'hu-hu', 'Gondolkodó Őrző', 'Egy hős, aki cselekvés előtt tervez; a biztonság jól megfontolt stratégiákból ered.', 'A Gondolkodó Őrző nem kőből épít erődöket, hanem logikából és előrelátásból.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_thoughtful_guardian|ro-ro'), 'hero_thoughtful_guardian', 'ro-ro', 'Gardianul Gânditor', 'Un erou ce planifică înainte de a acționa; siguranța vine din strategiile bine gândite.', 'Gardianul Gânditor construiește fortărețe nu din piatră, ci din logică și prevedere.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_creative_guardian|en-us'), 'hero_creative_guardian', 'en-us', 'Creative Guardian', 'A guardian blending imagination with caution.', 'The Creative Guardian defends while inspiring inventive solutions.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_creative_guardian|hu-hu'), 'hero_creative_guardian', 'hu-hu', 'Kreatív Őrző', 'Egy találékony védelmező, aki eredeti megoldásokat talál a biztonság biztosítására.', 'A Kreatív Őrző nem falakat épít, hanem egy olyan világot képzel el, ahol ezek nem szükségesek.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_creative_guardian|ro-ro'), 'hero_creative_guardian', 'ro-ro', 'Gardianul Creativ', 'Un protector inventiv care găsește soluții originale pentru a asigura siguranța.', 'Gardianul Creativ protejează lumea nu construind ziduri, ci imaginând o lume în care acestea nu sunt necesare.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise|en-us'), 'hero_brave_curious_wise', 'en-us', 'Wise Lion Sage', 'A majestic lion combining courage, curiosity, and wisdom.', 'The Wise Lion Sage leads with both strength and insight, inspiring others through noble example.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise|hu-hu'), 'hero_brave_curious_wise', 'hu-hu', 'Bölcs Oroszlán Bölcs', 'Egy fenséges oroszlán, aki egyesíti a bátorságot, kíváncsiságot és bölcsességet.', 'A Bölcs Oroszlán Bölcs erővel és megértéssel vezet, másokat inspirálva nemes példájával.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise|ro-ro'), 'hero_brave_curious_wise', 'ro-ro', 'Leul Înțelept Savant', 'Un leu măreț care îmbină curajul, curiozitatea și înțelepciunea.', 'Leul Înțelept Savant conduce cu atât putere cât și înțelegere, inspirându-i pe alții prin exemplul nobil.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_creative|en-us'), 'hero_brave_curious_creative', 'en-us', 'Brave Lion Artist', 'A courageous lion expressing creativity through bold artistic vision.', 'The Brave Lion Artist creates masterpieces that inspire courage and imagination in all who see them.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_creative|hu-hu'), 'hero_brave_curious_creative', 'hu-hu', 'Bátor Oroszlán Művész', 'Egy bátor oroszlán, aki kreativitást fejez ki merész művészi látomáson keresztül.', 'A Bátor Oroszlán Művész remekműveket alkot, amelyek bátorságot és képzeletet inspirálnak mindenki számára, aki látja őket.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_creative|ro-ro'), 'hero_brave_curious_creative', 'ro-ro', 'Leul Curajos Artist', 'Un leu curajos care exprimă creativitatea prin viziune artistică îndrăzneață.', 'Leul Curajos Artist creează capodopere care inspiră curaj și imaginație în toți cei care le văd.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_safe|en-us'), 'hero_brave_curious_safe', 'en-us', 'Guardian Lion', 'A protective lion combining courage, curiosity, and safety.', 'The Guardian Lion watches over the realm, ensuring safety while encouraging exploration and discovery.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_safe|hu-hu'), 'hero_brave_curious_safe', 'hu-hu', 'Őrző Oroszlán', 'Egy védelmező oroszlán, aki egyesíti a bátorságot, kíváncsiságot és biztonságot.', 'Az Őrző Oroszlán a birodalom felett őrködik, biztonságot biztosítva, miközben felfedezést és megismerést ösztönöz.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_safe|ro-ro'), 'hero_brave_curious_safe', 'ro-ro', 'Leul Gardian', 'Un leu protector care îmbină curajul, curiozitatea și siguranța.', 'Leul Gardian veghează asupra regatului, asigurând siguranța în timp ce încurajează explorarea și descoperirea.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_creative|en-us'), 'hero_brave_wise_creative', 'en-us', 'Brave Sage Artist', 'A wise artist blending courage, thinking, and creativity.', 'The Brave Sage Artist creates profound works that combine intellectual depth with artistic beauty.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_creative|hu-hu'), 'hero_brave_wise_creative', 'hu-hu', 'Bátor Bölcs Művész', 'Egy bölcs művész, aki egyesíti a bátorságot, gondolkodást és kreativitást.', 'A Bátor Bölcs Művész mély műveket alkot, amelyek egyesítik az intellektuális mélységet a művészi szépséggel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_creative|ro-ro'), 'hero_brave_wise_creative', 'ro-ro', 'Artistul Înțelept Curajos', 'Un artist înțelept care îmbină curajul, gândirea și creativitatea.', 'Artistul Înțelept Curajos creează opere profunde care combină adâncimea intelectuală cu frumusețea artistică.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_safe|en-us'), 'hero_brave_wise_safe', 'en-us', 'Brave Sage Guardian', 'A protective sage combining courage, wisdom, and safety.', 'The Brave Sage Guardian defends with both strength and wisdom, ensuring safety through thoughtful protection.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_safe|hu-hu'), 'hero_brave_wise_safe', 'hu-hu', 'Bátor Bölcs Őrző', 'Egy védelmező bölcs, aki egyesíti a bátorságot, bölcsességet és biztonságot.', 'A Bátor Bölcs Őrző erővel és bölcsességgel véd, biztonságot biztosítva gondos védelemmel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_safe|ro-ro'), 'hero_brave_wise_safe', 'ro-ro', 'Gardianul Înțelept Curajos', 'Un gardian protector care îmbină curajul, înțelepciunea și siguranța.', 'Gardianul Înțelept Curajos se apără cu atât putere cât și înțelepciune, asigurând siguranța prin protecție gândită.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_creative_safe|en-us'), 'hero_brave_creative_safe', 'en-us', 'Brave Artist Guardian', 'A creative guardian blending courage, creativity, and safety.', 'The Brave Artist Guardian protects through creative solutions, combining artistic vision with vigilant care.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_creative_safe|hu-hu'), 'hero_brave_creative_safe', 'hu-hu', 'Bátor Művész Őrző', 'Egy kreatív őrző, aki egyesíti a bátorságot, kreativitást és biztonságot.', 'A Bátor Művész Őrző kreatív megoldásokkal véd, művészi látomást kombinálva figyelmes gondossággal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_creative_safe|ro-ro'), 'hero_brave_creative_safe', 'ro-ro', 'Gardianul Artist Curajos', 'Un gardian creativ care îmbină curajul, creativitatea și siguranța.', 'Gardianul Artist Curajos protejează prin soluții creative, combinând viziunea artistică cu grija vigilentă.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_creative|en-us'), 'hero_curious_wise_creative', 'en-us', 'Wise Explorer Artist', 'An intellectual artist combining curiosity, thinking, and creativity.', 'The Wise Explorer Artist discovers new artistic frontiers through thoughtful exploration and creative expression.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_creative|hu-hu'), 'hero_curious_wise_creative', 'hu-hu', 'Bölcs Felfedező Művész', 'Egy intellektuális művész, aki egyesíti a kíváncsiságot, gondolkodást és kreativitást.', 'A Bölcs Felfedező Művész új művészi határokat fedez fel gondos felfedezésen és kreatív kifejezésen keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_creative|ro-ro'), 'hero_curious_wise_creative', 'ro-ro', 'Artistul Explorator Înțelept', 'Un artist intelectual care îmbină curiozitatea, gândirea și creativitatea.', 'Artistul Explorator Înțelept descoperă noi frontiere artistice prin explorare gândită și expresie creativă.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_safe|en-us'), 'hero_curious_wise_safe', 'en-us', 'Wise Guardian Explorer', 'A protective explorer combining curiosity, wisdom, and safety.', 'The Wise Guardian Explorer ventures safely into unknown territories, guided by wisdom and protected by caution.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_safe|hu-hu'), 'hero_curious_wise_safe', 'hu-hu', 'Bölcs Őrző Felfedező', 'Egy védelmező felfedező, aki egyesíti a kíváncsiságot, bölcsességet és biztonságot.', 'A Bölcs Őrző Felfedező biztonságosan merészkedik ismeretlen területekre, bölcsesség vezetésével és óvatosság védelmével.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_safe|ro-ro'), 'hero_curious_wise_safe', 'ro-ro', 'Exploratorul Gardian Înțelept', 'Un explorator protector care îmbină curiozitatea, înțelepciunea și siguranța.', 'Exploratorul Gardian Înțelept se aventurează în siguranță în teritorii necunoscute, ghidat de înțelepciune și protejat de precauție.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_creative_safe|en-us'), 'hero_curious_creative_safe', 'en-us', 'Creative Guardian Explorer', 'An inventive explorer combining curiosity, creativity, and safety.', 'The Creative Guardian Explorer discovers new worlds through imaginative solutions while ensuring safe passage.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_creative_safe|hu-hu'), 'hero_curious_creative_safe', 'hu-hu', 'Kreatív Őrző Felfedező', 'Egy találékony felfedező, aki egyesíti a kíváncsiságot, kreativitást és biztonságot.', 'A Kreatív Őrző Felfedező új világokat fedez fel képzeletbeli megoldásokon keresztül, miközben biztonságos átkelést biztosít.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_creative_safe|ro-ro'), 'hero_curious_creative_safe', 'ro-ro', 'Exploratorul Gardian Creativ', 'Un explorator inventiv care îmbină curiozitatea, creativitatea și siguranța.', 'Exploratorul Gardian Creativ descoperă lumi noi prin soluții imaginare în timp ce asigură trecerea în siguranță.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_creative_safe|en-us'), 'hero_wise_creative_safe', 'en-us', 'Wise Creative Guardian', 'A thoughtful guardian combining wisdom, creativity, and safety.', 'The Wise Creative Guardian protects through innovative solutions, blending intellectual insight with artistic vision.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_creative_safe|hu-hu'), 'hero_wise_creative_safe', 'hu-hu', 'Bölcs Kreatív Őrző', 'Egy gondolkodó őrző, aki egyesíti a bölcsességet, kreativitást és biztonságot.', 'A Bölcs Kreatív Őrző innovatív megoldásokkal véd, intellektuális megértést kombinálva művészi látomással.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_wise_creative_safe|ro-ro'), 'hero_wise_creative_safe', 'ro-ro', 'Gardianul Creativ Înțelept', 'Un gardian gânditor care îmbină înțelepciunea, creativitatea și siguranța.', 'Gardianul Creativ Înțelept protejează prin soluții inovatoare, îmbinând înțelegerea intelectuală cu viziunea artistică.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise_creative|en-us'), 'hero_brave_curious_wise_creative', 'en-us', 'Master Lion Artist', 'The ultimate artistic lion, master of courage, curiosity, wisdom, and creativity.', 'The Master Lion Artist creates legendary works that inspire all who witness their artistic genius and noble spirit.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise_creative|hu-hu'), 'hero_brave_curious_wise_creative', 'hu-hu', 'Mester Oroszlán Művész', 'A végső művészi oroszlán, a bátorság, kíváncsiság, bölcsesség és kreativitás mestere.', 'A Mester Oroszlán Művész legendás műveket alkot, amelyek mindenkit inspirálnak, aki tanúja művészi zsenialitásának és nemes szellemének.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise_creative|ro-ro'), 'hero_brave_curious_wise_creative', 'ro-ro', 'Maestrul Leul Artist', 'Leul artistic suprem, maestru al curajului, curiozității, înțelepciunii și creativității.', 'Maestrul Leul Artist creează opere legendare care inspiră toți cei care asistă la geniul său artistic și spiritul nobil.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise_safe|en-us'), 'hero_brave_curious_wise_safe', 'en-us', 'Master Guardian Lion', 'The ultimate protective lion, master of courage, curiosity, wisdom, and safety.', 'The Master Guardian Lion stands as the ultimate protector, ensuring safety while encouraging exploration and growth.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise_safe|hu-hu'), 'hero_brave_curious_wise_safe', 'hu-hu', 'Mester Őrző Oroszlán', 'A végső védelmező oroszlán, a bátorság, kíváncsiság, bölcsesség és biztonság mestere.', 'A Mester Őrző Oroszlán a végső védelmezőként áll, biztonságot biztosítva, miközben felfedezést és növekedést ösztönöz.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_wise_safe|ro-ro'), 'hero_brave_curious_wise_safe', 'ro-ro', 'Maestrul Leul Gardian', 'Leul protector suprem, maestru al curajului, curiozității, înțelepciunii și siguranței.', 'Maestrul Leul Gardian stă ca protectorul suprem, asigurând siguranța în timp ce încurajează explorarea și creșterea.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_creative_safe|en-us'), 'hero_brave_curious_creative_safe', 'en-us', 'Master Explorer Lion', 'The ultimate exploring lion, master of courage, curiosity, creativity, and safety.', 'The Master Explorer Lion ventures into the most challenging territories, discovering wonders while ensuring safe passage.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_creative_safe|hu-hu'), 'hero_brave_curious_creative_safe', 'hu-hu', 'Mester Felfedező Oroszlán', 'A végső felfedező oroszlán, a bátorság, kíváncsiság, kreativitás és biztonság mestere.', 'A Mester Felfedező Oroszlán a legkihívóbb területekre merészkedik, csodákat fedezve fel, miközben biztonságos átkelést biztosít.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_curious_creative_safe|ro-ro'), 'hero_brave_curious_creative_safe', 'ro-ro', 'Maestrul Leul Explorator', 'Leul explorator suprem, maestru al curajului, curiozității, creativității și siguranței.', 'Maestrul Leul Explorator se aventurează în cele mai provocatoare teritorii, descoperind minunății în timp ce asigură trecerea în siguranță.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_creative_safe|en-us'), 'hero_brave_wise_creative_safe', 'en-us', 'Master Sage Guardian', 'The ultimate wise guardian, master of courage, thinking, creativity, and safety.', 'The Master Sage Guardian combines profound wisdom with creative protection, ensuring safety through enlightened guidance.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_creative_safe|hu-hu'), 'hero_brave_wise_creative_safe', 'hu-hu', 'Mester Bölcs Őrző', 'A végső bölcs őrző, a bátorság, gondolkodás, kreativitás és biztonság mestere.', 'A Mester Bölcs Őrző mély bölcsességet kombinál kreatív védelemmel, biztonságot biztosítva megvilágosodott vezetéssel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_brave_wise_creative_safe|ro-ro'), 'hero_brave_wise_creative_safe', 'ro-ro', 'Maestrul Gardian Înțelept', 'Gardianul înțelept suprem, maestru al curajului, gândirii, creativității și siguranței.', 'Maestrul Gardian Înțelept combină înțelepciunea profundă cu protecția creativă, asigurând siguranța prin ghidare iluminată.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_creative_safe|en-us'), 'hero_curious_wise_creative_safe', 'en-us', 'Master Creative Explorer', 'The ultimate creative explorer, master of curiosity, thinking, creativity, and safety.', 'The Master Creative Explorer discovers new realms of possibility through innovative thinking and artistic vision.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_creative_safe|hu-hu'), 'hero_curious_wise_creative_safe', 'hu-hu', 'Mester Kreatív Felfedező', 'A végső kreatív felfedező, a kíváncsiság, gondolkodás, kreativitás és biztonság mestere.', 'A Mester Kreatív Felfedező új lehetőségek birodalmait fedez fel innovatív gondolkodáson és művészi látomáson keresztül.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_curious_wise_creative_safe|ro-ro'), 'hero_curious_wise_creative_safe', 'ro-ro', 'Maestrul Explorator Creativ', 'Exploratorul creativ suprem, maestru al curiozității, gândirii, creativității și siguranței.', 'Maestrul Explorator Creativ descoperă noi regnuri ale posibilității prin gândire inovatoare și viziune artistică.')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_alchimalian_dragon|en-us'), 'hero_alchimalian_dragon', 'en-us', 'Alchimalian Dragon', 'The ultimate hero, embodying all traits to perfection.', 'This mighty dragon represents the pinnacle of courage, curiosity, thinking, creativity, and safety.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_alchimalian_dragon|hu-hu'), 'hero_alchimalian_dragon', 'hu-hu', 'Legendás Alchimalia Sárkány', 'A legendás sárkány, aki a hősies tulajdonságok tökéletességét testesíti meg...', 'A Legendás Alchimalia Sárkány a kozmikus egyensúly tökéletességét testesíti meg.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'hero-def-tr:hero_alchimalian_dragon|ro-ro'), 'hero_alchimalian_dragon', 'ro-ro', 'Legendarul Dragon Alchimalian', 'Legendarul dragon care încorporează perfecțiunea tuturor trăsăturilor eroice...', 'Legendarul Dragon Alchimalian încorporează perfecțiunea echilibrului cosmic.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
COMMIT;
