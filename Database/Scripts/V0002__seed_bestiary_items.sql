-- Auto-generated from Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json
-- Run date: 2025-11-27 11:59:30+02:00

BEGIN;
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'Bunny', 'Giraffe', '—', 'Iepurafa', 'În ramurile Copacului Luminii s-a născut Iepurafa, o creatură unică, jumătate iepure, jumătate girafă. Brațele sale scurte și sprintene sapă tuneluri și aprind focuri vii, iar trupul său înalt, împodobit cu pete strălucitoare, vede departe în zări unde alții nu pot ajunge.

Legenda spune că Iepurafa s-a ivit atunci când un iepure mic și curajos s-a ascuns la rădăcina unei girafe bătrâne, în timp ce întunericul devora savana. Din îmbrățișarea lor disperată, Copacul Luminii a împletit două spirite într-un singur trup.

Iepurafa este un paznic al orizonturilor. Când aleargă prin iarbă, pământul tresaltă de bucurie. Când se ridică la luptă, umbrele fug rușinate. Însă puterea lui nu stă doar în viteză sau înălțime, ci în prietenia pe care o oferă celor ce rătăcesc printre ramurile lumilor.

Spiritele vechi spun că Iepurafa va fi mereu acolo unde cineva încearcă să repare ce a fost rupt, aducând curaj și speranță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'Bunny', 'Hippo', '—', 'Iepopotam', 'Iepopotam s-a născut atunci când un iepure și un hipopotam s-au întâlnit în mijlocul unei furtuni de ape negre. Din prietenia lor neașteptată, Copacul Luminii a împletit agilitatea iepurelui cu forța hipopotamului. Iepopotamul este blând și protector: cu brațele sale sprintene sapă ascunzișuri, iar cu trupul său puternic deschide drumuri prin ape. El aduce liniște acolo unde e teamă și ridică poduri de încredere între lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('ece938ea-108a-5c77-9471-5a65f3c5adaf', 'Giraffe', 'Hippo', '—', 'Girafopotam', 'Girafopotam este o creatură născută din visul unei girafe care privea spre stele și din cântecul unui hipopotam care răsuna sub ape. Copacul Luminii i-a unit într-un singur trup pentru a da echilibru: înălțimea girafei și stabilitatea hipopotamului. El veghează peste savanele de lumină și apele adânci, căutând să unească cerul cu pământul. Girafopotamul este un mediator între lumi, aducând înțelepciune și putere blândă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('b67d8c08-04ec-539d-8787-27e838807428', 'Bunny', 'Giraffe', 'Hippo', 'Iepurafopotam', 'Iepurafopotam este un erou legendar, născut dintr-o alianță imposibilă: un iepure, o girafă și un hipopotam care au ales să-și unească destinele. Copacul Luminii a văzut curajul lor și le-a dat o singură inimă. Cu brațele iepurelui, el ajută și protejează. Cu trupul girafei, vede departe. Cu capul hipopotamului, inspiră liniște și înțelepciune. El este un simbol al diversității și al prieteniei dintre lumi diferite.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('88010526-1b36-56eb-ade7-e6b60c80ae86', 'Giraffe', 'Bunny', '—', 'Girafon', 'Girafon s-a născut atunci când brațele unei girafe au îmbrățișat un iepure mic care tremura în noapte. Din acea clipă, Copacul Luminii i-a unit într-un hibrid neobișnuit. Girafon este jucăuș și neînfricat: folosește brațele lungi pentru a aduna stele și trupul de iepure pentru a sări printre umbre. El este un spirit al bucuriei, aducând râsete și lumină oriunde ajunge.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('eb68fa55-42df-56d7-9169-70148069d13c', 'Giraffe', 'Hippo', 'Bunny', 'Girafopotaniu', 'Girafopotaniu s-a născut în mijlocul unei furtuni de stele, atunci când o girafă, un hipopotam și un iepure au jurat să apere Copacul Luminii. Din jurământul lor, ramurile au unit trei spirite într-un singur trup. El este un simbol al prieteniei între ființe complet diferite. Cu brațele girafei ridică lumina spre cer, cu trupul hipopotamului protejează pământul, iar cu capul iepurelui aduce zâmbete și speranță. Girafopotaniu este văzut ca un gardian al echilibrului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('3de3b3e0-5241-5350-b01b-28361c99c363', 'Hippo', 'Bunny', '—', 'Hipurilă', 'Hipurilă s-a născut dintr-un vis jucăuș în care un iepure a cerut puterea unui hipopotam. Copacul Luminii i-a unit într-o ființă echilibrată: mică dar puternică, blândă dar hotărâtă. Hipurilă folosește brațele hipopotamului pentru a proteja și trupul de iepure pentru a sări printre obstacole. El aduce curaj celor mici și le arată că mărimea nu definește puterea unui suflet.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'Hippo', 'Giraffe', '—', 'Hipogira', 'Hipogira s-a născut atunci când un hipopotam a jurat să apere o girafă rătăcită de întuneric. Copacul Luminii i-a unit într-o singură ființă pentru a păstra promisiunea lor. Ea este puternică și înțeleaptă, ridicând poduri între cer și pământ. Hipogira este cunoscută drept spiritul care aduce echilibru între forță și grație.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('d86bb1a9-645b-5f66-8e6e-6c93457d2618', 'Hippo', 'Bunny', 'Giraffe', 'Hipurafon', 'Hipurafon este o creatură a contrastelor, născută atunci când trei lumi s-au ciocnit în Copacul Luminii. Din hipopotam a primit puterea, din iepure agilitatea și din girafă viziunea. El cutreieră ramurile copacului căutând armonia între extreme. Hipurafon este adesea descris ca un spirit care arată că adevărata putere vine din unirea celor diferiți.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'Bunny', 'Hippo', 'Giraffe', 'Iepopotira', 'Iepopotira s-a născut în mijlocul unei furtuni de lumină, atunci când trei prieteni — un iepure, un hipopotam și o girafă — au încercat să repare o ramură ruptă a Copacului Luminii. Sacrificiul lor a fost transformat în magie, și astfel s-a născut această creatură unică. Iepopotira este un paznic blând, care poartă în el speranța și curajul prieteniei.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('105daf4a-19b5-5d6b-b7b3-75532adb8790', 'Giraffe', '—', 'Hippo', 'Girafopotir', 'Girafopotir s-a născut dintr-un legământ între un hipopotam și o girafă care au vegheat împreună asupra unei ramuri căzute a Copacului Luminii. Copacul le-a unit forțele și le-a oferit un singur trup. Girafopotir este un spirit al echilibrului între putere și blândețe, purtând în el înțelepciunea celor două lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'Bunny', '—', 'Hippo', 'Iepurotam', 'Iepurotam s-a ivit atunci când un iepure și un hipopotam au stat împreună sub furtuna nopții. Copacul Luminii i-a împletit într-un singur trup, păstrând agilitatea iepurelui și înțelepciunea hipopotamului. Iepurotam este un simbol al solidarității și al prieteniei dintre cei mici și cei mari.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'Hippo', '—', 'Bunny', 'Hiponel', 'Hiponel a apărut atunci când un iepure a cerut protecția unui hipopotam pentru a supraviețui întunericului. Copacul Luminii le-a unit sufletele și a dat viață acestei ființe jucăușe, dar puternice. Hiponel este un protector vesel, aducând speranță acolo unde e teamă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('e2af4281-d05b-59d9-8532-2b64e395dd6b', '—', 'Bunny', 'Hippo', 'Iepuripos', 'Iepuripos s-a născut atunci când un iepure a visat să aibă puterea unui hipopotam. Copacul Luminii a transformat visul în realitate, unind două lumi opuse. Iepuripos este blând și pașnic, dar are o forță ascunsă care se trezește pentru a apăra lumina. El este un simbol al visurilor împlinite.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('d4b242da-e3c3-52ad-9c9e-022bc0f0db7e', '—', 'Giraffe', 'Hippo', 'Girafotam', 'Girafotam s-a ivit atunci când un hipopotam a căutat înălțimea unei girafe pentru a veghea peste savană. Copacul Luminii i-a unit și le-a oferit un trup comun. Girafotam este un spirit al stabilității, păstrând echilibrul între ape și cer. El este un protector al orizonturilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('430ea06a-9ce9-57c7-a0b1-6353996bf8b8', '—', 'Hippo', 'Giraffe', 'Hipogirafon', 'Hipogirafon s-a născut atunci când un hipopotam a visat să privească mai departe decât orizontul apelor. Copacul Luminii i-a unit cu spiritul unei girafe și i-a dat trup comun. Hipogirafon este un spirit al viziunii și al rezistenței, unind stabilitatea apelor cu înălțimea cerului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('05126872-ffdf-556d-818b-8df0d0f52fdf', 'Bunny', '—', 'Giraffe', 'Iepurafon', 'Iepurafon a fost creat atunci când un iepure și o girafă au încercat împreună să lumineze o ramură căzută a Copacului Luminii. Magia i-a unit într-un singur trup. Iepurafon este un spirit al curajului și al prieteniei, capabil să aducă speranță și lumină în întuneric.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'Giraffe', '—', 'Bunny', 'Girafurel', 'Girafurel s-a ivit din râsul unui iepure care s-a agățat de gâtul unei girafe. Copacul Luminii a găsit bucuria lor și le-a dat un trup comun. Girafurel este jucăuș și vesel, dar are și o inimă mare, gata să apere lumina. El este adesea descris ca un spirit al optimismului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'Hippo', '—', 'Giraffe', 'Hipofiraf', 'Hipofiraf s-a născut când un hipopotam și o girafă au vegheat împreună peste savană. Copacul Luminii i-a unit și le-a oferit o inimă comună. Hipofiraf este un spirit al vigilenței, aducând echilibru între forță și claritate. El este un protector al nopților înstelate.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('07084a31-3e3a-5b86-8536-56d2311359bd', '—', 'Hippo', 'Bunny', 'Hiponeluș', 'Hiponeluș a apărut atunci când un iepure a adormit pe spatele unui hipopotam sub razele Copacului Luminii. Magia i-a unit și a dat naștere unei creaturi pașnice și curajoase. Hiponeluș este cunoscut ca un spirit al prieteniei și al jocului, aducând zâmbete oriunde ajunge.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('420cc407-c891-5ff8-8f2a-c50c4242563f', '—', 'Bunny', 'Giraffe', 'Iepurafira', 'Iepurafira s-a născut atunci când un iepure a cerut ajutorul unei girafe pentru a veghea peste o poiană. Copacul Luminii le-a unit sufletele și a creat această ființă unică. Iepurafira este un spirit al vigilenței și al compasiunii, mereu prezent pentru a aduce echilibru între fragilitate și măreție.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'Bunny', 'Hippo', 'Giraffe', 'Iepogiraf', 'Iepogiraf este rezultatul unei prietenii între trei creaturi care au luptat împreună să salveze o ramură a Copacului Luminii. Din unitatea lor s-a născut o ființă ce poartă agilitatea iepurelui, puterea hipopotamului și viziunea girafei. Iepogiraf este un spirit al unității și al încrederii.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('d35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'Hippo', 'Giraffe', 'Bunny', 'Hipogiriel', 'Hipogiriel s-a născut în clipa în care un hipopotam, o girafă și un iepure au ridicat împreună privirea spre lumina Copacului. Spiritele lor s-au unit într-o ființă nouă. Hipogiriel este un spirit al colaborării și al echilibrului, aducând pace și înțelepciune în luptele dintre lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'Giraffe', 'Bunny', 'Hippo', 'Girafopam', 'Girafopam a apărut atunci când trei creaturi au jurat să apere împreună ramurile copacului. Din prietenia lor, Copacul Luminii a creat un trup comun. Girafopam este un spirit al unității și al prieteniei, purtând în el speranța de a uni lumi diferite.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('430ea06a-9ce9-57c7-a0b1-6353996bf8b8', '—', 'Hippo', 'Giraffe', 'Hiporaf', 'Hiporaf s-a născut atunci când un hipopotam a cerut înălțimea girafei pentru a veghea mai departe. Copacul Luminii a răspuns unindu-i într-o ființă unică. Hiporaf este un spirit al vigilenței și al rezistenței, aducând protecție și speranță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'Hippo', 'Giraffe', '—', 'Hipogaf', 'Hipogaf s-a născut când un hipopotam și o girafă au apărat împreună o ramură a Copacului Luminii. Din sacrificiul lor, copacul i-a unit într-un singur trup. Hipogaf este un spirit al rezistenței și clarității, capabil să protejeze și să vegheze asupra celor pierduți.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('05126872-ffdf-556d-818b-8df0d0f52fdf', 'Bunny', '—', 'Giraffe', 'Iepugaf', 'Iepugaf a apărut atunci când un iepure a dorit să vadă lumea de sus prin ochii unei girafe. Copacul Luminii a unit dorința lor într-un singur trup. Iepugaf este un spirit al curiozității și al prieteniei, inspirând pe cei mici să viseze mai departe.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('f9f2a40c-59d8-588b-893c-7025a14dc209', '—', 'Giraffe', 'Bunny', 'Girafunel', 'Girafunel s-a născut atunci când un iepure s-a urcat pe umerii unei girafe pentru a privi cerul. Copacul Luminii a văzut bucuria lor și i-a unit într-un trup comun. Girafunel este un spirit jucăuș și prietenos, aducând veselie și speranță peste tot.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('d86bb1a9-645b-5f66-8e6e-6c93457d2618', 'Hippo', 'Bunny', 'Giraffe', 'Hiponaf', 'Hiponaf a apărut atunci când un hipopotam, un iepure și o girafă au legat o prietenie imposibilă. Copacul Luminii i-a unit pentru a arăta că diferențele pot deveni puteri. Hiponaf este un spirit al colaborării și al prieteniei, un protector vesel al ramurilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('eb68fa55-42df-56d7-9169-70148069d13c', 'Giraffe', 'Hippo', 'Bunny', 'Girafotinel', 'Girafotinel s-a născut atunci când o girafă și un hipopotam au protejat un iepure de întuneric. Copacul Luminii le-a răsplătit prietenia cu un trup comun. Girafotinel este un spirit al protecției și al bucuriei, păstrând echilibrul între forță și blândețe.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('b67d8c08-04ec-539d-8787-27e838807428', 'Bunny', 'Giraffe', 'Hippo', 'Iepogafotam', 'Iepogafotam s-a născut dintr-o alianță între un iepure curajos, o girafă înțeleaptă și un hipopotam puternic. Copacul Luminii i-a unit într-un singur trup pentru a veghea peste ramurile fragile. Iepogafotam este un spirit al unității și al speranței, aducând echilibru între viteză, înălțime și forță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('e2af4281-d05b-59d9-8532-2b64e395dd6b', '—', 'Bunny', 'Hippo', 'Iepuripotam', 'Iepuripotam a apărut atunci când un iepure a cerut protecția unui hipopotam pentru a apăra poiana sa. Copacul Luminii i-a unit într-un trup comun. Iepuripotam este un spirit protector și blând, care aduce liniște celor înfricoșați.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('105daf4a-19b5-5d6b-b7b3-75532adb8790', 'Giraffe', '—', 'Hippo', 'Girafotamiel', 'Girafotamiel s-a născut atunci când o girafă a ridicat privirea spre stele și un hipopotam a cântat în apele adânci. Copacul Luminii a unit cântecul și visul lor într-un trup comun. Girafotamiel este un spirit al armoniei și al liniștii, aducând pace între cer și apă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'Hippo', '—', 'Bunny', 'Hipopurel', 'Hipopurel s-a ivit atunci când un iepure și un hipopotam s-au ascuns împreună de umbre. Copacul Luminii le-a unit sufletele pentru a crea un protector jucăuș. Hipopurel este un spirit vesel, dar hotărât, gata să apere lumina.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('d4b242da-e3c3-52ad-9c9e-022bc0f0db7e', '—', 'Giraffe', 'Hippo', 'Girafopotir', 'Girafopotir s-a născut când o girafă și un hipopotam au vegheat împreună peste ramurile fragile ale Copacului Luminii. Din prietenia lor s-a născut această ființă unică, un spirit al rezistenței și al stabilității.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('3de3b3e0-5241-5350-b01b-28361c99c363', 'Hippo', 'Bunny', '—', 'Hiponelus', 'Hiponelus s-a născut atunci când un hipopotam și un iepure au împărțit aceeași vizuină în mijlocul furtunii. Copacul Luminii a văzut solidaritatea lor și i-a unit într-un trup comun. Hiponelus este un spirit al prieteniei și al rezistenței, mereu prezent acolo unde umbrele se apropie.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('07084a31-3e3a-5b86-8536-56d2311359bd', '—', 'Hippo', 'Bunny', 'Hipuriel', 'Hipuriel a apărut atunci când un iepure și un hipopotam au cântat împreună lângă un lac luminat de stele. Copacul Luminii le-a împletit cântecul și râsul într-un trup comun. Hipuriel este un spirit al veseliei și al armoniei, aducând speranță celor care rătăcesc.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('ece938ea-108a-5c77-9471-5a65f3c5adaf', 'Giraffe', 'Hippo', '—', 'Girafopot', 'Girafopot s-a născut atunci când o girafă și un hipopotam au vegheat împreună peste o ramură căzută. Copacul Luminii a văzut curajul lor și i-a unit într-un trup comun. Girafopot este un spirit al echilibrului, aducând protecție și viziune celor care au nevoie.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'Bunny', 'Giraffe', '—', 'Iepurafonel', 'Iepurafonel s-a născut atunci când un iepure a încercat să salveze o girafă blocată între umbre. Copacul Luminii a unit sacrificiul lor într-un trup comun. Iepurafonel este un spirit al speranței și al prieteniei, aducând curaj celor mici.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('420cc407-c891-5ff8-8f2a-c50c4242563f', '—', 'Bunny', 'Giraffe', 'Iepuragiraf', 'Iepuragiraf a apărut atunci când un iepure a dorit să vegheze asupra unei savane. Copacul Luminii i-a ascultat dorința și l-a unit cu spiritul unei girafe. Iepuragiraf este un spirit al responsabilității și al bucuriei, purtând în el dorința de a proteja.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'Hippo', '—', 'Giraffe', 'Hipogiraf', 'Hipogiraf s-a născut atunci când un hipopotam a dorit să privească lumea prin ochii unei girafe. Copacul Luminii le-a unit visurile într-un trup comun. Hipogiraf este un spirit al viziunii și al stabilității, capabil să aducă echilibru între forță și înțelepciune.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'Giraffe', '—', 'Bunny', 'Girafiep', 'Girafiep s-a ivit atunci când un iepure și o girafă au vegheat împreună peste o ramură căzută a Copacului Luminii. Din prietenia lor a răsărit această creatură unică. Girafiep este un spirit al prieteniei și al speranței, aducând lumină acolo unde umbrele se adună.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('6a468b99-597b-5a87-b5d5-7dcade042a3c', 'Giraffe', 'Giraffe', 'Hippo', 'Girafodublu', 'Girafodublu s-a născut când două spirite de girafă au vegheat împreună un hipopotam rătăcit. Copacul Luminii le-a unit într-o ființă unică, înaltă și puternică, dar blândă. Girafodublu este un paznic al orizonturilor, aducând echilibru între forță și viziune.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'Giraffe', 'Giraffe', 'Bunny', 'Girafurelix', 'Girafurelix s-a ivit atunci când două girafe au dorit să-și împartă înălțimea cu un iepure mic. Copacul Luminii le-a unit într-o creatură a prieteniei și a veseliei. Girafurelix aduce speranță celor mici, arătând că măreția poate fi împărtășită.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('5d1b4419-c890-5462-bcb7-242dbe123890', 'Bunny', 'Bunny', 'Giraffe', 'Iepurafix', 'Iepurafix a apărut când doi iepuri au visat împreună să atingă cerul prin ochii unei girafe. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al curiozității și al curajului, sărind între lumi și aducând lumină în umbre.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('99d40f35-ede6-5dc5-8484-e22327080aa1', 'Bunny', 'Bunny', 'Hippo', 'Iepuripotix', 'Iepuripotix s-a născut din visul a doi iepuri care au cerut forța unui hipopotam pentru a-și apăra prietenii. Copacul Luminii le-a unit sufletele. El este un spirit jucăuș dar protector, aducând curaj celor mici și liniște celor rătăciți.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'Hippo', 'Hippo', 'Bunny', 'Hiponelix', 'Hiponelix a apărut când doi hipopotami au jurat să protejeze un iepure. Copacul Luminii le-a unit într-un singur trup. El este un spirit al blândeții și al forței, aducând pace între lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'Hippo', 'Hippo', 'Giraffe', 'Hipogafix', 'Hipogafix s-a născut atunci când doi hipopotami au căutat înțelepciunea unei girafe. Copacul Luminii i-a unit într-un spirit al stabilității și clarității. El veghează asupra ramurilor, aducând echilibru între apă și cer.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'Giraffe', 'Hippo', 'Hippo', 'Girafopotix', 'Girafopotix a apărut când o girafă a jurat să vegheze doi hipopotami în mijlocul unei furtuni. Copacul Luminii le-a unit într-un singur trup. El este un spirit al protecției și al prieteniei, purtând în el puterea apelor și viziunea cerului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('558b9240-3c3a-5017-a85a-706a397288fd', 'Bunny', 'Hippo', 'Hippo', 'Iepopotix', 'Iepopotix s-a născut când un iepure a adus bucurie a doi hipopotami. Copacul Luminii le-a unit într-o ființă protectoare și veselă. El este un spirit al prieteniei și al curajului, aducând lumină în întuneric.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'Bunny', 'Giraffe', 'Giraffe', 'Iepurogaf', 'Iepurogaf a apărut când un iepure a dorit să împărtășească jocul său cu două girafe. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al curiozității și al prieteniei, unind agerimea cu măreția.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('d27c226d-6d8a-5257-a77d-477e0167a815', 'Hippo', 'Giraffe', 'Giraffe', 'Hipogirafox', 'Hipogirafox s-a ivit atunci când un hipopotam și două girafe au vegheat împreună o ramură a copacului. Din prietenia lor s-a născut această ființă unică, un spirit al echilibrului și al înțelepciunii.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('880c0786-0e60-51d0-be8f-0822a13d439a', 'Hippo', 'Bunny', 'Bunny', 'Hiponepix', 'Hiponepix s-a născut când un hipopotam a protejat doi iepuri înfricoșați de întuneric. Copacul Luminii le-a unit într-un trup comun. El este un spirit al bucuriei și al rezistenței, aducând speranță celor mici.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'Giraffe', 'Bunny', 'Bunny', 'Girafonix', 'Girafonix a apărut atunci când o girafă a îmbrățișat doi iepuri. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al compasiunii și al jocului, aducând veselie în ramurile lumilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'Bunny', 'Hippo', 'Bunny', 'Iepopoturel', 'Iepopoturel s-a născut atunci când un iepure a cerut prietenia unui hipopotam, iar Copacul Luminii le-a unit destinele. Cu brațe agile și cap jucăuș, dar trup solid, el este un spirit al echilibrului între fragilitate și putere, aducând veselie și protecție în ramurile lumilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('fc1b57f3-4751-51f5-a497-174648433b88', 'Bunny', 'Giraffe', 'Bunny', 'Iepurafonix', 'Iepurafonix s-a ivit atunci când doi iepuri și-au dorit să vadă lumea de sus. Copacul Luminii a răspuns și le-a oferit trupul unei girafe, dar i-a păstrat inimile și chipurile jucăușe. El este un spirit al viselor și al speranței, purtând lumina copilăriei în ochii săi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('c8386f26-e510-5ddf-90f5-fb5378222b78', 'Giraffe', 'Hippo', 'Giraffe', 'Girafopotrel', 'Girafopotrel a apărut atunci când două girafe au vegheat un hipopotam rătăcit sub lumina stelelor. Copacul Luminii le-a unit pentru a păstra echilibrul între cer și apă. El este un spirit al protecției și al clarității, aducând viziune și putere blândă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('ce884695-d11a-52bb-b6fe-f6e892075bab', 'Giraffe', 'Bunny', 'Giraffe', 'Girafiepix', 'Girafiepix s-a născut atunci când două girafe au protejat un iepure mic. Copacul Luminii i-a unit într-un singur trup, dăruindu-le agilitatea pământului și viziunea cerului. El este un spirit al prieteniei și al înțelepciunii, capabil să unească lumi opuse.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('8e05b69d-9b31-5702-8ec6-b479f6a923db', 'Hippo', 'Bunny', 'Hippo', 'Hipopoturel', 'Hipopoturel a apărut atunci când doi hipopotami au dorit să protejeze un iepure fragil. Copacul Luminii i-a unit într-un trup comun, mic dar puternic. El este un spirit al protecției și al curajului, aducând echilibru între joacă și forță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    ('5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'Hippo', 'Giraffe', 'Hippo', 'Hipogafon', 'Hipogafon s-a născut atunci când doi hipopotami și o girafă și-au împărțit un vis comun: să unească apele și cerul. Copacul Luminii i-a unit într-un singur trup. El este un spirit al stabilității și al înțelepciunii, aducând claritate în mijlocul furtunii.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
COMMIT;
