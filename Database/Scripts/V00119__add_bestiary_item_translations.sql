-- Add BestiaryItemTranslations table for localized bestiary entries
-- Idempotent: safe to run multiple times

BEGIN;

CREATE TABLE IF NOT EXISTS alchimalia_schema."BestiaryItemTranslations" (
    "Id" uuid NOT NULL,
    "BestiaryItemId" uuid NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "Name" text NOT NULL,
    "Story" text NOT NULL,
    CONSTRAINT "PK_BestiaryItemTranslations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_BestiaryItemTranslations_BestiaryItems"
        FOREIGN KEY ("BestiaryItemId") REFERENCES alchimalia_schema."BestiaryItems"("Id")
            ON DELETE CASCADE,
    CONSTRAINT "UQ_BestiaryItemTranslations_Item_Lang"
        UNIQUE ("BestiaryItemId", "LanguageCode")
);

COMMIT;

