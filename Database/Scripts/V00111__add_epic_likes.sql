-- Add EpicLikes table for tracking epic likes per user
-- Description: Adds EpicLikes table to track epic likes (aligned with StoryLikes).

BEGIN;

CREATE TABLE IF NOT EXISTS "alchimalia_schema"."EpicLikes"
(
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "EpicId" character varying(200) NOT NULL,
    "LikedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_EpicLikes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EpicLikes_AlchimaliaUsers_UserId"
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_EpicLikes_UserId_EpicId"
    ON "alchimalia_schema"."EpicLikes" ("UserId", "EpicId");

CREATE INDEX IF NOT EXISTS "IX_EpicLikes_EpicId"
    ON "alchimalia_schema"."EpicLikes" ("EpicId");

COMMIT;
