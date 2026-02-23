# AGENTS.md — Project Golden Rules (Frontend + Backend)

## Non-negotiables (ALWAYS)
1) **Do not break existing business logic.**
   - Prefer additive changes.
   - If behavior must change: call it out explicitly in SPEC.md + tests.

2) **Small, reusable units.**
   - No 2000-line components/services/controllers.
   - If a touched file is too large, refactor is **part of the task**, not “maybe later”.

3) **Tests must stay green.**
   - Add/adjust tests for any behavior change or bugfix.
   - Never “fix” by removing tests.

4) **No surprise architecture rewrites.**
   - If refactor is big: propose a plan first, then execute in safe steps.

5) **Keep diffs surgical.**
   - Touch the fewest files possible.
   - Avoid unrelated formatting/renames.

## Refactor Triggers (When you MUST refactor)
If you touch a file and it violates:
- **File size:** > 400 LOC (hard warning), > 700 LOC (hard stop)
- **Function/method size:** > 60 LOC
- **Cyclomatic complexity:** feels “branchy” / too many nested ifs (aim < 12)
Then:
- Create a subtask: “Refactor into smaller units” + keep behavior identical + tests.

---

# Frontend — Angular Rules

## Structure & Maintainability
- Prefer **small components** (smart/container + dumb/presentational).
- Keep templates readable:
  - If template becomes huge or too nested: extract child component(s).
- Prefer **reusable** components/services; avoid duplicated logic.
- Prefer `OnPush` where possible.
- Prefer `async` pipe over manual subscriptions; avoid nested subscriptions.
- Use `trackBy` for large `*ngFor`.

## Hard limits (pragmatic)
- Component class: aim < 200 LOC (refactor if > 300)
- Template: aim < 200 lines (refactor if > 300)
- Service: aim < 250 LOC (split by responsibility)

## Safety
- Do not change behavior of existing flows without updating SPEC.md + tests.
- Avoid breaking public component APIs unless explicitly required.

---

# Backend — .NET / C# Rules

## Clean Code & Patterns
- Prefer clarity over cleverness.
- Use SOLID principles and clear layering (API -> Application -> Domain -> Infrastructure).
- Use **Factory** pattern only when object creation logic is non-trivial or varies by config.
- Use **Extension methods** for:
  - mapping, small helpers, fluent composition
  - but avoid “god extensions” that hide business logic.
- Reuse shared utilities; do not duplicate logic.

## Safety (Business logic preservation)
- Default approach: behavior-preserving refactor.
- If changing business rules: update SPEC.md acceptance criteria + add tests.

## Hard limits (pragmatic)
- Class: aim < 300 LOC (refactor if > 450)
- Method: aim < 60 LOC (refactor if > 90)
- Avoid deep nesting: prefer guard clauses.

## Reliability
- Validate inputs at boundaries.
- Prefer cancellation tokens in async flows.
- Log important domain events (without leaking secrets).