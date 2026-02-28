# TASK.md — Current task (auto-generated)

## Goal
Backend performance and stability: pagination (StoryCrafts, GetUserCreatedStories), marketplace batch likes, DirectUploadRateLimitService lock pool, MarketplaceCatalogCache ResetAll cleanup, GetCurrentUserId removed in favor of GetUserIdAsync + 401.

## Baseline to preserve
- Existing API response shapes; additive query params (page, pageSize) with defaults.
- Authorization and business logic unchanged.

## Acceptance criteria
- [x] StoryCrafts endpoint: pagination (page, pageSize), TotalCount, HasMore; ListByOwnerPagedAsync / ListAllPagedAsync.
- [x] GetUserCreatedStories: pagination (page, pageSize), TotalCount, HasMore.
- [x] GetFeaturedStoriesAsync: batch GetStoryLikesCountsAsync (no N+1).
- [x] DirectUploadRateLimitService: fixed lock pool (64), no unbounded dictionary.
- [x] MarketplaceCatalogCache.ResetAll(): clear _knownStoryLocales and _knownEpicLocales.
- [x] GetUserOwnedStoriesEndpoint and RemoveStoryFromLibraryEndpoint use GetUserIdAsync + 401; GetCurrentUserId removed from IUserContextService.

## Plan (T1..Tn)
- T1: Pagination StoryCrafts (repository paged methods, endpoint page/pageSize, HasMore).
- T2: Pagination GetUserCreatedStories (page/pageSize, HasMore).
- T3: GetFeaturedStoriesAsync batch likes.
- T4: DirectUploadRateLimitService lock pool.
- T5: MarketplaceCatalogCache.ResetAll() clear locale dictionaries.
- T6: GetCurrentUserId → GetUserIdAsync in two endpoints; remove GetCurrentUserId.

## Status
- All items implemented. Build compiles; copy step may fail if XooCreator.BA process is running (stop app then rebuild).
