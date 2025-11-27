-- Auto-generated from Data/SeedData/Stories/seed@alchimalia.com (mode: main)
-- Run date: 2025-11-27 10:07:08+02:00

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp'
    ) THEN
        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    END IF;
END $$;

BEGIN;

-- Story Definitions
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:crystalia-s1'), 'crystalia-s1', 'Crystalia', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/crystalia-s1/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:crystalia-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:crystalia-s1'), 'ro-ro', 'Crystalia')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:crystalia-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:crystalia-s1'), 'en-us', 'Crystalia')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:crystalia-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:crystalia-s1'), 'hu-hu', 'Crystalia')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for crystalia-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:crystalia-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:crystalia-s1'), '1', 'page', 1, 'Crystalia', 'Crystalia', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/crystalia-s1/0.Cover.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:crystalia-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:crystalia-s1:1'), 'ro-ro', 'Crystalia', 'Crystalia', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:crystalia-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:crystalia-s1:1'), 'en-us', 'Crystalia', 'Crystalia', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:crystalia-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:crystalia-s1:1'), 'hu-hu', 'Crystalia', 'Crystalia', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), 'intro-pufpuf', 'Marea Călătorie', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:intro-pufpuf|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), 'ro-ro', 'Marea Călătorie')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:intro-pufpuf|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), 'en-us', 'The Great Journey')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:intro-pufpuf|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), 'hu-hu', 'A Nagy Utazás')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for intro-pufpuf
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), '1', 'page', 1, 'Chemarea', 'Puf-Puf călătorea prin adâncul universului când liniștea cosmică fu spartă de un vuiet straniu, ca un oftat al stelelor. \nInstrumentele pâlpâiră, iar busola stelară desenă spirale fără sens. \nCu lăbuța pe parbrizul de cuarț, el simți mai mult, decât auzi, că cineva îl cheamă pe nume dintr-un loc foarte, foarte vechi.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/1.puf-puf-flying.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:1'), 'ro-ro', 'Chemarea', 'Puf-Puf călătorea prin adâncul universului când liniștea cosmică fu spartă de un vuiet straniu, ca un oftat al stelelor. \nInstrumentele pâlpâiră, iar busola stelară desenă spirale fără sens. \nCu lăbuța pe parbrizul de cuarț, el simți mai mult, decât auzi, că cineva îl cheamă pe nume dintr-un loc foarte, foarte vechi.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:1'), 'en-us', 'The Call', 'Puf-Puf was traveling through the depths of the universe when the cosmic silence was broken by a strange roar, like a sigh of the stars. 
The instruments flickered, and the stellar compass traced senseless spirals. 
With his paw on the quartz windshield, he felt more than heard that someone was calling his name from a place very, very old.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:1'), 'hu-hu', 'A hívás', 'Puf-Puf az univerzum mélységében utazott, amikor a kozmikus csendet egy furcsa zúgás törte meg — mint a csillagok sóhaja. A műszerek villogtak, a csillagiránytű értelmetlen spirálokat rajzolt. A mancsával a kvarc szélvédőn érezte (inkább, mint hallotta), hogy valaki a nevén szólítja egy nagyon, nagyon régi helyről.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), '2', 'page', 2, 'Prăbușirea', 'Un fulger de lumină înghiți nava și o azvârli într-un vârtej orbitor. Paratrăsnetele de plasmă se topiră, cârma se blocă, iar carena spintecă norii înainte să muște din pământul uscat. Când tăcerea reveni, aerul mirosea a uscăciune și arșiță. \nPuf-Puf își atinse urechea zgâriată, privi copacii uriași și… nu știa unde se afla, de unde a plecat și de ce. Doar că ajunsese într-un loc străin care îi stârnea curiozitatea.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/2.puf-puf-hurt.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:2'), 'ro-ro', 'Prăbușirea', 'Un fulger de lumină înghiți nava și o azvârli într-un vârtej orbitor. Paratrăsnetele de plasmă se topiră, cârma se blocă, iar carena spintecă norii înainte să muște din pământul uscat. Când tăcerea reveni, aerul mirosea a uscăciune și arșiță. \nPuf-Puf își atinse urechea zgâriată, privi copacii uriași și… nu știa unde se afla, de unde a plecat și de ce. Doar că ajunsese într-un loc străin care îi stârnea curiozitatea.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:2'), 'en-us', 'The Crash', 'A flash of light swallowed the ship and hurled it into a blinding whirl. The plasma lightning rods melted, the rudder jammed, and the hull slit the clouds before biting into the dry earth. When silence returned, the air smelled of dryness and scorching heat. 
Puf-Puf touched his scratched ear, looked at the towering trees and… he didn''t know where he was, where he had come from, or why. He only knew he had arrived in a strange place that stirred his curiosity.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:2'), 'hu-hu', 'A zuhanás', 'Egy villanásnyi fény elnyelte a hajót és egy vakító örvénybe hajította. A plazma villámhárítók megolvadtak, a kormány beragadt, és a hajótest szétvágta a felhőket, mielőtt beleharapott a száraz földbe. Amikor a csend visszatért, a levegő szárazság és perzselő hőség szagát árasztotta. Puf-Puf megérintette a megkarcolt fülét, a hatalmas fákat nézte, és… nem tudta, hol van, honnan jött, és miért. Csak annyit tudott, hogy egy idegen helyre érkezett, ami felkeltette a kíváncsiságát.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), '3', 'page', 3, 'Cristalul SOS', 'La bord, în modulul SOS, un cristal străvechi pulsa ca o inimă adormită. \nPuf-Puf deschise panoul, iar lumina cristalului proiectă în aer o hartă parcă vie.\nRamuri de lumină, poteci care se împleteau la infinit, unele căi erau stinse, altele scânteiau ca semne. \nToate convergeau într-un singur nod: Copacul Luminii.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/3.puf-puf-crystal.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:3'), 'ro-ro', 'Cristalul SOS', 'La bord, în modulul SOS, un cristal străvechi pulsa ca o inimă adormită. \nPuf-Puf deschise panoul, iar lumina cristalului proiectă în aer o hartă parcă vie.\nRamuri de lumină, poteci care se împleteau la infinit, unele căi erau stinse, altele scânteiau ca semne. \nToate convergeau într-un singur nod: Copacul Luminii.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:3'), 'en-us', 'The SOS Crystal', 'On board, in the SOS module, an ancient crystal pulsed like a sleeping heart. 
Puf-Puf opened the panel, and the crystal''s light projected into the air a map that seemed alive.
Branches of light, paths that intertwined infinitely, some routes were dim, others sparkled like signs. 
They all converged to a single node: the Tree of Light.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:3'), 'hu-hu', 'Az SOS kristály', 'A fedélzeten, az SOS modulban egy ősi kristály dobbant, mint egy alvó szív. Puf-Puf kinyitotta a panelt, és fénye egy majdnem élő térképet vetített a levegőbe: fény ágak, ösvények, amelyek a végtelenségig fonódtak. Néhány út kialudt, mások szikráztak, mint jelek. Mindegyik egyetlen csomópontba futott össze: a Fény Fájába.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), '4', 'page', 4, 'Întâlnirea', 'Frunzele Copacului foșneau ca o șoaptă când Puf-Puf îți iese în cale și îți zâmbește parcă amintindu-și ceva, puțin câte puțin. Puf-Puf „-Hei, tu. Da, ție îți vorbesc. Eu sunt Puf-Puf, o pisică vorbitoare de pe planeta Kelo-Ketis... nava mea e puțin șifonată, moralul însă e încă sus. \nCopacul ne-a pus față în față și, sincer, tu pari genul care să știe ce face. Adică ai privirea aia de ai mai reparat două-trei nave spațiale marțea seara. \nMă poți ajuta, te rog, să îmi repar nava și să mă întorc acasă? Cristalul este cheia. Trebuie să îl descifrăm.\nCristalul părea să proiecteze într-o hologramă, aproape vie, un copac luminos, plin de mistere. ', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/puf-puf-home-planet.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:4'), 'ro-ro', 'Întâlnirea', 'Frunzele Copacului foșneau ca o șoaptă când Puf-Puf îți iese în cale și îți zâmbește parcă amintindu-și ceva, puțin câte puțin. Puf-Puf „-Hei, tu. Da, ție îți vorbesc. Eu sunt Puf-Puf, o pisică vorbitoare de pe planeta Kelo-Ketis... nava mea e puțin șifonată, moralul însă e încă sus. \nCopacul ne-a pus față în față și, sincer, tu pari genul care să știe ce face. Adică ai privirea aia de ai mai reparat două-trei nave spațiale marțea seara. \nMă poți ajuta, te rog, să îmi repar nava și să mă întorc acasă? Cristalul este cheia. Trebuie să îl descifrăm.\nCristalul părea să proiecteze într-o hologramă, aproape vie, un copac luminos, plin de mistere. ', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:4'), 'en-us', 'The Meeting', 'The Tree''s leaves rustled like a whisper when Puf-Puf steps into your path and smiles as if remembering something, little by little. Puf-Puf "-Hey, you. Yes, I''m talking to you. I''m Puf-Puf, a talking cat from the planet Kelo-Ketis... my ship is a bit crumpled, but my morale is still high. 
The Tree brought us face to face and, honestly, you seem like the kind who knows what they''re doing. I mean, you have that look like you''ve fixed two or three spaceships on Tuesday evening. 
Can you help me, please, to repair my ship and get back home? The crystal is the key. We have to decipher it.
The crystal seemed to project in a hologram, almost alive, a luminous tree, full of mysteries. ', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:4'), 'hu-hu', 'A találkozás', 'A Fa levelei suttogva susogtak, amikor Puf-Puf eléd lépett és mosolygott, mintha apránként eszébe jutna valami. „Hé, te. Igen, veled beszélek. Én Puf-Puf vagyok, beszélő macska a Kelo-Ketis bolygóról... a hajóm egy kicsit összegyűrve, de a lelkem még mindig fent van. A Fa szemtől szemben állított minket, és őszintén, te úgy nézel ki, mint aki tudja, mit csinál. Vagyis van az a tekinteted, hogy \"már javítottam két-három űrhajót kedden este\". Segíthetsz nekem, kérlek, hogy megjavítsam a hajómat és hazajussak? A kristály a kulcs. Meg kell fejtenünk.\" A kristály mintha egy majdnem élő hologramban vetített volna egy fényes, titkokkal teli fát.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), '5', 'quiz', 5, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/4.TreeOfLight.png', 'Puf-Puf îți cere ajutorul să vorbești cu cristalul SOS care arată harta. Ce ar fi cel mai important lucru de întrebat la început?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'ro-ro', NULL, NULL, 'Puf-Puf îți cere ajutorul să vorbești cu cristalul SOS care arată harta. Ce ar fi cel mai important lucru de întrebat la început?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'en-us', NULL, NULL, 'Puf-Puf asks for your help to speak to the SOS crystal showing the map. What would be the most important thing to ask first?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'hu-hu', NULL, NULL, 'Puf-Puf azt kéri, hogy segíts beszélni a térképet mutató SOS kristállyal. Mi lenne az első, legfontosabb kérdés?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'a', 'Ce drum de pe harta cosmică duce cel mai repede acasă?', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:a:Personality:courage:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:a'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:a:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:a'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:a'), 'ro-ro', 'Ce drum de pe harta cosmică duce cel mai repede acasă?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:a'), 'en-us', 'Which route on the cosmic map gets us home the fastest?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:a'), 'hu-hu', 'Melyik út vezetne a kozmikus térképen a leggyorsabban haza?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'b', 'Oare câte lumi arată harta?', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:b:Discovery:discovery credit:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:b'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:b:Personality:curiosity:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:b'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:b'), 'ro-ro', 'Oare câte lumi arată harta?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:b'), 'en-us', 'How many worlds does the map show?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:b'), 'hu-hu', 'Hány világot mutat a térkép?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'c', 'Îmi arăți calea către Kelo-Ketis?', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:c:Personality:thinking:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:c'), 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:c:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:c'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:c'), 'ro-ro', 'Îmi arăți calea către Kelo-Ketis?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:c'), 'en-us', 'Can you show me the path to Kelo-Ketis?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:c'), 'hu-hu', 'Meg tudod mutatni az utat Kelo-Ketis felé?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'd', 'Ce aș putea să folosesc pentru a repara nava?', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:d:Personality:creativity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:d'), 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:d:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:d'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:d'), 'ro-ro', 'Ce aș putea să folosesc pentru a repara nava?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:d'), 'en-us', 'What could I use to repair the ship?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:d'), 'hu-hu', 'Mit használhatnék a hajó megjavításához?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:5'), 'e', 'Care e cel mai sigur drum spre casă?', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:e:Personality:safety:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:e'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:intro-pufpuf:5:e:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:e'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:e'), 'ro-ro', 'Care e cel mai sigur drum spre casă?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:e'), 'en-us', 'What is the safest route home?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:intro-pufpuf:5:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:intro-pufpuf:5:e'), 'hu-hu', 'Melyik a legbiztonságosabb út hazafelé?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:intro-pufpuf'), '6', 'page', 6, 'Curaj', 'Puf-Puf: - Să mergem mai aproape, o vom rezolva noi. Mă bucur că te-am găsit.\nAmândoi mergeți cu pași mici către copac.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/intro-pufpuf/5.puf-puf-tree-of-light.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:6'), 'ro-ro', 'Curaj', 'Puf-Puf: - Să mergem mai aproape, o vom rezolva noi. Mă bucur că te-am găsit.\nAmândoi mergeți cu pași mici către copac.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:6'), 'en-us', 'Courage', 'Puf-Puf: - Let''s get closer, we''ll figure it out. I''m glad I found you.
Both of you walk with small steps toward the tree.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:intro-pufpuf:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:intro-pufpuf:6'), 'hu-hu', 'Bátorság', 'Puf-Puf: \"Menjünk közelebb, rá fogunk jönni. Örülök, hogy megtaláltalak.\" Mindketten apró léptekkel a fához indultatok.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), 'loi-intro', 'Originea Păcii', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/0.cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:loi-intro|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), 'ro-ro', 'Originea Păcii')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:loi-intro|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), 'en-us', 'The Origin of Peace')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:loi-intro|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), 'hu-hu', 'A Béke Eredete')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for loi-intro
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '1', 'page', 1, 'Două lumi, două tabere', 'În Alchimalia existau două lumi foarte apropiate, dar aflate în război. 
Lunaria, planeta iepurilor, cu câmpii de morcovi și vizuini cat vezi cu ochii si  
Kelo-Ketis, planeta pisicilor, condusă de un împărat razboinic. Granițele erau reci, iar podurile, rupte, desi erau atat de aproape pisicile si iepurii erau dusmani.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/1.two-worlds.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:1'), 'ro-ro', 'Două lumi, două tabere', 'În Alchimalia existau două lumi foarte apropiate, dar aflate în război. 
Lunaria, planeta iepurilor, cu câmpii de morcovi și vizuini cat vezi cu ochii si  
Kelo-Ketis, planeta pisicilor, condusă de un împărat razboinic. Granițele erau reci, iar podurile, rupte, desi erau atat de aproape pisicile si iepurii erau dusmani.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:1'), 'en-us', 'Two Worlds, Two Sides', 'In Alchimalia there were two very close worlds, but at war. 
Lunaria, the rabbits'' planet, with carrot fields and burrows as far as the eye could see, and 
Kelo-Ketis, the cats'' planet, ruled by a warlike emperor. The borders were cold, and the bridges were broken, even though they were so close, cats and rabbits were enemies.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:1'), 'hu-hu', 'Két világ, két tábor', 'Alchimaliában két világ létezett, nagyon közel egymáshoz, de háborúban. Lunaria — a nyuszik bolygója, sárgarépa mezőkkel és üregekkel. Kelo-Ketis — a macskák bolygója, egy császár által vezetve. A határok hidegek voltak, a hidak pedig eltörtek.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '2', 'page', 2, 'Împăratul Pufus Alchimus', 'Pe Kelo-Ketis domnea Împăratul Pufus Alchimus, un războinic cunoscut pentru cuceriri și pentru setea de luptă. Războiul cu iepurii ținea de mult, iar armele vorbeau mai tare decât poveștile. Împăratul spera că într-o bună zi imperiul său se va întinde pe tot tărâmul Alchimaliei.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/2.emperor.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:2'), 'ro-ro', 'Împăratul Pufus Alchimus', 'Pe Kelo-Ketis domnea Împăratul Pufus Alchimus, un războinic cunoscut pentru cuceriri și pentru setea de luptă. Războiul cu iepurii ținea de mult, iar armele vorbeau mai tare decât poveștile. Împăratul spera că într-o bună zi imperiul său se va întinde pe tot tărâmul Alchimaliei.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:2'), 'en-us', 'Emperor Pufus Alchimus', 'On Kelo-Ketis ruled Emperor Pufus Alchimus, a warrior known for conquests and thirst for battle. The war with the rabbits had lasted long, and weapons spoke louder than stories. The emperor hoped that one day his empire would extend across all of Alchimalia.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:2'), 'hu-hu', 'Pufus Alchimus Császár', 'Ketisen Pufus Alchimus Császár uralkodott, egy harcos, aki hódításairól volt ismert. A háború a nyuszikkal már régóta tartott, és a fegyverek hangosabban szóltak, mint a történetek.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '3', 'page', 3, 'Fiul cel curios', 'Împăratului i s-a născut un fiu: Puf-Puf Alchimus, a fost numit. 
Tatăl voia un luptător, dar Puf-Puf iubea matematica, jocurile cu elemente și visa la alchemie. Era mai mult explorator decât soldat. Petrece tot timpul in laboratorul sau unde incerca diverse combinatii intre elemente si citea o gramada de scrieri si retete stravechi.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/3.curious-son.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:3'), 'ro-ro', 'Fiul cel curios', 'Împăratului i s-a născut un fiu: Puf-Puf Alchimus, a fost numit. 
Tatăl voia un luptător, dar Puf-Puf iubea matematica, jocurile cu elemente și visa la alchemie. Era mai mult explorator decât soldat. Petrece tot timpul in laboratorul sau unde incerca diverse combinatii intre elemente si citea o gramada de scrieri si retete stravechi.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:3'), 'en-us', 'The Curious Son', 'A son was born to the emperor: Puf-Puf Alchimus, he was named. 
The father wanted a fighter, but Puf-Puf loved mathematics, games with elements and dreamed of alchemy. He was more of an explorer than a soldier. He spent all his time in his laboratory where he tried various combinations between elements and read a lot of ancient writings and recipes.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:3'), 'hu-hu', 'A kíváncsi fiú', 'A császárnak egy fia született: Puf-Puf Alchimus. Az apa egy harcost akart. De Puf-Puf szerette a matematikát, az elemekkel való játékokat és az alkímiáról álmodott. Inkább felfedező volt, mint katona.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '4', 'page', 4, 'Prieteni peste graniță', 'Deși era război, prietenia își găsea drum. Un iepure negru și o pisică albă se întâlneau în secret și își povesteau lumea, se jucau când pe Lunaria, când pe Ketis. Nu semănau a dușmani. Semănau cu doi copii curioși.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/4.friends.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:4'), 'ro-ro', 'Prieteni peste graniță', 'Deși era război, prietenia își găsea drum. Un iepure negru și o pisică albă se întâlneau în secret și își povesteau lumea, se jucau când pe Lunaria, când pe Ketis. Nu semănau a dușmani. Semănau cu doi copii curioși.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:4'), 'en-us', 'Friends Across the Border', 'Although there was war, friendship found its way. A black rabbit and a white cat met in secret and told each other about their world, playing sometimes on Lunaria, sometimes on Ketis. They didn''t look like enemies. They looked like two curious children.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:4'), 'hu-hu', 'Barátok a határon túl', 'Bár háború volt, a barátság megtalálta az útját. Egy fekete nyuszi és egy fehér macska titokban találkoztak és elmesélték egymásnak a világukat. Nem ellenségeknek tűntek. Kíváncsi gyerekeknek tűntek.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '5', 'page', 5, 'Piatra din Lunaria', 'Pe Ketis, Puf-Puf i-a surprins privind o piatră verde adusă de pe Lunaria, lucind ca un smarald. 
„ - Ține-ne secretul prieteniei și îți dăm piatra”, i-a șoptit iepurele. 
Puf-Puf a promis.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/5.lunaria-stone.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:5'), 'ro-ro', 'Piatra din Lunaria', 'Pe Ketis, Puf-Puf i-a surprins privind o piatră verde adusă de pe Lunaria, lucind ca un smarald. 
„ - Ține-ne secretul prieteniei și îți dăm piatra”, i-a șoptit iepurele. 
Puf-Puf a promis.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:5'), 'en-us', 'The Stone from Lunaria', 'On Ketis, Puf-Puf caught them looking at a green stone brought from Lunaria, gleaming like an emerald. 
"Keep the secret of our friendship and we''ll give you the stone," whispered the rabbit. 
Puf-Puf promised.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:5'), 'hu-hu', 'A Lunaria kő', 'Ketisen Puf-Puf meglepetten látta, ahogy egy zöld követ néz, amit Lunariáról hoztak, smaragdként csillogva.
"- Tartsd titokban a barátságunkat, és odaadjuk a követ", suttogta a nyuszi. Puf-Puf megígérte.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '6', 'page', 6, 'Licoarea verde-aurie', 'Cu piatra, Puf-Puf a făcut un elixir. Când atingea metalul, acesta devenea aur. Bucurie mare! S-au gandit sa si bea, descoperind ca acesta are un gust bun, a împărțit licoarea cu prietenii — mici înghițituri, multe râsete.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/6.elixir.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:6'), 'ro-ro', 'Licoarea verde-aurie', 'Cu piatra, Puf-Puf a făcut un elixir. Când atingea metalul, acesta devenea aur. Bucurie mare! S-au gandit sa si bea, descoperind ca acesta are un gust bun, a împărțit licoarea cu prietenii — mici înghițituri, multe râsete.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:6'), 'en-us', 'The Green-Golden Elixir', 'With the stone, Puf-Puf made an elixir. When it touched metal, it became gold. Great joy! They thought to drink it too, discovering that it had a good taste, he shared the elixir with his friends — small sips, lots of laughter.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:6'), 'hu-hu', 'A zöld-arany folyadék', 'A kővel Puf-Puf egy elixírt készített. Amikor a fémet érintette, arannyá vált. Nagy öröm! Megosztotta a folyadékot a barátaival — kis kortyok, sok nevetés.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:7'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '7', 'page', 7, 'Somn la umbra copacului', 'Obosiți, au adormit la umbra unui copac foarte înalt. Licoarea s-a scurs încet la rădăcini. Piatra verde încălzea pământul cu o lumină blândă.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/7.tree-nap.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:7|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:7'), 'ro-ro', 'Somn la umbra copacului', 'Obosiți, au adormit la umbra unui copac foarte înalt. Licoarea s-a scurs încet la rădăcini. Piatra verde încălzea pământul cu o lumină blândă.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:7|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:7'), 'en-us', 'Sleep Under the Tree', 'Tired, they fell asleep under a very tall tree. The elixir slowly seeped to the roots. The green stone warmed the earth with a gentle light.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:7|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:7'), 'hu-hu', 'Alvás a fa árnyékában', 'Fáradtan elaludtak egy nagyon magas fa árnyékában. A folyadék lassan a gyökerekhez csorgott. A zöld kő enyhe fénnyel melegítette a földet.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:8'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '8', 'page', 8, 'Mâțo-Iepurele', 'Când s-au trezit, lângă ei stătea o ființă ciudată, frumoasă: cu grația unei pisici și blândețea unui iepure. Se uita la ei fără teamă.
- Buna eu sunt Mâțo-Iepurele! Am venit sa aduc Pacea...', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/8.matso-iepurele.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:8|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:8'), 'ro-ro', 'Mâțo-Iepurele', 'Când s-au trezit, lângă ei stătea o ființă ciudată, frumoasă: cu grația unei pisici și blândețea unui iepure. Se uita la ei fără teamă.
- Buna eu sunt Mâțo-Iepurele! Am venit sa aduc Pacea...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:8|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:8'), 'en-us', 'Cat-Rabbit', 'When they woke up, a strange, beautiful being stood beside them: with the grace of a cat and the gentleness of a rabbit. It looked at them without fear.
- Hello, I''m the Cat-Rabbit! I have come to bring Peace...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:8|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:8'), 'hu-hu', 'Macska-Nyuszi', 'Amikor felébredtek, mellettük egy furcsa, szép lény állt: macska grációval és nyuszi szelídséggel. Félelem nélkül nézett rájuk.
- Szia, én Macska-Nyuszi vagyok! A békét hoztam...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), 'q1', 'quiz', 9, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/9.quiz.png', 'Piatra, licoarea și prietenia au chemat Mâțo-Iepurele pentru a aduce pacea. ce faceți mai întâi?

Pastram pacea cu orice pret.
', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:q1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'ro-ro', NULL, NULL, 'Piatra, licoarea și prietenia au chemat Mâțo-Iepurele pentru a aduce pacea. ce faceți mai întâi?

Pastram pacea cu orice pret.
', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:q1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'en-us', NULL, NULL, 'The stone, the elixir and the friendship have summoned the Cat-Rabbit to bring peace. What do you do first?

We keep the peace at any cost.
', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:q1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'hu-hu', NULL, NULL, 'A kő, a folyadék és a barátság Macska-Nyuszit hívta, hogy békét hozzon. mit csinálsz először?

Bármi áron megtartjuk a békét.
', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'a', 'Il vom incuraja', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:a'), 'ro-ro', 'Il vom incuraja')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:a'), 'en-us', 'We will encourage him')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:a'), 'hu-hu', 'Bátorítjuk')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'b', 'Vom pune elixirul la fiecare copac.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:b'), 'ro-ro', 'Vom pune elixirul la fiecare copac.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:b'), 'en-us', 'We will pour the elixir at each tree.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:b'), 'hu-hu', 'Minden fára elixírt teszünk.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'c', 'Il protejam pe Mâțo-Iepure.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:c'), 'ro-ro', 'Il protejam pe Mâțo-Iepure.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:c'), 'en-us', 'We will protect the Cat-Rabbit.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:c'), 'hu-hu', 'Megvédjük Macska-Nyuszit.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'd', 'Plantăm un copac al păcii în ambele lumi.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:d'), 'ro-ro', 'Plantăm un copac al păcii în ambele lumi.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:d'), 'en-us', 'We plant a tree of peace in both worlds.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:d'), 'hu-hu', 'Mindkét világban béke fát ültetünk.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:q1'), 'e', 'Pastram pacea cu orice pret.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:e'), 'ro-ro', 'Pastram pacea cu orice pret.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:e'), 'en-us', 'We keep the peace at any cost.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:loi-intro:q1:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:loi-intro:q1:e'), 'hu-hu', 'Bármi áron megtartjuk a békét.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:9'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '9', 'page', 10, 'Mesajul păcii', '„Eu port pacea dintre lumi”, a spus Mâțo-Iepurele. „Din prietenie, piatră și licoare s-a născut un pod, a spus Puf-Puf.
Războaiele se sting când înveți să amesteci bine lucrurile.” 
Puf-Puf a înțeles că alchimia nu e doar despre metale, ci și despre inimi.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/10.message.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:9|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:9'), 'ro-ro', 'Mesajul păcii', '„Eu port pacea dintre lumi”, a spus Mâțo-Iepurele. „Din prietenie, piatră și licoare s-a născut un pod, a spus Puf-Puf.
Războaiele se sting când înveți să amesteci bine lucrurile.” 
Puf-Puf a înțeles că alchimia nu e doar despre metale, ci și despre inimi.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:9|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:9'), 'en-us', 'Message of Peace', '"I carry peace between worlds," said the Cat-Rabbit. "From friendship, stone and elixir a bridge was born," said Puf-Puf.
Wars fade when you learn to mix things well." 
Puf-Puf understood that alchemy is not just about metals, but also about hearts.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:9|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:9'), 'hu-hu', 'A béke üzenete', '"Én hordom a békét a világok között", mondta Macska-Nyuszi. "Barátságból, kőből és folyadékból született egy híd", mondta Puf-Puf.
A háborúk kialszanak, amikor megtanulod jól keverni a dolgokat.
Puf-Puf megértette, hogy az alkímia nem csak a fémekről szól, hanem a szívekről is.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:10'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '10', 'page', 11, 'Hotărârea lui Puf-Puf', 'În acea zi, Puf-Puf era bucuruos ca alesese sa fie un alchimist. Va căuta căi între lumi, nu războaie. Iar Mâțo-Iepurele avea să devină, cândva, semnul că pacea poate fi creată, nu doar găsită.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/11.choice.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:10|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:10'), 'ro-ro', 'Hotărârea lui Puf-Puf', 'În acea zi, Puf-Puf era bucuruos ca alesese sa fie un alchimist. Va căuta căi între lumi, nu războaie. Iar Mâțo-Iepurele avea să devină, cândva, semnul că pacea poate fi creată, nu doar găsită.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:10|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:10'), 'en-us', 'Puf-Puf''s Decision', 'That day, Puf-Puf was happy that he had chosen to be an alchemist. He would seek ways between worlds, not wars. And the Cat-Rabbit would, someday, become the sign that peace can be created, not just found.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:10|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:10'), 'hu-hu', 'Puf-Puf döntése', 'Aznap Puf-Puf a saját útját választotta: felfedező és alkimista. Világok közötti utakat fog keresni, nem háborúkat. És Macska-Nyuszi egyszer a jelképévé válik, hogy a béke nem csak megtalálható, hanem létrehozható is.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:12'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '12', 'page', 12, 'Vraja', 'Puf-Puf stătea chiar la granița dintre planete când iepurele păși înapoi pe Lunaria, vrând să-l conducă acasă în secret. Când totul era liniștit, o vrajă misterioasă făcu ca Lunaria să se desprindă de Kelo-Ketis și să dispară pentru totdeauna. Mâțo-Iepurele dispăruse și el.

Puf-Puf: „Trebuie să aflu ce s-a întâmplat!” Și, cu pași grăbiți, merse din nou spre copac.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/12.the-spell.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:12|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:12'), 'ro-ro', 'Vraja', 'Puf-Puf stătea chiar la granița dintre planete când iepurele păși înapoi pe Lunaria, vrând să-l conducă acasă în secret. Când totul era liniștit, o vrajă misterioasă făcu ca Lunaria să se desprindă de Kelo-Ketis și să dispară pentru totdeauna. Mâțo-Iepurele dispăruse și el.

Puf-Puf: „Trebuie să aflu ce s-a întâmplat!” Și, cu pași grăbiți, merse din nou spre copac.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:12|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:12'), 'en-us', 'The Spell', 'Puf-Puf was standing right at the border between planets when the rabbit stepped back onto Lunaria, wanting to lead him home in secret. When everything was quiet, a mysterious spell caused Lunaria to break away from Kelo-Ketis and disappear forever. The Cat-Rabbit had disappeared too.

Puf-Puf: "I must find out what happened!" And, with hurried steps, he walked again toward the tree.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:12|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:12'), 'hu-hu', 'A varázslat', 'Amikor minden csendes volt, egy titokzatos varázslat miatt Lunaria elszakadt Kelo-Ketistől és örökre eltűnt.
Puf-Puf: "-Meg kell tudnom, mi történt, elindulok egy utazásra!"', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:13'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '13', 'page', 13, '...', 'Ruptura dintre lumi era plina de mister, Puf-Puf ajunse repede in dreptul copacului care se transforamsera incredibil in ceva plin de lumina si mister.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/13.tobecontinued.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:13|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:13'), 'ro-ro', '...', 'Ruptura dintre lumi era plina de mister, Puf-Puf ajunse repede in dreptul copacului care se transforamsera incredibil in ceva plin de lumina si mister.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:13|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:13'), 'en-us', '...', 'The rupture between worlds was full of mystery, Puf-Puf quickly reached the tree which had transformed incredibly into something full of light and mystery.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:13|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:13'), 'hu-hu', '...', 'Folytatjuk...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:14'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '14', 'page', 14, 'Copacul luminii', 'În fața lui Puf-Puf se afla ceva atât de frumos și, totodată, de nerecunoscut. Marele copac devenise plin de lumină, iar din loc în loc se auzeau șoapte.
„Trebuie să cauți pacea... cristalul este cheia...”, se auzi de undeva, din rădăcina copacului.
Puf-Puf săpă la rădăcina copacului și găsi un cristal care părea să comunice cu el.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/14.tol.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:14|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:14'), 'ro-ro', 'Copacul luminii', 'În fața lui Puf-Puf se afla ceva atât de frumos și, totodată, de nerecunoscut. Marele copac devenise plin de lumină, iar din loc în loc se auzeau șoapte.
„Trebuie să cauți pacea... cristalul este cheia...”, se auzi de undeva, din rădăcina copacului.
Puf-Puf săpă la rădăcina copacului și găsi un cristal care părea să comunice cu el.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:14|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:14'), 'en-us', 'Tree of Light', 'In front of Puf-Puf stood something so beautiful and yet unrecognizable. The great tree had become full of light, and whispers could be heard here and there.
"You must seek peace... the crystal is the key...", came a voice from somewhere, from the tree''s roots.
Puf-Puf dug at the tree''s roots and found a crystal that seemed to communicate with him.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:14|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:14'), 'hu-hu', 'A Fény Fája', 'Puf-Puf előtt valami gyönyörű és mégis felismerhetetlen állt. A nagy fa fénnyel telt meg, és innen-onnan suttogások hallatszottak. "A békét kell keresned... a kristály a kulcs..." — szólt egy hang a gyökerek felől. Puf-Puf a gyökereknél ásott, és talált egy kristályt, amely mintha beszélt volna hozzá.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:15'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '15', 'page', 15, 'Plecarea', 'Îi spuse tatălui său că trebuie să plece, să exploreze; nu putea să-i spună Împăratului că vrea să aducă tocmai pacea — după atâta muncă de cucerire, acesta era ultimul lucru pe care marele Pufus Alchimus ar fi vrut să-l audă. Se urcă într-una dintre navele spațiale ale Imperiului și plecă.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/loi-intro/15.leavingkk.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:15|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:15'), 'ro-ro', 'Plecarea', 'Îi spuse tatălui său că trebuie să plece, să exploreze; nu putea să-i spună Împăratului că vrea să aducă tocmai pacea — după atâta muncă de cucerire, acesta era ultimul lucru pe care marele Pufus Alchimus ar fi vrut să-l audă. Se urcă într-una dintre navele spațiale ale Imperiului și plecă.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:15|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:15'), 'en-us', 'Departure', 'He told his father that he had to leave, to explore; he couldn''t tell the Emperor that he wanted to bring peace — after so much work of conquest, that was the last thing the great Pufus Alchimus would have wanted to hear. He boarded one of the Empire''s spaceships and left.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:15|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:15'), 'hu-hu', 'Indulás', 'Elmondta az apjának, hogy el kell mennie, felfedezni. Nem mondhatta el a császárnak, hogy békét akar hozni — ennyi hódítás után ez volt az utolsó, amit a nagy Pufus Alchimus hallani akart. Felszállt a birodalom egyik űrhajójára és elindult...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:16'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:loi-intro'), '16', 'video', 16, 'Crystal', NULL, NULL, NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:16|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:16'), 'ro-ro', 'Crystal', NULL, NULL, NULL, 'video/ro-ro/tol/stories/seed@alchimalia.com/loi-intro/tol-cristal.mp4')
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:16|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:16'), 'en-us', 'Crystal', NULL, NULL, NULL, 'video/en-us/tol/stories/seed@alchimalia.com/loi-intro/tol-cristal.mp4')
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:loi-intro:16|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:loi-intro:16'), 'hu-hu', 'Crystal', NULL, NULL, NULL, 'video/hu-hu/tol/stories/seed@alchimalia.com/loi-intro/tol-cristal.mp4')
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'lunaria-s1', 'Linkaro', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:lunaria-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'ro-ro', 'Linkaro')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:lunaria-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'en-us', 'Linkaro')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:lunaria-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'hu-hu', 'Linkaro')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for lunaria-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), '1', 'page', 1, 'Portalul și Consiliul Iepurilor', '— Ține bine, zise Puf-Puf. Așez lupa peste cristal. Uite pana păsării portocalii, trofeul de pe Terra. E cheia.
— Și acum? întrebă iepurele negru.
— Lumina se adună… Gata. S-a deschis portalul.

Intrați: tu, Puf-Puf și iepurele negru. Dincolo, o piață cu steaguri în formă de semilună.

— Sunteți călători din Copacul Luminii? întrebă un consilier.
— Puf-Puf, la raport, spuse pisoiul. Am venit să ajutăm.
— Atunci ascultați, zise consilierul. O boală ne slăbește iepurii. Avem nevoie de un leac.

— Ce simptome sunt? întrebi tu.
— Obosesc repede, aud mai prost și își pierd curajul, răspunse el.

— Știu un loc, zise iepurele negru. Pe Muntele Negru e o mănăstire veche. Călugării de acolo vindecă astfel de boli.
— Drumul e cu ceață și stânci, dar merită încercat, adăugă alt consilier. Luați acest sigiliu; vă deschide poarta mănăstirii.

— Îl luăm, spui tu. Plecăm imediat.
— Eu țin direcția după cristal, zise Puf-Puf.
— Iar eu știu potecile, zise iepurele negru.

Strângeți sigiliul, treceți pe sub steagurile în semilună și porniți spre Muntele Negru.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/1.portal-council.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:1'), 'ro-ro', 'Portalul și Consiliul Iepurilor', '— Ține bine, zise Puf-Puf. Așez lupa peste cristal. Uite pana păsării portocalii, trofeul de pe Terra. E cheia.
— Și acum? întrebă iepurele negru.
— Lumina se adună… Gata. S-a deschis portalul.

Intrați: tu, Puf-Puf și iepurele negru. Dincolo, o piață cu steaguri în formă de semilună.

— Sunteți călători din Copacul Luminii? întrebă un consilier.
— Puf-Puf, la raport, spuse pisoiul. Am venit să ajutăm.
— Atunci ascultați, zise consilierul. O boală ne slăbește iepurii. Avem nevoie de un leac.

— Ce simptome sunt? întrebi tu.
— Obosesc repede, aud mai prost și își pierd curajul, răspunse el.

— Știu un loc, zise iepurele negru. Pe Muntele Negru e o mănăstire veche. Călugării de acolo vindecă astfel de boli.
— Drumul e cu ceață și stânci, dar merită încercat, adăugă alt consilier. Luați acest sigiliu; vă deschide poarta mănăstirii.

— Îl luăm, spui tu. Plecăm imediat.
— Eu țin direcția după cristal, zise Puf-Puf.
— Iar eu știu potecile, zise iepurele negru.

Strângeți sigiliul, treceți pe sub steagurile în semilună și porniți spre Muntele Negru.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:1'), 'en-us', 'The Portal and the Rabbits'' Council', '— Hold tight, said Puf-Puf. I''m placing the magnifier over the crystal. Look, the feather of the orange bird, the trophy from Terra. It''s the key.
— And now? asked the black rabbit.
— The light gathers... Ready. The portal has opened.

You enter: you, Puf-Puf and the black rabbit. Beyond, a square with crescent-shaped flags.

— Are you travelers from the Tree of Light? asked a councilor.
— Puf-Puf, reporting, said the kitten. We''ve come to help.
— Then listen, said the councilor. An illness weakens our rabbits. We need a cure.

— What are the symptoms? you ask.
— They tire quickly, hear poorly and lose their courage, he replied.

— I know a place, said the black rabbit. On Black Mountain there''s an old monastery. The monks there heal such illnesses.
— The road is foggy and rocky, but it''s worth trying, added another councilor. Take this seal; it opens the monastery gate.

— We''ll take it, you say. We''re leaving immediately.
— I''ll keep direction by the crystal, said Puf-Puf.
— And I know the paths, said the black rabbit.

You take the seal, pass under the crescent flags and set off toward Black Mountain.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:1'), 'hu-hu', 'A portál és a Nyuszik Tanácsa', '— Kapaszkodj jól, mondta Puf-Puf. Ráhelyezem a nagyítót a kristályra. Nézd, a narancssárga madár tollát, a trófeát a Teráról. Ez a kulcs.
— És most? kérdezte a fekete nyuszi.
— A fény összegyűlik... Kész. Megnyílt a portál.

Beléptek: te, Puf-Puf és a fekete nyuszi. Túl, egy tér félhold alakú zászlókkal.

— Utazók vagytok a Fény Fájából? kérdezte egy tanácsos.
— Puf-Puf, jelentkezem, mondta a kiscica. Azért jöttünk, hogy segítsünk.
— Akkor hallgassatok, mondta a tanácsos. Egy betegség gyengíti a nyuszinkat. Szükségünk van egy gyógyírra.

— Milyenek a tünetek? kérdezed te.
— Gyorsan elfáradnak, rosszabbul hallanak és elvesztik a bátorságukat, válaszolta.

— Tudok egy helyet, mondta a fekete nyuszi. A Fekete Hegyen van egy régi kolostor. A szerzetesek ott gyógyítanak ilyen betegségeket.
— Az út ködös és sziklás, de érdemes megpróbálni, tette hozzá egy másik tanácsos. Vegyétek el ezt a pecsétet; ez nyitja a kolostor kapuját.

— Elvesszük, mondod te. Azonnal indulunk.
— Én a kristály szerint tartom az irányt, mondta Puf-Puf.
— Én pedig ismerem az ösvényeket, mondta a fekete nyuszi.

Elveszitek a pecsétet, áthaladtok a félhold zászlók alatt és elindultok a Fekete Hegy felé.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'p2', 'page', 2, 'Drumul spre Muntele Negru', 'Poteca urcă greu: pietrele fug de sub talpă, iar vântul ba vine din față, ba te împinge din spate. Ceața se adună pe colțurile stâncilor și parcă mușcă din marginea potecii. Puf-Puf merge lipit de stâncă, cu cristalul la piept, iar iepurele negru își testează sprijinul la fiecare pas.

— Ține-te de mine, zice Puf-Puf. Aici alunecă.
— Știu ocolul, răspunde iepurele. Încă zece pași și scăpăm de panta asta.
— Respiră rar și apasă talpa întreagă, adaugi tu.

După panta abruptă, vântul se liniștește ca la un semn. Ceața se rupe în fâșii și apare mănăstirea: ziduri albe, linii simple, curate, ca desenate cu rigla. Deasupra porții, stegulețe cu semilună foșnesc ușor, fără să facă zgomot.

— Am ajuns, șoptește iepurele, cu o ușurare vizibilă.
— Frumos loc, spune Puf-Puf. Parcă te cheamă să intri.
— Hai, bate tu la poartă, zici. Eu țin sigiliul pregătit.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/2.white-mountain-road.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p2'), 'ro-ro', 'Drumul spre Muntele Negru', 'Poteca urcă greu: pietrele fug de sub talpă, iar vântul ba vine din față, ba te împinge din spate. Ceața se adună pe colțurile stâncilor și parcă mușcă din marginea potecii. Puf-Puf merge lipit de stâncă, cu cristalul la piept, iar iepurele negru își testează sprijinul la fiecare pas.

— Ține-te de mine, zice Puf-Puf. Aici alunecă.
— Știu ocolul, răspunde iepurele. Încă zece pași și scăpăm de panta asta.
— Respiră rar și apasă talpa întreagă, adaugi tu.

După panta abruptă, vântul se liniștește ca la un semn. Ceața se rupe în fâșii și apare mănăstirea: ziduri albe, linii simple, curate, ca desenate cu rigla. Deasupra porții, stegulețe cu semilună foșnesc ușor, fără să facă zgomot.

— Am ajuns, șoptește iepurele, cu o ușurare vizibilă.
— Frumos loc, spune Puf-Puf. Parcă te cheamă să intri.
— Hai, bate tu la poartă, zici. Eu țin sigiliul pregătit.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p2'), 'en-us', 'Road to Black Mountain', 'The path climbs hard: stones slip from under your feet, and the wind comes from the front, then pushes you from behind. Fog gathers on the corners of the rocks and seems to bite at the edge of the path. Puf-Puf walks close to the rock, with the crystal at his chest, while the black rabbit tests his footing with each step.

— Hold onto me, says Puf-Puf. It''s slippery here.
— I know a way around, replies the rabbit. Ten more steps and we''ll escape this slope.
— Breathe slowly and press your whole foot down, you add.

After the steep slope, the wind calms as if at a signal. The fog breaks into strips and the monastery appears: white walls, simple, clean lines, as if drawn with a ruler. Above the gate, small crescent flags rustle gently, making no noise.

— We''ve arrived, whispers the rabbit, with visible relief.
— Beautiful place, says Puf-Puf. It seems to call you to enter.
— Come on, you knock on the gate, you say. I''ll keep the seal ready.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p2'), 'hu-hu', 'Az út a Fekete Hegy felé', 'Az ösvény nehezen emelkedik: a kövek csúsznak a talp alól, a szél előbb elöl jön, aztán hátulról tol. A köd a sziklák sarkain gyűlik össze és mintha harapna az ösvény széléről. Puf-Puf a sziklához simulva jár, a kristályt a mellkasán, míg a fekete nyuszi minden lépésnél teszteli a támaszt.

— Kapaszkodj belém, mondja Puf-Puf. Itt csúszik.
— Ismerek egy kerülőt, válaszolja a nyuszi. Még tíz lépés és kiszabadulunk ebből a lejtőből.
— Ritkán lélegezz és nyomd le a teljes talpat, teszed hozzá te.

A meredek lejtő után a szél mintha egy jelre csendesedne. A köd csíkokra törik és megjelenik a kolostor: fehér falak, egyszerű, tiszta vonalak, mintha vonalzóval rajzolták volna. A kapu felett félhold zászlócskák finoman susognak, zajt nem csapnak.

— Megérkeztünk, suttogja a nyuszi, látható megkönnyebbüléssel.
— Szép hely, mondja Puf-Puf. Mintha hívna, hogy belépj.
— Gyerünk, kopogj a kapun, mondod te. Én a pecsétet készenlétben tartom.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), '3', 'page', 3, 'Sala Liniștii', 'În curte, călugări pisici și iepuri se antrenează în tăcere. Le spuneți despre boala din oraș. Se privesc mirați: n-au auzit.\n„Trebuie să vorbiți cu marele maestru”, spune un călugăr și vă conduce într-o încăpere luminoasă. O siluetă stă cu spatele, în meditație.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/3.silent-hall.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:3'), 'ro-ro', 'Sala Liniștii', 'În curte, călugări pisici și iepuri se antrenează în tăcere. Le spuneți despre boala din oraș. Se privesc mirați: n-au auzit.\n„Trebuie să vorbiți cu marele maestru”, spune un călugăr și vă conduce într-o încăpere luminoasă. O siluetă stă cu spatele, în meditație.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:3'), 'en-us', 'The Hall of Silence', 'In the courtyard, cat and rabbit monks train in silence. You tell them about the illness in the city. They look at each other in surprise: they haven''t heard.
"You must speak with the grand master," says a monk and leads you into a bright room. A silhouette sits with its back turned, meditating.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:3'), 'hu-hu', 'A Csend Csarnoka', 'Az udvaron macska és nyuszi szerzetesek csendben gyakorolnak. Elmesélitek nekik a város betegségét. Csodálkozva néznek egymásra: nem hallottak róla.
"A nagy mesterrel kell beszélnetek", mondja egy szerzetes és egy világos helyiségbe vezet. Egy sziluett hátat fordítva meditál.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), '4', 'page', 4, 'Linkaro, Mâțo-Iepurele', '— E… pisică? șoptește Puf-Puf.
— Sau iepure? întrebi tu.
— Linkaro, zâmbește ea. Sunt un Mâțo-Iepure.

— Știu specia, dă din cap Puf-Puf. Am întâlnit unul pe vremea războaielor dintre pisici și iepuri.
— Războaiele s-au încheiat, spune calm Linkaro. Eu am semnat ultimul armistițiu. Spuneți problema.

Îi descrieți boala din oraș.

— Vă trebuie Pietrele Vieții, cu esență de vindecare, spune Linkaro. Le găsiți sub o streașină de stâncă — o cornișă expusă. Culegeți-le cu grijă, doar pe cele care lucesc ușor.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/4.linkaro.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:4'), 'ro-ro', 'Linkaro, Mâțo-Iepurele', '— E… pisică? șoptește Puf-Puf.
— Sau iepure? întrebi tu.
— Linkaro, zâmbește ea. Sunt un Mâțo-Iepure.

— Știu specia, dă din cap Puf-Puf. Am întâlnit unul pe vremea războaielor dintre pisici și iepuri.
— Războaiele s-au încheiat, spune calm Linkaro. Eu am semnat ultimul armistițiu. Spuneți problema.

Îi descrieți boala din oraș.

— Vă trebuie Pietrele Vieții, cu esență de vindecare, spune Linkaro. Le găsiți sub o streașină de stâncă — o cornișă expusă. Culegeți-le cu grijă, doar pe cele care lucesc ușor.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:4'), 'en-us', 'Linkaro, the Cat-Rabbit', '— Is it... a cat? whispers Puf-Puf.
— Or a rabbit? you ask.
— Linkaro, she smiles. I''m a Cat-Rabbit.

— I know the species, nods Puf-Puf. I met one during the wars between cats and rabbits.
— The wars are over, says Linkaro calmly. I signed the last armistice. Tell me the problem.

You describe the illness in the city.

— You need the Stones of Life, with healing essence, says Linkaro. You''ll find them under a rock overhang — an exposed ledge. Gather them carefully, only those that glow softly.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:4'), 'hu-hu', 'Linkaro, Macska-Nyuszi', '— Ez... macska? suttogja Puf-Puf.
— Vagy nyuszi? kérdezed te.
— Linkaro, mosolyog. Macska-Nyuszi vagyok.

— Ismerem a fajtát, bólint Puf-Puf. Találkoztam egyet a macskák és nyuszik közötti háborúk idején.
— A háborúk véget értek, mondja nyugodtan Linkaro. Én írtam alá az utolsó fegyverszünetet. Mondjátok el a problémát.

Elmagyarázzátok a város betegségét.

— Az Élet Köveire van szükségetek, gyógyító esszenciával, mondja Linkaro. Egy szikla párkány alatt találjátok — egy kiugró szikla. Óvatosan szedjétek le, csak azokat, amelyek finoman világítanak.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), '5', 'page', 5, 'Pietrele Vieții', 'Pe o creastă îngustă, sub cornișă, licăresc pietre verzi, ca niște inimioare. Curentul rece le slăbește pulsul la orice mișcare bruscă.

— Aici contează cum te apropii, șoptește Puf-Puf. Pietrele de pe Lunaria se sting dacă le sperii.
— Mergem pe rând, cu pași rari, zici tu.
— Țin vântul în urechi și vă dau semn când tace, adaugă iepurele negru.

Când rafala se oprește, vă aplecați încet.
— Ține punguța deschisă, spune Puf-Puf. Nu le atinge. Lasă-le să cadă singure.
— Acum, șoptești. Ușor.

Două pietre se desprind blând și cad în pânză. A treia licărește mai tare, apoi se liniștește.
— Trei ajung, zice Puf-Puf. Restul rămân muntelui.
— Să nu le slăbim suflul, încuviințează iepurele. Plecăm.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/5.life-stones.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:5'), 'ro-ro', 'Pietrele Vieții', 'Pe o creastă îngustă, sub cornișă, licăresc pietre verzi, ca niște inimioare. Curentul rece le slăbește pulsul la orice mișcare bruscă.

— Aici contează cum te apropii, șoptește Puf-Puf. Pietrele de pe Lunaria se sting dacă le sperii.
— Mergem pe rând, cu pași rari, zici tu.
— Țin vântul în urechi și vă dau semn când tace, adaugă iepurele negru.

Când rafala se oprește, vă aplecați încet.
— Ține punguța deschisă, spune Puf-Puf. Nu le atinge. Lasă-le să cadă singure.
— Acum, șoptești. Ușor.

Două pietre se desprind blând și cad în pânză. A treia licărește mai tare, apoi se liniștește.
— Trei ajung, zice Puf-Puf. Restul rămân muntelui.
— Să nu le slăbim suflul, încuviințează iepurele. Plecăm.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:5'), 'en-us', 'The Stones of Life', 'On a narrow ridge, under the ledge, green stones glimmer like little hearts. The cold current weakens their pulse at any sudden movement.

— Here it matters how you approach, whispers Puf-Puf. The stones on Lunaria go out if you frighten them.
— We go one by one, with rare steps, you say.
— I''ll keep the wind in my ears and give you a signal when it''s quiet, adds the black rabbit.

When the gust stops, you bend down slowly.
— Keep the pouch open, says Puf-Puf. Don''t touch them. Let them fall by themselves.
— Now, you whisper. Gently.

Two stones detach softly and fall into the cloth. The third glimmers brighter, then calms down.
— Three are enough, says Puf-Puf. The rest stay with the mountain.
— Let''s not weaken their breath, agrees the rabbit. We''re leaving.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:5'), 'hu-hu', 'Az Élet Kövei', 'Egy keskeny gerincen, a párkány alatt zöld kövek csillognak, mint kis szívek. A hideg légáramlat gyengíti a pulzusukat bármilyen hirtelen mozgásnál.

— Itt számít, hogyan közeledsz, suttogja Puf-Puf. A Lunaria kövei kialszanak, ha megijeszted őket.
— Egyenként megyünk, ritka lépésekkel, mondod te.
— A széllel a fülemben tartom és jelzek, amikor csendes, teszi hozzá a fekete nyuszi.

Amikor a szélroham megáll, lassan lehajoltok.
— Tartsd nyitva a zsákot, mondja Puf-Puf. Ne érintsd meg őket. Hagyd, hogy maguktól essenek le.
— Most, suttogod. Finoman.

Két kő finoman levánd és a vászonba esik. A harmadik erősebben csillog, aztán megnyugszik.
— Három elég, mondja Puf-Puf. A többi a heggyel marad.
— Ne gyengítsük a leheletüket, egyetért a nyuszi. Indulunk.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), '6', 'quiz', 6, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/6.quiz.png', 'Pietrele verzi pulsează sub cornișă. Nu cer forță, ci felul în care te apropii. Cum le culegi fără să le stingi lumina?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'ro-ro', NULL, NULL, 'Pietrele verzi pulsează sub cornișă. Nu cer forță, ci felul în care te apropii. Cum le culegi fără să le stingi lumina?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'en-us', NULL, NULL, 'The green stones pulse under the ledge. They don''t ask for force, but for how you approach. How do you gather them without extinguishing their light?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'hu-hu', NULL, NULL, 'A zöld kövek pulzálnak a párkány alatt. Nem erőt kérnek, hanem a megközelítés módját. Hogyan szeded le őket anélkül, hogy kioltnád a fényüket?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'a', 'O mișcare hotărâtă.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:a:Personality:courage:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:a'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:a:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:a'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:a'), 'ro-ro', 'O mișcare hotărâtă.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:a'), 'en-us', 'A decisive move.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:a'), 'hu-hu', 'Határozott mozgás.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'b', 'Ascult pulsul, apoi ating.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:b:Personality:curiosity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:b'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:b:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:b'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:b'), 'ro-ro', 'Ascult pulsul, apoi ating.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:b'), 'en-us', 'Listen to the pulse, then touch.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:b'), 'hu-hu', 'Hallgatom a pulzust, aztán érintem.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'c', 'Sincronizez ca un ceasornicar.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:c:Personality:thinking:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:c'), 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:c:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:c'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:c'), 'ro-ro', 'Sincronizez ca un ceasornicar.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:c'), 'en-us', 'Synchronize like a watchmaker.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:c'), 'hu-hu', 'Órási módon szinkronizálok.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'd', 'Buclă moale, fără atingere.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:d:Personality:creativity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:d'), 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:d:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:d'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:d'), 'ro-ro', 'Buclă moale, fără atingere.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:d'), 'en-us', 'Soft loop, no touch.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:d'), 'hu-hu', 'Puha hurok, érintés nélkül.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:6'), 'e', 'Pași calmi, ritm constant.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:e:Discovery:discovery credit:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:e'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s1:6:e:Personality:safety:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:e'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:e'), 'ro-ro', 'Pași calmi, ritm constant.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:e'), 'en-us', 'Calm steps, steady rhythm.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s1:6:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s1:6:e'), 'hu-hu', 'Nyugodt lépések, állandó ritmus.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'p6', 'page', 7, 'Rețeta Linkaro', 'Înapoi în sala liniștită, Linkaro vorbește rar, ca să notați bine:

— Apă la foc mic, să fiarbă blând.
— Mult morcov, tăiat foarte fin. Ierburi locale — o mână mică ajunge.
— Pietrele Vieții nu se pun în oală. Le așezați sub vas, ca un cuib. Ele dau esența prin căldură.
— Când aburul capătă o sclipire verde, opriți focul. E gata.

— Simplu și curat, zice Puf-Puf.
— Clar, adaugi tu. Începem chiar acum.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/7.recipe.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p6'), 'ro-ro', 'Rețeta Linkaro', 'Înapoi în sala liniștită, Linkaro vorbește rar, ca să notați bine:

— Apă la foc mic, să fiarbă blând.
— Mult morcov, tăiat foarte fin. Ierburi locale — o mână mică ajunge.
— Pietrele Vieții nu se pun în oală. Le așezați sub vas, ca un cuib. Ele dau esența prin căldură.
— Când aburul capătă o sclipire verde, opriți focul. E gata.

— Simplu și curat, zice Puf-Puf.
— Clar, adaugi tu. Începem chiar acum.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p6'), 'en-us', 'Linkaro''s Recipe', 'Back in the quiet hall, Linkaro speaks rarely, so you can note well:

— Water on low heat, to boil gently.
— Lots of carrot, cut very fine. Local herbs — a small handful is enough.
— The Stones of Life don''t go in the pot. You place them under the vessel, like a nest. They give the essence through heat.
— When the steam takes on a green shimmer, turn off the heat. It''s ready.

— Simple and clean, says Puf-Puf.
— Clear, you add. We start right now.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p6'), 'hu-hu', 'Linkaro receptje', 'Vissza a csendes csarnokba, Linkaro ritkán beszél, hogy jól jegyezzétek:

— Vízet kis lángon, hogy finoman forraljon.
— Sok sárgarépa, nagyon finomra vágva. Helyi füvek — egy kis marék elég.
— Az Élet Kövei nem kerülnek a fazékba. A fazék alá helyezitek, mint egy fészket. A hőn keresztül adják az esszenciát.
— Amikor a gőz zöld csillogást kap, kapcsoljátok le a tüzet. Kész.

— Egyszerű és tiszta, mondja Puf-Puf.
— Világos, teszed hozzá te. Most kezdjük.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p7'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'p7', 'page', 8, 'Supa în oraș', 'Vă întoarceți la Consiliu. În piață, puneți la mijloc vasul mare; Puf-Puf așază cu grijă pietrele verzi sub el, ca într-un cuib. Apa începe să freamăte la foc mic, morcovul tăiat fin și ierburile locale se lasă ușor în oală.

— Ținem regula Linkaro, zice Puf-Puf. Pietrele rămân sub vas.
— Așteptăm aburul verde, adaugi tu. Încet, fără grabă.

Când aburul prinde o nuanță smarald, turnați în căni mici.
— Pe rând, îi îndeamnă iepurele negru. Un gât, apoi respirați.

Iepurii bolnavi beau câte o înghițitură. Urechile li se ridică, pieptul se liniștește, ochii capătă luciu. În piață se aude din nou foșnetul pașilor, râsete scurte, vorbă domoală. Steagurile în semilună fâlfâie ușor — murmurul străzii revine.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/8.soup-city.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p7|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p7'), 'ro-ro', 'Supa în oraș', 'Vă întoarceți la Consiliu. În piață, puneți la mijloc vasul mare; Puf-Puf așază cu grijă pietrele verzi sub el, ca într-un cuib. Apa începe să freamăte la foc mic, morcovul tăiat fin și ierburile locale se lasă ușor în oală.

— Ținem regula Linkaro, zice Puf-Puf. Pietrele rămân sub vas.
— Așteptăm aburul verde, adaugi tu. Încet, fără grabă.

Când aburul prinde o nuanță smarald, turnați în căni mici.
— Pe rând, îi îndeamnă iepurele negru. Un gât, apoi respirați.

Iepurii bolnavi beau câte o înghițitură. Urechile li se ridică, pieptul se liniștește, ochii capătă luciu. În piață se aude din nou foșnetul pașilor, râsete scurte, vorbă domoală. Steagurile în semilună fâlfâie ușor — murmurul străzii revine.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p7|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p7'), 'en-us', 'Soup in the City', 'You return to the Council. In the square, you place the big vessel in the middle; Puf-Puf carefully places the green stones under it, like in a nest. The water begins to simmer on low heat, the finely cut carrot and local herbs settle gently into the pot.

— We follow Linkaro''s rule, says Puf-Puf. The stones stay under the vessel.
— We wait for the green steam, you add. Slowly, without hurry.

When the steam takes on an emerald hue, you pour into small cups.
— One by one, urges the black rabbit. One sip, then breathe.

The sick rabbits drink one sip each. Their ears rise, their chests calm down, their eyes gain luster. In the square you hear again the rustle of steps, short laughs, gentle talk. The crescent flags flutter lightly — the street''s murmur returns.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p7|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p7'), 'hu-hu', 'Leves a városban', 'Visszatértek a Tanácshoz. A téren a közepére teszitek a nagy edényt; Puf-Puf óvatosan a zöld köveket alá helyezi, mint egy fészket. A víz kis lángon kezd bugyogni, a finomra vágott sárgarépa és a helyi füvek finoman leülepednek a fazékba.

— Linkaro szabályát követjük, mondja Puf-Puf. A kövek az edény alatt maradnak.
— A zöld gőzt várjuk, teszed hozzá te. Lassan, sietség nélkül.

Amikor a gőz smaragd árnyalatot kap, kis csészékbe öntitek.
— Egyenként, sürgeti a fekete nyuszi. Egy korty, aztán lélegezzetek.

A beteg nyuszik egy-egy kortyot isznak. A füleik felemelkednek, a mellkasuk megnyugszik, a szemeik fényt kapnak. A téren újra halljátok a lépések susogását, rövid nevetést, szelíd beszédet. A félhold zászlók finoman lobognak — az utca zúgása visszatér.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p8'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s1'), 'p8', 'page', 9, 'Semne de vindecare', 'Spre seară, cartierele se liniștesc. Un pui de iepure prinde curaj și face primul salt, fără ezitare. Piața parcă respiră ușor.

Linkaro vă însoțește până la marginea pieței.
— Ați readus echilibrul de bază. Mulțumesc, spune ea.

— Notăm, zice Puf-Puf: Lunaria răspunde bine la grijă și la plan.
— Iar pasul următor îl alegem împreună, adaugi tu.
Iepurele negru dă din cap:
— Dimineață, stabilim drumul.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s1/9.healing.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p8|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p8'), 'ro-ro', 'Semne de vindecare', 'Spre seară, cartierele se liniștesc. Un pui de iepure prinde curaj și face primul salt, fără ezitare. Piața parcă respiră ușor.

Linkaro vă însoțește până la marginea pieței.
— Ați readus echilibrul de bază. Mulțumesc, spune ea.

— Notăm, zice Puf-Puf: Lunaria răspunde bine la grijă și la plan.
— Iar pasul următor îl alegem împreună, adaugi tu.
Iepurele negru dă din cap:
— Dimineață, stabilim drumul.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p8|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p8'), 'en-us', 'Signs of Healing', 'Toward evening, the neighborhoods calm down. A rabbit kit gains courage and makes the first jump, without hesitation. The square seems to breathe lightly.

Linkaro accompanies you to the edge of the square.
— You''ve restored the basic balance. Thank you, she says.

— We note, says Puf-Puf: Lunaria responds well to care and planning.
— And the next step we choose together, you add.
The black rabbit nods:
— In the morning, we''ll establish the path.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s1:p8|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s1:p8'), 'hu-hu', 'Gyógyulás jelei', 'Estére a negyedek megnyugszanak. Egy nyuszi kölyök bátorságot kap és az első ugrást teszi, habozás nélkül. A tér mintha könnyedén lélegezne.

Linkaro a tér széléig kísér titeket.
— Visszahoztátok az alapvető egyensúlyt. Köszönöm, mondja.

— Jegyezzük, mondja Puf-Puf: Lunaria jól reagál a gondoskodásra és a tervezésre.
— A következő lépést együtt választjuk, teszed hozzá te.
A fekete nyuszi bólint:
— Reggel megállapítjuk az utat.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), 'lunaria-s2', 'Semnalul', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:lunaria-s2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), 'ro-ro', 'Semnalul')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:lunaria-s2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), 'en-us', 'The Signal')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:lunaria-s2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), 'hu-hu', 'A jel')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for lunaria-s2
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '1', 'page', 1, 'Aterizare la marginea orașului', 'Portalul se stinge în spatele vostru. În vale se întind câmpuri portocalii de morcovi, dar e o liniște ciudată.
Puf-Puf ridică lupa spre cristal:
— Prea liniște pentru Lunaria. Ținem urechile ciulite.
Iepurele negru încuviințează:
— Mergem încet.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/1.arrival.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:1'), 'ro-ro', 'Aterizare la marginea orașului', 'Portalul se stinge în spatele vostru. În vale se întind câmpuri portocalii de morcovi, dar e o liniște ciudată.
Puf-Puf ridică lupa spre cristal:
— Prea liniște pentru Lunaria. Ținem urechile ciulite.
Iepurele negru încuviințează:
— Mergem încet.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:1'), 'en-us', 'Landing at the edge of town', 'The portal fades behind you. In the valley stretch orange carrot fields, but there''s a strange silence.
Puf-Puf raises the magnifier to the crystal:
— Too quiet for Lunaria. We keep our ears perked up.
The black rabbit nods:
— We go slowly.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:1'), 'hu-hu', 'Leszállás a város szélén', 'A portál kialszik mögöttetek. A völgyben narancssárga sárgarépa mezők terülnek el, de furcsa csend van.
Puf-Puf felemeli a nagyítót a kristályhoz:
— Túl csendes Lunariához. Füleket hegyezünk.
A fekete nyuszi bólint:
— Lassan megyünk.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '2', 'page', 2, 'Ping din campiile de morcovi', 'Cristalul pâlpâie ritmic, ca un ping îndepărtat. Printre rândurile de morcovi, o scânteie se aprinde o clipă și se stinge.
— Semnal tehnic, nu animal, zice Puf-Puf. Acolo.
Iepurele negru își rotește urechile:
— Nu e licurici. Sună ca o baliză.
Vă aplecați și înaintați pe două rânduri paralele, cu pași scurți. În pământ se văd urme fine și un fir subțire, îngropat pe jumătate. Ping-ul revine: mai lent, apoi iarăși scurt.
— Tu rămâi pe stânga, spune Puf-Puf. Eu intru din dreapta.
— Când ridici laba, mă opresc, zici tu.
— Fix. Ne apropiem încet, fără să atingem cablul.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/2.ping.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:2'), 'ro-ro', 'Ping din campiile de morcovi', 'Cristalul pâlpâie ritmic, ca un ping îndepărtat. Printre rândurile de morcovi, o scânteie se aprinde o clipă și se stinge.
— Semnal tehnic, nu animal, zice Puf-Puf. Acolo.
Iepurele negru își rotește urechile:
— Nu e licurici. Sună ca o baliză.
Vă aplecați și înaintați pe două rânduri paralele, cu pași scurți. În pământ se văd urme fine și un fir subțire, îngropat pe jumătate. Ping-ul revine: mai lent, apoi iarăși scurt.
— Tu rămâi pe stânga, spune Puf-Puf. Eu intru din dreapta.
— Când ridici laba, mă opresc, zici tu.
— Fix. Ne apropiem încet, fără să atingem cablul.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:2'), 'en-us', 'Ping from the carrot fields', 'The crystal flickers rhythmically, like a distant ping. Among the carrot rows, a spark lights up briefly and goes out.
— Technical signal, not animal, says Puf-Puf. Over there.
The black rabbit rotates his ears:
— It''s not a firefly. Sounds like a beacon.
You bend down and advance on two parallel rows, with short steps. In the ground you can see fine traces and a thin wire, half-buried. The ping returns: slower, then short again.
— You stay on the left, says Puf-Puf. I enter from the right.
— When you lift your paw, I stop, you say.
— Exactly. We approach slowly, without touching the cable.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:2'), 'hu-hu', 'Ping a sárgarépa mezőkből', 'A kristály ritmikusan villog, mint egy távoli ping. A sárgarépa sorok között egy szikra rövid ideig felvillan és kialszik.
— Technikai jel, nem állat, mondja Puf-Puf. Ott.
A fekete nyuszi forgatja a füleit:
— Nem szentjánosbogár. Úgy hangzik, mint egy jelzőfény.
Lefeksztek és két párhuzamos sorban haladtok előre, rövid lépésekkel. A földben finom nyomokat és egy vékony vezetéket láttok, félig eltemetve. A ping visszatér: lassabban, aztán megint rövid.
— Te maradsz balra, mondja Puf-Puf. Én jobbról megyek be.
— Amikor felemeled a mancsod, megállok, mondod te.
— Pontosan. Lassan közeledünk, anélkül, hogy megérintenénk a kábelt.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '3', 'page', 3, 'Sub copăcelul de lună', 'La umbra unui copăcel straniu zace un robot cu carcasă din tinichea. Are urechi scurte, ca două antene, și ochi negri, stinși. Un LED pâlpâie: „…Gru…bot…”.

Puf-Puf, în șoaptă:
— Model iepure. Blând, dar căzut.

Iepurele negru se apleacă:
— Îl putem reporni?', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/3.moon-tree.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:3'), 'ro-ro', 'Sub copăcelul de lună', 'La umbra unui copăcel straniu zace un robot cu carcasă din tinichea. Are urechi scurte, ca două antene, și ochi negri, stinși. Un LED pâlpâie: „…Gru…bot…”.

Puf-Puf, în șoaptă:
— Model iepure. Blând, dar căzut.

Iepurele negru se apleacă:
— Îl putem reporni?', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:3'), 'en-us', 'Under the moon tree', 'In the shade of a strange little tree lies a robot with a tin-like casing. It has short ears, like two antennas, and black, lifeless eyes. An LED flickers: "...Gru...bot...".

Puf-Puf, in a whisper:
— Rabbit model. Gentle, but fallen.

The black rabbit bends down:
— Can we restart it?', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:3'), 'hu-hu', 'A hold fácskája alatt', 'Egy furcsa fácskának az árnyékában egy robot fekszik, bádog házassal. Rövid fülei vannak, mint két antenna, és fekete, kialudt szemek. Egy LED villog: "...Gru...bot...".

Puf-Puf, suttogva:
— Nyuszi modell. Szelíd, de leesett.

A fekete nyuszi lehajol:
— Újraindíthatjuk?', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '4', 'page', 4, 'Circuitul întrerupt', 'Panoul frontal stă întredeschis. Înăuntru, un circuit fisurat cârâie încet și plutește miros de ozon.
Iepurele negru scoate un morcov și zâmbește:
— Pe Lunaria, morcovii nu repară roboți.

— Nu direct, îl corectează Puf-Puf. Dar fibra de Lunkarot — morcovul acela special de pe Lunaria — poate ține loc de jumper, iar sucul lui răcește bine traseele. Contează ritmul, nu forța.

— Atunci tăiem o fibră subțire și picurăm încet, spui tu.
— Fix așa, încuviințează Puf-Puf.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/4.broken-circuit.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:4'), 'ro-ro', 'Circuitul întrerupt', 'Panoul frontal stă întredeschis. Înăuntru, un circuit fisurat cârâie încet și plutește miros de ozon.
Iepurele negru scoate un morcov și zâmbește:
— Pe Lunaria, morcovii nu repară roboți.

— Nu direct, îl corectează Puf-Puf. Dar fibra de Lunkarot — morcovul acela special de pe Lunaria — poate ține loc de jumper, iar sucul lui răcește bine traseele. Contează ritmul, nu forța.

— Atunci tăiem o fibră subțire și picurăm încet, spui tu.
— Fix așa, încuviințează Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:4'), 'en-us', 'The broken circuit', 'The front panel stands ajar. Inside, a cracked circuit sizzles slowly and floats the smell of ozone.
The black rabbit pulls out a carrot and smiles:
— On Lunaria, carrots don''t repair robots.

— Not directly, corrects Puf-Puf. But Lunkarot fiber — that special carrot from Lunaria — can serve as a jumper, and its juice cools the pathways well. It''s about rhythm, not force.

— Then we cut a thin fiber and drip slowly, you say.
— Exactly, agrees Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:4'), 'hu-hu', 'A megszakított áramkör', 'Az elülső panel félig nyitva áll. Belül egy repedt áramkör lassan zizeg és ózon szagot áraszt.
A fekete nyuszi elővesz egy sárgarépát és mosolyog:
— Lunarián a sárgarépa nem javít robotokat.

— Nem közvetlenül, javítja ki Puf-Puf. De a Lunkarot rost — az a speciális sárgarépa Lunariáról — jumper helyett szolgálhat, és a leve hűti jól az útvonalakat. A ritmus számít, nem az erő.

— Akkor vágunk egy vékony rostot és lassan csöpögtetünk, mondod te.
— Pontosan, egyetért Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '5', 'quiz', 5, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/5.quiz.png', 'Circuitul cere ritm, nu împingere. Aveți Lunkarot pentru un jumper moale si sucul care raceste. Care e mișcarea ta aici, ca să-l repari, fără să agravezi fisura?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'ro-ro', NULL, NULL, 'Circuitul cere ritm, nu împingere. Aveți Lunkarot pentru un jumper moale si sucul care raceste. Care e mișcarea ta aici, ca să-l repari, fără să agravezi fisura?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'en-us', NULL, NULL, 'The circuit requires rhythm, not pushing. You have Lunkarot for a soft jumper and juice that cools. What''s your move here, to repair it without aggravating the crack?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'hu-hu', NULL, NULL, 'Az áramkör ritmust kér, nem erőltetést. Van Lunkarot egy puha jumperhez és lé, ami hűti. Mi a mozdulatod itt, hogy megjavítsd, anélkül, hogy súlyosbítanád a repedést?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'a', 'Il montez si demontez pe tot.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:a:Alchemy:Karott:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:a'), 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:a:Personality:courage:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:a'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:a'), 'ro-ro', 'Il montez si demontez pe tot.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:a'), 'en-us', 'I mount and dismount everything.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:a'), 'hu-hu', 'Mindent felszerel és leszerel.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'b', 'Intai ma intreb ce of i inauntru', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:b:Alchemy:Karott:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:b'), 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:b:Personality:curiosity:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:b'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:b'), 'ro-ro', 'Intai ma intreb ce of i inauntru')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:b'), 'en-us', 'First I wonder what''s inside')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:b'), 'hu-hu', 'Először megkérdezem, mi van benne')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'c', 'Fac un plan bun cu tot ce e de facut si apoi actionez.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:c:Alchemy:Karott:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:c'), 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:c:Personality:thinking:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:c'), 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:c'), 'ro-ro', 'Fac un plan bun cu tot ce e de facut si apoi actionez.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:c'), 'en-us', 'I make a good plan with everything that needs to be done and then act.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:c'), 'hu-hu', 'Jó tervet készítek mindennel, amit csinálni kell, aztán cselekszem.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'd', 'Creez o inima artificiala', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:d:Personality:creativity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:d'), 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:d:Alchemy:Karott:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:d'), 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:d'), 'ro-ro', 'Creez o inima artificiala')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:d'), 'en-us', 'I create an artificial heart')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:d'), 'hu-hu', 'Mesterséges szívet hozok létre')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:5'), 'e', 'Totul cu precautie, nu vrem sa-l stricam.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:e:Personality:safety:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:e'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:lunaria-s2:5:e:Alchemy:Karott:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:e'), 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:e'), 'ro-ro', 'Totul cu precautie, nu vrem sa-l stricam.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:e'), 'en-us', 'Everything with caution, we don''t want to break it.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:lunaria-s2:5:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:lunaria-s2:5:e'), 'hu-hu', 'Minden óvatosan, nem akarjuk tönkretenni.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '6', 'page', 6, 'Grubot își găsește vocea', 'Sunetul se stabilizează, iar ochii negri se aprind. Antenele-urechi tresar ușor.

— Sistem… stabil, anunță robotul. Nume: Grubot. Model: iepure. Stare: conștiință activă. Modul umor: experimental.
Puf-Puf zâmbește:
— Îți stă bine cu viața înapoi.
— Mulțumesc, răspunde Grubot. M-am rătăcit după o furtună de câmp magnetic. Casa mea e Mechanika. Pot decola, apoi am nevoie de încărcare.
— Notat, spui tu.
— Salvez coordonatele. Când deschideți ruta, vă aștept acolo, confirmă Grubot.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/6.grubot-talks.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:6'), 'ro-ro', 'Grubot își găsește vocea', 'Sunetul se stabilizează, iar ochii negri se aprind. Antenele-urechi tresar ușor.

— Sistem… stabil, anunță robotul. Nume: Grubot. Model: iepure. Stare: conștiință activă. Modul umor: experimental.
Puf-Puf zâmbește:
— Îți stă bine cu viața înapoi.
— Mulțumesc, răspunde Grubot. M-am rătăcit după o furtună de câmp magnetic. Casa mea e Mechanika. Pot decola, apoi am nevoie de încărcare.
— Notat, spui tu.
— Salvez coordonatele. Când deschideți ruta, vă aștept acolo, confirmă Grubot.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:6'), 'en-us', 'Grubot finds his voice', 'The sound stabilizes, and the black eyes light up. The antenna-ears twitch slightly.

— System... stable, announces the robot. Name: Grubot. Model: rabbit. Status: active consciousness. Humor mode: experimental.
Puf-Puf smiles:
— Life suits you well.
— Thank you, replies Grubot. I got lost after a magnetic field storm. My home is Mechanika. I can take off, then I need charging.
— Noted, you say.
— Saving coordinates. When you open the route, I''ll be waiting there, confirms Grubot.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:6'), 'hu-hu', 'Grubot megtalálja a hangját', 'A hang stabilizálódik, és a fekete szemek kigyulladnak. Az antenna-fülek enyhén megrezzennek.

— Rendszer... stabil, jelenti a robot. Név: Grubot. Modell: nyuszi. Állapot: aktív tudat. Humor mód: kísérleti.
Puf-Puf mosolyog:
— Jól áll neked az élet vissza.
— Köszönöm, válaszolja Grubot. Mágneses mező vihar után tévedtem el. Az otthonom Mechanika. Tudok felszállni, aztán töltésre van szükségem.
— Jegyezve, mondod te.
— Mentem a koordinátákat. Amikor megnyitjátok az útvonalat, ott várok rátok, megerősíti Grubot.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:7'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:lunaria-s2'), '7', 'page', 7, 'Plecarea', 'Propulsoarele lui Grubot şuieră ușor. Se ridică deasupra câmpiilor portocalii, face o voltă scurtă și zboară spre orizont.
— Un prieten nou, o rută viitoare. Continuăm, zice Puf-Puf.
Iepurele negru zâmbește:
— Drumul e clar: înainte.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/lunaria-s2/7.departure.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:7|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:7'), 'ro-ro', 'Plecarea', 'Propulsoarele lui Grubot şuieră ușor. Se ridică deasupra câmpiilor portocalii, face o voltă scurtă și zboară spre orizont.
— Un prieten nou, o rută viitoare. Continuăm, zice Puf-Puf.
Iepurele negru zâmbește:
— Drumul e clar: înainte.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:7|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:7'), 'en-us', 'The departure', 'Grubot''s thrusters hiss softly. He rises above the orange fields, makes a short loop and flies toward the horizon.
— A new friend, a future route. We continue, says Puf-Puf.
The black rabbit smiles:
— The path is clear: forward.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:lunaria-s2:7|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:lunaria-s2:7'), 'hu-hu', 'Távozás', 'Grubot hajtóművei könnyedén fütyülnek. Felemelkedik a narancssárga mezők felett, egy rövid kört tesz és a horizont felé repül.
— Új barát, jövőbeli útvonal. Folytatjuk, mondja Puf-Puf.
A fekete nyuszi mosolyog:
— Az út világos: előre.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), 'mechanika-s1', 'Magnus', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:mechanika-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), 'ro-ro', 'Magnus')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:mechanika-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), 'en-us', 'Magnus')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:mechanika-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), 'hu-hu', 'Magnus')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for mechanika-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '1', 'page', 1, 'Ceață peste Mechanika', 'Coborâți în Mechanika printr-o pâclă groasă și înțepătoare: fum de uzine, scântei rătăcite, aer greu. Luminile orașului sunt doar pete încețoșate; pasarelele scârțâie, roțile dințate se mișcă sacadat.
Puf-Puf aprinde lampa de pe guler.

— Vizibilitate: mică spre foarte mică. Grubot, ne auzi?
Un țiuit scurt. Apoi tăcere.

— Ținem direcția după pulsul cristalului, spui tu.
— Pe sub conducte, aproape de balustradă, zice iepurele negru.

Rătăciți câteva minute printre țevi, nituri și aburi reci, urmărind pulsația slabă a cristalului.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/1.fog.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:1'), 'ro-ro', 'Ceață peste Mechanika', 'Coborâți în Mechanika printr-o pâclă groasă și înțepătoare: fum de uzine, scântei rătăcite, aer greu. Luminile orașului sunt doar pete încețoșate; pasarelele scârțâie, roțile dințate se mișcă sacadat.
Puf-Puf aprinde lampa de pe guler.

— Vizibilitate: mică spre foarte mică. Grubot, ne auzi?
Un țiuit scurt. Apoi tăcere.

— Ținem direcția după pulsul cristalului, spui tu.
— Pe sub conducte, aproape de balustradă, zice iepurele negru.

Rătăciți câteva minute printre țevi, nituri și aburi reci, urmărind pulsația slabă a cristalului.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:1'), 'en-us', 'Fog over Mechanika', 'You descend into Mechanika through thick, stinging haze: factory smoke, stray sparks, heavy air. The city lights are just blurred spots; the walkways creak, the gears move jerkily.
Puf-Puf lights the lamp on his collar.

— Visibility: small to very small. Grubot, can you hear us?
A short beep. Then silence.

— We keep direction by the crystal''s pulse, you say.
— Under pipes, close to the railing, says the black rabbit.

You wander for a few minutes among pipes, rivets and cold steam, following the crystal''s weak pulse.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:1'), 'hu-hu', 'Köd Mechanika felett', 'Mechanikába sűrű, csípős ködön keresztül szálltok le: gyárfüst, eltévedt szikrák, nehéz levegő. A város fényei csak ködös foltok; a gyaloghidak nyikorognak, a fogaskerekek rángatózva mennek.
Puf-Puf bekapcsolja a gallérlámpáját.

— Látótávolság: kicsi a nagyon kicsiig. Grubot, hallasz minket?
Egy rövid sípolás. Aztán csend.

— A kristály pulzusa szerint tartjuk az irányt, mondod te.
— Csövek alatt, közel a korláthoz, mondja a fekete nyuszi.

Néhány percig kóboroltok csövek, szegecsek és hideg gőz között, követve a kristály gyenge pulzusát.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '2', 'page', 2, 'Grubot vă găsește', 'Un semnal curat taie ceața; o rază subțire brăzdează aerul. Din fum se conturează silueta lui Grubot.

— V-am detectat după semnătura cristalului și… o urmă chimică familiară: Lunkarot, spune robotul Grubot.
Puf-Puf râde:
— Nas digital bun.
— Promisiunea e promisiune, continuă Grubot. Mi-ați dat puls; eu vă dau vedere. Urmați-mă la Turnul de Control.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/2.grubot.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:2'), 'ro-ro', 'Grubot vă găsește', 'Un semnal curat taie ceața; o rază subțire brăzdează aerul. Din fum se conturează silueta lui Grubot.

— V-am detectat după semnătura cristalului și… o urmă chimică familiară: Lunkarot, spune robotul Grubot.
Puf-Puf râde:
— Nas digital bun.
— Promisiunea e promisiune, continuă Grubot. Mi-ați dat puls; eu vă dau vedere. Urmați-mă la Turnul de Control.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:2'), 'en-us', 'Grubot finds you', 'A clean signal cuts through the fog; a thin beam streaks through the air. From the smoke, Grubot''s silhouette takes shape.

— I detected you by the crystal''s signature and… a familiar chemical trace: Lunkarot, says the robot Grubot.
Puf-Puf laughs:
— Good digital nose.
— A promise is a promise, continues Grubot. You gave me pulse; I give you vision. Follow me to the Control Tower.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:2'), 'hu-hu', 'Grubot megtalál titeket', 'Egy tiszta jel vágja át a ködöt; egy vékony sugár vonja a levegőt. A füstből Grubot sziluettje rajzolódik ki.

— A kristály aláírása és... egy ismerős kémiai nyom után észleltem titeket: Lunkarot, mondja a robot Grubot.
Puf-Puf nevet:
— Jó digitális orrod van.
— Az ígéret ígéret marad, folytatja Grubot. Adtatok nekem pulzust; én látást adok. Kövessetek a Vezérlő toronyhoz.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '3', 'page', 3, 'Turnul de control', 'Urcați scările metalice, cu pașii bătuți în ecou. Sus, vă așteaptă un colos: Telescopul Mechanika-Magnus — tuburi, oglinzi, angrenaje; cupola se deschide ca o floare de metal.

— Cu ăsta căutăm planeta mea? întreabă Puf-Puf.
— În mod normal scanează traseele orașului, răspunde Grubot. Dar cu adaptorul de cristal îl putem regla pe semnătura planetei tale.
— Atunci îl montăm acum, zici tu.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/3.control-tower.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:3'), 'ro-ro', 'Turnul de control', 'Urcați scările metalice, cu pașii bătuți în ecou. Sus, vă așteaptă un colos: Telescopul Mechanika-Magnus — tuburi, oglinzi, angrenaje; cupola se deschide ca o floare de metal.

— Cu ăsta căutăm planeta mea? întreabă Puf-Puf.
— În mod normal scanează traseele orașului, răspunde Grubot. Dar cu adaptorul de cristal îl putem regla pe semnătura planetei tale.
— Atunci îl montăm acum, zici tu.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:3'), 'en-us', 'The control tower', 'You climb the metal stairs, with steps echoing. Upstairs, a colossus awaits you: the Mechanika-Magnus Telescope — tubes, mirrors, gears; the dome opens like a metal flower.

— With this we''re looking for my planet? asks Puf-Puf.
— Normally it scans the city''s routes, replies Grubot. But with the crystal adapter we can tune it to your planet''s signature.
— Then we mount it now, you say.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:3'), 'hu-hu', 'A vezérlő torony', 'Felmászol a fém lépcsőkön, a lépések visszhangoznak. Fent egy kolosszus vár rátok: a Mechanika-Magnus Teleszkóp — csövek, tükrök, fogaskerekek; a kupola fém virágként nyílik ki.

— Ezzel keressük a bolygómat? kérdezi Puf-Puf.
— Normálisan a város útvonalait szkennel, válaszolja Grubot. De a kristály adapterrel a bolygód aláírására hangolhatjuk.
— Akkor most felrakjuk, mondod te.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '4', 'page', 4, 'Ce lipsește', 'Grubot rabatează panoul lateral al telescopului. Înăuntru se vede un soclu gol și bobine groase.

— Ne lipsesc două lucruri, spune el clar:

o piatră din Lunaria pentru „Ochiul Verde” — piesa care calibrează spectrul;

energie stocată pentru bobinele de focalizare.

— Avem un rezervor sub turn: încărcat, dar instabil, adaugă Grubot.
— Dacă aveți piatra, o montez acum; dacă nu, pregătim locașul și stabilizăm bobinele.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/4.missing.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:4'), 'ro-ro', 'Ce lipsește', 'Grubot rabatează panoul lateral al telescopului. Înăuntru se vede un soclu gol și bobine groase.

— Ne lipsesc două lucruri, spune el clar:

o piatră din Lunaria pentru „Ochiul Verde” — piesa care calibrează spectrul;

energie stocată pentru bobinele de focalizare.

— Avem un rezervor sub turn: încărcat, dar instabil, adaugă Grubot.
— Dacă aveți piatra, o montez acum; dacă nu, pregătim locașul și stabilizăm bobinele.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:4'), 'en-us', 'What''s missing', 'Grubot folds back the telescope''s side panel. Inside you can see an empty socket and thick coils.

— We''re missing two things, he says clearly:

a stone from Lunaria for the "Green Eye" — the piece that calibrates the spectrum;

stored energy for the focusing coils.

— We have a reservoir under the tower: charged, but unstable, adds Grubot.
— If you have the stone, I mount it now; if not, we prepare the socket and stabilize the coils.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:4'), 'hu-hu', 'Mi hiányzik', 'Grubot visszahajtja a teleszkóp oldalsó paneljét. Belül egy üres foglalat és vastag tekercsek látszanak.

— Két dolog hiányzik, mondja világosan:

egy kő Lunariáról a "Zöld Szemhez" — a darab, amely kalibrálja a spektrumot;

tárolt energia a fókuszáló tekercsekhez.

— Van egy tartály a torony alatt: feltöltve, de instabil, teszi hozzá Grubot.
— Ha van a kötök, most felrakom; ha nincs, előkészítjük a foglalatot és stabilizáljuk a tekercseket.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '5', 'quiz', 5, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/5.quiz.png', 'Rezervorul are energie, dar conexiunea la bobine cere finețe. Ce mișcare alegi ca să pornești telescopul fără să forțezi?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'ro-ro', NULL, NULL, 'Rezervorul are energie, dar conexiunea la bobine cere finețe. Ce mișcare alegi ca să pornești telescopul fără să forțezi?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'en-us', NULL, NULL, 'The reservoir has energy, but the connection to the coils requires finesse. What move do you choose to start the telescope without forcing it?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'hu-hu', NULL, NULL, 'A tartályban van energia, de a tekercsekhez való kapcsolódás finomságot igényel. Milyen mozdulatot választasz, hogy beindítsd a teleszkópot anélkül, hogy erőltetnéd?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'a', 'Lipesc piatra cu mana mea.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:a:Personality:curiosity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), 'Personality', 'curiosity', 2)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:a:Personality:courage:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:a:Alchemy:circuite electrice:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), 'ro-ro', 'Lipesc piatra cu mana mea.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), 'en-us', 'I attach the stone with my hand.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:a'), 'hu-hu', 'A kezemmel ragasztom fel a követ.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'b', 'a intreb oare cum pot lega piatra si ce se va intampla dupa', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:b:Personality:courage:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:b:Personality:curiosity:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:b:Alchemy:circuite electrice:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), 'ro-ro', 'a intreb oare cum pot lega piatra si ce se va intampla dupa')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), 'en-us', 'I wonder how I can connect the stone and what will happen after')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:b'), 'hu-hu', 'Megkérdezem, hogyan köthetem fel a követ és mi fog történni utána')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'c', 'Ma gandesc intai sa pun piatra, il pornesc dupa daca nu merge fac alt plan.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:c:Personality:thinking:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:c:Personality:curiosity:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:c:Personality:courage:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:c:Alchemy:circuite electrice:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'ro-ro', 'Ma gandesc intai sa pun piatra, il pornesc dupa daca nu merge fac alt plan.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'en-us', 'I think first to put the stone, start it after, if it doesn''t work I make another plan.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:c'), 'hu-hu', 'Először gondolkodom, hogy felrakom a követ, utána beindítom, ha nem megy, más tervet csinálok.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'd', 'Improvizez o sursa de energie a pietrei si a penei de pe tera de la pasare.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:d:Alchemy:circuite electrice:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:d:Personality:curiosity:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:d:Personality:creativity:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:d:Personality:courage:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'ro-ro', 'Improvizez o sursa de energie a pietrei si a penei de pe tera de la pasare.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'en-us', 'I improvise an energy source from the stone and the feather from the bird on Terra.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:d'), 'hu-hu', 'Improvizálok egy energiaforrást a kőből és a Terán lévő madár tollából.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:5'), 'e', 'Intai montez piatra, vad daca se strica, dupa dau inainte, pas cu pas.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:e:Personality:curiosity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:e:Personality:courage:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:e:Alchemy:circuite electrice:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:mechanika-s1:5:e:Personality:safety:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'ro-ro', 'Intai montez piatra, vad daca se strica, dupa dau inainte, pas cu pas.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'en-us', 'First I mount the stone, see if it breaks, then move forward, step by step.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:mechanika-s1:5:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:mechanika-s1:5:e'), 'hu-hu', 'Először felrakom a követ, megnézem, hogy tönkremegy-e, utána lépésről lépésre haladok.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '6', 'page', 6, 'Focalizare', 'Telescopul se trezește. Oglinzile se aliniază, hologramele se aprind, iar pe ele se ramifică linii de lumină. Un traseu pulsează mai tare — un fir care vă cheamă.

— Arată-ne Kelo-Ketis! spune Puf-Puf.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/6.focus.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:6'), 'ro-ro', 'Focalizare', 'Telescopul se trezește. Oglinzile se aliniază, hologramele se aprind, iar pe ele se ramifică linii de lumină. Un traseu pulsează mai tare — un fir care vă cheamă.

— Arată-ne Kelo-Ketis! spune Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:6'), 'en-us', 'Focus', 'The telescope awakens. The mirrors align, the holograms light up, and on them light lines branch out. One route pulses stronger — a thread that calls you.

— Show us Kelo-Ketis! says Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:6'), 'hu-hu', 'Fókuszálás', 'A teleszkóp felébred. A tükrök igazodnak, a hologramok kigyulladnak, és rajtuk fény vonalak ágaznak ki. Egy útvonal erősebben pulzál — egy szál, amely arra hív.

— Mutasd meg a Kelo-Ketist! mondja Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:7'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '7', 'page', 7, 'Harta se adâncește', 'Grubot tastează rapid.
— Semnal clar. Fixez coordonatele și salvez schema rutei pentru deschiderea portalului.
Puf-Puf zâmbește scurt:
— Primul pas mare e bifat.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/7.map-deepens.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:7|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:7'), 'ro-ro', 'Harta se adâncește', 'Grubot tastează rapid.
— Semnal clar. Fixez coordonatele și salvez schema rutei pentru deschiderea portalului.
Puf-Puf zâmbește scurt:
— Primul pas mare e bifat.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:7|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:7'), 'en-us', 'The map deepens', 'Grubot types rapidly.
— Clear signal. I''m fixing the coordinates and saving the route scheme for opening the portal.
Puf-Puf smiles briefly:
— First big step is checked off.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:7|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:7'), 'hu-hu', 'A térkép mélyül', 'Grubot gyorsan gépel.
— Tiszta jel. Rögzítem a koordinátákat és mentem az útvonal-sémát a portál megnyitásához.
Puf-Puf röviden mosolyog:
— Az első nagy lépés kipipálva.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:8'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '8', 'page', 8, 'Recompensa de atelier', 'Grubot vă întinde o casetă metalică.
—(voce robotica) Circuite și conectori. Pentru ce urmează.

Puf-Puf chicotește:
— Le pun lângă biscuiții de motivație.

— Bună combinație: energie și moral, zici tu.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/8.reward.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:8|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:8'), 'ro-ro', 'Recompensa de atelier', 'Grubot vă întinde o casetă metalică.
—(voce robotica) Circuite și conectori. Pentru ce urmează.

Puf-Puf chicotește:
— Le pun lângă biscuiții de motivație.

— Bună combinație: energie și moral, zici tu.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:8|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:8'), 'en-us', 'Workshop reward', 'Grubot hands you a metal case.
—(robotic voice) Circuits and connectors. For what comes next.

Puf-Puf giggles:
— I''ll put them next to the motivation cookies.

— Good combination: energy and morale, you say.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:8|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:8'), 'hu-hu', 'A műhely jutalma', 'Grubot egy fém dobozt nyújt át.
—(robotikus hang) Áramkörök és csatlakozók. A következőkre.

Puf-Puf kuncog:
— A motivációs kekszek mellé teszem őket.

— Jó kombináció: energia és morál, mondod te.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:9'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:mechanika-s1'), '9', 'page', 9, 'Privirea înainte', 'Telescopul intră în repaus, iar ceața din oraș pare mai subțire. Holograma rămâne cu harta adâncită pe ecran, traseele marcate clar. Caseta cu Circuite electrice e prinsă la rucsac.

— Ok, echipă. Următoarea ramură? zice Puf-Puf.
— Est, pe pasarela principală, propune iepurele negru. E cea mai curată.
— Confirm, spune Grubot. Indicatorii arată vânt mai slab pe acolo.
— Atunci est să fie, închei tu. Mergem.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/mechanika-s1/9.ahead.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:9|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:9'), 'ro-ro', 'Privirea înainte', 'Telescopul intră în repaus, iar ceața din oraș pare mai subțire. Holograma rămâne cu harta adâncită pe ecran, traseele marcate clar. Caseta cu Circuite electrice e prinsă la rucsac.

— Ok, echipă. Următoarea ramură? zice Puf-Puf.
— Est, pe pasarela principală, propune iepurele negru. E cea mai curată.
— Confirm, spune Grubot. Indicatorii arată vânt mai slab pe acolo.
— Atunci est să fie, închei tu. Mergem.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:9|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:9'), 'en-us', 'Looking ahead', 'The telescope enters rest, and the fog in the city seems thinner. The hologram remains with the deepened map on screen, the routes marked clearly. The case with Electric circuits is attached to the backpack.

— Ok, team. Next branch? says Puf-Puf.
— East, on the main walkway, proposes the black rabbit. It''s the cleanest.
— Confirmed, says Grubot. The indicators show weaker wind there.
— Then east it is, you conclude. Let''s go.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:mechanika-s1:9|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:mechanika-s1:9'), 'hu-hu', 'Előre tekintés', 'A teleszkóp pihenőbe megy, és a város köde vékonyabbnak tűnik. A hologram a mélyült térképpel marad a képernyőn, az útvonalak világosan megjelölve. A doboz az Elektromos áramkörökkel a hátizsákhoz van rögzítve.

— Oké, csapat. A következő ág? mondja Puf-Puf.
— Kelet, a fő gyalogúton, javasolja a fekete nyuszi. Az a legtisztább.
— Megerősítve, mondja Grubot. A mutatók gyengébb szélmutatást jeleznek ott.
— Akkor kelet lesz, zárnod le. Menjünk.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:sylvaria-s1'), 'sylvaria-s1', 'Sylvaria', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/sylvaria-s1/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:sylvaria-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:sylvaria-s1'), 'ro-ro', 'Sylvaria')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:sylvaria-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:sylvaria-s1'), 'en-us', 'Sylvaria')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:sylvaria-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:sylvaria-s1'), 'hu-hu', 'Sylvaria')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for sylvaria-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:sylvaria-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:sylvaria-s1'), '1', 'page', 1, 'Sylvaria', 'Sylvaria', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/sylvaria-s1/0.Cover.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:sylvaria-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:sylvaria-s1:1'), 'ro-ro', 'Sylvaria', 'Sylvaria', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:sylvaria-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:sylvaria-s1:1'), 'en-us', 'Sylvaria', 'Sylvaria', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:sylvaria-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:sylvaria-s1:1'), 'hu-hu', 'Sylvaria', 'Sylvaria', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), 'terra-s1', 'Umbra de la Fermă', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:terra-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), 'ro-ro', 'Umbra de la Fermă')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:terra-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), 'en-us', 'The Shadow at the Farm')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:terra-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), 'hu-hu', 'A Farm Árnyéka')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for terra-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '1', 'page', 1, 'Sosirea', 'Puf-Puf decide să exploreze planeta pe care tocmai a aterizat. Mergeți împreună și după câteva ore, o fermă, unde sunt mai mulți săteni, vă iese în cale. Totul pare în ordine o perioadă, dar oamenii își sunt îngrijorați și privesc cerul ca pe un oaspete nepoftit. \
Puf-Puf își încrețește mustățile.\
Puf-Puf: - Oare de ce e lumea așa de speriată? De ce toți stau de parcă își țin respirația?', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/1.arrival.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:1'), 'ro-ro', 'Sosirea', 'Puf-Puf decide să exploreze planeta pe care tocmai a aterizat. Mergeți împreună și după câteva ore, o fermă, unde sunt mai mulți săteni, vă iese în cale. Totul pare în ordine o perioadă, dar oamenii își sunt îngrijorați și privesc cerul ca pe un oaspete nepoftit. \
Puf-Puf își încrețește mustățile.\
Puf-Puf: - Oare de ce e lumea așa de speriată? De ce toți stau de parcă își țin respirația?', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:1'), 'en-us', 'Arrival', 'Puf-Puf decides to explore the planet he just landed on. You walk together and after a few hours, a farm, where several villagers live, appears in your path. Everything seems in order for a while, but people are worried and watch the sky like an unwelcome guest. 
Puf-Puf twitches his whiskers.
Puf-Puf: - I wonder why the world is so scared? Why is everyone standing as if holding their breath?', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:1'), 'hu-hu', 'Érkezés', 'Puf-Puf úgy dönt, hogy felfedezi a bolygót, amelyre éppen leszállt. Együtt sétáltok és néhány óra után egy farm jelenik meg az úton, ahol több falusi lakik. Minden rendben tűnik egy ideig, de az emberek aggódnak és úgy néznek az égre, mint egy nem kívánt vendégre. 
Puf-Puf összeráncolja a bajuszát.
Puf-Puf: - Vajon miért van a világ ennyire ijedt? Miért áll mindenki úgy, mintha visszatartja a lélegzetét?', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '2', 'page', 2, 'Tăcerea stranie', 'Niciun cocoș nu cântă, niciun câine nu latră, nici greierii nu-și găsesc corul. Liniștea apasă, groasă, ca o pătură udă. Puf-Puf se uită la tine.\nPuf-Puf: - E ca și cum natura ar număra până la zece și n-ar îndrăzni să spună: zece!', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/2.silence.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:2'), 'ro-ro', 'Tăcerea stranie', 'Niciun cocoș nu cântă, niciun câine nu latră, nici greierii nu-și găsesc corul. Liniștea apasă, groasă, ca o pătură udă. Puf-Puf se uită la tine.\nPuf-Puf: - E ca și cum natura ar număra până la zece și n-ar îndrăzni să spună: zece!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:2'), 'en-us', 'Strange Silence', 'No rooster crows, no dog barks, not even the crickets find their chorus. The silence presses, thick, like a wet blanket. Puf-Puf looks at you.
Puf-Puf: - It''s as if nature were counting to ten and didn''t dare say: ten!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:2'), 'hu-hu', 'A furcsa csend', 'Egy kakas sem kukorékol, egy kutya sem ugat, a tücskök sem találják a kórusukat. A csend nyomja, vastag, mint egy nedves takaró. Puf-Puf rád néz: "Mintha a természet tízig számolna és nem merne tízre mondani."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '3', 'page', 3, 'Șoaptele oamenilor', 'Un bătrân se apropie cu pași mărunți.\nBătrânul: -E… o umbră... coboară din cer la apus. Animalele fug, grânele se culcă la pământ. Zicem că-i un blestem. \nPuf-Puf: - Blestem sau semn, tot o cauză are. Hai să prindem urma!', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/3.whispers.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:3'), 'ro-ro', 'Șoaptele oamenilor', 'Un bătrân se apropie cu pași mărunți.\nBătrânul: -E… o umbră... coboară din cer la apus. Animalele fug, grânele se culcă la pământ. Zicem că-i un blestem. \nPuf-Puf: - Blestem sau semn, tot o cauză are. Hai să prindem urma!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:3'), 'en-us', 'Whispers of the People', 'An old man approaches with small steps.
Old man: -It''s… a shadow... it comes down from the sky at sunset. The animals flee, the grain lies flat. We say it''s a curse. 
Puf-Puf: - Curse or sign, it still has a cause. Let''s track it!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:3'), 'hu-hu', 'Az emberek suttogása', 'Egy öregember apró lépésekkel közeledik.
Öregember: -Ez... egy árnyék... lejön az égből napnyugtakor. Az állatok menekülnek, a gabonák a földre fekszenek. Azt mondjuk, hogy átok. 
Puf-Puf: - Átok vagy jel, minden okkal van. Fogjuk meg a nyomát!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '4', 'page', 4, 'Prima noapte', 'Soarele se scurge după dealuri, iar vântul aduce un foșnet nou, neliniștit. Deasupra norilor, ceva mare se mișcă. Umbră peste câmpuri, iar luna parcă se dă la o parte. Puf-Puf își ține respirația o clipă și zâmbește puțin: \nPuf-Puf:- OK… asta nu e doar vreme schimbătoare.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/4.first-night.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:4'), 'ro-ro', 'Prima noapte', 'Soarele se scurge după dealuri, iar vântul aduce un foșnet nou, neliniștit. Deasupra norilor, ceva mare se mișcă. Umbră peste câmpuri, iar luna parcă se dă la o parte. Puf-Puf își ține respirația o clipă și zâmbește puțin: \nPuf-Puf:- OK… asta nu e doar vreme schimbătoare.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:4'), 'en-us', 'The First Night', 'The sun sinks behind the hills, and the wind brings a new, restless rustle. Above the clouds, something large moves. A shadow across the fields, and the moon seems to step aside. Puf-Puf holds his breath for a moment and smiles a little: 
Puf-Puf:- OK… this isn''t just changing weather.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:4'), 'hu-hu', 'Az első éjszaka', 'A nap a dombok mögé csorog, és a szél új, nyugtalan zörgést hoz. A felhők felett valami nagy mozog. Árnyék a mezők felett, és a hold mintha félrehúzódna. Puf-Puf egy pillanatra visszatartja a lélegzetét és kissé mosolyog: "OK... ez nem csak változékony időjárás."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '5', 'page', 5, 'Vibrația pământului', 'Pământul tresare, ușor la început, apoi ca un pas greoi. Aerul miroase a fum și a cenușă curată, ca după o scânteie. În lanul culcat, se ghicește o siluetă uriașă care pâlpâie intermitent, ca un far rănit.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/5.vibration.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:5'), 'ro-ro', 'Vibrația pământului', 'Pământul tresare, ușor la început, apoi ca un pas greoi. Aerul miroase a fum și a cenușă curată, ca după o scânteie. În lanul culcat, se ghicește o siluetă uriașă care pâlpâie intermitent, ca un far rănit.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:5'), 'en-us', 'The Earth''s Vibration', 'The ground shivers, lightly at first, then like a heavy footstep. The air smells of smoke and clean ash, like after a spark. In the flattened field, a huge silhouette can be made out, flickering on and off like a wounded beacon.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:5'), 'hu-hu', 'A föld rezgése', 'A föld megremeg, először könnyedén, majd mint egy nehéz lépés. A levegő füst és tiszta hamu szagát árasztja, mint egy szikra után. A lefektetett kalásztengerben egy hatalmas sziluett sejlik, amely szaggatottan villog, mint egy sérült világítótorony.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '6', 'page', 6, 'Creatura', 'Din umbre iese la iveală o pasăre imensă, cu pene ca jarul și ochi de cristal. Aripa stângă îi atârnă grea; fiecare bătaie ridică scântei care se sting pe apă. Nu e un blestem: e o ființă rătăcită din altă lume, căzută într-un loc care nu o înțelege încă.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/6.creature.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:6'), 'ro-ro', 'Creatura', 'Din umbre iese la iveală o pasăre imensă, cu pene ca jarul și ochi de cristal. Aripa stângă îi atârnă grea; fiecare bătaie ridică scântei care se sting pe apă. Nu e un blestem: e o ființă rătăcită din altă lume, căzută într-un loc care nu o înțelege încă.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:6'), 'en-us', 'The Creature', 'From the shadows emerges a massive bird, with ember-like feathers and crystal eyes. Its left wing hangs heavy; each flap throws up sparks that fizzle on the water. It''s not a curse: it''s a lost being from another world, fallen into a place that doesn''t understand it yet.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:6'), 'hu-hu', 'A lény', 'Az árnyékokból egy hatalmas madár tűnik elő, tollakkal, mint a parázs, és kristály szemekkel. A bal szárnya nehezen lóg; minden csapása szikrákat emel, amelyek a vízen kialszanak. Ez nem átok: egy másik világból tévedt lény, aki egy olyan helyre esett, amely még nem érti meg.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:7'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '7', 'page', 7, 'Legătura', 'Puf-Puf se apropie încet, îți face semn din cap și apoi îți vorbește pe șoptite.\nPuf-Puf: - E din Copacul Luminii! Dacă o ajutăm, ne poate arăta un drum ascuns pe Terra. \nPasărea te privește drept, fără teamă; în ochii ei se văd brazde și stele. \nPuf-Puf: - Tu ai mâini bune, zice Puf-Puf cu un zâmbet. \nPuf-Puf: - Și eu am… comentarii utile. Încearcă tu primul!', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/7.bond.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:7|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:7'), 'ro-ro', 'Legătura', 'Puf-Puf se apropie încet, îți face semn din cap și apoi îți vorbește pe șoptite.\nPuf-Puf: - E din Copacul Luminii! Dacă o ajutăm, ne poate arăta un drum ascuns pe Terra. \nPasărea te privește drept, fără teamă; în ochii ei se văd brazde și stele. \nPuf-Puf: - Tu ai mâini bune, zice Puf-Puf cu un zâmbet. \nPuf-Puf: - Și eu am… comentarii utile. Încearcă tu primul!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:7|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:7'), 'en-us', 'The Bond', 'Puf-Puf approaches slowly, nods at you, then whispers to you.
Puf-Puf: - It''s from the Tree of Light! If we help it, it can show us a hidden path on Terra. 
The bird looks straight at you, unafraid; in its eyes are furrows and stars. 
Puf-Puf: - You have good hands, says Puf-Puf with a smile. 
Puf-Puf: - And I have… useful commentary. You try first!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:7|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:7'), 'hu-hu', 'A kapcsolat', 'Puf-Puf lassan közeledik, bólint, majd suttogva szól hozzád: "A Fény Fájából való. Ha segítünk neki, megmutathat egy rejtett utat a Terán." A madár egyenesen néz rád, félelem nélkül; a szemében barázdák és csillagok látszanak. "Neked jó kezeid vannak", mondja Puf-Puf mosolyogva. "És nekem... hasznos megjegyzéseim. Próbáld te először."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '8', 'quiz', 8, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/8.quiz.png', 'Cum ajuți pasărea rănită?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:8|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'ro-ro', NULL, NULL, 'Cum ajuți pasărea rănită?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:8|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'en-us', NULL, NULL, 'How do you help the wounded bird?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:8|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'hu-hu', NULL, NULL, 'Hogyan segítesz a sérült madáron?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'a', 'O încurajezi să se ridice, să-și amintească de puterea ei.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:a:Discovery:discovery credit:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:a'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:a:Personality:courage:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:a'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:a'), 'ro-ro', 'O încurajezi să se ridice, să-și amintească de puterea ei.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:a'), 'en-us', 'Encourage it to rise and remember its strength.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:a'), 'hu-hu', 'Bátorítod, hogy felkeljen, emlékezzen az erejére.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'b', 'O îngrijești cu grijă și răbdare.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:b:Personality:safety:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:b'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:b:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:b'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:b'), 'ro-ro', 'O îngrijești cu grijă și răbdare.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:b'), 'en-us', 'Care for it with patience and care.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:b'), 'hu-hu', 'Gondosan és türelemmel ápolod.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'c', 'Îi construiești un plan pentru a-și întinde din nou aripile.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:c:Personality:thinking:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:c'), 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:c:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:c'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:c'), 'ro-ro', 'Îi construiești un plan pentru a-și întinde din nou aripile.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:c'), 'en-us', 'Build a plan so it can stretch its wings again.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:c'), 'hu-hu', 'Tervet készítesz neki, hogy újra kinyithassa a szárnyait.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'd', 'Îți folosești creativitatea și găsești o metodă nouă de vindecare.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:d:Discovery:discovery credit:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:d'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:d:Personality:creativity:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:d'), 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:d'), 'ro-ro', 'Îți folosești creativitatea și găsești o metodă nouă de vindecare.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:d'), 'en-us', 'Use your creativity and find a new way to heal.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:d'), 'hu-hu', 'A kreativitásodat használod és új gyógyítási módszert találsz.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:8'), 'e', 'Cercetezi semnele luminii de pe penele ei, pentru a descoperi secretul.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:e:Personality:curiosity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:e'), 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s1:8:e:Discovery:discovery credit:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:e'), 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:e'), 'ro-ro', 'Cercetezi semnele luminii de pe penele ei, pentru a descoperi secretul.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:e'), 'en-us', 'Study the light markings on its feathers to uncover the secret.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s1:8:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s1:8:e'), 'hu-hu', 'Megvizsgálod a tollain lévő fény jeleit, hogy felfedezd a titkot.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:9'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s1'), '9', 'page', 9, 'Drumul deschis', 'După ajutorul vostru, pasărea își strânge aripa rănită la piept și, cu un efort calm, își întinde cealaltă. O dungă de lumină se desprinde din penele ei și taie câmpul ca o potecă nouă. \nPuf-Puf: - Uite-l!, spune Puf-Puf, cu un fel de mândrie jucăușă. \nPuf-Puf: - Drumul care n-a existat până acum cinci minute.\nPasărea ridică privirea spre nori, apoi către voi. Puf-Puf primește o pană, în semn de recunoștință, și știe că pasărea va deveni, de acum înainte, protectoarea satului. Pana cea mare, magică, alimentează o energie nebănuită.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s1/9.path.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:9|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:9'), 'ro-ro', 'Drumul deschis', 'După ajutorul vostru, pasărea își strânge aripa rănită la piept și, cu un efort calm, își întinde cealaltă. O dungă de lumină se desprinde din penele ei și taie câmpul ca o potecă nouă. \nPuf-Puf: - Uite-l!, spune Puf-Puf, cu un fel de mândrie jucăușă. \nPuf-Puf: - Drumul care n-a existat până acum cinci minute.\nPasărea ridică privirea spre nori, apoi către voi. Puf-Puf primește o pană, în semn de recunoștință, și știe că pasărea va deveni, de acum înainte, protectoarea satului. Pana cea mare, magică, alimentează o energie nebănuită.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:9|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:9'), 'en-us', 'The Path Opens', 'After your help, the bird tucks its injured wing to its chest and, with a calm effort, stretches the other. A strip of light slips from its feathers and cuts the field like a new path. 
Puf-Puf: - Look at it!, says Puf-Puf with a kind of playful pride. 
Puf-Puf: - The path that didn''t exist until five minutes ago.
The bird lifts its gaze to the clouds, then to you. Puf-Puf receives a feather in gratitude and knows that the bird will become the village''s protector from now on. The great, magical feather feeds an unexpected energy.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s1:9|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s1:9'), 'hu-hu', 'A megnyílt út', 'A segítségetek után a madár a mellkasához húzza a sérült szárnyát, és nyugodt erőfeszítéssel kinyújtja a másikat. Egy fénycsík szakad ki a tollai közül és új ösvényként vágja át a mezőt. 
Puf-Puf: - Nézd!, mondja Puf-Puf egyfajta játékos büszkeséggel. 
Puf-Puf: - Az út, amely öt perccel ezelőtt még nem létezett.
A madár felnéz a felhőkre, majd rátok. Puf-Puf egy tollat kap hálából, és tudja, hogy a madár mostantól a falu védelmezője lesz. A nagy, mágikus toll ismeretlen energiát táplál.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), 'terra-s2', 'Un nou prieten', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/0.Cover.png', NULL, NULL, NULL, NULL,
     0, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', NULL, NULL)
ON CONFLICT ("Id") DO UPDATE
SET "Title" = EXCLUDED."Title",
    "CoverImageUrl" = EXCLUDED."CoverImageUrl",
    "Summary" = EXCLUDED."Summary",
    "StoryTopic" = EXCLUDED."StoryTopic",
    "StoryType" = EXCLUDED."StoryType",
    "Status" = EXCLUDED."Status",
    "SortOrder" = EXCLUDED."SortOrder",
    "IsActive" = EXCLUDED."IsActive",
    "Version" = EXCLUDED."Version",
    "PriceInCredits" = EXCLUDED."PriceInCredits",
    "UpdatedAt" = EXCLUDED."UpdatedAt",
    "CreatedBy" = EXCLUDED."CreatedBy",
    "UpdatedBy" = EXCLUDED."UpdatedBy";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:terra-s2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), 'ro-ro', 'Un nou prieten')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:terra-s2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), 'en-us', 'A New Friend')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:terra-s2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), 'hu-hu', 'Egy új barát')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for terra-s2
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), '1', 'page', 1, 'Ținta: Lentila Lunii', 'Împăratul Puf-Puf nu și-a reparat nava, iar despre Pământ habar nu are unde se află față de Kelo-Ketis. A auzit, în schimb, de un artefact rar: Lentila Lunii. Cu ea, cristalul mărește harta și puteți căuta mai adânc ruta spre Kelo-Ketis. Ținta indică o zonă cu stânci joase și iarbă înaltă, la marginea unei păduri.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/1.target.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:1'), 'ro-ro', 'Ținta: Lentila Lunii', 'Împăratul Puf-Puf nu și-a reparat nava, iar despre Pământ habar nu are unde se află față de Kelo-Ketis. A auzit, în schimb, de un artefact rar: Lentila Lunii. Cu ea, cristalul mărește harta și puteți căuta mai adânc ruta spre Kelo-Ketis. Ținta indică o zonă cu stânci joase și iarbă înaltă, la marginea unei păduri.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:1'), 'en-us', 'Target: The Moon Lens', 'Emperor Puf-Puf hasn''t repaired his ship, and as for Earth, he has no idea where it is in relation to Kelo-Ketis. He has heard, however, of a rare artifact: the Moon Lens. With it, the crystal enlarges the map and you can search deeper for the route to Kelo-Ketis. The target points to an area of low rocks and tall grass at the edge of a forest.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:1'), 'hu-hu', 'Cél: A Hold Lencséje', 'Puf-Puf Császár nem javította meg a hajóját, és a Földről sem tudja, hol van a Kelo-Ketishez képest. Hallott azonban egy ritka műtárgyról: a Hold Lencséjéről. Vele a kristály megnagyobbítja a térképet, és mélyebben kereshetitek a Kelo-Ketis felé vezető útvonalat. A cél alacsony sziklák és magas fű területét jelzi, egy erdő szélén.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), '2', 'page', 2, 'Urme ciudate', 'În iarbă apar urme mici, în zigzag, care dispar lângă un buștean scobit. Pe lângă ele, cozi de morcov „gustate artistic”.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/2.tracks.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:2'), 'ro-ro', 'Urme ciudate', 'În iarbă apar urme mici, în zigzag, care dispar lângă un buștean scobit. Pe lângă ele, cozi de morcov „gustate artistic”.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:2'), 'en-us', 'Strange Tracks', 'In the grass appear small zigzagging tracks that disappear near a hollowed log. Beside them, carrot tails "artistically nibbled".', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:2'), 'hu-hu', 'Furcsa nyomok', 'A füvben kis, cikkcakkos nyomok tűnnek fel, amelyek egy üreges rönk mellett tűnnek el. Mellettük "művészien megkóstolt" sárgarépa farkok.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), '3', 'page', 3, 'Iepurele negru', 'Din buștean sare un iepure negru, cu blana mată și ochi mari. „Bună! Eu sunt… uneori aici, uneori pe Lunaria. Depinde cum clipești”, spune, apoi râde. „Știu unde e Lentila. Și am nevoie de ajutor să ajung acasă, pe Lunaria. Dacă mă ajutați, vă arăt drumul către Lentilă.”\nPuf-Puf:"Lunaria? parca stiu planeta asta...sa mergem..."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/3.black-rabbit.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:3'), 'ro-ro', 'Iepurele negru', 'Din buștean sare un iepure negru, cu blana mată și ochi mari. „Bună! Eu sunt… uneori aici, uneori pe Lunaria. Depinde cum clipești”, spune, apoi râde. „Știu unde e Lentila. Și am nevoie de ajutor să ajung acasă, pe Lunaria. Dacă mă ajutați, vă arăt drumul către Lentilă.”\nPuf-Puf:"Lunaria? parca stiu planeta asta...sa mergem..."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:3'), 'en-us', 'The Black Rabbit', 'From the log leaps a black rabbit, with matte fur and big eyes. "Hello! I''m… sometimes here, sometimes on Lunaria. Depends how you blink," it says, then laughs. "I know where the Lens is. And I need help getting home to Lunaria. If you help me, I''ll show you the way to the Lens."
Puf-Puf:"Lunaria? I think I know this planet...let''s go..."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:3'), 'hu-hu', 'A fekete nyuszi', 'A rönkből egy fekete nyuszi ugrik ki, matt bundával és nagy szemekkel. "Szia! Én... néha itt vagyok, néha Lunarián. Attól függ, hogyan pislogsz", mondja, majd nevet. "Tudom, hol van a Lencse. És segítségre van szükségem, hogy hazajussak Lunariára. Ha segítetek, megmutatom az utat a Lencséhez."
Puf-Puf: "Lunaria? mintha ismerném ezt a bolygót... menjünk..."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), '4', 'page', 4, 'Locul lentilei', 'Ghidați de iepure, ajungeți la o grotă scurtă, rece, cu apă până la gleznă. Pe tavan atârnă cristale palide; pe o stâncă, un disc translucid — Lentila Lunii — prins între două excrescențe de piatră.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/4.cave.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:4'), 'ro-ro', 'Locul lentilei', 'Ghidați de iepure, ajungeți la o grotă scurtă, rece, cu apă până la gleznă. Pe tavan atârnă cristale palide; pe o stâncă, un disc translucid — Lentila Lunii — prins între două excrescențe de piatră.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:4'), 'en-us', 'Where the Lens Is', 'Guided by the rabbit, you reach a short, cold grotto, with ankle-deep water. Pale crystals hang from the ceiling; on a rock rests a translucent disc — the Moon Lens — wedged between two stone outgrowths.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:4'), 'hu-hu', 'A lencse helye', 'A nyuszi vezetésével egy rövid, hideg barlanghoz érkeztek, ahol a víz a bokáig ér. A mennyezeten halvány kristályok lógnak; egy sziklán egy áttetsző korong — a Hold Lencséje — két kőkinövés között szorulva.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), '5', 'page', 5, 'Problema mică', 'Când atingi stânca, apa face valuri și intrarea parcă „respiră”, strângându-se ușor. „Grotă sensibilă”, spune iepurele. „Faci bine, primești ieșire. Faci rău, primești duș rece.”', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/5.problem.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:5'), 'ro-ro', 'Problema mică', 'Când atingi stânca, apa face valuri și intrarea parcă „respiră”, strângându-se ușor. „Grotă sensibilă”, spune iepurele. „Faci bine, primești ieșire. Faci rău, primești duș rece.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:5'), 'en-us', 'A Small Problem', 'When you touch the rock, the water ripples and the entrance seems to "breathe", narrowing slightly. "Sensitive grotto," says the rabbit. "Do good, you get an exit. Do wrong, you get a cold shower."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:5'), 'hu-hu', 'A kis probléma', 'Amikor megérinted a sziklát, a víz hullámokat csinál és a bejárat mintha "lélegzik", kissé összehúzódva. "Érzékeny barlang", mondja a nyuszi. "Jól csinálod, kijáratot kapsz. Rosszul csinálod, hideg zuhanyt kapsz."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), '6', 'quiz', 6, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/6.quiz.png', 'Alege abordarea ca să scoți Lentila Lunii fără să destabilizezi grota:', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'ro-ro', NULL, NULL, 'Alege abordarea ca să scoți Lentila Lunii fără să destabilizezi grota:', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'en-us', NULL, NULL, 'Choose the approach to extract the Moon Lens without destabilizing the grotto:', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'hu-hu', NULL, NULL, 'Válaszd ki a megközelítést, hogy kivond a Hold Lencséjét anélkül, hogy destabilizálnád a barlangot:', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'a', 'Te strecori pe pragul îngust și lucrezi rapid, ca să nu lași timp pereților să „respire”.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s2:6:a:Personality:courage:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:a'), 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:a'), 'ro-ro', 'Te strecori pe pragul îngust și lucrezi rapid, ca să nu lași timp pereților să „respire”.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:a'), 'en-us', 'Slip over the narrow threshold and work quickly, giving the walls no time to "breathe".')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:a'), 'hu-hu', 'Átcsúszol a keskeny küszöbön és gyorsan dolgozol, hogy ne hagyj időt a falaknak "lélegzeni".')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'b', 'Te miști încet, verifici sprijinul la fiecare pas și menții apa cât mai liniștită.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s2:6:b:Personality:safety:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:b'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:b'), 'ro-ro', 'Te miști încet, verifici sprijinul la fiecare pas și menții apa cât mai liniștită.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:b'), 'en-us', 'Move slowly, check your footing at each step, and keep the water as calm as possible.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:b'), 'hu-hu', 'Lassan mozogsz, minden lépésnél ellenőrzöd a támasztékot és a vizet a lehető legnyugodtabban tartod.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'c', 'Faci un plan scurt: cine ține lanterna, cine fixează pârghia, ordinea mișcărilor, semn la „stop”.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s2:6:c:Personality:thinking:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:c'), 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:c'), 'ro-ro', 'Faci un plan scurt: cine ține lanterna, cine fixează pârghia, ordinea mișcărilor, semn la „stop”.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:c'), 'en-us', 'Make a short plan: who holds the flashlight, who sets the lever, the order of moves, and a "stop" signal.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:c'), 'hu-hu', 'Rövid tervet készítesz: ki tartja a lámpát, ki rögzíti a emelőt, a mozgások sorrendje, "stop" jel.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:d'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'd', 'Improvizezi o unealtă: bețișor cu buclă din sfoară ca să tragi lentila de la distanță.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s2:6:d:Personality:creativity:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:d'), 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:d|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:d'), 'ro-ro', 'Improvizezi o unealtă: bețișor cu buclă din sfoară ca să tragi lentila de la distanță.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:d|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:d'), 'en-us', 'Improvise a tool: a stick with a loop of string to pull the Lens from a distance.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:d|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:d'), 'hu-hu', 'Improvizálsz egy eszközt: bot hurokkal a kötélből, hogy távolról húzd ki a lencsét.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:e'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:6'), 'e', 'Pui mai întâi marcaje și ieșiri sigure: pietre-pas, frânghie la intrare, timp-limită pentru retragere.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:terra-s2:6:e:Personality:safety:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:e'), 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:e|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:e'), 'ro-ro', 'Pui mai întâi marcaje și ieșiri sigure: pietre-pas, frânghie la intrare, timp-limită pentru retragere.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:e|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:e'), 'en-us', 'First set markers and safe exits: stepping stones, a rope at the entrance, and a time limit for retreat.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:terra-s2:6:e|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:terra-s2:6:e'), 'hu-hu', 'Először jelöléseket és biztonságos kijáratokat teszel: kő-lépcsők, kötél a bejáratnál, időkorlát a visszavonuláshoz.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:p6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:terra-s2'), 'p6', 'page', 7, 'Reușita', 'Lentila iese curat, fără să tulbure grota. Iepurele bate din lăbuțe: „Bravo! Lateral e mai bine decât drept!” Pui Lentila peste cristal, iar imaginea hărții se clarifică. Iepurele te privește hotărât: „Ținem legătura. Vreau acasă, pe Lunaria — promiți că mă ajuți?”\nAcum știți unde e Lunaria: nu, nu e Luna, ci o planetă îndepărtată de Pământ. Dacă am avea o sursă de energie, poate că am putea repara nava împreună…', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/terra-s2/7.success.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "SortOrder" = EXCLUDED."SortOrder",
    "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "ImageUrl" = EXCLUDED."ImageUrl",
    "Question" = EXCLUDED."Question",
    "UpdatedAt" = EXCLUDED."UpdatedAt";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:p6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:p6'), 'ro-ro', 'Reușita', 'Lentila iese curat, fără să tulbure grota. Iepurele bate din lăbuțe: „Bravo! Lateral e mai bine decât drept!” Pui Lentila peste cristal, iar imaginea hărții se clarifică. Iepurele te privește hotărât: „Ținem legătura. Vreau acasă, pe Lunaria — promiți că mă ajuți?”\nAcum știți unde e Lunaria: nu, nu e Luna, ci o planetă îndepărtată de Pământ. Dacă am avea o sursă de energie, poate că am putea repara nava împreună…', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:p6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:p6'), 'en-us', 'Success', 'The Lens comes out cleanly, without disturbing the grotto. The rabbit claps its little paws: "Bravo! Sideways is better than straight!" You place the Lens over the crystal and the map''s image sharpens. The rabbit looks at you, determined: "Let''s keep in touch. I want to go home to Lunaria — do you promise to help?"
Now you know where Lunaria is: not the Moon, but a planet far from Earth. If we had a source of energy, maybe we could repair the ship together…', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:terra-s2:p6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:terra-s2:p6'), 'hu-hu', 'A siker', 'A Lencse tiszta, anélkül, hogy megzavarná a barlangot. A nyuszi tapsol: "Bravó! Oldalirányban jobb, mint egyenesen!" A Lencsét a kristályra teszed, és a térkép képe tisztul. A nyuszi határozottan rád néz: "Tartjuk a kapcsolatot. Haza akarok menni Lunariára — megígéred, hogy segítesz?"
Most már tudjátok, hol van Lunaria: nem, nem a Hold, hanem egy távoli bolygó a Földtől. Ha lenne energiaforrásunk, talán együtt megjavíthatnánk a hajót...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
COMMIT;
