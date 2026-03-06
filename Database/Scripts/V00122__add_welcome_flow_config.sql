-- Welcome Flow config: single row table, seed with current appsettings values (fallback when table empty).
-- Idempotent: CREATE TABLE IF NOT EXISTS; INSERT ON CONFLICT DO UPDATE.

CREATE TABLE IF NOT EXISTS alchimalia_schema."WelcomeFlowConfig" (
    "Id" integer NOT NULL,
    "EntryPointStoryId" character varying(128) NOT NULL DEFAULT '',
    "KindergartenGirl" character varying(128) NOT NULL DEFAULT '',
    "KindergartenBoy" character varying(128) NOT NULL DEFAULT '',
    "PrimaryGirl" character varying(128) NOT NULL DEFAULT '',
    "PrimaryBoy" character varying(128) NOT NULL DEFAULT '',
    "OlderGirl" character varying(128) NOT NULL DEFAULT '',
    "OlderBoy" character varying(128) NOT NULL DEFAULT '',
    CONSTRAINT "PK_WelcomeFlowConfig" PRIMARY KEY ("Id")
);

INSERT INTO alchimalia_schema."WelcomeFlowConfig" (
    "Id",
    "EntryPointStoryId",
    "KindergartenGirl",
    "KindergartenBoy",
    "PrimaryGirl",
    "PrimaryBoy",
    "OlderGirl",
    "OlderBoy"
) VALUES (
    1,
    'marianterinte-s260304024556',
    'ionelbacosca-s260130175902',
    'ionelbacosca-s251212185030',
    'nicoletacirdei-s260113133330',
    'marianterinte-s260228163555',
    'nicoletacirdei-s260113133330',
    'marianterinte-s260228163555'
) ON CONFLICT ("Id") DO NOTHING;
