-- Auto-generated from Data/SeedData/Stories/seed@alchimalia.com (mode: indie)
-- Run date: 2025-11-27 12:00:32+02:00

BEGIN;

-- Story Definitions
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    ('7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'learn-math-s1', 'Probleme pe Mechanika', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/0.cover.png', 'Ajută-l pe Grubot să rezolve probleme pe Mechanika.', 'Matematică', NULL, NULL,
     1, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', '33333333-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333')
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
    ('c9d4b4de-44a4-5ae3-9b27-66cc045a8c71', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'ro-ro', 'Probleme pe Mechanika')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('12318cac-ce0b-55e0-9646-51ef87e2c1be', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'en-us', 'Trouble on Mechanika')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('a78ecd16-f1d0-5892-9340-dc891b934664', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'hu-hu', 'Problémák Mechanikán')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for learn-math-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('6655f304-e53a-5bf4-8563-138caa2c25c3', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', '1', 'page', 1, 'Alarm în Turnul de Control', 'Pe planeta Mechanika, o alarmă roșie a clipit în Turnul de Control. "Alertă! Generatorul North-3 a încetat să pulseze!" Fără el, cetățenii roboți nu se pot reîncărca. Grubot și-a îndreptat antenele: "Voi ajunge la Inima Energiei și o voi repune în funcțiune. Pas cu pas."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/1.workshop.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('68fe689a-6780-5bd1-afb6-8dac986e28c2', '6655f304-e53a-5bf4-8563-138caa2c25c3', 'ro-ro', 'Alarm în Turnul de Control', 'Pe planeta Mechanika, o alarmă roșie a clipit în Turnul de Control. "Alertă! Generatorul North-3 a încetat să pulseze!" Fără el, cetățenii roboți nu se pot reîncărca. Grubot și-a îndreptat antenele: "Voi ajunge la Inima Energiei și o voi repune în funcțiune. Pas cu pas."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('3cd53485-8f18-54c9-aa2e-c13ef8e723f7', '6655f304-e53a-5bf4-8563-138caa2c25c3', 'en-us', 'Alarm in the Control Tower', 'On planet Mechanika, a red alarm blinked in the Control Tower. “Alert! Generator North-3 stopped pulsing!” Without it, the robot citizens cannot recharge. Grubot straightened his antennas: “I’ll reach the Heart of Energy and bring it back online. Step by step.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('288677dc-6763-5379-94ce-cf314b94e6ce', '6655f304-e53a-5bf4-8563-138caa2c25c3', 'hu-hu', 'Riasztás a Vezérlőtoronyban', 'A Mechanika bolygón egy piros riasztás villogott a Vezérlőtornyban. "Riasztás! A North-3 generátor leállt!" Nélküle a robot polgárok nem tudnak újratöltődni. Grubot robot kiegyenesítette az antennáit: "El fogom érni az Energia Szívét és újra üzembe helyezem. Lépésről lépésre."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('95b1dc08-b4ba-55a1-ae58-aa73e70e2dfd', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', '2', 'page', 2, 'Coridorul Luminii', 'Drumul către Inimă trece printr-un tunel de lămpi colorate. Fiecare lampă strălucește doar când se adaugă energia potrivită. "Prea puțin și se estompează, prea mult și se sparge," a spus Grubot. "Echilibrul mai întâi, apoi înainte."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/2.bridge.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('ff4cc5bc-7a37-5747-a22a-484e29bf9a07', '95b1dc08-b4ba-55a1-ae58-aa73e70e2dfd', 'ro-ro', 'Coridorul Luminii', 'Drumul către Inimă trece printr-un tunel de lămpi colorate. Fiecare lampă strălucește doar când se adaugă energia potrivită. "Prea puțin și se estompează, prea mult și se sparge," a spus Grubot. "Echilibrul mai întâi, apoi înainte."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('73e3f77f-2cb7-56ff-8518-29bcbf0cec94', '95b1dc08-b4ba-55a1-ae58-aa73e70e2dfd', 'en-us', 'Corridor of Light', 'The path to the Heart passes through a tunnel of colored lamps. Each lamp glows only when the right energy is added. “Too little and it fades, too much and it pops,” said Grubot. “Balance first, then move on.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('4d7a96b9-b059-5b3f-9a33-56262e78b47c', '95b1dc08-b4ba-55a1-ae58-aa73e70e2dfd', 'hu-hu', 'A Fény Folyosója', 'Az Út a Szívhez színes lámpák alagútján halad át. Minden lámpa csak akkor világít, amikor a megfelelő energiát adják hozzá. "Túl kevés és elhalványul, túl sok és kipukkad," mondta Grubot. "Először az egyensúly, aztán tovább."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('4fe7fe84-479e-5a5b-a540-14082587fdf7', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'q1', 'quiz', 3, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/3.quiz.png', 'Pe panou, 3 scântei verzi sunt aprinse. Grubot adaugă încă 2 pentru a se potrivi cu modelul lămpii. Câte scântei strălucesc acum?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('015fe0cc-3401-5d5a-9169-2420e9710ca1', '4fe7fe84-479e-5a5b-a540-14082587fdf7', 'ro-ro', NULL, NULL, 'Pe panou, 3 scântei verzi sunt aprinse. Grubot adaugă încă 2 pentru a se potrivi cu modelul lămpii. Câte scântei strălucesc acum?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('b40257b2-e54d-515b-88a9-20c8ab97b163', '4fe7fe84-479e-5a5b-a540-14082587fdf7', 'en-us', NULL, NULL, 'On the panel, 3 green sparks are lit. Grubot adds 2 more to match the lamp’s pattern. How many sparks glow now?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('281df9c9-b640-50e5-86e4-c4d66922d0a1', '4fe7fe84-479e-5a5b-a540-14082587fdf7', 'hu-hu', NULL, NULL, 'A paneleken 3 zöld szikra világít. Grubot még 2-t ad hozzá, hogy megfeleljen a lámpa mintájának. Hány szikra világít most?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('f4b36a0f-3b0e-5404-b069-5bd469f0c71a', '4fe7fe84-479e-5a5b-a540-14082587fdf7', 'a', '4', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e0c93553-8d46-55f4-bcce-12cd8b36c3d0', 'f4b36a0f-3b0e-5404-b069-5bd469f0c71a', 'ro-ro', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a7bc8303-e260-5361-bfa4-e6e707218fdb', 'f4b36a0f-3b0e-5404-b069-5bd469f0c71a', 'en-us', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ac9b9657-874d-50a8-b1f1-38aa001a115f', 'f4b36a0f-3b0e-5404-b069-5bd469f0c71a', 'hu-hu', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('d33b410a-f4e3-55c5-9fe4-9bb394902775', '4fe7fe84-479e-5a5b-a540-14082587fdf7', 'b', '5', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('131f1ec5-2f6a-5b12-822c-6ebb4f8dab69', 'd33b410a-f4e3-55c5-9fe4-9bb394902775', 'Discovery', 'math_addition', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ca14f60b-f53a-5705-a459-62e76ba7c161', 'd33b410a-f4e3-55c5-9fe4-9bb394902775', 'ro-ro', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('8ac05130-3b4f-5fb8-84f1-1fd481e1a85a', 'd33b410a-f4e3-55c5-9fe4-9bb394902775', 'en-us', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e2c51647-b0e5-5796-9c06-edfcc96e1926', 'd33b410a-f4e3-55c5-9fe4-9bb394902775', 'hu-hu', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('84545e22-b3d9-5a15-9151-be36256aed07', '4fe7fe84-479e-5a5b-a540-14082587fdf7', 'c', '6', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('78c657f0-fa81-53d4-8649-c5f7e9f3739e', '84545e22-b3d9-5a15-9151-be36256aed07', 'ro-ro', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('cdca348e-1ce7-57ea-b37d-82c753b4e955', '84545e22-b3d9-5a15-9151-be36256aed07', 'en-us', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('763485b8-e69b-5615-96d5-66cbbd11d7f8', '84545e22-b3d9-5a15-9151-be36256aed07', 'hu-hu', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('465e5571-8c3a-59c4-bbdc-bb4b9d7fe4eb', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', '3', 'page', 4, 'Camera Echilibrului', 'Următoarea încăpere: manometre și țevi șuieră încet. Un mesaj clipă: "Presiune prea mare. Reduceți la nivel sigur." Grubot bate pe supape. "Nu prea mult, nu prea puțin—exact potrivit."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/4.tower.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('7c484868-d836-5851-b808-88ab346b87d8', '465e5571-8c3a-59c4-bbdc-bb4b9d7fe4eb', 'ro-ro', 'Camera Echilibrului', 'Următoarea încăpere: manometre și țevi șuieră încet. Un mesaj clipă: "Presiune prea mare. Reduceți la nivel sigur." Grubot bate pe supape. "Nu prea mult, nu prea puțin—exact potrivit."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('7e99068d-fc86-5ace-830a-8f2d55524041', '465e5571-8c3a-59c4-bbdc-bb4b9d7fe4eb', 'en-us', 'Chamber of Balance', 'Next room: gauges and pipes hiss softly. A message flickers: “Pressure too high. Reduce to safe level.” Grubot taps the valves. “Not too much, not too little—just right.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('0c723177-6dca-5a56-8509-ca1027a83dd2', '465e5571-8c3a-59c4-bbdc-bb4b9d7fe4eb', 'hu-hu', 'Az Egyensúly Kamrája', 'A következő szoba: manométerek és csövek halkan sziszegnek. Egy üzenet villog: "A nyomás túl magas. Csökkentsd biztonságos szintre." Grubot megkopogtatja a szelepeket. "Nem túl sok, nem túl kevés—éppen elég."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('1f48bdc0-ad4b-557b-8655-49c6b8247da8', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'q2', 'quiz', 5, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/5.quiz.png', 'Manometrul arată 10 unități de presiune. Grubot eliberează 4 unități pentru a ajunge la semnul sigur. Ce număr rămâne pe manometru?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('ec02e24f-198e-55d5-bda2-2ed00b6b0cf8', '1f48bdc0-ad4b-557b-8655-49c6b8247da8', 'ro-ro', NULL, NULL, 'Manometrul arată 10 unități de presiune. Grubot eliberează 4 unități pentru a ajunge la semnul sigur. Ce număr rămâne pe manometru?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('5a903892-e88a-5e59-bb90-7f89fcc409ad', '1f48bdc0-ad4b-557b-8655-49c6b8247da8', 'en-us', NULL, NULL, 'The gauge shows 10 units of pressure. Grubot releases 4 units to reach the safe mark. What number stays on the gauge?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('2650ad1d-2a1d-57ca-950d-f8aa0dae027b', '1f48bdc0-ad4b-557b-8655-49c6b8247da8', 'hu-hu', NULL, NULL, 'A manométer 10 egységnyi nyomást mutat. Grubot 4 egységet enged ki, hogy elérje a biztonságos jelet. Milyen szám marad a manométeren?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('467a9f90-ec38-5bdc-90f7-04ed23e95237', '1f48bdc0-ad4b-557b-8655-49c6b8247da8', 'a', '5', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('028a870b-6def-50a0-8c29-5fd87955bede', '467a9f90-ec38-5bdc-90f7-04ed23e95237', 'ro-ro', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d41a5717-4c40-571c-b6f2-4cfa74e71b24', '467a9f90-ec38-5bdc-90f7-04ed23e95237', 'en-us', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('9584a4a9-77d7-5b57-8253-b655a40e8364', '467a9f90-ec38-5bdc-90f7-04ed23e95237', 'hu-hu', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('ee9835d3-4687-5db9-84af-12f362362e25', '1f48bdc0-ad4b-557b-8655-49c6b8247da8', 'b', '6', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('dce53588-7e77-5020-a3ad-3e48b263349e', 'ee9835d3-4687-5db9-84af-12f362362e25', 'Discovery', 'math_subtraction', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('7169cf86-0b7e-5ea2-9482-3cc77f3e5bcd', 'ee9835d3-4687-5db9-84af-12f362362e25', 'ro-ro', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('e6740443-1501-57b9-9717-2f7f3883d40b', 'ee9835d3-4687-5db9-84af-12f362362e25', 'en-us', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('051bed15-df7e-516b-b78a-b5cac1d1ab9d', 'ee9835d3-4687-5db9-84af-12f362362e25', 'hu-hu', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('f32ffed5-f642-53fe-b2b8-26b9d8dfa971', '1f48bdc0-ad4b-557b-8655-49c6b8247da8', 'c', '8', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('6124e436-80e4-5cf6-873e-301c71fc77d6', 'f32ffed5-f642-53fe-b2b8-26b9d8dfa971', 'ro-ro', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('3dbbdf7b-023a-5bd6-8de6-97226522c97d', 'f32ffed5-f642-53fe-b2b8-26b9d8dfa971', 'en-us', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('92b39927-b853-5aa6-a555-58423c409181', 'f32ffed5-f642-53fe-b2b8-26b9d8dfa971', 'hu-hu', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('cd7aa34a-2eae-523d-a0e5-fa8c015a84e2', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', '4', 'page', 6, 'Sala Turbinelor Gemeni', 'Mai adânc în interior, două turbine uriașe zumzăie în întuneric. Inele de lumini mici înconjoară fiecare rotor. "Trebuie să le sincronizez ritmul," se gândește Grubot. "Toate luminile pulsând împreună."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/6.portals.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('2522f9f9-6296-5d9a-b96e-8f9680c625b7', 'cd7aa34a-2eae-523d-a0e5-fa8c015a84e2', 'ro-ro', 'Sala Turbinelor Gemeni', 'Mai adânc în interior, două turbine uriașe zumzăie în întuneric. Inele de lumini mici înconjoară fiecare rotor. "Trebuie să le sincronizez ritmul," se gândește Grubot. "Toate luminile pulsând împreună."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('83586dcb-0def-567b-9679-012d133e6475', 'cd7aa34a-2eae-523d-a0e5-fa8c015a84e2', 'en-us', 'Hall of Twin Turbines', 'Deeper inside, two giant turbines hum in the dark. Rings of tiny lights circle each rotor. “I must sync their rhythm,” thinks Grubot. “All lights pulsing together.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('32526b1a-a00a-528e-8dee-a11fefb1e990', 'cd7aa34a-2eae-523d-a0e5-fa8c015a84e2', 'hu-hu', 'Az Iker Turbinák Csarnoka', 'Mélyebben belül, két hatalmas turbina zümmög a sötétben. Apró fények gyűrűi ölelik körül minden rotort. "Szinronizálnom kell a ritmusukat," gondolja Grubot. "Minden fény együtt pulzál."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('25022f08-2a39-5f90-a3f0-3d5a145a9550', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'q3', 'quiz', 7, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/7.quiz.png', 'Fiecare turbină are 2 inele. Fiecare inel are 3 lumini care trebuie să strălucească împreună. Câte lumini ar trebui să strălucească în total pe ambele turbine?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('156619d2-21ce-5135-8520-2fbfd35676d2', '25022f08-2a39-5f90-a3f0-3d5a145a9550', 'ro-ro', NULL, NULL, 'Fiecare turbină are 2 inele. Fiecare inel are 3 lumini care trebuie să strălucească împreună. Câte lumini ar trebui să strălucească în total pe ambele turbine?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('c8da5c22-eb5e-5c19-b8b9-11390043fc05', '25022f08-2a39-5f90-a3f0-3d5a145a9550', 'en-us', NULL, NULL, 'Each turbine has 2 rings. Each ring has 3 lights that must glow together. How many lights should glow in total on both turbines?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('fd30ca0f-10e4-50d8-95fa-f135817c5511', '25022f08-2a39-5f90-a3f0-3d5a145a9550', 'hu-hu', NULL, NULL, 'Minden turbinának 2 gyűrűje van. Minden gyűrűnek 3 fénye van, amiknek együtt kell világítaniuk. Hány fénynek kell összesen világítania mindkét turbinán?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('170654fe-d9d6-5cc5-ad42-0272997e0fec', '25022f08-2a39-5f90-a3f0-3d5a145a9550', 'a', '5', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('7de2cac3-a0c3-556a-8134-a6d2040b699b', '170654fe-d9d6-5cc5-ad42-0272997e0fec', 'ro-ro', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('3c19af81-b63a-52f2-a8b9-2ae91974d852', '170654fe-d9d6-5cc5-ad42-0272997e0fec', 'en-us', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('75dd4722-f9a2-5d61-a3ab-14fb79883a7a', '170654fe-d9d6-5cc5-ad42-0272997e0fec', 'hu-hu', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('c98e6311-ade2-5548-a8b3-11359095cf19', '25022f08-2a39-5f90-a3f0-3d5a145a9550', 'b', '6', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('f826c3c3-4098-51ff-8725-d491c5f7ec35', 'c98e6311-ade2-5548-a8b3-11359095cf19', 'ro-ro', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('40a57178-49d7-5fc6-991e-9f2adccbc635', 'c98e6311-ade2-5548-a8b3-11359095cf19', 'en-us', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('423aae0f-99d5-5cf9-9547-42cc6ff1b219', 'c98e6311-ade2-5548-a8b3-11359095cf19', 'hu-hu', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('d59285a2-6288-5289-8006-67c650a5743f', '25022f08-2a39-5f90-a3f0-3d5a145a9550', 'c', '12', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('933cd5af-9d46-5831-9e89-fdae25975d3c', 'd59285a2-6288-5289-8006-67c650a5743f', 'Discovery', 'math_multiplication', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('8db5afd6-e68d-54d8-8c87-a89d56a500a9', 'd59285a2-6288-5289-8006-67c650a5743f', 'ro-ro', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('fdff2404-2a53-554b-902a-8298e931aa8f', 'd59285a2-6288-5289-8006-67c650a5743f', 'en-us', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('0095e63b-1daa-52d1-8b42-fa5de8dbc6c0', 'd59285a2-6288-5289-8006-67c650a5743f', 'hu-hu', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('aea1c854-83e6-5bbb-9506-dcde42c39576', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', '5', 'page', 8, 'Poarta Distribuției Cristalelor', 'O poartă înaltă blochează miezul. Patru cristale albastre așteaptă o parte echitabilă de putere. "Dacă împart fluxul în mod egal, lacătul se va deschide," spune Grubot. "Lumină egală pentru fiecare cristal."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/8.return.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('df66865a-d632-51a4-a2ab-5ca63e15961f', 'aea1c854-83e6-5bbb-9506-dcde42c39576', 'ro-ro', 'Poarta Distribuției Cristalelor', 'O poartă înaltă blochează miezul. Patru cristale albastre așteaptă o parte echitabilă de putere. "Dacă împart fluxul în mod egal, lacătul se va deschide," spune Grubot. "Lumină egală pentru fiecare cristal."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('792bc0f8-f9b5-5c1a-a331-07909c6dfe82', 'aea1c854-83e6-5bbb-9506-dcde42c39576', 'en-us', 'Crystal Sharing Gate', 'A tall gate blocks the core. Four blue crystals wait for a fair share of power. “If I divide the flow evenly, the lock will open,” says Grubot. “Equal light for every crystal.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('3f69ba50-682c-5b1d-a535-e580b1980573', 'aea1c854-83e6-5bbb-9506-dcde42c39576', 'hu-hu', 'A Kristály Megosztás Kapuja', 'Egy magas kapu elzárja a magot. Négy kék kristály várja a hatalom méltányos részét. "Ha egyenletesen osztom el az áramlást, a zár kinyílik," mondja Grubot. "Egyenlő fény minden kristály számára."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('893e0dec-174a-5182-888b-8f154e6b9195', '7dc93dbb-007f-5f52-b8bc-a1c42aac26b7', 'q4', 'quiz', 9, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/9.quiz.png', 'Sunt 12 sfere de energie de trimis în mod egal la 3 cristale. Câte sfere primește fiecare cristal?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9dddd4c8-ba76-5888-a71e-2c13f7c48376', '893e0dec-174a-5182-888b-8f154e6b9195', 'ro-ro', NULL, NULL, 'Sunt 12 sfere de energie de trimis în mod egal la 3 cristale. Câte sfere primește fiecare cristal?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('cc9f5659-2b12-51f1-a70e-25a41eb8640a', '893e0dec-174a-5182-888b-8f154e6b9195', 'en-us', NULL, NULL, 'There are 12 energy orbs to send equally to 3 crystals. How many orbs does each crystal receive?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('fb479634-2ddd-5185-b29d-56c7392d1d96', '893e0dec-174a-5182-888b-8f154e6b9195', 'hu-hu', NULL, NULL, '12 energia gömb van, amit egyenlően kell küldeni 3 kristályhoz. Hány gömböt kap minden kristály?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('133b447c-53ab-545e-ad19-b2c3efe54615', '893e0dec-174a-5182-888b-8f154e6b9195', 'a', '8', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1010e125-79bc-5bb9-939c-b10b03f2c40c', '133b447c-53ab-545e-ad19-b2c3efe54615', 'ro-ro', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('d290f310-8dcd-5311-814f-536eaa98170b', '133b447c-53ab-545e-ad19-b2c3efe54615', 'en-us', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('1d49c8d3-b335-5f8a-8906-351b6b6b3f89', '133b447c-53ab-545e-ad19-b2c3efe54615', 'hu-hu', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('d58d4d08-8a53-503e-9d22-d1eed911f9a9', '893e0dec-174a-5182-888b-8f154e6b9195', 'b', '4', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    ('c37ecb12-e4ec-5da3-ab5e-fca19ee78ca5', 'd58d4d08-8a53-503e-9d22-d1eed911f9a9', 'Discovery', 'math_division', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a82aabc9-5201-556c-ad90-d310da79ee46', 'd58d4d08-8a53-503e-9d22-d1eed911f9a9', 'ro-ro', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('654e4161-f37f-543b-a6f1-5ad205143e44', 'd58d4d08-8a53-503e-9d22-d1eed911f9a9', 'en-us', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('fb2b8970-6a59-506c-b369-06b927c6ab8b', 'd58d4d08-8a53-503e-9d22-d1eed911f9a9', 'hu-hu', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    ('e43414dd-3a6d-55cd-817c-8f26f66dff9c', '893e0dec-174a-5182-888b-8f154e6b9195', 'c', '12', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('ab539dbb-8c1f-59b6-8459-d59fdea5811a', 'e43414dd-3a6d-55cd-817c-8f26f66dff9c', 'ro-ro', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a5e4bfd7-6787-5bb5-b0b9-4ffada9cfb70', 'e43414dd-3a6d-55cd-817c-8f26f66dff9c', 'en-us', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    ('a49b9b02-3365-5b81-9ed4-36f631673679', 'e43414dd-3a6d-55cd-817c-8f26f66dff9c', 'hu-hu', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    ('1d0465b7-17dc-5d60-8f7b-dccb4de9c044', 'learn-to-read-s1', 'Puf-Puf și prietenul pierdut', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/0.cover.png', 'Exersează citirea în timpul acestei povești în care Puf-Puf descoperă un prieten nou.', 'Citit', NULL, NULL,
     1, 5, 1, TRUE, 1, 0,
     '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z', '33333333-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333')
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
    ('5a04eba2-b6d9-566e-a7c3-0c8147af268c', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', 'ro-ro', 'Puf-Puf și prietenul pierdut')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('35fb8bb7-8f6a-5032-b249-d8e5c5fbfaec', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', 'en-us', 'Puf-Puf and the Lost Friend')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    ('50334828-7a58-59ea-a47d-7460686971ba', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', 'hu-hu', 'Puf-Puf és az elveszett barát')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for learn-to-read-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('dec2d9e9-b019-5f4e-9711-be3fb5253699', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', '1', 'page', 1, 'Puf-Puf pleacă', 'Puf-Puf e un pisoi mic și pufos. Plouă peste pădure. Poartă o pelerină portocalie. „Ce plăcut e parfumul ploii!”, spune Puf-Puf.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/1.start.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('d741fe95-9c01-55b9-bde5-8c8eb5b137e0', 'dec2d9e9-b019-5f4e-9711-be3fb5253699', 'ro-ro', 'Puf-Puf pleacă', 'Puf-Puf e un pisoi mic și pufos. Plouă peste pădure. Poartă o pelerină portocalie. „Ce plăcut e parfumul ploii!”, spune Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('0883a05d-8e75-5227-a3f6-25ccae8d2556', 'dec2d9e9-b019-5f4e-9711-be3fb5253699', 'en-us', 'Puf-Puf starts', 'Puf-Puf is a small, fluffy cat. It pours over the forest. He wears an orange poncho. "The perfume of rain is so pleasant!" says Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('815e56df-5cad-5c1f-883d-4673d077d3bf', 'dec2d9e9-b019-5f4e-9711-be3fb5253699', 'hu-hu', 'Puf-Puf elindul', 'Puf-Puf egy pici, puha cica. Párás erdő felett esik az eső. Piros köpenyt visel. „Milyen pompás az eső illata!” – mondja Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('08117f5d-bc9d-58f5-9f75-d1f0a582bb9c', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', '2', 'page', 2, 'Un plânset mic', 'Se aude: „Pfff… pfff…”. Puf-Puf se oprește. „Cine plânge?” Din tufiș apare un iepuraș mic.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/2.cry.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('9e6f0791-0feb-508f-bd2f-45d6571ca65c', '08117f5d-bc9d-58f5-9f75-d1f0a582bb9c', 'ro-ro', 'Un plânset mic', 'Se aude: „Pfff… pfff…”. Puf-Puf se oprește. „Cine plânge?” Din tufiș apare un iepuraș mic.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('03b9903b-b9a8-5358-a46a-b4d27921d338', '08117f5d-bc9d-58f5-9f75-d1f0a582bb9c', 'en-us', 'A little cry', 'He hears: “Pfff… pfff…” Puf-Puf stops. “Who’s crying?” From the bush, a small bunny appears.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('6171e260-2bda-5e0f-b7f3-80900f48ce25', '08117f5d-bc9d-58f5-9f75-d1f0a582bb9c', 'hu-hu', 'Egy pici sírás', 'Hallatszik: „Pff… pff…”. Puf-Puf megáll. „Ki pityereg?” – kérdezi. A bokorból egy pici nyuszi bújik elő.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('1649c0a5-fac7-5483-a56a-b6d1ec2711d2', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', '3', 'page', 3, 'Prietenul pierdut', '„M-am pierdut”, spune iepurașul. „Nu plânge”, spune Puf-Puf. „Te voi duce pe potecă spre casă!”', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/3.lost-friend.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('cc47b210-3843-5158-a4aa-17edab1290e2', '1649c0a5-fac7-5483-a56a-b6d1ec2711d2', 'ro-ro', 'Prietenul pierdut', '„M-am pierdut”, spune iepurașul. „Nu plânge”, spune Puf-Puf. „Te voi duce pe potecă spre casă!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('0f279f7d-84c7-53c9-9634-0c3bbf6d9e11', '1649c0a5-fac7-5483-a56a-b6d1ec2711d2', 'en-us', 'The lost friend', '"I got lost," says the bunny. "Don''t worry," says Puf-Puf. "I''ll help you find the path home!"', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('04b71b78-a140-5e1b-9fca-b5e5bb2ded5c', '1649c0a5-fac7-5483-a56a-b6d1ec2711d2', 'hu-hu', 'Az elveszett pajtás', '„Elvesztem” – mondja a nyuszi. „Ne pityeregj!” – mondja Puf-Puf. „Elkísérlek a patak mentén hazáig!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('e2b7b608-265b-529f-98df-ae9a99ba1a96', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', '4', 'page', 4, 'Prin ploaie și pădure', 'Merg împreună prin pădure. Ploaia picură pe pietre. Puf-Puf ține umbrela. Iepurașul sare prin bălți: „Plip-plop!”', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/4.rain-forest.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('db97f251-7668-5235-aeb1-131b314b8b37', 'e2b7b608-265b-529f-98df-ae9a99ba1a96', 'ro-ro', 'Prin ploaie și pădure', 'Merg împreună prin pădure. Ploaia picură pe pietre. Puf-Puf ține umbrela. Iepurașul sare prin bălți: „Plip-plop!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('a0dd65d4-10b5-5eb6-982f-62f753f180a6', 'e2b7b608-265b-529f-98df-ae9a99ba1a96', 'en-us', 'Through rain and forest', 'They walk together through the forest. The rain drips on the pebbles. Puf-Puf holds the umbrella. The bunny jumps in puddles: “Plip-plop!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('57a6068e-924e-52fc-8939-5679734083c6', 'e2b7b608-265b-529f-98df-ae9a99ba1a96', 'hu-hu', 'Párás patak mellett', 'Együtt mennek az erdőben. A patak partján pocsolyák csillognak. Puf-Puf tartja az esernyőt. A nyuszi pancsol: „Plip-plop!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('9c946440-342b-58d8-9f90-6a48fe991188', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', '5', 'page', 5, 'Poiana cu pană', 'Ajung într-o poiană plină de flori. Pe jos e o pană strălucitoare. „Poiana asta e aproape!”, spune iepurașul. Zâmbește cu putere.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/5.meadow.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('3873a300-1802-5af4-820b-01144ad0c4cd', '9c946440-342b-58d8-9f90-6a48fe991188', 'ro-ro', 'Poiana cu pană', 'Ajung într-o poiană plină de flori. Pe jos e o pană strălucitoare. „Poiana asta e aproape!”, spune iepurașul. Zâmbește cu putere.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('8d6799e2-f5e3-5197-81f0-4f16881c88f5', '9c946440-342b-58d8-9f90-6a48fe991188', 'en-us', 'The meadow with a feather', 'They reach a pretty meadow full of flowers. On the ground lies a shiny peacock feather. "This place is near my home!" says the bunny, smiling proudly.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('c311a0a5-a9d8-5f3e-a234-4f20ced570c9', '9c946440-342b-58d8-9f90-6a48fe991188', 'hu-hu', 'A pillangós pázsit', 'Egy virágos pázsitra érnek. A fűben egy pici, puha toll fekszik. „Ez a pázsit már közel van!” – mondja a nyuszi. Mosolyog, mint a nap.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    ('5dc0ab73-70be-5e6e-beee-f6e3a0a7f1e5', '1d0465b7-17dc-5d60-8f7b-dccb4de9c044', '6', 'page', 6, 'Povestea prieteniei', '„Mulțumesc, Puf-Puf!” „Prietenii se ajută!”, spune pisoiul. Ploaia se oprește. Pe cer, un curcubeu. P ca Puf-Puf, P ca prieten!', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/6.end.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    ('599f4515-1817-5b53-b6a7-84431cf233ae', '5dc0ab73-70be-5e6e-beee-f6e3a0a7f1e5', 'ro-ro', 'Povestea prieteniei', '„Mulțumesc, Puf-Puf!” „Prietenii se ajută!”, spune pisoiul. Ploaia se oprește. Pe cer, un curcubeu. P ca Puf-Puf, P ca prieten!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('ed7c26bf-553c-54a2-be38-29be8fc88f39', '5dc0ab73-70be-5e6e-beee-f6e3a0a7f1e5', 'en-us', 'The story of friendship', '“Thank you, Puf-Puf!” “Friends help each other,” says the kitten. The rain stops. A rainbow shines above. **P** for Puf-Puf, **P** for partner, **P** for peace!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    ('1ad96194-c3ef-529e-9fee-efec3a33f136', '5dc0ab73-70be-5e6e-beee-f6e3a0a7f1e5', 'hu-hu', 'A pajtás története', '„Köszönöm, Puf-Puf!” – mondja a nyuszi. „A pajtások segítenek egymásnak!” – feleli a cica. Az eső eláll, és egy csodaszép szivárvány jelenik meg ahogy a nap kisüt. P mint Puf-Puf, P mint pajtás!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
COMMIT;
