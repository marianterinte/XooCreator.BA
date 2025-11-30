-- Auto-generated from Data/SeedData/BookOfHeroes/i18n/hu-HU/hero-tree.json
-- Run date: 2025-12-01 00:04:15+02:00
-- This script adds/updates Hungarian translations for Tree of Heroes
-- It is idempotent: safe to run multiple times

BEGIN;

INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9b055714-fcd9-572a-ae9d-92bbb7c31cf0', 'seed', 'hu-hu', 'HÅ‘sies Mag', 'A mag, amelybÅ‘l minden hÅ‘s nÅ‘. Kattints, hogy felfedezd hÅ‘sies potenciÃ¡lodat.', 'A szÃ­vedben egy hÅ‘siessÃ©g magja dobog. Kattints rÃ¡, hogy felfedezd elsÅ‘ hÅ‘seidet Ã©s kezdd el hÅ‘sies utadat.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('57fa10c3-0e0c-528a-97f4-a13b93012620', 'alchimalia_hero', 'hu-hu', 'Alchimalia HÅ‘se', 'Minden hÅ‘s gyÃ¶kere, az Ã¡talakulÃ¡s forrÃ¡sa.', 'HÅ±sÃ©ges tÃ¡rsad, a hÅ‘sies Ãºt kezdete')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c16ffb1b-4ef2-5544-a26d-27f693f482fd', 'hero_brave_puppy', 'hu-hu', 'BÃ¡tor Kiskutya', 'Egy bÃ¡tor lÃ©lek, aki megadja neked az erÅ‘t, hogy szembenÃ©zz bÃ¡rmilyen kihÃ­vÃ¡ssal...', 'A BÃ¡tor Kiskutya megtanÃ­t, hogy az igazi bÃ¡torsÃ¡g nem a fÃ©lelem hiÃ¡nya, hanem a cselekvÃ©s a fÃ©lelem ellenÃ©re.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('b5776e59-90a4-53b9-a6ad-e17da9f476c2', 'hero_brave_pup_lvl2', 'hu-hu', 'ÅrzÅ‘ Kutya', 'Egy hÅ±sÃ©ges vÃ©delmezÅ‘ rendÃ­thetetlen bÃ¡torsÃ¡ggal Ã©s odaadÃ¡ssal.', 'Az ÅrzÅ‘ Kutya Å‘rkÃ¶dik, megvÃ©dve a szÃ¼ksÃ©gben lÃ©vÅ‘ket bÃ¡tor elszÃ¡ntsÃ¡ggal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('da1d7f1a-d7fc-539b-b59c-f33c9f253987', 'hero_brave_pup_lvl3', 'hu-hu', 'LegendÃ¡s Kutya', 'Egy legendÃ¡s hÅ‘s pÃ¡ratlan bÃ¡torsÃ¡ggal Ã©s nemes szellemmel.', 'A LegendÃ¡s Kutya legendÃ¡kat inspirÃ¡l, a hÅ‘sies bÃ¡torsÃ¡g legtisztÃ¡bb formÃ¡jÃ¡t megtestesÃ­tve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('06e69f63-c81a-5a98-a664-fa3cf06b208f', 'hero_curious_cat', 'hu-hu', 'KÃ­vÃ¡ncsi Macska', 'Egy kÃ­vÃ¡ncsi lÃ©ny, aki megnyitja a szemed a vilÃ¡g csodÃ¡i felÃ©...', 'A KÃ­vÃ¡ncsi Macska vezet tÃ©ged a kÃ©rdÃ©sek labirintusÃ¡n, megmutatva, hogy minden rejtÃ©ly egy csodÃ¡t rejt.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ae0609b8-fb76-5089-a3a6-f00bfaa17c89', 'hero_curious_cat_lvl2', 'hu-hu', 'FelfedezÅ‘ Macska', 'Egy rettenthetetlen felfedezÅ‘, akit vÃ©gtelen kÃ­vÃ¡ncsisÃ¡g Ã©s csodÃ¡lat vezet.', 'A FelfedezÅ‘ Macska ismeretlen terÃ¼letekre merÃ©szkedik, rejtett titkokat Ã©s rejtÃ©lyeket fedezve fel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a17d49ae-7795-5618-ad32-f7eb6fed7d28', 'hero_curious_cat_lvl3', 'hu-hu', 'JÃ³slÃ³ Macska', 'Egy misztikus lÃ¡tnok mÃ©ly megÃ©rtÃ©ssel Ã©s prÃ³fÃ©tai bÃ¶lcsessÃ©ggel.', 'A JÃ³slÃ³ Macska a valÃ³sÃ¡g fÃ¡tyla mÃ¶gÃ© lÃ¡t, igazsÃ¡gokat tÃ¡rva fel, amelyek a hÃ©tkÃ¶znapi szemnek rejtve maradnak.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4865fde5-2c60-564e-aa4b-d0423402d5cd', 'hero_wise_owl', 'hu-hu', 'BÃ¶lcs Bagoly', 'Egy bÃ¶lcs mentor, aki megosztja veled a tudÃ¡s titkait...', 'A BÃ¶lcs Bagoly suttogja a Ã©jszaka titkait Ã©s megnyitja a szemed a rejtett igazsÃ¡gok felÃ©.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('142970eb-3d34-50de-bd51-509582b6f27e', 'hero_wise_owl_lvl2', 'hu-hu', 'TudÃ³s Bagoly', 'Egy tanult tudÃ³s mÃ©ly tudÃ¡ssal Ã©s analitikus gondolkodÃ¡ssal.', 'A TudÃ³s Bagoly a vilÃ¡g rejtÃ©lyeit tanulmÃ¡nyozza, bÃ¶lcsessÃ©get osztva meg gondos elemzÃ©sen keresztÃ¼l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7dc21d29-47cc-5d99-a246-2980b7c90a24', 'hero_wise_owl_lvl3', 'hu-hu', 'Åsi Bagoly', 'Egy Å‘si bÃ¶lcs idÅ‘tlen bÃ¶lcsessÃ©ggel Ã©s Ã¶rÃ¶k tudÃ¡ssal.', 'Az Åsi Bagoly tanÃºja volt a koroknak, a vilÃ¡gegyetem legmÃ©lyebb titkait Å‘rizve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2805af01-06a7-51b0-bbbe-fae34c926591', 'hero_playful_horse', 'hu-hu', 'JÃ¡tÃ©kos KiscsikÃ³', 'Egy jÃ¡tÃ©kos tÃ¡rs, aki korlÃ¡tok nÃ©lkÃ¼l felszabadÃ­tja a kÃ©pzeletedet...', 'A JÃ¡tÃ©kos KiscsikÃ³ hozza neked a tiszta alkotÃ¡s Ã¶rÃ¶mÃ©t, ahol a kÃ©pzelet kantÃ¡r nÃ©lkÃ¼l vÃ¡gtÃ¡zik.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2580dd24-43ea-5736-8f0d-703db9e1b6ce', 'hero_playful_horse_lvl2', 'hu-hu', 'MÅ±vÃ©szi LÃ³', 'Egy kreatÃ­v mÅ±vÃ©sz, aki szÃ©psÃ©get fejez ki kÃ©pzelet Ã©s mestersÃ©g rÃ©vÃ©n.', 'A MÅ±vÃ©szi LÃ³ remekmÅ±veket alkot, Ã¡lmokat Ã©letre hozva kreatÃ­v kifejezÃ©sen keresztÃ¼l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('ef919be2-c331-580e-96c4-9bbb5984ae10', 'hero_playful_horse_lvl3', 'hu-hu', 'Zseni LÃ³', 'Egy kreatÃ­v zseni pÃ¡ratlan mÅ±vÃ©szi lÃ¡tomÃ¡ssal Ã©s innovÃ¡ciÃ³val.', 'A Zseni LÃ³ tÃºlmutat a hÃ©tkÃ¶znapi kreativitÃ¡son, tiszta mÅ±vÃ©szi ragyogÃ¡s mÅ±veket alkotva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('3c5cc213-9923-5f9c-b35e-757ae5124207', 'hero_cautious_hedgehog', 'hu-hu', 'Ã“vatos SÃ¼n', 'Egy Ã³vatos vÃ©delmezÅ‘, aki biztonsÃ¡got Ã©s egyensÃºlyt ad neked...', 'Az Ã“vatos SÃ¼n megtanÃ­t, hogy a biztonsÃ¡g nem fÃ©lelmet jelent, hanem bÃ¶lcsessÃ©get.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('93503004-92fe-57e9-b59a-eaf8a5bcda58', 'hero_cautious_hedgehog_lvl2', 'hu-hu', 'Pajzs-TÃ¼skÃ©s', 'Gondos vÃ©delem, Ã³vatossÃ¡gbÃ³l Ã©s tapasztalatbÃ³l Ã©pÃ­tve.', 'TÃ¼skÃ©it pajzskÃ©nt hasznÃ¡lja, hogy megvÃ©dje, ami a legfontosabb.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('446724a3-e204-5959-9543-4af1dc98a148', 'hero_cautious_hedgehog_lvl3', 'hu-hu', 'ÅrzÅ‘ TÃ¼ske', 'RendÃ­thetetlen Å‘rszem, elszÃ¡ntan a biztonsÃ¡g fenntartÃ¡sÃ¡ra.', 'Az Å‘rsÃ©get mÅ±vÃ©szettÃ© alakÃ­tja, a vilÃ¡g hatÃ¡rain Å‘rkÃ¶dve.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('841f6265-2965-53e6-bbce-13a6c7cd5cbd', 'hero_catdog_interstellar', 'hu-hu', 'CsillagkÃ¶zi Macska-Kutya', 'Egy kozmikus felfedezÅ‘, aki a bÃ¡torsÃ¡g Ã©s kÃ­vÃ¡ncsisÃ¡g egyesÃ¼lÃ©sÃ©bÅ‘l szÃ¼letett...', 'A CsillagkÃ¶zi Macska-Kutya egyesÃ­ti a vÃ©gtelen kÃ­vÃ¡ncsisÃ¡got a bÃ¡torsÃ¡ggal, hogy felfedezze az ismeretlent.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('14f1bdac-cb71-5775-8e34-fa5dbe4225c2', 'hero_brave_sage', 'hu-hu', 'BÃ¡tor BÃ¶lcs', 'Egy bÃ¶lcs lÃ©lek, aki bÃ¡torsÃ¡got Ã©s bÃ¶lcsessÃ©get egyesÃ­t...', 'A BÃ¡tor BÃ¶lcs megmutatja, hogy az igazi erÅ‘ a tudÃ¡s Ã©s a bÃ¡torsÃ¡g harmÃ³niÃ¡jÃ¡ban rejlik.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('619e330e-e53a-5ce8-9984-64debc73b52a', 'hero_brave_artist', 'hu-hu', 'BÃ¡tor MÅ±vÃ©sz', 'Egy hÅ‘s, aki mer alkotni ott, ahol mÃ¡sok fÃ©lnek, Ãºj horizontokat nyitva mÅ±vÃ©szetÃ©vel.', 'A BÃ¡tor MÅ±vÃ©sz megmutatja, hogy az alkotÃ¡s cselekedete a legfÅ‘bb bÃ¡torsÃ¡g, Ãºj vilÃ¡gokat Ã©letre hozva a semmibÅ‘l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('135f15d2-37d7-599c-816c-1c81299155c0', 'hero_brave_guardian', 'hu-hu', 'BÃ¡tor ÅrzÅ‘', 'Egy hÅ‘s, ahol a bÃ¡torsÃ¡g Ã©s vÃ©delem kÃ©z a kÃ©zben jÃ¡r, vÃ©dve habozÃ¡s nÃ©lkÃ¼l, de bÃ¶lcsen.', 'A BÃ¡tor ÅrzÅ‘ a remegtetlen pajzs, megvÃ©dve az Ã¡rtatlanokat rendÃ­thetetlen elszÃ¡ntsÃ¡ggal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4ad6ad68-7263-5383-9195-2cfeb92ee662', 'hero_wise_researcher', 'hu-hu', 'BÃ¶lcs KutatÃ³', 'Egy hÅ‘s, aki egyesÃ­ti az analitikus elmÃ©t a fÃ¡radhatatlan kÃ­vÃ¡ncsisÃ¡ggal, hogy rejtett igazsÃ¡gokat fedezzen fel.', 'A BÃ¶lcs KutatÃ³ kÃ¶veti a tudÃ¡s fonalÃ¡t a legnagyobb labirintusokon keresztÃ¼l, fÃ©nyt talÃ¡lva a sÃ¶tÃ©tsÃ©gben.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9fed3e9b-03f4-5f88-a095-84155f0483ca', 'hero_creative_explorer', 'hu-hu', 'KreatÃ­v FelfedezÅ‘', 'Egy hÅ‘s, aki egyesÃ­ti a kÃ©pzeletet a megismerÃ©s vÃ¡gyÃ¡val; minden lÃ©pÃ©s egy Ãºj talÃ¡lmÃ¡ny.', 'A KreatÃ­v FelfedezÅ‘ szÃ¡mÃ¡ra a tÃ©rkÃ©p rajzolÃ³dik ki, ahogy bejÃ¡rja, az ismeretlent mÅ±vÃ©szeti alkotÃ¡ssÃ¡ alakÃ­tva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('1c2fb32d-20d3-545a-b567-1d9f2b8087f6', 'hero_guard_cat', 'hu-hu', 'ÅrzÅ‘ Macska', 'Egy kalandos vÃ©delmezÅ‘, aki Ã³vatosan felfedez.', 'Az ÅrzÅ‘ Macska ismeri terÃ¼letÃ©nek minden szÃ¶gletÃ©t, biztosÃ­tva, hogy minden kÃ­vÃ¡ncsisÃ¡g kielÃ©gÃ¼ljÃ¶n a biztonsÃ¡g veszÃ©lyeztetÃ©se nÃ©lkÃ¼l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('cdec2124-822d-524d-bd0a-accc271d8329', 'hero_wise_pegasus', 'hu-hu', 'BÃ¶lcs Pegazus', 'A tudÃ¡s Å‘rzÅ‘je, aki a hatÃ¡rokon tÃºl tud repÃ¼lni.', 'A BÃ¶lcs Pegazus gondolatokat visz szÃ¡rnyain, az absztrakt Ã¶tleteket repÃ¼lÅ‘ valÃ³sÃ¡gokkÃ¡ alakÃ­tva.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('05af3db4-4a05-511d-9959-4bb31d3ffc16', 'hero_thoughtful_guardian', 'hu-hu', 'GondolkodÃ³ ÅrzÅ‘', 'Egy hÅ‘s, aki cselekvÃ©s elÅ‘tt tervez; a biztonsÃ¡g jÃ³l megfontolt stratÃ©giÃ¡kbÃ³l ered.', 'A GondolkodÃ³ ÅrzÅ‘ nem kÅ‘bÅ‘l Ã©pÃ­t erÅ‘dÃ¶ket, hanem logikÃ¡bÃ³l Ã©s elÅ‘relÃ¡tÃ¡sbÃ³l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('d2e9f52e-505b-51f7-b639-ee9fcb5970bf', 'hero_creative_guardian', 'hu-hu', 'KreatÃ­v ÅrzÅ‘', 'Egy talÃ¡lÃ©kony vÃ©delmezÅ‘, aki eredeti megoldÃ¡sokat talÃ¡l a biztonsÃ¡g biztosÃ­tÃ¡sÃ¡ra.', 'A KreatÃ­v ÅrzÅ‘ nem falakat Ã©pÃ­t, hanem egy olyan vilÃ¡got kÃ©pzel el, ahol ezek nem szÃ¼ksÃ©gesek.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('759b3051-fbdc-5566-b96f-48fec4846bc9', 'hero_brave_curious_wise', 'hu-hu', 'BÃ¶lcs OroszlÃ¡n BÃ¶lcs', 'Egy fensÃ©ges oroszlÃ¡n, aki egyesÃ­ti a bÃ¡torsÃ¡got, kÃ­vÃ¡ncsisÃ¡got Ã©s bÃ¶lcsessÃ©get.', 'A BÃ¶lcs OroszlÃ¡n BÃ¶lcs erÅ‘vel Ã©s megÃ©rtÃ©ssel vezet, mÃ¡sokat inspirÃ¡lva nemes pÃ©ldÃ¡jÃ¡val.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('213b8a6b-20a8-599d-82a4-61df82cb44ad', 'hero_brave_curious_creative', 'hu-hu', 'BÃ¡tor OroszlÃ¡n MÅ±vÃ©sz', 'Egy bÃ¡tor oroszlÃ¡n, aki kreativitÃ¡st fejez ki merÃ©sz mÅ±vÃ©szi lÃ¡tomÃ¡son keresztÃ¼l.', 'A BÃ¡tor OroszlÃ¡n MÅ±vÃ©sz remekmÅ±veket alkot, amelyek bÃ¡torsÃ¡got Ã©s kÃ©pzeletet inspirÃ¡lnak mindenki szÃ¡mÃ¡ra, aki lÃ¡tja Å‘ket.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5bdb70c9-5ba3-5e01-aabd-db63514eed20', 'hero_brave_curious_safe', 'hu-hu', 'ÅrzÅ‘ OroszlÃ¡n', 'Egy vÃ©delmezÅ‘ oroszlÃ¡n, aki egyesÃ­ti a bÃ¡torsÃ¡got, kÃ­vÃ¡ncsisÃ¡got Ã©s biztonsÃ¡got.', 'Az ÅrzÅ‘ OroszlÃ¡n a birodalom felett Å‘rkÃ¶dik, biztonsÃ¡got biztosÃ­tva, mikÃ¶zben felfedezÃ©st Ã©s megismerÃ©st Ã¶sztÃ¶nÃ¶z.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4c32daf3-d28e-57c4-aca6-191eb6626580', 'hero_brave_wise_creative', 'hu-hu', 'BÃ¡tor BÃ¶lcs MÅ±vÃ©sz', 'Egy bÃ¶lcs mÅ±vÃ©sz, aki egyesÃ­ti a bÃ¡torsÃ¡got, gondolkodÃ¡st Ã©s kreativitÃ¡st.', 'A BÃ¡tor BÃ¶lcs MÅ±vÃ©sz mÃ©ly mÅ±veket alkot, amelyek egyesÃ­tik az intellektuÃ¡lis mÃ©lysÃ©get a mÅ±vÃ©szi szÃ©psÃ©ggel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('2363b36a-86a5-52e7-a6b0-70246efcda92', 'hero_brave_wise_safe', 'hu-hu', 'BÃ¡tor BÃ¶lcs ÅrzÅ‘', 'Egy vÃ©delmezÅ‘ bÃ¶lcs, aki egyesÃ­ti a bÃ¡torsÃ¡got, bÃ¶lcsessÃ©get Ã©s biztonsÃ¡got.', 'A BÃ¡tor BÃ¶lcs ÅrzÅ‘ erÅ‘vel Ã©s bÃ¶lcsessÃ©ggel vÃ©d, biztonsÃ¡got biztosÃ­tva gondos vÃ©delemmel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('70daae84-4204-546e-addb-6353ad6db2da', 'hero_brave_creative_safe', 'hu-hu', 'BÃ¡tor MÅ±vÃ©sz ÅrzÅ‘', 'Egy kreatÃ­v Å‘rzÅ‘, aki egyesÃ­ti a bÃ¡torsÃ¡got, kreativitÃ¡st Ã©s biztonsÃ¡got.', 'A BÃ¡tor MÅ±vÃ©sz ÅrzÅ‘ kreatÃ­v megoldÃ¡sokkal vÃ©d, mÅ±vÃ©szi lÃ¡tomÃ¡st kombinÃ¡lva figyelmes gondossÃ¡ggal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('cd86f6d3-0b2b-5ff9-9794-1b78402bbeca', 'hero_curious_wise_creative', 'hu-hu', 'BÃ¶lcs FelfedezÅ‘ MÅ±vÃ©sz', 'Egy intellektuÃ¡lis mÅ±vÃ©sz, aki egyesÃ­ti a kÃ­vÃ¡ncsisÃ¡got, gondolkodÃ¡st Ã©s kreativitÃ¡st.', 'A BÃ¶lcs FelfedezÅ‘ MÅ±vÃ©sz Ãºj mÅ±vÃ©szi hatÃ¡rokat fedez fel gondos felfedezÃ©sen Ã©s kreatÃ­v kifejezÃ©sen keresztÃ¼l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('40683f84-deb5-5e55-9b46-69a54c420b50', 'hero_curious_wise_safe', 'hu-hu', 'BÃ¶lcs ÅrzÅ‘ FelfedezÅ‘', 'Egy vÃ©delmezÅ‘ felfedezÅ‘, aki egyesÃ­ti a kÃ­vÃ¡ncsisÃ¡got, bÃ¶lcsessÃ©get Ã©s biztonsÃ¡got.', 'A BÃ¶lcs ÅrzÅ‘ FelfedezÅ‘ biztonsÃ¡gosan merÃ©szkedik ismeretlen terÃ¼letekre, bÃ¶lcsessÃ©g vezetÃ©sÃ©vel Ã©s Ã³vatossÃ¡g vÃ©delmÃ©vel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('8527c98a-16a6-54b2-b86f-01245588c1be', 'hero_curious_creative_safe', 'hu-hu', 'KreatÃ­v ÅrzÅ‘ FelfedezÅ‘', 'Egy talÃ¡lÃ©kony felfedezÅ‘, aki egyesÃ­ti a kÃ­vÃ¡ncsisÃ¡got, kreativitÃ¡st Ã©s biztonsÃ¡got.', 'A KreatÃ­v ÅrzÅ‘ FelfedezÅ‘ Ãºj vilÃ¡gokat fedez fel kÃ©pzeletbeli megoldÃ¡sokon keresztÃ¼l, mikÃ¶zben biztonsÃ¡gos Ã¡tkelÃ©st biztosÃ­t.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('defe07f0-5b32-5c47-87e3-a60d6db2fdde', 'hero_wise_creative_safe', 'hu-hu', 'BÃ¶lcs KreatÃ­v ÅrzÅ‘', 'Egy gondolkodÃ³ Å‘rzÅ‘, aki egyesÃ­ti a bÃ¶lcsessÃ©get, kreativitÃ¡st Ã©s biztonsÃ¡got.', 'A BÃ¶lcs KreatÃ­v ÅrzÅ‘ innovatÃ­v megoldÃ¡sokkal vÃ©d, intellektuÃ¡lis megÃ©rtÃ©st kombinÃ¡lva mÅ±vÃ©szi lÃ¡tomÃ¡ssal.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('4841b149-bd26-595d-ab10-26444df994fa', 'hero_brave_curious_wise_creative', 'hu-hu', 'Mester OroszlÃ¡n MÅ±vÃ©sz', 'A vÃ©gsÅ‘ mÅ±vÃ©szi oroszlÃ¡n, a bÃ¡torsÃ¡g, kÃ­vÃ¡ncsisÃ¡g, bÃ¶lcsessÃ©g Ã©s kreativitÃ¡s mestere.', 'A Mester OroszlÃ¡n MÅ±vÃ©sz legendÃ¡s mÅ±veket alkot, amelyek mindenkit inspirÃ¡lnak, aki tanÃºja mÅ±vÃ©szi zsenialitÃ¡sÃ¡nak Ã©s nemes szellemÃ©nek.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('5ab42d21-0ae6-5968-9202-beadc6cefdf1', 'hero_brave_curious_wise_safe', 'hu-hu', 'Mester ÅrzÅ‘ OroszlÃ¡n', 'A vÃ©gsÅ‘ vÃ©delmezÅ‘ oroszlÃ¡n, a bÃ¡torsÃ¡g, kÃ­vÃ¡ncsisÃ¡g, bÃ¶lcsessÃ©g Ã©s biztonsÃ¡g mestere.', 'A Mester ÅrzÅ‘ OroszlÃ¡n a vÃ©gsÅ‘ vÃ©delmezÅ‘kÃ©nt Ã¡ll, biztonsÃ¡got biztosÃ­tva, mikÃ¶zben felfedezÃ©st Ã©s nÃ¶vekedÃ©st Ã¶sztÃ¶nÃ¶z.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('a2d2df18-2202-52da-bd78-3929e3577555', 'hero_brave_curious_creative_safe', 'hu-hu', 'Mester FelfedezÅ‘ OroszlÃ¡n', 'A vÃ©gsÅ‘ felfedezÅ‘ oroszlÃ¡n, a bÃ¡torsÃ¡g, kÃ­vÃ¡ncsisÃ¡g, kreativitÃ¡s Ã©s biztonsÃ¡g mestere.', 'A Mester FelfedezÅ‘ OroszlÃ¡n a legkihÃ­vÃ³bb terÃ¼letekre merÃ©szkedik, csodÃ¡kat fedezve fel, mikÃ¶zben biztonsÃ¡gos Ã¡tkelÃ©st biztosÃ­t.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('7b48b0bc-9556-5f08-9a14-89b88e238b8f', 'hero_brave_wise_creative_safe', 'hu-hu', 'Mester BÃ¶lcs ÅrzÅ‘', 'A vÃ©gsÅ‘ bÃ¶lcs Å‘rzÅ‘, a bÃ¡torsÃ¡g, gondolkodÃ¡s, kreativitÃ¡s Ã©s biztonsÃ¡g mestere.', 'A Mester BÃ¶lcs ÅrzÅ‘ mÃ©ly bÃ¶lcsessÃ©get kombinÃ¡l kreatÃ­v vÃ©delemmel, biztonsÃ¡got biztosÃ­tva megvilÃ¡gosodott vezetÃ©ssel.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('9f63395d-0288-517c-8f77-68f84442b2ef', 'hero_curious_wise_creative_safe', 'hu-hu', 'Mester KreatÃ­v FelfedezÅ‘', 'A vÃ©gsÅ‘ kreatÃ­v felfedezÅ‘, a kÃ­vÃ¡ncsisÃ¡g, gondolkodÃ¡s, kreativitÃ¡s Ã©s biztonsÃ¡g mestere.', 'A Mester KreatÃ­v FelfedezÅ‘ Ãºj lehetÅ‘sÃ©gek birodalmait fedez fel innovatÃ­v gondolkodÃ¡son Ã©s mÅ±vÃ©szi lÃ¡tomÃ¡son keresztÃ¼l.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."HeroDefinitionTranslations"
    ("Id", "HeroDefinitionId", "LanguageCode", "Name", "Description", "Story")
VALUES
    ('c1f2bb07-3cbb-5e15-8186-66bb2d5a4fea', 'hero_alchimalian_dragon', 'hu-hu', 'LegendÃ¡s Alchimalia SÃ¡rkÃ¡ny', 'A legendÃ¡s sÃ¡rkÃ¡ny, aki a hÅ‘sies tulajdonsÃ¡gok tÃ¶kÃ©letessÃ©gÃ©t testesÃ­ti meg...', 'A LegendÃ¡s Alchimalia SÃ¡rkÃ¡ny a kozmikus egyensÃºly tÃ¶kÃ©letessÃ©gÃ©t testesÃ­ti meg.')
ON CONFLICT ("HeroDefinitionId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description",
    "Story" = EXCLUDED."Story";
COMMIT;

