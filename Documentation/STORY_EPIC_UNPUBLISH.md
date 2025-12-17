# Story Epic Unpublish Feature

## Overview

This document describes the unpublish functionality for Story Epics, which allows creators to decomission published epics while preserving player progress and history.

## Architecture

### Soft Delete Approach

Story Epic unpublish uses a **soft delete** strategy:
- Sets `Status = "unpublished"` 
- Sets `IsActive = false`
- Preserves all data in database (including player progress)
- Deletes published assets from blob storage

### Why Preserve Player Progress?

Unlike Hero and Region unpublish (which have minimal player impact), Story Epics contain:
- Complete player progress (`EpicProgress`)
- Completed stories (`EpicStoryProgress`)
- Unlocked heroes (`StoryEpicHeroReferences`)

**Decision**: We preserve this data so players can:
1. See their historical progress
2. Understand the epic was decomissioned (not lost)
3. Receive a friendly message about why it's unavailable

## Database Changes

### Migration: V0062

Adds `IsActive` column to `StoryEpicDefinitions`:

```sql
ALTER TABLE alchimalia_schema."StoryEpicDefinitions"
ADD COLUMN "IsActive" boolean NOT NULL DEFAULT true;

CREATE INDEX "IX_StoryEpicDefinitions_IsActive" 
    ON alchimalia_schema."StoryEpicDefinitions" ("IsActive");

CREATE INDEX "IX_StoryEpicDefinitions_Status_IsActive" 
    ON alchimalia_schema."StoryEpicDefinitions" ("Status", "IsActive");
```

### Entity Changes

**StoryEpicDefinition.cs**:
```csharp
public bool IsActive { get; set; } = true; // For soft delete / unpublish
```

## API Endpoint

### POST `/api/story-editor/epics/{epicId}/unpublish`

**Authorization**: Required (Creator or Admin)

**Request Body**:
```json
{
  "confirmEpicId": "my-epic-v1",
  "reason": "Security vulnerability found in story content"
}
```

**Response** (200 OK):
```json
{
  "ok": true,
  "status": "unpublished"
}
```

**Validation**:
- Epic ID confirmation must match route parameter
- Reason is required (for audit purposes)
- User must own the epic
- Epic must be currently published and active

## Service Implementation

### StoryEpicService.UnpublishAsync

```csharp
public async Task UnpublishAsync(Guid ownerUserId, string epicId, string reason, CancellationToken ct)
{
    // 1. Load epic definition
    // 2. Validate ownership and status
    // 3. Mark as unpublished (Status + IsActive)
    // 4. Save changes
    // 5. Delete published assets from blob storage
    // 6. Log operation
}
```

### Asset Cleanup

**EpicPublishedAssetCleanupService** deletes:
- Cover images (`images/tales-of-alchimalia/epics/{email}/{epicId}/`)
- Reward images (from story nodes)
- Any other epic-related assets

**Path Pattern**:
```
{category}/tales-of-alchimalia/epics/{ownerEmail}/{epicId}/*
```

Where `category` ∈ {images, audio, video}

## Query Filtering

### Public Queries (Players)

Must filter by `IsActive`:

```csharp
// GetAllPublishedEpicsAsync
.Where(d => d.Status == "published" && d.IsActive && d.PublishedAtUtc != null)

// GetPublishedEpicAsync
if (definition.Status != "published" || !definition.IsActive)
{
    return null; // Epic not available
}
```

### Owner Queries (Creators)

Should **NOT** filter by `IsActive`:
- `GetEpicAsync` - allows editing unpublished epics
- `ListEpicsByOwnerAsync` - shows all epics (including unpublished)

This allows creators to:
1. See their unpublished epics
2. Create new versions from unpublished epics
3. Re-publish if needed

## Frontend Integration

### Player Experience

When a player tries to access an unpublished epic:

**Recommended Message**:
```
"Acest epic nu mai este disponibil din motive de siguranță. 
Progresul tău a fost păstrat pentru referință."

(This epic is no longer available for security reasons. 
Your progress has been preserved for reference.)
```

### Creator Experience

In the epic list, unpublished epics should:
- Show status badge: "Unpublished"
- Display in a muted/disabled style
- Allow "Create New Version" action
- Show unpublish reason (if stored)

## Data Preservation

### What is Preserved

✅ **Preserved**:
- `StoryEpicDefinition` record (Status = "unpublished", IsActive = false)
- All child records (Regions, StoryNodes, UnlockRules, Translations)
- Player progress (`EpicProgress`, `EpicStoryProgress`)
- Hero references (`StoryEpicHeroReferences`)

❌ **Deleted**:
- Published assets in blob storage (images, audio, video)

### Recovery

To "recover" an unpublished epic:
1. Creator creates a new version from the unpublished definition
2. Makes necessary changes in the draft
3. Submits for review and publishes again

The new publish creates a fresh `StoryEpicDefinition` with a new version number.

## Security Considerations

### Ownership Validation

- Only the epic owner can unpublish
- Admin role override is NOT implemented (by design)
- Unpublish reason is logged for audit trail

### Asset Cleanup

- Assets are permanently deleted from blob storage
- No automatic backup is created
- Assets can be recovered from draft if needed

### Player Data

- Player progress is never deleted
- Players retain access to their historical data
- No player notification is sent (handled by frontend)

## Testing Checklist

- [ ] Unpublish a published epic
- [ ] Verify epic no longer appears in public epic list
- [ ] Verify player cannot access unpublished epic
- [ ] Verify player progress is preserved
- [ ] Verify assets are deleted from blob storage
- [ ] Verify owner can still see epic in their list
- [ ] Verify owner can create new version from unpublished epic
- [ ] Verify unpublish validation (ownership, status, confirmation)
- [ ] Verify unpublish reason is logged

## Related Files

### Backend
- `StoryEpicDefinition.cs` - Entity with IsActive flag
- `UnpublishStoryEpicModels.cs` - Request/Response models
- `UnpublishStoryEpicEndpoint.cs` - HTTP endpoint
- `IStoryEpicService.cs` - Service interface
- `StoryEpicService.cs` - Service implementation
- `EpicPublishedAssetCleanupService.cs` - Asset cleanup service
- `V0062__add_isactive_to_story_epic_definitions.sql` - Database migration

### Frontend
- TBD: Epic list component (show unpublished status)
- TBD: Unpublish dialog/confirmation
- TBD: Player epic access (show decomission message)

## See Also

- `UnpublishHeroEndpoint.cs` - Similar pattern for heroes
- `UnpublishRegionEndpoint.cs` - Similar pattern for regions
- `STORY_EPIC_LEGACY_REMOVAL.md` - Epic architecture evolution

