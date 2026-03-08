-- Bundle of exclusive story/epic IDs per Supporter Pack plan (07). Configurable from admin.
CREATE TABLE IF NOT EXISTS alchimalia_schema."SupporterPackPlanExclusives"
(
    "PlanId" varchar(50) PRIMARY KEY,
    "ExclusiveStoryIdsJson" varchar(4000),
    "ExclusiveEpicIdsJson" varchar(4000)
);

COMMENT ON TABLE alchimalia_schema."SupporterPackPlanExclusives" IS 'Exclusive content bundle per plan (Bronze/Silver/Gold/Platinum); JSON arrays of story/epic IDs';

-- Seed empty bundles so admin can edit (idempotent: insert only if missing)
INSERT INTO alchimalia_schema."SupporterPackPlanExclusives" ("PlanId", "ExclusiveStoryIdsJson", "ExclusiveEpicIdsJson")
VALUES
    ('Bronze', '[]', '[]'),
    ('Silver', '[]', '[]'),
    ('Gold', '[]', '[]'),
    ('Platinum', '[]', '[]')
ON CONFLICT ("PlanId") DO NOTHING;
