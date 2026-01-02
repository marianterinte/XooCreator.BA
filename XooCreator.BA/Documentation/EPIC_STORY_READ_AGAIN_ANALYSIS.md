# Analiza: Funcționalitatea "Review Story" și Reset Total Tokeni

## Notă Importantă
**Status**: Nu suntem încă în producție, deci putem face modificări directe fără restricții de backward compatibility sau migrații complexe. Putem suprascrie orice este necesar.

## Context

Utilizatorii pot completa story-uri în epic-uri dinamice. Când un story este completat, la click pe story-ul completat în epic player, se afișează un modal (ResultModalComponent) cu card flip care arată:
- **Front**: Imaginea mare (cover image)
- **Back**: Detalii despre story (nume, descriere)

**Cerințe noi** (simplificate):

### 1. Review Story (Simplu)
- Adăugare buton **"Review Story"** pe card-ul de story completat
- Permite re-vizualizare story-ului cu quiz-uri în **readonly mode** (afișează răspunsurile date anterior)
- **NU** permite reset - doar review/vizualizare
- **NU** acordă tokeni noi
- **NU** salvează progres nou

### 2. Reset Total Tokeni (în Settings)
- Două butoane separate în Settings (sub secțiunea "Setări Poveste"):
  - **Reset Personality Tokens (Tree of Heroes)**: 
    - Primește înapoi toți tokenii de personalitate consumați
    - Resetează progresul din Tree of Heroes
  - **Reset Discovery Tokens**: 
    - Primește înapoi toți tokenii de discovery consumați
    - Resetează progresul din Discovery tab (Cartea Eroilor)
    - Resetează progresul din Laboratorul Imaginației
- Ambele cu confirmare că vei primi tokenii înapoi

## Analiza Impactului

### 1. Review Story - Datele necesare

#### A. Quiz Answers (Readonly Mode)
- **Tabel**: `StoryQuizAnswers`
- **Câmpuri relevante**: `StoryId`, `TileId`, `SelectedAnswerId`, `IsCorrect`
- **Acțiune**: 
  - **Readonly**: Încărcare răspunsuri existente pentru afișare
  - **NU** se salvează progres nou
  - **NU** se acordă tokeni noi

#### B. Story Read Progress
- **Tabel**: `UserStoryReadProgress`
- **Acțiune**: 
  - Păstrare (pentru a permite navigare la tile-uri deja citite)
  - **NU** se salvează progres nou

### 2. Reset Total Tokeni - Datele care trebuie gestionate

#### A. Personality Tokens (Tree of Heroes)
**Tabele identificate**:
- `UserTokenBalances` (Type = "Personality", Value = "courage", "curiosity", "thinking", "creativity", "safety")
- `HeroTreeProgress` - nodurile deblocate în Tree of Heroes (conține costurile de tokeni pentru fiecare nod)
- `HeroProgress` - eroii deblocați/transformați

**Acțiune pentru Reset**:
- **Returnare tokeni**: Adăugare înapoi tokenii din `HeroTreeProgress` (suma costurilor pentru toate nodurile deblocate)
- **Reset progres**: Ștergere toate înregistrările din `HeroTreeProgress` și `HeroProgress` pentru user

#### B. Discovery Tokens
**Tabele identificate**:
- `UserTokenBalances` (Type = "Discovery", Value = "discovery credit")
- `CreditWallet.DiscoveryBalance` - balance-ul de discovery credits
- `UserBestiary` (BestiaryType = "discovery") - pentru Discovery tab (Cartea Eroilor)
- `AlchimaliaUser.HasVisitedImaginationLaboratory` - flag pentru Laboratorul Imaginației

**Acțiune pentru Reset**:
- **Returnare tokeni**: Reset `CreditWallet.DiscoveryBalance` la 0 și adăugare tokenii înapoi în `UserTokenBalances`
- **Reset progres**: 
  - Ștergere toate înregistrările din `UserBestiary` cu `BestiaryType = "discovery"`
  - Reset `AlchimaliaUser.HasVisitedImaginationLaboratory = false` (opțional - poate rămâne true pentru că e doar un flag de vizită)

#### C. Epic Story Progress
- **Tabel**: `EpicStoryProgress`
- **Acțiune**: 
  - **NU** se resetează la reset tokeni (progresul story-urilor rămâne)
  - Doar tokenii sunt returnați

#### D. Story Progress (Tree of Light)
- **Tabel**: `StoryProgress`, `TreeProgress`
- **Acțiune**: 
  - **NU** se resetează (doar pentru reset discovery tokens, dacă este relevant)

### 3. Probleme Identificate

#### Problema 1: Quiz Answers Readonly Mode
**Situația actuală**:
- `StoryQuizComponent` nu are suport pentru readonly mode
- Trebuie să afișeze răspunsurile date anterior fără să permită modificare

**Soluție necesară**:
- Adăugare `@Input() readonly: boolean = false` în `StoryQuizComponent`
- Încărcare răspunsuri existente din `StoryQuizAnswers` când story-ul este în modul "review"
- Disable toate butoanele de selecție răspuns
- Evidențiere răspunsul selectat anterior

#### Problema 2: Calculare Tokeni Consumați pentru Returnare
**Situația actuală**:
- Pentru Personality Tokens: `HeroTreeProgress` conține costurile de tokeni pentru fiecare nod deblocat
- Pentru Discovery Tokens: `CreditWallet.DiscoveryBalance` conține balance-ul curent

**Soluție necesară**:
- **Personality Tokens**: 
  - Sumă tokenii din `HeroTreeProgress` pentru user (TokensCostCourage, TokensCostCuriosity, etc.)
  - Adăugare înapoi acești tokeni în `UserTokenBalances`
- **Discovery Tokens**: 
  - Citire `CreditWallet.DiscoveryBalance` pentru user
  - Adăugare înapoi tokenii în `UserTokenBalances` (Type = "Discovery", Value = "discovery credit")
  - Reset `CreditWallet.DiscoveryBalance = 0`

### 4. Flow-uri de Utilizare

#### Flow 1: Review Story (Simplu)
1. User click pe story completat în epic player
2. Modal se deschide cu card flip (front: imagine, back: detalii)
3. User click pe buton **"Review Story"**
4. Navigare directă la story reading cu flag `reviewMode=true`
5. Story reading:
   - Încarcă story-ul normal
   - Încarcă quiz answers existente din DB
   - Quiz-urile sunt afișate în mod readonly (răspunsurile selectate anterior sunt evidențiate)
   - User poate naviga prin story dar nu poate modifica răspunsurile
6. La finalizare: Nu se salvează progres nou, nu se acordă tokeni noi, nu se marchează story-ul ca completat din nou

#### Flow 2: Reset Personality Tokens (Tree of Heroes)
1. User merge în **Settings**
2. User scroll la secțiunea "Setări Poveste"
3. User vede butonul **"Reset Personality Tokens"**
4. User click pe buton
5. **Confirmare**: Modal afișează warning:
   - "Această acțiune va reseta progresul din Tree of Heroes"
   - "Vei primi înapoi toți tokenii de personalitate consumați"
   - "Ești sigur că vrei să continui?"
6. User confirmă
7. Backend:
   - Calculează tokenii consumați (sau returnează toți tokenii din `UserTokenBalances` pentru Personality)
   - Adaugă înapoi tokenii în `UserTokenBalances`
   - Șterge progresul din `HeroTreeProgress`
   - Șterge progresul din `HeroProgress`
8. Frontend: Refresh Tree of Heroes pentru a reflecta reset-ul

#### Flow 3: Reset Discovery Tokens
1. User merge în **Settings**
2. User scroll la secțiunea "Setări Poveste"
3. User vede butonul **"Reset Discovery Tokens"**
4. User click pe buton
5. **Confirmare**: Modal afișează warning:
   - "Această acțiune va reseta progresul din Discovery (Cartea Eroilor) și Laboratorul Imaginației"
   - "Vei primi înapoi toți tokenii de discovery consumați"
   - "Ești sigur că vrei să continui?"
6. User confirmă
7. Backend:
   - Calculează tokenii consumați (sau returnează toți tokenii din `UserTokenBalances` pentru Discovery)
   - Adaugă înapoi tokenii în `UserTokenBalances`
   - Resetează progresul din Discovery tab (Cartea Eroilor)
   - Resetează progresul din Laboratorul Imaginației
8. Frontend: Refresh componentele relevante pentru a reflecta reset-ul

## Arhitectură Propusă

### Backend (BA)

#### 1. Endpoint nou: Get Story Quiz Answers (pentru Review)
**Route**: `GET /api/{locale}/stories/{storyId}/quiz-answers`
**Response**:
```csharp
public record StoryQuizAnswersResponse
{
    public List<QuizAnswerDto> Answers { get; init; } = new();
}

public record QuizAnswerDto
{
    public string TileId { get; init; } = string.Empty;
    public string SelectedAnswerId { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
```

#### 2. Endpoint nou: Reset Personality Tokens
**Route**: `POST /api/tokens/personality/reset`
**Response**:
```csharp
public record ResetPersonalityTokensResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TokensReturned { get; init; } // Număr de tokeni returnați (pentru feedback)
}
```

#### 3. Endpoint nou: Reset Discovery Tokens
**Route**: `POST /api/tokens/discovery/reset`
**Response**:
```csharp
public record ResetDiscoveryTokensResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TokensReturned { get; init; } // Număr de tokeni returnați (pentru feedback)
}
```

#### 4. Repository Methods: Reset Token Progress
**Location**: `TreeOfHeroesRepository` sau repository nou
**Methods**:
```csharp
Task<ResetTokensResult> ResetPersonalityTokensAsync(Guid userId);
Task<ResetTokensResult> ResetDiscoveryTokensAsync(Guid userId);
```

**Logică pentru Reset Personality Tokens**:
1. **Calculează tokenii consumați**: Sumă tokenii din `HeroTreeProgress` pentru user (TokensCostCourage, TokensCostCuriosity, TokensCostThinking, TokensCostCreativity, TokensCostSafety)
2. **Returnare tokeni**: Adaugă înapoi tokenii calculați în `UserTokenBalances` (Type = "Personality")
3. **Reset progres**: Șterge toate înregistrările din `HeroTreeProgress` pentru user
4. **Reset progres**: Șterge toate înregistrările din `HeroProgress` pentru user

**Logică pentru Reset Discovery Tokens**:
1. **Calculează tokenii consumați**: Citește `CreditWallet.DiscoveryBalance` pentru user
2. **Returnare tokeni**: Adaugă înapoi tokenii în `UserTokenBalances` (Type = "Discovery", Value = "discovery credit")
3. **Reset balance**: Setează `CreditWallet.DiscoveryBalance = 0`
4. **Reset progres Discovery tab**: Șterge toate înregistrările din `UserBestiary` cu `BestiaryType = "discovery"` pentru user
5. **Reset Laborator Imaginație**: Opțional - reset `AlchimaliaUser.HasVisitedImaginationLaboratory = false` (sau poate rămâne true)

### Frontend (FE)

#### 1. Extindere ResultModalComponent
**Location**: `creature-builder/modals/result.modal.html`
**Modificări**:
- Adăugare buton **"Review Story"** pe back side al card-ului (când este story epic)
- Butonul navighează direct la story reading cu `reviewMode=true`

#### 2. Story Reading Component Modificări
**Location**: `story-reading/story-reading.component.ts`
**Modificări**:
- Adăugare query param `reviewMode`
- Dacă `reviewMode=true`: 
  - Încarcă quiz answers existente din backend
  - Setează flag `readonlyMode=true`
  - Nu salvează progres nou la finalizare
  - Nu acordă tokeni noi

#### 3. Story Quiz Component Modificări
**Location**: `story-reading/components/story-quiz/story-quiz.component.ts`
**Modificări**:
- Adăugare `@Input() readonly: boolean = false`
- Adăugare `@Input() existingAnswers: Map<string, string> = new Map()` (TileId -> SelectedAnswerId)
- Când `readonly=true`:
  - Disable toate butoanele de răspuns
  - Evidențiere răspunsul selectat anterior (dacă există)
  - Nu permite modificare

#### 4. Settings Component Modificări
**Location**: `pages/settings/settings.component.ts` și `settings.component.html`
**Modificări**:
- Adăugare nouă secțiune sub "Setări Poveste"
- Titlu: "Reset Tokeni" sau similar
- Două butoane:
  - "Reset Personality Tokens (Tree of Heroes)"
  - "Reset Discovery Tokens"
- Handler pentru fiecare buton cu confirmare modal
- Apelare endpoint-uri backend pentru reset

#### 5. Epic Player Component Modificări
**Location**: `story-epic-player/story-epic-player.component.ts`
**Modificări**:
- Adăugare handler pentru butonul "Review Story" din modal
- Navigare la story cu query param `reviewMode=true`

## Baby Steps pentru Implementare

### Faza 1: Frontend - Review Story (Simplu)
**Obiectiv**: Adăugare buton "Review Story" și suport readonly mode

**Step 1.1**: Extindere ResultModalComponent
- **Fișier**: `result.modal.ts` și `result.modal.html`
- **Modificări**:
  - Adăugare `@Input() showReviewStory: boolean = false`
  - Adăugare `@Input() storyId: string | null = null`
  - Adăugare `@Output() reviewStory = new EventEmitter<void>()`
  - Adăugare buton "Review Story" pe back side al card-ului

**Step 1.2**: Integrare în Epic Player
- **Fișier**: `story-epic-player.component.ts` și `story-epic-player.component.html`
- **Modificări**:
  - Pasează `showReviewStory=true` și `storyId` la `result-modal`
  - Handler pentru `(reviewStory)` event
  - Navigare la `/story/{storyId}?reviewMode=true`

**Step 1.3**: Modificare Story Reading Component
- **Fișier**: `story-reading.component.ts`
- **Modificări**:
  - Citire query param `reviewMode`
  - Dacă `reviewMode=true`: 
    - Setează flag `readonlyMode=true`
    - Nu salvează progres nou la finalizare
    - Nu acordă tokeni noi

**Step 1.4**: API pentru obținere quiz answers
- **Fișier**: `stories-api.service.ts` (sau service relevant)
- **Method**: `getStoryQuizAnswers(storyId: string): Observable<QuizAnswer[]>`
- **Endpoint**: `GET /api/{locale}/stories/{storyId}/quiz-answers`

**Step 1.5**: Backend Endpoint pentru Quiz Answers
- **Fișier**: `GetStoryQuizAnswersEndpoint.cs` (nou)
- **Route**: `GET /api/{locale}/stories/{storyId}/quiz-answers`
- **Logică**: Returnează toate quiz answers pentru story-ul respectiv și user-ul curent

**Step 1.6**: Modificare Story Quiz Component
- **Fișier**: `story-quiz.component.ts` și `story-quiz.component.html`
- **Modificări**:
  - Adăugare `@Input() readonly: boolean = false`
  - Adăugare `@Input() existingAnswers: Map<string, string> = new Map()`
  - Când `readonly=true`: Disable butoane, evidențiere răspuns selectat

**Step 1.7**: Integrare în Story Reading
- **Fișier**: `story-reading.component.html`
- **Modificări**: Pasează `[readonly]` și `[existingAnswers]` la `story-quiz`

### Faza 2: Backend - Reset Personality Tokens
**Obiectiv**: Endpoint și logică pentru reset personality tokens

**Step 2.1**: Adăugare DTOs
- **Fișier**: `TreeOfHeroesDtos.cs` sau fișier nou
- **Adăugare**: `ResetPersonalityTokensResponse`

**Step 2.2**: Adăugare repository method
- **Fișier**: `TreeOfHeroesRepository.cs`
- **Method**: `ResetPersonalityTokensAsync(Guid userId)`
- **Logică**: 
  - Calculează tokenii consumați
  - Adaugă înapoi tokenii în `UserTokenBalances`
  - Șterge progresul din `HeroTreeProgress` și `HeroProgress`

**Step 2.3**: Creare endpoint
- **Fișier**: `ResetPersonalityTokensEndpoint.cs` (nou)
- **Route**: `POST /api/tokens/personality/reset`
- **Logică**: Validează user, apelează repository, returnează response

**Step 2.4**: Test endpoint
- Test că tokenii sunt returnați corect
- Test că progresul este resetat

### Faza 3: Backend - Reset Discovery Tokens
**Obiectiv**: Endpoint și logică pentru reset discovery tokens

**Step 3.1**: Identificare tabele Discovery Progress
- **Research**: Identifică tabelele relevante pentru Discovery progress
- **Tabele posibile**: 
  - Discovery tab (Cartea Eroilor)
  - Laboratorul Imaginației

**Step 3.2**: Adăugare DTOs
- **Fișier**: Repository relevant sau fișier nou
- **Adăugare**: `ResetDiscoveryTokensResponse`

**Step 3.3**: Adăugare repository method
- **Fișier**: Repository relevant
- **Method**: `ResetDiscoveryTokensAsync(Guid userId)`
- **Logică**: 
  - Calculează tokenii consumați
  - Adaugă înapoi tokenii în `UserTokenBalances`
  - Resetează progresul din tabelele relevante

**Step 3.4**: Creare endpoint
- **Fișier**: `ResetDiscoveryTokensEndpoint.cs` (nou)
- **Route**: `POST /api/tokens/discovery/reset`
- **Logică**: Validează user, apelează repository, returnează response

**Step 3.5**: Test endpoint
- Test că tokenii sunt returnați corect
- Test că progresul este resetat

### Faza 4: Frontend - Settings UI
**Obiectiv**: Adăugare butoane de reset în Settings

**Step 4.1**: Modificare Settings Component HTML
- **Fișier**: `settings.component.html`
- **Modificări**:
  - Adăugare nouă secțiune sub "Setări Poveste"
  - Titlu: "Reset Tokeni"
  - Două butoane cu stil similar cu celelalte setări

**Step 4.2**: Modificare Settings Component TS
- **Fișier**: `settings.component.ts`
- **Modificări**:
  - Adăugare metode pentru reset personality tokens
  - Adăugare metode pentru reset discovery tokens
  - Handler pentru confirmare modal

**Step 4.3**: Creare Service pentru Reset Tokens
- **Fișier**: `reset-tokens.service.ts` (nou)
- **Methods**:
  - `resetPersonalityTokens(): Observable<ResetPersonalityTokensResponse>`
  - `resetDiscoveryTokens(): Observable<ResetDiscoveryTokensResponse>`

**Step 4.4**: Adăugare Confirmare Modal
- **Fișier**: `settings.component.ts`
- **Modificări**:
  - Modal de confirmare pentru fiecare reset
  - Mesaje clare despre consecințe

**Step 4.5**: Refresh UI după Reset
- **Fișier**: `settings.component.ts`
- **Modificări**:
  - Refresh Tree of Heroes după reset personality tokens
  - Refresh componentele relevante după reset discovery tokens

### Faza 5: Testing și Polish
**Obiectiv**: Testare completă și îmbunătățiri UX

**Step 5.1**: Test Review Story
- Test că quiz-urile sunt readonly
- Test că răspunsurile anterioare sunt afișate corect
- Test că nu se salvează progres nou
- Test că nu se acordă tokeni noi

**Step 5.2**: Test Reset Personality Tokens
- Test că tokenii sunt returnați corect
- Test că progresul Tree of Heroes este resetat
- Test că UI se actualizează corect

**Step 5.3**: Test Reset Discovery Tokens
- Test că tokenii sunt returnați corect
- Test că progresul Discovery este resetat
- Test că UI se actualizează corect

**Step 5.4**: UX Improvements
- Loading states pentru reset
- Success/error messages
- Confirmation dialogs clare
- Visual feedback pentru readonly mode

## Open Questions

### Q1: Calculare Tokeni Consumați pentru Returnare
**Întrebare**: Cum calculăm exact câți tokeni au fost consumați pentru a-i returna utilizatorului?

**Soluție identificată**:
- **Personality Tokens**: 
  - `HeroTreeProgress` conține costurile exacte de tokeni pentru fiecare nod deblocat
  - Sumăm `TokensCostCourage`, `TokensCostCuriosity`, `TokensCostThinking`, `TokensCostCreativity`, `TokensCostSafety` pentru toate nodurile deblocate
  - Returnăm acești tokeni în `UserTokenBalances`
- **Discovery Tokens**: 
  - `CreditWallet.DiscoveryBalance` conține balance-ul curent de discovery credits
  - Returnăm acest balance în `UserTokenBalances` (Type = "Discovery", Value = "discovery credit")
  - Resetăm `CreditWallet.DiscoveryBalance = 0`

**Recomandare**: Folosim datele existente din tabelele de progres pentru a calcula exact tokenii consumați. Nu trebuie să calculăm diferențe - avem tracking direct în `HeroTreeProgress` și `CreditWallet`.

### Q2: Reset Progres Discovery - Tabele Relevante
**Întrebare**: Ce tabele trebuie resetate pentru Discovery tokens?

**Tabele identificate**:
- `UserBestiary` (BestiaryType = "discovery") - pentru Discovery tab (Cartea Eroilor)
- `CreditWallet.DiscoveryBalance` - balance-ul de discovery credits (reset la 0)
- `AlchimaliaUser.HasVisitedImaginationLaboratory` - flag pentru Laboratorul Imaginației (opțional - poate rămâne true)

**Recomandare**: 
- Resetăm `UserBestiary` pentru toate înregistrările cu `BestiaryType = "discovery"`
- Resetăm `CreditWallet.DiscoveryBalance = 0`
- Opțional: Resetăm `HasVisitedImaginationLaboratory = false` (sau poate rămâne true pentru că e doar un flag de vizită)

### Q3: Review Story - Story-uri fără Quiz-uri
**Întrebare**: Ce se întâmplă când user-ul vrea să revadă un story fără quiz-uri?

**Opțiuni**:
- **A**: Butonul "Review Story" apare doar pentru story-uri cu quiz-uri
- **B**: Butonul apare pentru toate, dar nu are efect dacă nu există quiz-uri

**Recomandare**: Opțiunea **B** - Butonul apare pentru toate story-urile. Dacă nu există quiz-uri, story-ul se afișează normal în mod review (fără readonly mode).

### Q4: Review Story - Story-uri Evaluative
**Întrebare**: Cum gestionăm "Review Story" pentru story-uri evaluative (cu scoring)?

**Opțiuni**:
- **A**: Permitem review, dar păstrăm scorul
- **B**: Permitem review, dar nu afișăm scorul în mod review

**Recomandare**: Opțiunea **B** - Permitem review, dar nu afișăm scorul. Utilizatorul poate vedea story-ul și răspunsurile, dar scorul rămâne păstrat.

### Q5: UI/UX pentru Butoane Reset în Settings
**Întrebare**: Cum arată butoanele de reset în Settings?

**Opțiuni**:
- **A**: Butoane simple cu text (similar cu toggle-urile)
- **B**: Butoane cu icon și text
- **C**: Butoane cu warning color (roșu/galben)

**Recomandare**: Opțiunea **C** - Butoane cu warning color pentru a indica că sunt acțiuni distructive. Text clar despre ce face fiecare buton.

### Q6: Confirmare Modal pentru Reset
**Întrebare**: Cum afișăm confirmarea pentru reset tokens?

**Opțiuni**:
- **A**: Modal separat de confirmare
- **B**: Toast notification + confirmare inline
- **C**: Dialog nativ browser

**Recomandare**: Opțiunea **A** - Modal separat de confirmare cu mesaje clare despre consecințe.

## Riscuri și Considerații

### Risc 1: Eșec Parțial al Reset-ului
**Descriere**: În cazuri extreme (eroare de sistem, timeout, constraint violation), reset-ul ar putea eșua parțial.

**Context**: 
- Cu tranzacții database, acest risc este **minimizat la maximum**
- Eșecul parțial ar putea apărea doar în cazuri extreme: eroare de sistem, timeout de conexiune, constraint violation, sau eroare de aplicație

**Mitigare (Best Practices)**:
- **Tranzacții database**: Folosire `BeginTransactionAsync()` și `RollbackAsync()` pentru toate operațiunile de reset
- **Pattern existent**: Deja folosit în `ResetAllProgressEndpoint` și `ResetUserProgressAsync` - urmăm același pattern
- **Atomicity**: Toate operațiunile (returnare tokeni + reset progres) se execută într-o singură tranzacție
- **Error handling**: Try-catch cu rollback automat în caz de eroare
- **Avantaj**: Nu suntem în prod, deci putem face cleanup manual dacă apare vreo problemă extremă

**Exemplu de implementare**:
```csharp
using var transaction = await _context.Database.BeginTransactionAsync(ct);
try
{
    // 1. Calculează și returnează tokenii
    // 2. Resetează progresul
    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

### Risc 2: Performance
**Descriere**: Query-urile pentru reset progres pot fi lente dacă există multe date.

**Mitigare**: 
- **Index-uri**: Asigură index-uri pe `UserId` în tabelele relevante (`HeroTreeProgress`, `HeroProgress`, `UserBestiary`)
- **ExecuteDeleteAsync**: Folosește `ExecuteDeleteAsync()` pentru ștergere eficientă în bloc (deja folosit în codebase)
- **Optimizare**: Query-uri optimizate pentru ștergere în bloc, nu iterativă

### Risc 3: UX Clarity
**Descriere**: Utilizatorul trebuie să înțeleagă clar diferența între "Review Story" și "Reset Tokens".

**Mitigare (Best Practices)**:
- **Review Story**: 
  - Buton clar etichetat "Review Story" sau "Citește din nou"
  - Tooltip/explicație: "Vizualizează story-ul și răspunsurile tale anterioare (fără modificare)"
- **Reset Tokens**: 
  - Butoane separate în Settings cu etichetă clară: "Reset Personality Tokens" și "Reset Discovery Tokens"
  - Mesaj de confirmare explicit: "Vei primi înapoi toți tokenii consumați și progresul va fi resetat"
  - Explicație clară despre ce se întâmplă: "Tokenii vor fi returnați în contul tău pentru a-i folosi din nou în Tree of Heroes / Discovery"
- **Visual distinction**: Butoanele de reset pot avea culoare de warning (galben/portocaliu) pentru a indica acțiune distructivă
- **Confirmation required**: Modal de confirmare obligatoriu pentru reset tokens

## Concluzie

Implementarea funcționalității este **mult simplificată** comparativ cu abordarea inițială:

1. **Review Story** - Simplu, doar buton pe card, fără opțiuni complexe
2. **Reset Total Tokeni** - Două butoane separate în Settings, fără reset per story

**Avantaje**:
- Logică mult mai simplă
- Nu trebuie să tracking tokeni per story
- Utilizatorul are control total asupra reset-ului
- Separare clară între review (readonly) și reset (distructiv)

**Recomandare**: Implementare în faze:
1. **Faza 1**: Review Story (simplu, fără backend complex)
2. **Faza 2**: Reset Personality Tokens
3. **Faza 3**: Reset Discovery Tokens
4. **Faza 4**: Testing și polish

Putem face modificări directe fără să ne îngrijorăm de compatibilitate cu date existente.
