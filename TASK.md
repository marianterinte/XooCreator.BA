# TASK.md — Current task (auto-generated)

## Goal
Improve private story generation resilience for safety and image failures: hard-stop unsafe child-harmful prompts, avoid global job failure on partial image errors, and return completed jobs with structured warnings.

## Baseline to preserve
- Scope only private story flow (`GeneratePrivateStory`) for behavior changes.
- Keep existing fallback behavior for scene plan/text when Gemini blocks content.
- Preserve existing API/job status contract (`Completed`/`Failed`) and keep additive changes only.

## Acceptance criteria
- [ ] Unsafe prompt content for children is blocked early with UI-safe message.
- [ ] Scene planner prompts are softened for safe-kids output without removing fallbacks.
- [ ] Image failures like `IMAGE_OTHER` do not fail the entire private story job.
- [ ] Per-page image generation uses a generic fallback prompt before final skip.
- [ ] Private story jobs can complete with warnings when images are partially missing.
- [ ] Logging/error codes are structured for safety violations and partial image failures.

## Plan (T1..Tn)
- T1: Add private-story safety gate and map explicit error code for UI.
- T2: Soften ScenePlanner prompting for child-safe scene outputs.
- T3: Refactor private image flow to continue per-page on `IMAGE_OTHER`/missing content.
- T4: Add generic fallback image prompt attempt before skipping page image.
- T5: Return `Completed` job with aggregated warnings for partial image completion.
- T6: Add structured logs and error code mapping for safety/partial image outcomes.

## Risks / edge cases
- Safety gate may create false positives if rules are too broad.
- Generic fallback prompt can reduce image quality for difficult scenes.
- Completed-with-warnings behavior must be clearly surfaced in UI to avoid confusion.
