-- Migration: Add IdempotencyKey to SupporterPackOrders for deduplicating create-order requests

ALTER TABLE alchimalia_schema."SupporterPackOrders"
ADD COLUMN IF NOT EXISTS "IdempotencyKey" varchar(64);

-- Ensure fast lookup and uniqueness per user + idempotency key
CREATE UNIQUE INDEX IF NOT EXISTS "UX_SupporterPackOrders_UserId_IdempotencyKey"
ON alchimalia_schema."SupporterPackOrders" ("UserId", "IdempotencyKey")
WHERE "IdempotencyKey" IS NOT NULL;

