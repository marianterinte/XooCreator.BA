-- Auto-generated from Data/SeedData/Discovery/i18n/ro-ro/discover-bestiary.json
-- Run date: 2025-11-27 07:42:25+02:00

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_extension
        WHERE extname = 'uuid-ossp'
    ) THEN
        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    END IF;
END $$;

BEGIN;
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Giraffe|—'), 'Bunny', 'Giraffe', '—', 'Iepurafa', 'În ramurile Copacului Luminii s-a născut Iepurafa, o creatură unică, jumătate iepure, jumătate girafă. Brațele sale scurte și sprintene sapă tuneluri și aprind focuri vii, iar trupul său înalt, împodobit cu pete strălucitoare, vede departe în zări unde alții nu pot ajunge.

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
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Hippo|—'), 'Bunny', 'Hippo', '—', 'Iepopotam', 'Iepopotam s-a născut atunci când un iepure și un hipopotam s-au întâlnit în mijlocul unei furtuni de ape negre. Din prietenia lor neașteptată, Copacul Luminii a împletit agilitatea iepurelui cu forța hipopotamului. Iepopotamul este blând și protector: cu brațele sale sprintene sapă ascunzișuri, iar cu trupul său puternic deschide drumuri prin ape. El aduce liniște acolo unde e teamă și ridică poduri de încredere între lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Hippo|—'), 'Giraffe', 'Hippo', '—', 'Girafopotam', 'Girafopotam este o creatură născută din visul unei girafe care privea spre stele și din cântecul unui hipopotam care răsuna sub ape. Copacul Luminii i-a unit într-un singur trup pentru a da echilibru: înălțimea girafei și stabilitatea hipopotamului. El veghează peste savanele de lumină și apele adânci, căutând să unească cerul cu pământul. Girafopotamul este un mediator între lumi, aducând înțelepciune și putere blândă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Giraffe|Hippo'), 'Bunny', 'Giraffe', 'Hippo', 'Iepurafopotam', 'Iepurafopotam este un erou legendar, născut dintr-o alianță imposibilă: un iepure, o girafă și un hipopotam care au ales să-și unească destinele. Copacul Luminii a văzut curajul lor și le-a dat o singură inimă. Cu brațele iepurelui, el ajută și protejează. Cu trupul girafei, vede departe. Cu capul hipopotamului, inspiră liniște și înțelepciune. El este un simbol al diversității și al prieteniei dintre lumi diferite.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Bunny|—'), 'Giraffe', 'Bunny', '—', 'Girafon', 'Girafon s-a născut atunci când brațele unei girafe au îmbrățișat un iepure mic care tremura în noapte. Din acea clipă, Copacul Luminii i-a unit într-un hibrid neobișnuit. Girafon este jucăuș și neînfricat: folosește brațele lungi pentru a aduna stele și trupul de iepure pentru a sări printre umbre. El este un spirit al bucuriei, aducând râsete și lumină oriunde ajunge.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Hippo|Bunny'), 'Giraffe', 'Hippo', 'Bunny', 'Girafopotaniu', 'Girafopotaniu s-a născut în mijlocul unei furtuni de stele, atunci când o girafă, un hipopotam și un iepure au jurat să apere Copacul Luminii. Din jurământul lor, ramurile au unit trei spirite într-un singur trup. El este un simbol al prieteniei între ființe complet diferite. Cu brațele girafei ridică lumina spre cer, cu trupul hipopotamului protejează pământul, iar cu capul iepurelui aduce zâmbete și speranță. Girafopotaniu este văzut ca un gardian al echilibrului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Bunny|—'), 'Hippo', 'Bunny', '—', 'Hipurilă', 'Hipurilă s-a născut dintr-un vis jucăuș în care un iepure a cerut puterea unui hipopotam. Copacul Luminii i-a unit într-o ființă echilibrată: mică dar puternică, blândă dar hotărâtă. Hipurilă folosește brațele hipopotamului pentru a proteja și trupul de iepure pentru a sări printre obstacole. El aduce curaj celor mici și le arată că mărimea nu definește puterea unui suflet.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Giraffe|—'), 'Hippo', 'Giraffe', '—', 'Hipogira', 'Hipogira s-a născut atunci când un hipopotam a jurat să apere o girafă rătăcită de întuneric. Copacul Luminii i-a unit într-o singură ființă pentru a păstra promisiunea lor. Ea este puternică și înțeleaptă, ridicând poduri între cer și pământ. Hipogira este cunoscută drept spiritul care aduce echilibru între forță și grație.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Bunny|Giraffe'), 'Hippo', 'Bunny', 'Giraffe', 'Hipurafon', 'Hipurafon este o creatură a contrastelor, născută atunci când trei lumi s-au ciocnit în Copacul Luminii. Din hipopotam a primit puterea, din iepure agilitatea și din girafă viziunea. El cutreieră ramurile copacului căutând armonia între extreme. Hipurafon este adesea descris ca un spirit care arată că adevărata putere vine din unirea celor diferiți.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Hippo|Giraffe'), 'Bunny', 'Hippo', 'Giraffe', 'Iepopotira', 'Iepopotira s-a născut în mijlocul unei furtuni de lumină, atunci când trei prieteni — un iepure, un hipopotam și o girafă — au încercat să repare o ramură ruptă a Copacului Luminii. Sacrificiul lor a fost transformat în magie, și astfel s-a născut această creatură unică. Iepopotira este un paznic blând, care poartă în el speranța și curajul prieteniei.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|—|Hippo'), 'Giraffe', '—', 'Hippo', 'Girafopotir', 'Girafopotir s-a născut dintr-un legământ între un hipopotam și o girafă care au vegheat împreună asupra unei ramuri căzute a Copacului Luminii. Copacul le-a unit forțele și le-a oferit un singur trup. Girafopotir este un spirit al echilibrului între putere și blândețe, purtând în el înțelepciunea celor două lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|—|Hippo'), 'Bunny', '—', 'Hippo', 'Iepurotam', 'Iepurotam s-a ivit atunci când un iepure și un hipopotam au stat împreună sub furtuna nopții. Copacul Luminii i-a împletit într-un singur trup, păstrând agilitatea iepurelui și înțelepciunea hipopotamului. Iepurotam este un simbol al solidarității și al prieteniei dintre cei mici și cei mari.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|—|Bunny'), 'Hippo', '—', 'Bunny', 'Hiponel', 'Hiponel a apărut atunci când un iepure a cerut protecția unui hipopotam pentru a supraviețui întunericului. Copacul Luminii le-a unit sufletele și a dat viață acestei ființe jucăușe, dar puternice. Hiponel este un protector vesel, aducând speranță acolo unde e teamă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Bunny|Hippo'), '—', 'Bunny', 'Hippo', 'Iepuripos', 'Iepuripos s-a născut atunci când un iepure a visat să aibă puterea unui hipopotam. Copacul Luminii a transformat visul în realitate, unind două lumi opuse. Iepuripos este blând și pașnic, dar are o forță ascunsă care se trezește pentru a apăra lumina. El este un simbol al visurilor împlinite.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Giraffe|Hippo'), '—', 'Giraffe', 'Hippo', 'Girafotam', 'Girafotam s-a ivit atunci când un hipopotam a căutat înălțimea unei girafe pentru a veghea peste savană. Copacul Luminii i-a unit și le-a oferit un trup comun. Girafotam este un spirit al stabilității, păstrând echilibrul între ape și cer. El este un protector al orizonturilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Hippo|Giraffe'), '—', 'Hippo', 'Giraffe', 'Hipogirafon', 'Hipogirafon s-a născut atunci când un hipopotam a visat să privească mai departe decât orizontul apelor. Copacul Luminii i-a unit cu spiritul unei girafe și i-a dat trup comun. Hipogirafon este un spirit al viziunii și al rezistenței, unind stabilitatea apelor cu înălțimea cerului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|—|Giraffe'), 'Bunny', '—', 'Giraffe', 'Iepurafon', 'Iepurafon a fost creat atunci când un iepure și o girafă au încercat împreună să lumineze o ramură căzută a Copacului Luminii. Magia i-a unit într-un singur trup. Iepurafon este un spirit al curajului și al prieteniei, capabil să aducă speranță și lumină în întuneric.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|—|Bunny'), 'Giraffe', '—', 'Bunny', 'Girafurel', 'Girafurel s-a ivit din râsul unui iepure care s-a agățat de gâtul unei girafe. Copacul Luminii a găsit bucuria lor și le-a dat un trup comun. Girafurel este jucăuș și vesel, dar are și o inimă mare, gata să apere lumina. El este adesea descris ca un spirit al optimismului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|—|Giraffe'), 'Hippo', '—', 'Giraffe', 'Hipofiraf', 'Hipofiraf s-a născut când un hipopotam și o girafă au vegheat împreună peste savană. Copacul Luminii i-a unit și le-a oferit o inimă comună. Hipofiraf este un spirit al vigilenței, aducând echilibru între forță și claritate. El este un protector al nopților înstelate.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Hippo|Bunny'), '—', 'Hippo', 'Bunny', 'Hiponeluș', 'Hiponeluș a apărut atunci când un iepure a adormit pe spatele unui hipopotam sub razele Copacului Luminii. Magia i-a unit și a dat naștere unei creaturi pașnice și curajoase. Hiponeluș este cunoscut ca un spirit al prieteniei și al jocului, aducând zâmbete oriunde ajunge.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Bunny|Giraffe'), '—', 'Bunny', 'Giraffe', 'Iepurafira', 'Iepurafira s-a născut atunci când un iepure a cerut ajutorul unei girafe pentru a veghea peste o poiană. Copacul Luminii le-a unit sufletele și a creat această ființă unică. Iepurafira este un spirit al vigilenței și al compasiunii, mereu prezent pentru a aduce echilibru între fragilitate și măreție.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Hippo|Giraffe'), 'Bunny', 'Hippo', 'Giraffe', 'Iepogiraf', 'Iepogiraf este rezultatul unei prietenii între trei creaturi care au luptat împreună să salveze o ramură a Copacului Luminii. Din unitatea lor s-a născut o ființă ce poartă agilitatea iepurelui, puterea hipopotamului și viziunea girafei. Iepogiraf este un spirit al unității și al încrederii.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Giraffe|Bunny'), 'Hippo', 'Giraffe', 'Bunny', 'Hipogiriel', 'Hipogiriel s-a născut în clipa în care un hipopotam, o girafă și un iepure au ridicat împreună privirea spre lumina Copacului. Spiritele lor s-au unit într-o ființă nouă. Hipogiriel este un spirit al colaborării și al echilibrului, aducând pace și înțelepciune în luptele dintre lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Bunny|Hippo'), 'Giraffe', 'Bunny', 'Hippo', 'Girafopam', 'Girafopam a apărut atunci când trei creaturi au jurat să apere împreună ramurile copacului. Din prietenia lor, Copacul Luminii a creat un trup comun. Girafopam este un spirit al unității și al prieteniei, purtând în el speranța de a uni lumi diferite.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Hippo|Giraffe'), '—', 'Hippo', 'Giraffe', 'Hiporaf', 'Hiporaf s-a născut atunci când un hipopotam a cerut înălțimea girafei pentru a veghea mai departe. Copacul Luminii a răspuns unindu-i într-o ființă unică. Hiporaf este un spirit al vigilenței și al rezistenței, aducând protecție și speranță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Giraffe|—'), 'Hippo', 'Giraffe', '—', 'Hipogaf', 'Hipogaf s-a născut când un hipopotam și o girafă au apărat împreună o ramură a Copacului Luminii. Din sacrificiul lor, copacul i-a unit într-un singur trup. Hipogaf este un spirit al rezistenței și clarității, capabil să protejeze și să vegheze asupra celor pierduți.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|—|Giraffe'), 'Bunny', '—', 'Giraffe', 'Iepugaf', 'Iepugaf a apărut atunci când un iepure a dorit să vadă lumea de sus prin ochii unei girafe. Copacul Luminii a unit dorința lor într-un singur trup. Iepugaf este un spirit al curiozității și al prieteniei, inspirând pe cei mici să viseze mai departe.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Giraffe|Bunny'), '—', 'Giraffe', 'Bunny', 'Girafunel', 'Girafunel s-a născut atunci când un iepure s-a urcat pe umerii unei girafe pentru a privi cerul. Copacul Luminii a văzut bucuria lor și i-a unit într-un trup comun. Girafunel este un spirit jucăuș și prietenos, aducând veselie și speranță peste tot.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Bunny|Giraffe'), 'Hippo', 'Bunny', 'Giraffe', 'Hiponaf', 'Hiponaf a apărut atunci când un hipopotam, un iepure și o girafă au legat o prietenie imposibilă. Copacul Luminii i-a unit pentru a arăta că diferențele pot deveni puteri. Hiponaf este un spirit al colaborării și al prieteniei, un protector vesel al ramurilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Hippo|Bunny'), 'Giraffe', 'Hippo', 'Bunny', 'Girafotinel', 'Girafotinel s-a născut atunci când o girafă și un hipopotam au protejat un iepure de întuneric. Copacul Luminii le-a răsplătit prietenia cu un trup comun. Girafotinel este un spirit al protecției și al bucuriei, păstrând echilibrul între forță și blândețe.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Giraffe|Hippo'), 'Bunny', 'Giraffe', 'Hippo', 'Iepogafotam', 'Iepogafotam s-a născut dintr-o alianță între un iepure curajos, o girafă înțeleaptă și un hipopotam puternic. Copacul Luminii i-a unit într-un singur trup pentru a veghea peste ramurile fragile. Iepogafotam este un spirit al unității și al speranței, aducând echilibru între viteză, înălțime și forță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Bunny|Hippo'), '—', 'Bunny', 'Hippo', 'Iepuripotam', 'Iepuripotam a apărut atunci când un iepure a cerut protecția unui hipopotam pentru a apăra poiana sa. Copacul Luminii i-a unit într-un trup comun. Iepuripotam este un spirit protector și blând, care aduce liniște celor înfricoșați.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|—|Hippo'), 'Giraffe', '—', 'Hippo', 'Girafotamiel', 'Girafotamiel s-a născut atunci când o girafă a ridicat privirea spre stele și un hipopotam a cântat în apele adânci. Copacul Luminii a unit cântecul și visul lor într-un trup comun. Girafotamiel este un spirit al armoniei și al liniștii, aducând pace între cer și apă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|—|Bunny'), 'Hippo', '—', 'Bunny', 'Hipopurel', 'Hipopurel s-a ivit atunci când un iepure și un hipopotam s-au ascuns împreună de umbre. Copacul Luminii le-a unit sufletele pentru a crea un protector jucăuș. Hipopurel este un spirit vesel, dar hotărât, gata să apere lumina.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Giraffe|Hippo'), '—', 'Giraffe', 'Hippo', 'Girafopotir', 'Girafopotir s-a născut când o girafă și un hipopotam au vegheat împreună peste ramurile fragile ale Copacului Luminii. Din prietenia lor s-a născut această ființă unică, un spirit al rezistenței și al stabilității.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Bunny|—'), 'Hippo', 'Bunny', '—', 'Hiponelus', 'Hiponelus s-a născut atunci când un hipopotam și un iepure au împărțit aceeași vizuină în mijlocul furtunii. Copacul Luminii a văzut solidaritatea lor și i-a unit într-un trup comun. Hiponelus este un spirit al prieteniei și al rezistenței, mereu prezent acolo unde umbrele se apropie.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Hippo|Bunny'), '—', 'Hippo', 'Bunny', 'Hipuriel', 'Hipuriel a apărut atunci când un iepure și un hipopotam au cântat împreună lângă un lac luminat de stele. Copacul Luminii le-a împletit cântecul și râsul într-un trup comun. Hipuriel este un spirit al veseliei și al armoniei, aducând speranță celor care rătăcesc.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Hippo|—'), 'Giraffe', 'Hippo', '—', 'Girafopot', 'Girafopot s-a născut atunci când o girafă și un hipopotam au vegheat împreună peste o ramură căzută. Copacul Luminii a văzut curajul lor și i-a unit într-un trup comun. Girafopot este un spirit al echilibrului, aducând protecție și viziune celor care au nevoie.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Giraffe|—'), 'Bunny', 'Giraffe', '—', 'Iepurafonel', 'Iepurafonel s-a născut atunci când un iepure a încercat să salveze o girafă blocată între umbre. Copacul Luminii a unit sacrificiul lor într-un trup comun. Iepurafonel este un spirit al speranței și al prieteniei, aducând curaj celor mici.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:—|Bunny|Giraffe'), '—', 'Bunny', 'Giraffe', 'Iepuragiraf', 'Iepuragiraf a apărut atunci când un iepure a dorit să vegheze asupra unei savane. Copacul Luminii i-a ascultat dorința și l-a unit cu spiritul unei girafe. Iepuragiraf este un spirit al responsabilității și al bucuriei, purtând în el dorința de a proteja.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|—|Giraffe'), 'Hippo', '—', 'Giraffe', 'Hipogiraf', 'Hipogiraf s-a născut atunci când un hipopotam a dorit să privească lumea prin ochii unei girafe. Copacul Luminii le-a unit visurile într-un trup comun. Hipogiraf este un spirit al viziunii și al stabilității, capabil să aducă echilibru între forță și înțelepciune.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|—|Bunny'), 'Giraffe', '—', 'Bunny', 'Girafiep', 'Girafiep s-a ivit atunci când un iepure și o girafă au vegheat împreună peste o ramură căzută a Copacului Luminii. Din prietenia lor a răsărit această creatură unică. Girafiep este un spirit al prieteniei și al speranței, aducând lumină acolo unde umbrele se adună.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Giraffe|Hippo'), 'Giraffe', 'Giraffe', 'Hippo', 'Girafodublu', 'Girafodublu s-a născut când două spirite de girafă au vegheat împreună un hipopotam rătăcit. Copacul Luminii le-a unit într-o ființă unică, înaltă și puternică, dar blândă. Girafodublu este un paznic al orizonturilor, aducând echilibru între forță și viziune.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Giraffe|Bunny'), 'Giraffe', 'Giraffe', 'Bunny', 'Girafurelix', 'Girafurelix s-a ivit atunci când două girafe au dorit să-și împartă înălțimea cu un iepure mic. Copacul Luminii le-a unit într-o creatură a prieteniei și a veseliei. Girafurelix aduce speranță celor mici, arătând că măreția poate fi împărtășită.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Bunny|Giraffe'), 'Bunny', 'Bunny', 'Giraffe', 'Iepurafix', 'Iepurafix a apărut când doi iepuri au visat împreună să atingă cerul prin ochii unei girafe. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al curiozității și al curajului, sărind între lumi și aducând lumină în umbre.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Bunny|Hippo'), 'Bunny', 'Bunny', 'Hippo', 'Iepuripotix', 'Iepuripotix s-a născut din visul a doi iepuri care au cerut forța unui hipopotam pentru a-și apăra prietenii. Copacul Luminii le-a unit sufletele. El este un spirit jucăuș dar protector, aducând curaj celor mici și liniște celor rătăciți.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Hippo|Bunny'), 'Hippo', 'Hippo', 'Bunny', 'Hiponelix', 'Hiponelix a apărut când doi hipopotami au jurat să protejeze un iepure. Copacul Luminii le-a unit într-un singur trup. El este un spirit al blândeții și al forței, aducând pace între lumi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Hippo|Giraffe'), 'Hippo', 'Hippo', 'Giraffe', 'Hipogafix', 'Hipogafix s-a născut atunci când doi hipopotami au căutat înțelepciunea unei girafe. Copacul Luminii i-a unit într-un spirit al stabilității și clarității. El veghează asupra ramurilor, aducând echilibru între apă și cer.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Hippo|Hippo'), 'Giraffe', 'Hippo', 'Hippo', 'Girafopotix', 'Girafopotix a apărut când o girafă a jurat să vegheze doi hipopotami în mijlocul unei furtuni. Copacul Luminii le-a unit într-un singur trup. El este un spirit al protecției și al prieteniei, purtând în el puterea apelor și viziunea cerului.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Hippo|Hippo'), 'Bunny', 'Hippo', 'Hippo', 'Iepopotix', 'Iepopotix s-a născut când un iepure a adus bucurie a doi hipopotami. Copacul Luminii le-a unit într-o ființă protectoare și veselă. El este un spirit al prieteniei și al curajului, aducând lumină în întuneric.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Giraffe|Giraffe'), 'Bunny', 'Giraffe', 'Giraffe', 'Iepurogaf', 'Iepurogaf a apărut când un iepure a dorit să împărtășească jocul său cu două girafe. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al curiozității și al prieteniei, unind agerimea cu măreția.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Giraffe|Giraffe'), 'Hippo', 'Giraffe', 'Giraffe', 'Hipogirafox', 'Hipogirafox s-a ivit atunci când un hipopotam și două girafe au vegheat împreună o ramură a copacului. Din prietenia lor s-a născut această ființă unică, un spirit al echilibrului și al înțelepciunii.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Bunny|Bunny'), 'Hippo', 'Bunny', 'Bunny', 'Hiponepix', 'Hiponepix s-a născut când un hipopotam a protejat doi iepuri înfricoșați de întuneric. Copacul Luminii le-a unit într-un trup comun. El este un spirit al bucuriei și al rezistenței, aducând speranță celor mici.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Bunny|Bunny'), 'Giraffe', 'Bunny', 'Bunny', 'Girafonix', 'Girafonix a apărut atunci când o girafă a îmbrățișat doi iepuri. Copacul Luminii le-a unit într-o singură ființă. El este un spirit al compasiunii și al jocului, aducând veselie în ramurile lumilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Hippo|Bunny'), 'Bunny', 'Hippo', 'Bunny', 'Iepopoturel', 'Iepopoturel s-a născut atunci când un iepure a cerut prietenia unui hipopotam, iar Copacul Luminii le-a unit destinele. Cu brațe agile și cap jucăuș, dar trup solid, el este un spirit al echilibrului între fragilitate și putere, aducând veselie și protecție în ramurile lumilor.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Bunny|Giraffe|Bunny'), 'Bunny', 'Giraffe', 'Bunny', 'Iepurafonix', 'Iepurafonix s-a ivit atunci când doi iepuri și-au dorit să vadă lumea de sus. Copacul Luminii a răspuns și le-a oferit trupul unei girafe, dar i-a păstrat inimile și chipurile jucăușe. El este un spirit al viselor și al speranței, purtând lumina copilăriei în ochii săi.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Hippo|Giraffe'), 'Giraffe', 'Hippo', 'Giraffe', 'Girafopotrel', 'Girafopotrel a apărut atunci când două girafe au vegheat un hipopotam rătăcit sub lumina stelelor. Copacul Luminii le-a unit pentru a păstra echilibrul între cer și apă. El este un spirit al protecției și al clarității, aducând viziune și putere blândă.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Giraffe|Bunny|Giraffe'), 'Giraffe', 'Bunny', 'Giraffe', 'Girafiepix', 'Girafiepix s-a născut atunci când două girafe au protejat un iepure mic. Copacul Luminii i-a unit într-un singur trup, dăruindu-le agilitatea pământului și viziunea cerului. El este un spirit al prieteniei și al înțelepciunii, capabil să unească lumi opuse.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Bunny|Hippo'), 'Hippo', 'Bunny', 'Hippo', 'Hipopoturel', 'Hipopoturel a apărut atunci când doi hipopotami au dorit să protejeze un iepure fragil. Copacul Luminii i-a unit într-un trup comun, mic dar puternic. El este un spirit al protecției și al curajului, aducând echilibru între joacă și forță.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
INSERT INTO alchimalia_schema."BestiaryItems"
    ("Id", "ArmsKey", "BodyKey", "HeadKey", "Name", "Story", "CreatedAt")
VALUES
    (uuid_generate_v5('00000000-0000-0000-0000-000000000000', 'bestiary:Hippo|Giraffe|Hippo'), 'Hippo', 'Giraffe', 'Hippo', 'Hipogafon', 'Hipogafon s-a născut atunci când doi hipopotami și o girafă și-au împărțit un vis comun: să unească apele și cerul. Copacul Luminii i-a unit într-un singur trup. El este un spirit al stabilității și al înțelepciunii, aducând claritate în mijlocul furtunii.', '2025-01-01T00:00:00Z')
ON CONFLICT ("Id") DO UPDATE
SET "ArmsKey" = EXCLUDED."ArmsKey",
    "BodyKey" = EXCLUDED."BodyKey",
    "HeadKey" = EXCLUDED."HeadKey",
    "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";
COMMIT;
