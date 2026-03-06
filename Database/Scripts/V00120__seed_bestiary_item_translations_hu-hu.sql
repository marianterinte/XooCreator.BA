-- Auto-generated from Data/SeedData/Discovery/i18n/hu-hu/discover-bestiary.json
-- Locale: hu-hu
-- Run date: 2026-03-03T09:03:26.678Z
-- This script seeds BestiaryItemTranslations for hu-hu bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('385e69a3-0382-4a9f-82fa-77fc58f15fd6', '5d1b4419-c890-5462-bcb7-242dbe123890', 'hu-hu', 'Nyuszi-nyuszi-zsiráf', 'A Nyuszi-nyuszi-zsiráf akkor jelent meg, amikor két nyuszi együtt álmodott arról, hogy elérjék az eget egy zsiráf szemein keresztül. A Fény Fája egyetlen lénybe egyesítette őket. Kíváncsiság és bátorság szelleme, világok között ugrálva, fényt hozva az árnyékokba.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('feb58895-2a49-4786-b66d-c54c9292ab93', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'hu-hu', 'Nyuszi-nyuszi-vizi', 'A Nyuszi-nyuszi-vizi két nyuszi álmából született, akik egy víziló erejét kérték, hogy megvédjék barátaikat. A Fény Fája egyesítette lelküket. Játékos, de védelmező szellem, bátorságot hozva a kicsiknek és békét a tévelygőknek.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('788495f5-4da4-4913-b33f-7f096d2ef361', 'fc1b57f3-4751-51f5-a497-174648433b88', 'hu-hu', 'Nyuszi-zsiráf-nyuszi', 'A Nyuszi-zsiráf-nyuszi akkor jelent meg, amikor két nyuszi azt kívánta, hogy felülről lássa a világot. A Fény Fája válaszolt és egy zsiráf testét adta nekik, de megtartotta játékos szívüket és arcukat. Álom és remény szelleme, gyermekkori fényt hordozva a szemében.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('65b7e91f-64fd-423d-a9da-6fb76a11ef99', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'hu-hu', 'Nyuszi-zsiráf-zsiráf', 'A Nyuszi-zsiráf-zsiráf akkor jelent meg, amikor egy nyuszi meg akarta osztani játékát két zsiráffal. A Fény Fája egyetlen lénybe egyesítette őket. Kíváncsiság és barátság szelleme, egyesítve a fürgeséget a nagysággal.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('9947641e-5fd2-44eb-803b-4a72b9d765c4', 'b67d8c08-04ec-539d-8787-27e838807428', 'hu-hu', 'Nyuszi-zsiráf-vizi', 'A Nyuszi-zsiráf-vizi egy legendás hős, aki egy lehetetlen szövetségből született: egy nyuszi, egy zsiráf és egy víziló, akik úgy döntöttek, hogy egyesítik sorsukat. A Fény Fája látta bátorságukat és egyetlen szívet adott nekik. A nyuszi karjaival segít és véd. A zsiráf testével messzire lát. A víziló fejével békét és bölcsességet lehel. A különböző világok közötti sokszínűség és barátság szimbóluma.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1fa9fcef-2aa9-4d18-92b0-0cc3c0997dce', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'hu-hu', 'Nyuszi-zsiráf', 'A Fény Fája ágai között született a Nyuszi-zsiráf, egy egyedi lény, félig nyuszi, félig zsiráf. Rövid, fürge karjai alagutakat ásnak és élő tüzeket gyújtanak, míg magas, csillogó foltokkal díszített teste messzire lát a horizontokon, ahol mások nem érhetnek el.

A legenda szerint a Nyuszi-zsiráf akkor jelent meg, amikor egy kis, bátor nyuszi egy öreg zsiráf gyökereihez bújt, miközben a sötétség felfalta a szavannát. Kétségbeesett ölelésükből a Fény Fája két lelket szőtt egyetlen testbe.

A Nyuszi-zsiráf a horizontok őre. Amikor a fűvön szalad, a föld örömtől ugrál. Amikor harcra kel, az árnyékok rémülten menekülnek. Igaz ereje nem csak a sebességben vagy magasságban rejlik, hanem a barátságban, amit azoknak ajánl, akik a világok ágai között kóborolnak.

A régi lelkek azt mondják, a Nyuszi-zsiráf mindig ott lesz, ahol valaki megpróbálja helyreállítani azt, ami eltört, bátorságot és reményt hozva.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a2d9cc3a-6727-41e1-96de-5f1283a92961', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'hu-hu', 'Nyuszi-vizi-nyuszi', 'A Nyuszi-vizi-nyuszi akkor született, amikor egy nyuszi egy víziló barátságát kereste, és a Fény Fája egyesítette sorsukat. Fürge karokkal és játékos fejjel, de szilárd testtel, ez a törékenység és erő közötti egyensúly szelleme, vidámságot és védelmet hozva a világok ágaihoz.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('06f0134e-4779-4723-8448-d15dee0a3238', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'hu-hu', 'Nyuszi-vizi-zsiráf', 'A Nyuszi-vizi-zsiráf egy fényviharban született, amikor három barát — egy nyuszi, egy víziló és egy zsiráf — megpróbálta helyreállítani a Fény Fája egy törött ágát. Áldozatuk mágiává változott, és így született ez az egyedi lény. A Nyuszi-vizi-zsiráf egy szelíd őrző, magában hordozva a barátság reményét és bátorságát.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('19b1628d-b2bd-47e8-881a-f217dae73b12', '558b9240-3c3a-5017-a85a-706a397288fd', 'hu-hu', 'Nyuszi-vizi-vizi', 'A Nyuszi-vizi-vizi akkor született, amikor egy nyuszi örömet hozott két vízilónak. A Fény Fája egyesítette őket egy védelmező és vidám lénybe. Barátság és bátorság szelleme, fényt hozva a sötétségbe.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('31195a60-a72a-4cc7-8735-654967590bab', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'hu-hu', 'Nyuszi-vizi', 'A Nyuszi-vizi akkor született, amikor egy nyuszi és egy víziló találkozott egy fekete víz vihar közepén. Váratlan barátságukból a Fény Fája összefonva a nyuszi fürgeségét a víziló erejével. A Nyuszi-vizi szelíd és védelmező: rövid karjaival rejtekhelyeket ás, erős testével pedig utakat nyit a vizeken keresztül. Békét hoz oda, ahol félelem van, és bizalmi hidakat épít a világok között.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8f62f142-2f22-4af7-8ea2-67da4aa84f81', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'hu-hu', 'Nyuszi-zsiráf', 'A Nyuszi-zsiráf akkor jött létre, amikor egy nyuszi és egy zsiráf együtt próbálta megvilágítani a Fény Fája egy lehullott ágát. A mágia egyetlen testbe egyesítette őket. A Nyuszi-zsiráf bátorság és barátság szelleme, képes reményt és fényt hozni a sötétségbe.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('21b72c07-0fdb-45a8-99c5-cc18cedda4ad', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'hu-hu', 'Nyuszi-vizi', 'A Nyuszi-vizi akkor jelent meg, amikor egy nyuszi és egy víziló együtt állt az éjszaka vihara alatt. A Fény Fája egyetlen testbe szőtte őket, megőrizve a nyuszi fürgeségét és a víziló bölcsességét. A Nyuszi-vizi a kicsik és nagyok közötti szolidaritás és barátság szimbóluma.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('cd297b51-5b2f-405f-b457-170ff31454f3', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'hu-hu', 'Zsiráf-nyuszi-nyuszi', 'A Zsiráf-nyuszi-nyuszi akkor jelent meg, amikor egy zsiráf ölelte át két nyuszit. A Fény Fája egyetlen lénybe egyesítette őket. Együttérzés és játék szelleme, vidámságot hozva a világok ágaihoz.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('559112c0-8504-455e-8d11-b93c2033c87e', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'hu-hu', 'Zsiráf-nyuszi-zsiráf', 'A Zsiráf-nyuszi-zsiráf akkor született, amikor két zsiráf megvédett egy kis nyuszit. A Fény Fája egyetlen testbe egyesítette őket, megadva nekik a föld fürgeségét és az ég látását. Barátság és bölcsesség szelleme, képes ellentétes világokat egyesíteni.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1e051765-d051-4641-a068-ebb85bdfd350', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'hu-hu', 'Zsiráf-nyuszi-vizi', 'A Zsiráf-nyuszi-vizi akkor jelent meg, amikor három lény esküt tett, hogy együtt védik meg a fa ágait. Barátságukból a Fény Fája egy közös testet hozott létre. A Zsiráf-nyuszi-vizi egység és barátság szelleme, magában hordozva a különböző világok egyesítésének reményét.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('daffa2ea-4797-4fcd-95e4-d8ef6527ed4d', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'hu-hu', 'Zsiráf-nyuszi', 'A Zsiráf-nyuszi akkor született, amikor egy zsiráf karjai ölelték meg egy kis nyuszit, aki az éjszakában remegett. Attól a pillanattól kezdve a Fény Fája egyesítette őket egy szokatlan hibridbe. A Zsiráf-nyuszi játékos és rettenthetetlen: hosszú karjaival csillagokat gyűjt, nyuszi testével pedig árnyékokon át ugrik. Öröm szelleme, nevetést és fényt hozva bárhová megy.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('99c5f1ce-1b67-4eca-9545-b3f32133cdcf', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'hu-hu', 'Zsiráf-zsiráf-nyuszi', 'A Zsiráf-zsiráf-nyuszi akkor jelent meg, amikor két zsiráf meg akarta osztani magasságát egy kis nyuszival. A Fény Fája egyesítette őket egy barátság és öröm lényévé. A Zsiráf-zsiráf-nyuszi reményt hoz a kicsiknek, megmutatva, hogy a nagyság megosztható.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('857bc32e-a15d-43a5-9bee-4e9ad4257c95', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'hu-hu', 'Kettős-zsiráf-vizi', 'A Kettős-zsiráf-vizi akkor született, amikor két zsiráf szellem együtt őrizte egy elveszett vízilót. A Fény Fája egyesítette őket egyetlen lénybe, magas és erős, mégis szelíd. A Kettős-zsiráf-vizi a horizontok őrzője, egyensúlyt hozva az erő és a látás között.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('15f9d37a-cd8b-48ed-88fb-11d24f9f2085', 'eb68fa55-42df-56d7-9169-70148069d13c', 'hu-hu', 'Zsiráf-vizi-nyuszi', 'A Zsiráf-vizi-nyuszi egy meteorzápor közepén született, amikor egy zsiráf, egy víziló és egy nyuszi esküt tett, hogy megvédik a Fény Fáját. Esküjükből az ágak három szellemet egyesítettek egyetlen testbe. Teljesen különböző lények közötti barátság szimbóluma. A zsiráf karjaival fényt emel az égbe; a víziló testével védi a földet; a nyuszi fejével mosolyokat és reményt hoz. A Zsiráf-vizi-nyuszi az egyensúly őrzőjeként ismert.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('584eb97e-1caf-41b2-a44b-5a36904ef41d', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'hu-hu', 'Zsiráf-vizi-zsiráf', 'A Zsiráf-vizi-zsiráf akkor jelent meg, amikor két zsiráf egy elveszett vízilót őrzött a csillagfény alatt. A Fény Fája egyesítette őket, hogy fenntartsa az ég és víz közötti egyensúlyt. Védelmezés és tisztaság szelleme, látást és szelíd erőt hozva.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('74222623-5ceb-415e-ac7c-8681b5929ff2', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'hu-hu', 'Zsiráf-vizi-vizi', 'A Zsiráf-vizi-vizi akkor jelent meg, amikor egy zsiráf esküt tett, hogy egy vihar közepén két vízilót őriz. A Fény Fája egyesítette őket egyetlen testbe. Védelmezés és barátság szelleme, magában hordozva a vizek erejét és az ég látását.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('00ddafef-39d3-48ad-9e01-2d967aa6e20c', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'hu-hu', 'Zsiráf-vizi', 'A Zsiráf-vizi egy lény, amely egy zsiráf álmából született, aki a csillagokat nézte, és egy víziló énekéből, amely a vizek alatt visszhangzott. A Fény Fája egyesítette őket egyetlen testbe, hogy egyensúlyt hozzon: a zsiráf magasságát és a víziló stabilitását. A fény szavannáit és a mély vizeket őrzi, igyekszik egyesíteni az eget a földdel. A Zsiráf-vizi közvetítő a világok között, bölcsességet és szelíd erőt hozva.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('338252c9-b8ce-49e8-9169-baf514fbd6db', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'hu-hu', 'Zsiráf-nyuszi', 'A Zsiráf-nyuszi egy nyuszi nevetéséből jelent meg, aki egy zsiráf nyakához kapaszkodott. A Fény Fája megtalálta örömüket és közös testet adott nekik. A Zsiráf-nyuszi játékos és vidám, de nagy szíve is van, készen állva a fény védelmére. Gyakran optimizmus szellemeként írják le.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('54131c86-5a9e-44ae-8d21-28b23cacc8d5', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'hu-hu', 'Zsiráf-vizi', 'A Zsiráf-vizi egy egyezségből született egy víziló és egy zsiráf között, akik együtt őrizték a Fény Fája egy lehullott ágát. A Fa egyesítette erőiket és egyetlen testet adott nekik. A Zsiráf-vizi az erő és szelídség közötti egyensúly szelleme, magában hordozva mindkét világ bölcsességét.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('ce4d7f8d-50ba-4060-8cb6-ac7fb1562a85', '880c0786-0e60-51d0-be8f-0822a13d439a', 'hu-hu', 'Vizi-nyuszi-nyuszi', 'A Vizi-nyuszi-nyuszi akkor született, amikor egy víziló megvédett két nyuszit, akiket a sötétség megijesztett. A Fény Fája egyesítette őket egy közös testbe. Öröm és ellenállóképesség szelleme, reményt hozva a kicsiknek.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('47a53a77-b0c9-4b0c-9182-e8d0affab0a3', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'hu-hu', 'Vizi-nyuszi-zsiráf', 'A Vizi-nyuszi-zsiráf ellentétek lénye, amely akkor született, amikor három világ összeütközött a Fény Fájában. A vízilótól erőt kapott; a nyuszitól fürgeséget; a zsiráftól látást. A fa ágai között kóborol, keresi a szélsőségek közötti harmóniát. A Vizi-nyuszi-zsiráf gyakran úgy írják le, mint egy szellemet, amely megmutatja, hogy az igazi erő a különbözők egyesítéséből származik.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('766328f8-7464-47c1-b421-f45709b836f8', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'hu-hu', 'Vizi-nyuszi-vizi', 'A Vizi-nyuszi-vizi akkor jelent meg, amikor két víziló meg akart védni egy törékeny nyuszit. A Fény Fája egyesítette őket egy közös testbe, kicsi, de erős. Védelmezés és bátorság szelleme, egyensúlyt hozva a játék és az erő között.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('89e254ef-45cf-4aa5-92d6-0a3d77caa05c', '3de3b3e0-5241-5350-b01b-28361c99c363', 'hu-hu', 'Vizi-nyuszi', 'A Vizi-nyuszi egy játékos álomból született, amelyben egy nyuszi egy víziló erejét kérte. A Fény Fája egyesítette őket egy kiegyensúlyozott lénybe: kicsi, de erős, szelíd, de eltökélt. A Vizi-nyuszi a víziló karjait használja a védelmezéshez, a nyuszi testét pedig az akadályokon való ugráshoz. Bátorságot hoz a kicsiknek, és megmutatja nekik, hogy a méret nem határozza meg egy lélek erejét.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e50d5c70-5822-4d09-8425-a4b8eda1ca42', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'hu-hu', 'Vizi-zsiráf-nyuszi', 'A Vizi-zsiráf-nyuszi abban a pillanatban született, amikor egy víziló, egy zsiráf és egy nyuszi együtt nézett fel a Fa fényére. Szellemük új lénybe egyesült. A Vizi-zsiráf-nyuszi együttműködés és egyensúly szelleme, békét és bölcsességet hozva a világok közötti küzdelmekben.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('9b5f41f9-7680-41c2-9a36-4284d0233023', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'hu-hu', 'Vizi-zsiráf-zsiráf', 'A Vizi-zsiráf-zsiráf akkor jelent meg, amikor egy víziló és két zsiráf együtt őrizte a fa egy ágát. Barátságukból ez az egyedi lény született, egyensúly és bölcsesség szelleme.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('130964de-ed76-44f4-a573-81068e9b4e8a', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'hu-hu', 'Vizi-zsiráf-vizi', 'A Vizi-zsiráf-vizi akkor született, amikor két víziló és egy zsiráf megosztotta egy közös álmot: hogy egyesítsék a vizeket és az eget. A Fény Fája egyesítette őket egyetlen testbe. Stabilitás és bölcsesség szelleme, tisztaságot hozva a vihar közepén.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2a3eb002-a427-4a46-a807-d5629234a51c', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'hu-hu', 'Vizi-zsiráf', 'A Vizi-zsiráf akkor született, amikor egy víziló esküt tett, hogy megvéd egy elveszett zsiráfot a sötétségtől. A Fény Fája egyesítette őket egyetlen lénybe, hogy megtartsa ígéretüket. Erős és bölcs, hidakat épít az ég és a föld között. A Vizi-zsiráf az erő és a kegyesség közötti egyensúlyt hozó szellemként ismert.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('836ed3ac-8e95-4ae3-be2d-ac2cdcc8a74d', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'hu-hu', 'Vizi-vizi-nyuszi', 'A Vizi-vizi-nyuszi akkor jelent meg, amikor két víziló esküt tett, hogy megvéd egy nyuszit. A Fény Fája egyesítette őket egyetlen testbe. Szelídség és erő szelleme, békét hozva a világok között.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('370cebdf-f47c-4ee5-b503-d5d9ce98bb25', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'hu-hu', 'Vizi-vizi-zsiráf', 'A Vizi-vizi-zsiráf akkor született, amikor két víziló egy zsiráf bölcsességét kereste. A Fény Fája egyesítette őket egy stabilitás és tisztaság szellemébe. Az ágakat őrzi, egyensúlyt hozva a víz és az ég között.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('80c3ad34-e0b2-4c6a-8d3c-f69f017b3e40', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'hu-hu', 'Vizi-nyuszi', 'A Vizi-nyuszi akkor jelent meg, amikor egy nyuszi egy víziló védelmét kérte, hogy túlélje a sötétséget. A Fény Fája egyesítette lelküket és életet adott ennek a játékos, mégis erős lénynek. A Vizi-nyuszi egy vidám védelmező, reményt hozva oda, ahol félelem van.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c8ec74c6-08e7-455c-915c-cb5c13eb5f02', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'hu-hu', 'Vizi-zsiráf', 'A Vizi-zsiráf akkor született, amikor egy víziló és egy zsiráf együtt őrizte a szavannát. A Fény Fája egyesítette őket és közös szívet adott nekik. A Vizi-zsiráf a figyelmesség szelleme, egyensúlyt hozva az erő és a tisztaság között. A csillagos éjszakák védelmezője.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4ac12e49-4873-401b-bf53-64a515c6cae4', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'hu-hu', 'Nyuszi-zsiráf', 'A Nyuszi-zsiráf akkor jelent meg, amikor egy nyuszi azt kívánta, hogy őrizze egy szavannát. A Fény Fája meghallotta kívánságát és egyesítette egy zsiráf szellemével. A Nyuszi-zsiráf felelősség és öröm szelleme, magában hordozva a védelmezés vágyát.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c3026cf1-8b71-44dc-838f-77c80eaa7e3f', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'hu-hu', 'Nyuszi-vizi', 'A Nyuszi-vizi akkor született, amikor egy nyuszi arról álmodott, hogy egy víziló erejével rendelkezik. A Fény Fája az álmot valósággá változtatta, egyesítve két ellentétes világot. A Nyuszi-vizi szelíd és békés, de rejtett ereje van, amely felébred, hogy megvédje a fényt. A teljesült álmok szimbóluma.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8be8f615-b19f-422a-a69f-7e96bdb5f3e6', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'hu-hu', 'Zsiráf-nyuszi', 'A Zsiráf-nyuszi akkor született, amikor egy nyuszi egy zsiráf vállára mászott, hogy az eget nézze. A Fény Fája látta örömüket és egyesítette őket egy közös testbe. A Zsiráf-nyuszi egy játékos és barátságos szellem, vidámságot és reményt hozva mindenhová.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7b357903-9072-419c-ade0-464ff95aa832', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'hu-hu', 'Zsiráf-vizi', 'A Zsiráf-vizi akkor jelent meg, amikor egy víziló egy zsiráf magasságát kereste, hogy őrizze a szavannát. A Fény Fája egyesítette őket és közös testet adott nekik. A Zsiráf-vizi a stabilitás szelleme, fenntartva az egyensúlyt a vizek és az ég között. A horizontok védelmezője.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2bf6ab22-8d8c-4629-9d87-c183c722ee5f', '07084a31-3e3a-5b86-8536-56d2311359bd', 'hu-hu', 'Vizi-nyuszi', 'A Vizi-nyuszi akkor jelent meg, amikor egy nyuszi és egy víziló együtt énekelt egy csillagokkal megvilágított tó mellett. A Fény Fája éneküket és nevetésüket egy közös testbe szőtte. A Vizi-nyuszi a vidámság és harmónia szelleme, reményt hozva azoknak, akik kóborolnak.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('74ac3f55-d44e-47ca-9d40-5cc0b57c4ea3', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'hu-hu', 'Vizi-zsiráf', 'A Vizi-zsiráf akkor született, amikor egy víziló arról álmodott, hogy a vizek horizontján túl lát. A Fény Fája egyesítette egy zsiráf szellemével és közös testet adott nekik. A Vizi-zsiráf a látás és kitartás szelleme, egyesítve a vizek stabilitását az ég magasságával.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
