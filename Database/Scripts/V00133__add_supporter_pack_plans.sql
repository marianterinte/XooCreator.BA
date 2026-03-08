-- Configurable Supporter Pack plan definitions (price, credits, quota, etc.).
-- Seed with values from 05-PACK-TIERS.

CREATE TABLE IF NOT EXISTS alchimalia_schema."SupporterPackPlans"
(
    "PlanId" varchar(50) PRIMARY KEY,
    "PriceRon" decimal(18,2) NOT NULL,
    "GenerativeCredits" int NOT NULL DEFAULT 0,
    "PrintQuota" int NOT NULL DEFAULT 0,
    "ExclusiveStoryAccessCount" int NOT NULL DEFAULT 0,
    "ExclusiveEpicAccessCount" int NOT NULL DEFAULT 0,
    "FullStoryGenerationCount" int NOT NULL DEFAULT 0,
    "IsActive" boolean NOT NULL DEFAULT true,
    "UpdatedAtUtc" timestamptz NOT NULL DEFAULT now()
);

COMMENT ON TABLE alchimalia_schema."SupporterPackPlans" IS 'Configurable plan definitions for Supporter Packs (Bronze, Silver, Gold, Platinum)';

INSERT INTO alchimalia_schema."SupporterPackPlans"
    ("PlanId", "PriceRon", "GenerativeCredits", "PrintQuota", "ExclusiveStoryAccessCount", "ExclusiveEpicAccessCount", "FullStoryGenerationCount", "IsActive", "UpdatedAtUtc")
VALUES
    ('Bronze', 30, 5, 10, 1, 0, 0, true, now()),
    ('Silver', 50, 10, 20, 2, 1, 0, true, now()),
    ('Gold', 100, 30, 50, 4, 1, 0, true, now()),
    ('Platinum', 250, 30, 50, 4, 2, 3, true, now())
ON CONFLICT ("PlanId") DO NOTHING;
