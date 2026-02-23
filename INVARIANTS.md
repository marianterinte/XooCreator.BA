# INVARIANTS.md — Backend invariants (must never break)

## Business invariants
- Do not change existing business rules unless explicitly required.
- Keep existing API response shapes compatible unless explicitly requested.
- Preserve existing authorization rules.
- Migrations must be safe and reversible (if DB involved).

## Reliability invariants
- Tests must remain green.
- No silent behavior changes; if behavior changes, document in TASK.md + add tests.

## Code invariants
- Prefer reuse over duplication.
- Patterns (Factory/Extensions) only if they improve clarity; never hide business logic.