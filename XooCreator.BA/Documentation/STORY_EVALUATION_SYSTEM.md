# Story Evaluation System - Implementation Plan

## Overview

This document outlines the complete implementation plan for adding an evaluation/scoring system to stories that contain quiz questions. When a story is marked as "evaluative", users will be scored based on their quiz answers, and results will be displayed at the end of the story.

## Goals

1. **Mark stories as evaluative** - Add a flag to identify stories that should be evaluated
2. **Track quiz answers** - Store individual quiz answers during story reading
3. **Calculate scores** - Compute final score based on correct/incorrect answers
4. **Display results** - Show evaluation results screen at story completion
5. **Store best scores** - Keep track of user's best performance per story

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
/// One record per story completion attempt. Best score is tracked separately.
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
    public bool IsBestScore { get; set; } = false; // True if this is the user's best score for this story
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}
```

**Indexes:**
- `(UserId, StoryId, CompletedAt)` - For querying user's history
- `(UserId, StoryId, ScorePercentage DESC)` - For finding best score
- Unique constraint: `(UserId, StoryId, SessionId)` - One result per session

### 1.4 Migration Script

**File:** `V0011__add_story_evaluation_system.sql`

```sql
BEGIN;

-- Add IsEvaluative flag to StoryDefinitions
ALTER TABLE alchimalia_schema."StoryDefinitions"
    ADD COLUMN IF NOT EXISTS "IsEvaluative" boolean NOT NULL DEFAULT false;

-- Create StoryQuizAnswer table
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryQuizAnswers" (
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
    ON alchimalia_schema."StoryQuizAnswers" ("UserId", "StoryId", "TileId", "SessionId");

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_UserStory" 
    ON alchimalia_schema."StoryQuizAnswers" ("UserId", "StoryId");

CREATE INDEX IF NOT EXISTS "IX_StoryQuizAnswers_Session" 
    ON alchimalia_schema."StoryQuizAnswers" ("SessionId");

-- Create StoryEvaluationResult table
CREATE TABLE IF NOT EXISTS alchimalia_schema."StoryEvaluationResults" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StoryId" text NOT NULL,
    "SessionId" uuid NOT NULL,
    "TotalQuizzes" integer NOT NULL,
    "CorrectAnswers" integer NOT NULL,
    "ScorePercentage" integer NOT NULL,
    "CompletedAt" timestamp with time zone NOT NULL,
    "IsBestScore" boolean NOT NULL DEFAULT false,
    CONSTRAINT "PK_StoryEvaluationResults" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StoryEvaluationResults_AlchimaliaUsers_UserId" 
        FOREIGN KEY ("UserId") REFERENCES alchimalia_schema."AlchimaliaUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StoryEvaluationResults_UserStorySession" 
        UNIQUE ("UserId", "StoryId", "SessionId")
);

CREATE INDEX IF NOT EXISTS "IX_StoryEvaluationResults_UserStoryCompleted" 
    ON alchimalia_schema."StoryEvaluationResults" ("UserId", "StoryId", "CompletedAt");

CREATE INDEX IF NOT EXISTS "IX_StoryEvaluationResults_UserStoryScore" 
    ON alchimalia_schema."StoryEvaluationResults" ("UserId", "StoryId", "ScorePercentage" DESC);

COMMIT;
```

---

## Phase 2: Backend API

### 2.1 Submit Quiz Answer Endpoint

**Endpoint:** `POST /api/{locale}/stories/{storyId}/quiz-answer`

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
4. Create or update `StoryQuizAnswer` record
5. Generate `SessionId` if not provided (new reading session)
6. Return result with `IsCorrect` flag

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
    
    // Save answer
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
    await ep._context.SaveChangesAsync(ct);
    
    return TypedResults.Ok(new SubmitQuizAnswerResponse
    {
        Success = true,
        IsCorrect = isCorrect,
        SessionId = sessionId
    });
}
```

### 2.2 Complete Evaluative Story Endpoint

**Endpoint:** `POST /api/{locale}/stories/{storyId}/complete-evaluation`

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
    public bool IsBestScore { get; init; }     // True if this is user's best score
    public int? PreviousBestScore { get; init; } // Previous best score (if exists)
    public string? ErrorMessage { get; init; }
}
```

**Logic:**
1. Load story and verify `IsEvaluative == true`
2. Count total quiz tiles in story
3. Query all `StoryQuizAnswer` records for this `SessionId`
4. Count correct answers (`IsCorrect == true`)
5. Calculate score: `(CorrectAnswers / TotalQuizzes) * 100`
6. Check if this is best score (query `StoryEvaluationResult` for user+story)
7. Create `StoryEvaluationResult` record
8. Update `IsBestScore` flag if this is new best score
9. Return evaluation results

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
    
    // Check previous best score
    var previousBest = await ep._context.StoryEvaluationResults
        .Where(r => r.UserId == userId && r.StoryId == storyId && r.IsBestScore)
        .OrderByDescending(r => r.ScorePercentage)
        .FirstOrDefaultAsync(ct);
    
    var isBestScore = previousBest == null || scorePercentage > previousBest.ScorePercentage;
    
    // If this is new best, unmark previous best
    if (isBestScore && previousBest != null)
    {
        previousBest.IsBestScore = false;
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
        IsBestScore = isBestScore,
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
        IsBestScore = isBestScore,
        PreviousBestScore = previousBest?.ScorePercentage
    });
}
```

### 2.3 Get Evaluation History Endpoint

**Endpoint:** `GET /api/{locale}/stories/{storyId}/evaluation-history`

**Response:**
```csharp
public record EvaluationHistoryResponse
{
    public List<EvaluationHistoryItem> Attempts { get; init; } = new();
    public int? BestScore { get; init; }
    public int TotalAttempts { get; init; }
}

public record EvaluationHistoryItem
{
    public Guid SessionId { get; init; }
    public int ScorePercentage { get; init; }
    public int CorrectAnswers { get; init; }
    public int TotalQuizzes { get; init; }
    public DateTime CompletedAt { get; init; }
    public bool IsBestScore { get; init; }
}
```

---

## Phase 3: Frontend - Story Reading

### 3.1 Update Story Reading Component

**File:** `story-reading.component.ts`

**Changes:**
1. Add `sessionId` signal to track reading session
2. Add `quizAnswers` signal to track answers during reading
3. Add `isEvaluative` flag from story metadata
4. Modify quiz answer submission to call backend

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
    
    // Initialize session ID for evaluative stories
    if (this.isEvaluative()) {
      this.sessionId.set(crypto.randomUUID());
    }
  });
}
```

**On Quiz Answer Selection:**
```typescript
onQuizAnswerSelected(tileId: string, answerId: string): void {
  // ... existing UI update code ...
  
  // If evaluative, submit to backend
  if (this.isEvaluative() && this.sessionId()) {
    this.storiesApi.submitQuizAnswer(
      this.storyId, 
      tileId, 
      answerId, 
      this.sessionId()!
    ).subscribe(response => {
      if (response.success) {
        // Store answer locally
        this.quizAnswers.update(answers => {
          const newMap = new Map(answers);
          newMap.set(tileId, {
            answerId: answerId,
            isCorrect: response.isCorrect
          });
          return newMap;
        });
        
        // Show immediate feedback if needed
        if (response.isCorrect) {
          // Show success indicator
        } else {
          // Show incorrect indicator
        }
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
          // Show evaluation results screen
          this.showEvaluationResults(response);
        }
      });
  } else {
    // Normal completion (existing flow)
    this.handleNormalCompletion();
  }
}
```

### 3.3 Evaluation Results Component

**New Component:** `evaluation-results.component.ts`

**Template:**
```html
<div class="evaluation-results-modal" *ngIf="isVisible()">
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
    
    <!-- Best Score Badge -->
    <div class="best-score-badge" *ngIf="isBestScore()">
      ⭐ {{ 'evaluation_best_score' | translate }}
    </div>
    
    <!-- Previous Best -->
    <div class="previous-best" *ngIf="previousBestScore() !== null">
      <p>{{ 'evaluation_previous_best' | translate }}: {{ previousBestScore() }}%</p>
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
  imports: [CommonModule, TranslateModule],
  templateUrl: './evaluation-results.component.html',
  styleUrl: './evaluation-results.component.css'
})
export class EvaluationResultsComponent {
  @Input() result: CompleteEvaluationResponse | null = null;
  @Output() retry = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();
  
  isVisible = signal(false);
  scorePercentage = signal(0);
  correctAnswers = signal(0);
  totalQuizzes = signal(0);
  isBestScore = signal(false);
  previousBestScore = signal<number | null>(null);
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['result'] && this.result) {
      this.scorePercentage.set(this.result.scorePercentage);
      this.correctAnswers.set(this.result.correctAnswers);
      this.totalQuizzes.set(this.result.totalQuizzes);
      this.isBestScore.set(this.result.isBestScore);
      this.previousBestScore.set(this.result.previousBestScore ?? null);
      this.isVisible.set(true);
    }
  }
  
  onRetry(): void {
    this.isVisible.set(false);
    this.retry.emit();
  }
  
  onClose(): void {
    this.isVisible.set(false);
    this.close.emit();
  }
}
```

---

## Phase 4: Story Editor Integration

### 4.1 Add Evaluation Toggle in Story Editor

**File:** `story-basic-info-tab.component.html`

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

### 4.2 Validation in Story Editor

**Validation Rules:**
- If `isEvaluative = true`, story MUST have at least one quiz tile
- Show warning if user tries to save evaluative story without quizzes
- Auto-detect quizzes: count tiles with `type === "quiz"`

**Validation Logic:**
```typescript
validateStory(): ValidationResult {
  const errors: string[] = [];
  
  if (this.isEvaluative() && this.tiles().filter(t => t.type === 'quiz').length === 0) {
    errors.push('Evaluative stories must contain at least one quiz');
  }
  
  return { isValid: errors.length === 0, errors };
}
```

### 4.3 Update Story Save Endpoint

**Backend:** `SaveStoryEndpoint.cs`

**Include `IsEvaluative` in save:**
```csharp
// Update StoryDefinition
story.IsEvaluative = dto.IsEvaluative;
```

---

## Phase 5: UI/UX Enhancements

### 5.1 Quiz Answer Feedback

**During Reading:**
- Show immediate feedback when answer is submitted (for evaluative stories)
- Green checkmark for correct, red X for incorrect
- Optional: show correct answer after submission (configurable)

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
- "Quiz 1 of 4" indicator
- Progress bar showing completed quizzes
- Only visible for evaluative stories

### 5.3 Results Screen Styling

**Score Display:**
- Large circular score display (0-100%)
- Color coding:
  - 90-100%: Green (Excellent)
  - 70-89%: Yellow (Good)
  - <70%: Orange (Needs Improvement)
- Animated score reveal
- Confetti effect for perfect scores (100%)

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
  "evaluation_best_score": "Cel mai bun scor!",
  "evaluation_previous_best": "Scor anterior",
  "evaluation_retry": "Încearcă din nou",
  "evaluation_close": "Închide",
  
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
   - All quiz answers tracked
   - Score calculated correctly
   - Results screen shown
   - Best score flag set

3. **Evaluative Story - Retry:**
   - New session ID generated
   - Previous answers not overwritten
   - New score calculated
   - Best score updated if improved

4. **Partial Completion:**
   - User answers some quizzes but doesn't complete story
   - Answers still saved
   - Can resume later (same session or new)

5. **Story Without Quizzes:**
   - Cannot mark as evaluative (validation)
   - Warning shown in editor

### 7.2 Edge Cases

1. **User answers same quiz twice:**
   - Allow overwrite (update existing `StoryQuizAnswer`)
   - Or: prevent re-answering (UI disabled after first answer)

2. **Story modified after user started:**
   - Use version tracking
   - Evaluation uses quiz structure from story version at completion time

3. **Network failure during answer submission:**
   - Retry mechanism
   - Queue answers locally, sync when online

4. **Multiple tabs/devices:**
   - Session ID should be unique per reading session
   - Each tab gets new session ID

---

## Phase 8: Implementation Order

### Step 1: Database & Entities (Backend)
1. Create migration script `V0011__add_story_evaluation_system.sql`
2. Add `IsEvaluative` to `StoryDefinition` entity
3. Create `StoryQuizAnswer` entity
4. Create `StoryEvaluationResult` entity
5. Update `XooDbContext`

### Step 2: Backend API Endpoints
1. Implement `SubmitQuizAnswerEndpoint`
2. Implement `CompleteEvaluationEndpoint`
3. Implement `GetEvaluationHistoryEndpoint`
4. Add validation logic

### Step 3: Story Editor Integration (Frontend)
1. Add `isEvaluative` checkbox in story editor
2. Add validation for evaluative stories
3. Update save/load logic
4. Add translations

### Step 4: Story Reading Integration (Frontend)
1. Update `story-reading.component.ts` to track session
2. Add quiz answer submission on selection
3. Add evaluation completion handler
4. Create `evaluation-results.component`

### Step 5: UI Polish
1. Add quiz progress indicator
2. Add answer feedback styling
3. Style results screen
4. Add animations

### Step 6: Testing
1. Test all scenarios
2. Test edge cases
3. Performance testing
4. User acceptance testing

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

- ✅ Marks stories as evaluative via `IsEvaluative` flag
- ✅ Tracks individual quiz answers during reading
- ✅ Calculates scores based on correct/incorrect answers
- ✅ Displays results screen at completion
- ✅ Stores best scores per user per story
- ✅ Supports multiple attempts/retries
- ✅ Integrates seamlessly with existing story reading flow
- ✅ Provides clear feedback to users
- ✅ Validates data integrity

The implementation is incremental, allowing for testing at each phase, and maintains backward compatibility with existing non-evaluative stories.

