# TASK.md — Current task (auto-generated)

## Goal
Identify why a story publish can hang/fail in production while local works, by auditing all JSON-derived fields (not only reward tokens) that can violate DB constraints or publish assumptions. Add pre-publish and save-boundary validation with copyable diagnostics.

## Baseline to preserve
- Existing successful publish flow unchanged (regression-safe).
- Existing API response shapes and authorization rules (INVARIANTS).
- No breaking changes to existing editor payloads; only block clear constraint violations.

## Acceptance criteria
- [x] A full list of JSON fields that can break publish is documented and cross-checked against DB constraints (PUBLISH_JSON_CONSTRAINTS.md).
- [x] Pre-publish validation guard runs before UpsertFromCraftAsync; on violation job fails with structured, copyable diagnostic message.
- [x] Save boundary rejects obvious invalid payloads (duplicate IDs, token length overflow) before persisting.
- [x] Targeted tests for token overflow, duplicate IDs, invalid dialog hero reference, and valid craft regression (PublishJsonConstraintsTests).
- [ ] After deploy: re-attempt publish in production; use banner "Copiază detaliile" / Application Insights to confirm failure cause if it persists.

## Plan (T1..Tn)
- T1: Build constraint matrix (DTO → craft → definition → DB) — done; see Features/Story-Editor/PUBLISH_JSON_CONSTRAINTS.md.
- T2: Add IStoryPublishCraftValidator + StoryPublishCraftValidator; invoke in StoryPublishQueueWorker before publish; fail job with ToDiagnosticMessage().
- T3: Add SavePayloadValidator (static); call from SaveStoryEditEndpoint after deserialize; return BadRequest on violations.
- T4: Add unit tests (SavePayloadValidator + StoryPublishCraftValidator): token overflow, duplicate tile/answer/node IDs, hero not in DialogParticipants, valid regression.
- T5: Update TASK.md (this file).

## Refactor triggers / notes
- None triggered; new files and small, surgical edits only.

## Risks / edge cases
- Old craft rows in production may already violate constraints; pre-publish guard will fail them with clear message instead of hang.
- Over-validating at save could block legacy-but-recoverable drafts; current checks are limited to duplicate IDs and token max lengths (publish limits).
- Multiple workers: diagnostics keyed by jobId/storyId/draftVersion; ErrorMessage truncated to 1900 chars for DB.

## Status
- T1: done (PUBLISH_JSON_CONSTRAINTS.md)
- T2: done (StoryPublishCraftValidator, PublishCraftValidationResult, worker integration)
- T3: done (SavePayloadValidator, SaveStoryEditEndpoint)
- T4: done (PublishJsonConstraintsTests)
- T5: done (this update)
