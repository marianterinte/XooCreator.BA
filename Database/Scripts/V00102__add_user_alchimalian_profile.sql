-- User Alchimalian Profile: selected hero for Alchimalian Hero page and profile picture.
-- Separate table so profile data is not always loaded with user.

BEGIN;

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."UserAlchimalianProfiles" (
    "UserId" uuid NOT NULL,
    "SelectedHeroId" character varying(100),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_UserAlchimalianProfiles" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_UserAlchimalianProfiles_AlchimaliaUsers_UserId"
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_UserAlchimalianProfiles_UserId"
    ON "alchimalia_schema"."UserAlchimalianProfiles" ("UserId");

COMMIT;
