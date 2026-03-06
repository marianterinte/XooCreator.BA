-- Add BodyPartTranslations for es-es, fr-fr, de-de, it-it
-- Run date: 2026-01-20

BEGIN;

-- es-es
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'head', 'es-es', 'Cabeza') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'body', 'es-es', 'Cuerpo') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'arms', 'es-es', 'Brazos') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'legs', 'es-es', 'Piernas') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'tail', 'es-es', 'Cola') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'wings', 'es-es', 'Alas') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horn', 'es-es', 'Cuerno') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horns', 'es-es', 'Cuernos') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;

-- fr-fr
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'head', 'fr-fr', 'Tête') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'body', 'fr-fr', 'Corps') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'arms', 'fr-fr', 'Bras') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'legs', 'fr-fr', 'Jambes') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'tail', 'fr-fr', 'Queue') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'wings', 'fr-fr', 'Ailes') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horn', 'fr-fr', 'Corne') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horns', 'fr-fr', 'Cornes') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;

-- de-de
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'head', 'de-de', 'Kopf') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'body', 'de-de', 'Körper') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'arms', 'de-de', 'Arme') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'legs', 'de-de', 'Beine') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'tail', 'de-de', 'Schwanz') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'wings', 'de-de', 'Flügel') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horn', 'de-de', 'Horn') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horns', 'de-de', 'Hörner') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;

-- it-it
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'head', 'it-it', 'Testa') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'body', 'it-it', 'Corpo') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'arms', 'it-it', 'Braccia') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'legs', 'it-it', 'Gambe') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'tail', 'it-it', 'Coda') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'wings', 'it-it', 'Ali') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horn', 'it-it', 'Corno') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;
INSERT INTO "alchimalia_schema"."BodyPartTranslations" ("Id", "BodyPartKey", "LanguageCode", "Name") VALUES (gen_random_uuid(), 'horns', 'it-it', 'Corna') ON CONFLICT ("BodyPartKey", "LanguageCode") DO NOTHING;

COMMIT;
