-- Auto-generated from Data/SeedData/Stories/seed@alchimalia.com (mode: main)
-- Run date: 2025-11-27 12:00:21+02:00

BEGIN;

-- Story Definitions
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    ('17c662f8-ff75-5d65-a2d9-3bec69226725', 'crystalia-s1', 'Crystalia', 'images/tol/stories/seed@alchimalia.com/crystalia-s1/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('7ca7d1cd-9054-52c4-9c89-454f9ba445f0', '17c662f8-ff75-5d65-a2d9-3bec69226725', 'ro-ro', 'Crystalia')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('100043de-0a24-565c-86a3-1a22d50ae76c', '17c662f8-ff75-5d65-a2d9-3bec69226725', 'en-us', 'Crystalia')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('05753d70-f5f7-5142-b49d-b2a4e5adc5d5', '17c662f8-ff75-5d65-a2d9-3bec69226725', 'hu-hu', 'Crystalia')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for crystalia-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('c9f5ba7f-d6ba-56a7-bd06-c9e3eedc8d51', '17c662f8-ff75-5d65-a2d9-3bec69226725', '1', 'page', 1, 'Crystalia', 'Crystalia', 'images/tol/stories/seed@alchimalia.com/crystalia-s1/0.Cover.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('612f72dd-1a37-5345-bff8-e9f72b5a68ed', 'c9f5ba7f-d6ba-56a7-bd06-c9e3eedc8d51', 'ro-ro', 'Crystalia', 'Crystalia', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('7613ab19-d199-53ad-a10b-09d0a4fb1849', 'c9f5ba7f-d6ba-56a7-bd06-c9e3eedc8d51', 'en-us', 'Crystalia', 'Crystalia', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('93759377-b0ed-5d1f-b835-53647e8bc569', 'c9f5ba7f-d6ba-56a7-bd06-c9e3eedc8d51', 'hu-hu', 'Crystalia', 'Crystalia', NULL, NULL, NULL)
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
    ('e3bce7d0-1206-5b52-aab8-b83efc70b745', 'intro-pufpuf', 'Marea Călătorie', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('b80ac0c4-b9b4-5147-b9fa-9b559f13a99a', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', 'ro-ro', 'Marea Călătorie')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('fd8d054c-0cfe-5869-895d-09d83745a64b', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', 'en-us', 'The Great Journey')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('efd0533d-ab81-508f-b9c9-da47c359c65a', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', 'hu-hu', 'A Nagy Utazás')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for intro-pufpuf
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('de62b3a0-d9c7-5e24-a662-70b325ac9134', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', '1', 'page', 1, 'Chemarea', 'Puf-Puf călătorea prin adâncul universului când liniștea cosmică fu spartă de un vuiet straniu, ca un oftat al stelelor. \nInstrumentele pâlpâiră, iar busola stelară desenă spirale fără sens. \nCu lăbuța pe parbrizul de cuarț, el simți mai mult, decât auzi, că cineva îl cheamă pe nume dintr-un loc foarte, foarte vechi.', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/1.puf-puf-flying.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('551237ee-2995-535d-8452-c210d1ec4e49', 'de62b3a0-d9c7-5e24-a662-70b325ac9134', 'ro-ro', 'Chemarea', 'Puf-Puf călătorea prin adâncul universului când liniștea cosmică fu spartă de un vuiet straniu, ca un oftat al stelelor. \nInstrumentele pâlpâiră, iar busola stelară desenă spirale fără sens. \nCu lăbuța pe parbrizul de cuarț, el simți mai mult, decât auzi, că cineva îl cheamă pe nume dintr-un loc foarte, foarte vechi.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('02988e52-29a4-51a4-bf74-feb4f0bf96b9', 'de62b3a0-d9c7-5e24-a662-70b325ac9134', 'en-us', 'The Call', 'Puf-Puf was traveling through the depths of the universe when the cosmic silence was broken by a strange roar, like a sigh of the stars. 
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
    ('4f3496da-5db4-5c4d-b475-8cd4d6b3d90e', 'de62b3a0-d9c7-5e24-a662-70b325ac9134', 'hu-hu', 'A hívás', 'Puf-Puf az univerzum mélységében utazott, amikor a kozmikus csendet egy furcsa zúgás törte meg — mint a csillagok sóhaja. A műszerek villogtak, a csillagiránytű értelmetlen spirálokat rajzolt. A mancsával a kvarc szélvédőn érezte (inkább, mint hallotta), hogy valaki a nevén szólítja egy nagyon, nagyon régi helyről.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('be77d5d9-0050-522e-9f3f-f21fcca909f9', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', '2', 'page', 2, 'Prăbușirea', 'Un fulger de lumină înghiți nava și o azvârli într-un vârtej orbitor. Paratrăsnetele de plasmă se topiră, cârma se blocă, iar carena spintecă norii înainte să muște din pământul uscat. Când tăcerea reveni, aerul mirosea a uscăciune și arșiță. \nPuf-Puf își atinse urechea zgâriată, privi copacii uriași și… nu știa unde se afla, de unde a plecat și de ce. Doar că ajunsese într-un loc străin care îi stârnea curiozitatea.', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/2.puf-puf-hurt.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9f69f49b-2b3b-5580-bee9-5b11b4fc5a50', 'be77d5d9-0050-522e-9f3f-f21fcca909f9', 'ro-ro', 'Prăbușirea', 'Un fulger de lumină înghiți nava și o azvârli într-un vârtej orbitor. Paratrăsnetele de plasmă se topiră, cârma se blocă, iar carena spintecă norii înainte să muște din pământul uscat. Când tăcerea reveni, aerul mirosea a uscăciune și arșiță. \nPuf-Puf își atinse urechea zgâriată, privi copacii uriași și… nu știa unde se afla, de unde a plecat și de ce. Doar că ajunsese într-un loc străin care îi stârnea curiozitatea.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('336d5863-9a21-5385-b955-d9ff231135dc', 'be77d5d9-0050-522e-9f3f-f21fcca909f9', 'en-us', 'The Crash', 'A flash of light swallowed the ship and hurled it into a blinding whirl. The plasma lightning rods melted, the rudder jammed, and the hull slit the clouds before biting into the dry earth. When silence returned, the air smelled of dryness and scorching heat. 
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
    ('1f4c116e-cec5-56c8-bc2d-fdb1016a1abe', 'be77d5d9-0050-522e-9f3f-f21fcca909f9', 'hu-hu', 'A zuhanás', 'Egy villanásnyi fény elnyelte a hajót és egy vakító örvénybe hajította. A plazma villámhárítók megolvadtak, a kormány beragadt, és a hajótest szétvágta a felhőket, mielőtt beleharapott a száraz földbe. Amikor a csend visszatért, a levegő szárazság és perzselő hőség szagát árasztotta. Puf-Puf megérintette a megkarcolt fülét, a hatalmas fákat nézte, és… nem tudta, hol van, honnan jött, és miért. Csak annyit tudott, hogy egy idegen helyre érkezett, ami felkeltette a kíváncsiságát.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('2972afcf-eabd-5de9-8ef3-7534d06568dd', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', '3', 'page', 3, 'Cristalul SOS', 'La bord, în modulul SOS, un cristal străvechi pulsa ca o inimă adormită. \nPuf-Puf deschise panoul, iar lumina cristalului proiectă în aer o hartă parcă vie.\nRamuri de lumină, poteci care se împleteau la infinit, unele căi erau stinse, altele scânteiau ca semne. \nToate convergeau într-un singur nod: Copacul Luminii.', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/3.puf-puf-crystal.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9608e828-6874-5f91-a894-e6120d4c4e39', '2972afcf-eabd-5de9-8ef3-7534d06568dd', 'ro-ro', 'Cristalul SOS', 'La bord, în modulul SOS, un cristal străvechi pulsa ca o inimă adormită. \nPuf-Puf deschise panoul, iar lumina cristalului proiectă în aer o hartă parcă vie.\nRamuri de lumină, poteci care se împleteau la infinit, unele căi erau stinse, altele scânteiau ca semne. \nToate convergeau într-un singur nod: Copacul Luminii.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('feba725b-3e89-5e10-99f6-ec94fe3b4c24', '2972afcf-eabd-5de9-8ef3-7534d06568dd', 'en-us', 'The SOS Crystal', 'On board, in the SOS module, an ancient crystal pulsed like a sleeping heart. 
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
    ('7dc4c40c-f8f0-5acb-a358-443d52e087d8', '2972afcf-eabd-5de9-8ef3-7534d06568dd', 'hu-hu', 'Az SOS kristály', 'A fedélzeten, az SOS modulban egy ősi kristály dobbant, mint egy alvó szív. Puf-Puf kinyitotta a panelt, és fénye egy majdnem élő térképet vetített a levegőbe: fény ágak, ösvények, amelyek a végtelenségig fonódtak. Néhány út kialudt, mások szikráztak, mint jelek. Mindegyik egyetlen csomópontba futott össze: a Fény Fájába.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('11e56ef3-da34-5875-88ee-761ced7c93e0', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', '4', 'page', 4, 'Întâlnirea', 'Frunzele Copacului foșneau ca o șoaptă când Puf-Puf îți iese în cale și îți zâmbește parcă amintindu-și ceva, puțin câte puțin. Puf-Puf „-Hei, tu. Da, ție îți vorbesc. Eu sunt Puf-Puf, o pisică vorbitoare de pe planeta Kelo-Ketis... nava mea e puțin șifonată, moralul însă e încă sus. \nCopacul ne-a pus față în față și, sincer, tu pari genul care să știe ce face. Adică ai privirea aia de ai mai reparat două-trei nave spațiale marțea seara. \nMă poți ajuta, te rog, să îmi repar nava și să mă întorc acasă? Cristalul este cheia. Trebuie să îl descifrăm.\nCristalul părea să proiecteze într-o hologramă, aproape vie, un copac luminos, plin de mistere. ', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/puf-puf-home-planet.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e8b1ebc5-0293-515b-8798-2c252631457d', '11e56ef3-da34-5875-88ee-761ced7c93e0', 'ro-ro', 'Întâlnirea', 'Frunzele Copacului foșneau ca o șoaptă când Puf-Puf îți iese în cale și îți zâmbește parcă amintindu-și ceva, puțin câte puțin. Puf-Puf „-Hei, tu. Da, ție îți vorbesc. Eu sunt Puf-Puf, o pisică vorbitoare de pe planeta Kelo-Ketis... nava mea e puțin șifonată, moralul însă e încă sus. \nCopacul ne-a pus față în față și, sincer, tu pari genul care să știe ce face. Adică ai privirea aia de ai mai reparat două-trei nave spațiale marțea seara. \nMă poți ajuta, te rog, să îmi repar nava și să mă întorc acasă? Cristalul este cheia. Trebuie să îl descifrăm.\nCristalul părea să proiecteze într-o hologramă, aproape vie, un copac luminos, plin de mistere. ', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4052be9d-6d46-5181-bf83-5ce5b112a4c6', '11e56ef3-da34-5875-88ee-761ced7c93e0', 'en-us', 'The Meeting', 'The Tree''s leaves rustled like a whisper when Puf-Puf steps into your path and smiles as if remembering something, little by little. Puf-Puf "-Hey, you. Yes, I''m talking to you. I''m Puf-Puf, a talking cat from the planet Kelo-Ketis... my ship is a bit crumpled, but my morale is still high. 
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
    ('abb6ee07-781c-5bb8-8bd5-09242b6c6bbc', '11e56ef3-da34-5875-88ee-761ced7c93e0', 'hu-hu', 'A találkozás', 'A Fa levelei suttogva susogtak, amikor Puf-Puf eléd lépett és mosolygott, mintha apránként eszébe jutna valami. „Hé, te. Igen, veled beszélek. Én Puf-Puf vagyok, beszélő macska a Kelo-Ketis bolygóról... a hajóm egy kicsit összegyűrve, de a lelkem még mindig fent van. A Fa szemtől szemben állított minket, és őszintén, te úgy nézel ki, mint aki tudja, mit csinál. Vagyis van az a tekinteted, hogy \"már javítottam két-három űrhajót kedden este\". Segíthetsz nekem, kérlek, hogy megjavítsam a hajómat és hazajussak? A kristály a kulcs. Meg kell fejtenünk.\" A kristály mintha egy majdnem élő hologramban vetített volna egy fényes, titkokkal teli fát.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('39820899-7e12-5c37-9302-c04edd261937', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', '5', 'quiz', 5, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/4.TreeOfLight.png', 'Puf-Puf îți cere ajutorul să vorbești cu cristalul SOS care arată harta. Ce ar fi cel mai important lucru de întrebat la început?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('f07be6c9-c661-56b4-84eb-b43cf0c34ef7', '39820899-7e12-5c37-9302-c04edd261937', 'ro-ro', NULL, NULL, 'Puf-Puf îți cere ajutorul să vorbești cu cristalul SOS care arată harta. Ce ar fi cel mai important lucru de întrebat la început?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('9b5a8ad8-b331-5863-b128-7e2ea7c5b0ef', '39820899-7e12-5c37-9302-c04edd261937', 'en-us', NULL, NULL, 'Puf-Puf asks for your help to speak to the SOS crystal showing the map. What would be the most important thing to ask first?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('a8dc5f45-343d-58c4-a4d0-1e8dab7e8251', '39820899-7e12-5c37-9302-c04edd261937', 'hu-hu', NULL, NULL, 'Puf-Puf azt kéri, hogy segíts beszélni a térképet mutató SOS kristállyal. Mi lenne az első, legfontosabb kérdés?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('1327ed63-949e-5cb3-8c5f-d870a2bfc2ca', '39820899-7e12-5c37-9302-c04edd261937', 'a', 'Ce drum de pe harta cosmică duce cel mai repede acasă?', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('2ddc05c8-dc13-5289-a9d6-e3067a9fe772', '1327ed63-949e-5cb3-8c5f-d870a2bfc2ca', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('4990dcc2-f49b-587a-8ddc-6c0c3a8e7c2d', '1327ed63-949e-5cb3-8c5f-d870a2bfc2ca', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('856358d4-6ac7-5f30-9bd1-95ba958fabb5', '1327ed63-949e-5cb3-8c5f-d870a2bfc2ca', 'ro-ro', 'Ce drum de pe harta cosmică duce cel mai repede acasă?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('c8313e09-41d4-59d4-b0df-89d573793586', '1327ed63-949e-5cb3-8c5f-d870a2bfc2ca', 'en-us', 'Which route on the cosmic map gets us home the fastest?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('2191b4e0-ea76-5338-973d-54556f2355e1', '1327ed63-949e-5cb3-8c5f-d870a2bfc2ca', 'hu-hu', 'Melyik út vezetne a kozmikus térképen a leggyorsabban haza?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('bb3a414c-5161-59d6-ad48-50b0015a61f3', '39820899-7e12-5c37-9302-c04edd261937', 'b', 'Oare câte lumi arată harta?', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('22adb34b-fb62-5f10-9ff1-a8ebb594fab4', 'bb3a414c-5161-59d6-ad48-50b0015a61f3', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('9f88e11d-20d8-5841-b4df-acb64c96238b', 'bb3a414c-5161-59d6-ad48-50b0015a61f3', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f20b7e86-415d-5b5d-8aec-cf9f39f9a5eb', 'bb3a414c-5161-59d6-ad48-50b0015a61f3', 'ro-ro', 'Oare câte lumi arată harta?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f356386f-846c-526a-98df-3d640ea9da30', 'bb3a414c-5161-59d6-ad48-50b0015a61f3', 'en-us', 'How many worlds does the map show?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('7b81756f-3736-5319-ac35-49b76bcd54c1', 'bb3a414c-5161-59d6-ad48-50b0015a61f3', 'hu-hu', 'Hány világot mutat a térkép?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('6efcfeaa-a8b9-5e2e-9f7f-14e1e1ed504f', '39820899-7e12-5c37-9302-c04edd261937', 'c', 'Îmi arăți calea către Kelo-Ketis?', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('ba4f8b84-8cb5-59de-bf99-89ab3e228ecb', '6efcfeaa-a8b9-5e2e-9f7f-14e1e1ed504f', 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('5434fa48-1e2f-552e-ad00-b97c63770926', '6efcfeaa-a8b9-5e2e-9f7f-14e1e1ed504f', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('548d4fcb-5451-5239-8225-17fac910f5d0', '6efcfeaa-a8b9-5e2e-9f7f-14e1e1ed504f', 'ro-ro', 'Îmi arăți calea către Kelo-Ketis?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('bb7149df-6ef8-53b2-a4c4-a5b76bd50ff2', '6efcfeaa-a8b9-5e2e-9f7f-14e1e1ed504f', 'en-us', 'Can you show me the path to Kelo-Ketis?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('8e52b476-44fb-57fc-a6dc-7e0138fbeb0b', '6efcfeaa-a8b9-5e2e-9f7f-14e1e1ed504f', 'hu-hu', 'Meg tudod mutatni az utat Kelo-Ketis felé?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('fc21b9cb-20de-5bfc-8c46-1cd68fd1e5dc', '39820899-7e12-5c37-9302-c04edd261937', 'd', 'Ce aș putea să folosesc pentru a repara nava?', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('58b451eb-892e-51bd-917b-09ba0948cdd0', 'fc21b9cb-20de-5bfc-8c46-1cd68fd1e5dc', 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('72384507-6c55-53ec-9dff-69f4e3b92fe1', 'fc21b9cb-20de-5bfc-8c46-1cd68fd1e5dc', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('2418ba39-28fd-560a-8676-22baa9f001eb', 'fc21b9cb-20de-5bfc-8c46-1cd68fd1e5dc', 'ro-ro', 'Ce aș putea să folosesc pentru a repara nava?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('5504b558-2683-51f4-8179-c824be67ed97', 'fc21b9cb-20de-5bfc-8c46-1cd68fd1e5dc', 'en-us', 'What could I use to repair the ship?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('51e70793-0e0c-5039-ad66-35e7412a7ab7', 'fc21b9cb-20de-5bfc-8c46-1cd68fd1e5dc', 'hu-hu', 'Mit használhatnék a hajó megjavításához?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('5d3705e9-8635-55bf-926a-b35264d28d3f', '39820899-7e12-5c37-9302-c04edd261937', 'e', 'Care e cel mai sigur drum spre casă?', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('158b6ef5-b7cc-5055-a4b4-1a3e6e9e274c', '5d3705e9-8635-55bf-926a-b35264d28d3f', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('d5769216-ed7c-5429-b3c5-4f32a08894aa', '5d3705e9-8635-55bf-926a-b35264d28d3f', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('aec23506-a8de-5923-8a9b-205d9425659b', '5d3705e9-8635-55bf-926a-b35264d28d3f', 'ro-ro', 'Care e cel mai sigur drum spre casă?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('58b05f6f-bf2b-5323-ae96-ed8c1d3cd440', '5d3705e9-8635-55bf-926a-b35264d28d3f', 'en-us', 'What is the safest route home?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e5acf839-7fd7-5ea2-8d0a-e3e9fc71f4c8', '5d3705e9-8635-55bf-926a-b35264d28d3f', 'hu-hu', 'Melyik a legbiztonságosabb út hazafelé?')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('089a4a8e-2778-546c-bab4-c1009875b775', 'e3bce7d0-1206-5b52-aab8-b83efc70b745', '6', 'page', 6, 'Curaj', 'Puf-Puf: - Să mergem mai aproape, o vom rezolva noi. Mă bucur că te-am găsit.\nAmândoi mergeți cu pași mici către copac.', 'images/tol/stories/seed@alchimalia.com/intro-pufpuf/5.puf-puf-tree-of-light.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('38db5d21-a429-5fc4-af03-b1fd150b9a03', '089a4a8e-2778-546c-bab4-c1009875b775', 'ro-ro', 'Curaj', 'Puf-Puf: - Să mergem mai aproape, o vom rezolva noi. Mă bucur că te-am găsit.\nAmândoi mergeți cu pași mici către copac.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('fd4488a5-8602-54dd-9bad-ff74f2d32d05', '089a4a8e-2778-546c-bab4-c1009875b775', 'en-us', 'Courage', 'Puf-Puf: - Let''s get closer, we''ll figure it out. I''m glad I found you.
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
    ('b341439a-4349-5fb5-a898-242501ae078f', '089a4a8e-2778-546c-bab4-c1009875b775', 'hu-hu', 'Bátorság', 'Puf-Puf: \"Menjünk közelebb, rá fogunk jönni. Örülök, hogy megtaláltalak.\" Mindketten apró léptekkel a fához indultatok.', NULL, NULL, NULL)
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
    ('ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', 'loi-intro', 'Originea Păcii', 'images/tol/stories/seed@alchimalia.com/loi-intro/0.cover.png', NULL, NULL, NULL, NULL,
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
    ('e6dbb0de-ab32-5b8b-809f-4af86505b9ca', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', 'ro-ro', 'Originea Păcii')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('53e4ffe2-8b47-54ab-af46-86553bc4f9cf', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', 'en-us', 'The Origin of Peace')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('87d9a7b0-c69d-57f3-acac-162ff2f8bf82', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', 'hu-hu', 'A Béke Eredete')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for loi-intro
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('37a7bb36-8236-5da8-bb5c-fe91241dbf58', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '1', 'page', 1, 'Două lumi, două tabere', 'În Alchimalia existau două lumi foarte apropiate, dar aflate în război. 
Lunaria, planeta iepurilor, cu câmpii de morcovi și vizuini cat vezi cu ochii si  
Kelo-Ketis, planeta pisicilor, condusă de un împărat razboinic. Granițele erau reci, iar podurile, rupte, desi erau atat de aproape pisicile si iepurii erau dusmani.', 'images/tol/stories/seed@alchimalia.com/loi-intro/1.two-worlds.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('aaf6790d-df91-5b08-b8eb-be46ec371d70', '37a7bb36-8236-5da8-bb5c-fe91241dbf58', 'ro-ro', 'Două lumi, două tabere', 'În Alchimalia existau două lumi foarte apropiate, dar aflate în război. 
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
    ('10143478-29e2-5d34-86bd-07ab94a7c15e', '37a7bb36-8236-5da8-bb5c-fe91241dbf58', 'en-us', 'Two Worlds, Two Sides', 'In Alchimalia there were two very close worlds, but at war. 
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
    ('0ae2f9fd-9b54-5fbd-8a6f-099c16c99bb7', '37a7bb36-8236-5da8-bb5c-fe91241dbf58', 'hu-hu', 'Két világ, két tábor', 'Alchimaliában két világ létezett, nagyon közel egymáshoz, de háborúban. Lunaria — a nyuszik bolygója, sárgarépa mezőkkel és üregekkel. Kelo-Ketis — a macskák bolygója, egy császár által vezetve. A határok hidegek voltak, a hidak pedig eltörtek.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('cd1c4edd-f452-579d-84ee-f0a9b879ffd0', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '2', 'page', 2, 'Împăratul Pufus Alchimus', 'Pe Kelo-Ketis domnea Împăratul Pufus Alchimus, un războinic cunoscut pentru cuceriri și pentru setea de luptă. Războiul cu iepurii ținea de mult, iar armele vorbeau mai tare decât poveștile. Împăratul spera că într-o bună zi imperiul său se va întinde pe tot tărâmul Alchimaliei.', 'images/tol/stories/seed@alchimalia.com/loi-intro/2.emperor.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('159c2a4a-47dc-51e2-84bf-43c6d17f795f', 'cd1c4edd-f452-579d-84ee-f0a9b879ffd0', 'ro-ro', 'Împăratul Pufus Alchimus', 'Pe Kelo-Ketis domnea Împăratul Pufus Alchimus, un războinic cunoscut pentru cuceriri și pentru setea de luptă. Războiul cu iepurii ținea de mult, iar armele vorbeau mai tare decât poveștile. Împăratul spera că într-o bună zi imperiul său se va întinde pe tot tărâmul Alchimaliei.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('d993df00-f56d-5b35-91e6-c434a9d146ea', 'cd1c4edd-f452-579d-84ee-f0a9b879ffd0', 'en-us', 'Emperor Pufus Alchimus', 'On Kelo-Ketis ruled Emperor Pufus Alchimus, a warrior known for conquests and thirst for battle. The war with the rabbits had lasted long, and weapons spoke louder than stories. The emperor hoped that one day his empire would extend across all of Alchimalia.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('25e55fe9-8d36-59b0-8313-dcbe3ec54e7a', 'cd1c4edd-f452-579d-84ee-f0a9b879ffd0', 'hu-hu', 'Pufus Alchimus Császár', 'Ketisen Pufus Alchimus Császár uralkodott, egy harcos, aki hódításairól volt ismert. A háború a nyuszikkal már régóta tartott, és a fegyverek hangosabban szóltak, mint a történetek.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('74d88560-7a29-5f43-b70d-492cf86c3a79', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '3', 'page', 3, 'Fiul cel curios', 'Împăratului i s-a născut un fiu: Puf-Puf Alchimus, a fost numit. 
Tatăl voia un luptător, dar Puf-Puf iubea matematica, jocurile cu elemente și visa la alchemie. Era mai mult explorator decât soldat. Petrece tot timpul in laboratorul sau unde incerca diverse combinatii intre elemente si citea o gramada de scrieri si retete stravechi.', 'images/tol/stories/seed@alchimalia.com/loi-intro/3.curious-son.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('0394ac23-03ea-501e-b6c3-ba1e41fe91a6', '74d88560-7a29-5f43-b70d-492cf86c3a79', 'ro-ro', 'Fiul cel curios', 'Împăratului i s-a născut un fiu: Puf-Puf Alchimus, a fost numit. 
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
    ('a448254a-1e36-55d0-b657-a87da8aef464', '74d88560-7a29-5f43-b70d-492cf86c3a79', 'en-us', 'The Curious Son', 'A son was born to the emperor: Puf-Puf Alchimus, he was named. 
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
    ('840e9a0a-acd2-5567-a453-443f6370e9d1', '74d88560-7a29-5f43-b70d-492cf86c3a79', 'hu-hu', 'A kíváncsi fiú', 'A császárnak egy fia született: Puf-Puf Alchimus. Az apa egy harcost akart. De Puf-Puf szerette a matematikát, az elemekkel való játékokat és az alkímiáról álmodott. Inkább felfedező volt, mint katona.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('f017766e-40c5-5c35-9590-86c5463b586f', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '4', 'page', 4, 'Prieteni peste graniță', 'Deși era război, prietenia își găsea drum. Un iepure negru și o pisică albă se întâlneau în secret și își povesteau lumea, se jucau când pe Lunaria, când pe Ketis. Nu semănau a dușmani. Semănau cu doi copii curioși.', 'images/tol/stories/seed@alchimalia.com/loi-intro/4.friends.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('f8d47292-a6b2-5b48-87d0-682ca203224d', 'f017766e-40c5-5c35-9590-86c5463b586f', 'ro-ro', 'Prieteni peste graniță', 'Deși era război, prietenia își găsea drum. Un iepure negru și o pisică albă se întâlneau în secret și își povesteau lumea, se jucau când pe Lunaria, când pe Ketis. Nu semănau a dușmani. Semănau cu doi copii curioși.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4b2b6379-a77b-50cf-9231-975e7e0631b1', 'f017766e-40c5-5c35-9590-86c5463b586f', 'en-us', 'Friends Across the Border', 'Although there was war, friendship found its way. A black rabbit and a white cat met in secret and told each other about their world, playing sometimes on Lunaria, sometimes on Ketis. They didn''t look like enemies. They looked like two curious children.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('12f8ad5f-193f-593d-a3b5-efe7151675b6', 'f017766e-40c5-5c35-9590-86c5463b586f', 'hu-hu', 'Barátok a határon túl', 'Bár háború volt, a barátság megtalálta az útját. Egy fekete nyuszi és egy fehér macska titokban találkoztak és elmesélték egymásnak a világukat. Nem ellenségeknek tűntek. Kíváncsi gyerekeknek tűntek.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('382c6de5-ac2c-5c42-880d-6048c9f1538b', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '5', 'page', 5, 'Piatra din Lunaria', 'Pe Ketis, Puf-Puf i-a surprins privind o piatră verde adusă de pe Lunaria, lucind ca un smarald. 
„ - Ține-ne secretul prieteniei și îți dăm piatra”, i-a șoptit iepurele. 
Puf-Puf a promis.', 'images/tol/stories/seed@alchimalia.com/loi-intro/5.lunaria-stone.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('a5c26e1e-4aed-5739-a9b2-a7653dfd2fe5', '382c6de5-ac2c-5c42-880d-6048c9f1538b', 'ro-ro', 'Piatra din Lunaria', 'Pe Ketis, Puf-Puf i-a surprins privind o piatră verde adusă de pe Lunaria, lucind ca un smarald. 
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
    ('7ea4a6e3-3e3b-56fe-8ace-7e3294c83c4a', '382c6de5-ac2c-5c42-880d-6048c9f1538b', 'en-us', 'The Stone from Lunaria', 'On Ketis, Puf-Puf caught them looking at a green stone brought from Lunaria, gleaming like an emerald. 
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
    ('6448e87f-ecc5-595d-81df-24afb75be54b', '382c6de5-ac2c-5c42-880d-6048c9f1538b', 'hu-hu', 'A Lunaria kő', 'Ketisen Puf-Puf meglepetten látta, ahogy egy zöld követ néz, amit Lunariáról hoztak, smaragdként csillogva.
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
    ('10bdcf2f-a4a5-564d-bfcf-08de4293d3d7', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '6', 'page', 6, 'Licoarea verde-aurie', 'Cu piatra, Puf-Puf a făcut un elixir. Când atingea metalul, acesta devenea aur. Bucurie mare! S-au gandit sa si bea, descoperind ca acesta are un gust bun, a împărțit licoarea cu prietenii — mici înghițituri, multe râsete.', 'images/tol/stories/seed@alchimalia.com/loi-intro/6.elixir.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('38e84a7a-5670-5f75-a3aa-bb84e4b30421', '10bdcf2f-a4a5-564d-bfcf-08de4293d3d7', 'ro-ro', 'Licoarea verde-aurie', 'Cu piatra, Puf-Puf a făcut un elixir. Când atingea metalul, acesta devenea aur. Bucurie mare! S-au gandit sa si bea, descoperind ca acesta are un gust bun, a împărțit licoarea cu prietenii — mici înghițituri, multe râsete.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('a21a7157-2273-57e2-8433-cddc2720d5c4', '10bdcf2f-a4a5-564d-bfcf-08de4293d3d7', 'en-us', 'The Green-Golden Elixir', 'With the stone, Puf-Puf made an elixir. When it touched metal, it became gold. Great joy! They thought to drink it too, discovering that it had a good taste, he shared the elixir with his friends — small sips, lots of laughter.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('d743986e-df67-5a07-b95a-1855d4bf98cc', '10bdcf2f-a4a5-564d-bfcf-08de4293d3d7', 'hu-hu', 'A zöld-arany folyadék', 'A kővel Puf-Puf egy elixírt készített. Amikor a fémet érintette, arannyá vált. Nagy öröm! Megosztotta a folyadékot a barátaival — kis kortyok, sok nevetés.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('51fd5baf-b484-558c-9a52-99145885e1eb', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '7', 'page', 7, 'Somn la umbra copacului', 'Obosiți, au adormit la umbra unui copac foarte înalt. Licoarea s-a scurs încet la rădăcini. Piatra verde încălzea pământul cu o lumină blândă.', 'images/tol/stories/seed@alchimalia.com/loi-intro/7.tree-nap.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('38f4b9b3-fa82-58df-9a47-830e3eac1e84', '51fd5baf-b484-558c-9a52-99145885e1eb', 'ro-ro', 'Somn la umbra copacului', 'Obosiți, au adormit la umbra unui copac foarte înalt. Licoarea s-a scurs încet la rădăcini. Piatra verde încălzea pământul cu o lumină blândă.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('a0add096-be61-5522-bdf9-fcbe989d9066', '51fd5baf-b484-558c-9a52-99145885e1eb', 'en-us', 'Sleep Under the Tree', 'Tired, they fell asleep under a very tall tree. The elixir slowly seeped to the roots. The green stone warmed the earth with a gentle light.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('10d53b81-a71a-57c2-a86f-7da8447c04b5', '51fd5baf-b484-558c-9a52-99145885e1eb', 'hu-hu', 'Alvás a fa árnyékában', 'Fáradtan elaludtak egy nagyon magas fa árnyékában. A folyadék lassan a gyökerekhez csorgott. A zöld kő enyhe fénnyel melegítette a földet.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('76009b73-90bf-517b-857c-56a40eb586e0', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '8', 'page', 8, 'Mâțo-Iepurele', 'Când s-au trezit, lângă ei stătea o ființă ciudată, frumoasă: cu grația unei pisici și blândețea unui iepure. Se uita la ei fără teamă.
- Buna eu sunt Mâțo-Iepurele! Am venit sa aduc Pacea...', 'images/tol/stories/seed@alchimalia.com/loi-intro/8.matso-iepurele.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e8044f40-8703-5235-9755-a4ca26795cb9', '76009b73-90bf-517b-857c-56a40eb586e0', 'ro-ro', 'Mâțo-Iepurele', 'Când s-au trezit, lângă ei stătea o ființă ciudată, frumoasă: cu grația unei pisici și blândețea unui iepure. Se uita la ei fără teamă.
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
    ('e98c5400-5b2c-5315-8ca1-890c06674bcd', '76009b73-90bf-517b-857c-56a40eb586e0', 'en-us', 'Cat-Rabbit', 'When they woke up, a strange, beautiful being stood beside them: with the grace of a cat and the gentleness of a rabbit. It looked at them without fear.
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
    ('82ac21e2-9985-5693-88c3-86232c01a17d', '76009b73-90bf-517b-857c-56a40eb586e0', 'hu-hu', 'Macska-Nyuszi', 'Amikor felébredtek, mellettük egy furcsa, szép lény állt: macska grációval és nyuszi szelídséggel. Félelem nélkül nézett rájuk.
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
    ('65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', 'q1', 'quiz', 9, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/loi-intro/9.quiz.png', 'Piatra, licoarea și prietenia au chemat Mâțo-Iepurele pentru a aduce pacea. ce faceți mai întâi?

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
    ('2a1c3dc7-2eae-59fc-b3d0-7dcffbd6d6a3', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'ro-ro', NULL, NULL, 'Piatra, licoarea și prietenia au chemat Mâțo-Iepurele pentru a aduce pacea. ce faceți mai întâi?

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
    ('e5de8f94-687d-532c-b0e1-bbec8d2c587d', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'en-us', NULL, NULL, 'The stone, the elixir and the friendship have summoned the Cat-Rabbit to bring peace. What do you do first?

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
    ('f2270243-a193-5334-b186-137bef8d2d35', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'hu-hu', NULL, NULL, 'A kő, a folyadék és a barátság Macska-Nyuszit hívta, hogy békét hozzon. mit csinálsz először?

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
    ('fe541365-1a42-5e95-92aa-338eb815bd09', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'a', 'Il vom incuraja', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1545d303-0ea4-5a0f-bcba-6644a075174d', 'fe541365-1a42-5e95-92aa-338eb815bd09', 'ro-ro', 'Il vom incuraja')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('42ed3849-bd8e-5f61-a9e5-9ecdb770a718', 'fe541365-1a42-5e95-92aa-338eb815bd09', 'en-us', 'We will encourage him')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('bf6ea118-1855-5d12-b008-92505af36fd3', 'fe541365-1a42-5e95-92aa-338eb815bd09', 'hu-hu', 'Bátorítjuk')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('928d8c5f-8bb3-5c1d-81af-d66a9b8bed1e', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'b', 'Vom pune elixirul la fiecare copac.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('9f393ee4-cdf0-5daa-be96-9552ea336919', '928d8c5f-8bb3-5c1d-81af-d66a9b8bed1e', 'ro-ro', 'Vom pune elixirul la fiecare copac.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d534bee5-3e9c-57ae-9942-85fea35ff210', '928d8c5f-8bb3-5c1d-81af-d66a9b8bed1e', 'en-us', 'We will pour the elixir at each tree.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a6dacd41-93c9-579c-aade-c27fa4526f93', '928d8c5f-8bb3-5c1d-81af-d66a9b8bed1e', 'hu-hu', 'Minden fára elixírt teszünk.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('b14b3279-7727-5b3f-b18f-70cff9b48d50', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'c', 'Il protejam pe Mâțo-Iepure.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('596da253-d158-563a-9f23-78a90164a046', 'b14b3279-7727-5b3f-b18f-70cff9b48d50', 'ro-ro', 'Il protejam pe Mâțo-Iepure.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('af97dd51-6be9-5821-84f9-92f8d060df04', 'b14b3279-7727-5b3f-b18f-70cff9b48d50', 'en-us', 'We will protect the Cat-Rabbit.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f1d40a0b-964e-542c-969e-2f597364a1ec', 'b14b3279-7727-5b3f-b18f-70cff9b48d50', 'hu-hu', 'Megvédjük Macska-Nyuszit.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('14dc28b2-0fcb-5a06-84c4-74e4a2d81f7b', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'd', 'Plantăm un copac al păcii în ambele lumi.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1b865d36-4041-56c2-9888-48dd8412ebd8', '14dc28b2-0fcb-5a06-84c4-74e4a2d81f7b', 'ro-ro', 'Plantăm un copac al păcii în ambele lumi.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ada998ef-572e-537c-aa3f-3b2f98bb70b2', '14dc28b2-0fcb-5a06-84c4-74e4a2d81f7b', 'en-us', 'We plant a tree of peace in both worlds.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e9cdccc8-bf31-5baa-b662-3cf46e4d9f87', '14dc28b2-0fcb-5a06-84c4-74e4a2d81f7b', 'hu-hu', 'Mindkét világban béke fát ültetünk.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('714c8666-5941-5184-9aa2-bfa566869290', '65a2c419-1efc-59fc-b92b-3f47b7d25bb2', 'e', 'Pastram pacea cu orice pret.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('24ea1a2a-02ae-5a8c-8c4f-a76fe1842030', '714c8666-5941-5184-9aa2-bfa566869290', 'ro-ro', 'Pastram pacea cu orice pret.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ff7b577f-c9bf-5b1c-af31-80e476422948', '714c8666-5941-5184-9aa2-bfa566869290', 'en-us', 'We keep the peace at any cost.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('df14f0c0-1dab-5108-a267-e652ada82d87', '714c8666-5941-5184-9aa2-bfa566869290', 'hu-hu', 'Bármi áron megtartjuk a békét.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('cb639e99-ec58-5a29-ab66-05888019a280', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '9', 'page', 10, 'Mesajul păcii', '„Eu port pacea dintre lumi”, a spus Mâțo-Iepurele. „Din prietenie, piatră și licoare s-a născut un pod, a spus Puf-Puf.
Războaiele se sting când înveți să amesteci bine lucrurile.” 
Puf-Puf a înțeles că alchimia nu e doar despre metale, ci și despre inimi.', 'images/tol/stories/seed@alchimalia.com/loi-intro/10.message.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9f5d19d2-a1f1-5e7a-a075-b5befc0dff12', 'cb639e99-ec58-5a29-ab66-05888019a280', 'ro-ro', 'Mesajul păcii', '„Eu port pacea dintre lumi”, a spus Mâțo-Iepurele. „Din prietenie, piatră și licoare s-a născut un pod, a spus Puf-Puf.
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
    ('b11fc097-9dbb-56dd-9469-95b37cbd3db1', 'cb639e99-ec58-5a29-ab66-05888019a280', 'en-us', 'Message of Peace', '"I carry peace between worlds," said the Cat-Rabbit. "From friendship, stone and elixir a bridge was born," said Puf-Puf.
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
    ('ff18ec3e-be6f-5d49-9e8f-3494cdef70a7', 'cb639e99-ec58-5a29-ab66-05888019a280', 'hu-hu', 'A béke üzenete', '"Én hordom a békét a világok között", mondta Macska-Nyuszi. "Barátságból, kőből és folyadékból született egy híd", mondta Puf-Puf.
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
    ('4e667263-930d-5cd7-9668-3f3d37f60b59', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '10', 'page', 11, 'Hotărârea lui Puf-Puf', 'În acea zi, Puf-Puf era bucuruos ca alesese sa fie un alchimist. Va căuta căi între lumi, nu războaie. Iar Mâțo-Iepurele avea să devină, cândva, semnul că pacea poate fi creată, nu doar găsită.', 'images/tol/stories/seed@alchimalia.com/loi-intro/11.choice.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('5690dfe6-4b8a-5a23-9133-a739f311b8e1', '4e667263-930d-5cd7-9668-3f3d37f60b59', 'ro-ro', 'Hotărârea lui Puf-Puf', 'În acea zi, Puf-Puf era bucuruos ca alesese sa fie un alchimist. Va căuta căi între lumi, nu războaie. Iar Mâțo-Iepurele avea să devină, cândva, semnul că pacea poate fi creată, nu doar găsită.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('0299e087-c1b4-542c-9950-580f04e9d6e3', '4e667263-930d-5cd7-9668-3f3d37f60b59', 'en-us', 'Puf-Puf''s Decision', 'That day, Puf-Puf was happy that he had chosen to be an alchemist. He would seek ways between worlds, not wars. And the Cat-Rabbit would, someday, become the sign that peace can be created, not just found.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4306f134-4d48-5efb-8205-4ee20553d8e7', '4e667263-930d-5cd7-9668-3f3d37f60b59', 'hu-hu', 'Puf-Puf döntése', 'Aznap Puf-Puf a saját útját választotta: felfedező és alkimista. Világok közötti utakat fog keresni, nem háborúkat. És Macska-Nyuszi egyszer a jelképévé válik, hogy a béke nem csak megtalálható, hanem létrehozható is.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('62b131ca-3f3f-5600-aa93-6640525f3a89', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '12', 'page', 12, 'Vraja', 'Puf-Puf stătea chiar la granița dintre planete când iepurele păși înapoi pe Lunaria, vrând să-l conducă acasă în secret. Când totul era liniștit, o vrajă misterioasă făcu ca Lunaria să se desprindă de Kelo-Ketis și să dispară pentru totdeauna. Mâțo-Iepurele dispăruse și el.

Puf-Puf: „Trebuie să aflu ce s-a întâmplat!” Și, cu pași grăbiți, merse din nou spre copac.', 'images/tol/stories/seed@alchimalia.com/loi-intro/12.the-spell.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e4c71127-4721-503b-867e-e267286dd084', '62b131ca-3f3f-5600-aa93-6640525f3a89', 'ro-ro', 'Vraja', 'Puf-Puf stătea chiar la granița dintre planete când iepurele păși înapoi pe Lunaria, vrând să-l conducă acasă în secret. Când totul era liniștit, o vrajă misterioasă făcu ca Lunaria să se desprindă de Kelo-Ketis și să dispară pentru totdeauna. Mâțo-Iepurele dispăruse și el.

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
    ('50363ca2-e087-59f0-bd99-39c235e92f17', '62b131ca-3f3f-5600-aa93-6640525f3a89', 'en-us', 'The Spell', 'Puf-Puf was standing right at the border between planets when the rabbit stepped back onto Lunaria, wanting to lead him home in secret. When everything was quiet, a mysterious spell caused Lunaria to break away from Kelo-Ketis and disappear forever. The Cat-Rabbit had disappeared too.

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
    ('56d14069-e5d9-5f08-a9e6-2142c643e1f9', '62b131ca-3f3f-5600-aa93-6640525f3a89', 'hu-hu', 'A varázslat', 'Amikor minden csendes volt, egy titokzatos varázslat miatt Lunaria elszakadt Kelo-Ketistől és örökre eltűnt.
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
    ('9a79d3b6-c3a5-513b-9252-3e0b2a6b58cb', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '13', 'page', 13, '...', 'Ruptura dintre lumi era plina de mister, Puf-Puf ajunse repede in dreptul copacului care se transforamsera incredibil in ceva plin de lumina si mister.', 'images/tol/stories/seed@alchimalia.com/loi-intro/13.tobecontinued.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('f01daeeb-aee1-5581-be61-666c95bb9bf8', '9a79d3b6-c3a5-513b-9252-3e0b2a6b58cb', 'ro-ro', '...', 'Ruptura dintre lumi era plina de mister, Puf-Puf ajunse repede in dreptul copacului care se transforamsera incredibil in ceva plin de lumina si mister.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('514d418f-715b-516d-99a7-572191f893c5', '9a79d3b6-c3a5-513b-9252-3e0b2a6b58cb', 'en-us', '...', 'The rupture between worlds was full of mystery, Puf-Puf quickly reached the tree which had transformed incredibly into something full of light and mystery.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('2fd18f0b-8c60-56a3-8b4f-d2c524636484', '9a79d3b6-c3a5-513b-9252-3e0b2a6b58cb', 'hu-hu', '...', 'Folytatjuk...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('a5a7eb7c-2d63-501f-9e67-df6c18d935b7', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '14', 'page', 14, 'Copacul luminii', 'În fața lui Puf-Puf se afla ceva atât de frumos și, totodată, de nerecunoscut. Marele copac devenise plin de lumină, iar din loc în loc se auzeau șoapte.
„Trebuie să cauți pacea... cristalul este cheia...”, se auzi de undeva, din rădăcina copacului.
Puf-Puf săpă la rădăcina copacului și găsi un cristal care părea să comunice cu el.', 'images/tol/stories/seed@alchimalia.com/loi-intro/14.tol.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e44853db-c395-5015-8828-e3fde14a366c', 'a5a7eb7c-2d63-501f-9e67-df6c18d935b7', 'ro-ro', 'Copacul luminii', 'În fața lui Puf-Puf se afla ceva atât de frumos și, totodată, de nerecunoscut. Marele copac devenise plin de lumină, iar din loc în loc se auzeau șoapte.
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
    ('39863120-b3c8-579e-9966-ca6c610fd338', 'a5a7eb7c-2d63-501f-9e67-df6c18d935b7', 'en-us', 'Tree of Light', 'In front of Puf-Puf stood something so beautiful and yet unrecognizable. The great tree had become full of light, and whispers could be heard here and there.
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
    ('94b3eace-489e-53ca-91f5-deb7768ab27e', 'a5a7eb7c-2d63-501f-9e67-df6c18d935b7', 'hu-hu', 'A Fény Fája', 'Puf-Puf előtt valami gyönyörű és mégis felismerhetetlen állt. A nagy fa fénnyel telt meg, és innen-onnan suttogások hallatszottak. "A békét kell keresned... a kristály a kulcs..." — szólt egy hang a gyökerek felől. Puf-Puf a gyökereknél ásott, és talált egy kristályt, amely mintha beszélt volna hozzá.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('7d028b73-9ae8-5d70-ad7f-baf1e8fc6954', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '15', 'page', 15, 'Plecarea', 'Îi spuse tatălui său că trebuie să plece, să exploreze; nu putea să-i spună Împăratului că vrea să aducă tocmai pacea — după atâta muncă de cucerire, acesta era ultimul lucru pe care marele Pufus Alchimus ar fi vrut să-l audă. Se urcă într-una dintre navele spațiale ale Imperiului și plecă.', 'images/tol/stories/seed@alchimalia.com/loi-intro/15.leavingkk.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('f90dcc9a-c6e5-5129-845b-feba0da66b69', '7d028b73-9ae8-5d70-ad7f-baf1e8fc6954', 'ro-ro', 'Plecarea', 'Îi spuse tatălui său că trebuie să plece, să exploreze; nu putea să-i spună Împăratului că vrea să aducă tocmai pacea — după atâta muncă de cucerire, acesta era ultimul lucru pe care marele Pufus Alchimus ar fi vrut să-l audă. Se urcă într-una dintre navele spațiale ale Imperiului și plecă.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('9ea6a2b0-b729-5761-b665-7a9216006b9f', '7d028b73-9ae8-5d70-ad7f-baf1e8fc6954', 'en-us', 'Departure', 'He told his father that he had to leave, to explore; he couldn''t tell the Emperor that he wanted to bring peace — after so much work of conquest, that was the last thing the great Pufus Alchimus would have wanted to hear. He boarded one of the Empire''s spaceships and left.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('5da568ed-9f06-55d2-bfdd-d630f051fc94', '7d028b73-9ae8-5d70-ad7f-baf1e8fc6954', 'hu-hu', 'Indulás', 'Elmondta az apjának, hogy el kell mennie, felfedezni. Nem mondhatta el a császárnak, hogy békét akar hozni — ennyi hódítás után ez volt az utolsó, amit a nagy Pufus Alchimus hallani akart. Felszállt a birodalom egyik űrhajójára és elindult...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('fb7d08a7-8eb6-5882-8675-5cc083161b00', 'ac62cd8e-6fc4-5ee4-9d0b-6e594d2d5369', '16', 'video', 16, 'Crystal', NULL, NULL, NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('7a4ae266-14db-5c3e-9091-4d9f9dc4b349', 'fb7d08a7-8eb6-5882-8675-5cc083161b00', 'ro-ro', 'Crystal', NULL, NULL, NULL, 'video/ro-ro/tol/stories/seed@alchimalia.com/loi-intro/tol-cristal.mp4')
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('7f6a22b1-abf9-597e-a5e4-c93bcf830162', 'fb7d08a7-8eb6-5882-8675-5cc083161b00', 'en-us', 'Crystal', NULL, NULL, NULL, 'video/en-us/tol/stories/seed@alchimalia.com/loi-intro/tol-cristal.mp4')
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('d919232d-1d09-5f30-a450-86efe3742de2', 'fb7d08a7-8eb6-5882-8675-5cc083161b00', 'hu-hu', 'Crystal', NULL, NULL, NULL, 'video/hu-hu/tol/stories/seed@alchimalia.com/loi-intro/tol-cristal.mp4')
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
    ('d41a5784-17ff-5e45-9b97-63b319c351f1', 'lunaria-s1', 'Linkaro', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('1945d1f3-0e2b-59de-95d8-1108b56305ac', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'ro-ro', 'Linkaro')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('838d27b3-82fa-56a9-a793-fe3a48e17dbc', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'en-us', 'Linkaro')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('54520b82-04ed-5467-ba46-73d54f509c7d', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'hu-hu', 'Linkaro')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for lunaria-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('45c3c53f-87d3-5dd5-9f79-7add1e6b9092', 'd41a5784-17ff-5e45-9b97-63b319c351f1', '1', 'page', 1, 'Portalul și Consiliul Iepurilor', '— Ține bine, zise Puf-Puf. Așez lupa peste cristal. Uite pana păsării portocalii, trofeul de pe Terra. E cheia.
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

Strângeți sigiliul, treceți pe sub steagurile în semilună și porniți spre Muntele Negru.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/1.portal-council.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('779ae342-7a1a-58aa-9a79-827cb1872347', '45c3c53f-87d3-5dd5-9f79-7add1e6b9092', 'ro-ro', 'Portalul și Consiliul Iepurilor', '— Ține bine, zise Puf-Puf. Așez lupa peste cristal. Uite pana păsării portocalii, trofeul de pe Terra. E cheia.
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
    ('1d99e887-0d4e-5427-860b-e841f076176a', '45c3c53f-87d3-5dd5-9f79-7add1e6b9092', 'en-us', 'The Portal and the Rabbits'' Council', '— Hold tight, said Puf-Puf. I''m placing the magnifier over the crystal. Look, the feather of the orange bird, the trophy from Terra. It''s the key.
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
    ('ea131b22-8262-54de-8150-84f4841092ba', '45c3c53f-87d3-5dd5-9f79-7add1e6b9092', 'hu-hu', 'A portál és a Nyuszik Tanácsa', '— Kapaszkodj jól, mondta Puf-Puf. Ráhelyezem a nagyítót a kristályra. Nézd, a narancssárga madár tollát, a trófeát a Teráról. Ez a kulcs.
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
    ('d752b10c-27b0-58ff-8b57-945727ad3b92', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'p2', 'page', 2, 'Drumul spre Muntele Negru', 'Poteca urcă greu: pietrele fug de sub talpă, iar vântul ba vine din față, ba te împinge din spate. Ceața se adună pe colțurile stâncilor și parcă mușcă din marginea potecii. Puf-Puf merge lipit de stâncă, cu cristalul la piept, iar iepurele negru își testează sprijinul la fiecare pas.

— Ține-te de mine, zice Puf-Puf. Aici alunecă.
— Știu ocolul, răspunde iepurele. Încă zece pași și scăpăm de panta asta.
— Respiră rar și apasă talpa întreagă, adaugi tu.

După panta abruptă, vântul se liniștește ca la un semn. Ceața se rupe în fâșii și apare mănăstirea: ziduri albe, linii simple, curate, ca desenate cu rigla. Deasupra porții, stegulețe cu semilună foșnesc ușor, fără să facă zgomot.

— Am ajuns, șoptește iepurele, cu o ușurare vizibilă.
— Frumos loc, spune Puf-Puf. Parcă te cheamă să intri.
— Hai, bate tu la poartă, zici. Eu țin sigiliul pregătit.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/2.white-mountain-road.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('55493e4e-4acd-527f-975a-e35151656a12', 'd752b10c-27b0-58ff-8b57-945727ad3b92', 'ro-ro', 'Drumul spre Muntele Negru', 'Poteca urcă greu: pietrele fug de sub talpă, iar vântul ba vine din față, ba te împinge din spate. Ceața se adună pe colțurile stâncilor și parcă mușcă din marginea potecii. Puf-Puf merge lipit de stâncă, cu cristalul la piept, iar iepurele negru își testează sprijinul la fiecare pas.

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
    ('6da6102b-69f3-58f3-bb2d-1f3812eb02ff', 'd752b10c-27b0-58ff-8b57-945727ad3b92', 'en-us', 'Road to Black Mountain', 'The path climbs hard: stones slip from under your feet, and the wind comes from the front, then pushes you from behind. Fog gathers on the corners of the rocks and seems to bite at the edge of the path. Puf-Puf walks close to the rock, with the crystal at his chest, while the black rabbit tests his footing with each step.

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
    ('05eb4158-7384-5562-ad33-dc18dc1542a0', 'd752b10c-27b0-58ff-8b57-945727ad3b92', 'hu-hu', 'Az út a Fekete Hegy felé', 'Az ösvény nehezen emelkedik: a kövek csúsznak a talp alól, a szél előbb elöl jön, aztán hátulról tol. A köd a sziklák sarkain gyűlik össze és mintha harapna az ösvény széléről. Puf-Puf a sziklához simulva jár, a kristályt a mellkasán, míg a fekete nyuszi minden lépésnél teszteli a támaszt.

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
    ('6b096a1b-043c-5ca0-957e-00186ece7294', 'd41a5784-17ff-5e45-9b97-63b319c351f1', '3', 'page', 3, 'Sala Liniștii', 'În curte, călugări pisici și iepuri se antrenează în tăcere. Le spuneți despre boala din oraș. Se privesc mirați: n-au auzit.\n„Trebuie să vorbiți cu marele maestru”, spune un călugăr și vă conduce într-o încăpere luminoasă. O siluetă stă cu spatele, în meditație.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/3.silent-hall.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('a9e6fe12-2c80-5dee-a294-d2696043cb31', '6b096a1b-043c-5ca0-957e-00186ece7294', 'ro-ro', 'Sala Liniștii', 'În curte, călugări pisici și iepuri se antrenează în tăcere. Le spuneți despre boala din oraș. Se privesc mirați: n-au auzit.\n„Trebuie să vorbiți cu marele maestru”, spune un călugăr și vă conduce într-o încăpere luminoasă. O siluetă stă cu spatele, în meditație.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('eb898ec9-1716-5a89-821a-eb4bc22b1712', '6b096a1b-043c-5ca0-957e-00186ece7294', 'en-us', 'The Hall of Silence', 'In the courtyard, cat and rabbit monks train in silence. You tell them about the illness in the city. They look at each other in surprise: they haven''t heard.
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
    ('639b6573-cfea-57b7-aa64-8634756a81f7', '6b096a1b-043c-5ca0-957e-00186ece7294', 'hu-hu', 'A Csend Csarnoka', 'Az udvaron macska és nyuszi szerzetesek csendben gyakorolnak. Elmesélitek nekik a város betegségét. Csodálkozva néznek egymásra: nem hallottak róla.
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
    ('e4b9bcba-b03c-5ae4-939f-1dbdf9a7badf', 'd41a5784-17ff-5e45-9b97-63b319c351f1', '4', 'page', 4, 'Linkaro, Mâțo-Iepurele', '— E… pisică? șoptește Puf-Puf.
— Sau iepure? întrebi tu.
— Linkaro, zâmbește ea. Sunt un Mâțo-Iepure.

— Știu specia, dă din cap Puf-Puf. Am întâlnit unul pe vremea războaielor dintre pisici și iepuri.
— Războaiele s-au încheiat, spune calm Linkaro. Eu am semnat ultimul armistițiu. Spuneți problema.

Îi descrieți boala din oraș.

— Vă trebuie Pietrele Vieții, cu esență de vindecare, spune Linkaro. Le găsiți sub o streașină de stâncă — o cornișă expusă. Culegeți-le cu grijă, doar pe cele care lucesc ușor.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/4.linkaro.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('4b41965c-1c08-5532-9ab5-a5a531a0320f', 'e4b9bcba-b03c-5ae4-939f-1dbdf9a7badf', 'ro-ro', 'Linkaro, Mâțo-Iepurele', '— E… pisică? șoptește Puf-Puf.
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
    ('02a851e6-636b-511f-9f1c-2df350121428', 'e4b9bcba-b03c-5ae4-939f-1dbdf9a7badf', 'en-us', 'Linkaro, the Cat-Rabbit', '— Is it... a cat? whispers Puf-Puf.
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
    ('4966188e-95d0-55b6-b2da-be38a10e8c05', 'e4b9bcba-b03c-5ae4-939f-1dbdf9a7badf', 'hu-hu', 'Linkaro, Macska-Nyuszi', '— Ez... macska? suttogja Puf-Puf.
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
    ('00318a04-aa49-5dda-a0eb-3ad34d406301', 'd41a5784-17ff-5e45-9b97-63b319c351f1', '5', 'page', 5, 'Pietrele Vieții', 'Pe o creastă îngustă, sub cornișă, licăresc pietre verzi, ca niște inimioare. Curentul rece le slăbește pulsul la orice mișcare bruscă.

— Aici contează cum te apropii, șoptește Puf-Puf. Pietrele de pe Lunaria se sting dacă le sperii.
— Mergem pe rând, cu pași rari, zici tu.
— Țin vântul în urechi și vă dau semn când tace, adaugă iepurele negru.

Când rafala se oprește, vă aplecați încet.
— Ține punguța deschisă, spune Puf-Puf. Nu le atinge. Lasă-le să cadă singure.
— Acum, șoptești. Ușor.

Două pietre se desprind blând și cad în pânză. A treia licărește mai tare, apoi se liniștește.
— Trei ajung, zice Puf-Puf. Restul rămân muntelui.
— Să nu le slăbim suflul, încuviințează iepurele. Plecăm.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/5.life-stones.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('7343d950-b86d-5dca-9a25-0acfe2f20b18', '00318a04-aa49-5dda-a0eb-3ad34d406301', 'ro-ro', 'Pietrele Vieții', 'Pe o creastă îngustă, sub cornișă, licăresc pietre verzi, ca niște inimioare. Curentul rece le slăbește pulsul la orice mișcare bruscă.

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
    ('455ae67e-19db-5310-84ba-68a3ee99113c', '00318a04-aa49-5dda-a0eb-3ad34d406301', 'en-us', 'The Stones of Life', 'On a narrow ridge, under the ledge, green stones glimmer like little hearts. The cold current weakens their pulse at any sudden movement.

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
    ('aecde5c6-999e-5857-8271-a3133ae360a1', '00318a04-aa49-5dda-a0eb-3ad34d406301', 'hu-hu', 'Az Élet Kövei', 'Egy keskeny gerincen, a párkány alatt zöld kövek csillognak, mint kis szívek. A hideg légáramlat gyengíti a pulzusukat bármilyen hirtelen mozgásnál.

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
    ('cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'd41a5784-17ff-5e45-9b97-63b319c351f1', '6', 'quiz', 6, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/lunaria-s1/6.quiz.png', 'Pietrele verzi pulsează sub cornișă. Nu cer forță, ci felul în care te apropii. Cum le culegi fără să le stingi lumina?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('49ae0b8a-3de6-56d3-a168-7f939bba580c', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'ro-ro', NULL, NULL, 'Pietrele verzi pulsează sub cornișă. Nu cer forță, ci felul în care te apropii. Cum le culegi fără să le stingi lumina?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('abb6706e-fdce-5379-8251-727422476fd1', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'en-us', NULL, NULL, 'The green stones pulse under the ledge. They don''t ask for force, but for how you approach. How do you gather them without extinguishing their light?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('7b48c8fd-b080-5b28-8f8c-ff5a5ca992d8', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'hu-hu', NULL, NULL, 'A zöld kövek pulzálnak a párkány alatt. Nem erőt kérnek, hanem a megközelítés módját. Hogyan szeded le őket anélkül, hogy kioltnád a fényüket?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('ae6066ec-61bc-549f-b64f-e1205e8fe332', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'a', 'O mișcare hotărâtă.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('2e36dff7-ac71-5b41-9d3c-3f99675b63c8', 'ae6066ec-61bc-549f-b64f-e1205e8fe332', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('3782ab97-1a06-5b80-9f5a-b49ddd8e1ccf', 'ae6066ec-61bc-549f-b64f-e1205e8fe332', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('272d03e2-067e-5d3c-ac90-61dfac6e0d7b', 'ae6066ec-61bc-549f-b64f-e1205e8fe332', 'ro-ro', 'O mișcare hotărâtă.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ddf6d1d2-a6cb-54cb-8e4a-9b0553f8273b', 'ae6066ec-61bc-549f-b64f-e1205e8fe332', 'en-us', 'A decisive move.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e358e3bd-dda4-50f1-9349-6c1d28b9677c', 'ae6066ec-61bc-549f-b64f-e1205e8fe332', 'hu-hu', 'Határozott mozgás.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('9e976328-e2d6-553d-913f-27e5300a6f4f', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'b', 'Ascult pulsul, apoi ating.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('040c55cb-013d-505f-a8c0-43e341de2c3b', '9e976328-e2d6-553d-913f-27e5300a6f4f', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('24a8adbd-b6f2-5517-ba97-37c220482468', '9e976328-e2d6-553d-913f-27e5300a6f4f', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('c07fda63-c2a5-511e-966e-7fde59264c02', '9e976328-e2d6-553d-913f-27e5300a6f4f', 'ro-ro', 'Ascult pulsul, apoi ating.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('8c5a20be-5dec-5d8e-9618-bee19a4ef5e0', '9e976328-e2d6-553d-913f-27e5300a6f4f', 'en-us', 'Listen to the pulse, then touch.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('76e316e6-c24f-5778-b0b4-8715759d6f54', '9e976328-e2d6-553d-913f-27e5300a6f4f', 'hu-hu', 'Hallgatom a pulzust, aztán érintem.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('6f05192e-566a-53de-afeb-4dc660b709d4', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'c', 'Sincronizez ca un ceasornicar.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('98522f02-99fb-53f4-a110-f67ba70fe2ff', '6f05192e-566a-53de-afeb-4dc660b709d4', 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('aaf8a349-367f-5cbc-8b47-7bdb3f6d8474', '6f05192e-566a-53de-afeb-4dc660b709d4', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1088cc27-7a82-5ee7-9ae2-e92e99f5a442', '6f05192e-566a-53de-afeb-4dc660b709d4', 'ro-ro', 'Sincronizez ca un ceasornicar.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('292795e7-bfde-521c-827c-4b047113151d', '6f05192e-566a-53de-afeb-4dc660b709d4', 'en-us', 'Synchronize like a watchmaker.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('03dd5f0d-864a-5f9b-a5f0-44108c8c8efa', '6f05192e-566a-53de-afeb-4dc660b709d4', 'hu-hu', 'Órási módon szinkronizálok.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('d72b0aa6-4251-5ff8-b991-b63ea6cab0ef', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'd', 'Buclă moale, fără atingere.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('7b8dfab5-bcf5-5d06-9930-08d98eda4a57', 'd72b0aa6-4251-5ff8-b991-b63ea6cab0ef', 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('a2cab842-0212-5c5f-8bda-02e22319ca3d', 'd72b0aa6-4251-5ff8-b991-b63ea6cab0ef', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('eeae61cc-8d7e-571e-98eb-114ffe3f20c0', 'd72b0aa6-4251-5ff8-b991-b63ea6cab0ef', 'ro-ro', 'Buclă moale, fără atingere.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e25d714e-be68-59b4-b21a-0967d0876835', 'd72b0aa6-4251-5ff8-b991-b63ea6cab0ef', 'en-us', 'Soft loop, no touch.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a35846a4-660d-53a9-80f1-9210899f07a0', 'd72b0aa6-4251-5ff8-b991-b63ea6cab0ef', 'hu-hu', 'Puha hurok, érintés nélkül.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('1ceb0a75-ced2-540e-9fd7-404c43402df3', 'cfca90bc-cf10-5ad8-8c31-29c53e05eea6', 'e', 'Pași calmi, ritm constant.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('e25a020d-315b-5346-9ad6-9167e15f9d86', '1ceb0a75-ced2-540e-9fd7-404c43402df3', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('3623f590-c3fc-55b6-ad33-70f4ab340b87', '1ceb0a75-ced2-540e-9fd7-404c43402df3', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('74268c47-e7b9-5e5f-9aeb-6b3906759e0e', '1ceb0a75-ced2-540e-9fd7-404c43402df3', 'ro-ro', 'Pași calmi, ritm constant.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('2b8d1db2-9646-595c-abd7-874306192669', '1ceb0a75-ced2-540e-9fd7-404c43402df3', 'en-us', 'Calm steps, steady rhythm.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('3ed0e711-4e25-58cc-9965-0790aa5ab1ab', '1ceb0a75-ced2-540e-9fd7-404c43402df3', 'hu-hu', 'Nyugodt lépések, állandó ritmus.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('e320052f-6b9b-56ec-a1ef-8f88bfc0ed6a', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'p6', 'page', 7, 'Rețeta Linkaro', 'Înapoi în sala liniștită, Linkaro vorbește rar, ca să notați bine:

— Apă la foc mic, să fiarbă blând.
— Mult morcov, tăiat foarte fin. Ierburi locale — o mână mică ajunge.
— Pietrele Vieții nu se pun în oală. Le așezați sub vas, ca un cuib. Ele dau esența prin căldură.
— Când aburul capătă o sclipire verde, opriți focul. E gata.

— Simplu și curat, zice Puf-Puf.
— Clar, adaugi tu. Începem chiar acum.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/7.recipe.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('48425f15-007e-51f7-b61d-c4073e62ef78', 'e320052f-6b9b-56ec-a1ef-8f88bfc0ed6a', 'ro-ro', 'Rețeta Linkaro', 'Înapoi în sala liniștită, Linkaro vorbește rar, ca să notați bine:

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
    ('2d2b623b-e9e7-52b3-a665-bd2f6d5afda9', 'e320052f-6b9b-56ec-a1ef-8f88bfc0ed6a', 'en-us', 'Linkaro''s Recipe', 'Back in the quiet hall, Linkaro speaks rarely, so you can note well:

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
    ('31ddebe7-366b-57ec-bb07-8474066f7e26', 'e320052f-6b9b-56ec-a1ef-8f88bfc0ed6a', 'hu-hu', 'Linkaro receptje', 'Vissza a csendes csarnokba, Linkaro ritkán beszél, hogy jól jegyezzétek:

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
    ('a420a196-3512-57ad-8254-b23e036a116f', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'p7', 'page', 8, 'Supa în oraș', 'Vă întoarceți la Consiliu. În piață, puneți la mijloc vasul mare; Puf-Puf așază cu grijă pietrele verzi sub el, ca într-un cuib. Apa începe să freamăte la foc mic, morcovul tăiat fin și ierburile locale se lasă ușor în oală.

— Ținem regula Linkaro, zice Puf-Puf. Pietrele rămân sub vas.
— Așteptăm aburul verde, adaugi tu. Încet, fără grabă.

Când aburul prinde o nuanță smarald, turnați în căni mici.
— Pe rând, îi îndeamnă iepurele negru. Un gât, apoi respirați.

Iepurii bolnavi beau câte o înghițitură. Urechile li se ridică, pieptul se liniștește, ochii capătă luciu. În piață se aude din nou foșnetul pașilor, râsete scurte, vorbă domoală. Steagurile în semilună fâlfâie ușor — murmurul străzii revine.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/8.soup-city.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('014213d8-2dbb-5796-8cff-650eab99686f', 'a420a196-3512-57ad-8254-b23e036a116f', 'ro-ro', 'Supa în oraș', 'Vă întoarceți la Consiliu. În piață, puneți la mijloc vasul mare; Puf-Puf așază cu grijă pietrele verzi sub el, ca într-un cuib. Apa începe să freamăte la foc mic, morcovul tăiat fin și ierburile locale se lasă ușor în oală.

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
    ('65c314d4-f020-584b-aaeb-a12f3490a72c', 'a420a196-3512-57ad-8254-b23e036a116f', 'en-us', 'Soup in the City', 'You return to the Council. In the square, you place the big vessel in the middle; Puf-Puf carefully places the green stones under it, like in a nest. The water begins to simmer on low heat, the finely cut carrot and local herbs settle gently into the pot.

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
    ('1ad45245-6291-57dd-aba7-19ecad86204f', 'a420a196-3512-57ad-8254-b23e036a116f', 'hu-hu', 'Leves a városban', 'Visszatértek a Tanácshoz. A téren a közepére teszitek a nagy edényt; Puf-Puf óvatosan a zöld köveket alá helyezi, mint egy fészket. A víz kis lángon kezd bugyogni, a finomra vágott sárgarépa és a helyi füvek finoman leülepednek a fazékba.

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
    ('94bd4fae-c7bc-5331-8be8-6fd6670b359a', 'd41a5784-17ff-5e45-9b97-63b319c351f1', 'p8', 'page', 9, 'Semne de vindecare', 'Spre seară, cartierele se liniștesc. Un pui de iepure prinde curaj și face primul salt, fără ezitare. Piața parcă respiră ușor.

Linkaro vă însoțește până la marginea pieței.
— Ați readus echilibrul de bază. Mulțumesc, spune ea.

— Notăm, zice Puf-Puf: Lunaria răspunde bine la grijă și la plan.
— Iar pasul următor îl alegem împreună, adaugi tu.
Iepurele negru dă din cap:
— Dimineață, stabilim drumul.', 'images/tol/stories/seed@alchimalia.com/lunaria-s1/9.healing.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('24586aa3-f7bd-5bcb-977c-c612f9e5e33b', '94bd4fae-c7bc-5331-8be8-6fd6670b359a', 'ro-ro', 'Semne de vindecare', 'Spre seară, cartierele se liniștesc. Un pui de iepure prinde curaj și face primul salt, fără ezitare. Piața parcă respiră ușor.

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
    ('f47e16d5-e8c3-52a4-957e-eecc04be926d', '94bd4fae-c7bc-5331-8be8-6fd6670b359a', 'en-us', 'Signs of Healing', 'Toward evening, the neighborhoods calm down. A rabbit kit gains courage and makes the first jump, without hesitation. The square seems to breathe lightly.

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
    ('ed3ed876-a700-598c-b197-797fad36afd3', '94bd4fae-c7bc-5331-8be8-6fd6670b359a', 'hu-hu', 'Gyógyulás jelei', 'Estére a negyedek megnyugszanak. Egy nyuszi kölyök bátorságot kap és az első ugrást teszi, habozás nélkül. A tér mintha könnyedén lélegezne.

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
    ('8543adc1-309f-53d9-a28c-baea44e2683d', 'lunaria-s2', 'Semnalul', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('5aeada60-3801-55f6-a634-7ba856a67fcd', '8543adc1-309f-53d9-a28c-baea44e2683d', 'ro-ro', 'Semnalul')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('ff31915c-95e6-5bb3-ad0f-19acabd4a855', '8543adc1-309f-53d9-a28c-baea44e2683d', 'en-us', 'The Signal')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('b86090f2-fc0c-53e1-885a-85381437344f', '8543adc1-309f-53d9-a28c-baea44e2683d', 'hu-hu', 'A jel')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for lunaria-s2
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('6650bed5-6701-563c-9062-a3d262af4dfa', '8543adc1-309f-53d9-a28c-baea44e2683d', '1', 'page', 1, 'Aterizare la marginea orașului', 'Portalul se stinge în spatele vostru. În vale se întind câmpuri portocalii de morcovi, dar e o liniște ciudată.
Puf-Puf ridică lupa spre cristal:
— Prea liniște pentru Lunaria. Ținem urechile ciulite.
Iepurele negru încuviințează:
— Mergem încet.', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/1.arrival.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('af2a8510-47eb-5220-9f5a-2ad160185438', '6650bed5-6701-563c-9062-a3d262af4dfa', 'ro-ro', 'Aterizare la marginea orașului', 'Portalul se stinge în spatele vostru. În vale se întind câmpuri portocalii de morcovi, dar e o liniște ciudată.
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
    ('1ae74621-1564-58aa-96b4-8ac8c8655d5d', '6650bed5-6701-563c-9062-a3d262af4dfa', 'en-us', 'Landing at the edge of town', 'The portal fades behind you. In the valley stretch orange carrot fields, but there''s a strange silence.
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
    ('ece528cf-c9db-54e7-ad46-e4df3d752923', '6650bed5-6701-563c-9062-a3d262af4dfa', 'hu-hu', 'Leszállás a város szélén', 'A portál kialszik mögöttetek. A völgyben narancssárga sárgarépa mezők terülnek el, de furcsa csend van.
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
    ('38b34565-7bbd-5be7-a135-971c47e1caa0', '8543adc1-309f-53d9-a28c-baea44e2683d', '2', 'page', 2, 'Ping din campiile de morcovi', 'Cristalul pâlpâie ritmic, ca un ping îndepărtat. Printre rândurile de morcovi, o scânteie se aprinde o clipă și se stinge.
— Semnal tehnic, nu animal, zice Puf-Puf. Acolo.
Iepurele negru își rotește urechile:
— Nu e licurici. Sună ca o baliză.
Vă aplecați și înaintați pe două rânduri paralele, cu pași scurți. În pământ se văd urme fine și un fir subțire, îngropat pe jumătate. Ping-ul revine: mai lent, apoi iarăși scurt.
— Tu rămâi pe stânga, spune Puf-Puf. Eu intru din dreapta.
— Când ridici laba, mă opresc, zici tu.
— Fix. Ne apropiem încet, fără să atingem cablul.', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/2.ping.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('911aa3c5-06c3-5e4c-af4e-2dcb7eb503c8', '38b34565-7bbd-5be7-a135-971c47e1caa0', 'ro-ro', 'Ping din campiile de morcovi', 'Cristalul pâlpâie ritmic, ca un ping îndepărtat. Printre rândurile de morcovi, o scânteie se aprinde o clipă și se stinge.
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
    ('3d9fe0a0-ab13-541d-88eb-1deb886981a3', '38b34565-7bbd-5be7-a135-971c47e1caa0', 'en-us', 'Ping from the carrot fields', 'The crystal flickers rhythmically, like a distant ping. Among the carrot rows, a spark lights up briefly and goes out.
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
    ('1725f1e5-8d87-5a08-b555-81b719be1791', '38b34565-7bbd-5be7-a135-971c47e1caa0', 'hu-hu', 'Ping a sárgarépa mezőkből', 'A kristály ritmikusan villog, mint egy távoli ping. A sárgarépa sorok között egy szikra rövid ideig felvillan és kialszik.
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
    ('73e87fe6-7508-5b21-b498-b1468f01b6b7', '8543adc1-309f-53d9-a28c-baea44e2683d', '3', 'page', 3, 'Sub copăcelul de lună', 'La umbra unui copăcel straniu zace un robot cu carcasă din tinichea. Are urechi scurte, ca două antene, și ochi negri, stinși. Un LED pâlpâie: „…Gru…bot…”.

Puf-Puf, în șoaptă:
— Model iepure. Blând, dar căzut.

Iepurele negru se apleacă:
— Îl putem reporni?', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/3.moon-tree.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('25ea8178-32a2-591e-9c48-271491cf32eb', '73e87fe6-7508-5b21-b498-b1468f01b6b7', 'ro-ro', 'Sub copăcelul de lună', 'La umbra unui copăcel straniu zace un robot cu carcasă din tinichea. Are urechi scurte, ca două antene, și ochi negri, stinși. Un LED pâlpâie: „…Gru…bot…”.

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
    ('d4f2942b-e26e-5a51-853a-40deb22bc0a6', '73e87fe6-7508-5b21-b498-b1468f01b6b7', 'en-us', 'Under the moon tree', 'In the shade of a strange little tree lies a robot with a tin-like casing. It has short ears, like two antennas, and black, lifeless eyes. An LED flickers: "...Gru...bot...".

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
    ('2ce553f4-9842-5815-a840-c48fdae1609b', '73e87fe6-7508-5b21-b498-b1468f01b6b7', 'hu-hu', 'A hold fácskája alatt', 'Egy furcsa fácskának az árnyékában egy robot fekszik, bádog házassal. Rövid fülei vannak, mint két antenna, és fekete, kialudt szemek. Egy LED villog: "...Gru...bot...".

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
    ('27b7ffdf-2ddf-5faa-97b9-68a4d997faff', '8543adc1-309f-53d9-a28c-baea44e2683d', '4', 'page', 4, 'Circuitul întrerupt', 'Panoul frontal stă întredeschis. Înăuntru, un circuit fisurat cârâie încet și plutește miros de ozon.
Iepurele negru scoate un morcov și zâmbește:
— Pe Lunaria, morcovii nu repară roboți.

— Nu direct, îl corectează Puf-Puf. Dar fibra de Lunkarot — morcovul acela special de pe Lunaria — poate ține loc de jumper, iar sucul lui răcește bine traseele. Contează ritmul, nu forța.

— Atunci tăiem o fibră subțire și picurăm încet, spui tu.
— Fix așa, încuviințează Puf-Puf.', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/4.broken-circuit.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('99f39421-1b44-509a-aaa0-2a45889a9c48', '27b7ffdf-2ddf-5faa-97b9-68a4d997faff', 'ro-ro', 'Circuitul întrerupt', 'Panoul frontal stă întredeschis. Înăuntru, un circuit fisurat cârâie încet și plutește miros de ozon.
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
    ('c9c5faba-f75e-549d-9d9a-76b5216871ca', '27b7ffdf-2ddf-5faa-97b9-68a4d997faff', 'en-us', 'The broken circuit', 'The front panel stands ajar. Inside, a cracked circuit sizzles slowly and floats the smell of ozone.
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
    ('99059233-f528-5088-b1fd-f4363d717bf0', '27b7ffdf-2ddf-5faa-97b9-68a4d997faff', 'hu-hu', 'A megszakított áramkör', 'Az elülső panel félig nyitva áll. Belül egy repedt áramkör lassan zizeg és ózon szagot áraszt.
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
    ('997d3eab-9d30-522d-8eac-104a940903d5', '8543adc1-309f-53d9-a28c-baea44e2683d', '5', 'quiz', 5, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/lunaria-s2/5.quiz.png', 'Circuitul cere ritm, nu împingere. Aveți Lunkarot pentru un jumper moale si sucul care raceste. Care e mișcarea ta aici, ca să-l repari, fără să agravezi fisura?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('f6df75c5-0c2a-5903-bcb0-2cb4ffbf0703', '997d3eab-9d30-522d-8eac-104a940903d5', 'ro-ro', NULL, NULL, 'Circuitul cere ritm, nu împingere. Aveți Lunkarot pentru un jumper moale si sucul care raceste. Care e mișcarea ta aici, ca să-l repari, fără să agravezi fisura?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('219ceaaf-6158-55b1-ac40-5d9a100b069f', '997d3eab-9d30-522d-8eac-104a940903d5', 'en-us', NULL, NULL, 'The circuit requires rhythm, not pushing. You have Lunkarot for a soft jumper and juice that cools. What''s your move here, to repair it without aggravating the crack?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('71938356-a2bc-5bca-9e64-614c7ed68930', '997d3eab-9d30-522d-8eac-104a940903d5', 'hu-hu', NULL, NULL, 'Az áramkör ritmust kér, nem erőltetést. Van Lunkarot egy puha jumperhez és lé, ami hűti. Mi a mozdulatod itt, hogy megjavítsd, anélkül, hogy súlyosbítanád a repedést?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('53b09be8-cfa6-5a83-8d83-d4cc193e18eb', '997d3eab-9d30-522d-8eac-104a940903d5', 'a', 'Il montez si demontez pe tot.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('cdb3d019-2701-5375-b5a7-68e0eeaad47e', '53b09be8-cfa6-5a83-8d83-d4cc193e18eb', 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('f3a91bc7-f2de-5545-b264-6c3825ae1f5f', '53b09be8-cfa6-5a83-8d83-d4cc193e18eb', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('60fcb474-2576-5308-aab8-af192b32d071', '53b09be8-cfa6-5a83-8d83-d4cc193e18eb', 'ro-ro', 'Il montez si demontez pe tot.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f0a53c1c-a120-5842-8d25-4bf1e391865a', '53b09be8-cfa6-5a83-8d83-d4cc193e18eb', 'en-us', 'I mount and dismount everything.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('08c702da-7ca7-5a98-b4bf-5ea9291dd4ec', '53b09be8-cfa6-5a83-8d83-d4cc193e18eb', 'hu-hu', 'Mindent felszerel és leszerel.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('784c28f1-0052-51f4-8b6f-b06bd696c146', '997d3eab-9d30-522d-8eac-104a940903d5', 'b', 'Intai ma intreb ce of i inauntru', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('a3c2ab0e-c077-533d-bf5c-39d5f9e53440', '784c28f1-0052-51f4-8b6f-b06bd696c146', 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('fec1de9a-cb90-5ca6-908d-84253055a4c8', '784c28f1-0052-51f4-8b6f-b06bd696c146', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1676e79a-7b93-5bc9-9841-b2086aeed416', '784c28f1-0052-51f4-8b6f-b06bd696c146', 'ro-ro', 'Intai ma intreb ce of i inauntru')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('9f314f95-efcd-531c-ac50-9fa7f7cfd533', '784c28f1-0052-51f4-8b6f-b06bd696c146', 'en-us', 'First I wonder what''s inside')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('c328a874-16e6-5d08-9e84-1161cee12ed8', '784c28f1-0052-51f4-8b6f-b06bd696c146', 'hu-hu', 'Először megkérdezem, mi van benne')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('defc33d4-ac29-5595-9821-1bdedf9c28fa', '997d3eab-9d30-522d-8eac-104a940903d5', 'c', 'Fac un plan bun cu tot ce e de facut si apoi actionez.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('baf6ed94-f9a9-5762-8c5e-bcf751b45d97', 'defc33d4-ac29-5595-9821-1bdedf9c28fa', 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('c6c0904a-e892-5c53-aaf1-cfdd5b042521', 'defc33d4-ac29-5595-9821-1bdedf9c28fa', 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('6aec1c43-d87f-5619-b547-2e3f7bd526cb', 'defc33d4-ac29-5595-9821-1bdedf9c28fa', 'ro-ro', 'Fac un plan bun cu tot ce e de facut si apoi actionez.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('2f15853d-5f58-5085-aab7-98a36f075ef6', 'defc33d4-ac29-5595-9821-1bdedf9c28fa', 'en-us', 'I make a good plan with everything that needs to be done and then act.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d0807b59-b2a0-5de2-9016-dcd2e84c7382', 'defc33d4-ac29-5595-9821-1bdedf9c28fa', 'hu-hu', 'Jó tervet készítek mindennel, amit csinálni kell, aztán cselekszem.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('89ce1cf9-e346-5c4f-85e1-79808f598ac6', '997d3eab-9d30-522d-8eac-104a940903d5', 'd', 'Creez o inima artificiala', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('be3eea0f-9eef-55e0-8d57-1d36d7289ed3', '89ce1cf9-e346-5c4f-85e1-79808f598ac6', 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('e4ab878f-e8f3-57b4-8e31-8a2925cafc54', '89ce1cf9-e346-5c4f-85e1-79808f598ac6', 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('2f2a3466-3dd5-5b46-beae-9e022cb4e2ab', '89ce1cf9-e346-5c4f-85e1-79808f598ac6', 'ro-ro', 'Creez o inima artificiala')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('0ee754d7-fe0f-52a6-a9be-b7b259dafeac', '89ce1cf9-e346-5c4f-85e1-79808f598ac6', 'en-us', 'I create an artificial heart')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('b10ba1f0-57ae-5489-9e4a-e36cfbbcfeeb', '89ce1cf9-e346-5c4f-85e1-79808f598ac6', 'hu-hu', 'Mesterséges szívet hozok létre')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('d8212932-13ff-594a-8edb-b22737dfb945', '997d3eab-9d30-522d-8eac-104a940903d5', 'e', 'Totul cu precautie, nu vrem sa-l stricam.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('40d1934e-b3a9-5b16-be7f-4390970f86fe', 'd8212932-13ff-594a-8edb-b22737dfb945', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('e269a4d4-8eef-582b-8259-39e9e40d8e61', 'd8212932-13ff-594a-8edb-b22737dfb945', 'Alchemy', 'Karott', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f9140aec-091c-5fef-9529-83d9746f402d', 'd8212932-13ff-594a-8edb-b22737dfb945', 'ro-ro', 'Totul cu precautie, nu vrem sa-l stricam.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('97d68d72-fe16-5e75-a39c-fead6ed4f886', 'd8212932-13ff-594a-8edb-b22737dfb945', 'en-us', 'Everything with caution, we don''t want to break it.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('424692b3-0957-54e3-8664-6e2fb9a4f557', 'd8212932-13ff-594a-8edb-b22737dfb945', 'hu-hu', 'Minden óvatosan, nem akarjuk tönkretenni.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('0b7be5f9-f3ad-59f6-a1b3-3c4184e0da71', '8543adc1-309f-53d9-a28c-baea44e2683d', '6', 'page', 6, 'Grubot își găsește vocea', 'Sunetul se stabilizează, iar ochii negri se aprind. Antenele-urechi tresar ușor.

— Sistem… stabil, anunță robotul. Nume: Grubot. Model: iepure. Stare: conștiință activă. Modul umor: experimental.
Puf-Puf zâmbește:
— Îți stă bine cu viața înapoi.
— Mulțumesc, răspunde Grubot. M-am rătăcit după o furtună de câmp magnetic. Casa mea e Mechanika. Pot decola, apoi am nevoie de încărcare.
— Notat, spui tu.
— Salvez coordonatele. Când deschideți ruta, vă aștept acolo, confirmă Grubot.', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/6.grubot-talks.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9f3901b0-fd2f-53f0-a3f1-635ee0a96124', '0b7be5f9-f3ad-59f6-a1b3-3c4184e0da71', 'ro-ro', 'Grubot își găsește vocea', 'Sunetul se stabilizează, iar ochii negri se aprind. Antenele-urechi tresar ușor.

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
    ('56963eda-85ad-56ef-8d97-9c19a5eab137', '0b7be5f9-f3ad-59f6-a1b3-3c4184e0da71', 'en-us', 'Grubot finds his voice', 'The sound stabilizes, and the black eyes light up. The antenna-ears twitch slightly.

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
    ('15a010d3-8d9e-55af-8437-86b072c3e8c3', '0b7be5f9-f3ad-59f6-a1b3-3c4184e0da71', 'hu-hu', 'Grubot megtalálja a hangját', 'A hang stabilizálódik, és a fekete szemek kigyulladnak. Az antenna-fülek enyhén megrezzennek.

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
    ('338410c6-68a7-58ca-8431-0268502233c0', '8543adc1-309f-53d9-a28c-baea44e2683d', '7', 'page', 7, 'Plecarea', 'Propulsoarele lui Grubot şuieră ușor. Se ridică deasupra câmpiilor portocalii, face o voltă scurtă și zboară spre orizont.
— Un prieten nou, o rută viitoare. Continuăm, zice Puf-Puf.
Iepurele negru zâmbește:
— Drumul e clar: înainte.', 'images/tol/stories/seed@alchimalia.com/lunaria-s2/7.departure.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('20401c6f-5db2-58cd-b9fc-9419b18dfbb0', '338410c6-68a7-58ca-8431-0268502233c0', 'ro-ro', 'Plecarea', 'Propulsoarele lui Grubot şuieră ușor. Se ridică deasupra câmpiilor portocalii, face o voltă scurtă și zboară spre orizont.
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
    ('8065aab4-924c-5852-bcdf-103d3c4eca4d', '338410c6-68a7-58ca-8431-0268502233c0', 'en-us', 'The departure', 'Grubot''s thrusters hiss softly. He rises above the orange fields, makes a short loop and flies toward the horizon.
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
    ('a89d17c1-a617-56b9-8ba0-c1318fa9df0c', '338410c6-68a7-58ca-8431-0268502233c0', 'hu-hu', 'Távozás', 'Grubot hajtóművei könnyedén fütyülnek. Felemelkedik a narancssárga mezők felett, egy rövid kört tesz és a horizont felé repül.
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
    ('df7eba27-d86b-5911-b115-9a68ca0ac999', 'mechanika-s1', 'Magnus', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('aa5c00fb-2c7f-51d9-be3a-a9c7857f932e', 'df7eba27-d86b-5911-b115-9a68ca0ac999', 'ro-ro', 'Magnus')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('abbd1c9e-c154-5f19-ba72-90fe4ead274c', 'df7eba27-d86b-5911-b115-9a68ca0ac999', 'en-us', 'Magnus')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('ff98044f-5d22-5862-9449-7a4f8f0692a6', 'df7eba27-d86b-5911-b115-9a68ca0ac999', 'hu-hu', 'Magnus')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for mechanika-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('2052966a-2a2d-5d9d-af13-887f3b48eba9', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '1', 'page', 1, 'Ceață peste Mechanika', 'Coborâți în Mechanika printr-o pâclă groasă și înțepătoare: fum de uzine, scântei rătăcite, aer greu. Luminile orașului sunt doar pete încețoșate; pasarelele scârțâie, roțile dințate se mișcă sacadat.
Puf-Puf aprinde lampa de pe guler.

— Vizibilitate: mică spre foarte mică. Grubot, ne auzi?
Un țiuit scurt. Apoi tăcere.

— Ținem direcția după pulsul cristalului, spui tu.
— Pe sub conducte, aproape de balustradă, zice iepurele negru.

Rătăciți câteva minute printre țevi, nituri și aburi reci, urmărind pulsația slabă a cristalului.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/1.fog.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('c3f7d5f2-ca82-5e1b-ac82-756247c96146', '2052966a-2a2d-5d9d-af13-887f3b48eba9', 'ro-ro', 'Ceață peste Mechanika', 'Coborâți în Mechanika printr-o pâclă groasă și înțepătoare: fum de uzine, scântei rătăcite, aer greu. Luminile orașului sunt doar pete încețoșate; pasarelele scârțâie, roțile dințate se mișcă sacadat.
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
    ('b85abeee-16f0-5f7b-8c64-8842dea64e33', '2052966a-2a2d-5d9d-af13-887f3b48eba9', 'en-us', 'Fog over Mechanika', 'You descend into Mechanika through thick, stinging haze: factory smoke, stray sparks, heavy air. The city lights are just blurred spots; the walkways creak, the gears move jerkily.
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
    ('242f2720-41f7-5752-bc8a-554b8ecf5764', '2052966a-2a2d-5d9d-af13-887f3b48eba9', 'hu-hu', 'Köd Mechanika felett', 'Mechanikába sűrű, csípős ködön keresztül szálltok le: gyárfüst, eltévedt szikrák, nehéz levegő. A város fényei csak ködös foltok; a gyaloghidak nyikorognak, a fogaskerekek rángatózva mennek.
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
    ('4f9e7fdd-16fe-571d-bd91-239a7e9b9ac9', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '2', 'page', 2, 'Grubot vă găsește', 'Un semnal curat taie ceața; o rază subțire brăzdează aerul. Din fum se conturează silueta lui Grubot.

— V-am detectat după semnătura cristalului și… o urmă chimică familiară: Lunkarot, spune robotul Grubot.
Puf-Puf râde:
— Nas digital bun.
— Promisiunea e promisiune, continuă Grubot. Mi-ați dat puls; eu vă dau vedere. Urmați-mă la Turnul de Control.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/2.grubot.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e37955fd-76db-52c0-9b66-1b03f8c133d5', '4f9e7fdd-16fe-571d-bd91-239a7e9b9ac9', 'ro-ro', 'Grubot vă găsește', 'Un semnal curat taie ceața; o rază subțire brăzdează aerul. Din fum se conturează silueta lui Grubot.

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
    ('e7333f8b-c84e-596c-81be-70f25ff09fb6', '4f9e7fdd-16fe-571d-bd91-239a7e9b9ac9', 'en-us', 'Grubot finds you', 'A clean signal cuts through the fog; a thin beam streaks through the air. From the smoke, Grubot''s silhouette takes shape.

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
    ('067ffd2c-9f3b-5236-ae51-e3151c6c3293', '4f9e7fdd-16fe-571d-bd91-239a7e9b9ac9', 'hu-hu', 'Grubot megtalál titeket', 'Egy tiszta jel vágja át a ködöt; egy vékony sugár vonja a levegőt. A füstből Grubot sziluettje rajzolódik ki.

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
    ('b55c270e-0114-54a5-9967-9c4960b398c9', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '3', 'page', 3, 'Turnul de control', 'Urcați scările metalice, cu pașii bătuți în ecou. Sus, vă așteaptă un colos: Telescopul Mechanika-Magnus — tuburi, oglinzi, angrenaje; cupola se deschide ca o floare de metal.

— Cu ăsta căutăm planeta mea? întreabă Puf-Puf.
— În mod normal scanează traseele orașului, răspunde Grubot. Dar cu adaptorul de cristal îl putem regla pe semnătura planetei tale.
— Atunci îl montăm acum, zici tu.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/3.control-tower.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('c6e01f19-762d-5137-85e8-4c60bff8ef61', 'b55c270e-0114-54a5-9967-9c4960b398c9', 'ro-ro', 'Turnul de control', 'Urcați scările metalice, cu pașii bătuți în ecou. Sus, vă așteaptă un colos: Telescopul Mechanika-Magnus — tuburi, oglinzi, angrenaje; cupola se deschide ca o floare de metal.

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
    ('5826476c-f558-540a-afa7-81f037c053c1', 'b55c270e-0114-54a5-9967-9c4960b398c9', 'en-us', 'The control tower', 'You climb the metal stairs, with steps echoing. Upstairs, a colossus awaits you: the Mechanika-Magnus Telescope — tubes, mirrors, gears; the dome opens like a metal flower.

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
    ('9a62b667-f289-576d-aad3-2cb3e89906fb', 'b55c270e-0114-54a5-9967-9c4960b398c9', 'hu-hu', 'A vezérlő torony', 'Felmászol a fém lépcsőkön, a lépések visszhangoznak. Fent egy kolosszus vár rátok: a Mechanika-Magnus Teleszkóp — csövek, tükrök, fogaskerekek; a kupola fém virágként nyílik ki.

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
    ('3849fddc-2c97-5e7a-8184-ee61ee0e6975', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '4', 'page', 4, 'Ce lipsește', 'Grubot rabatează panoul lateral al telescopului. Înăuntru se vede un soclu gol și bobine groase.

— Ne lipsesc două lucruri, spune el clar:

o piatră din Lunaria pentru „Ochiul Verde” — piesa care calibrează spectrul;

energie stocată pentru bobinele de focalizare.

— Avem un rezervor sub turn: încărcat, dar instabil, adaugă Grubot.
— Dacă aveți piatra, o montez acum; dacă nu, pregătim locașul și stabilizăm bobinele.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/4.missing.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('c7ecbfa0-44a5-5f65-9c31-20b69f00182a', '3849fddc-2c97-5e7a-8184-ee61ee0e6975', 'ro-ro', 'Ce lipsește', 'Grubot rabatează panoul lateral al telescopului. Înăuntru se vede un soclu gol și bobine groase.

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
    ('4dcef6e0-0596-5bdb-8b6a-a7e09eba7a0c', '3849fddc-2c97-5e7a-8184-ee61ee0e6975', 'en-us', 'What''s missing', 'Grubot folds back the telescope''s side panel. Inside you can see an empty socket and thick coils.

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
    ('1f337c06-9dfb-58c1-89aa-87df4db280b6', '3849fddc-2c97-5e7a-8184-ee61ee0e6975', 'hu-hu', 'Mi hiányzik', 'Grubot visszahajtja a teleszkóp oldalsó paneljét. Belül egy üres foglalat és vastag tekercsek látszanak.

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
    ('ccecc34a-3249-535e-8cfc-480096b3045c', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '5', 'quiz', 5, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/mechanika-s1/5.quiz.png', 'Rezervorul are energie, dar conexiunea la bobine cere finețe. Ce mișcare alegi ca să pornești telescopul fără să forțezi?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('671aaadf-4d70-5601-b2ff-2954191d988b', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'ro-ro', NULL, NULL, 'Rezervorul are energie, dar conexiunea la bobine cere finețe. Ce mișcare alegi ca să pornești telescopul fără să forțezi?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('76389e21-4473-56d8-bb05-63542bf3a059', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'en-us', NULL, NULL, 'The reservoir has energy, but the connection to the coils requires finesse. What move do you choose to start the telescope without forcing it?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('dddb9162-9711-571a-91e7-6e79eab977c0', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'hu-hu', NULL, NULL, 'A tartályban van energia, de a tekercsekhez való kapcsolódás finomságot igényel. Milyen mozdulatot választasz, hogy beindítsd a teleszkópot anélkül, hogy erőltetnéd?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'a', 'Lipesc piatra cu mana mea.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('4b19466e-a05a-5afb-9b74-05ba9aab2677', 'b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'Personality', 'curiosity', 2)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('f2e29b26-9888-593a-81d1-b79215c018fb', 'b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('4de01079-318f-5964-a44d-7adf57ae6808', 'b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1366323d-20ea-5576-b520-79270d5ab83c', 'b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'ro-ro', 'Lipesc piatra cu mana mea.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('9260f531-f595-587b-a264-384dee4855fc', 'b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'en-us', 'I attach the stone with my hand.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('4bced502-8cba-5804-8af1-125929afd047', 'b28ba0cf-9523-506d-9f0e-8bea49c22bb8', 'hu-hu', 'A kezemmel ragasztom fel a követ.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'b', 'a intreb oare cum pot lega piatra si ce se va intampla dupa', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('f4bbb8b4-3a65-5d49-834b-dc5b59a52a92', 'f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('0bdc56b5-d480-5ab5-8726-c82f4401fec8', 'f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('21893df6-7c12-5de3-9563-544937916f7b', 'f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('316556bb-e6b8-5f4f-b793-bdcf61165c5d', 'f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'ro-ro', 'a intreb oare cum pot lega piatra si ce se va intampla dupa')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e490b560-6a8c-5550-967e-bbde673f246e', 'f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'en-us', 'I wonder how I can connect the stone and what will happen after')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('88ffc0ab-9909-56d8-9e32-e0cef2018652', 'f76a2acd-0519-5c31-862a-e0f3d3fb9882', 'hu-hu', 'Megkérdezem, hogyan köthetem fel a követ és mi fog történni utána')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('ff1e124a-065f-5159-b492-4af8129694f8', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'c', 'Ma gandesc intai sa pun piatra, il pornesc dupa daca nu merge fac alt plan.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('e4b5a9dd-eb13-5e40-a1fd-eb785c035df1', 'ff1e124a-065f-5159-b492-4af8129694f8', 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('d65a245a-57f6-56a4-89fc-84723e3a87cb', 'ff1e124a-065f-5159-b492-4af8129694f8', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('cff0a7ce-8c34-5418-8b87-83c052b6ead1', 'ff1e124a-065f-5159-b492-4af8129694f8', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('2bf4642b-9a9b-554e-b7f2-54553c61dbd9', 'ff1e124a-065f-5159-b492-4af8129694f8', 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('7ed35cfe-8aa7-59cf-86c2-c238a65e41d2', 'ff1e124a-065f-5159-b492-4af8129694f8', 'ro-ro', 'Ma gandesc intai sa pun piatra, il pornesc dupa daca nu merge fac alt plan.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('59fcb324-2b65-5a46-8988-f7756b808c91', 'ff1e124a-065f-5159-b492-4af8129694f8', 'en-us', 'I think first to put the stone, start it after, if it doesn''t work I make another plan.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('64b58b2e-b87e-5e40-a6ad-3faa58b15964', 'ff1e124a-065f-5159-b492-4af8129694f8', 'hu-hu', 'Először gondolkodom, hogy felrakom a követ, utána beindítom, ha nem megy, más tervet csinálok.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('c345160b-3556-5ce3-959a-a1dd3955ff12', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'd', 'Improvizez o sursa de energie a pietrei si a penei de pe tera de la pasare.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('4642bbf6-170d-591c-9d3d-c1da8181d56b', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('90b68e5f-dcd2-5a2c-9d55-6c66ef003f38', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('75a4f6da-73ea-55d7-9d78-683b8f8f1889', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('675e9113-3205-5e86-811b-7e65335e6dff', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('3bb293c6-c089-5f5c-8193-1dc406ea059e', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'ro-ro', 'Improvizez o sursa de energie a pietrei si a penei de pe tera de la pasare.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a8a1a2a7-e825-55a4-9cd5-8ae5dc7d181f', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'en-us', 'I improvise an energy source from the stone and the feather from the bird on Terra.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('deff4ecf-5c95-58d7-be04-b31de5b3e5ad', 'c345160b-3556-5ce3-959a-a1dd3955ff12', 'hu-hu', 'Improvizálok egy energiaforrást a kőből és a Terán lévő madár tollából.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'ccecc34a-3249-535e-8cfc-480096b3045c', 'e', 'Intai montez piatra, vad daca se strica, dupa dau inainte, pas cu pas.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('5fd98ff1-c942-5178-a3ec-e6b1979b7ed3', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('fb9c809f-9ace-553b-b03b-74ba3c1d3b24', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('4589ebde-2353-5a4e-82fa-1e3234d3e128', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'Alchemy', 'circuite electrice', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('0d1ee592-3ca9-55a1-9857-cbcaa0a05675', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('37daec07-1a60-5d89-9dcd-34fa2b6facbb', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'ro-ro', 'Intai montez piatra, vad daca se strica, dupa dau inainte, pas cu pas.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d0116925-5a1b-58d8-82b3-4c81ca6e25a6', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'en-us', 'First I mount the stone, see if it breaks, then move forward, step by step.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('c1f702e8-8323-5f04-919e-efa988f9ad85', '1eb6f19a-b8df-5b8c-8622-502e3030ae93', 'hu-hu', 'Először felrakom a követ, megnézem, hogy tönkremegy-e, utána lépésről lépésre haladok.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('dd21cca9-9c78-5586-8e9e-2121c9f26176', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '6', 'page', 6, 'Focalizare', 'Telescopul se trezește. Oglinzile se aliniază, hologramele se aprind, iar pe ele se ramifică linii de lumină. Un traseu pulsează mai tare — un fir care vă cheamă.

— Arată-ne Kelo-Ketis! spune Puf-Puf.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/6.focus.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('59b3b4b7-27e0-5d94-abdd-1183d18b7bfc', 'dd21cca9-9c78-5586-8e9e-2121c9f26176', 'ro-ro', 'Focalizare', 'Telescopul se trezește. Oglinzile se aliniază, hologramele se aprind, iar pe ele se ramifică linii de lumină. Un traseu pulsează mai tare — un fir care vă cheamă.

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
    ('a2fe3d72-b471-5404-a669-d4e4524591a6', 'dd21cca9-9c78-5586-8e9e-2121c9f26176', 'en-us', 'Focus', 'The telescope awakens. The mirrors align, the holograms light up, and on them light lines branch out. One route pulses stronger — a thread that calls you.

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
    ('9f3a507a-8697-5b02-a114-ded0f4cfe21e', 'dd21cca9-9c78-5586-8e9e-2121c9f26176', 'hu-hu', 'Fókuszálás', 'A teleszkóp felébred. A tükrök igazodnak, a hologramok kigyulladnak, és rajtuk fény vonalak ágaznak ki. Egy útvonal erősebben pulzál — egy szál, amely arra hív.

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
    ('86255b3f-3084-597d-8a57-3669f440641f', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '7', 'page', 7, 'Harta se adâncește', 'Grubot tastează rapid.
— Semnal clar. Fixez coordonatele și salvez schema rutei pentru deschiderea portalului.
Puf-Puf zâmbește scurt:
— Primul pas mare e bifat.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/7.map-deepens.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('7fd84758-ccdd-51db-99c8-5283e85abb70', '86255b3f-3084-597d-8a57-3669f440641f', 'ro-ro', 'Harta se adâncește', 'Grubot tastează rapid.
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
    ('09b4b98f-0a7c-54aa-84f2-8afc1d419152', '86255b3f-3084-597d-8a57-3669f440641f', 'en-us', 'The map deepens', 'Grubot types rapidly.
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
    ('7fad83a1-6865-5bec-a9b2-d40c5d8d884f', '86255b3f-3084-597d-8a57-3669f440641f', 'hu-hu', 'A térkép mélyül', 'Grubot gyorsan gépel.
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
    ('475e82fa-708b-5b71-8839-59c6318bf826', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '8', 'page', 8, 'Recompensa de atelier', 'Grubot vă întinde o casetă metalică.
—(voce robotica) Circuite și conectori. Pentru ce urmează.

Puf-Puf chicotește:
— Le pun lângă biscuiții de motivație.

— Bună combinație: energie și moral, zici tu.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/8.reward.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('d4bb6e76-b8dd-50ec-a637-464f46a05bcd', '475e82fa-708b-5b71-8839-59c6318bf826', 'ro-ro', 'Recompensa de atelier', 'Grubot vă întinde o casetă metalică.
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
    ('722451b1-23f7-572b-9916-6598fe747788', '475e82fa-708b-5b71-8839-59c6318bf826', 'en-us', 'Workshop reward', 'Grubot hands you a metal case.
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
    ('675c8930-de3c-595c-b63b-3bd048d8c583', '475e82fa-708b-5b71-8839-59c6318bf826', 'hu-hu', 'A műhely jutalma', 'Grubot egy fém dobozt nyújt át.
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
    ('ffb1aa9d-3b99-5fab-876c-0f5fc0238cf5', 'df7eba27-d86b-5911-b115-9a68ca0ac999', '9', 'page', 9, 'Privirea înainte', 'Telescopul intră în repaus, iar ceața din oraș pare mai subțire. Holograma rămâne cu harta adâncită pe ecran, traseele marcate clar. Caseta cu Circuite electrice e prinsă la rucsac.

— Ok, echipă. Următoarea ramură? zice Puf-Puf.
— Est, pe pasarela principală, propune iepurele negru. E cea mai curată.
— Confirm, spune Grubot. Indicatorii arată vânt mai slab pe acolo.
— Atunci est să fie, închei tu. Mergem.', 'images/tol/stories/seed@alchimalia.com/mechanika-s1/9.ahead.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('530013aa-084d-51e9-ac6d-94f252291fac', 'ffb1aa9d-3b99-5fab-876c-0f5fc0238cf5', 'ro-ro', 'Privirea înainte', 'Telescopul intră în repaus, iar ceața din oraș pare mai subțire. Holograma rămâne cu harta adâncită pe ecran, traseele marcate clar. Caseta cu Circuite electrice e prinsă la rucsac.

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
    ('b5e850f3-41e9-515c-9c8c-440454832210', 'ffb1aa9d-3b99-5fab-876c-0f5fc0238cf5', 'en-us', 'Looking ahead', 'The telescope enters rest, and the fog in the city seems thinner. The hologram remains with the deepened map on screen, the routes marked clearly. The case with Electric circuits is attached to the backpack.

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
    ('4dbbe8c9-347a-54d9-878c-60c41de9e3a9', 'ffb1aa9d-3b99-5fab-876c-0f5fc0238cf5', 'hu-hu', 'Előre tekintés', 'A teleszkóp pihenőbe megy, és a város köde vékonyabbnak tűnik. A hologram a mélyült térképpel marad a képernyőn, az útvonalak világosan megjelölve. A doboz az Elektromos áramkörökkel a hátizsákhoz van rögzítve.

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
    ('fd8bd719-a7cf-53f9-8342-f4d7b0387acd', 'sylvaria-s1', 'Sylvaria', 'images/tol/stories/seed@alchimalia.com/sylvaria-s1/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('532f69b9-b568-5394-9617-02078beaeec8', 'fd8bd719-a7cf-53f9-8342-f4d7b0387acd', 'ro-ro', 'Sylvaria')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('3e0c1d4c-76d0-5ad2-8e7a-a83a1a486c3d', 'fd8bd719-a7cf-53f9-8342-f4d7b0387acd', 'en-us', 'Sylvaria')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('5ed6026e-c3fb-5a54-8f5e-1c0a7778ae40', 'fd8bd719-a7cf-53f9-8342-f4d7b0387acd', 'hu-hu', 'Sylvaria')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for sylvaria-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('7c23d421-9fbb-544c-9693-82e28ee3fddc', 'fd8bd719-a7cf-53f9-8342-f4d7b0387acd', '1', 'page', 1, 'Sylvaria', 'Sylvaria', 'images/tol/stories/seed@alchimalia.com/sylvaria-s1/0.Cover.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('6b630e41-cdd9-5757-9798-4717473ac125', '7c23d421-9fbb-544c-9693-82e28ee3fddc', 'ro-ro', 'Sylvaria', 'Sylvaria', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('2c123d88-7e80-55f0-868a-ab1dd70ec9c6', '7c23d421-9fbb-544c-9693-82e28ee3fddc', 'en-us', 'Sylvaria', 'Sylvaria', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4d35fbe7-671b-54fc-8786-fff3e9615c4b', '7c23d421-9fbb-544c-9693-82e28ee3fddc', 'hu-hu', 'Sylvaria', 'Sylvaria', NULL, NULL, NULL)
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
    ('4fd60fcf-4a2e-5820-9be5-412562bd4d30', 'terra-s1', 'Umbra de la Fermă', 'images/tol/stories/seed@alchimalia.com/terra-s1/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('43c0c2b6-dc8a-520d-9d08-1f2c362f0f45', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', 'ro-ro', 'Umbra de la Fermă')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('0b722dd1-3b7f-5c11-882c-a00af8672c3c', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', 'en-us', 'The Shadow at the Farm')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('ceca2b69-4aaf-55c7-9732-8933a3160563', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', 'hu-hu', 'A Farm Árnyéka')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for terra-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('d5ff9ff8-ce3b-5018-80c7-7ca5f5509450', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '1', 'page', 1, 'Sosirea', 'Puf-Puf decide să exploreze planeta pe care tocmai a aterizat. Mergeți împreună și după câteva ore, o fermă, unde sunt mai mulți săteni, vă iese în cale. Totul pare în ordine o perioadă, dar oamenii își sunt îngrijorați și privesc cerul ca pe un oaspete nepoftit. \
Puf-Puf își încrețește mustățile.\
Puf-Puf: - Oare de ce e lumea așa de speriată? De ce toți stau de parcă își țin respirația?', 'images/tol/stories/seed@alchimalia.com/terra-s1/1.arrival.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9fc234ee-d692-565b-86e8-6b2ed3faad05', 'd5ff9ff8-ce3b-5018-80c7-7ca5f5509450', 'ro-ro', 'Sosirea', 'Puf-Puf decide să exploreze planeta pe care tocmai a aterizat. Mergeți împreună și după câteva ore, o fermă, unde sunt mai mulți săteni, vă iese în cale. Totul pare în ordine o perioadă, dar oamenii își sunt îngrijorați și privesc cerul ca pe un oaspete nepoftit. \
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
    ('953ceb3f-c87c-5762-8ba2-eacb2d830d39', 'd5ff9ff8-ce3b-5018-80c7-7ca5f5509450', 'en-us', 'Arrival', 'Puf-Puf decides to explore the planet he just landed on. You walk together and after a few hours, a farm, where several villagers live, appears in your path. Everything seems in order for a while, but people are worried and watch the sky like an unwelcome guest. 
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
    ('4be7bcf3-1706-55aa-8e3f-831a73d185f0', 'd5ff9ff8-ce3b-5018-80c7-7ca5f5509450', 'hu-hu', 'Érkezés', 'Puf-Puf úgy dönt, hogy felfedezi a bolygót, amelyre éppen leszállt. Együtt sétáltok és néhány óra után egy farm jelenik meg az úton, ahol több falusi lakik. Minden rendben tűnik egy ideig, de az emberek aggódnak és úgy néznek az égre, mint egy nem kívánt vendégre. 
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
    ('49590e4f-5c8a-5ceb-b4cb-862f29920502', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '2', 'page', 2, 'Tăcerea stranie', 'Niciun cocoș nu cântă, niciun câine nu latră, nici greierii nu-și găsesc corul. Liniștea apasă, groasă, ca o pătură udă. Puf-Puf se uită la tine.\nPuf-Puf: - E ca și cum natura ar număra până la zece și n-ar îndrăzni să spună: zece!', 'images/tol/stories/seed@alchimalia.com/terra-s1/2.silence.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('6885c19c-7485-57de-96c6-4015a776cccc', '49590e4f-5c8a-5ceb-b4cb-862f29920502', 'ro-ro', 'Tăcerea stranie', 'Niciun cocoș nu cântă, niciun câine nu latră, nici greierii nu-și găsesc corul. Liniștea apasă, groasă, ca o pătură udă. Puf-Puf se uită la tine.\nPuf-Puf: - E ca și cum natura ar număra până la zece și n-ar îndrăzni să spună: zece!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('c556e79e-eca0-5377-a304-dbbca35b36e8', '49590e4f-5c8a-5ceb-b4cb-862f29920502', 'en-us', 'Strange Silence', 'No rooster crows, no dog barks, not even the crickets find their chorus. The silence presses, thick, like a wet blanket. Puf-Puf looks at you.
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
    ('576da035-0370-5c8d-91ac-7174de229013', '49590e4f-5c8a-5ceb-b4cb-862f29920502', 'hu-hu', 'A furcsa csend', 'Egy kakas sem kukorékol, egy kutya sem ugat, a tücskök sem találják a kórusukat. A csend nyomja, vastag, mint egy nedves takaró. Puf-Puf rád néz: "Mintha a természet tízig számolna és nem merne tízre mondani."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('54989967-df1b-5264-95d0-e70a8f4abe52', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '3', 'page', 3, 'Șoaptele oamenilor', 'Un bătrân se apropie cu pași mărunți.\nBătrânul: -E… o umbră... coboară din cer la apus. Animalele fug, grânele se culcă la pământ. Zicem că-i un blestem. \nPuf-Puf: - Blestem sau semn, tot o cauză are. Hai să prindem urma!', 'images/tol/stories/seed@alchimalia.com/terra-s1/3.whispers.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('f89ddf1e-d0b2-5a50-850b-3d8e4137fc8d', '54989967-df1b-5264-95d0-e70a8f4abe52', 'ro-ro', 'Șoaptele oamenilor', 'Un bătrân se apropie cu pași mărunți.\nBătrânul: -E… o umbră... coboară din cer la apus. Animalele fug, grânele se culcă la pământ. Zicem că-i un blestem. \nPuf-Puf: - Blestem sau semn, tot o cauză are. Hai să prindem urma!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('85027224-3f89-5aeb-9801-e0e63dc1d9ea', '54989967-df1b-5264-95d0-e70a8f4abe52', 'en-us', 'Whispers of the People', 'An old man approaches with small steps.
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
    ('f678e5e3-291c-599f-aa9b-4b001a571c67', '54989967-df1b-5264-95d0-e70a8f4abe52', 'hu-hu', 'Az emberek suttogása', 'Egy öregember apró lépésekkel közeledik.
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
    ('e8258c95-2efa-57e9-ac35-c1006922dd95', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '4', 'page', 4, 'Prima noapte', 'Soarele se scurge după dealuri, iar vântul aduce un foșnet nou, neliniștit. Deasupra norilor, ceva mare se mișcă. Umbră peste câmpuri, iar luna parcă se dă la o parte. Puf-Puf își ține respirația o clipă și zâmbește puțin: \nPuf-Puf:- OK… asta nu e doar vreme schimbătoare.', 'images/tol/stories/seed@alchimalia.com/terra-s1/4.first-night.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e94b412f-64a8-5b59-aa6e-9fb2df18ebfc', 'e8258c95-2efa-57e9-ac35-c1006922dd95', 'ro-ro', 'Prima noapte', 'Soarele se scurge după dealuri, iar vântul aduce un foșnet nou, neliniștit. Deasupra norilor, ceva mare se mișcă. Umbră peste câmpuri, iar luna parcă se dă la o parte. Puf-Puf își ține respirația o clipă și zâmbește puțin: \nPuf-Puf:- OK… asta nu e doar vreme schimbătoare.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('c6820879-9284-5143-b9c9-03eca5aa123e', 'e8258c95-2efa-57e9-ac35-c1006922dd95', 'en-us', 'The First Night', 'The sun sinks behind the hills, and the wind brings a new, restless rustle. Above the clouds, something large moves. A shadow across the fields, and the moon seems to step aside. Puf-Puf holds his breath for a moment and smiles a little: 
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
    ('9647a93c-75ae-5acd-8ecc-f3e53d310f8e', 'e8258c95-2efa-57e9-ac35-c1006922dd95', 'hu-hu', 'Az első éjszaka', 'A nap a dombok mögé csorog, és a szél új, nyugtalan zörgést hoz. A felhők felett valami nagy mozog. Árnyék a mezők felett, és a hold mintha félrehúzódna. Puf-Puf egy pillanatra visszatartja a lélegzetét és kissé mosolyog: "OK... ez nem csak változékony időjárás."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('b2572aa2-4956-5e41-a400-f1e498398769', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '5', 'page', 5, 'Vibrația pământului', 'Pământul tresare, ușor la început, apoi ca un pas greoi. Aerul miroase a fum și a cenușă curată, ca după o scânteie. În lanul culcat, se ghicește o siluetă uriașă care pâlpâie intermitent, ca un far rănit.', 'images/tol/stories/seed@alchimalia.com/terra-s1/5.vibration.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('681547cf-a7d8-57c8-8ca0-300f442f7bd3', 'b2572aa2-4956-5e41-a400-f1e498398769', 'ro-ro', 'Vibrația pământului', 'Pământul tresare, ușor la început, apoi ca un pas greoi. Aerul miroase a fum și a cenușă curată, ca după o scânteie. În lanul culcat, se ghicește o siluetă uriașă care pâlpâie intermitent, ca un far rănit.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('d97ad907-325e-5aae-bc6a-3ae24eef8117', 'b2572aa2-4956-5e41-a400-f1e498398769', 'en-us', 'The Earth''s Vibration', 'The ground shivers, lightly at first, then like a heavy footstep. The air smells of smoke and clean ash, like after a spark. In the flattened field, a huge silhouette can be made out, flickering on and off like a wounded beacon.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('44242168-270f-5dd3-830e-3f5466ab31aa', 'b2572aa2-4956-5e41-a400-f1e498398769', 'hu-hu', 'A föld rezgése', 'A föld megremeg, először könnyedén, majd mint egy nehéz lépés. A levegő füst és tiszta hamu szagát árasztja, mint egy szikra után. A lefektetett kalásztengerben egy hatalmas sziluett sejlik, amely szaggatottan villog, mint egy sérült világítótorony.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('26b3e7f6-230d-5e98-88d1-aae7e865c47e', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '6', 'page', 6, 'Creatura', 'Din umbre iese la iveală o pasăre imensă, cu pene ca jarul și ochi de cristal. Aripa stângă îi atârnă grea; fiecare bătaie ridică scântei care se sting pe apă. Nu e un blestem: e o ființă rătăcită din altă lume, căzută într-un loc care nu o înțelege încă.', 'images/tol/stories/seed@alchimalia.com/terra-s1/6.creature.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('11369089-6b79-5322-96a5-937c6597d5fa', '26b3e7f6-230d-5e98-88d1-aae7e865c47e', 'ro-ro', 'Creatura', 'Din umbre iese la iveală o pasăre imensă, cu pene ca jarul și ochi de cristal. Aripa stângă îi atârnă grea; fiecare bătaie ridică scântei care se sting pe apă. Nu e un blestem: e o ființă rătăcită din altă lume, căzută într-un loc care nu o înțelege încă.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('7abb91c7-ea2a-5334-a4c0-ce85ab3d9c51', '26b3e7f6-230d-5e98-88d1-aae7e865c47e', 'en-us', 'The Creature', 'From the shadows emerges a massive bird, with ember-like feathers and crystal eyes. Its left wing hangs heavy; each flap throws up sparks that fizzle on the water. It''s not a curse: it''s a lost being from another world, fallen into a place that doesn''t understand it yet.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('49d36448-5431-5218-8826-9d1b4ce9b252', '26b3e7f6-230d-5e98-88d1-aae7e865c47e', 'hu-hu', 'A lény', 'Az árnyékokból egy hatalmas madár tűnik elő, tollakkal, mint a parázs, és kristály szemekkel. A bal szárnya nehezen lóg; minden csapása szikrákat emel, amelyek a vízen kialszanak. Ez nem átok: egy másik világból tévedt lény, aki egy olyan helyre esett, amely még nem érti meg.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('f8dccfcf-dfe6-59d5-8e45-13e4bb5df6da', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '7', 'page', 7, 'Legătura', 'Puf-Puf se apropie încet, îți face semn din cap și apoi îți vorbește pe șoptite.\nPuf-Puf: - E din Copacul Luminii! Dacă o ajutăm, ne poate arăta un drum ascuns pe Terra. \nPasărea te privește drept, fără teamă; în ochii ei se văd brazde și stele. \nPuf-Puf: - Tu ai mâini bune, zice Puf-Puf cu un zâmbet. \nPuf-Puf: - Și eu am… comentarii utile. Încearcă tu primul!', 'images/tol/stories/seed@alchimalia.com/terra-s1/7.bond.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('c0c00a4a-a9cb-5815-a228-3ae66cd4c8c1', 'f8dccfcf-dfe6-59d5-8e45-13e4bb5df6da', 'ro-ro', 'Legătura', 'Puf-Puf se apropie încet, îți face semn din cap și apoi îți vorbește pe șoptite.\nPuf-Puf: - E din Copacul Luminii! Dacă o ajutăm, ne poate arăta un drum ascuns pe Terra. \nPasărea te privește drept, fără teamă; în ochii ei se văd brazde și stele. \nPuf-Puf: - Tu ai mâini bune, zice Puf-Puf cu un zâmbet. \nPuf-Puf: - Și eu am… comentarii utile. Încearcă tu primul!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('c267ee68-74cc-5d85-b2a2-497358cc095d', 'f8dccfcf-dfe6-59d5-8e45-13e4bb5df6da', 'en-us', 'The Bond', 'Puf-Puf approaches slowly, nods at you, then whispers to you.
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
    ('3ff1bdbb-25e9-505d-9980-043f5bfff625', 'f8dccfcf-dfe6-59d5-8e45-13e4bb5df6da', 'hu-hu', 'A kapcsolat', 'Puf-Puf lassan közeledik, bólint, majd suttogva szól hozzád: "A Fény Fájából való. Ha segítünk neki, megmutathat egy rejtett utat a Terán." A madár egyenesen néz rád, félelem nélkül; a szemében barázdák és csillagok látszanak. "Neked jó kezeid vannak", mondja Puf-Puf mosolyogva. "És nekem... hasznos megjegyzéseim. Próbáld te először."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('8c62b5f6-e22c-580f-a38a-90daed78a123', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '8', 'quiz', 8, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/terra-s1/8.quiz.png', 'Cum ajuți pasărea rănită?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('92968499-b1d1-598b-b2a3-7c41cd777a22', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'ro-ro', NULL, NULL, 'Cum ajuți pasărea rănită?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('08568d67-fe03-5ac4-ac40-84a2555336d7', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'en-us', NULL, NULL, 'How do you help the wounded bird?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('fa7a76bd-a745-5691-86c0-eaf450ccb0ee', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'hu-hu', NULL, NULL, 'Hogyan segítesz a sérült madáron?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('2293109e-4eb7-52ff-83cc-6046d7cd6d31', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'a', 'O încurajezi să se ridice, să-și amintească de puterea ei.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('234bdfb8-4e85-528b-bc3c-e84bb070296d', '2293109e-4eb7-52ff-83cc-6046d7cd6d31', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('e59d41a8-73a1-5bae-8d72-6dd5e131cacf', '2293109e-4eb7-52ff-83cc-6046d7cd6d31', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d5e0f72b-7cec-5326-9105-b83fd86eccbe', '2293109e-4eb7-52ff-83cc-6046d7cd6d31', 'ro-ro', 'O încurajezi să se ridice, să-și amintească de puterea ei.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1dc37c14-f423-5a02-926f-6bb7612043e3', '2293109e-4eb7-52ff-83cc-6046d7cd6d31', 'en-us', 'Encourage it to rise and remember its strength.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('30edd476-2517-56c8-976d-7a4b2397e631', '2293109e-4eb7-52ff-83cc-6046d7cd6d31', 'hu-hu', 'Bátorítod, hogy felkeljen, emlékezzen az erejére.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('dce0b172-bb70-5116-b78c-7f5bfb79a1aa', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'b', 'O îngrijești cu grijă și răbdare.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('8372ca17-877f-53e4-b34e-1c92d66dcb04', 'dce0b172-bb70-5116-b78c-7f5bfb79a1aa', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('45d1f672-1196-56c8-a111-3091004daa53', 'dce0b172-bb70-5116-b78c-7f5bfb79a1aa', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('c1c662f2-3aaa-5867-8c81-ea955d2f1d35', 'dce0b172-bb70-5116-b78c-7f5bfb79a1aa', 'ro-ro', 'O îngrijești cu grijă și răbdare.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d5823e46-986e-577b-b398-7cc6a6ee719a', 'dce0b172-bb70-5116-b78c-7f5bfb79a1aa', 'en-us', 'Care for it with patience and care.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('77a9ba69-f59c-568a-aec3-d51228b328e4', 'dce0b172-bb70-5116-b78c-7f5bfb79a1aa', 'hu-hu', 'Gondosan és türelemmel ápolod.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('cc49fd3e-f711-5c4a-b26f-6db0d17b2d27', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'c', 'Îi construiești un plan pentru a-și întinde din nou aripile.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('dfdafe95-a248-5e23-b2a3-292ad2c33a9c', 'cc49fd3e-f711-5c4a-b26f-6db0d17b2d27', 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('3799864e-350d-53cd-9d97-0d8fbb1493b7', 'cc49fd3e-f711-5c4a-b26f-6db0d17b2d27', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('9bb255eb-1ccf-57b6-b262-ea5c0ab5373e', 'cc49fd3e-f711-5c4a-b26f-6db0d17b2d27', 'ro-ro', 'Îi construiești un plan pentru a-și întinde din nou aripile.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('6cb1de80-8773-5968-b613-39cf7e031f76', 'cc49fd3e-f711-5c4a-b26f-6db0d17b2d27', 'en-us', 'Build a plan so it can stretch its wings again.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1440ab8c-fa90-5840-86f4-86838d0086f2', 'cc49fd3e-f711-5c4a-b26f-6db0d17b2d27', 'hu-hu', 'Tervet készítesz neki, hogy újra kinyithassa a szárnyait.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('2dbeced4-f9a3-59fa-b29b-a46dc71244e0', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'd', 'Îți folosești creativitatea și găsești o metodă nouă de vindecare.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('9828929f-209e-5e36-a6ca-20aba8b87f35', '2dbeced4-f9a3-59fa-b29b-a46dc71244e0', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('27e9d543-0e83-59f4-8ecd-8e4dd2b73e68', '2dbeced4-f9a3-59fa-b29b-a46dc71244e0', 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('0a83e7e2-5f95-5475-8265-343e536e9b0e', '2dbeced4-f9a3-59fa-b29b-a46dc71244e0', 'ro-ro', 'Îți folosești creativitatea și găsești o metodă nouă de vindecare.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('bb797a47-58a9-5cd4-9439-d49e477749c0', '2dbeced4-f9a3-59fa-b29b-a46dc71244e0', 'en-us', 'Use your creativity and find a new way to heal.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d2eb1c73-7d34-553c-92a6-c33fe2c3fa4c', '2dbeced4-f9a3-59fa-b29b-a46dc71244e0', 'hu-hu', 'A kreativitásodat használod és új gyógyítási módszert találsz.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('99b35ca6-20f8-58b8-80c4-7424d32f8158', '8c62b5f6-e22c-580f-a38a-90daed78a123', 'e', 'Cercetezi semnele luminii de pe penele ei, pentru a descoperi secretul.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('b552e9ee-c5f5-573e-9243-6b234a7d697e', '99b35ca6-20f8-58b8-80c4-7424d32f8158', 'Personality', 'curiosity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('12914112-3d02-5570-a37e-ccbf4685e157', '99b35ca6-20f8-58b8-80c4-7424d32f8158', 'Discovery', 'discovery credit', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('becebd7e-d5ba-54da-b081-a4dd74995e7a', '99b35ca6-20f8-58b8-80c4-7424d32f8158', 'ro-ro', 'Cercetezi semnele luminii de pe penele ei, pentru a descoperi secretul.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('98799d56-0604-5661-a2fd-fa1905ce899a', '99b35ca6-20f8-58b8-80c4-7424d32f8158', 'en-us', 'Study the light markings on its feathers to uncover the secret.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('62d4b7b6-521f-5c47-af00-60513cd27f49', '99b35ca6-20f8-58b8-80c4-7424d32f8158', 'hu-hu', 'Megvizsgálod a tollain lévő fény jeleit, hogy felfedezd a titkot.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('c3334b7d-d994-5cb5-a424-2ecf8a2a47cb', '4fd60fcf-4a2e-5820-9be5-412562bd4d30', '9', 'page', 9, 'Drumul deschis', 'După ajutorul vostru, pasărea își strânge aripa rănită la piept și, cu un efort calm, își întinde cealaltă. O dungă de lumină se desprinde din penele ei și taie câmpul ca o potecă nouă. \nPuf-Puf: - Uite-l!, spune Puf-Puf, cu un fel de mândrie jucăușă. \nPuf-Puf: - Drumul care n-a existat până acum cinci minute.\nPasărea ridică privirea spre nori, apoi către voi. Puf-Puf primește o pană, în semn de recunoștință, și știe că pasărea va deveni, de acum înainte, protectoarea satului. Pana cea mare, magică, alimentează o energie nebănuită.', 'images/tol/stories/seed@alchimalia.com/terra-s1/9.path.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('062cf711-3147-5014-9de1-14d7ca5f4764', 'c3334b7d-d994-5cb5-a424-2ecf8a2a47cb', 'ro-ro', 'Drumul deschis', 'După ajutorul vostru, pasărea își strânge aripa rănită la piept și, cu un efort calm, își întinde cealaltă. O dungă de lumină se desprinde din penele ei și taie câmpul ca o potecă nouă. \nPuf-Puf: - Uite-l!, spune Puf-Puf, cu un fel de mândrie jucăușă. \nPuf-Puf: - Drumul care n-a existat până acum cinci minute.\nPasărea ridică privirea spre nori, apoi către voi. Puf-Puf primește o pană, în semn de recunoștință, și știe că pasărea va deveni, de acum înainte, protectoarea satului. Pana cea mare, magică, alimentează o energie nebănuită.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('ab4fafbb-a3ed-55e9-94c6-f6c0572110ed', 'c3334b7d-d994-5cb5-a424-2ecf8a2a47cb', 'en-us', 'The Path Opens', 'After your help, the bird tucks its injured wing to its chest and, with a calm effort, stretches the other. A strip of light slips from its feathers and cuts the field like a new path. 
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
    ('9031498f-1c37-5590-8758-294a65d44efe', 'c3334b7d-d994-5cb5-a424-2ecf8a2a47cb', 'hu-hu', 'A megnyílt út', 'A segítségetek után a madár a mellkasához húzza a sérült szárnyát, és nyugodt erőfeszítéssel kinyújtja a másikat. Egy fénycsík szakad ki a tollai közül és új ösvényként vágja át a mezőt. 
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
    ('f7af6022-5f07-5484-ae65-c0ef93cb9a84', 'terra-s2', 'Un nou prieten', 'images/tol/stories/seed@alchimalia.com/terra-s2/0.Cover.png', NULL, NULL, NULL, NULL,
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
    ('2cf6cd78-7e53-57e7-a46b-08b781f60112', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', 'ro-ro', 'Un nou prieten')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('5e48b002-d8d0-54ac-a6fc-18300b896743', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', 'en-us', 'A New Friend')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('58cd3d63-3e7a-5196-9dd0-feb3d4a4eca0', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', 'hu-hu', 'Egy új barát')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for terra-s2
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('9523af44-b882-53c5-bb58-b407d352b0c0', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', '1', 'page', 1, 'Ținta: Lentila Lunii', 'Împăratul Puf-Puf nu și-a reparat nava, iar despre Pământ habar nu are unde se află față de Kelo-Ketis. A auzit, în schimb, de un artefact rar: Lentila Lunii. Cu ea, cristalul mărește harta și puteți căuta mai adânc ruta spre Kelo-Ketis. Ținta indică o zonă cu stânci joase și iarbă înaltă, la marginea unei păduri.', 'images/tol/stories/seed@alchimalia.com/terra-s2/1.target.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('a2ff19ef-6ffa-520e-9882-44ca4c04b651', '9523af44-b882-53c5-bb58-b407d352b0c0', 'ro-ro', 'Ținta: Lentila Lunii', 'Împăratul Puf-Puf nu și-a reparat nava, iar despre Pământ habar nu are unde se află față de Kelo-Ketis. A auzit, în schimb, de un artefact rar: Lentila Lunii. Cu ea, cristalul mărește harta și puteți căuta mai adânc ruta spre Kelo-Ketis. Ținta indică o zonă cu stânci joase și iarbă înaltă, la marginea unei păduri.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('e0e12133-e2b3-5ba4-b328-f20bdc714405', '9523af44-b882-53c5-bb58-b407d352b0c0', 'en-us', 'Target: The Moon Lens', 'Emperor Puf-Puf hasn''t repaired his ship, and as for Earth, he has no idea where it is in relation to Kelo-Ketis. He has heard, however, of a rare artifact: the Moon Lens. With it, the crystal enlarges the map and you can search deeper for the route to Kelo-Ketis. The target points to an area of low rocks and tall grass at the edge of a forest.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('36a633a0-1d49-5368-8305-9a731fb1eb79', '9523af44-b882-53c5-bb58-b407d352b0c0', 'hu-hu', 'Cél: A Hold Lencséje', 'Puf-Puf Császár nem javította meg a hajóját, és a Földről sem tudja, hol van a Kelo-Ketishez képest. Hallott azonban egy ritka műtárgyról: a Hold Lencséjéről. Vele a kristály megnagyobbítja a térképet, és mélyebben kereshetitek a Kelo-Ketis felé vezető útvonalat. A cél alacsony sziklák és magas fű területét jelzi, egy erdő szélén.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('1377e3ea-92b2-5574-8596-af889dc6bef2', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', '2', 'page', 2, 'Urme ciudate', 'În iarbă apar urme mici, în zigzag, care dispar lângă un buștean scobit. Pe lângă ele, cozi de morcov „gustate artistic”.', 'images/tol/stories/seed@alchimalia.com/terra-s2/2.tracks.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('e703c7d6-8932-5beb-b166-b747512b2bfd', '1377e3ea-92b2-5574-8596-af889dc6bef2', 'ro-ro', 'Urme ciudate', 'În iarbă apar urme mici, în zigzag, care dispar lângă un buștean scobit. Pe lângă ele, cozi de morcov „gustate artistic”.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('60898e1d-62b4-5756-bc43-761afd9305f7', '1377e3ea-92b2-5574-8596-af889dc6bef2', 'en-us', 'Strange Tracks', 'In the grass appear small zigzagging tracks that disappear near a hollowed log. Beside them, carrot tails "artistically nibbled".', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('ff10c11b-46d7-52fc-be5b-f489c5e7deb8', '1377e3ea-92b2-5574-8596-af889dc6bef2', 'hu-hu', 'Furcsa nyomok', 'A füvben kis, cikkcakkos nyomok tűnnek fel, amelyek egy üreges rönk mellett tűnnek el. Mellettük "művészien megkóstolt" sárgarépa farkok.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('951d279d-54a8-5e91-a51b-75167ea2da4f', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', '3', 'page', 3, 'Iepurele negru', 'Din buștean sare un iepure negru, cu blana mată și ochi mari. „Bună! Eu sunt… uneori aici, uneori pe Lunaria. Depinde cum clipești”, spune, apoi râde. „Știu unde e Lentila. Și am nevoie de ajutor să ajung acasă, pe Lunaria. Dacă mă ajutați, vă arăt drumul către Lentilă.”\nPuf-Puf:"Lunaria? parca stiu planeta asta...sa mergem..."', 'images/tol/stories/seed@alchimalia.com/terra-s2/3.black-rabbit.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('4c7337b9-0403-5147-aa5c-98c6584d699b', '951d279d-54a8-5e91-a51b-75167ea2da4f', 'ro-ro', 'Iepurele negru', 'Din buștean sare un iepure negru, cu blana mată și ochi mari. „Bună! Eu sunt… uneori aici, uneori pe Lunaria. Depinde cum clipești”, spune, apoi râde. „Știu unde e Lentila. Și am nevoie de ajutor să ajung acasă, pe Lunaria. Dacă mă ajutați, vă arăt drumul către Lentilă.”\nPuf-Puf:"Lunaria? parca stiu planeta asta...sa mergem..."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('d7f05419-8431-57d9-ae51-37c3d83a26a7', '951d279d-54a8-5e91-a51b-75167ea2da4f', 'en-us', 'The Black Rabbit', 'From the log leaps a black rabbit, with matte fur and big eyes. "Hello! I''m… sometimes here, sometimes on Lunaria. Depends how you blink," it says, then laughs. "I know where the Lens is. And I need help getting home to Lunaria. If you help me, I''ll show you the way to the Lens."
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
    ('2bde432a-9697-579d-8231-56af9eaaaa92', '951d279d-54a8-5e91-a51b-75167ea2da4f', 'hu-hu', 'A fekete nyuszi', 'A rönkből egy fekete nyuszi ugrik ki, matt bundával és nagy szemekkel. "Szia! Én... néha itt vagyok, néha Lunarián. Attól függ, hogyan pislogsz", mondja, majd nevet. "Tudom, hol van a Lencse. És segítségre van szükségem, hogy hazajussak Lunariára. Ha segítetek, megmutatom az utat a Lencséhez."
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
    ('bf44b536-5bef-5641-95f5-70cd3f83bc17', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', '4', 'page', 4, 'Locul lentilei', 'Ghidați de iepure, ajungeți la o grotă scurtă, rece, cu apă până la gleznă. Pe tavan atârnă cristale palide; pe o stâncă, un disc translucid — Lentila Lunii — prins între două excrescențe de piatră.', 'images/tol/stories/seed@alchimalia.com/terra-s2/4.cave.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('57c963f4-aa95-5dd0-8328-1d306d5f25d3', 'bf44b536-5bef-5641-95f5-70cd3f83bc17', 'ro-ro', 'Locul lentilei', 'Ghidați de iepure, ajungeți la o grotă scurtă, rece, cu apă până la gleznă. Pe tavan atârnă cristale palide; pe o stâncă, un disc translucid — Lentila Lunii — prins între două excrescențe de piatră.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4bd6365b-eb8c-5f3d-a178-eb12b0890785', 'bf44b536-5bef-5641-95f5-70cd3f83bc17', 'en-us', 'Where the Lens Is', 'Guided by the rabbit, you reach a short, cold grotto, with ankle-deep water. Pale crystals hang from the ceiling; on a rock rests a translucent disc — the Moon Lens — wedged between two stone outgrowths.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('546a84ba-9dfd-5e03-9a37-68f048374855', 'bf44b536-5bef-5641-95f5-70cd3f83bc17', 'hu-hu', 'A lencse helye', 'A nyuszi vezetésével egy rövid, hideg barlanghoz érkeztek, ahol a víz a bokáig ér. A mennyezeten halvány kristályok lógnak; egy sziklán egy áttetsző korong — a Hold Lencséje — két kőkinövés között szorulva.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('8ad0d111-5b94-5bb5-8a4e-a1e60cc2d7d5', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', '5', 'page', 5, 'Problema mică', 'Când atingi stânca, apa face valuri și intrarea parcă „respiră”, strângându-se ușor. „Grotă sensibilă”, spune iepurele. „Faci bine, primești ieșire. Faci rău, primești duș rece.”', 'images/tol/stories/seed@alchimalia.com/terra-s2/5.problem.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('6b04c630-6085-5b81-b7ca-d1b3daabb930', '8ad0d111-5b94-5bb5-8a4e-a1e60cc2d7d5', 'ro-ro', 'Problema mică', 'Când atingi stânca, apa face valuri și intrarea parcă „respiră”, strângându-se ușor. „Grotă sensibilă”, spune iepurele. „Faci bine, primești ieșire. Faci rău, primești duș rece.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4704a7ca-ba81-56e9-bc3f-a4ccca1b45b8', '8ad0d111-5b94-5bb5-8a4e-a1e60cc2d7d5', 'en-us', 'A Small Problem', 'When you touch the rock, the water ripples and the entrance seems to "breathe", narrowing slightly. "Sensitive grotto," says the rabbit. "Do good, you get an exit. Do wrong, you get a cold shower."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('428511f8-9f3b-5451-a439-56af9a013743', '8ad0d111-5b94-5bb5-8a4e-a1e60cc2d7d5', 'hu-hu', 'A kis probléma', 'Amikor megérinted a sziklát, a víz hullámokat csinál és a bejárat mintha "lélegzik", kissé összehúzódva. "Érzékeny barlang", mondja a nyuszi. "Jól csinálod, kijáratot kapsz. Rosszul csinálod, hideg zuhanyt kapsz."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', '6', 'quiz', 6, NULL, NULL, 'images/tol/stories/seed@alchimalia.com/terra-s2/6.quiz.png', 'Alege abordarea ca să scoți Lentila Lunii fără să destabilizezi grota:', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('c8278d7f-1470-508a-8965-ad6035b0dffc', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'ro-ro', NULL, NULL, 'Alege abordarea ca să scoți Lentila Lunii fără să destabilizezi grota:', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('08b740e2-3e45-5986-ae34-83ede7941b70', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'en-us', NULL, NULL, 'Choose the approach to extract the Moon Lens without destabilizing the grotto:', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('398c7583-a734-599c-9398-987094ff3ee9', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'hu-hu', NULL, NULL, 'Válaszd ki a megközelítést, hogy kivond a Hold Lencséjét anélkül, hogy destabilizálnád a barlangot:', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('5abec5a4-00d6-5e96-9496-511beaf03d9a', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'a', 'Te strecori pe pragul îngust și lucrezi rapid, ca să nu lași timp pereților să „respire”.', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('858ac58f-1635-5303-8b4b-3a7f305c6301', '5abec5a4-00d6-5e96-9496-511beaf03d9a', 'Personality', 'courage', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('9eb4457a-fcde-53aa-b61f-514c9ebec080', '5abec5a4-00d6-5e96-9496-511beaf03d9a', 'ro-ro', 'Te strecori pe pragul îngust și lucrezi rapid, ca să nu lași timp pereților să „respire”.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ebac512c-3015-5ec1-a431-428242f67071', '5abec5a4-00d6-5e96-9496-511beaf03d9a', 'en-us', 'Slip over the narrow threshold and work quickly, giving the walls no time to "breathe".')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('abcc2358-808b-56dc-8daa-6d8684836a87', '5abec5a4-00d6-5e96-9496-511beaf03d9a', 'hu-hu', 'Átcsúszol a keskeny küszöbön és gyorsan dolgozol, hogy ne hagyj időt a falaknak "lélegzeni".')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('3aa5e28c-1592-54d6-bd06-8ecb0e5d43b8', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'b', 'Te miști încet, verifici sprijinul la fiecare pas și menții apa cât mai liniștită.', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('b3ae4947-e188-5fe4-8555-77c62cbae941', '3aa5e28c-1592-54d6-bd06-8ecb0e5d43b8', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('fe282048-697b-505b-8e01-a110f624b86c', '3aa5e28c-1592-54d6-bd06-8ecb0e5d43b8', 'ro-ro', 'Te miști încet, verifici sprijinul la fiecare pas și menții apa cât mai liniștită.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ce408332-3044-5659-9cb4-3303c138fc6d', '3aa5e28c-1592-54d6-bd06-8ecb0e5d43b8', 'en-us', 'Move slowly, check your footing at each step, and keep the water as calm as possible.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('758cb1a8-dee5-5cc7-bb3a-58e140172e27', '3aa5e28c-1592-54d6-bd06-8ecb0e5d43b8', 'hu-hu', 'Lassan mozogsz, minden lépésnél ellenőrzöd a támasztékot és a vizet a lehető legnyugodtabban tartod.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('05b536df-ec81-5ea7-8068-eec3619771c5', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'c', 'Faci un plan scurt: cine ține lanterna, cine fixează pârghia, ordinea mișcărilor, semn la „stop”.', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('be5d2929-254d-57a2-9342-6b3e254f02ed', '05b536df-ec81-5ea7-8068-eec3619771c5', 'Personality', 'thinking', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ea2530c7-971e-5e64-a2e5-3c0e68b6f96f', '05b536df-ec81-5ea7-8068-eec3619771c5', 'ro-ro', 'Faci un plan scurt: cine ține lanterna, cine fixează pârghia, ordinea mișcărilor, semn la „stop”.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('b24aca34-7e73-5c29-b960-53783f8e75b8', '05b536df-ec81-5ea7-8068-eec3619771c5', 'en-us', 'Make a short plan: who holds the flashlight, who sets the lever, the order of moves, and a "stop" signal.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a973bd5c-c223-55bd-92e3-619128a1cc0a', '05b536df-ec81-5ea7-8068-eec3619771c5', 'hu-hu', 'Rövid tervet készítesz: ki tartja a lámpát, ki rögzíti a emelőt, a mozgások sorrendje, "stop" jel.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('533c4c46-2533-5890-a7bf-c4f8c8a82799', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'd', 'Improvizezi o unealtă: bețișor cu buclă din sfoară ca să tragi lentila de la distanță.', NULL, 4, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('5173e5f9-baa4-5b1e-a0d5-db389f3e2090', '533c4c46-2533-5890-a7bf-c4f8c8a82799', 'Personality', 'creativity', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('14712a55-9602-5c99-8e14-a2b7e4575c90', '533c4c46-2533-5890-a7bf-c4f8c8a82799', 'ro-ro', 'Improvizezi o unealtă: bețișor cu buclă din sfoară ca să tragi lentila de la distanță.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a0caefd0-ec18-5101-8cbe-383c57cd994a', '533c4c46-2533-5890-a7bf-c4f8c8a82799', 'en-us', 'Improvise a tool: a stick with a loop of string to pull the Lens from a distance.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d92eb708-db82-51c8-83b5-e91b4763617b', '533c4c46-2533-5890-a7bf-c4f8c8a82799', 'hu-hu', 'Improvizálsz egy eszközt: bot hurokkal a kötélből, hogy távolról húzd ki a lencsét.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('053bc673-3647-511c-948c-1e49c0de2275', '99134bfa-2b6d-5ea4-af47-cd25b0c2fec2', 'e', 'Pui mai întâi marcaje și ieșiri sigure: pietre-pas, frânghie la intrare, timp-limită pentru retragere.', NULL, 5, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('b0de0593-1949-55c2-b4d8-3506e40b1de6', '053bc673-3647-511c-948c-1e49c0de2275', 'Personality', 'safety', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f4edc0f3-5938-5c14-ac52-644f0479d859', '053bc673-3647-511c-948c-1e49c0de2275', 'ro-ro', 'Pui mai întâi marcaje și ieșiri sigure: pietre-pas, frânghie la intrare, timp-limită pentru retragere.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('092aa3be-b0f2-5d02-9b74-8171add88da6', '053bc673-3647-511c-948c-1e49c0de2275', 'en-us', 'First set markers and safe exits: stepping stones, a rope at the entrance, and a time limit for retreat.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('57ceb05c-5dc4-5153-b1bf-782380862439', '053bc673-3647-511c-948c-1e49c0de2275', 'hu-hu', 'Először jelöléseket és biztonságos kijáratokat teszel: kő-lépcsők, kötél a bejáratnál, időkorlát a visszavonuláshoz.')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('8d27cd0a-1db9-52e7-aed0-f7af6f5d4efa', 'f7af6022-5f07-5484-ae65-c0ef93cb9a84', 'p6', 'page', 7, 'Reușita', 'Lentila iese curat, fără să tulbure grota. Iepurele bate din lăbuțe: „Bravo! Lateral e mai bine decât drept!” Pui Lentila peste cristal, iar imaginea hărții se clarifică. Iepurele te privește hotărât: „Ținem legătura. Vreau acasă, pe Lunaria — promiți că mă ajuți?”\nAcum știți unde e Lunaria: nu, nu e Luna, ci o planetă îndepărtată de Pământ. Dacă am avea o sursă de energie, poate că am putea repara nava împreună…', 'images/tol/stories/seed@alchimalia.com/terra-s2/7.success.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('99947800-6262-50cd-812a-cda79ed9e221', '8d27cd0a-1db9-52e7-aed0-f7af6f5d4efa', 'ro-ro', 'Reușita', 'Lentila iese curat, fără să tulbure grota. Iepurele bate din lăbuțe: „Bravo! Lateral e mai bine decât drept!” Pui Lentila peste cristal, iar imaginea hărții se clarifică. Iepurele te privește hotărât: „Ținem legătura. Vreau acasă, pe Lunaria — promiți că mă ajuți?”\nAcum știți unde e Lunaria: nu, nu e Luna, ci o planetă îndepărtată de Pământ. Dacă am avea o sursă de energie, poate că am putea repara nava împreună…', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('1ac69c3a-5532-545d-9bf4-99941c0edb5a', '8d27cd0a-1db9-52e7-aed0-f7af6f5d4efa', 'en-us', 'Success', 'The Lens comes out cleanly, without disturbing the grotto. The rabbit claps its little paws: "Bravo! Sideways is better than straight!" You place the Lens over the crystal and the map''s image sharpens. The rabbit looks at you, determined: "Let''s keep in touch. I want to go home to Lunaria — do you promise to help?"
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
    ('ae1fc274-1da2-5806-b529-4e5ed563dac2', '8d27cd0a-1db9-52e7-aed0-f7af6f5d4efa', 'hu-hu', 'A siker', 'A Lencse tiszta, anélkül, hogy megzavarná a barlangot. A nyuszi tapsol: "Bravó! Oldalirányban jobb, mint egyenesen!" A Lencsét a kristályra teszed, és a térkép képe tisztul. A nyuszi határozottan rád néz: "Tartjuk a kapcsolatot. Haza akarok menni Lunariára — megígéred, hogy segítesz?"
Most már tudjátok, hol van Lunaria: nem, nem a Hold, hanem egy távoli bolygó a Földtől. Ha lenne energiaforrásunk, talán együtt megjavíthatnánk a hajót...', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
COMMIT;
