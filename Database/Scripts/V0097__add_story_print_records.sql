-- Audit table: records when a user prints a story (client-side PDF generation).
-- No blob, no job; same permission rules as PDF export.

CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryPrintRecords"
(
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL,
    "StoryId" VARCHAR(255) NOT NULL,
    "PrintedAtUtc" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "LanguageCode" VARCHAR(10) NOT NULL DEFAULT 'ro-ro',
    "IsDraft" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE INDEX IF NOT EXISTS "IX_StoryPrintRecords_UserId" ON alchimalia_schema."StoryPrintRecords" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_StoryPrintRecords_StoryId" ON alchimalia_schema."StoryPrintRecords" ("StoryId");
CREATE INDEX IF NOT EXISTS "IX_StoryPrintRecords_PrintedAtUtc" ON alchimalia_schema."StoryPrintRecords" ("PrintedAtUtc");

COMMENT ON TABLE alchimalia_schema."StoryPrintRecords" IS 'Audit: user printed a story (client-side PDF); same permissions as document export';
