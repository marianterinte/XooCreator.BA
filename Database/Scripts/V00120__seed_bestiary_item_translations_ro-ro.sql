-- Auto-generated from Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json
-- Locale: ro-ro
-- Run date: 2026-03-03T09:03:26.677Z
-- This script seeds BestiaryItemTranslations for ro-ro bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f931362b-9623-4a30-b3a3-799ad8859997', '5d1b4419-c890-5462-bcb7-242dbe123890', 'ro-ro', 'Iepurafix', 'Iepurafix a apărut când doi iepuri au visat împreună să atingă cerul prin ochii unei girafe. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al curiozității și al curajului, sărind între lumi și aducând lumină în umbre.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e5ced02d-b026-42c3-89e9-68b0ce448715', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'ro-ro', 'Iepuripotix', 'Iepuripotix s-a născut din visul a doi iepuri care au cerut forța unui hipopotam pentru a-și apăra prietenii. Copacul Luminii le-a unit sufletele. El este un spirit jucăuș dar protector, aducând curaj celor mici și liniște celor rătăciți.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('79b6b621-c5e6-4502-a7fc-64469571c3af', 'fc1b57f3-4751-51f5-a497-174648433b88', 'ro-ro', 'Iepurafonix', 'Iepurafonix s-a ivit atunci când doi iepuri și-au dorit să vadă lumea de sus. Copacul Luminii a răspuns și le-a oferit trupul unei girafe, dar i-a păstrat inimile și chipurile jucăușe. El este un spirit al viselor și al speranței, purtând lumina copilăriei în ochii săi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('52d9ddb0-3485-4bc7-bebf-4fc51bbf8390', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'ro-ro', 'Iepurogaf', 'Iepurogaf a apărut când un iepure a dorit să împărtășească jocul său cu două girafe. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al curiozității și al prieteniei, unind agerimea cu măreția.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6243ca30-dba7-4696-8ae8-a89b2946613d', 'b67d8c08-04ec-539d-8787-27e838807428', 'ro-ro', 'Iepogafotam', 'Iepogafotam s-a născut dintr-o alianță între un iepure curajos, o girafă înțeleaptă și un hipopotam puternic. Copacul Luminii i-a unit într-un singur trup pentru a veghea peste ramurile fragile. Iepogafotam este un spirit al unității și al speranței, aducând echilibru între viteză, înălțime și forță.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('abf20662-24e1-4149-b8f5-a6babe636d97', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'ro-ro', 'Iepurafonel', 'Iepurafonel s-a născut atunci când un iepure a încercat să salveze o girafă blocată între umbre. Copacul Luminii a unit sacrificiul lor într-un trup comun. Iepurafonel este un spirit al speranței și al prieteniei, aducând curaj celor mici.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('65e5dbb4-5fb3-4062-a962-480ad8c581ae', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'ro-ro', 'Iepopoturel', 'Iepopoturel s-a născut atunci când un iepure a cerut prietenia unui hipopotam, iar Copacul Luminii le-a unit destinele. Cu brațe agile și cap jucăuș, dar trup solid, el este un spirit al echilibrului între fragilitate și putere, aducând veselie și protecție în ramurile lumilor.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('aec163cf-7309-40b1-accd-ca563bd9707e', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'ro-ro', 'Iepogiraf', 'Iepogiraf este rezultatul unei prietenii între trei creaturi care au luptat împreună să salveze o ramură a Copacului Luminii. Din unitatea lor s-a născut o ființă ce poartă agilitatea iepurelui, puterea hipopotamului și viziunea girafei. Iepogiraf este un spirit al unității și al încrederii.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f5154b6d-33a0-409d-accc-a5e4cbfcb727', '558b9240-3c3a-5017-a85a-706a397288fd', 'ro-ro', 'Iepopotix', 'Iepopotix s-a născut când un iepure a adus bucurie a doi hipopotami. Copacul Luminii le-a unit într-o ființă protectoare și veselă. El este un spirit al prieteniei și al curajului, aducând lumină în întuneric.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0285cfb6-5c8d-43c2-9f5b-9f6525dbfc71', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'ro-ro', 'Iepopotam', 'Iepopotam s-a născut atunci când un iepure și un hipopotam s-au întâlnit în mijlocul unei furtuni de ape negre. Din prietenia lor neașteptată, Copacul Luminii a împletit agilitatea iepurelui cu forța hipopotamului. Iepopotamul este blând și protector: cu brațele sale sprintene sapă ascunzișuri, iar cu trupul său puternic deschide drumuri prin ape. El aduce liniște acolo unde e teamă și ridică poduri de încredere între lumi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e5428695-2bd8-45b6-a4f2-3cd58cb10056', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'ro-ro', 'Iepugaf', 'Iepugaf a apărut atunci când un iepure a dorit să vadă lumea de sus prin ochii unei girafe. Copacul Luminii a unit dorința lor într-un singur trup. Iepugaf este un spirit al curiozității și al prieteniei, inspirând pe cei mici să viseze mai departe.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1e98ce05-2a8f-4f92-a05f-b723b8bd00d8', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'ro-ro', 'Iepurotam', 'Iepurotam s-a ivit atunci când un iepure și un hipopotam au stat împreună sub furtuna nopții. Copacul Luminii i-a împletit într-un singur trup, păstrând agilitatea iepurelui și înțelepciunea hipopotamului. Iepurotam este un simbol al solidarității și al prieteniei dintre cei mici și cei mari.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6a3c1cd0-caa4-42ad-8924-44011dbad299', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'ro-ro', 'Girafonix', 'Girafonix a apărut atunci când o girafă a îmbrățișat doi iepuri. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al compasiunii și al jocului, aducând veselie în ramurile lumilor.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('fb4397b8-f86b-43e0-9d00-66c806e435db', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'ro-ro', 'Girafiepix', 'Girafiepix s-a născut atunci când două girafe au protejat un iepure mic. Copacul Luminii i-a unit într-un singur trup, dăruindu-le agilitatea pământului și viziunea cerului. El este un spirit al prieteniei și al înțelepciunii, capabil să unească lumi opuse.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('916d269c-c23c-4156-b323-0dcca6b4d363', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'ro-ro', 'Girafopam', 'Girafopam a apărut atunci când trei creaturi au jurat să apere împreună ramurile copacului. Din prietenia lor, Copacul Luminii a creat un trup comun. Girafopam este un spirit al unității și al prieteniei, purtând în el speranța de a uni lumi diferite.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('fd697092-7113-40e2-b459-ed0110a1c709', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'ro-ro', 'Girafon', 'Girafon s-a născut atunci când brațele unei girafe au îmbrățișat un iepure mic care tremura în noapte. Din acea clipă, Copacul Luminii i-a unit într-un hibrid neobișnuit. Girafon este jucăuș și neînfricat: folosește brațele lungi pentru a aduna stele și trupul de iepure pentru a sări printre umbre. El este un spirit al bucuriei, aducând râsete și lumină oriunde ajunge.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3f49f32c-e990-4427-a35a-c5a4244b9f40', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'ro-ro', 'Girafurelix', 'Girafurelix s-a ivit atunci când două girafe au dorit să-și împartă înălțimea cu un iepure mic. Copacul Luminii le-a unit într-o creatură a prieteniei și a veseliei. Girafurelix aduce speranță celor mici, arătând că măreția poate fi împărtășită.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('16de6ced-eb70-40fa-a344-29c5f88a2999', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'ro-ro', 'Girafodublu', 'Girafodublu s-a născut când două spirite de girafă au vegheat împreună un hipopotam rătăcit. Copacul Luminii le-a unit într-o ființă unică, înaltă și puternică, dar blândă. Girafodublu este un paznic al orizonturilor, aducând echilibru între forță și viziune.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('99e5548b-8daa-4d6b-9237-f9eee7ff281f', 'eb68fa55-42df-56d7-9169-70148069d13c', 'ro-ro', 'Girafotinel', 'Girafotinel s-a născut atunci când o girafă și un hipopotam au protejat un iepure de întuneric. Copacul Luminii le-a răsplătit prietenia cu un trup comun. Girafotinel este un spirit al protecției și al bucuriei, păstrând echilibrul între forță și blândețe.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('29b7448f-3ba0-42d1-a41b-8f806b7f4407', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'ro-ro', 'Girafopotrel', 'Girafopotrel a apărut atunci când două girafe au vegheat un hipopotam rătăcit sub lumina stelelor. Copacul Luminii le-a unit pentru a păstra echilibrul între cer și apă. El este un spirit al protecției și al clarității, aducând viziune și putere blândă.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('95a6632e-f872-41b0-9041-eeeeeb0f3cbd', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'ro-ro', 'Girafopotix', 'Girafopotix a apărut când o girafă a jurat să vegheze doi hipopotami în mijlocul unei furtuni. Copacul Luminii le-a unit într-un singur trup. El este un spirit al protecției și al prieteniei, purtând în el puterea apelor și viziunea cerului.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2cd12d49-113c-4642-b772-dd03f7800156', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'ro-ro', 'Girafopot', 'Girafopot s-a născut atunci când o girafă și un hipopotam au vegheat împreună peste o ramură căzută. Copacul Luminii a văzut curajul lor și i-a unit într-un trup comun. Girafopot este un spirit al echilibrului, aducând protecție și viziune celor care au nevoie.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f64e3e53-0525-4fd3-b796-8505e3385029', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'ro-ro', 'Girafiep', 'Girafiep s-a ivit atunci când un iepure și o girafă au vegheat împreună peste o ramură căzută a Copacului Luminii. Din prietenia lor a răsărit această creatură unică. Girafiep este un spirit al prieteniei și al speranței, aducând lumină acolo unde umbrele se adună.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('89d1226a-540f-42ce-a335-677a4e34e7c1', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'ro-ro', 'Girafotamiel', 'Girafotamiel s-a născut atunci când o girafă a ridicat privirea spre stele și un hipopotam a cântat în apele adânci. Copacul Luminii a unit cântecul și visul lor într-un trup comun. Girafotamiel este un spirit al armoniei și al liniștii, aducând pace între cer și apă.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('cb6edbb7-db54-464d-99b1-2987ee64cd1d', '880c0786-0e60-51d0-be8f-0822a13d439a', 'ro-ro', 'Hiponepix', 'Hiponepix s-a născut când un hipopotam a protejat doi iepuri înfricoșați de întuneric. Copacul Luminii le-a unit într-un trup comun. El este un spirit al bucuriei și al rezistenței, aducând speranță celor mici.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6ed240c9-bed8-4ad8-8634-929f99033dfd', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'ro-ro', 'Hiponaf', 'Hiponaf a apărut atunci când un hipopotam, un iepure și o girafă au legat o prietenie imposibilă. Copacul Luminii i-a unit pentru a arăta că diferențele pot deveni puteri. Hiponaf este un spirit al colaborării și al prieteniei, un protector vesel al ramurilor.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6c2285d0-e648-474a-99d7-f3232d5cb151', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'ro-ro', 'Hipopoturel', 'Hipopoturel a apărut atunci când doi hipopotami au dorit să protejeze un iepure fragil. Copacul Luminii i-a unit într-un trup comun, mic dar puternic. El este un spirit al protecției și al curajului, aducând echilibru între joacă și forță.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('33bfaff2-dac2-4af6-8c18-9a117646bd62', '3de3b3e0-5241-5350-b01b-28361c99c363', 'ro-ro', 'Hiponelus', 'Hiponelus s-a născut atunci când un hipopotam și un iepure au împărțit aceeași vizuină în mijlocul furtunii. Copacul Luminii a văzut solidaritatea lor și i-a unit într-un trup comun. Hiponelus este un spirit al prieteniei și al rezistenței, mereu prezent acolo unde umbrele se apropie.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d532045b-ed04-4209-9771-2edaec712236', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'ro-ro', 'Hipogiriel', 'Hipogiriel s-a născut în clipa în care un hipopotam, o girafă și un iepure au ridicat împreună privirea spre lumina Copacului. Spiritele lor s-au unit într-o ființă nouă. Hipogiriel este un spirit al colaborării și al echilibrului, aducând pace și înțelepciune în luptele dintre lumi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('ac8a33c1-8188-4da3-a93e-fae851b327d6', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'ro-ro', 'Hipogirafox', 'Hipogirafox s-a ivit atunci când un hipopotam și două girafe au vegheat împreună o ramură a copacului. Din prietenia lor s-a născut această ființă unică, un spirit al echilibrului și al înțelepciunii.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('ff899d21-69ed-4b7e-b5e0-4111de7e5bfb', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'ro-ro', 'Hipogafon', 'Hipogafon s-a născut atunci când doi hipopotami și o girafă și-au împărțit un vis comun: să unească apele și cerul. Copacul Luminii i-a unit într-un singur trup. El este un spirit al stabilității și al înțelepciunii, aducând claritate în mijlocul furtunii.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('93734dc8-9272-4c5e-bb4e-507bbd996d4b', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'ro-ro', 'Hipogaf', 'Hipogaf s-a născut când un hipopotam și o girafă au apărat împreună o ramură a Copacului Luminii. Din sacrificiul lor, copacul i-a unit într-un singur trup. Hipogaf este un spirit al rezistenței și clarității, capabil să protejeze și să vegheze asupra celor pierduți.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1ad370f1-f969-4b09-bfe2-df22cb53e63b', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'ro-ro', 'Hiponelix', 'Hiponelix a apărut când doi hipopotami au jurat să protejeze un iepure. Copacul Luminii le-a unit într-un singur trup. El este un spirit al blândeții și al forței, aducând pace între lumi.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('386e4908-0d2c-43fc-bb9b-d1f8a037dc73', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'ro-ro', 'Hipogafix', 'Hipogafix s-a născut atunci când doi hipopotami au căutat înțelepciunea unei girafe. Copacul Luminii i-a unit într-un spirit al stabilității și clarității. El veghează asupra ramurilor, aducând echilibru între apă și cer.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('78d12977-d14d-494f-9408-be002c1ce94e', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'ro-ro', 'Hipopurel', 'Hipopurel s-a ivit atunci când un iepure și un hipopotam s-au ascuns împreună de umbre. Copacul Luminii le-a unit sufletele pentru a crea un protector jucăuș. Hipopurel este un spirit vesel, dar hotărât, gata să apere lumina.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3cf0e367-f2c8-4853-8dde-c02ae29b2947', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'ro-ro', 'Hipogiraf', 'Hipogiraf s-a născut atunci când un hipopotam a dorit să privească lumea prin ochii unei girafe. Copacul Luminii le-a unit visurile într-un trup comun. Hipogiraf este un spirit al viziunii și al stabilității, capabil să aducă echilibru între forță și înțelepciune.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('aaa624a6-dd74-4b69-89b5-96588fa9de9b', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'ro-ro', 'Iepuragiraf', 'Iepuragiraf a apărut atunci când un iepure a dorit să vegheze asupra unei savane. Copacul Luminii i-a ascultat dorința și l-a unit cu spiritul unei girafe. Iepuragiraf este un spirit al responsabilității și al bucuriei, purtând în el dorința de a proteja.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8950e791-824f-40f0-b3dd-a179323ca81b', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'ro-ro', 'Iepuripotam', 'Iepuripotam a apărut atunci când un iepure a cerut protecția unui hipopotam pentru a apăra poiana sa. Copacul Luminii i-a unit într-un trup comun. Iepuripotam este un spirit protector și blând, care aduce liniște celor înfricoșați.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8d12a34a-15cb-41b1-a9d8-fe510204d2e3', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'ro-ro', 'Girafunel', 'Girafunel s-a născut atunci când un iepure s-a urcat pe umerii unei girafe pentru a privi cerul. Copacul Luminii a văzut bucuria lor și i-a unit într-un trup comun. Girafunel este un spirit jucăuș și prietenos, aducând veselie și speranță peste tot.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d0f79da2-acf0-42b6-a64c-50509985d033', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'ro-ro', 'Girafopotir', 'Girafopotir s-a născut când o girafă și un hipopotam au vegheat împreună peste ramurile fragile ale Copacului Luminii. Din prietenia lor s-a născut această ființă unică, un spirit al rezistenței și al stabilității.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3b95ac09-09d8-4cf4-af58-06d19e0dbb25', '07084a31-3e3a-5b86-8536-56d2311359bd', 'ro-ro', 'Hipuriel', 'Hipuriel a apărut atunci când un iepure și un hipopotam au cântat împreună lângă un lac luminat de stele. Copacul Luminii le-a împletit cântecul și râsul într-un trup comun. Hipuriel este un spirit al veseliei și al armoniei, aducând speranță celor care rătăcesc.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c875c5d2-541a-48de-bac7-2174076ad0cc', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'ro-ro', 'Hiporaf', 'Hiporaf s-a născut atunci când un hipopotam a cerut înălțimea girafei pentru a veghea mai departe. Copacul Luminii a răspuns unindu-i într-o ființă unică. Hiporaf este un spirit al vigilenței și al rezistenței, aducând protecție și speranță.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
