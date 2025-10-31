# Story Editor - New Story Feature Implementation

## Overview
This document describes the implementation of the "New Story" feature in the Story Editor, including confirmation dialog, editable StoryID field, validation, and backend endpoint for checking story ID availability.

## Implementation Steps

### Step 1: Confirmation Dialog for New Story
**Location:** `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts`

**Changes:**
- Add confirmation dialog before executing `newStory()`
- Dialog message: "Do you really want to start a new story? You will lose all current changes."
- Only show warning if there are unsaved changes (check if tiles.length > 0 or title/coverImageUrl is set)
- If confirmed, proceed with `newStory()`, otherwise cancel

**Implementation:**
```typescript
newStory() {
  // Check if there are changes to warn about
  const hasChanges = this.tiles().length > 0 || 
                     this.title.trim() !== '' || 
                     this.coverImageUrl.trim() !== '';
  
  if (hasChanges) {
    const confirmed = confirm('Do you really want to start a new story? You will lose all current changes.');
    if (!confirmed) return;
  }
  
  // Proceed with new story...
}
```

### Step 2: Make StoryID Field Editable in New Story Mode
**Location:** `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts`

**Changes:**
- Add signal `isNewStoryMode = signal(false)`
- In header template, make StoryID field editable when `isNewStoryMode() === true`
- When `newStory()` is called, set `isNewStoryMode.set(true)`
- When loading existing story, set `isNewStoryMode.set(false)`

**Template Update:**
```html
<div style="font-size:16px; color:#fff; font-weight:500">
  <span *ngIf="!isNewStoryMode()">Editor for story: <strong>{{ storyId }}</strong></span>
  <div *ngIf="isNewStoryMode()" style="display:flex; align-items:center; gap:8px">
    <span>Story ID:</span>
    <input [(ngModel)]="storyId" (blur)="validateStoryId()" style="padding:4px 8px; border:1px solid #555; background:#333; color:#fff; border-radius:4px; font-size:14px" />
    <button (click)="checkStoryId()" [disabled]="!isStoryIdValid() || isCheckingStoryId()" style="padding:4px 8px; border:none; background:#007bff; color:#fff; border-radius:4px; cursor:pointer; font-size:12px">✓</button>
    <span *ngIf="storyIdError()" style="color:#dc3545; font-size:12px">{{ storyIdError() }}</span>
  </div>
</div>
```

### Step 3: Client-Side StoryID Validator
**Location:** `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts`

**Validation Rules:**
- StoryID must end with pattern: `-s1`, `-s2`, `-s3`, etc. (where number >= 1)
- Pattern: `-s[1-9]\d*` (starts with -s, followed by at least one digit, first digit 1-9)
- Valid examples: `learn-to-read-s1`, `my-story-s2`, `test-s10`
- Invalid examples: `learn-to-read`, `story-s0`, `story-s`

**Implementation:**
```typescript
isStoryIdValid(): boolean {
  if (!this.storyId || this.storyId.trim() === '') return false;
  const pattern = /-s[1-9]\d*$/;
  return pattern.test(this.storyId.trim());
}

storyIdError = signal<string | null>(null);
```

### Step 4: Check Button and Backend Integration
**Location Frontend:** `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts`

**Location Backend:** `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/CheckStoryIdEndpoint.cs`

**Frontend Changes:**
- Add check button (✓) next to StoryID input
- Button disabled if:
  - StoryID is not valid (doesn't match pattern)
  - Currently checking (loading state)
- On click, call backend endpoint
- Display error message if story ID already exists

**Backend Endpoint:**
- Route: `GET /api/{locale}/stories/editor/check-id/{storyId}`
- Check if storyId exists in database
- Return: `{ exists: boolean, message?: string }`
- Authorize required

**Frontend Service Method:**
```typescript
checkStoryId(): void {
  if (!this.isStoryIdValid()) {
    this.storyIdError.set('Story ID must end with -s1, -s2, -s3, etc.');
    return;
  }
  
  this.isCheckingStoryId.set(true);
  this.storyIdError.set(null);
  
  this.editorService.checkStoryId(this.storyId.trim()).subscribe({
    next: (response) => {
      if (response.exists) {
        this.storyIdError.set('This story ID already exists. Please choose a different one.');
      } else {
        this.storyIdError.set(null);
        // Story ID is available
      }
    },
    error: (error) => {
      this.storyIdError.set('Failed to check story ID. Please try again.');
    },
    finally: () => {
      this.isCheckingStoryId.set(false);
    }
  });
}
```

### Step 5: Backend Endpoint Implementation

**File:** `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/CheckStoryIdEndpoint.cs`

**Structure:**
```csharp
[Endpoint]
public class CheckStoryIdEndpoint
{
    private readonly IStoriesRepository _repository;
    
    [Route("/api/{locale}/stories/editor/check-id/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<CheckStoryIdResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] CheckStoryIdEndpoint ep,
        [FromRoute] string storyId)
    {
        var exists = await ep._repository.StoryIdExistsAsync(storyId);
        return TypedResults.Ok(new CheckStoryIdResponse 
        { 
            Exists = exists 
        });
    }
}
```

**DTO:**
```csharp
public record CheckStoryIdResponse
{
    public bool Exists { get; init; }
}
```

**Repository Method:**
Add to `IStoriesRepository` and `StoriesRepository`:
```csharp
Task<bool> StoryIdExistsAsync(string storyId);
```

### Step 6: Update StoryEditorService

**Location:** `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.service.ts`

**Add Method:**
```typescript
checkStoryId(storyId: string): Observable<CheckStoryIdResponse> {
  const locale = this.languageService.getLanguage();
  return this.http.get<CheckStoryIdResponse>(
    `${this.apiUrl}/api/${locale}/stories/editor/check-id/${encodeURIComponent(storyId)}`
  );
}
```

**Type:**
```typescript
export interface CheckStoryIdResponse {
  exists: boolean;
}
```

## Complete Flow

1. User clicks "New Story" button
2. System checks if there are unsaved changes
3. If changes exist, show confirmation dialog: "Do you really want to start a new story? You will lose all current changes."
4. If confirmed, enter "New Story Mode":
   - Clear all tiles (keep one empty)
   - Reset title, coverImageUrl, unlockedStoryHeroes
   - Set `isNewStoryMode = true`
   - Make StoryID field editable
5. User enters StoryID
6. Client-side validation:
   - Check pattern: must end with `-s1`, `-s2`, etc.
   - Show error if invalid pattern
7. User clicks check button (✓)
8. Frontend calls backend endpoint: `GET /api/{locale}/stories/editor/check-id/{storyId}`
9. Backend checks database for existing storyId
10. If exists: show error message "This story ID already exists. Please choose a different one."
11. If not exists: clear error, allow user to proceed

## Files to Modify

### Frontend:
1. `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.component.ts`
   - Add confirmation dialog to `newStory()`
   - Add `isNewStoryMode` signal
   - Add `storyIdError` signal
   - Add `isCheckingStoryId` signal
   - Add `isStoryIdValid()` method
   - Add `checkStoryId()` method
   - Update template with editable StoryID field
   - Update template with check button

2. `FE/XooCreator/XooCreator/xoo-creator/src/app/story/story-editor/story-editor.service.ts`
   - Add `checkStoryId()` method
   - Add `CheckStoryIdResponse` interface

### Backend:
1. `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/CheckStoryIdEndpoint.cs` (NEW)
   - Create endpoint for checking story ID existence

2. `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Repositories/StoriesRepository.cs`
   - Add `StoryIdExistsAsync()` method

3. `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Repositories/IStoriesRepository.cs`
   - Add `Task<bool> StoryIdExistsAsync(string storyId);` to interface

4. `BA/XooCreator.BA/XooCreator.BA/Features/Stories/DTOs/StoriesDtos.cs`
   - Add `CheckStoryIdResponse` record

## Implementation Status

### ✅ Completed

1. **Documentation** - `StoryEditor.md` created with complete implementation guide
2. **Confirmation Dialog** - Added to `newStory()` method with check for unsaved changes
3. **New Story Mode** - Added `isNewStoryMode` signal to track when in new story mode
4. **Editable StoryID Field** - StoryID becomes editable when `isNewStoryMode() === true`
5. **Client-Side Validator** - Pattern validation `/^-s[1-9]\d*$/` implemented
6. **Check Button** - Blue check button (✓) next to StoryID input
7. **Frontend Service** - `checkStoryId()` method added to `StoryEditorService`
8. **Backend Endpoint** - `CheckStoryIdEndpoint` created at `/api/{locale}/stories/editor/check-id/{storyId}`
9. **Repository Method** - `StoryIdExistsAsync()` added to check database
10. **DTO** - `CheckStoryIdResponse` added to `StoriesDtos.cs`

### Implementation Details

**Frontend Signals:**
- `isNewStoryMode`: Tracks if user is creating a new story (StoryID editable)
- `storyIdError`: Error message string or null
- `isCheckingStoryId`: Loading state for backend check
- `storyIdChecked`: True when check was successful and ID is available

**Validation Pattern:**
- Regex: `/-s[1-9]\d*$/`
- Must end with `-s` followed by at least one digit (1-9), then optionally more digits
- Examples: `learn-to-read-s1`, `my-story-s2`, `test-s10`
- Invalid: `learn-to-read`, `story-s0`, `story-s` (no number)

**Backend Endpoint:**
- Route: `GET /api/{locale}/stories/editor/check-id/{storyId}`
- Authorization: Required
- Returns: `{ exists: boolean }`
- Uses `StoryIdExistsAsync()` which normalizes and checks `StoryDefinitions` table

**User Flow:**
1. Click "New Story" → Confirmation dialog (if changes exist)
2. Enter "New Story Mode" → StoryID field becomes editable
3. User types StoryID → Client-side validation on blur
4. If pattern valid → Check button enabled
5. Click check button → Backend call
6. If available → Green "✓ Available" message
7. If exists → Red error message

## Notes

- The StoryID field should only be editable when in "New Story Mode"
- When loading an existing story, StoryID remains read-only
- Pattern validation happens client-side before backend check
- Backend check is only called when pattern is valid and user clicks check button
- Error messages are user-friendly and displayed inline next to the StoryID field
- The "✓ Available" message only shows after a successful check (not just pattern validation)

