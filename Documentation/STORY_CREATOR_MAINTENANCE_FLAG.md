# Story Creator Maintenance Flag

## Overview
- A global toggle disables Story Crafts and the Story Editor for all creators and reviewers, redirecting them to an informational maintenance page.
- When the flag is **enabled** (maintenance on):
  - **Frontend:** Guarded routes redirect to the maintenance page.
  - **Job submission:** API endpoints that enqueue Story jobs return **503 Service Unavailable** with body `{ "code": "StoryCreatorMaintenance", "message": "Story Creator is currently in maintenance. Job submission is disabled." }`. No new jobs are created or enqueued.
  - **Workers:** Story-related queue workers do **not** process messages while the flag is on. They skip work and do **not** delete the message, so it becomes visible again after the visibility timeout and will be processed once maintenance is turned off.
- State is stored in `PlatformSettings` with key `story-creator-disabled`.
- Admins can change the flag from the Admin Dashboard or via API.

## Database
- Table: `PlatformSettings`
  - Columns: `Key` (PK), `BoolValue`, `StringValue`, `UpdatedAt`, `UpdatedBy`
- Seed: `story-creator-disabled = false`
- Script: `Database/Scripts/V0022__story_creator_maintenance_flag.sql` (run via `XooCreator.DbScriptRunner`).

## Backend API
- `GET /api/story-creator/status` (auth required): returns `{ isDisabled, updatedAt?, updatedBy? }`.
- `PUT /api/admin/story-creator/status` (admin only): body `{ "isDisabled": true|false }`, updates flag and timestamps.

## Frontend Behavior
- Routes guarded: `story-crafts/*`, `story/*/edit`, `story/new/edit`, `story/mandatory/edit`.
- If disabled, users are redirected to `/story-creator-maintenance` with a maintenance message.
- Guard service: `StoryCreatorAvailabilityService` + `storyCreatorAvailabilityGuard`.

## Admin Dashboard Control
- Location: Admin Dashboard → Story Management tab.
- Control: toggle “Story Creator maintenance”; shows current status and errors; writes via `PUT /api/admin/story-creator/status`.

## How to Enable/Disable
1) Use Admin Dashboard toggle (preferred).
2) Or call API:
   - Disable: `PUT /api/admin/story-creator/status` body `{"isDisabled": true}`
   - Enable:  `PUT /api/admin/story-creator/status` body `{"isDisabled": false}`

## Notes
- Flag is honored for authenticated users on guarded routes and for all Story job submission endpoints (503 when disabled).
- When disabled, Story queue workers skip processing and leave messages in the queue; jobs are not deleted and will be processed after the flag is enabled again.
- Default remains enabled unless explicitly set to disabled.

