-- Rollback for V0001__initial_full_schema.sql
-- Drops and recreates the alchimalia_schema schema (including schema_versions)
-- Use when the entire schema needs to be recreated from scratch before replaying scripts.

BEGIN;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_namespace
        WHERE nspname = 'alchimalia_schema'
    ) THEN
        EXECUTE 'DROP SCHEMA IF EXISTS alchimalia_schema CASCADE';
    END IF;
END $$;

CREATE SCHEMA IF NOT EXISTS alchimalia_schema;

CREATE TABLE IF NOT EXISTS alchimalia_schema.schema_versions
(
    script_name TEXT PRIMARY KEY,
    checksum TEXT NOT NULL,
    executed_at TIMESTAMPTZ NOT NULL,
    execution_time_ms BIGINT,
    status TEXT NOT NULL
);

COMMIT;

