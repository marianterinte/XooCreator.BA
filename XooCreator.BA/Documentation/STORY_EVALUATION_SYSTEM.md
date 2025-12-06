# Story Evaluation System - Implementation Plan

## Overview

This document outlines the complete implementation plan for adding an evaluation/scoring system to stories that contain quiz questions. When a story is marked as "evaluative", users will be scored based on their quiz answers, and results will be displayed at the end of the story.

## Implementation Status

### ✅ Backend - COMPLETED

**Files Created:**
- `BA/XooCreator.BA/XooCreator.BA/Data/Entities/StoryQuizAnswer.cs`
- `BA/XooCreator.BA/XooCreator.BA/Data/Entities/StoryEvaluationResult.cs`
- `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/SubmitQuizAnswerEndpoint.cs`
- `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/CompleteEvaluationEndpoint.cs`
- `BA/XooCreator.BA/Database/Scripts/V0016__add_story_evaluation_system.sql`

**Files Modified:**
- `BA/XooCreator.BA/XooCreator.BA/Data/Entities/StoryDefinition.cs` - Added `IsEvaluative`
- `BA/XooCreator.BA/XooCreator.BA/Data/Entities/StoryAnswer.cs` - Added `IsCorrect`
- `BA/XooCreator.BA/XooCreator.BA/Data/Entities/StoryCraft.cs` - Added `IsEvaluative` and `IsCorrect` to `StoryCraftAnswer`
- `BA/XooCreator.BA/XooCreator.BA/Data/XooDbContext.cs` - Added DbSets and entity configurations
- `BA/XooCreator.BA/XooCreator.BA/Data/SeedData/DTOs/StorySeedData.cs` - Added `IsEvaluative` and `IsCorrect`
- `BA/XooCreator.BA/XooCreator.BA/Data/SeedData/DTOs/StoriesSeedData.cs` - Added `IsEvaluative` and `IsCorrect`
- `BA/XooCreator.BA/XooCreator.BA/Features/Stories/DTOs/StoriesDtos.cs` - Added `IsEvaluative` and `IsCorrect`
- `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Mappers/StoryDefinitionMapper.cs` - Updated mappers
- `BA/XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/Content/EditableStoryDtos.cs` - Added `IsEvaluative` and `IsCorrect`
- `BA/XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/StoryEditorService.cs` - Save `IsEvaluative`
- `BA/XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/Content/StoryAnswerUpdater.cs` - Save `IsCorrect`
- `BA/XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/StoryPublishingService.cs` - Copy fields on publish
- `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Services/StoriesService.cs` - Load fields in editor

**Phase 1: Data Model & Database** - ✅ COMPLETED
- ✅ Added `IsEvaluative` to `StoryDefinition` entity
- ✅ Added `IsEvaluative` to `StoryCraft` entity (for editor)
- ✅ Added `IsCorrect` to `StoryAnswer` entity
- ✅ Added `IsCorrect` to `StoryCraftAnswer` entity (for editor)
- ✅ Created `StoryQuizAnswer` entity
- ✅ Created `StoryEvaluationResult` entity
- ✅ Created migration script `V0016__add_story_evaluation_system.sql`
- ✅ Updated `XooDbContext` with new entities and configurations

**Phase 2: Backend API** - ✅ COMPLETED
- ✅ Implemented `SubmitQuizAnswerEndpoint` (`POST /api/{locale}/stories/{storyId}/quiz-answer`)
- ✅ Implemented `CompleteEvaluationEndpoint` (`POST /api/{locale}/stories/{storyId}/complete-evaluation`)
- ✅ Both endpoints include proper validation and error handling

**Phase 3: Editor Integration** - ✅ COMPLETED
- ✅ Added `IsEvaluative` to `EditableStoryDto`
- ✅ Added `IsCorrect` to `EditableAnswerDto`
- ✅ Updated `StoryEditorService.SaveDraftAsync` to save `IsEvaluative`
- ✅ Updated `StoryAnswerUpdater` to save `IsCorrect` from DTO to `StoryCraftAnswer`
- ✅ Updated `StoriesService.GetStoryForEditAsync` to load `IsEvaluative` and `IsCorrect`

**Phase 4: Publishing Integration** - ✅ COMPLETED
- ✅ Updated `StoryPublishingService` to copy `IsEvaluative` from `StoryCraft` to `StoryDefinition` on publish
- ✅ Updated `StoryPublishingService` to copy `IsCorrect` from `StoryCraftAnswer` to `StoryAnswer` on publish

**Phase 5: Reading Integration** - ✅ COMPLETED
- ✅ Added `IsEvaluative` to `StoryContentDto`
- ✅ Added `IsCorrect` to `StoryAnswerDto`
- ✅ Updated `StoryDefinitionMapper` to include `IsEvaluative` and `IsCorrect` in DTOs

**Phase 6: Seed Data** - ✅ COMPLETED
- ✅ Updated `StoryDefinitionSeedData` and `StoryAnswerSeedData` DTOs to support `IsEvaluative` and `IsCorrect`
- ✅ Updated `StorySeedData` and `AnswerSeedData` DTOs (legacy format)
- ✅ Updated `StoryDefinitionMapper` to map `IsEvaluative` and `IsCorrect` from JSON seed data

### ⏳ Frontend - PENDING

**Phase 3: Story Reading Integration** - ⏳ PENDING
- ⏳ Update `story-reading.component.ts` to track session (extend existing continuation mechanism)
- ⏳ Add quiz answer submission on selection (silent, no immediate feedback)
- ⏳ Add evaluation completion handler (navigate to separate results route)
- ⏳ Create `evaluation-results.component` (separate route, no breaking changes)
- ⏳ Add route for evaluation results page

**Phase 4: Story Editor Integration** - ⏳ PENDING
- ⏳ Add `isEvaluative` checkbox in story editor
- ⏳ Add `isCorrect` checkbox/toggle for each answer in quiz tiles
- ⏳ Update save/load logic to handle new fields
- ⏳ Add translations

**Phase 5: UI Polish** - ⏳ PENDING
- ⏳ Add quiz progress indicator (optional)
- ⏳ Style results screen
- ⏳ Add animations

**Phase 6: Translations** - ⏳ PENDING
- ⏳ Add translation keys for all evaluation-related UI elements

## Goals

1. **Mark stories as evaluative** - Add a flag to identify stories that should be evaluated
2. **Track quiz answers** - Store individual quiz answers during story reading
3. **Calculate scores** - Compute final score based on correct/incorrect answers
4. **Display results** - Show evaluation results screen at story completion with breakdown per quiz
5. **Store evaluation history** - Keep track of all attempts (no best score tracking for MVP)

## Design Decisions

### Session Management
- **Session ID**: Extend existing story reading continuation mechanism. Session ID is generated at story start and persisted in localStorage to allow resuming.
- **Resume behavior**: If user closes browser and returns, they can continue with the same session ID stored in localStorage.

### Feedback & Navigation
- **Feedback timing**: Feedback is shown only at the end in the results screen, not immediately after each answer.
- **Quiz navigation**: Users cannot skip quizzes - they must select an answer to proceed (quizzes are part of the story flow).

### Retry Behavior
- **Retry**: When user retries, they start from zero with no influence from previous attempts (fresh start).

### Tokens & Scoring
- **Tokens**: Keep simple for MVP - only scoring. Complex token configurations will be added later.
- **Score calculation**: Simple formula: `(CorrectAnswers / TotalQuizzes) * 100`

### Editor Validation
- **isEvaluative = false**: No warnings or suggestions. It's the creator's responsibility to mark stories as evaluative. Stories can have quizzes without being evaluative (they just give tokens as before).

### Results Display
- **Component**: Separate component and route to avoid breaking existing logic.
- **Content**: Show score + breakdown per quiz (which were correct/incorrect).

### Data Management
- **Story versioning**: Use current story version at completion time (not snapshot from start).
- **Multiple attempts**: Store all attempts (no limit).
- **Answer overwrite**: If user goes back and changes answer, store only the last one (overwrite previous answer for same tile in same session).
- **Integration**: Extend existing `UserStoryReadProgress` logic without breaking it. If better to keep separate, keep separate but similar.

### Best Score
- **Best score tracking**: Not implemented in MVP. Will be added later in parent dashboard with different scoring system.

---

## Phase 1: Data Model & Database

### 1.1 Add Evaluation Flag to StoryDefinition

**Entity Change:**
```csharp
public class StoryDefinition
{
    // ... existing fields ...
    
    /// <summary>
    /// If true, this story contains quizzes that should be evaluated.
    /// When true, quiz answers are tracked and a score is calculated at completion.
    /// </summary>
    public bool IsEvaluative { get; set; } = false;
}
```

**Migration:**
- Add `IsEvaluative` column (boolean, default false) to `StoryDefinitions` table
- Update existing stories: set to `true` only for stories that have quizzes AND should be evaluated

**JSON Structure:**
```json
{
  "storyId": "learn-math-s1",
  "title": "Trouble on Mechanika",
  "isEvaluative": true,  // NEW: Flag indicating this story should be evaluated
  "tiles": [
    {
      "tileId": "q1",
      "type": "quiz",
      "question": "...",
      "answers": [
        {
          "answerId": "a",
          "text": "4",
          "isCorrect": false  // Already exists
        },
        {
          "answerId": "b",
          "text": "5",
          "isCorrect": true   // Already exists
        }
      ]
    }
  ]
}
```

### 1.2 Create Quiz Answer Tracking Table

**New Entity: `StoryQuizAnswer`**
```csharp
namespace XooCreator.BA.Data;

/// <summary>
/// Tracks individual quiz answers submitted by users during story reading.
/// One record per quiz question answered.
/// </summary>
public class StoryQuizAnswer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "learn-math-s1"
    public string TileId { get; set; } = string.Empty;  // e.g., "q1", "q2"
    public string SelectedAnswerId { get; set; } = string.Empty; // e.g., "a", "b", "c"
    public bool IsCorrect { get; set; } // Calculated: true if selected answer has isCorrect=true
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    
    // Session tracking (for retries)
    public Guid? SessionId { get; set; } // Groups answers from same story reading session
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}
```

**Indexes:**
- `(UserId, StoryId, TileId, SessionId)` - For querying answers in a session
- `(UserId, StoryId)` - For getting all answers for a story
- `(SessionId)` - For grouping answers by reading session

### 1.3 Create Story Evaluation Results Table

**New Entity: `StoryEvaluationResult`**
```csharp
namespace XooCreator.BA.Data;

/// <summary>
/// Stores evaluation results for a user's completion of an evaluative story.
/// One record per story completion attempt. All attempts are stored (no limit).
/// Best score tracking will be added later in parent dashboard.
/// </summary>
public class StoryEvaluationResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid SessionId { get; set; } // Links to StoryQuizAnswer records
    
    // Score calculation
    public int TotalQuizzes { get; set; }        // Total number of quiz tiles in story
    public int CorrectAnswers { get; set; }      // Number of correct answers
    public int ScorePercentage { get; set; }     // 0-100: (CorrectAnswers / TotalQuizzes) * 100
    
    // Completion tracking
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}
```

**Indexes:**
- `(UserId, StoryId, CompletedAt)` - For querying user's history
- Unique constraint: `(UserId, StoryId, SessionId)` - One result per session

### 1.4 Migration Script

**File:** `V0016__add_story_evaluation_system.sql` ✅ IMPLEMENTED

```sql
BEGIN;

-- Add IsEvaluative flag to StoryDefinitions
ALTER TABLE "alchimalia_schema"."StoryDefinitions"
    ADD COLUMN IF NOT EXISTS "IsEvaluative" boolean NOT NULL DEFAULT false;

-- Add IsEvaluative flag to StoryCrafts (for editor drafts)
ALTER TABLE "alchimalia_schema"."StoryCrafts"
    ADD COLUMN IF NOT EXISTS "IsEvaluative" boolean NOT NULL DEFAULT false;

-- Add IsCorrect flag to StoryAnswers
ALTER TABLE "alchimalia_schema"."StoryAnswers"
    ADD COLUMN IF NOT EXISTS "IsCorrect" boolean NOT NULL DEFAULT false;

-- Add IsCorrect flag to StoryCraftAnswers (for editor drafts)
ALTER TABLE "alchimalia_schema"."StoryCraftAnswers"
    ADD COLUMN IF NOT EXISTS "IsCorrect" boolean NOT NULL DEFAULT false;

-- Create StoryQuizAnswer table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryQuizAnswers" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" text NOT NULL,
    "TileId" text NOT NULL,
    "SelectedAnswerId" text NOT NULL,
    "IsCorrect" boolean NOT NULL,
    "AnsweredAt" timestamp with time zone NOT NULL,
    "SessionId" uuid,
    CONSTRAINT "PK_StoryQuizAnswers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryQuizAnswers_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_UserStoryTileSession" 
    ON "alchimalia_schema"."StoryQuizAnswers" ("UserId", "StoryId", "TileId", "SessionId");

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_UserStory" 
    ON "alchimalia_schema"."StoryQuizAnswers" ("UserId", "StoryId");

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_Session" 
    ON "alchimalia_schema"."StoryQuizAnswers" ("SessionId");

-- Create StoryEvaluationResult table
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryEvaluationResults" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" text NOT NULL,
    "SessionId" uuid NOT NULL,
    "TotalQuizzes" integer NOT NULL,
    "CorrectAnswers" integer NOT NULL,
    "ScorePercentage" integer NOT NULL,
    "CompletedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_StoryEvaluationResults" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryEvaluationResults_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "alchimalia_schema"."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryEvaluationResults_UserStorySession" 
        UNIQUE ("UserId", "StoryId", "SessionId")
);

CREATE INDEX IF NOT EXISTS "IX_StoryEvaluationResults_UserStoryCompleted" 
    ON "alchimalia_schema"."StoryEvaluationResults" ("UserId", "StoryId", "CompletedAt");

COMMIT;
```

---

## Phase 2: Backend API ✅ COMPLETED

### 2.1 Submit Quiz Answer Endpoint ✅ IMPLEMENTED

**Endpoint:** `POST /api/{locale}/stories/{storyId}/quiz-answer`

**Location:** `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/SubmitQuizAnswerEndpoint.cs`

**Request:**
```csharp
public record SubmitQuizAnswerRequest
{
    public required string TileId { get; init; }        // e.g., "q1"
    public required string SelectedAnswerId { get; init; } // e.g., "a", "b", "c"
    public Guid? SessionId { get; init; }               // Optional: for grouping answers
}
```

**Response:**
```csharp
public record SubmitQuizAnswerResponse
{
    public bool Success { get; init; }
    public bool IsCorrect { get; init; }
    public Guid SessionId { get; init; }  // Returned session ID (new or existing)
    public string? ErrorMessage { get; init; }
}
```

**Logic:**
1. Load story and find quiz tile by `TileId`
2. Find selected answer in quiz tile
3. Check if `isCorrect` is true
4. **Overwrite existing answer** if user already answered this quiz in the same session (update existing `StoryQuizAnswer` record for same `UserId`, `StoryId`, `TileId`, `SessionId`)
5. Generate `SessionId` if not provided (new reading session) - but prefer using existing session from continuation mechanism
6. Return result with `IsCorrect` flag (for internal tracking, not shown to user immediately)

**Implementation:**
```csharp
[Route("/api/{locale}/stories/{storyId}/quiz-answer")]
[Authorize]
public static async Task<Results<Ok<SubmitQuizAnswerResponse>, BadRequest<string>>> HandleSubmit(
    [FromRoute] string locale,
    [FromRoute] string storyId,
    [FromBody] SubmitQuizAnswerRequest request,
    [FromServices] SubmitQuizAnswerEndpoint ep,
    ClaimsPrincipal user,
    CancellationToken ct)
{
    var userId = ep.GetUserId(user);
    var sessionId = request.SessionId ?? Guid.NewGuid();
    
    // Load story
    var story = await ep._repository.GetStoryDefinitionByIdAsync(storyId);
    if (story == null)
        return TypedResults.BadRequest("Story not found");
    
    // Find quiz tile
    var quizTile = story.Tiles.FirstOrDefault(t => t.TileId == request.TileId && t.Type == "quiz");
    if (quizTile == null)
        return TypedResults.BadRequest("Quiz tile not found");
    
    // Find selected answer
    var selectedAnswer = quizTile.Answers.FirstOrDefault(a => a.AnswerId == request.SelectedAnswerId);
    if (selectedAnswer == null)
        return TypedResults.BadRequest("Answer not found");
    
    // Check if correct (isCorrect flag from JSON)
    var isCorrect = selectedAnswer.IsCorrect;
    
    // Check if answer already exists for this session (overwrite)
    var existingAnswer = await ep._context.StoryQuizAnswers
        .FirstOrDefaultAsync(a => a.UserId == userId 
                                && a.StoryId == storyId 
                                && a.TileId == request.TileId 
                                && a.SessionId == sessionId, ct);
    
    if (existingAnswer != null)
    {
        // Update existing answer
        existingAnswer.SelectedAnswerId = request.SelectedAnswerId;
        existingAnswer.IsCorrect = isCorrect;
        existingAnswer.AnsweredAt = DateTime.UtcNow;
    }
    else
    {
        // Create new answer
        var quizAnswer = new StoryQuizAnswer
        {
            UserId = userId,
            StoryId = storyId,
            TileId = request.TileId,
            SelectedAnswerId = request.SelectedAnswerId,
            IsCorrect = isCorrect,
            SessionId = sessionId,
            AnsweredAt = DateTime.UtcNow
        };
        
        ep._context.StoryQuizAnswers.Add(quizAnswer);
    }
    
    await ep._context.SaveChangesAsync(ct);
    
    return TypedResults.Ok(new SubmitQuizAnswerResponse
    {
        Success = true,
        IsCorrect = isCorrect,
        SessionId = sessionId
    });
}
```

### 2.2 Complete Evaluative Story Endpoint ✅ IMPLEMENTED

**Endpoint:** `POST /api/{locale}/stories/{storyId}/complete-evaluation`

**Location:** `BA/XooCreator.BA/XooCreator.BA/Features/Stories/Endpoints/CompleteEvaluationEndpoint.cs`

**Request:**
```csharp
public record CompleteEvaluationRequest
{
    public required Guid SessionId { get; init; } // Session ID from quiz answers
}
```

**Response:**
```csharp
public record CompleteEvaluationResponse
{
    public bool Success { get; init; }
    public int TotalQuizzes { get; init; }
    public int CorrectAnswers { get; init; }
    public int ScorePercentage { get; init; } // 0-100
    public List<QuizAnswerDetail> QuizDetails { get; init; } = new(); // Breakdown per quiz
    public string? ErrorMessage { get; init; }
}

public record QuizAnswerDetail
{
    public string TileId { get; init; } = string.Empty;
    public string Question { get; init; } = string.Empty;
    public string SelectedAnswerId { get; init; } = string.Empty;
    public string SelectedAnswerText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public string? CorrectAnswerId { get; init; } // Only if incorrect
    public string? CorrectAnswerText { get; init; } // Only if incorrect
}
```

**Logic:**
1. Load story and verify `IsEvaluative == true` (use current story version at completion time)
2. Count total quiz tiles in story
3. Query all `StoryQuizAnswer` records for this `SessionId`
4. Count correct answers (`IsCorrect == true`)
5. Calculate score: `(CorrectAnswers / TotalQuizzes) * 100`
6. Build quiz details breakdown (for each quiz: question, selected answer, correct answer if wrong)
7. Create `StoryEvaluationResult` record
8. Return evaluation results with breakdown

**Implementation:**
```csharp
[Route("/api/{locale}/stories/{storyId}/complete-evaluation")]
[Authorize]
public static async Task<Results<Ok<CompleteEvaluationResponse>, BadRequest<string>>> HandleComplete(
    [FromRoute] string locale,
    [FromRoute] string storyId,
    [FromBody] CompleteEvaluationRequest request,
    [FromServices] CompleteEvaluationEndpoint ep,
    ClaimsPrincipal user,
    CancellationToken ct)
{
    var userId = ep.GetUserId(user);
    
    // Load story
    var story = await ep._repository.GetStoryDefinitionByIdAsync(storyId);
    if (story == null)
        return TypedResults.BadRequest("Story not found");
    
    if (!story.IsEvaluative)
        return TypedResults.BadRequest("Story is not evaluative");
    
    // Count total quizzes
    var totalQuizzes = story.Tiles.Count(t => t.Type == "quiz");
    if (totalQuizzes == 0)
        return TypedResults.BadRequest("Story has no quizzes");
    
    // Get all answers for this session
    var answers = await ep._context.StoryQuizAnswers
        .Where(a => a.UserId == userId 
                 && a.StoryId == storyId 
                 && a.SessionId == request.SessionId)
        .ToListAsync(ct);
    
    // Count correct answers
    var correctAnswers = answers.Count(a => a.IsCorrect);
    
    // Calculate score
    var scorePercentage = (int)Math.Round((double)correctAnswers / totalQuizzes * 100);
    
    // Build quiz details breakdown
    var quizDetails = new List<QuizAnswerDetail>();
    var quizTiles = story.Tiles.Where(t => t.Type == "quiz").ToList();
    
    foreach (var quizTile in quizTiles)
    {
        var answer = answers.FirstOrDefault(a => a.TileId == quizTile.TileId);
        var selectedAnswer = answer != null 
            ? quizTile.Answers.FirstOrDefault(a => a.AnswerId == answer.SelectedAnswerId)
            : null;
        
        var correctAnswer = quizTile.Answers.FirstOrDefault(a => a.IsCorrect);
        
        quizDetails.Add(new QuizAnswerDetail
        {
            TileId = quizTile.TileId,
            Question = quizTile.Question ?? string.Empty,
            SelectedAnswerId = answer?.SelectedAnswerId ?? string.Empty,
            SelectedAnswerText = selectedAnswer?.Text ?? string.Empty,
            IsCorrect = answer?.IsCorrect ?? false,
            CorrectAnswerId = answer?.IsCorrect == false ? correctAnswer?.AnswerId : null,
            CorrectAnswerText = answer?.IsCorrect == false ? correctAnswer?.Text : null
        });
    }
    
    // Create evaluation result
    var result = new StoryEvaluationResult
    {
        UserId = userId,
        StoryId = storyId,
        SessionId = request.SessionId,
        TotalQuizzes = totalQuizzes,
        CorrectAnswers = correctAnswers,
        ScorePercentage = scorePercentage,
        CompletedAt = DateTime.UtcNow
    };
    
    ep._context.StoryEvaluationResults.Add(result);
    await ep._context.SaveChangesAsync(ct);
    
    return TypedResults.Ok(new CompleteEvaluationResponse
    {
        Success = true,
        TotalQuizzes = totalQuizzes,
        CorrectAnswers = correctAnswers,
        ScorePercentage = scorePercentage,
        QuizDetails = quizDetails
    });
}
```

### 2.3 Get Evaluation History Endpoint ⏳ NOT IMPLEMENTED (Optional for MVP)

**Endpoint:** `GET /api/{locale}/stories/{storyId}/evaluation-history`

**Status:** Not implemented in MVP. Can be added later if needed for displaying user's attempt history.

**Response:**
```csharp
public record EvaluationHistoryResponse
{
    public List<EvaluationHistoryItem> Attempts { get; init; } = new();
    public int TotalAttempts { get; init; }
}

public record EvaluationHistoryItem
{
    public Guid SessionId { get; init; }
    public int ScorePercentage { get; init; }
    public int CorrectAnswers { get; init; }
    public int TotalQuizzes { get; init; }
    public DateTime CompletedAt { get; init; }
}
```

---

## Phase 3: Frontend - Story Reading ⏳ PENDING

### 3.1 Update Story Reading Component

**File:** `story-reading.component.ts`

**Changes:**
1. Add `sessionId` signal to track reading session (extend existing continuation mechanism)
2. Add `quizAnswers` signal to track answers during reading (for internal tracking only)
3. Add `isEvaluative` flag from story metadata
4. Modify quiz answer submission to call backend (silently, no immediate feedback)
5. Ensure quiz answers cannot be skipped (already enforced by story flow)

**New Signals:**
```typescript
private sessionId = signal<Guid | null>(null);
private quizAnswers = signal<Map<string, { answerId: string; isCorrect: boolean }>>(new Map());
private isEvaluative = signal<boolean>(false);
```

**On Story Load:**
```typescript
ngOnInit() {
  // ... existing code ...
  
  // Check if story is evaluative
  this.storiesApi.getStoryById(this.storyId, this.locale).subscribe(story => {
    this.isEvaluative.set(story.isEvaluative ?? false);
    
    // Initialize or restore session ID for evaluative stories
    if (this.isEvaluative()) {
      // Try to restore session from localStorage (extend continuation mechanism)
      const storedSessionId = localStorage.getItem(`story-session-${this.storyId}`);
      if (storedSessionId) {
        this.sessionId.set(storedSessionId);
      } else {
        const newSessionId = crypto.randomUUID();
        this.sessionId.set(newSessionId);
        localStorage.setItem(`story-session-${this.storyId}`, newSessionId);
      }
    }
  });
}
```

**On Quiz Answer Selection:**
```typescript
onQuizAnswerSelected(tileId: string, answerId: string): void {
  // ... existing UI update code (allow navigation after selection) ...
  
  // If evaluative, submit to backend silently (no immediate feedback)
  if (this.isEvaluative() && this.sessionId()) {
    this.storiesApi.submitQuizAnswer(
      this.storyId, 
      tileId, 
      answerId, 
      this.sessionId()!
    ).subscribe({
      next: (response) => {
        if (response.success) {
          // Store answer locally for internal tracking
          this.quizAnswers.update(answers => {
            const newMap = new Map(answers);
            newMap.set(tileId, {
              answerId: answerId,
              isCorrect: response.isCorrect
            });
            return newMap;
          });
        }
      },
      error: (error) => {
        console.error('Failed to submit quiz answer:', error);
        // Don't block user flow on error
      }
    });
  }
}
```

### 3.2 Story Completion Handler

**On Story Complete:**
```typescript
onStoryComplete(): void {
  if (this.isEvaluative() && this.sessionId()) {
    // Complete evaluation
    this.storiesApi.completeEvaluation(this.storyId, this.sessionId()!)
      .subscribe(response => {
        if (response.success) {
          // Navigate to evaluation results page (separate route)
          this.router.navigate(['/stories', this.storyId, 'evaluation-results'], {
            state: { evaluationResult: response }
          });
          
          // Clear session from localStorage
          localStorage.removeItem(`story-session-${this.storyId}`);
        } else {
          // Fallback to normal completion on error
          this.handleNormalCompletion();
        }
      });
  } else {
    // Normal completion (existing flow)
    this.handleNormalCompletion();
  }
}
```

### 3.3 Evaluation Results Component

**New Component:** `evaluation-results.component.ts` (separate route: `/stories/:storyId/evaluation-results`)

**Template:**
```html
<div class="evaluation-results-page">
  <div class="results-card">
    <h2>{{ 'evaluation_results_title' | translate }}</h2>
    
    <!-- Score Display -->
    <div class="score-section">
      <div class="score-circle" [class.excellent]="scorePercentage() >= 90"
                              [class.good]="scorePercentage() >= 70 && scorePercentage() < 90"
                              [class.needs-improvement]="scorePercentage() < 70">
        <span class="score-number">{{ scorePercentage() }}%</span>
      </div>
      <p class="score-label">
        {{ correctAnswers() }} / {{ totalQuizzes() }} {{ 'evaluation_correct' | translate }}
      </p>
    </div>
    
    <!-- Quiz Breakdown -->
    <div class="quiz-breakdown">
      <h3>{{ 'evaluation_breakdown_title' | translate }}</h3>
      <div class="quiz-list">
        <div *ngFor="let quiz of quizDetails()" 
             class="quiz-item" 
             [class.correct]="quiz.isCorrect"
             [class.incorrect]="!quiz.isCorrect">
          <div class="quiz-header">
            <span class="quiz-icon">{{ quiz.isCorrect ? '✓' : '✗' }}</span>
            <span class="quiz-question">{{ quiz.question }}</span>
          </div>
          <div class="quiz-answer">
            <p class="selected-answer">
              <strong>{{ 'evaluation_your_answer' | translate }}:</strong> 
              {{ quiz.selectedAnswerText }}
            </p>
            <p *ngIf="!quiz.isCorrect" class="correct-answer">
              <strong>{{ 'evaluation_correct_answer' | translate }}:</strong> 
              {{ quiz.correctAnswerText }}
            </p>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Action Buttons -->
    <div class="actions">
      <button class="btn-primary" (click)="onRetry()">
        {{ 'evaluation_retry' | translate }}
      </button>
      <button class="btn-secondary" (click)="onClose()">
        {{ 'evaluation_close' | translate }}
      </button>
    </div>
  </div>
</div>
```

**Component Logic:**
```typescript
@Component({
  selector: 'app-evaluation-results',
  standalone: true,
  imports: [CommonModule, TranslateModule, RouterModule],
  templateUrl: './evaluation-results.component.html',
  styleUrl: './evaluation-results.component.css'
})
export class EvaluationResultsComponent implements OnInit {
  storyId = signal<string>('');
  scorePercentage = signal(0);
  correctAnswers = signal(0);
  totalQuizzes = signal(0);
  quizDetails = signal<QuizAnswerDetail[]>([]);
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location
  ) {}
  
  ngOnInit(): void {
    // Get storyId from route
    this.storyId.set(this.route.snapshot.paramMap.get('storyId') || '');
    
    // Get evaluation result from navigation state
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras?.state as { evaluationResult?: CompleteEvaluationResponse };
    
    if (state?.evaluationResult) {
      this.loadResults(state.evaluationResult);
    } else {
      // If no state, redirect back to story
      this.router.navigate(['/stories', this.storyId()]);
    }
  }
  
  private loadResults(result: CompleteEvaluationResponse): void {
    this.scorePercentage.set(result.scorePercentage);
    this.correctAnswers.set(result.correctAnswers);
    this.totalQuizzes.set(result.totalQuizzes);
    this.quizDetails.set(result.quizDetails);
  }
  
  onRetry(): void {
    // Clear any session data and restart story
    localStorage.removeItem(`story-session-${this.storyId()}`);
    this.router.navigate(['/stories', this.storyId(), 'read']);
  }
  
  onClose(): void {
    // Navigate back to story details or home
    this.router.navigate(['/stories', this.storyId()]);
  }
}
```

---

## Phase 4: Story Editor Integration ⏳ PENDING (Backend Ready ✅)

### 4.1 Add Evaluation Toggle in Story Editor ⏳ PENDING

**Backend Status:** ✅ Ready - `IsEvaluative` is already supported in `EditableStoryDto` and saved via `StoryEditorService`

**File:** `story-basic-info-tab.component.html` (Frontend - to be implemented)

**Add checkbox:**
```html
<div class="form-label">
  <label class="checkbox-label">
    <input 
      type="checkbox" 
      [checked]="isEvaluative()"
      (change)="onIsEvaluativeChange($event)"
      [disabled]="isReadOnly()" />
    <span>{{ 'storyeditor_is_evaluative' | translate }}</span>
  </label>
  <p class="form-hint">{{ 'storyeditor_is_evaluative_hint' | translate }}</p>
</div>
```

**Component Logic:**
```typescript
isEvaluative = signal<boolean>(false);

onIsEvaluativeChange(event: Event): void {
  const checked = (event.target as HTMLInputElement).checked;
  this.isEvaluative.set(checked);
  this.onStoryMetadataChange();
}

// Include in save payload
getStoryMetadata(): EditableStory {
  return {
    // ... existing fields ...
    isEvaluative: this.isEvaluative()
  };
}
```

### 4.2 Validation in Story Editor ✅ IMPLEMENTED

**Validation Rules:**
- ✅ No validation for `isEvaluative` flag - it's creator's responsibility
- ✅ Stories can have quizzes without being evaluative (they just give tokens as before)
- ✅ If `isEvaluative = false`, no warnings or suggestions are shown

### 4.3 Update Story Save Endpoint ✅ IMPLEMENTED

**Backend:** `StoryEditorService.SaveDraftAsync`

**Implementation:**
```csharp
// Update StoryCraft
craft.IsEvaluative = dto.IsEvaluative;
```

**Status:** ✅ Already implemented - `IsEvaluative` is saved from `EditableStoryDto` to `StoryCraft` when saving draft.

### 4.4 Add IsCorrect Support in Editor ⏳ PENDING (Backend Ready ✅)

**Backend Status:** ✅ Ready - `IsCorrect` is already supported in `EditableAnswerDto` and saved via `StoryAnswerUpdater`

**Frontend:** Need to add UI controls in story editor to mark answers as correct/incorrect for quiz tiles.

---

## Phase 5: UI/UX Enhancements

### 5.1 Quiz Answer Feedback

**During Reading:**
- No immediate feedback during reading (answers are tracked silently)
- Feedback is shown only at the end in the results screen

**Visual Indicators:**
```css
.quiz-answer.correct {
  border: 2px solid #10b981;
  background: rgba(16, 185, 129, 0.1);
}

.quiz-answer.incorrect {
  border: 2px solid #ef4444;
  background: rgba(239, 68, 68, 0.1);
}
```

### 5.2 Progress Indicator

**Show quiz progress during reading:**
- Optional: "Quiz 1 of 4" indicator (can be added later if needed)
- Progress bar showing completed quizzes (optional enhancement)
- Only visible for evaluative stories

### 5.3 Results Screen Styling

**Score Display:**
- Large circular score display (0-100%)
- Color coding:
  - 90-100%: Green (Excellent)
  - 70-89%: Yellow (Good)
  - <70%: Orange (Needs Improvement)
- Animated score reveal
- Confetti effect for perfect scores (100%) - optional

**Quiz Breakdown:**
- List of all quizzes with:
  - Question text
  - Selected answer (marked with ✓ or ✗)
  - Correct answer (shown only if incorrect)
- Visual distinction between correct/incorrect answers

---

## Phase 6: Translations

### 6.1 Add Translation Keys

**File:** `ro-RO.json`, `en-US.json`, `hu-HU.json`

```json
{
  "storyeditor_is_evaluative": "Poveste evaluativă",
  "storyeditor_is_evaluative_hint": "Dacă este activată, utilizatorii vor fi evaluați pe baza răspunsurilor la quiz-uri",
  
  "evaluation_results_title": "Rezultate Evaluare",
  "evaluation_correct": "corecte",
  "evaluation_retry": "Încearcă din nou",
  "evaluation_close": "Închide",
  "evaluation_breakdown_title": "Detalii Răspunsuri",
  "evaluation_your_answer": "Răspunsul tău",
  "evaluation_correct_answer": "Răspunsul corect",
  
  "evaluation_score_excellent": "Excelent!",
  "evaluation_score_good": "Bun!",
  "evaluation_score_needs_improvement": "Mai multă practică necesară"
}
```

---

## Phase 7: Testing & Edge Cases

### 7.1 Test Scenarios

1. **Normal Story (non-evaluative):**
   - Quiz answers not tracked
   - No evaluation results screen
   - Normal completion flow

2. **Evaluative Story - First Attempt:**
   - All quiz answers tracked silently
   - Score calculated correctly at completion
   - Results screen shown with breakdown
   - All attempts stored

3. **Evaluative Story - Retry:**
   - New session ID generated (fresh start)
   - Previous answers not shown (no influence)
   - New score calculated
   - All attempts stored (no limit)

4. **Partial Completion:**
   - User must answer all quizzes to proceed (quizzes are part of story flow)
   - Answers saved as user progresses
   - Can resume later with same session ID from localStorage

5. **Story Without Quizzes:**
   - Can mark as evaluative (no validation)
   - No warnings shown (creator's responsibility)

### 7.2 Edge Cases

1. **User answers same quiz twice (goes back and changes answer):**
   - Allow overwrite (update existing `StoryQuizAnswer` for same session)
   - Only last answer is stored

2. **Story modified after user started:**
   - Use current story version at completion time (not snapshot from start)
   - Evaluation uses quiz structure from story version at completion

3. **Network failure during answer submission:**
   - Retry mechanism
   - Queue answers locally, sync when online (optional enhancement)

4. **Multiple tabs/devices:**
   - Session ID stored in localStorage per story
   - Each tab/device gets its own session ID
   - Session persists across browser restarts

---

## Phase 8: Implementation Order

### ✅ Step 1: Database & Entities (Backend) - COMPLETED
1. ✅ Create migration script `V0016__add_story_evaluation_system.sql`
2. ✅ Add `IsEvaluative` to `StoryDefinition` entity
3. ✅ Add `IsEvaluative` to `StoryCraft` entity
4. ✅ Add `IsCorrect` to `StoryAnswer` entity
5. ✅ Add `IsCorrect` to `StoryCraftAnswer` entity
6. ✅ Create `StoryQuizAnswer` entity
7. ✅ Create `StoryEvaluationResult` entity
8. ✅ Update `XooDbContext` with new entities and configurations

### ✅ Step 2: Backend API Endpoints - COMPLETED
1. ✅ Implement `SubmitQuizAnswerEndpoint`
2. ✅ Implement `CompleteEvaluationEndpoint`
3. ⏳ `GetEvaluationHistoryEndpoint` - Not implemented (optional for MVP)
4. ✅ Add validation logic

### ✅ Step 3: Backend Editor Integration - COMPLETED
1. ✅ Add `IsEvaluative` to `EditableStoryDto`
2. ✅ Add `IsCorrect` to `EditableAnswerDto`
3. ✅ Update `StoryEditorService.SaveDraftAsync` to save `IsEvaluative`
4. ✅ Update `StoryAnswerUpdater` to save `IsCorrect`
5. ✅ Update `StoriesService.GetStoryForEditAsync` to load both fields
6. ✅ Update `StoryPublishingService` to copy fields on publish

### ✅ Step 4: Backend Reading Integration - COMPLETED
1. ✅ Add `IsEvaluative` to `StoryContentDto`
2. ✅ Add `IsCorrect` to `StoryAnswerDto`
3. ✅ Update `StoryDefinitionMapper` to include both fields in DTOs

### ⏳ Step 5: Story Editor Integration (Frontend) - PENDING
1. ⏳ Add `isEvaluative` checkbox in story editor
2. ⏳ Add `isCorrect` toggle for each answer in quiz tiles
3. ✅ Backend save/load logic already supports these fields
4. ⏳ Add translations

### ⏳ Step 6: Story Reading Integration (Frontend) - PENDING
1. ⏳ Update `story-reading.component.ts` to track session (extend existing continuation mechanism)
2. ⏳ Add quiz answer submission on selection (silent, no immediate feedback)
3. ⏳ Add evaluation completion handler (navigate to separate results route)
4. ⏳ Create `evaluation-results.component` (separate route, no breaking changes)
5. ⏳ Add route for evaluation results page

### ⏳ Step 7: UI Polish - PENDING
1. ⏳ Add quiz progress indicator (optional)
2. ⏳ Style results screen
3. ⏳ Add animations

### ⏳ Step 8: Testing - PENDING
1. ⏳ Test all scenarios
2. ⏳ Test edge cases
3. ⏳ Performance testing
4. ⏳ User acceptance testing

---

## Phase 9: Future Enhancements

### 9.1 Advanced Features (Post-MVP)

1. **Detailed Answer Review:**
   - Show which questions were answered correctly/incorrectly
   - Show correct answers for wrong questions
   - Review mode after completion

2. **Time Tracking:**
   - Track time spent on each quiz
   - Show average time per question
   - Speed bonus for fast correct answers

3. **Difficulty Levels:**
   - Assign difficulty to each quiz
   - Weighted scoring (hard questions worth more)
   - Adaptive difficulty

4. **Leaderboards:**
   - Compare scores with other users
   - Best scores per story
   - Achievement badges

5. **Analytics:**
   - Most missed questions
   - Average scores per story
   - Learning progress tracking

---

## Summary

This implementation plan provides a robust, step-by-step approach to adding evaluation capabilities to stories. The system:

### ✅ Backend Implementation Status

**Completed:**
- ✅ Marks stories as evaluative via `IsEvaluative` flag (both `StoryDefinition` and `StoryCraft`)
- ✅ Stores `IsCorrect` flag on answers (both `StoryAnswer` and `StoryCraftAnswer`)
- ✅ Tracks individual quiz answers during reading via `StoryQuizAnswer` entity
- ✅ Calculates scores based on correct/incorrect answers (simple formula)
- ✅ Returns results with breakdown per quiz via `CompleteEvaluationEndpoint`
- ✅ Stores all evaluation attempts via `StoryEvaluationResult` entity (no limit)
- ✅ Supports multiple attempts/retries (fresh start each time via new SessionId)
- ✅ Extends existing story reading continuation mechanism for session management
- ✅ No breaking changes - all changes are additive
- ✅ No validation in editor - creator's responsibility
- ✅ Allows answer overwrite if user goes back and changes answer
- ✅ Uses current story version at completion time
- ✅ Full editor support - `IsEvaluative` and `IsCorrect` saved/loaded from `StoryCraft`
- ✅ Full publishing support - fields copied from `StoryCraft` to `StoryDefinition` on publish
- ✅ Full reading support - `IsEvaluative` and `IsCorrect` exposed in DTOs

**Backend Files Created/Modified:**
- ✅ `StoryDefinition.cs` - Added `IsEvaluative`
- ✅ `StoryCraft.cs` - Added `IsEvaluative` and `IsCorrect` to `StoryCraftAnswer`
- ✅ `StoryAnswer.cs` - Added `IsCorrect`
- ✅ `StoryQuizAnswer.cs` - New entity
- ✅ `StoryEvaluationResult.cs` - New entity
- ✅ `XooDbContext.cs` - Added DbSets and configurations
- ✅ `V0016__add_story_evaluation_system.sql` - Migration script
- ✅ `SubmitQuizAnswerEndpoint.cs` - New endpoint
- ✅ `CompleteEvaluationEndpoint.cs` - New endpoint
- ✅ `EditableStoryDto` - Added `IsEvaluative`
- ✅ `EditableAnswerDto` - Added `IsCorrect`
- ✅ `StoryContentDto` - Added `IsEvaluative`
- ✅ `StoryAnswerDto` - Added `IsCorrect`
- ✅ `StoryDefinitionMapper` - Updated to map new fields
- ✅ `StoryEditorService` - Updated to save `IsEvaluative`
- ✅ `StoryAnswerUpdater` - Updated to save `IsCorrect`
- ✅ `StoryPublishingService` - Updated to copy fields on publish
- ✅ `StoriesService` - Updated to load fields in editor
- ✅ Seed data DTOs - Updated to support new fields

### ⏳ Frontend Implementation Status

**Pending:**
- ⏳ Story editor UI - Add checkbox for `isEvaluative` and toggles for `isCorrect` on answers
- ⏳ Story reading - Track session, submit answers, complete evaluation
- ⏳ Evaluation results component - Display score and breakdown
- ⏳ Translations - Add all evaluation-related keys

The backend implementation is complete and ready for frontend integration. All changes are additive and maintain backward compatibility with existing non-evaluative stories. Best score tracking and complex token configurations will be added later in parent dashboard.

