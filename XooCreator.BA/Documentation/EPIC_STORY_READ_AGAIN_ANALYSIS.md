# Analiza: Funcționalitatea "Read Again" pentru Epic Stories

## Notă Importantă
**Status**: Nu suntem încă în producție, deci putem face modificări directe fără restricții de backward compatibility sau migrații complexe. Putem suprascrie orice este necesar.

## Context

Utilizatorii pot completa story-uri în epic-uri dinamice. Când un story este completat, la click pe story-ul completat în epic player, se afișează un modal (ResultModalComponent) cu card flip care arată:
- **Front**: Imaginea mare (cover image)
- **Back**: Detalii despre story (nume, descriere)

**Cerință nouă**: Adăugarea unui buton "Read Again" care să permită utilizatorului să recitească story-ul cu două opțiuni:
1. **Read Again fără reset rewards** - quiz-urile sunt readonly (afișează răspunsurile date anterior)
2. **Read Again cu reset rewards** - resetează progresul, tokenii acordați, și permite răspunsuri noi

## Analiza Impactului

### 1. Datele care trebuie gestionate

#### A. Epic Story Progress
- **Tabel**: `EpicStoryProgress`
- **Câmpuri relevante**: `StoryId`, `SelectedAnswer`, `TokensJson`, `CompletedAt`, `EpicId`
- **Acțiune**: Ștergere înregistrare pentru story-ul specific

#### B. Quiz Answers
- **Tabel**: `StoryQuizAnswers`
- **Câmpuri relevante**: `StoryId`, `TileId`, `SelectedAnswerId`, `IsCorrect`, `SessionId`
- **Acțiune**: 
  - **Fără reset**: Păstrare, dar quiz-urile devin readonly
  - **Cu reset**: Ștergere toate răspunsurile pentru story-ul respectiv

#### C. Token Rewards (Tree of Heroes)
- **Tabel**: `UserTokenBalances`
- **Câmpuri relevante**: `Type`, `Value`, `Quantity`
- **Acțiune**: 
  - **Fără reset**: Păstrare tokenii
  - **Cu reset**: Decrementare tokenii acordați pentru story-ul respectiv
  - **Problema**: Trebuie să știm exact ce tokeni au fost acordați pentru acest story

#### D. Story Read Progress
- **Tabel**: `UserStoryReadProgress`
- **Câmpuri relevante**: `StoryId`, `TileId`
- **Acțiune**: 
  - **Fără reset**: Păstrare (pentru a permite navigare la tile-uri deja citite)
  - **Cu reset**: Ștergere progresul pentru story-ul respectiv

#### E. Story Evaluation Results (dacă story-ul este evaluative)
- **Tabel**: `StoryEvaluationResults`
- **Acțiune**: 
  - **Fără reset**: Păstrare
  - **Cu reset**: Ștergere rezultatul evaluării

### 2. Probleme Identificate

#### Problema 1: Tracking Token Rewards per Story
**Situația actuală**: 
- Tokenii sunt acordați prin `TreeOfHeroesRepository.AwardTokensAsync()`
- Nu există un mecanism de tracking care să asocieze tokenii acordați cu story-ul completat
- `EpicStoryProgress.TokensJson` există dar nu este populat în implementarea actuală

**Soluție necesară** (simplificată - nu suntem în prod):
- **Modificare directă**: Populare `TokensJson` în `EpicStoryProgress` când se completează story-ul
- Nu este nevoie de tabel nou - putem folosi câmpul existent `TokensJson`
- Modificăm `CompleteEpicStoryEndpoint` și `StoryEpicProgressService` să salveze tokenii în `TokensJson`

#### Problema 2: Decrementare Tokeni
**Situația actuală**:
- `TreeOfHeroesRepository` are `SpendTokensAsync()` pentru decrementare
- Dar trebuie să decrementăm exact tokenii acordați pentru story-ul respectiv
- Dacă utilizatorul a folosit deja tokenii pentru Tree of Heroes, decrementarea ar putea duce la valori negative

**Soluție necesară**:
- Verificare dacă utilizatorul are suficienți tokeni înainte de decrementare
- Sau: Nu decrementăm dacă tokenii au fost deja folosiți (doar resetăm progresul story-ului)

#### Problema 3: Tree of Heroes Progress
**Situația actuală**:
- Dacă utilizatorul a folosit tokenii pentru a debloca eroi în Tree of Heroes, resetarea tokenilor ar putea afecta progresul
- **Decizie necesară**: Resetăm și progresul Tree of Heroes sau doar progresul story-ului?

#### Problema 4: Quiz Answers Readonly Mode
**Situația actuală**:
- `StoryQuizComponent` nu are suport pentru readonly mode
- Trebuie să afișeze răspunsurile date anterior fără să permită modificare

**Soluție necesară**:
- Adăugare `@Input() readonly: boolean` în `StoryQuizComponent`
- Încărcare răspunsuri existente din `StoryQuizAnswers` când story-ul este în modul "read again fără reset"
- Disable toate butoanele de selecție răspuns

#### Problema 5: Session Management
**Situația actuală**:
- `StoryQuizAnswer` are `SessionId` pentru tracking sesiuni
- La "read again", trebuie să creăm o nouă sesiune sau să folosim sesiunea existentă?

**Soluție necesară**:
- Creare sesiune nouă pentru "read again"
- Sau: Folosire sesiune existentă pentru readonly mode

### 3. Flow-uri de Utilizare

#### Flow 1: Read Again fără Reset Rewards
1. User click pe story completat în epic player
2. Modal se deschide cu card flip (front: imagine, back: detalii)
3. User click pe buton "Read Again"
4. Modal se extinde/actualizează cu opțiuni:
   - "Read Again (Keep Rewards)" - quiz-urile readonly
   - "Read Again (Reset Rewards)" - reset complet
5. User selectează "Keep Rewards"
6. Navigare la story reading cu flag `readAgainKeepRewards=true`
7. Story reading:
   - Încarcă story-ul normal
   - Încarcă quiz answers existente din DB
   - Quiz-urile sunt afișate în mod readonly (răspunsurile selectate anterior sunt evidențiate)
   - User poate naviga prin story dar nu poate modifica răspunsurile
8. La finalizare: Nu se salvează progres nou, nu se acordă tokeni noi

#### Flow 2: Read Again cu Reset Rewards
1. User click pe story completat în epic player
2. Modal se deschide cu card flip
3. User click pe buton "Read Again"
4. Modal se extinde cu opțiuni
5. User selectează "Reset Rewards"
6. **Confirmare**: Modal afișează warning despre consecințe:
   - "Această acțiune va șterge progresul pentru acest story"
   - "Tokenii acordați vor fi eliminați din contul tău"
   - "Dacă ai folosit deja tokenii pentru Tree of Heroes, aceștia vor fi eliminați"
   - "Ești sigur că vrei să continui?"
7. User confirmă
8. Backend:
   - Șterge `EpicStoryProgress` pentru story
   - Șterge `StoryQuizAnswers` pentru story
   - Decrementă tokenii din `UserTokenBalances` (dacă există)
   - Șterge `UserStoryReadProgress` pentru story
   - Șterge `StoryEvaluationResults` pentru story (dacă există)
9. Navigare la story reading cu flag `readAgainResetRewards=true`
10. Story reading funcționează normal (ca la prima citire)

## Arhitectură Propusă

### Backend (BA)

#### 1. Endpoint nou: Reset Epic Story Progress
**Route**: `POST /api/story-epic/{epicId}/stories/{storyId}/reset-progress`
**Request**:
```csharp
public record ResetEpicStoryProgressRequest
{
    public bool ResetRewards { get; init; } // true = reset complet, false = doar pentru re-citire
}
```
**Response**:
```csharp
public record ResetEpicStoryProgressResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TokensRemoved { get; init; } // Număr de tokeni eliminați (pentru feedback)
}
```

#### 2. Repository Method: ResetStoryProgressAsync
**Location**: `IEpicProgressRepository` / `EpicProgressRepository`
**Signature**:
```csharp
Task<bool> ResetStoryProgressAsync(Guid userId, string epicId, string storyId, bool resetRewards);
```

**Logică**:
- Dacă `resetRewards = false`: Doar marchează story-ul ca "available for re-reading" (nu șterge progresul)
- Dacă `resetRewards = true`:
  1. Șterge `EpicStoryProgress` pentru story
  2. Șterge `StoryQuizAnswers` pentru story
  3. Șterge `UserStoryReadProgress` pentru story
  4. Șterge `StoryEvaluationResults` pentru story
  5. **CRITICAL**: Decrementă tokenii din `UserTokenBalances` (trebuie să știm ce tokeni au fost acordați)

#### 3. Service Method: GetStoryTokenRewards
**Location**: `StoryEpicProgressService`
**Purpose**: Obține lista de tokeni acordați pentru un story completat
**Logică**:
- Citește `EpicStoryProgress.TokensJson` (dacă este populat)
- Sau: Query `StoryQuizAnswers` pentru story și extrage tokenii din răspunsurile selectate

#### 4. Service Method: DecrementStoryTokens
**Location**: `StoryEpicProgressService` sau `TreeOfHeroesRepository`
**Purpose**: Decrementă tokenii acordați pentru un story
**Logică**:
- Obține lista de tokeni acordați pentru story
- Pentru fiecare token, decrementează din `UserTokenBalances`
- Verifică dacă există suficienți tokeni (nu permite valori negative)

### Frontend (FE)

#### 1. Extindere ResultModalComponent
**Location**: `creature-builder/modals/result.modal.html`
**Modificări**:
- Adăugare buton "Read Again" pe back side al card-ului (când este story epic)
- Butonul deschide un sub-panel în același modal cu opțiuni

#### 2. Read Again Options Panel
**Component nou sau extindere modal**:
- Buton "Read Again (Keep Rewards)"
- Buton "Read Again (Reset Rewards)"
- Când se selectează "Reset Rewards", se afișează warning și confirmare

#### 3. Story Reading Component Modificări
**Location**: `story-reading/story-reading.component.ts`
**Modificări**:
- Adăugare query param `readAgainKeepRewards` și `readAgainResetRewards`
- Logică pentru readonly mode:
  - Dacă `readAgainKeepRewards=true`: Încarcă quiz answers existente și setează readonly mode
  - Dacă `readAgainResetRewards=true`: Funcționează normal (ca prima citire)

#### 4. Story Quiz Component Modificări
**Location**: `story-reading/components/story-quiz/story-quiz.component.ts`
**Modificări**:
- Adăugare `@Input() readonly: boolean = false`
- Când `readonly=true`:
  - Disable toate butoanele de răspuns
  - Evidențiere răspunsul selectat anterior
  - Nu permite modificare

#### 5. Epic Player Component Modificări
**Location**: `story-epic-player/story-epic-player.component.ts`
**Modificări**:
- Adăugare handler pentru butonul "Read Again" din modal
- Navigare la story cu query params corespunzători

## Baby Steps pentru Implementare

### Faza 1: Preparare Backend - Tracking Token Rewards
**Obiectiv**: Asigură că știm exact ce tokeni au fost acordați pentru fiecare story

**Step 1.1**: Populare `EpicStoryProgress.TokensJson`
- **Fișier**: `EpicProgressRepository.CompleteStoryAsync()` și `StoryEpicProgressService.CompleteStoryAsync()`
- **Modificare**: 
  - Serializează tokenii acordați în JSON
  - Salvează în `EpicStoryProgress.TokensJson` când se completează story-ul
  - **Simplificare**: Putem modifica direct structura, nu avem nevoie de migrații pentru date existente
- **Test**: Verifică că `TokensJson` este populat după completarea unui story

**Step 1.2**: Adăugare helper method pentru deserializare tokeni
- **Fișier**: `EpicProgressRepository` sau `StoryEpicProgressService`
- **Method**: `GetStoryTokenRewardsAsync(Guid userId, string epicId, string storyId)`
- **Logică**: Deserializează `TokensJson` din `EpicStoryProgress`

### Faza 2: Backend - Endpoint Reset Story Progress
**Obiectiv**: Endpoint pentru resetarea progresului unui story

**Step 2.1**: Adăugare DTOs
- **Fișier**: `StoryEpicDtos.cs`
- **Adăugare**: `ResetEpicStoryProgressRequest` și `ResetEpicStoryProgressResponse`

**Step 2.2**: Adăugare repository method
- **Fișier**: `IEpicProgressRepository.cs` și `EpicProgressRepository.cs`
- **Method**: `ResetStoryProgressAsync(Guid userId, string epicId, string storyId, bool resetRewards)`
- **Logică**: 
  - Dacă `resetRewards=false`: Nu face nimic (doar permite re-citire)
  - Dacă `resetRewards=true`: Șterge progresul (fără tokeni pentru moment)

**Step 2.3**: Adăugare service method
- **Fișier**: `IStoryEpicProgressService.cs` și `StoryEpicProgressService.cs`
- **Method**: `ResetStoryProgressAsync(string epicId, Guid userId, string storyId, bool resetRewards, CancellationToken ct)`
- **Logică**: Apelează repository method

**Step 2.4**: Creare endpoint
- **Fișier**: `ResetEpicStoryProgressEndpoint.cs` (nou)
- **Route**: `POST /api/story-epic/{epicId}/stories/{storyId}/reset-progress`
- **Logică**: Validează request, apelează service, returnează response

**Step 2.5**: Test endpoint
- Test cu `resetRewards=false` (nu ar trebui să șteargă nimic)
- Test cu `resetRewards=true` (ar trebui să șteargă progresul)

### Faza 3: Backend - Decrementare Tokeni
**Obiectiv**: Decrementarea tokenilor acordați pentru story

**Step 3.1**: Adăugare method pentru decrementare tokeni per story
- **Fișier**: `TreeOfHeroesRepository.cs`
- **Method**: `DecrementStoryTokensAsync(Guid userId, string storyId, List<TokenReward> tokens)`
- **Logică**: 
  - Pentru fiecare token, decrementează din `UserTokenBalances`
  - Verifică dacă există suficienți tokeni (nu permite valori negative)
  - Returnează numărul de tokeni decrementați cu succes

**Step 3.2**: Integrare în reset progress
- **Fișier**: `StoryEpicProgressService.ResetStoryProgressAsync()`
- **Modificare**: 
  - Obține tokenii acordați pentru story
  - Apelează `DecrementStoryTokensAsync()` dacă `resetRewards=true`
  - Gestionează erorile (dacă nu există suficienți tokeni)

**Step 3.3**: Test decrementare tokeni
- Test cu tokeni suficienți
- Test cu tokeni insuficienți (ar trebui să returneze eroare sau să decrementeze doar cât are)

### Faza 4: Frontend - UI pentru Read Again
**Obiectiv**: Adăugare buton și opțiuni în modal

**Step 4.1**: Extindere ResultModalComponent
- **Fișier**: `result.modal.ts` și `result.modal.html`
- **Modificări**:
  - Adăugare `@Input() showReadAgain: boolean = false`
  - Adăugare `@Input() storyId: string | null = null`
  - Adăugare `@Input() epicId: string | null = null`
  - Adăugare `@Output() readAgain = new EventEmitter<{ keepRewards: boolean }>()`

**Step 4.2**: Adăugare buton "Read Again" în modal
- **Fișier**: `result.modal.html`
- **Location**: Pe back side al card-ului, după textul story-ului
- **Conditional**: Se afișează doar dacă `showReadAgain=true`

**Step 4.3**: Creare Read Again Options Panel
- **Component nou**: `read-again-options.component.ts` (sau inline în modal)
- **UI**: 
  - Buton "Read Again (Keep Rewards)"
  - Buton "Read Again (Reset Rewards)"
  - Când se click pe "Reset Rewards", se afișează warning panel

**Step 4.4**: Integrare în Epic Player
- **Fișier**: `story-epic-player.component.ts` și `story-epic-player.component.html`
- **Modificări**:
  - Pasează `showReadAgain=true` și `storyId`/`epicId` la `result-modal`
  - Handler pentru `(readAgain)` event
  - Navigare la story cu query params corespunzători

### Faza 5: Frontend - Story Reading cu Readonly Mode
**Obiectiv**: Suport pentru re-citire cu quiz-uri readonly

**Step 5.1**: Modificare Story Reading Component
- **Fișier**: `story-reading.component.ts`
- **Modificări**:
  - Citire query params `readAgainKeepRewards` și `readAgainResetRewards`
  - Dacă `readAgainKeepRewards=true`: 
    - Încarcă quiz answers existente din backend
    - Setează flag `readonlyMode=true`
  - Dacă `readAgainResetRewards=true`: Funcționează normal

**Step 5.2**: API pentru obținere quiz answers
- **Fișier**: `stories-api.service.ts`
- **Method**: `getStoryQuizAnswers(storyId: string): Observable<QuizAnswer[]>`
- **Endpoint**: `GET /api/{locale}/stories/{storyId}/quiz-answers`

**Step 5.3**: Modificare Story Quiz Component
- **Fișier**: `story-quiz.component.ts` și `story-quiz.component.html`
- **Modificări**:
  - Adăugare `@Input() readonly: boolean = false`
  - Adăugare `@Input() existingAnswers: Map<string, string> = new Map()` (TileId -> SelectedAnswerId)
  - Când `readonly=true`:
    - Disable toate butoanele de răspuns
    - Evidențiere răspunsul selectat anterior (dacă există)
    - Nu permite modificare

**Step 5.4**: Integrare în Story Reading
- **Fișier**: `story-reading.component.html`
- **Modificări**: Pasează `[readonly]` și `[existingAnswers]` la `story-quiz`

**Step 5.5**: Prevenire salvare progres nou
- **Fișier**: `story-reading.component.ts`
- **Modificări**: 
  - Dacă `readAgainKeepRewards=true`, nu salvează progres nou la finalizare
  - Nu acordă tokeni noi
  - Nu marchează story-ul ca completat din nou

### Faza 6: Frontend - Navigare și State Management
**Obiectiv**: Gestionarea corectă a navigării și state-ului

**Step 6.1**: Navigare la story cu query params
- **Fișier**: `story-epic-player.component.ts`
- **Modificări**: 
  - Când user selectează "Read Again", navighează la `/story/{storyId}?readAgainKeepRewards=true` sau `?readAgainResetRewards=true`
  - Adăugare `redirectUrl` pentru a reveni la epic player

**Step 6.2**: Refresh epic progress după reset
- **Fișier**: `story-epic-player.component.ts`
- **Modificări**: 
  - După reset, refresh progress din backend
  - Actualizează UI pentru a reflecta că story-ul nu mai este completat

**Step 6.3**: Gestionare erori
- **Fișier**: `story-epic-player.component.ts`
- **Modificări**: 
  - Afișare eroare dacă reset-ul eșuează
  - Afișare warning dacă nu există suficienți tokeni pentru decrementare

### Faza 7: Testing și Polish
**Obiectiv**: Testare completă și îmbunătățiri UX

**Step 7.1**: Test Read Again fără Reset
- Test că quiz-urile sunt readonly
- Test că răspunsurile anterioare sunt afișate corect
- Test că nu se salvează progres nou
- Test că nu se acordă tokeni noi

**Step 7.2**: Test Read Again cu Reset
- Test că progresul este șters
- Test că tokenii sunt decrementați corect
- Test că story-ul poate fi citit din nou normal
- Test că tokenii noi sunt acordați la re-completare

**Step 7.3**: Test Edge Cases
- Test cu story fără quiz-uri
- Test cu story fără tokeni
- Test cu tokeni deja folosiți în Tree of Heroes
- Test cu tokeni insuficienți pentru decrementare

**Step 7.4**: UX Improvements
- Loading states pentru reset
- Success/error messages
- Confirmation dialogs
- Visual feedback pentru readonly mode

## Open Questions

### Q1: Comportament când tokenii au fost deja folosiți
**Întrebare**: Dacă utilizatorul a folosit deja tokenii acordați pentru a debloca eroi în Tree of Heroes, ce se întâmplă când resetează story-ul?

**Opțiuni**:
- **A**: Decrementăm tokenii oricum (poate duce la valori negative sau eroare)
- **B**: Nu decrementăm dacă tokenii au fost folosiți (doar resetăm progresul story-ului)
- **C**: Resetăm și progresul Tree of Heroes (eroii deblocați folosind acești tokeni)
- **D**: Permitem reset doar dacă tokenii nu au fost folosiți

**Recomandare**: Opțiunea **B** - Nu decrementăm dacă tokenii au fost folosiți. Resetăm doar progresul story-ului și permitem re-citire. Utilizatorul va primi tokeni noi la re-completare, dar nu va pierde progresul din Tree of Heroes.

### Q2: Tracking Token Rewards
**Întrebare**: Cum ținem evidența exactă a tokenilor acordați pentru fiecare story?

**Opțiuni**:
- **A**: Populare `EpicStoryProgress.TokensJson` (simplu, direct)
- **B**: Tabel nou `StoryTokenRewards` (UserId, StoryId, TokenType, TokenValue, Quantity, AwardedAt)
- **C**: Query `StoryQuizAnswers` și extragere tokeni din răspunsurile selectate (complex)

**Recomandare**: Opțiunea **A** - Populare `EpicStoryProgress.TokensJson`. Simplu, direct, și câmpul există deja. Nu suntem în prod, deci putem modifica direct implementarea fără migrații.

### Q3: Session Management pentru Read Again
**Întrebare**: Cum gestionăm sesiunile pentru quiz answers când user-ul recitește story-ul?

**Opțiuni**:
- **A**: Creare sesiune nouă (simplu, dar pierdem legătura cu sesiunea originală)
- **B**: Folosire sesiune existentă pentru readonly mode (păstrăm legătura)
- **C**: Nu folosim SessionId pentru read again (doar pentru evaluative stories)

**Recomandare**: Opțiunea **A** - Creare sesiune nouă pentru fiecare "read again". SessionId este folosit doar pentru evaluative stories, deci nu ar trebui să fie o problemă.

### Q4: Read Again pentru Story-uri fără Quiz-uri
**Întrebare**: Ce se întâmplă când user-ul vrea să recitească un story fără quiz-uri?

**Opțiuni**:
- **A**: Butonul "Read Again" apare doar pentru story-uri cu quiz-uri
- **B**: Butonul apare pentru toate story-urile, dar opțiunea "Keep Rewards" nu are sens
- **C**: Butonul apare pentru toate, dar doar opțiunea "Reset Rewards" este disponibilă

**Recomandare**: Opțiunea **C** - Butonul apare pentru toate story-urile, dar dacă story-ul nu are quiz-uri, doar opțiunea "Reset Rewards" este disponibilă (sau se afișează direct fără opțiuni).

### Q5: Read Again pentru Story-uri Evaluative
**Întrebare**: Cum gestionăm "Read Again" pentru story-uri evaluative (cu scoring)?

**Opțiuni**:
- **A**: Permitem read again, dar păstrăm cel mai bun scor
- **B**: Permitem read again, dar resetăm și scorul
- **C**: Nu permitem read again pentru story-uri evaluative

**Recomandare**: Opțiunea **B** - Resetăm și scorul. Utilizatorul poate încerca din nou să obțină un scor mai bun.

### Q6: UI/UX pentru Warning și Confirmare
**Întrebare**: Cum afișăm warning-ul și confirmarea pentru reset rewards?

**Opțiuni**:
- **A**: Sub-panel în același modal (cum a sugerat user-ul)
- **B**: Modal separat de confirmare
- **C**: Toast notification + confirmare inline

**Recomandare**: Opțiunea **A** - Sub-panel în același modal, pentru a evita "modal peste modal" și pentru o experiență mai fluidă.

### Q7: Decrementare Tokeni Parțială
**Întrebare**: Ce facem dacă utilizatorul nu are suficienți tokeni pentru decrementare completă?

**Opțiuni**:
- **A**: Eroare - nu permitem reset dacă nu există suficienți tokeni
- **B**: Decrementăm doar cât are disponibil
- **C**: Nu decrementăm deloc, doar resetăm progresul story-ului

**Recomandare**: Opțiunea **C** - Nu decrementăm dacă nu există suficienți tokeni. Resetăm doar progresul story-ului și permitem re-citire. Utilizatorul va primi tokeni noi la re-completare.

### Q8: Read Again pentru Story-uri din Tree of Light
**Întrebare**: Implementăm și pentru Tree of Light stories sau doar pentru Epic stories?

**Opțiuni**:
- **A**: Doar pentru Epic stories (scope limitat)
- **B**: Pentru ambele (consistență)

**Recomandare**: Opțiunea **A** pentru început - Doar pentru Epic stories. Dacă funcționează bine, putem extinde la Tree of Light stories mai târziu.

## Riscuri și Considerații

### Risc 1: Pierdere Tokeni
**Descriere**: Dacă decrementăm tokenii și utilizatorul a folosit deja tokenii pentru Tree of Heroes, ar putea pierde progresul.

**Mitigare**: 
- Verificare dacă tokenii au fost folosiți înainte de decrementare
- Sau: Nu decrementăm dacă tokenii au fost folosiți (doar resetăm progresul story-ului)

### Risc 2: Inconsistență Date
**Descriere**: Dacă reset-ul eșuează parțial (ex: șterge progresul dar nu decrementează tokenii), datele pot deveni inconsistente.

**Mitigare**: 
- Folosire tranzacții pentru toate operațiunile de reset
- Rollback dacă orice operațiune eșuează
- **Avantaj**: Nu suntem în prod, deci putem face cleanup manual dacă apare vreo problemă

### Risc 3: Performance
**Descriere**: Query-urile pentru obținere quiz answers și decrementare tokeni pot fi lente.

**Mitigare**: 
- Index-uri pe `StoryId` și `UserId` în tabelele relevante
- Cache pentru quiz answers dacă este necesar

### Risc 4: UX Confusion
**Descriere**: Utilizatorul poate fi confuz despre diferența între "Keep Rewards" și "Reset Rewards".

**Mitigare**: 
- Mesaje clare în UI
- Explicații despre consecințe
- Confirmare pentru reset rewards

## Concluzie

Implementarea funcționalității "Read Again" este fezabilă și **simplificată** de faptul că nu suntem în producție:
1. **Tracking token rewards** - Putem modifica direct `EpicStoryProgress.TokensJson` fără migrații
2. **Decrementare tokeni** - Putem implementa direct, fără să ne îngrijorăm de date existente
3. **Readonly mode pentru quiz-uri** - Suport nou, fără restricții de backward compatibility
4. **UI/UX clară** - Putem modifica modal-ul direct, fără să păstrăm vechiul comportament

**Recomandare**: Implementare în faze, începând cu "Read Again fără Reset" (mai simplu), apoi adăugarea opțiunii "cu Reset". Putem face modificări directe fără să ne îngrijorăm de compatibilitate cu date existente.

**Avantaje**:
- Nu avem nevoie de migrații pentru date existente
- Putem modifica direct structura bazei de date dacă este necesar
- Putem suprascrie comportamentul existent fără restricții
- Putem testa agresiv fără risc de afectare utilizatori

