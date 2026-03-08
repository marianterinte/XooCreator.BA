-- UserGeneratedLoiAnimal: LOI generative animals (image + mini-story) per user. One record per generation; 1 credit consumed.
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.tables
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'UserGeneratedLoiAnimals'
  ) THEN
    CREATE TABLE alchimalia_schema."UserGeneratedLoiAnimals" (
      "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
      "UserId" uuid NOT NULL,
      "ImageBlobPath" text NOT NULL,
      "StoryText" text NOT NULL,
      "PartsSnapshotJson" text,
      "Name" text NOT NULL DEFAULT 'Creature',
      "CreatedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
      CONSTRAINT "PK_UserGeneratedLoiAnimals" PRIMARY KEY ("Id"),
      CONSTRAINT "FK_UserGeneratedLoiAnimals_Users" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE
    );
    CREATE INDEX "IX_UserGeneratedLoiAnimals_UserId" ON alchimalia_schema."UserGeneratedLoiAnimals" ("UserId");
    CREATE INDEX "IX_UserGeneratedLoiAnimals_CreatedAtUtc" ON alchimalia_schema."UserGeneratedLoiAnimals" ("CreatedAtUtc" DESC);
    COMMENT ON TABLE alchimalia_schema."UserGeneratedLoiAnimals" IS 'LOI generative mode: one row per generated animal (image + story); 1 credit per generation';
  END IF;
END $$;
