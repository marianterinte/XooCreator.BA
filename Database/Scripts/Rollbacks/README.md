# Rollback scripts

Rollback files live here and follow the naming convention `RXXXX__description.sql`, where `XXXX`
matches the forward script you want to revert (e.g. rollback for `V0005__*` becomes
`R0005__*.sql`). Each file should:

1. Contain a short header describing what it reverts and any prerequisites.
2. Wrap statements in `BEGIN; ... COMMIT;` unless the file manages its own transactions.
3. Leave `alchimalia_schema.schema_versions` intact so the runner can log execution.

## Usage

Run a rollback manually with:

```bash
cd BA/XooCreator.BA
dotnet run --project XooCreator.DbScriptRunner -- \
  --connection "Host=...;Username=...;Password=...;Database=...;SearchPath=alchimalia_schema" \
  --rollback V0001
```

The runner resolves `V0001` → `R0001__*.sql` from this folder, executes it in a transaction,
then replays forward scripts as needed.

## Current scripts

- `R0001__drop_schema.sql` – drops and recreates `alchimalia_schema`, including
  a fresh `schema_versions` table, so the entire stack can be reapplied from `V0001`.

