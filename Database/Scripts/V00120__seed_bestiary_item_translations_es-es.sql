-- Auto-generated from Data/SeedData/Discovery/i18n/es-es/discover-bestiary.json
-- Locale: es-es
-- Run date: 2026-03-03T09:03:26.681Z
-- This script seeds BestiaryItemTranslations for es-es bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4ee8e250-068b-450e-86a3-0ba4643f565e', '5d1b4419-c890-5462-bcb7-242dbe123890', 'es-es', 'Conejiraffix', 'El Conejiraffix aparecio cuando dos conejos sonaron juntos con alcanzar el cielo a traves de los ojos de una jirafa. El Arbol de la Luz los unio en un unico ser. Es un espiritu de curiosidad y valentia que salta entre mundos llevando luz a las sombras.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8001f9b4-a24c-4511-a548-0d6fdf2e7c2d', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'es-es', 'Conejipotix', 'El Conejipotix nacio del sueno de dos conejos que pedian la fuerza de un hipopotamo para proteger a sus amigos. El Arbol de la Luz unio sus almas. Es un espiritu jugueton pero protector que trae valentia a los pequenos y paz a los perdidos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d8c08164-94cb-4c2b-85ad-c9e388a05c85', 'fc1b57f3-4751-51f5-a497-174648433b88', 'es-es', 'Conejiraffonix', 'El Conejiraffonix emergio cuando dos conejos desearon ver el mundo desde las alturas. El Arbol de la Luz respondio dandoles el cuerpo de una jirafa, pero preservo sus corazones juguetones. Es un espiritu de suenos y esperanza que porta en sus ojos la luz de la infancia.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('998dda4b-5a33-4041-9d77-5125b5cfba8e', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'es-es', 'Conejidojirafa', 'El Conejidojirafa aparecio cuando un conejo quiso compartir su juego con dos jirafas. El Arbol de la Luz los unio en un unico ser. Es un espiritu de curiosidad y amistad que une la vivacidad con la grandeza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('76ba4ddf-72d0-401b-876f-c7e9ca501731', 'b67d8c08-04ec-539d-8787-27e838807428', 'es-es', 'Conejirafopotamo', 'El Conejirafopotamo es un heroe legendario, nacido de una alianza imposible: un conejo, una jirafa y un hipopotamo que eligieron unir sus destinos. El Arbol de la Luz vio su valentia y les dio un solo corazon. Con los brazos del conejo ayuda y protege. Con el cuerpo de la jirafa ve lejos. Con la cabeza del hipopotamo inspira tranquilidad y sabiduria. Es el simbolo de la diversidad y la amistad entre mundos distintos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a4119691-8108-4485-8514-e885d304b663', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'es-es', 'Conejirafa', 'Entre las ramas del Arbol de la Luz nacio la Conejirafa, una criatura unica, mitad conejo, mitad jirafa. Sus brazos cortos y agiles cavan tuneles y encienden fuegos vivos, mientras que su cuerpo esbelto, adornado con manchas luminosas, divisa lejanos horizontes que otros no pueden alcanzar.

La leyenda cuenta que la Conejirafa surgio cuando un pequeno conejo valiente se escondia en las raices de una vieja jirafa mientras la oscuridad devoraba la sabana. De su desesperado abrazo, el Arbol de la Luz tejio dos espiritus en un solo cuerpo.

La Conejirafa es la guardiana de los horizontes. Cuando corre por la hierba, la tierra salta de alegria. Cuando se alza a luchar, las sombras huyen avergonzadas. Su verdadera fuerza no esta solo en la velocidad o la altura, sino en la amistad que ofrece a quienes vagan entre las ramas del mundo.

Los espiritus antiguos dicen que la Conejirafa siempre estara alli donde alguien intente reparar lo que fue roto, trayendo valentia y esperanza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c31ed4f7-3b94-4e7d-aebc-aa2581c4a657', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'es-es', 'Conejipotorel', 'El Conejipotorel nacio cuando un conejo busco la amistad de un hipopotamo y el Arbol de la Luz unio sus destinos. Con brazos agiles y cabeza juguetona, pero cuerpo solido, es un espiritu del equilibrio entre la fragilidad y la fuerza que trae alegria y proteccion a los mundos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a2ffe81d-1d79-4340-8877-a2eb3473350c', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'es-es', 'Conejiporajirafa', 'El Conejiporajirafa nacio en medio de una tormenta de luz, cuando tres amigos  un conejo, un hipopotamo y una jirafa  intentaron reparar una rama rota del Arbol de la Luz. Su sacrificio se transformo en magia y asi nacio esta criatura unica. Es un guardian gentil, portando en su interior la esperanza y el coraje de la amistad.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0bdfd117-94ec-4f5e-9274-563bc3a866b2', '558b9240-3c3a-5017-a85a-706a397288fd', 'es-es', 'Conejipotixdoble', 'El Conejipotixdoble nacio cuando un conejo trajo alegria a dos hipopotamos. El Arbol de la Luz los unio en un ser protector y alegre. Es un espiritu de amistad y valentia que porta luz a la oscuridad.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e881fa19-2933-4c64-88db-9ad0cfef4e81', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'es-es', 'Conejipotamo', 'El Conejipotamo nacio cuando un conejo y un hipopotamo se encontraron en medio de una tormenta de aguas oscuras. De su inesperada amistad, el Arbol de la Luz tejio la agilidad del conejo con la fuerza del hipopotamo. El Conejipotamo es gentil y protector: con sus agiles brazos excava refugios, y con su poderoso cuerpo abre caminos a traves de las aguas. Trae calma donde reina el miedo y construye puentes de confianza entre los mundos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('b0841c3d-8f67-4922-bb44-8c4e33778fd5', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'es-es', 'Conejirafin', 'El Conejirafin fue creado cuando un conejo y una jirafa intentaron juntos iluminar una rama caida del Arbol de la Luz. La magia los unio en un solo cuerpo. Es un espiritu de valentia y amistad, capaz de traer esperanza y luz a la oscuridad.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('61058cc2-5395-4b78-b186-a37db45d2e82', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'es-es', 'Conejipotam', 'El Conejipotam emergio cuando un conejo y un hipopotamo se enfrentaron juntos a la tormenta de la noche. El Arbol de la Luz los tejio en un solo cuerpo, preservando la agilidad del conejo y la sabiduria del hipopotamo. Es un simbolo de solidaridad y amistad entre el pequeno y el grande.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('758beeab-966c-41f4-a5f8-f92338880f2c', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'es-es', 'Jiraconejos', 'El Jiraconejos aparecio cuando una jirafa abrazoa dos conejos. El Arbol de la Luz los unio en un unico ser. Es un espiritu de compasion y juego que porta alegria entre las ramas de los mundos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5ab64961-7e94-41a0-8ef9-1dc8704c4fdf', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'es-es', 'Jirafepix', 'El Jirafepix nacio cuando dos jirafas protegieron a un pequeno conejo. El Arbol de la Luz los unio en un solo cuerpo, dotandolos de la agilidad de la tierra y la vision del cielo. Es un espiritu de amistad y sabiduria capaz de unir mundos opuestos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e7be146b-dc50-4891-89ac-fc0210ea652c', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'es-es', 'Jirafopotamico', 'El Jirafopotamico aparecio cuando tres criaturas juraron proteger juntas las ramas del arbol. De su amistad, el Arbol de la Luz creo un cuerpo comun. Es un espiritu de unidad y amistad que porta en si la esperanza de unir mundos distintos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('791c33ad-ec65-4f92-b2e1-d9fd35df8b2e', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'es-es', 'Jiranejo', 'El Jiranejo nacio cuando los brazos de una jirafa abrazaron a un pequeno conejo que temblaba en la noche. A partir de ese momento, el Arbol de la Luz los unio en un hibrido insolito. El Jiranejo es jugueton y valiente: usa sus largos brazos para recoger estrellas y el cuerpo del conejo para saltar entre las sombras. Es un espiritu de alegria que lleva risas y luz a donde vaya.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c4933c76-faac-4452-a4d5-e1cb3e33d107', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'es-es', 'Jirafunix', 'El Jirafunix emergio cuando dos jirafas quisieron compartir su altura con un pequeno conejo. El Arbol de la Luz los unio en una criatura de amistad y alegria. El Jirafunix trae esperanza a los pequenos, mostrando que la grandeza puede compartirse.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e73c8771-e1bc-48b6-80b9-87a9f86d8897', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'es-es', 'Doblejirafa', 'La Doblejirafa nacio cuando dos espiritus de jirafa velaron juntos sobre un hipopotamo perdido. El Arbol de la Luz los unio en un unico ser, alto y robusto, pero gentil. La Doblejirafa es la guardiana de los horizontes que porta equilibrio entre la fuerza y la vision.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('66d26175-853d-4370-a837-10d5dc596587', 'eb68fa55-42df-56d7-9169-70148069d13c', 'es-es', 'Jirafopotin', 'El Jirafopotin nacio en medio de una lluvia de meteoros, cuando una jirafa, un hipopotamo y un conejo juraron proteger el Arbol de la Luz. De su juramento, las ramas unieron tres espiritus en un solo cuerpo. Con los brazos de la jirafa eleva la luz hacia el cielo; con el cuerpo del hipopotamo protege la tierra; con la cabeza del conejo trae sonrisas y esperanza. Es considerado el guardian del equilibrio.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6f3fca40-0c1b-4f4b-a7d4-198b4c95a6e1', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'es-es', 'Jirafopotrel', 'El Jirafopotrel aparecio cuando dos jirafas velaron sobre un hipopotamo perdido bajo la luz de las estrellas. El Arbol de la Luz los unio para mantener el equilibrio entre el cielo y el agua. Es un espiritu de proteccion y claridad que porta vision y fuerza gentil.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5b40fd61-2f9b-4b75-8904-e54f039c4565', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'es-es', 'Jirafopotix', 'El Jirafopotix aparecio cuando una jirafa juro velar sobre dos hipopotamos en medio de una tormenta. El Arbol de la Luz los unio en un solo cuerpo. Es un espiritu de proteccion y amistad que porta el poder de las aguas y la vision del cielo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3308149c-6d5d-4956-b621-e7fe33a431d5', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'es-es', 'Jirafopotamo', 'El Jirafopotamo es una criatura nacida del sueno de una jirafa que contemplaba las estrellas y del canto de un hipopotamo que resonaba bajo las aguas. El Arbol de la Luz los unio en un solo cuerpo para traer equilibrio: la altura de la jirafa y la solidez del hipopotamo. Vigila las sabanas de luz y las aguas profundas, buscando unir el cielo con la tierra. Es un mediador entre mundos, portador de sabiduria y fuerza gentil.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('acffa86e-7f23-485b-aaee-70d1f11d6ba9', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'es-es', 'Jirafin', 'El Jirafin emergio de la risa de un conejo aferrado al cuello de una jirafa. El Arbol de la Luz encontro su alegria y les dio un cuerpo comun. El Jirafin es jugueton y alegre, pero tambien tiene un gran corazon, listo para defender la luz. Es a menudo descrito como el espiritu del optimismo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('72c50814-a228-4d88-bbd8-aa4d0d69683a', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'es-es', 'Jirafopotir', 'El Jirafopotir nacio de un pacto entre un hipopotamo y una jirafa que velaron juntos sobre una rama caida del Arbol de la Luz. El Arbol unio sus fuerzas y les dio un cuerpo comun. Es un espiritu de equilibrio entre el poder y la dulzura, portando en si la sabiduria de ambos mundos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c54cdd45-3c0b-4b26-8f3f-f2f20ba04ab9', '880c0786-0e60-51d0-be8f-0822a13d439a', 'es-es', 'Hipoconejos', 'El Hipoconejos nacio cuando un hipopotamo protgio a dos conejos asustados por la oscuridad. El Arbol de la Luz los unio en un cuerpo comun. Es un espiritu de alegria y resiliencia que porta esperanza a los pequenos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('ce408d74-11aa-4513-8f4d-6fa6dbe54974', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'es-es', 'Hipojirafo', 'El Hipojirafo es una criatura de contrastes, nacida cuando tres mundos colisionaron en el Arbol de la Luz. Del hipopotamo recibio la potencia; del conejo, la agilidad; de la jirafa, la vision. Recorre las ramas del arbol en busca de armonia entre los extremos. El Hipojirafo demuestra que el verdadero poder nace de la union de las diferencias.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('270009cc-9bb1-430b-851f-0a651e72f4ac', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'es-es', 'Hipopoturel', 'El Hipopoturel aparecio cuando dos hipopotamos quisieron proteger a un fragil conejo. El Arbol de la Luz los unio en un cuerpo comun, pequeno pero robusto. Es un espiritu de proteccion y valentia que trae equilibrio entre el juego y la fuerza.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c3da35e1-b4c9-4c02-8ef0-9ab100ec40fe', '3de3b3e0-5241-5350-b01b-28361c99c363', 'es-es', 'Hipopin', 'El Hipopin nacio de un sueno jugueton en el que un conejo pedia la fuerza de un hipopotamo. El Arbol de la Luz los unio en un ser equilibrado: pequeno pero robusto, gentil pero decidido. Usa los brazos del hipopotamo para proteger y el cuerpo del conejo para saltar los obstaculos. Trae valentia a los pequenos y les muestra que el tamano no define la fuerza del alma.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('19cb267e-2152-48e6-a247-ab4cbc569e86', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'es-es', 'Hipojirapin', 'El Hipojirapin nacio en el instante en que un hipopotamo, una jirafa y un conejo alzaron juntos la vista hacia la luz del Arbol. Sus espiritus se fusionaron en un nuevo ser. Es un espiritu de colaboracion y armonia que trae paz y sabiduria en las luchas entre los mundos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('dcd33099-8bff-491b-8ff4-5231036e1504', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'es-es', 'Hipodojirafa', 'El Hipodojirafa emergio cuando un hipopotamo y dos jirafas velaron juntos sobre una rama del arbol. De su amistad nacio este ser unico, un espiritu de equilibrio y sabiduria.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('1402c7ed-ba6b-4ca7-9667-7a934f5a93ef', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'es-es', 'Hipogafon', 'El Hipogafon nacio cuando dos hipopotamos y una jirafa compartieron un sueno comun: unir las aguas y el cielo. El Arbol de la Luz los unio en un solo cuerpo. Es un espiritu de estabilidad y sabiduria que trae claridad en medio de la tormenta.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4ac66d38-a7c2-4962-8012-ebfc9b1ed20b', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'es-es', 'Hipojirafa', 'La Hipojirafa nacio cuando un hipopotamo juro proteger a una jirafa perdida de la oscuridad. El Arbol de la Luz los unio en un solo ser para cumplir su promesa. Es fuerte y sabio, construye puentes entre el cielo y la tierra. La Hipojirafa es conocida como el espiritu que trae el equilibrio entre la fuerza y la gracia.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('7b200902-53d3-42e9-89e9-a2ab1801626a', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'es-es', 'Hiponelix', 'El Hiponelix aparecio cuando dos hipopotamos juraron proteger a un conejo. El Arbol de la Luz los unio en un solo cuerpo. Es un espiritu de dulzura y fuerza que porta paz entre los mundos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('172aac93-4c29-4d5e-b46a-5b32c2910b4d', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'es-es', 'Hipogaffix', 'El Hipogaffix nacio cuando dos hipopotamos buscaron la sabiduria de una jirafa. El Arbol de la Luz los unio en un espiritu de estabilidad y claridad. Vigila las ramas portando equilibrio entre el agua y el cielo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('135ed5a0-8d69-4a24-a6ac-8300215ac2c4', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'es-es', 'Hipoel', 'El Hipoel aparecio cuando un conejo pidio la proteccion de un hipopotamo para sobrevivir a la oscuridad. El Arbol de la Luz unio sus almas y dio vida a este ser jugueton pero poderoso. El Hipoel es un protector alegre que trae esperanza donde reina el miedo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2269c517-f0e7-4aea-8247-b9c0f060c83c', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'es-es', 'Hipirafa', 'La Hipirafa nacio cuando un hipopotamo y una jirafa vigilaron juntos la sabana. El Arbol de la Luz los unio dandoles un corazon comun. La Hipirafa es un espiritu de vigilancia que porta equilibrio entre la fuerza y la claridad. Es la protectora de las noches estrelladas.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('dfed97f7-d10f-4e5f-bf58-96333144f4d2', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'es-es', 'Conejiraffina', 'La Conejiraffina aparecio cuando un conejo deseo vigilar una sabana. El Arbol de la Luz escucho su deseo y lo unio con el espiritu de una jirafa. Es un espiritu de responsabilidad y alegria que porta en si el deseo de proteger.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('58917546-fb8d-4b92-8b39-3f146834297d', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'es-es', 'Conejipos', 'El Conejipos nacio cuando un conejo sono con tener la fuerza de un hipopotamo. El Arbol de la Luz convirtio el sueno en realidad, uniendo dos mundos opuestos. El Conejipos es gentil y pacifico, pero posee una fuerza oculta que despierta para defender la luz. Es el simbolo de los suenos cumplidos.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('731cc404-c4ce-405f-b4dc-35f0a2b8de0f', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'es-es', 'Jiraconeja', 'La Jiraconeja nacio cuando un conejo subio a los hombros de una jirafa para mirar el cielo. El Arbol de la Luz vio su alegria y los unio en un cuerpo comun. La Jiraconeja es un espiritu jugueton y amistoso que lleva alegria y esperanza a donde vaya.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('26a4137f-da37-4093-a98e-b7d9bbba8bfd', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'es-es', 'Jirafotamo', 'El Jirafotamo emergio cuando un hipopotamo busco la altura de una jirafa para vigilar la sabana. El Arbol de la Luz los unio dandoles un cuerpo comun. Es un espiritu de estabilidad que mantiene el equilibrio entre las aguas y el cielo. Es el protector de los horizontes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('16c8e515-d1cb-49bd-9a3b-9dca17a645b6', '07084a31-3e3a-5b86-8536-56d2311359bd', 'es-es', 'Hipouriel', 'El Hipouriel aparecio cuando un conejo y un hipopotamo cantaron juntos a orillas de un lago estrellado. El Arbol de la Luz tejio su cancion y sus risas en un cuerpo comun. El Hipouriel es un espiritu de alegria y armonia que porta esperanza a quienes vagan.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('3ed98967-ba9e-4afc-aeac-352466059998', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'es-es', 'Hipojiraffon', 'El Hipojiraffon nacio cuando un hipopotamo sono con ver mas alla del horizonte de las aguas. El Arbol de la Luz lo unio con el espiritu de una jirafa y les dio un cuerpo comun. Es un espiritu de vision y tenacidad que une la estabilidad de las aguas con la altura del cielo.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
