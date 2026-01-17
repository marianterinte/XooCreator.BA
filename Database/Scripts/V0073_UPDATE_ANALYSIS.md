# Analiză Actualizare V0073 - Alchimalia Universe Craft/Definition

**Data:** 2025-01-28  
**Status:** ✅ Analiză Completă

---

## Ce s-a implementat deja în Backend

### ✅ Entități C# - Versioning Fields Complete

**HeroDefinitionCraft:**
- ✅ `BaseVersion` (int)
- ✅ `LastDraftVersion` (int)
- ✅ `AssignedReviewerUserId` (Guid?)
- ✅ `ReviewStartedAt` (DateTime?)
- ✅ `ReviewEndedAt` (DateTime?)
- ✅ `ApprovedByUserId` (Guid?)

**HeroDefinitionDefinition:**
- ✅ `Version` (int)
- ✅ `BaseVersion` (int)
- ✅ `LastPublishedVersion` (int)

**AnimalCraft:**
- ✅ `BaseVersion` (int)
- ✅ `LastDraftVersion` (int)
- ✅ `AssignedReviewerUserId` (Guid?)
- ✅ `ReviewStartedAt` (DateTime?)
- ✅ `ReviewEndedAt` (DateTime?)
- ✅ `ApprovedByUserId` (Guid?)

**AnimalDefinition:**
- ✅ `Version` (int)
- ✅ `BaseVersion` (int)
- ✅ `LastPublishedVersion` (int)

### ✅ Change Log Entities

- ✅ `HeroDefinitionPublishChangeLog` - există în C#
- ✅ `AnimalPublishChangeLog` - există în C#
- ✅ `XooDbContext` are DbSet-uri pentru ambele

### ✅ Version Job Entities

- ✅ `HeroDefinitionVersionJob` - există în C#
- ✅ `AnimalVersionJob` - există în C#
- ✅ `XooDbContext` are DbSet-uri pentru ambele

---

## Ce LIPSEȘTE în Scriptul V0073

### ❌ Versioning Fields în Tabele

**HeroDefinitionCrafts:**
- ❌ `BaseVersion` (int, default 0)
- ❌ `LastDraftVersion` (int, default 0)
- ❌ `AssignedReviewerUserId` (uuid, nullable)
- ❌ `ReviewStartedAt` (timestamp, nullable)
- ❌ `ReviewEndedAt` (timestamp, nullable)
- ❌ `ApprovedByUserId` (uuid, nullable)

**HeroDefinitionDefinitions:**
- ❌ `Version` (int, default 1)
- ❌ `BaseVersion` (int, default 0)
- ❌ `LastPublishedVersion` (int, default 0)

**AnimalCrafts:**
- ❌ `BaseVersion` (int, default 0)
- ❌ `LastDraftVersion` (int, default 0)
- ❌ `AssignedReviewerUserId` (uuid, nullable)
- ❌ `ReviewStartedAt` (timestamp, nullable)
- ❌ `ReviewEndedAt` (timestamp, nullable)
- ❌ `ApprovedByUserId` (uuid, nullable)

**AnimalDefinitions:**
- ❌ `Version` (int, default 1)
- ❌ `BaseVersion` (int, default 0)
- ❌ `LastPublishedVersion` (int, default 0)

### ❌ Change Log Tables

- ❌ `HeroDefinitionPublishChangeLogs` - tabel complet
- ❌ `AnimalPublishChangeLogs` - tabel complet

### ❌ Version Job Tables

- ❌ `HeroDefinitionVersionJobs` - tabel complet
- ❌ `AnimalVersionJobs` - tabel complet

---

## Plan de Actualizare V0073

### Opțiunea 1: Modificare Directă V0073 (Recomandat)

**Avantaje:**
- ✅ Un singur script pentru toate modificările
- ✅ Consistență completă
- ✅ Nu trebuie deploy în prod/dev (utilizator confirmat)

**Pași:**
1. Adăugare câmpuri versioning în CREATE TABLE pentru Crafts
2. Adăugare câmpuri versioning în CREATE TABLE pentru Definitions
3. Adăugare CREATE TABLE pentru change logs
4. Adăugare CREATE TABLE pentru version jobs
5. Adăugare foreign keys și indexes

### Opțiunea 2: Script Nou V0079

**Avantaje:**
- ✅ Păstrează istoricul
- ✅ Mai ușor de rollback

**Dezavantaje:**
- ❌ Mai multe scripturi de gestionat
- ❌ Riscul de inconsistență

---

## Recomandare

**Modificare directă V0073** - pentru că:
- Nu există deploy în prod/dev
- Un singur script este mai ușor de gestionat
- Consistență completă într-un singur loc

---

## Structură Tabele Necesare

### HeroDefinitionPublishChangeLogs

```sql
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionPublishChangeLogs" (
    "Id" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "EntityType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "ChangeType" character varying(32) NOT NULL,
    "Hash" character varying(128),
    "PayloadJson" jsonb,
    "AssetDraftPath" character varying(1024),
    "AssetPublishedPath" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreatedBy" uuid,
    CONSTRAINT "PK_HeroDefinitionPublishChangeLogs" PRIMARY KEY ("Id")
);
```

### AnimalPublishChangeLogs

```sql
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalPublishChangeLogs" (
    "Id" uuid NOT NULL,
    "AnimalId" uuid NOT NULL,
    "DraftVersion" integer NOT NULL,
    "LanguageCode" character varying(10) NOT NULL,
    "EntityType" character varying(32) NOT NULL,
    "EntityId" character varying(200),
    "ChangeType" character varying(32) NOT NULL,
    "Hash" character varying(128),
    "PayloadJson" jsonb,
    "AssetDraftPath" character varying(1024),
    "AssetPublishedPath" character varying(1024),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreatedBy" uuid,
    CONSTRAINT "PK_AnimalPublishChangeLogs" PRIMARY KEY ("Id")
);
```

### HeroDefinitionVersionJobs

```sql
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."HeroDefinitionVersionJobs" (
    "Id" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_HeroDefinitionVersionJobs" PRIMARY KEY ("Id")
);
```

### AnimalVersionJobs

```sql
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."AnimalVersionJobs" (
    "Id" uuid NOT NULL,
    "AnimalId" uuid NOT NULL,
    "OwnerUserId" uuid NOT NULL,
    "RequestedByEmail" character varying(256),
    "BaseVersion" integer NOT NULL,
    "Status" character varying(32) NOT NULL,
    "DequeueCount" integer NOT NULL DEFAULT 0,
    "QueuedAtUtc" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "StartedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(2000),
    CONSTRAINT "PK_AnimalVersionJobs" PRIMARY KEY ("Id")
);
```

---

**Ultima actualizare:** 2025-01-28  
**Status:** ✅ Analiză Completă
