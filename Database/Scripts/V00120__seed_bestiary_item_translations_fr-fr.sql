-- Auto-generated from Data/SeedData/Discovery/i18n/fr-fr/discover-bestiary.json
-- Locale: fr-fr
-- Run date: 2026-03-03T09:03:26.684Z
-- This script seeds BestiaryItemTranslations for fr-fr bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('57555a34-7cda-4d49-97f8-7d431286bec4', '5d1b4419-c890-5462-bcb7-242dbe123890', 'fr-fr', 'Lapirafix', 'Le Lapirafix apparut lorsque deux lapins reva ensemble d''atteindre le ciel a travers les yeux d''une girafe. L''Arbre de Lumiere les unit en un seul etre. C''est un esprit de curiosite et de courage qui bondit entre les mondes en portant la lumiere dans les ombres.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a321c7bf-8e87-4fc5-996e-73a0a5e03a0d', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'fr-fr', 'Lapipotix', 'Le Lapipotix naquit du reve de deux lapins qui demandaient la force d''un hippopotame pour proteger leurs amis. L''Arbre de Lumiere unit leurs ames. C''est un esprit joueur mais protecteur qui apporte du courage aux petits et la paix aux perdus.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a2cd953c-d68d-464d-a033-144f07f3fc7c', 'fc1b57f3-4751-51f5-a497-174648433b88', 'fr-fr', 'Lapirafonix', 'Le Lapirafonix emergea lorsque deux lapins desirerent voir le monde d''en haut. L''Arbre de Lumiere repondit en leur donnant le corps d''une girafe, mais preserva leurs coeurs joueurs. C''est un esprit de reves et d''espoir qui porte dans ses yeux la lumiere de l''enfance.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('68e757d2-80e6-4089-938d-fe8cdf6d4c32', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'fr-fr', 'Lapibirgirafe', 'La Lapibirgirafe apparut lorsqu''un lapin voulut partager son jeu avec deux girafes. L''Arbre de Lumiere les unit en un seul etre. C''est un esprit de curiosite et d''amitie qui unit la vivacite a la grandeur.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7d0e0bc0-2e31-4a90-8b38-f168e6add4e9', 'b67d8c08-04ec-539d-8787-27e838807428', 'fr-fr', 'Lapigirafopotame', 'Le Lapigirafopotame est un heros legendaire, ne d''une alliance impossible : un lapin, une girafe et un hippopotame qui choisirent d''unir leurs destins. L''Arbre de Lumiere vit leur courage et leur donna un seul coeur. Avec les bras du lapin, il aide et protege. Avec le corps de la girafe, il voit loin. Avec la tete de l''hippopotame, il inspire la tranquillite et la sagesse. Il est le symbole de la diversite et de l''amitie entre des mondes differents.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('370f828a-23e9-41e4-b295-e315b5741172', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'fr-fr', 'Lapigirafe', 'Dans les branches de l''Arbre de Lumiere naquit la Lapigirafe, une creature unique, mi-lapin, mi-girafe. Ses bras courts et agiles creusent des tunnels et allument des feux vifs, tandis que son grand corps, orne de taches lumineuses, percoit des horizons lointains que les autres ne peuvent atteindre.

La legende dit que la Lapigirafe emergea lorsqu''un petit lapin courageux se cacha aux racines d''une vieille girafe tandis que l''obscurite devorait la savane. De leur etreinte desperee, l''Arbre de Lumiere tissa deux esprits en un seul corps.

La Lapigirafe est la gardienne des horizons. Quand elle court dans l''herbe, la terre bondit de joie. Quand elle s''eleve pour combattre, les ombres fuient honteuses. Mais sa vraie force ne reside pas seulement dans la vitesse ou la hauteur, mais dans l''amitie qu''elle offre a ceux qui errent entre les branches des mondes.

Les anciens esprits disent que la Lapigirafe sera toujours la ou quelqu''un tente de reparer ce qui a ete brise, apportant courage et espoir.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('b13ba0c2-29e5-4fcd-868c-aca8896e136f', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'fr-fr', 'Lapipotarel', 'Le Lapipotarel naquit lorsqu''un lapin chercha l''amitie d''un hippopotame et que l''Arbre de Lumiere unit leurs destins. Avec des bras agiles et une tete joueuse, mais un corps solide, c''est un esprit de l''equilibre entre la fragilite et la force, portant gaite et protection aux mondes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('255dcee5-b4a8-4f47-82ea-038e4aababfb', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'fr-fr', 'Lapipotamgirafe', 'Le Lapipotamgirafe naquit au milieu d''une tempete de lumiere, lorsque trois amis tenterent de reparer une branche brisee de l''Arbre de Lumiere. Leur sacrifice se transforma en magie. C''est un gardien doux, portant en lui l''espoir et le courage de l''amitie.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6b9335c0-f2aa-486c-8bab-1131e92cd819', '558b9240-3c3a-5017-a85a-706a397288fd', 'fr-fr', 'Lapipotixdouble', 'Le Lapipotixdouble naquit lorsqu''un lapin apporta de la joie a deux hippopotames. L''Arbre de Lumiere les unit en un etre protecteur et joyeux. C''est un esprit d''amitie et de courage qui porte la lumiere dans l''obscurite.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e5f8586b-8a53-4ac9-89cc-8f51c9f4c3a7', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'fr-fr', 'Lapipotame', 'Le Lapipotame naquit lorsqu''un lapin et un hippopotame se rencontrerent au milieu d''une tempete d''eaux sombres. De leur amitie inattendue, l''Arbre de Lumiere tissa l''agilite du lapin avec la force de l''hippopotame. Le Lapipotame est doux et protecteur : avec ses bras agiles, il creuse des refuges, et avec son corps puissant, il ouvre des chemins a travers les eaux. Il apporte le calme la ou regne la peur et construit des ponts de confiance entre les mondes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('57fe4023-51fc-4639-a6af-e240014cd330', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'fr-fr', 'Lapigiraffe', 'Le Lapigiraffe fut cree lorsqu''un lapin et une girafe tenterent ensemble d''illuminer une branche tombee de l''Arbre de Lumiere. La magie les unit en un seul corps. Le Lapigiraffe est un esprit de courage et d''amitie, capable d''apporter espoir et lumiere dans l''obscurite.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e426df4c-d048-4642-b832-e380a68de320', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'fr-fr', 'Lapipotam', 'Le Lapipotam emergea lorsqu''un lapin et un hippopotame se tinrent ensemble sous la tempete de la nuit. L''Arbre de Lumiere les tissa en un seul corps, preservant l''agilite du lapin et la sagesse de l''hippopotame. Le Lapipotam est un symbole de solidarite et d''amitie entre le petit et le grand.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5f946988-f744-486e-9373-5e3cb010519e', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'fr-fr', 'Girafelapins', 'La Girafelapins apparut lorsqu''une girafe embrassa deux lapins. L''Arbre de Lumiere les unit en un seul etre. C''est un esprit de compassion et de jeu qui porte la gaite entre les branches des mondes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('15990793-5840-4593-a067-74af1d18dea6', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'fr-fr', 'Giraffiepix', 'Le Giraffiepix naquit lorsque deux girafes protegerent un petit lapin. L''Arbre de Lumiere les unit en un seul corps, leur conferant l''agilite de la terre et la vision du ciel. C''est un esprit d''amitie et de sagesse capable d''unir des mondes opposes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('adb95210-1a7b-4d72-a3c2-dfa7a57afe20', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'fr-fr', 'Girafopotamos', 'Le Girafopotamos apparut lorsque trois creatures jurerent de proteger ensemble les branches de l''arbre. De leur amitie, l''Arbre de Lumiere crea un corps commun. Le Girafopotamos est un esprit d''unite et d''amitie qui porte en lui l''espoir d''unir des mondes differents.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f7f6230b-2bc7-4c41-b6b2-36d35e11dc04', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'fr-fr', 'Girapin', 'Le Girapin naquit lorsque les bras d''une girafe embrasserent un petit lapin tremblant dans la nuit. A partir de ce moment, l''Arbre de Lumiere les unit en un hybride inhabituel. Le Girapin est enjoue et intrepide : il utilise ses longs bras pour cueillir les etoiles et le corps du lapin pour bondir entre les ombres. C''est un esprit de joie qui porte rires et lumiere partout ou il va.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('50349f40-25cc-4ba5-8d85-f64cfdc42482', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'fr-fr', 'Girafunix', 'Le Girafunix emergea lorsque deux girafes voulurent partager leur hauteur avec un petit lapin. L''Arbre de Lumiere les unit en une creature d''amitie et de joie. Le Girafunix apporte l''espoir aux petits, montrant que la grandeur peut etre partagee.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('fe79ae10-3a90-4f24-9429-28498ee156ed', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'fr-fr', 'Doublegirafe', 'La Doublegirafe naquit lorsque deux esprits de girafe veillaient ensemble sur un hippopotame perdu. L''Arbre de Lumiere les unit en un seul etre, grand et robuste, mais doux. La Doublegirafe est la gardienne des horizons qui porte l''equilibre entre la force et la vision.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3c22d353-640e-4e3f-8e31-d18f7b523957', 'eb68fa55-42df-56d7-9169-70148069d13c', 'fr-fr', 'Girafopotin', 'Le Girafopotin naquit au milieu d''une pluie de meteores, lorsqu''une girafe, un hippopotame et un lapin jurerent de proteger l''Arbre de Lumiere. De leur serment, les branches unirent trois esprits en un seul corps. Avec les bras de la girafe, il eleve la lumiere vers le ciel ; avec le corps de l''hippopotame, il protege la terre ; avec la tete du lapin, il apporte sourires et espoir. Le Girafopotin est considere comme le gardien de l''equilibre.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5dfc88e1-524c-416a-bbe3-d2dddf255285', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'fr-fr', 'Girafopotamrel', 'Le Girafopotamrel apparut lorsque deux girafes veillaient sur un hippopotame perdu sous la lumiere des etoiles. L''Arbre de Lumiere les unit pour maintenir l''equilibre entre le ciel et l''eau. C''est un esprit de protection et de clarte qui porte vision et force douce.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('23972a82-452a-4546-95ad-fdf9f7870ee6', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'fr-fr', 'Girafopotix', 'Le Girafopotix apparut lorsqu''une girafe jura de veiller sur deux hippopotames au milieu d''une tempete. L''Arbre de Lumiere les unit en un seul corps. C''est un esprit de protection et d''amitie qui porte la puissance des eaux et la vision du ciel.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5cec46d4-df85-4249-8829-a2dcf2e09b2a', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'fr-fr', 'Girafopotame', 'Le Girafopotame est une creature nee du reve d''une girafe contemplant les etoiles et du chant d''un hippopotame resonnant sous les eaux. L''Arbre de Lumiere les unit en un seul corps pour apporter l''equilibre : la hauteur de la girafe et la solidite de l''hippopotame. Il veille sur les savanes de lumiere et les eaux profondes, cherchant a unir le ciel a la terre. Le Girafopotame est un mediateur entre les mondes, porteur de sagesse et de force douce.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('97b09a9f-8ca9-40b5-aefa-294cf14f8f2d', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'fr-fr', 'Girafin', 'Le Girafin emergea du rire d''un lapin accroche au cou d''une girafe. L''Arbre de Lumiere trouva leur joie et leur donna un corps commun. Le Girafin est joueur et joyeux, mais a aussi un grand coeur, pret a defendre la lumiere. Il est souvent decrit comme l''esprit de l''optimisme.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c8afe856-8c9a-4a5f-92e8-8a9ddfae5cd4', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'fr-fr', 'Girafopotamir', 'Le Girafopotamir naquit d''un pacte entre un hippopotame et une girafe qui veillaient ensemble sur une branche tombee de l''Arbre de Lumiere. L''Arbre unit leurs forces et leur donna un corps commun. Le Girafopotamir est un esprit d''equilibre entre le pouvoir et la douceur, portant en lui la sagesse des deux mondes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('11bc1e1f-e431-411e-85bc-b3c1023bcfec', '880c0786-0e60-51d0-be8f-0822a13d439a', 'fr-fr', 'Hippolapins', 'L''Hippolapins naquit lorsqu''un hippopotame protegea deux lapins effrayes par l''obscurite. L''Arbre de Lumiere les unit en un corps commun. C''est un esprit de joie et de resilience qui porte l''espoir aux petits.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3dbd3535-87dc-4779-bf68-ca6366488335', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'fr-fr', 'Hippogirafit', 'L''Hippogirafit est une creature de contrastes, nee lorsque trois mondes se rencontrerent dans l''Arbre de Lumiere. De l''hippopotame, il recut la puissance ; du lapin, l''agilite ; de la girafe, la vision. Il parcourt les branches de l''arbre en quete d''harmonie entre les extremes et montre que la vraie force nait de l''union des differences.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('012fddd6-81b4-4342-8cbb-d18fd60a704a', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'fr-fr', 'Hippopotarel', 'L''Hippopotarel apparut lorsque deux hippopotames voulurent proteger un fragile lapin. L''Arbre de Lumiere les unit en un corps commun, petit mais robuste. C''est un esprit de protection et de courage qui porte l''equilibre entre le jeu et la force.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('b3a9bf98-b19a-4f68-b0f3-10f0805b37bb', '3de3b3e0-5241-5350-b01b-28361c99c363', 'fr-fr', 'Hipposaute', 'L''Hipposaute naquit d''un reve joueur dans lequel un lapin demandait la force d''un hippopotame. L''Arbre de Lumiere les unit en un etre equilibre : petit mais robuste, doux mais determine. L''Hipposaute utilise les bras de l''hippopotame pour proteger et le corps du lapin pour sauter les obstacles. Il apporte du courage aux petits et leur montre que la taille ne definit pas la force de l''ame.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('96225eae-58c8-4713-b586-a501362c3563', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'fr-fr', 'Hippogirapin', 'L''Hippogirapin naquit au moment ou un hippopotame, une girafe et un lapin leverent ensemble les yeux vers la lumiere de l''Arbre. Leurs esprits se fondirent en un nouvel etre. C''est un esprit de collaboration et d''harmonie qui apporte paix et sagesse dans les luttes entre les mondes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('9d5b3c8e-fbff-453c-b71e-7a40f918d652', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'fr-fr', 'Hippobirgirafe', 'L''Hippobirgirafe emergea lorsqu''un hippopotame et deux girafes veillaient ensemble sur une branche de l''arbre. De leur amitie naquit cet etre unique, un esprit d''equilibre et de sagesse.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('09747708-1e14-4d1b-accf-a80e26b6d79b', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'fr-fr', 'Hippogaffon', 'L''Hippogaffon naquit lorsque deux hippopotames et une girafe partagerent un reve commun : unir les eaux et le ciel. L''Arbre de Lumiere les unit en un seul corps. C''est un esprit de stabilite et de sagesse qui apporte la clarte au milieu de la tempete.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('59ca20e0-c270-45fd-8e51-9b0b2e66d624', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'fr-fr', 'Hippogirafe', 'L''Hippogirafe naquit lorsqu''un hippopotame jura de proteger une girafe perdue de l''obscurite. L''Arbre de Lumiere les unit en un seul etre pour tenir leur promesse. Il est fort et sage, construit des ponts entre le ciel et la terre. L''Hippogirafe est connue comme l''esprit qui apporte l''equilibre entre la force et la grace.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('395ae6a5-159f-4e52-b3d0-b449ec93f0a8', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'fr-fr', 'Hipponelix', 'L''Hipponelix apparut lorsque deux hippopotames jurerent de proteger un lapin. L''Arbre de Lumiere les unit en un seul corps. C''est un esprit de douceur et de force qui porte la paix entre les mondes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('872f511d-1d82-44ea-95f7-fe1bce564056', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'fr-fr', 'Hippogaffix', 'L''Hippogaffix naquit lorsque deux hippopotames chercherent la sagesse d''une girafe. L''Arbre de Lumiere les unit en un esprit de stabilite et de clarte. Il veille sur les branches en portant l''equilibre entre l''eau et le ciel.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7380ce62-5a88-4130-9172-7ebbdce7aab6', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'fr-fr', 'Hippolapin', 'L''Hippolapin apparut lorsqu''un lapin demanda la protection d''un hippopotame pour survivre a l''obscurite. L''Arbre de Lumiere unit leurs ames et donna vie a cet etre joueur mais puissant. L''Hippolapin est un protecteur joyeux qui apporte l''espoir la ou regne la peur.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3f361452-a01a-4402-8312-ea0ca732b3aa', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'fr-fr', 'Hippiraf', 'L''Hippiraf naquit lorsqu''un hippopotame et une girafe veillaient ensemble sur la savane. L''Arbre de Lumiere les unit en leur donnant un coeur commun. L''Hippiraf est un esprit de vigilance qui porte l''equilibre entre la force et la clarte. Il est le protecteur des nuits etoilees.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7703e3f8-e097-4cef-8a5b-e52ad97a4eed', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'fr-fr', 'Lapigiraffine', 'La Lapigiraffine apparut lorsqu''un lapin souhaita surveiller une savane. L''Arbre de Lumiere entendit son souhait et l''unit avec l''esprit d''une girafe. La Lapigiraffine est un esprit de responsabilite et de joie qui porte en lui le desir de proteger.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5ebeed1f-418a-4155-a7b1-4553fac46c27', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'fr-fr', 'Lapipotamos', 'Le Lapipotamos naquit lorsqu''un lapin reva d''avoir la force d''un hippopotame. L''Arbre de Lumiere transforma le reve en realite, unissant deux mondes opposes. Le Lapipotamos est doux et pacifique, mais possede une force cachee qui s''eveille pour defendre la lumiere. Il est le symbole des reves accomplis.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8440be9e-3797-4ff2-804d-9afdcc5ec0db', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'fr-fr', 'Giralapin', 'Le Giralapin naquit lorsqu''un lapin monta sur les epaules d''une girafe pour regarder le ciel. L''Arbre de Lumiere vit leur joie et les unit en un corps commun. Le Giralapin est un esprit joueur et amical qui repand gaite et espoir partout ou il va.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('92e62adb-52b8-41c8-8f7e-cd216d4e7a06', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'fr-fr', 'Girafotame', 'Le Girafotame emergea lorsqu''un hippopotame chercha la hauteur d''une girafe pour surveiller la savane. L''Arbre de Lumiere les unit en leur donnant un corps commun. Le Girafotame est un esprit de stabilite qui maintient l''equilibre entre les eaux et le ciel. Il est le protecteur des horizons.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4eeeb604-1b5f-451f-83be-50a40b49363e', '07084a31-3e3a-5b86-8536-56d2311359bd', 'fr-fr', 'Hippouriel', 'L''Hippouriel apparut lorsqu''un lapin et un hippopotame chantaient ensemble au bord d''un lac etoile. L''Arbre de Lumiere tissa leur chant et leurs rires en un corps commun. L''Hippouriel est un esprit de gaite et d''harmonie qui porte l''espoir a ceux qui errent.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bf50f66f-f16e-4498-9ba9-8a5b63431123', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'fr-fr', 'Hippogiraffon', 'L''Hippogiraffon naquit lorsqu''un hippopotame reva de voir au-dela de l''horizon des eaux. L''Arbre de Lumiere l''unit avec l''esprit d''une girafe et leur donna un corps commun. L''Hippogiraffon est un esprit de vision et d''endurance qui unit la stabilite des eaux a la hauteur du ciel.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
