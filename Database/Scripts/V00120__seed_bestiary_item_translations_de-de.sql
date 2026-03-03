-- Auto-generated from Data/SeedData/Discovery/i18n/de-de/discover-bestiary.json
-- Locale: de-de
-- Run date: 2026-03-03T09:03:26.683Z
-- This script seeds BestiaryItemTranslations for de-de bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('75dde661-1910-4eb7-a871-14adc822b53d', '5d1b4419-c890-5462-bcb7-242dbe123890', 'de-de', 'Hasengiraffix', 'Der Hasengiraffix erschien, als zwei Hasen gemeinsam davon traeumten, den Himmel durch die Augen einer Giraffe zu erreichen. Der Baum des Lichts vereinte sie zu einem einzigen Wesen. Er ist ein Geist der Neugier und des Mutes, der zwischen Welten springt und Licht in die Schatten bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4ee8d532-07ee-48d0-bca5-94c2fde19a04', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'de-de', 'Hasen-Nilpferdix', 'Der Hasen-Nilpferdix wurde aus dem Traum zweier Hasen geboren, die sich die Staerke eines Nilpferds erbaten, um ihre Freunde zu schuetzen. Der Baum des Lichts vereinte ihre Seelen. Er ist ein verspielter, aber beschuetzender Geist, der den Kleinen Mut und den Verlorenen Frieden bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5d3caaee-e0d4-45a8-b370-64febdab4c19', 'fc1b57f3-4751-51f5-a497-174648433b88', 'de-de', 'Hasengiraffennix', 'Der Hasengiraffennix entstand, als zwei Hasen sich wuenschten, die Welt von oben zu sehen. Der Baum des Lichts antwortete und schenkte ihnen den Koerper einer Giraffe, bewahrte aber ihre verspielten Herzen. Er ist ein Geist der Traeume und Hoffnung, der das Licht der Kindheit in seinen Augen traegt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('45722696-d45d-4ac1-b8a0-535cf14b71e6', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'de-de', 'Hase-Doppelgiraffe', 'Die Hase-Doppelgiraffe erschien, als ein Hase sein Spiel mit zwei Giraffen teilen wollte. Der Baum des Lichts vereinte sie zu einem einzigen Wesen. Sie ist ein Geist der Neugier und Freundschaft, der Lebhaftigkeit mit Groesse vereint.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('acb91559-8116-4a23-ac10-ff452c1712cf', 'b67d8c08-04ec-539d-8787-27e838807428', 'de-de', 'Hase-Giraffe-Nilpferd', 'Das Hase-Giraffe-Nilpferd ist ein legendaerer Held, geboren aus einer unmoeglichen Allianz: ein Hase, eine Giraffe und ein Nilpferd, die ihre Schicksale vereinten. Der Baum des Lichts sah ihren Mut und schenkte ihnen ein einziges Herz. Mit den Armen des Hasen hilft und schuetzt es. Mit dem Koerper der Giraffe sieht es weit. Mit dem Kopf des Nilpferds inspiriert es Ruhe und Weisheit. Es ist das Symbol der Vielfalt und Freundschaft zwischen verschiedenen Welten.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1416229b-eb69-40e8-a818-f4f9fa211c34', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'de-de', 'Hasengiraffe', 'In den Aesten des Baumes des Lichts wurde die Hasengiraffe geboren, eine einzigartige Kreatur, halb Hase, halb Giraffe. Ihre kurzen, flinken Arme graben Tunnel und entfachen lebhafte Feuer, waehrend ihr grosser Koerper, verziert mit leuchtenden Flecken, weit in Horizonte blickt, die andere nicht erreichen koennen.

Der Legende nach erschien die Hasengiraffe, als sich ein kleiner, tapferer Hase an den Wurzeln einer alten Giraffe versteckte, waehrend die Dunkelheit die Savanne verschlang. Aus ihrer verzweifelten Umarmung webte der Baum des Lichts zwei Geister in einen einzigen Koerper.

Die Hasengiraffe ist die Hueterin der Horizonte. Wenn sie durch das Gras laeuft, springt die Erde vor Freude. Wenn sie aufsteht zu kaempfen, fliehen die Schatten beschaemt. Doch ihre Staerke liegt nicht nur in Geschwindigkeit oder Groesse, sondern in der Freundschaft, die sie jenen anbietet, die zwischen den Aesten der Welten umherirren.

Alte Geister sagen, die Hasengiraffe wird immer dort sein, wo jemand versucht zu reparieren, was zerbrochen ist, und Mut und Hoffnung bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d3af8bca-6842-40cc-ad37-fc83599d2e6e', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'de-de', 'Hasen-Nilpferd-Hasenling', 'Der Hasen-Nilpferd-Hasenling wurde geboren, als ein Hase die Freundschaft eines Nilpferds suchte, und der Baum des Lichts ihre Schicksale vereinte. Mit flinken Armen und verspielem Kopf, aber solidem Koerper, ist er ein Geist des Gleichgewichts zwischen Zerbrechlichkeit und Kraft, der Heiterkeit und Schutz in die Welten bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f6af3e3a-95ca-45d5-adf6-dc60dbac0846', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'de-de', 'Hase-Nilpferd-Giraffe', 'Das Hase-Nilpferd-Giraffe wurde inmitten eines Lichtsturms geboren, als drei Freunde einen zerbrochenen Ast des Baumes des Lichts zu reparieren versuchten. Ihr Opfer verwandelte sich in Magie, und so entstand dieses einzigartige Wesen. Das Hase-Nilpferd-Giraffe ist ein sanfter Beschuetzer, der die Hoffnung und den Mut der Freundschaft in sich traegt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('deb80f98-db7a-405b-9a2a-3d2a1151fa51', '558b9240-3c3a-5017-a85a-706a397288fd', 'de-de', 'Hase-Doppelnilpferd', 'Das Hase-Doppelnilpferd wurde geboren, als ein Hase zwei Nilpferden Freude brachte. Der Baum des Lichts vereinte sie zu einem schuetzenden und froehlichen Wesen. Es ist ein Geist der Freundschaft und des Mutes, der Licht in die Dunkelheit bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f7c6d9d5-cd5c-4e9d-87c9-c943003d0d3e', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'de-de', 'Hasennilpferd', 'Das Hasennilpferd wurde geboren, als ein Hase und ein Nilpferd inmitten eines Sturms dunkler Wasser aufeinandertrafen. Aus ihrer unerwarteten Freundschaft webte der Baum des Lichts die Agilitat des Hasen mit der Staerke des Nilpferds zusammen. Das Hasennilpferd ist sanft und beschuetzend: Mit seinen flinken Armen grabt es Verstecke, und mit seinem maechtigen Koerper oeffnet es Wege durch die Gewaesser. Es bringt Ruhe dorthin, wo Angst herrscht, und baut Bruecken des Vertrauens zwischen den Welten.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5b456310-5d23-4ed8-a2f0-e68e81e6b286', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'de-de', 'Hasengiraff', 'Der Hasengiraff wurde erschaffen, als ein Hase und eine Giraffe gemeinsam versuchten, einen gefallenen Ast des Baumes des Lichts zu beleuchten. Die Magie vereinte sie zu einem einzigen Koerper. Der Hasengiraff ist ein Geist des Mutes und der Freundschaft, faehig, Hoffnung und Licht in die Dunkelheit zu bringen.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('566046c5-0e65-439e-aa4f-debfb4612967', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'de-de', 'Hasenpotam', 'Der Hasenpotam tauchte auf, als ein Hase und ein Nilpferd gemeinsam dem Sturm der Nacht trotzten. Der Baum des Lichts webte sie zu einem einzigen Koerper zusammen, bewahrte die Agilitat des Hasen und die Weisheit des Nilpferds. Der Hasenpotam ist ein Symbol der Solidaritaet und Freundschaft zwischen dem Kleinen und dem Grossen.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('94558ff0-2e5a-41f5-8b80-7477bb767e09', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'de-de', 'Giraffe-Doppelhase', 'Die Giraffe-Doppelhase erschien, als eine Giraffe zwei Hasen umarmte. Der Baum des Lichts vereinte sie zu einem einzigen Wesen. Sie ist ein Geist des Mitgefuehls und des Spiels, der Froehlichkeit zwischen den Aesten der Welten verbreitet.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('34a4db3f-5b24-4672-ba4c-f21b0f7542af', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'de-de', 'Giraffenpix', 'Der Giraffenpix wurde geboren, als zwei Giraffen einen kleinen Hasen beschuetzten. Der Baum des Lichts vereinte sie zu einem einzigen Koerper und bestimmte sie mit der Agilitat der Erde und der Vision des Himmels. Er ist ein Geist der Freundschaft und Weisheit, faehig, gegensaetzliche Welten zu vereinen.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('754fe778-6df0-4a27-98ee-2175a6f1a9cd', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'de-de', 'Giraffe-Hasen-Nilpferd', 'Das Giraffe-Hasen-Nilpferd erschien, als drei Wesen schworen, gemeinsam die Aeste des Baumes zu schuetzen. Aus ihrer Freundschaft schuf der Baum des Lichts einen gemeinsamen Koerper. Das Giraffe-Hasen-Nilpferd ist ein Geist der Einheit und Freundschaft, der die Hoffnung traegt, verschiedene Welten zu vereinen.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('80b98140-3189-4efd-bf52-e31bf907e472', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'de-de', 'Giraffenhase', 'Der Giraffenhase wurde geboren, als die Arme einer Giraffe einen kleinen, zitternden Hasen in der Nacht umarmten. Von diesem Moment an vereinte der Baum des Lichts sie zu einem ungewoehnlichen Mischwesen. Der Giraffenhase ist verspielt und furchtlos: Er nutzt seine langen Arme, um Sterne zu sammeln, und den Koerper des Hasen, um durch Schatten zu springen. Er ist ein Geist der Freude, der Lachen und Licht ueberallhin bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('efada017-e71c-4d34-a42a-bea3383f7592', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'de-de', 'Giraffennix', 'Der Giraffennix entstand, als zwei Giraffen ihre Hoehe mit einem kleinen Hasen teilen wollten. Der Baum des Lichts vereinte sie zu einem Wesen der Freundschaft und Freude. Der Giraffennix bringt den Kleinen Hoffnung und zeigt, dass Groesse geteilt werden kann.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6ee67192-38ca-4c2f-a857-8daf43e39324', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'de-de', 'Doppelgiraffe', 'Die Doppelgiraffe wurde geboren, als zwei Giraffen-Geister gemeinsam ueber ein verlorenes Nilpferd wachten. Der Baum des Lichts vereinte sie zu einem einzigen Wesen, gross und stark, aber sanft. Die Doppelgiraffe ist die Hueterin der Horizonte, die Gleichgewicht zwischen Staerke und Vision bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('b3486c4b-6cc2-4402-827b-421f351bf075', 'eb68fa55-42df-56d7-9169-70148069d13c', 'de-de', 'Giraffe-Nilpferd-Hase', 'Das Giraffe-Nilpferd-Hase wurde inmitten eines Meteoritenschauers geboren, als eine Giraffe, ein Nilpferd und ein Hase schworen, den Baum des Lichts zu beschuetzen. Aus ihrem Schwur vereinten die Aeste drei Geister in einem einzigen Koerper. Es ist das Symbol der Freundschaft zwischen voellig verschiedenen Wesen. Mit den Armen der Giraffe hebt es Licht zum Himmel; mit dem Koerper des Nilpferds schuetzt es die Erde; mit dem Kopf des Hasen bringt es Laecheln und Hoffnung.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bb762aae-856c-4764-8047-13ede59d45af', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'de-de', 'Giraffe-Nilpferd-Giraffon', 'Das Giraffe-Nilpferd-Giraffon erschien, als zwei Giraffen ueber ein verlorenes Nilpferd im Sternenlicht wachten. Der Baum des Lichts vereinte sie, um das Gleichgewicht zwischen Himmel und Wasser zu bewahren. Es ist ein Geist des Schutzes und der Klarheit, der Vision und sanfte Kraft bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('34fee44a-22e0-4dbd-add7-f134411f4476', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'de-de', 'Giraffe-Doppelnilpferd', 'Das Giraffe-Doppelnilpferd erschien, als eine Giraffe schwor, inmitten eines Sturms ueber zwei Nilpferde zu wachen. Der Baum des Lichts vereinte sie zu einem einzigen Koerper. Es ist ein Geist des Schutzes und der Freundschaft, der die Macht der Gewaesser und die Vision des Himmels in sich traegt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('68b3c614-d5c4-4b7a-8fc9-530cc1fa04f8', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'de-de', 'Giraffennilpferd', 'Das Giraffennilpferd ist eine Kreatur, die aus dem Traum einer Giraffe, die die Sterne betrachtete, und dem Gesang eines Nilpferds, der unter den Wassern hallte, geboren wurde. Der Baum des Lichts vereinte sie in einem einzigen Koerper, um Gleichgewicht zu bringen: die Hoehe der Giraffe und die Bestaendigkeit des Nilpferds. Es wacht ueber die Savannen des Lichts und die tiefen Gewaesser und sucht, den Himmel mit der Erde zu vereinen. Das Giraffennilpferd ist ein Vermittler zwischen den Welten, Traeger von Weisheit und sanfter Kraft.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f36abf4a-532e-4b2f-9c30-5724e45805d1', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'de-de', 'Giraffin', 'Der Giraffin entstand aus dem Lachen eines Hasen, der sich an den Hals einer Giraffe klammerte. Der Baum des Lichts fand ihre Freude und schenkte ihnen einen gemeinsamen Koerper. Der Giraffin ist verspielt und froehlich, hat aber auch ein grosses Herz und ist bereit, das Licht zu verteidigen. Er wird oft als der Geist des Optimismus beschrieben.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('ffa6b8cd-3b95-4822-b0aa-1fe4b0f28dea', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'de-de', 'Giraffennilpferdchen', 'Das Giraffennilpferdchen wurde aus einem Pakt zwischen einem Nilpferd und einer Giraffe geboren, die gemeinsam ueber einen gefallenen Ast des Baumes des Lichts wachten. Der Baum vereinte ihre Kraefte und schenkte ihnen einen gemeinsamen Koerper. Das Giraffennilpferdchen ist ein Geist des Gleichgewichts zwischen Macht und Sanftheit.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bf7c1e3c-4a7e-4fe0-8429-03a855cf2eff', '880c0786-0e60-51d0-be8f-0822a13d439a', 'de-de', 'Nilpferd-Doppelhase', 'Das Nilpferd-Doppelhase wurde geboren, als ein Nilpferd zwei von der Dunkelheit erschreckte Hasen beschuetzte. Der Baum des Lichts vereinte sie zu einem gemeinsamen Koerper. Es ist ein Geist der Freude und Widerstandsfaehigkeit, der den Kleinen Hoffnung bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5725ad03-c551-4554-9f96-156c5eedf12a', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'de-de', 'Nilpferd-Hase-Giraffe', 'Das Nilpferd-Hase-Giraffe ist eine Kreatur der Gegensaetze, geboren, als drei Welten im Baum des Lichts aufeinandertrafen. Vom Nilpferd erhielt es Macht; vom Hasen Agilitat; von der Giraffe Vision. Es durchstreift die Aeste des Baumes auf der Suche nach Harmonie zwischen den Extremen und zeigt, dass wahre Kraft aus der Vereinigung der Verschiedenheit entsteht.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e8558de8-1f40-4793-8f61-c9ed399edf77', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'de-de', 'Nilpferd-Hasen-Nilpferd', 'Das Nilpferd-Hasen-Nilpferd erschien, als zwei Nilpferde einen zerbrechlichen Hasen schuetzen wollten. Der Baum des Lichts vereinte sie zu einem gemeinsamen Koerper, klein aber stark. Es ist ein Geist des Schutzes und Mutes, der Gleichgewicht zwischen Spiel und Staerke bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('aa80363e-b0cf-4a88-8d64-ba2109587c1f', '3de3b3e0-5241-5350-b01b-28361c99c363', 'de-de', 'Nilpferdhase', 'Der Nilpferdhase wurde aus einem verspielten Traum geboren, in dem ein Hase um die Staerke eines Nilpferds bat. Der Baum des Lichts vereinte sie zu einem ausgewogenen Wesen: klein, aber stark, sanft, aber entschlossen. Der Nilpferdhase nutzt die Arme des Nilpferds zum Schutz und den Koerper des Hasen, um Hindernisse zu ueberspringen. Er bringt den Kleinen Mut und zeigt ihnen, dass die Groesse nicht die Kraft der Seele bestimmt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('076b6b75-c0cc-4190-8ad3-d3b163f5da42', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'de-de', 'Nilpferd-Giraffe-Hase', 'Das Nilpferd-Giraffe-Hase wurde in dem Moment geboren, als ein Nilpferd, eine Giraffe und ein Hase gemeinsam zum Licht des Baumes aufblickten. Ihre Geister verschmolzen zu einem neuen Wesen. Das Nilpferd-Giraffe-Hase ist ein Geist der Zusammenarbeit und Harmonie, der Frieden und Weisheit in den Kaempfen zwischen den Welten bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('425b1587-8ac4-4cce-887a-513be0b9a114', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'de-de', 'Nilpferd-Doppelgiraffe', 'Die Nilpferd-Doppelgiraffe entstand, als ein Nilpferd und zwei Giraffen gemeinsam ueber einen Ast des Baumes wachten. Aus ihrer Freundschaft entstand dieses einzigartige Wesen, ein Geist des Gleichgewichts und der Weisheit.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6e325858-2c7c-4157-b717-dfd580c1ceed', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'de-de', 'Nilpferd-Giraffe-Nilpferd', 'Das Nilpferd-Giraffe-Nilpferd wurde geboren, als zwei Nilpferde und eine Giraffe einen gemeinsamen Traum teilten: die Gewaesser und den Himmel zu vereinen. Der Baum des Lichts vereinte sie zu einem einzigen Koerper. Es ist ein Geist der Bestaendigkeit und Weisheit, der Klarheit inmitten des Sturms bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2762909e-e692-4ef0-ba2c-10a16bb0e75c', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'de-de', 'Nilpferdgiraffe', 'Die Nilpferdgiraffe wurde geboren, als ein Nilpferd schwor, eine verlorene Giraffe vor der Dunkelheit zu schuetzen. Der Baum des Lichts vereinte sie zu einem einzigen Wesen, um ihr Versprechen zu halten. Sie ist stark und weise und baut Bruecken zwischen Himmel und Erde. Die Nilpferdgiraffe ist bekannt als der Geist, der Gleichgewicht zwischen Staerke und Anmut bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('744513e8-4f6e-43f0-a534-bcd46826dde0', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'de-de', 'Doppelnilpferdhase', 'Der Doppelnilpferdhase erschien, als zwei Nilpferde schworen, einen Hasen zu beschuetzen. Der Baum des Lichts vereinte sie zu einem einzigen Koerper. Er ist ein Geist der Sanftheit und Staerke, der Frieden zwischen den Welten bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2c2bfe84-30cc-41e7-9c22-3ddc0ba53151', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'de-de', 'Doppelnilpferdgiraffe', 'Die Doppelnilpferdgiraffe wurde geboren, als zwei Nilpferde die Weisheit einer Giraffe suchten. Der Baum des Lichts vereinte sie zu einem Geist der Bestaendigkeit und Klarheit. Sie wacht ueber die Aeste und bringt Gleichgewicht zwischen Wasser und Himmel.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('b4348cc4-c067-4095-8119-8b41f37bf1fa', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'de-de', 'Nilpferdhaesin', 'Das Nilpferdhaesin erschien, als ein Hase den Schutz eines Nilpferds erbat, um die Dunkelheit zu ueberleben. Der Baum des Lichts vereinte ihre Seelen und erweckte dieses verspielte, aber kraftvolle Wesen. Das Nilpferdhaesin ist ein froehlicher Beschuetzer, der Hoffnung bringt, wo Angst herrscht.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3dd8964c-4f40-4151-ac2b-4c11ec15fda8', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'de-de', 'Nilpferdiraffe', 'Die Nilpferdiraffe wurde geboren, als ein Nilpferd und eine Giraffe gemeinsam die Savanne bewachten. Der Baum des Lichts vereinte sie und schenkte ihnen ein gemeinsames Herz. Die Nilpferdiraffe ist ein Geist der Wachsamkeit, der Gleichgewicht zwischen Staerke und Klarheit bringt. Sie ist die Hueterin der Sternennaechte.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4f6a72fa-ac0f-48c4-8b20-eb1b0167e40f', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'de-de', 'Hasen-Giraffin', 'Der Hasen-Giraffin erschien, als ein Hase sich wuenschte, ueber eine Savanne zu wachen. Der Baum des Lichts erhoerte seinen Wunsch und vereinte ihn mit dem Geist einer Giraffe. Der Hasen-Giraffin ist ein Geist der Verantwortung und Freude, der in sich den Wunsch zu schuetzen traegt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0c5662a7-b91a-4293-b795-2ce7b7aea471', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'de-de', 'Hasen-Nilpferdling', 'Das Hasen-Nilpferdling wurde geboren, als ein Hase davon traeumte, die Staerke eines Nilpferds zu haben. Der Baum des Lichts verwandelte den Traum in Wirklichkeit und vereinte zwei gegensaetzliche Welten. Das Hasen-Nilpferdling ist sanft und friedlich, aber besitzt eine verborgene Staerke, die erwacht, um das Licht zu verteidigen. Es ist das Symbol der erfuellten Traeume.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e4889235-d852-440b-aad6-065703abfb2f', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'de-de', 'Giraffenhaesin', 'Das Giraffenhaesin wurde geboren, als ein Hase auf die Schultern einer Giraffe kletterte, um den Himmel zu betrachten. Der Baum des Lichts sah ihre Freude und vereinte sie zu einem gemeinsamen Koerper. Das Giraffenhaesin ist ein verspielter und freundlicher Geist, der ueberall Heiterkeit und Hoffnung verbreitet.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('41d764e3-4dc2-4eae-b084-29f39fc1566a', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'de-de', 'Giraffenpotam', 'Der Giraffenpotam entstand, als ein Nilpferd die Hoehe einer Giraffe suchte, um die Savanne zu bewachen. Der Baum des Lichts vereinte sie und schenkte ihnen einen gemeinsamen Koerper. Der Giraffenpotam ist ein Geist der Bestaendigkeit, der das Gleichgewicht zwischen den Gewaessern und dem Himmel aufrecht erhaelt. Er ist der Hueter der Horizonte.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bc3af8ab-f55a-42d6-be33-758466c020cd', '07084a31-3e3a-5b86-8536-56d2311359bd', 'de-de', 'Nilpferdriel', 'Das Nilpferdriel erschien, als ein Hase und ein Nilpferd gemeinsam an einem gestirnenbedeckten See sangen. Der Baum des Lichts webte ihr Lied und Lachen zu einem gemeinsamen Koerper. Das Nilpferdriel ist ein Geist der Froehlichkeit und Harmonie, der den Umherirrenden Hoffnung bringt.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('619ec11b-8207-401a-9613-029c7f6f054b', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'de-de', 'Nilpferdgiraffon', 'Der Nilpferdgiraffon wurde geboren, als ein Nilpferd davon traeumte, ueber den Horizont der Gewaesser hinaus zu sehen. Der Baum des Lichts vereinte es mit dem Geist einer Giraffe und schenkte ihnen einen gemeinsamen Koerper. Der Nilpferdgiraffon ist ein Geist der Vision und Ausdauer, der die Bestaendigkeit der Gewaesser mit der Hoehe des Himmels vereint.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
