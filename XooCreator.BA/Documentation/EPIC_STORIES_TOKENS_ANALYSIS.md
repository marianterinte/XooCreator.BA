# Analiza: Tokenii nu sunt persistați pentru Epic Stories

## Problema
Tokenii de personalitate oferiți ca reward în povestile din epic-uri dinamice nu sunt stocați în DB și nu sunt reflectați în Tree of Heroes, în timp ce pentru Tree of Light stories funcționează corect.

## Analiza diferențelor

### Tree of Light Stories (FUNCȚIONEAZĂ)

#### Frontend (FE)
1. **story-reading.component.ts** (linia 1171, 1183):
   - Trimite tokens la `stateService.resolveStory()`: `tokens && tokens.length ? tokens : undefined`
   - Tokenii sunt colectați din quiz answers în timpul citirii

2. **progress.service.ts** (linia 44-76):
   - Metoda `resolveStory()` primește `tokens?: TokenReward[]`
   - Trimite tokens la API: `this.apiService.completeStory({ storyId, selectedAnswer, tokens }, configId)`

3. **tree-of-light-api.service.ts**:
   - `completeStory()` trimite tokens în request body

#### Backend (BA)
1. **CompleteStoryRequest** (TreeOfLightDtos.cs):
   - Nu are explicit câmp pentru tokens în request, dar tokens sunt extrași din story JSON

2. **TreeOfLightService.cs** (linia 63-108):
   - `CompleteStoryAsync()` extrage tokenii din story JSON (din quiz answer):
     ```csharp
     var selectedAnswer = quizTile.Answers.FirstOrDefault(a => a.Id == request.SelectedAnswer);
     if (selectedAnswer != null && selectedAnswer.Tokens.Count > 0)
     {
         effectiveTokens.AddRange(selectedAnswer.Tokens);
     }
     ```
   - Acordă tokenii: `await _treeOfHeroesRepository.AwardTokensAsync(userId, effectiveTokens);`

### Epic Stories (NU FUNCȚIONEAZĂ)

#### Frontend (FE)
1. **story-reading.component.ts** (linia 1168):
   - Apelează `completeEpicStoryAndFinish(epicInfo.epicId, selectedAnswer ?? undefined, tokens)`
   - Tokenii sunt colectați dar **NU sunt trimiși la API**

2. **story-epic.service.ts** (linia 206-216):
   - Metoda `completeEpicStory()` **NU acceptă tokens**:
     ```typescript
     completeEpicStory(epicId: string, storyId: string, selectedAnswer?: string)
     ```
   - Trimite doar: `{ selectedAnswer: selectedAnswer || null }`

#### Backend (BA)
1. **CompleteEpicStoryRequest** (CompleteEpicStoryEndpoint.cs, linia 65-68):
   - **NU are câmp pentru tokens**:
     ```csharp
     public record CompleteEpicStoryRequest
     {
         public string? SelectedAnswer { get; init; }
     }
     ```

2. **StoryEpicProgressService.cs** (linia 98-160):
   - Metoda `CompleteStoryAsync()` **NU procesează tokenii deloc**
   - Doar marchează povestea ca completată și deblochează regiuni/eroi

## Concluzie

**Problema principală:** Epic stories nu trimit și nu procesează tokenii deloc, în timp ce Tree of Light extrage tokenii din story JSON și îi acordă utilizatorului.

**Diferențe cheie:**
1. Tree of Light extrage tokenii din story JSON (hardcoded în story definition)
2. Epic stories ar trebui să primească tokenii din FE (din quiz answers colectați în timpul citirii)
3. Epic stories nu trimit tokenii la API
4. Epic stories nu procesează tokenii în backend

## Soluție propusă

1. **Backend:**
   - Adăugare `List<TokenReward>? Tokens` în `CompleteEpicStoryRequest`
   - Modificare `StoryEpicProgressService.CompleteStoryAsync()` să proceseze tokens similar cu Tree of Light
   - Apelare `_treeOfHeroesRepository.AwardTokensAsync()` pentru a salva tokenii

2. **Frontend:**
   - Modificare `story-epic.service.ts` să accepte și să trimită tokens
   - Modificare `story-reading.component.ts` să trimită tokens la `completeEpicStory()`

## Note importante

- Tokenii pentru Tree of Light sunt extrași din story JSON (hardcoded)
- Tokenii pentru Epic stories ar trebui să fie trimiși din FE (din quiz answers colectați)
- Ambele ar trebui să folosească același mecanism de salvare: `TreeOfHeroesRepository.AwardTokensAsync()`

