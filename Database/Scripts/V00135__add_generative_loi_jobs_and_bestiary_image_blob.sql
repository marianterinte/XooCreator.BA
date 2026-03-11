-- GenerativeLoiJobs: async job for LOI generative (image + text, 1 credit)
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.tables
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'GenerativeLoiJobs'
  ) THEN
    CREATE TABLE alchimalia_schema."GenerativeLoiJobs" (
      "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
      "UserId" uuid NOT NULL,
      "Locale" character varying(16) NOT NULL,
      "CombinationJson" character varying(512) NOT NULL,
      "Status" character varying(32) NOT NULL DEFAULT 'Queued',
      "ProgressMessage" character varying(512),
      "ErrorMessage" character varying(2000),
      "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
      "StartedAtUtc" timestamp with time zone,
      "CompletedAtUtc" timestamp with time zone,
      "BestiaryItemId" uuid,
      "ResultName" character varying(128),
      "ResultImageUrl" character varying(1024),
      "ResultStoryText" text,
      CONSTRAINT "PK_GenerativeLoiJobs" PRIMARY KEY ("Id"),
      CONSTRAINT "FK_GenerativeLoiJobs_Users" FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE
    );
    CREATE INDEX "IX_GenerativeLoiJobs_UserId" ON alchimalia_schema."GenerativeLoiJobs" ("UserId");
    CREATE INDEX "IX_GenerativeLoiJobs_Status" ON alchimalia_schema."GenerativeLoiJobs" ("Status");
    CREATE INDEX "IX_GenerativeLoiJobs_QueuedAtUtc" ON alchimalia_schema."GenerativeLoiJobs" ("QueuedAtUtc" DESC);
  END IF;
END $$;

-- BestiaryItems: optional ImageBlobPath for generative type
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema = 'alchimalia_schema' AND table_name = 'BestiaryItems' AND column_name = 'ImageBlobPath'
  ) THEN
    ALTER TABLE alchimalia_schema."BestiaryItems" ADD COLUMN "ImageBlobPath" character varying(512) NULL;
  END IF;
END $$;
