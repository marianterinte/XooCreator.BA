-- Migration: Add SupporterPackOrders table for Order flow (create order -> pay -> admin Accept -> grant).
-- UserPackGrants.OrderId references this table (FK added below).

CREATE TABLE IF NOT EXISTS alchimalia_schema."SupporterPackOrders"
(
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL REFERENCES alchimalia_schema."AlchimaliaUsers"("Id") ON DELETE CASCADE,
    "PlanId" varchar(50) NOT NULL,
    "Amount" decimal(18,2) NOT NULL,
    "Status" int NOT NULL DEFAULT 0,
    "CreatedAtUtc" timestamptz NOT NULL DEFAULT now(),
    "OrderReference" varchar(255),
    "ProcessedAtUtc" timestamptz,
    "ProcessedByUserId" uuid REFERENCES alchimalia_schema."AlchimaliaUsers"("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_SupporterPackOrders_UserId" ON alchimalia_schema."SupporterPackOrders" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_SupporterPackOrders_Status" ON alchimalia_schema."SupporterPackOrders" ("Status");
CREATE INDEX IF NOT EXISTS "IX_SupporterPackOrders_CreatedAtUtc" ON alchimalia_schema."SupporterPackOrders" ("CreatedAtUtc");

COMMENT ON TABLE alchimalia_schema."SupporterPackOrders" IS 'Supporter Pack orders: PendingPayment -> admin Accept -> Fulfilled + Grant';

-- Optional FK from UserPackGrants to Orders (if not already present in V00129)
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.table_constraints
    WHERE constraint_name = 'FK_UserPackGrants_SupporterPackOrders_OrderId'
    AND table_schema = 'alchimalia_schema' AND table_name = 'UserPackGrants'
  ) THEN
    ALTER TABLE alchimalia_schema."UserPackGrants"
    ADD CONSTRAINT "FK_UserPackGrants_SupporterPackOrders_OrderId"
    FOREIGN KEY ("OrderId") REFERENCES alchimalia_schema."SupporterPackOrders"("Id") ON DELETE SET NULL;
  END IF;
END $$;
