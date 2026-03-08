-- Migration: Add UserPackGrants table for Supporter Packs (admin-granted or after order acceptance)
-- Grants stack; no expiry. Used for print quota sum, LOI credits, exclusive content.

CREATE TABLE IF NOT EXISTS alchimalia_schema."UserPackGrants"
(
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL REFERENCES alchimalia_schema."AlchimaliaUsers"("Id") ON DELETE CASCADE,
    "PlanId" varchar(50) NOT NULL,
    "GrantedAtUtc" timestamptz NOT NULL DEFAULT now(),
    "GrantedByUserId" uuid NOT NULL REFERENCES alchimalia_schema."AlchimaliaUsers"("Id") ON DELETE RESTRICT,
    "EmailUsed" varchar(255),
    "OrderId" uuid
);

CREATE INDEX IF NOT EXISTS "IX_UserPackGrants_UserId" ON alchimalia_schema."UserPackGrants" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_UserPackGrants_GrantedAtUtc" ON alchimalia_schema."UserPackGrants" ("GrantedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_UserPackGrants_OrderId" ON alchimalia_schema."UserPackGrants" ("OrderId");

COMMENT ON TABLE alchimalia_schema."UserPackGrants" IS 'Supporter Pack grants (admin or order acceptance); stacking, no expiry';
