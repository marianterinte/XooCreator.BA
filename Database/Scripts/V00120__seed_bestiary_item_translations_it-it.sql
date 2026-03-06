-- Auto-generated from Data/SeedData/Discovery/i18n/it-it/discover-bestiary.json
-- Locale: it-it
-- Run date: 2026-03-03T09:03:26.680Z
-- This script seeds BestiaryItemTranslations for it-it bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('840efb4f-5506-4921-9f1c-62ec1bb5af0e', '5d1b4419-c890-5462-bcb7-242dbe123890', 'it-it', 'Conigliraffice', 'Il Conigliraffice apparve quando due conigli sognarono insieme di raggiungere il cielo attraverso gli occhi di una giraffa. L''Albero della Luce li uni in un unico essere. E uno spirito di curiosita e coraggio, che salta tra i mondi portando luce nelle ombre.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2e46c49a-f484-45f8-9089-0bf467678c99', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'it-it', 'Coniglippotiche', 'Il Coniglippotiche nacque dal sogno di due conigli che chiedevano la forza di un ippopotamo per proteggere i loro amici. L''Albero della Luce uni le loro anime. E uno spirito giocoso ma protettivo, che porta coraggio ai piccoli e pace agli smarriti.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f90f7f36-06e7-4d6d-8f84-921932ea8e44', 'fc1b57f3-4751-51f5-a497-174648433b88', 'it-it', 'Conigliraffonica', 'Il Conigliraffonica emerse quando due conigli desiderarono vedere il mondo dall''alto. L''Albero della Luce rispose donando loro il corpo di una giraffa, ma preservo i loro cuori giocosi e i loro volti. E uno spirito di sogni e speranza, portando nei suoi occhi la luce dell''infanzia.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4c85f0c7-82c1-4c0f-9264-e591c62b90b2', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'it-it', 'Coniglibiraffa', 'Il Coniglibiraffa apparve quando un coniglio volle condividere il suo gioco con due giraffe. L''Albero della Luce li uni in un unico essere. E uno spirito di curiosita e amicizia, che unisce la vivacita con la grandezza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5b456c79-da1a-4da7-83b6-1af47bc8a151', 'b67d8c08-04ec-539d-8787-27e838807428', 'it-it', 'Conigliraffippo', 'Il Conigliraffippo e un eroe leggendario, nato da un''alleanza impossibile: un coniglio, una giraffa e un ippopotamo che scelsero di unire i loro destini. L''Albero della Luce vide il loro coraggio e dono loro un unico cuore. Con le braccia del coniglio aiuta e protegge. Con il corpo della giraffa vede lontano. Con la testa dell''ippopotamo ispira tranquillita e saggezza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bc05a140-6941-4c76-898e-dff16f419e20', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'it-it', 'Conigliraffe', 'Tra i rami dell''Albero della Luce nacque il Conigliraffe, creatura unica, meta coniglio, meta giraffa. Le sue braccia corte e agili scavano tunnel e accendono fuochi vivaci, mentre il suo corpo slanciato, adornato di macchie luminose, scruta orizzonti lontani che altri non possono raggiungere.

La leggenda narra che il Conigliraffe apparve quando un piccolo coniglio coraggioso si nascose tra le radici di una vecchia giraffa mentre l''oscurita divorava la savana. Dal loro abbraccio disperato, l''Albero della Luce intreccio due spiriti in un unico corpo.

Il Conigliraffe e il guardiano degli orizzonti. Quando corre tra l''erba, la terra trasalisce di gioia. Quando si alza in difesa, le ombre si ritirano vergognose. Ma la sua vera forza non sta nella velocita o nell''altezza, bensı nell''amicizia che offre a chi vaga tra i rami del mondo.

Gli spiriti antichi dicono che il Conigliraffe sara sempre la dove qualcuno tenta di riparare cio che e stato spezzato, portando coraggio e speranza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a2a30391-f916-4a83-9553-aaa048d5265b', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'it-it', 'Coniglippoturel', 'Il Coniglippoturel nacque quando un coniglio cerco l''amicizia di un ippopotamo, e l''Albero della Luce uni i loro destini. Con braccia agili e testa giocosa, ma corpo solido, e lo spirito dell''equilibrio tra fragilita e forza, portando allegria e protezione ai rami dei mondi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7a23aafa-c1e7-4ca6-a3fd-9c77306d3747', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'it-it', 'Coniglippogirafa', 'Il Coniglippogirafa nacque nel mezzo di una tempesta di luce, quando tre amici tentarono di riparare un ramo spezzato dell''Albero della Luce. Il loro sacrificio si trasformo in magia. E un guardiano gentile, che porta in se la speranza e il coraggio dell''amicizia.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8303a8c3-0d4b-44fb-90d7-6fa689eb5e39', '558b9240-3c3a-5017-a85a-706a397288fd', 'it-it', 'Coniglippotix', 'Il Coniglippotix nacque quando un coniglio porto gioia a due ippopotami. L''Albero della Luce li uni in un essere protettivo e allegro. E uno spirito di amicizia e coraggio, portando luce nell''oscurita.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1a0407fb-e835-484a-a0b4-4fdde8247873', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'it-it', 'Coniglippo', 'Il Coniglippo nacque quando un coniglio e un ippopotamo si incontrarono nel mezzo di una tempesta di acque scure. Dalla loro inaspettata amicizia, l''Albero della Luce intreccio l''agilita del coniglio con la forza dell''ippopotamo. Il Coniglippo e gentile e protettivo: con le sue braccia veloci scava rifugi, e con il suo corpo possente apre sentieri tra le acque. Porta calma dove regna la paura e costruisce ponti di fiducia tra i mondi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8690026f-dcb1-4a7d-a77d-e78275e97b9a', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'it-it', 'Conigliraffa', 'Il Conigliraffa fu creato quando un coniglio e una giraffa tentarono insieme di illuminare un ramo caduto dell''Albero della Luce. La magia li uni in un unico corpo. E uno spirito di coraggio e amicizia, capace di portare speranza e luce nell''oscurita.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('aa4e49c9-05f4-4901-871b-e85e1dcd29e4', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'it-it', 'Conigliippo', 'Il Conigliippo emerse quando un coniglio e un ippopotamo si fronteggiarono insieme sotto la tempesta della notte. L''Albero della Luce li intreccio in un unico corpo, preservando l''agilita del coniglio e la saggezza dell''ippopotamo. E un simbolo di solidarieta e amicizia tra il piccolo e il grande.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('dd281863-718f-47d7-a222-d9c98bc88c62', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'it-it', 'Giraffonigli', 'Il Giraffonigli apparve quando una giraffa abbraccio due conigli. L''Albero della Luce li uni in un unico essere. E uno spirito di compassione e gioco, portando allegria tra i rami dei mondi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0172c275-fd0e-416b-97f5-f74a21079ebf', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'it-it', 'Giraffepice', 'Il Giraffepice nacque quando due giraffe protessero un piccolo coniglio. L''Albero della Luce li uni in un unico corpo, donando loro l''agilita della terra e la visione del cielo. E uno spirito di amicizia e saggezza, capace di unire mondi opposti.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('39963299-c9bf-43ca-ac56-289b0b863b97', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'it-it', 'Giraffippamo', 'Il Giraffippamo apparve quando tre creature giurarono di proteggere insieme i rami dell''albero. Dalla loro amicizia, l''Albero della Luce creo un corpo comune. E uno spirito di unita e amicizia, portando la speranza di unire mondi diversi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7dab7f31-2e63-46c1-98f0-eeff9cac0357', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'it-it', 'Girafconiglio', 'Il Girafconiglio nacque quando le braccia di una giraffa abbracciarono un piccolo coniglio tremante nella notte. Da quel momento, l''Albero della Luce li uni in un ibrido insolito. Il Girafconiglio e giocoso e senza paura: usa le sue braccia lunghe per raccogliere le stelle e il corpo del coniglio per saltare tra le ombre. E uno spirito di gioia che porta risate e luce ovunque vada.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f78f7924-a180-4389-b888-4af7cacc6128', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'it-it', 'Giraffunniche', 'Il Giraffunniche emerse quando due giraffe vollero condividere la loro altezza con un piccolo coniglio. L''Albero della Luce li uni in una creatura di amicizia e gioia. Porta speranza ai piccoli, mostrando che la grandezza puo essere condivisa.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('431a7d1b-6bf6-4592-9049-4342b260b1bb', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'it-it', 'Doppiogiraffo', 'Il Doppiogiraffo nacque quando due spiriti di giraffa vegliarono insieme su un ippopotamo smarrito. L''Albero della Luce li uni in un unico essere, alto e robusto, ma gentile. E il guardiano degli orizzonti, portando equilibrio tra la forza e la visione.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4f2870dc-ae8d-4f7c-8ec8-2428aefa3cb4', 'eb68fa55-42df-56d7-9169-70148069d13c', 'it-it', 'Giraffippino', 'Il Giraffippino nacque nel mezzo di una pioggia di meteore, quando una giraffa, un ippopotamo e un coniglio giurarono di proteggere l''Albero della Luce. Dal loro giuramento, i rami unirono tre spiriti in un solo corpo. Con le braccia della giraffa innalza la luce; con il corpo dell''ippopotamo protegge la terra; con la testa del coniglio porta sorrisi e speranza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('390fd398-0246-43e5-8c49-7f8c5ca239ac', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'it-it', 'Giraffippotrello', 'Il Giraffippotrello apparve quando due giraffe vegliarono su un ippopotamo smarrito sotto la luce delle stelle. L''Albero della Luce li uni per mantenere l''equilibrio tra il cielo e l''acqua. E uno spirito di protezione e chiarezza, portando visione e forza gentile.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('243900da-5e1b-4bb9-a7ae-273aca371400', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'it-it', 'Giraffippotiche', 'Il Giraffippotiche apparve quando una giraffa giuro di vegliare su due ippopotami nel mezzo di una tempesta. L''Albero della Luce li uni in un unico corpo. E uno spirito di protezione e amicizia, portando la potenza delle acque e la visione del cielo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('955b6cc4-279a-48f0-bafb-d9f90ef61f05', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'it-it', 'Giraffippo', 'Il Giraffippo e una creatura nata dal sogno di una giraffa che contemplava le stelle e dal canto di un ippopotamo che riecheggiava sotto le acque. L''Albero della Luce li uni in un unico corpo per portare equilibrio: l''altezza della giraffa e la solidita dell''ippopotamo. Veglia sulle savane di luce e sulle acque profonde, cercando di unire il cielo con la terra. E un mediatore tra i mondi, portatore di saggezza e forza gentile.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e684f5c4-51f1-4766-81ba-eb772bcc9fee', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'it-it', 'Giraffino', 'Il Giraffino emerse dalla risata di un coniglio aggrappato al collo di una giraffa. L''Albero della Luce trovo la loro gioia e dono loro un corpo comune. E giocoso e allegro, ma ha anche un grande cuore, pronto a difendere la luce. E spesso descritto come lo spirito dell''ottimismo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e7626cb6-a499-41ca-94f6-ca7fd9a6bd1d', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'it-it', 'Giraffipotamo', 'Il Giraffipotamo nacque da un patto tra un ippopotamo e una giraffa che vegliarono insieme su un ramo caduto dell''Albero della Luce. L''Albero uni le loro forze e dono loro un corpo comune. E uno spirito di equilibrio tra potere e dolcezza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('950c7f5d-ccb0-4036-9f05-d77b832cf935', '880c0786-0e60-51d0-be8f-0822a13d439a', 'it-it', 'Ippoconigliette', 'L''Ippoconigliette nacque quando un ippopotamo protesse due conigli spaventati dall''oscurita. L''Albero della Luce li uni in un corpo comune. E uno spirito di gioia e resilienza, portando speranza ai piccoli.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('db0b7fba-7a1d-454f-bc4f-b7c21eac1757', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'it-it', 'Ipponigliraffe', 'L''Ipponigliraffe e una creatura di contrasti, nata quando tre mondi si incontrarono nell''Albero della Luce. Dall''ippopotamo ricevette la potenza; dal coniglio, l''agilita; dalla giraffa, la visione. Percorre i rami dell''albero in cerca di armonia tra gli estremi, dimostrando come la vera forza nasca dall''unione delle differenze.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('04f06936-5342-4213-aed4-5726aa20cd3d', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'it-it', 'Ippoconigliippo', 'L''Ippoconigliippo apparve quando due ippopotami vollero proteggere un fragile coniglio. L''Albero della Luce li uni in un corpo comune. E uno spirito di protezione e coraggio, portando equilibrio tra il gioco e la forza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f2b4f69f-4270-4f3c-af86-c922b9326b16', '3de3b3e0-5241-5350-b01b-28361c99c363', 'it-it', 'Ipposaltello', 'L''Ipposaltello nacque da un sogno giocoso in cui un coniglio chiedeva la forza di un ippopotamo. L''Albero della Luce li uni in un essere equilibrato: piccolo ma robusto, gentile ma determinato. Usa le braccia dell''ippopotamo per proteggere e il corpo del coniglio per saltare gli ostacoli. Porta coraggio ai piccoli e mostra loro che la stazza non definisce la forza dell''anima.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a0c31465-f06b-4b71-b9e2-7edd3160f3a6', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'it-it', 'Ippogirafconiglio', 'L''Ippogirafconiglio nacque nel momento in cui un ippopotamo, una giraffa e un coniglio alzarono insieme gli occhi verso la luce dell''Albero. I loro spiriti si fusero in un nuovo essere. E uno spirito di collaborazione e armonia, portando pace e saggezza nelle battaglie tra i mondi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6eb3fb9e-eb40-47e1-b329-b8e5300d9106', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'it-it', 'Ippobiraffa', 'L''Ippobiraffa emerse quando un ippopotamo e due giraffe vegliarono insieme su un ramo dell''albero. Dalla loro amicizia nacque questo essere unico, uno spirito di equilibrio e saggezza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('021f085c-7dfe-49f9-ba2d-8d6ba2db9d30', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'it-it', 'Ippogaffone', 'L''Ippogaffone nacque quando due ippopotami e una giraffa condivisero un sogno comune: unire le acque e il cielo. L''Albero della Luce li uni in un unico corpo. E uno spirito di stabilita e saggezza, portando chiarezza in mezzo alla tempesta.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('eef4c2c3-b903-4d0e-9fd1-00a64865859c', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'it-it', 'Ippogirafa', 'L''Ippogirafa nacque quando un ippopotamo giuro di proteggere una giraffa smarrita dall''oscurita. L''Albero della Luce li uni in un unico essere per mantenere la loro promessa. E forte e saggio, costruisce ponti tra il cielo e la terra. E conosciuta come lo spirito che porta l''equilibrio tra la forza e la grazia.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1bcce019-56b7-4d1b-a315-906de9c5f661', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'it-it', 'Ipponelice', 'L''Ipponelice apparve quando due ippopotami giurarono di proteggere un coniglio. L''Albero della Luce li uni in un unico corpo. E uno spirito di dolcezza e forza, portando pace tra i mondi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('05f6dff4-d5f7-424d-979b-9b8117552735', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'it-it', 'Ippogaffice', 'L''Ippogaffice nacque quando due ippopotami cercarono la saggezza di una giraffa. L''Albero della Luce li uni in uno spirito di stabilita e chiarezza. Veglia sui rami, portando equilibrio tra l''acqua e il cielo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c0944437-ea49-4e74-9431-03042e97deba', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'it-it', 'Ippoconiglio', 'L''Ippoconiglio apparve quando un coniglio chiese la protezione di un ippopotamo per sopravvivere all''oscurita. L''Albero della Luce uni le loro anime e diede vita a questo essere giocoso ma potente. E un protettore allegro che porta speranza dove regna la paura.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('197695f8-c2c3-4b5e-ab61-aba198ce067a', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'it-it', 'Ippiraffa', 'L''Ippiraffa nacque quando un ippopotamo e una giraffa vegliarono insieme sulla savana. L''Albero della Luce li uni donando loro un cuore comune. E uno spirito di vigilanza, portando equilibrio tra la forza e la chiarezza. E il protettore delle notti stellate.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2404ded6-a2ed-40e2-ad6f-3ff4770b1367', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'it-it', 'Conigliraffina', 'La Conigliraffina apparve quando un coniglio desidero sorvegliare una savana. L''Albero della Luce esaudi il suo desiderio e lo uni con lo spirito di una giraffa. E uno spirito di responsabilita e gioia, portando in se il desiderio di proteggere.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('aa7c96bb-ef07-4729-b1aa-97c181453f42', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'it-it', 'Coniglippos', 'Il Coniglippos nacque quando un coniglio sogno di avere la forza di un ippopotamo. L''Albero della Luce trasformo il sogno in realta, unendo due mondi opposti. E gentile e pacifico, ma possiede una forza nascosta che si risveglia per difendere la luce. E il simbolo dei sogni realizzati.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('63aba06a-e482-433d-95d5-e0e5445d6efe', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'it-it', 'Giraffina', 'La Giraffina nacque quando un coniglio sali sulle spalle di una giraffa per guardare il cielo. L''Albero della Luce vide la loro gioia e li uni in un corpo comune. E uno spirito giocoso e amichevole, portando allegria e speranza ovunque vada.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f9cf8f4e-b0b6-4d74-8487-ed3b0adbf6b9', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'it-it', 'Giraffottamo', 'Il Giraffottamo emerse quando un ippopotamo cerco l''altezza di una giraffa per sorvegliare la savana. L''Albero della Luce li uni donando loro un corpo comune. E uno spirito di stabilita, mantenendo l''equilibrio tra le acque e il cielo. E il protettore degli orizzonti.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4e5945a1-0cac-43d4-9a4e-19e2d4b31432', '07084a31-3e3a-5b86-8536-56d2311359bd', 'it-it', 'Ippurello', 'L''Ippurello apparve quando un coniglio e un ippopotamo cantarono insieme sulle rive di un lago stellato. L''Albero della Luce intreccio il loro canto e le loro risate in un corpo comune. E uno spirito di allegria e armonia, portando speranza a chi vaga.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('925cc046-afa6-46bf-9e1e-1c64dfd2d4fd', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'it-it', 'Ippogiraffone', 'L''Ippogiraffone nacque quando un ippopotamo sogno di vedere oltre l''orizzonte delle acque. L''Albero della Luce lo uni con lo spirito di una giraffa. E uno spirito di visione e tenacia, che unisce la stabilita delle acque con l''altezza del cielo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
