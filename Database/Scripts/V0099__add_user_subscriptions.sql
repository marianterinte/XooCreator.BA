-- Migration: Add UserSubscriptions table for Stripe Supporter Pack purchases
-- Used by Stripe webhook (checkout.session.completed) to grant print quota

CREATE TABLE IF NOT EXISTS alchimalia_schema."UserSubscriptions"
(
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL REFERENCES alchimalia_schema."AlchimaliaUsers"("Id") ON DELETE CASCADE,
    "PlanId" varchar(50) NOT NULL,
    "StripeSessionId" varchar(255),
    "StripeCustomerId" varchar(255),
    "StripePaymentIntentId" varchar(255),
    "PaidAtUtc" timestamptz NOT NULL,
    "ExpiresAtUtc" timestamptz,
    "CreatedAtUtc" timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS "IX_UserSubscriptions_UserId" ON alchimalia_schema."UserSubscriptions" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_UserSubscriptions_StripeSessionId" ON alchimalia_schema."UserSubscriptions" ("StripeSessionId");
CREATE INDEX IF NOT EXISTS "IX_UserSubscriptions_ExpiresAtUtc" ON alchimalia_schema."UserSubscriptions" ("ExpiresAtUtc");

COMMENT ON TABLE alchimalia_schema."UserSubscriptions" IS 'Stripe Supporter Pack purchases; ExpiresAtUtc = PaidAtUtc + 30 days for one-time';
