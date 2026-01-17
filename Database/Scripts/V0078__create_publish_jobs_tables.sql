CREATE TABLE alchimalia_schema."HeroPublishJobs" (
    "Id" uuid NOT NULL,
    "HeroId" text NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" text NOT NULL,
    "LangTag" text NOT NULL DEFAULT 'ro-ro',
    "ForceFull" boolean NOT NULL DEFAULT FALSE,
    "Status" text NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone NULL,
    "CompletedAtUtc" timestamp with time zone NULL,
    "ErrorMessage" text NULL,
    CONSTRAINT "PK_HeroPublishJobs" PRIMARY KEY ("Id")
);

CREATE TABLE alchimalia_schema."AnimalPublishJobs" (
    "Id" uuid NOT NULL,
    "AnimalId" text NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" text NOT NULL,
    "LangTag" text NOT NULL DEFAULT 'ro-ro',
    "ForceFull" boolean NOT NULL DEFAULT FALSE,
    "Status" text NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone NULL,
    "CompletedAtUtc" timestamp with time zone NULL,
    "ErrorMessage" text NULL,
    CONSTRAINT "PK_AnimalPublishJobs" PRIMARY KEY ("Id")
);
