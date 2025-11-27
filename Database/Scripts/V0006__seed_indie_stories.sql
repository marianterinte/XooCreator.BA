-- Auto-generated from Data/SeedData/Stories/seed@alchimalia.com (mode: indie)
-- Run date: 2025-11-27 10:07:13+02:00

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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'learn-math-s1', 'Probleme pe Mechanika', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/0.cover.png', 'Ajută-l pe Grubot să rezolve probleme pe Mechanika.', 'Matematică', NULL, NULL,
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:learn-math-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'ro-ro', 'Probleme pe Mechanika')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:learn-math-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'en-us', 'Trouble on Mechanika')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:learn-math-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'hu-hu', 'Problémák Mechanikán')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for learn-math-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), '1', 'page', 1, 'Alarm în Turnul de Control', 'Pe planeta Mechanika, o alarmă roșie a clipit în Turnul de Control. "Alertă! Generatorul North-3 a încetat să pulseze!" Fără el, cetățenii roboți nu se pot reîncărca. Grubot și-a îndreptat antenele: "Voi ajunge la Inima Energiei și o voi repune în funcțiune. Pas cu pas."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/1.workshop.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:1'), 'ro-ro', 'Alarm în Turnul de Control', 'Pe planeta Mechanika, o alarmă roșie a clipit în Turnul de Control. "Alertă! Generatorul North-3 a încetat să pulseze!" Fără el, cetățenii roboți nu se pot reîncărca. Grubot și-a îndreptat antenele: "Voi ajunge la Inima Energiei și o voi repune în funcțiune. Pas cu pas."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:1'), 'en-us', 'Alarm in the Control Tower', 'On planet Mechanika, a red alarm blinked in the Control Tower. “Alert! Generator North-3 stopped pulsing!” Without it, the robot citizens cannot recharge. Grubot straightened his antennas: “I’ll reach the Heart of Energy and bring it back online. Step by step.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:1'), 'hu-hu', 'Riasztás a Vezérlőtoronyban', 'A Mechanika bolygón egy piros riasztás villogott a Vezérlőtornyban. "Riasztás! A North-3 generátor leállt!" Nélküle a robot polgárok nem tudnak újratöltődni. Grubot robot kiegyenesítette az antennáit: "El fogom érni az Energia Szívét és újra üzembe helyezem. Lépésről lépésre."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), '2', 'page', 2, 'Coridorul Luminii', 'Drumul către Inimă trece printr-un tunel de lămpi colorate. Fiecare lampă strălucește doar când se adaugă energia potrivită. "Prea puțin și se estompează, prea mult și se sparge," a spus Grubot. "Echilibrul mai întâi, apoi înainte."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/2.bridge.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:2'), 'ro-ro', 'Coridorul Luminii', 'Drumul către Inimă trece printr-un tunel de lămpi colorate. Fiecare lampă strălucește doar când se adaugă energia potrivită. "Prea puțin și se estompează, prea mult și se sparge," a spus Grubot. "Echilibrul mai întâi, apoi înainte."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:2'), 'en-us', 'Corridor of Light', 'The path to the Heart passes through a tunnel of colored lamps. Each lamp glows only when the right energy is added. “Too little and it fades, too much and it pops,” said Grubot. “Balance first, then move on.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:2'), 'hu-hu', 'A Fény Folyosója', 'Az Út a Szívhez színes lámpák alagútján halad át. Minden lámpa csak akkor világít, amikor a megfelelő energiát adják hozzá. "Túl kevés és elhalványul, túl sok és kipukkad," mondta Grubot. "Először az egyensúly, aztán tovább."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'q1', 'quiz', 3, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/3.quiz.png', 'Pe panou, 3 scântei verzi sunt aprinse. Grubot adaugă încă 2 pentru a se potrivi cu modelul lămpii. Câte scântei strălucesc acum?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), 'ro-ro', NULL, NULL, 'Pe panou, 3 scântei verzi sunt aprinse. Grubot adaugă încă 2 pentru a se potrivi cu modelul lămpii. Câte scântei strălucesc acum?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), 'en-us', NULL, NULL, 'On the panel, 3 green sparks are lit. Grubot adds 2 more to match the lamp’s pattern. How many sparks glow now?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), 'hu-hu', NULL, NULL, 'A paneleken 3 zöld szikra világít. Grubot még 2-t ad hozzá, hogy megfeleljen a lámpa mintájának. Hány szikra világít most?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), 'a', '4', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:a'), 'ro-ro', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:a'), 'en-us', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:a'), 'hu-hu', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), 'b', '5', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:learn-math-s1:q1:b:Discovery:math_addition:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:b'), 'Discovery', 'math_addition', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:b'), 'ro-ro', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:b'), 'en-us', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:b'), 'hu-hu', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q1'), 'c', '6', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:c'), 'ro-ro', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:c'), 'en-us', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q1:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q1:c'), 'hu-hu', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), '3', 'page', 4, 'Camera Echilibrului', 'Următoarea încăpere: manometre și țevi șuieră încet. Un mesaj clipă: "Presiune prea mare. Reduceți la nivel sigur." Grubot bate pe supape. "Nu prea mult, nu prea puțin—exact potrivit."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/4.tower.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:3'), 'ro-ro', 'Camera Echilibrului', 'Următoarea încăpere: manometre și țevi șuieră încet. Un mesaj clipă: "Presiune prea mare. Reduceți la nivel sigur." Grubot bate pe supape. "Nu prea mult, nu prea puțin—exact potrivit."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:3'), 'en-us', 'Chamber of Balance', 'Next room: gauges and pipes hiss softly. A message flickers: “Pressure too high. Reduce to safe level.” Grubot taps the valves. “Not too much, not too little—just right.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:3'), 'hu-hu', 'Az Egyensúly Kamrája', 'A következő szoba: manométerek és csövek halkan sziszegnek. Egy üzenet villog: "A nyomás túl magas. Csökkentsd biztonságos szintre." Grubot megkopogtatja a szelepeket. "Nem túl sok, nem túl kevés—éppen elég."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'q2', 'quiz', 5, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/5.quiz.png', 'Manometrul arată 10 unități de presiune. Grubot eliberează 4 unități pentru a ajunge la semnul sigur. Ce număr rămâne pe manometru?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), 'ro-ro', NULL, NULL, 'Manometrul arată 10 unități de presiune. Grubot eliberează 4 unități pentru a ajunge la semnul sigur. Ce număr rămâne pe manometru?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), 'en-us', NULL, NULL, 'The gauge shows 10 units of pressure. Grubot releases 4 units to reach the safe mark. What number stays on the gauge?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), 'hu-hu', NULL, NULL, 'A manométer 10 egységnyi nyomást mutat. Grubot 4 egységet enged ki, hogy elérje a biztonságos jelet. Milyen szám marad a manométeren?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), 'a', '5', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:a'), 'ro-ro', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:a'), 'en-us', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:a'), 'hu-hu', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), 'b', '6', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:learn-math-s1:q2:b:Discovery:math_subtraction:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:b'), 'Discovery', 'math_subtraction', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:b'), 'ro-ro', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:b'), 'en-us', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:b'), 'hu-hu', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q2'), 'c', '8', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:c'), 'ro-ro', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:c'), 'en-us', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q2:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q2:c'), 'hu-hu', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), '4', 'page', 6, 'Sala Turbinelor Gemeni', 'Mai adânc în interior, două turbine uriașe zumzăie în întuneric. Inele de lumini mici înconjoară fiecare rotor. "Trebuie să le sincronizez ritmul," se gândește Grubot. "Toate luminile pulsând împreună."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/6.portals.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:4'), 'ro-ro', 'Sala Turbinelor Gemeni', 'Mai adânc în interior, două turbine uriașe zumzăie în întuneric. Inele de lumini mici înconjoară fiecare rotor. "Trebuie să le sincronizez ritmul," se gândește Grubot. "Toate luminile pulsând împreună."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:4'), 'en-us', 'Hall of Twin Turbines', 'Deeper inside, two giant turbines hum in the dark. Rings of tiny lights circle each rotor. “I must sync their rhythm,” thinks Grubot. “All lights pulsing together.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:4'), 'hu-hu', 'Az Iker Turbinák Csarnoka', 'Mélyebben belül, két hatalmas turbina zümmög a sötétben. Apró fények gyűrűi ölelik körül minden rotort. "Szinronizálnom kell a ritmusukat," gondolja Grubot. "Minden fény együtt pulzál."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'q3', 'quiz', 7, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/7.quiz.png', 'Fiecare turbină are 2 inele. Fiecare inel are 3 lumini care trebuie să strălucească împreună. Câte lumini ar trebui să strălucească în total pe ambele turbine?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), 'ro-ro', NULL, NULL, 'Fiecare turbină are 2 inele. Fiecare inel are 3 lumini care trebuie să strălucească împreună. Câte lumini ar trebui să strălucească în total pe ambele turbine?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), 'en-us', NULL, NULL, 'Each turbine has 2 rings. Each ring has 3 lights that must glow together. How many lights should glow in total on both turbines?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), 'hu-hu', NULL, NULL, 'Minden turbinának 2 gyűrűje van. Minden gyűrűnek 3 fénye van, amiknek együtt kell világítaniuk. Hány fénynek kell összesen világítania mindkét turbinán?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), 'a', '5', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:a'), 'ro-ro', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:a'), 'en-us', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:a'), 'hu-hu', '5')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), 'b', '6', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:b'), 'ro-ro', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:b'), 'en-us', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:b'), 'hu-hu', '6')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q3'), 'c', '12', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:learn-math-s1:q3:c:Discovery:math_multiplication:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:c'), 'Discovery', 'math_multiplication', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:c'), 'ro-ro', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:c'), 'en-us', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q3:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q3:c'), 'hu-hu', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), '5', 'page', 8, 'Poarta Distribuției Cristalelor', 'O poartă înaltă blochează miezul. Patru cristale albastre așteaptă o parte echitabilă de putere. "Dacă împart fluxul în mod egal, lacătul se va deschide," spune Grubot. "Lumină egală pentru fiecare cristal."', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/8.return.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:5'), 'ro-ro', 'Poarta Distribuției Cristalelor', 'O poartă înaltă blochează miezul. Patru cristale albastre așteaptă o parte echitabilă de putere. "Dacă împart fluxul în mod egal, lacătul se va deschide," spune Grubot. "Lumină egală pentru fiecare cristal."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:5'), 'en-us', 'Crystal Sharing Gate', 'A tall gate blocks the core. Four blue crystals wait for a fair share of power. “If I divide the flow evenly, the lock will open,” says Grubot. “Equal light for every crystal.”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:5'), 'hu-hu', 'A Kristály Megosztás Kapuja', 'Egy magas kapu elzárja a magot. Négy kék kristály várja a hatalom méltányos részét. "Ha egyenletesen osztom el az áramlást, a zár kinyílik," mondja Grubot. "Egyenlő fény minden kristály számára."', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-math-s1'), 'q4', 'quiz', 9, NULL, NULL, 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-math-s1/9.quiz.png', 'Sunt 12 sfere de energie de trimis în mod egal la 3 cristale. Câte sfere primește fiecare cristal?', '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), 'ro-ro', NULL, NULL, 'Sunt 12 sfere de energie de trimis în mod egal la 3 cristale. Câte sfere primește fiecare cristal?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), 'en-us', NULL, NULL, 'There are 12 energy orbs to send equally to 3 crystals. How many orbs does each crystal receive?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-math-s1:q4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), 'hu-hu', NULL, NULL, '12 energia gömb van, amit egyenlően kell küldeni 3 kristályhoz. Hány gömböt kap minden kristály?', NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:a'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), 'a', '8', NULL, 1, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:a|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:a'), 'ro-ro', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:a|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:a'), 'en-us', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:a|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:a'), 'hu-hu', '8')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:b'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), 'b', '4', NULL, 2, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTokens"
    ("Id", "StoryAnswerId", "Type", "Value", "Quantity")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytoken:learn-math-s1:q4:b:Discovery:math_division:0'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:b'), 'Discovery', 'math_division', 1)
ON CONFLICT ("Id") DO UPDATE
SET "Type" = EXCLUDED."Type",
    "Value" = EXCLUDED."Value",
    "Quantity" = EXCLUDED."Quantity";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:b|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:b'), 'ro-ro', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:b|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:b'), 'en-us', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:b|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:b'), 'hu-hu', '4')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswers"
    ("Id", "StoryTileId", "AnswerId", "Text", "TokensJson", "SortOrder", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:c'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-math-s1:q4'), 'c', '12', NULL, 3, '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "Text" = EXCLUDED."Text",
    "SortOrder" = EXCLUDED."SortOrder";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:c|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:c'), 'ro-ro', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:c|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:c'), 'en-us', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryAnswerTranslations"
    ("Id", "StoryAnswerId", "LanguageCode", "Text")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswertr:learn-math-s1:q4:c|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storyanswer:learn-math-s1:q4:c'), 'hu-hu', '12')
ON CONFLICT ("StoryAnswerId", "LanguageCode") DO UPDATE
SET "Text" = EXCLUDED."Text";
INSERT INTO alchimalia_schema."StoryDefinitions"
    ("Id", "StoryId", "Title", "CoverImageUrl", "Summary", "StoryTopic", "AuthorName", "ClassicAuthorId",
     "StoryType", "Status", "SortOrder", "IsActive", "Version", "PriceInCredits",
     "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), 'learn-to-read-s1', 'Puf-Puf și prietenul pierdut', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/0.cover.png', 'Exersează citirea în timpul acestei povești în care Puf-Puf descoperă un prieten nou.', 'Citit', NULL, NULL,
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:learn-to-read-s1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), 'ro-ro', 'Puf-Puf și prietenul pierdut')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:learn-to-read-s1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), 'en-us', 'Puf-Puf and the Lost Friend')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";
INSERT INTO alchimalia_schema."StoryDefinitionTranslations"
    ("Id", "StoryDefinitionId", "LanguageCode", "Title")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydeftr:learn-to-read-s1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), 'hu-hu', 'Puf-Puf és az elveszett barát')
ON CONFLICT ("StoryDefinitionId", "LanguageCode") DO UPDATE
SET "Title" = EXCLUDED."Title";

-- Story Tiles for learn-to-read-s1
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:1'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), '1', 'page', 1, 'Puf-Puf pleacă', 'Puf-Puf e un pisoi mic și pufos. Plouă peste pădure. Poartă o pelerină portocalie. „Ce plăcut e parfumul ploii!”, spune Puf-Puf.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/1.start.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:1|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:1'), 'ro-ro', 'Puf-Puf pleacă', 'Puf-Puf e un pisoi mic și pufos. Plouă peste pădure. Poartă o pelerină portocalie. „Ce plăcut e parfumul ploii!”, spune Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:1|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:1'), 'en-us', 'Puf-Puf starts', 'Puf-Puf is a small, fluffy cat. It pours over the forest. He wears an orange poncho. "The perfume of rain is so pleasant!" says Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:1|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:1'), 'hu-hu', 'Puf-Puf elindul', 'Puf-Puf egy pici, puha cica. Párás erdő felett esik az eső. Piros köpenyt visel. „Milyen pompás az eső illata!” – mondja Puf-Puf.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:2'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), '2', 'page', 2, 'Un plânset mic', 'Se aude: „Pfff… pfff…”. Puf-Puf se oprește. „Cine plânge?” Din tufiș apare un iepuraș mic.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/2.cry.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:2|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:2'), 'ro-ro', 'Un plânset mic', 'Se aude: „Pfff… pfff…”. Puf-Puf se oprește. „Cine plânge?” Din tufiș apare un iepuraș mic.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:2|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:2'), 'en-us', 'A little cry', 'He hears: “Pfff… pfff…” Puf-Puf stops. “Who’s crying?” From the bush, a small bunny appears.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:2|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:2'), 'hu-hu', 'Egy pici sírás', 'Hallatszik: „Pff… pff…”. Puf-Puf megáll. „Ki pityereg?” – kérdezi. A bokorból egy pici nyuszi bújik elő.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:3'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), '3', 'page', 3, 'Prietenul pierdut', '„M-am pierdut”, spune iepurașul. „Nu plânge”, spune Puf-Puf. „Te voi duce pe potecă spre casă!”', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/3.lost-friend.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:3|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:3'), 'ro-ro', 'Prietenul pierdut', '„M-am pierdut”, spune iepurașul. „Nu plânge”, spune Puf-Puf. „Te voi duce pe potecă spre casă!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:3|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:3'), 'en-us', 'The lost friend', '"I got lost," says the bunny. "Don''t worry," says Puf-Puf. "I''ll help you find the path home!"', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:3|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:3'), 'hu-hu', 'Az elveszett pajtás', '„Elvesztem” – mondja a nyuszi. „Ne pityeregj!” – mondja Puf-Puf. „Elkísérlek a patak mentén hazáig!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:4'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), '4', 'page', 4, 'Prin ploaie și pădure', 'Merg împreună prin pădure. Ploaia picură pe pietre. Puf-Puf ține umbrela. Iepurașul sare prin bălți: „Plip-plop!”', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/4.rain-forest.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:4|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:4'), 'ro-ro', 'Prin ploaie și pădure', 'Merg împreună prin pădure. Ploaia picură pe pietre. Puf-Puf ține umbrela. Iepurașul sare prin bălți: „Plip-plop!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:4|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:4'), 'en-us', 'Through rain and forest', 'They walk together through the forest. The rain drips on the pebbles. Puf-Puf holds the umbrella. The bunny jumps in puddles: “Plip-plop!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:4|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:4'), 'hu-hu', 'Párás patak mellett', 'Együtt mennek az erdőben. A patak partján pocsolyák csillognak. Puf-Puf tartja az esernyőt. A nyuszi pancsol: „Plip-plop!”', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:5'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), '5', 'page', 5, 'Poiana cu pană', 'Ajung într-o poiană plină de flori. Pe jos e o pană strălucitoare. „Poiana asta e aproape!”, spune iepurașul. Zâmbește cu putere.', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/5.meadow.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:5|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:5'), 'ro-ro', 'Poiana cu pană', 'Ajung într-o poiană plină de flori. Pe jos e o pană strălucitoare. „Poiana asta e aproape!”, spune iepurașul. Zâmbește cu putere.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:5|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:5'), 'en-us', 'The meadow with a feather', 'They reach a pretty meadow full of flowers. On the ground lies a shiny peacock feather. "This place is near my home!" says the bunny, smiling proudly.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:5|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:5'), 'hu-hu', 'A pillangós pázsit', 'Egy virágos pázsitra érnek. A fűben egy pici, puha toll fekszik. „Ez a pázsit már közel van!” – mondja a nyuszi. Mosolyog, mint a nap.', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTiles"
    ("Id", "StoryDefinitionId", "TileId", "Type", "SortOrder", "Caption", "Text", "ImageUrl", "Question", "CreatedAt", "UpdatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:6'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storydef:learn-to-read-s1'), '6', 'page', 6, 'Povestea prieteniei', '„Mulțumesc, Puf-Puf!” „Prietenii se ajută!”, spune pisoiul. Ploaia se oprește. Pe cer, un curcubeu. P ca Puf-Puf, P ca prieten!', 'images/tales-of-alchimalia/stories/seed@alchimalia.com/learn-to-read-s1/6.end.png', NULL, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z')
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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:6|ro-ro'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:6'), 'ro-ro', 'Povestea prieteniei', '„Mulțumesc, Puf-Puf!” „Prietenii se ajută!”, spune pisoiul. Ploaia se oprește. Pe cer, un curcubeu. P ca Puf-Puf, P ca prieten!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:6|en-us'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:6'), 'en-us', 'The story of friendship', '“Thank you, Puf-Puf!” “Friends help each other,” says the kitten. The rain stops. A rainbow shines above. **P** for Puf-Puf, **P** for partner, **P** for peace!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
INSERT INTO alchimalia_schema."StoryTileTranslations"
    ("Id", "StoryTileId", "LanguageCode", "Caption", "Text", "Question", "AudioUrl", "VideoUrl")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytiletr:learn-to-read-s1:6|hu-hu'), uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'storytile:learn-to-read-s1:6'), 'hu-hu', 'A pajtás története', '„Köszönöm, Puf-Puf!” – mondja a nyuszi. „A pajtások segítenek egymásnak!” – feleli a cica. Az eső eláll, és egy csodaszép szivárvány jelenik meg ahogy a nap kisüt. P mint Puf-Puf, P mint pajtás!', NULL, NULL, NULL)
ON CONFLICT ("StoryTileId", "LanguageCode") DO UPDATE
SET "Caption" = EXCLUDED."Caption",
    "Text" = EXCLUDED."Text",
    "Question" = EXCLUDED."Question",
    "AudioUrl" = EXCLUDED."AudioUrl",
    "VideoUrl" = EXCLUDED."VideoUrl";
COMMIT;
