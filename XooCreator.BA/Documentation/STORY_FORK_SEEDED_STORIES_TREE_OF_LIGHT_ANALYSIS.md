# Analiză: Story Fork pentru Seeded Stories (Tree of Light) și Migrare la Story Epic

**Data**: 17 Decembrie 2025  
**Scop**: Analiza posibilității de a face fork la story-urile seeded din Tree of Light și migrarea acestora la Story Epic

## 1. Rezumat Executiv

### 1.1 Situația Curentă
- **Tree of Light** conține 8 story-uri seeded de la `seed@alchimalia.com`
- Story-urile sunt **publicate direct în StoryDefinition** (nu există StoryCraft pentru ele)
- Fork-ul pentru seeded stories este **blocat la nivel de UI** (frontend)
- **Nu există restricții la nivel de backend** pentru fork-ul seeded stories

### 1.2 Oportunitate
Există o oportunitate de a permite fork-ul seeded stories pentru a facilita:
1. **Editarea** story-urilor din Tree of Light
2. **Migrarea** acestora de la seed data la Story Epic (editat și managed prin UI)
3. **Publicarea** unei versiuni proprii a Tree of Light Epic

### 1.3 Recomandare
**DA**, este posibil și recomandat să permitem fork-ul seeded stories cu următoarea abordare:
1. **Eliminarea restricției de UI** care blochează fork-ul pentru seeded stories
2. **Crearea de story crafts** prin fork pentru fiecare story din Tree of Light
3. **Editarea** story-urilor forked în editor
4. **Publicarea** story-urilor editate
5. **Crearea unui Story Epic** care referențiază story-urile publicate
6. **Publicarea Story Epic-ului** pentru a înlocui Tree of Light seeded

---

## 2. Analiza Story Fork - Cum Funcționează Momentan

### 2.1 Backend - Fork Story Endpoint

**Locație**: `XooCreator.BA/Features/Story-Editor/Endpoints/ForkStoryEndpoint.cs`

**Endpoint**: `POST /api/stories/{storyId}/fork`

**Request Body**:
```json
{
  "copyAssets": true
}
```

**Proces**:
1. **Autorizare**: Verifică dacă user-ul are rol `Creator` sau `Admin`
2. **Validare storyId**: Nu acceptă `"new"` ca storyId
3. **Încărcare story sursă**: 
   - Prioritizează `StoryDefinition` (published) peste `StoryCraft` (draft)
   - Metodă: `LoadSourceStoryAsync(storyId, ct)`
4. **Validare sursa**: Fork este permis **doar pentru published stories**
   ```csharp
   if (craft != null)
   {
       return TypedResults.BadRequest("Fork is only available for published stories. Use Copy for draft stories.");
   }
   ```
5. **Generare newStoryId**: Folosește `StoryIdGenerator` pentru a genera un ID unic bazat pe user
6. **Creare StoryForkJob**: 
   - Job asincron care copiază story-ul și assets-urile
   - Status inițial: `Queued`
7. **Procesare job**: 
   - Metoda `ProcessForkJobAsync()` creează `StoryCraft` nou
   - Owner-ul craft-ului = user-ul care a făcut fork
   - Assets-urile sunt copiate din published storage la draft storage (dacă `copyAssets = true`)

**Key Points**:
- ✅ Fork funcționează pentru **orice StoryDefinition publicat**
- ✅ Fork creează un **StoryCraft nou** (draft) cu owner-ul = current user
- ✅ **NU există verificări pentru owner-ul original** (poate fi oricine, inclusiv seed@alchimalia.com)
- ✅ Assets-urile sunt copiate automat

### 2.2 Frontend - Fork Story UI

**Locații**:
- `story-editor-list.component.ts` - lista de stories în editor
- `story-reading.component.ts` - story reading page
- `story-controls.component.html` - butoanele de control (Fork, Create Version, etc.)

**Condiții pentru afișarea butonului Fork**:

#### În Story Editor List (`story-editor-list.component.ts`, linia 1843-1845):
```typescript
canForkStory(story: CreatedStoryDto): boolean {
  return this.isCreator() && this.getStoryStatus(story) === 'published';
}
```
- ✅ User trebuie să fie Creator
- ✅ Story trebuie să fie Published
- ✅ **NU verifică dacă story-ul este seeded**

#### În Story Reading (`story-controls.component.html`, linia 33-38):
```html
<button 
  *ngIf="canEdit() && isPublished && !isSeedStory" 
  class="fork-button" 
  (click)="onFork()">
</button>
```
- ✅ User trebuie să aibă permisiuni de edit (Creator)
- ✅ Story trebuie să fie Published
- ❌ **Story NU trebuie să fie seeded** (`!isSeedStory`)

**Problema**: Restricția `!isSeedStory` **blochează fork-ul pentru seeded stories la nivel de UI**.

### 2.3 Verificarea isSeedStory

**Locație**: `story-reading.component.ts`, linia 415-451

**Logică**:
```typescript
private async checkOwnershipAndStatus(): Promise<void> {
  const currentStory = this.stateService.content();
  const ownerEmail = currentStory?.ownerEmail?.toLowerCase()?.trim();
  
  const response = await firstValueFrom(this.myLibrary.loadCreated());
  const stories = response?.stories || [];
  const story = stories.find(s => (s.storyId || s.id) === this.storyId);
  
  if (story) {
    // Story este în created stories - user este owner
    this.isSeedStory.set(false);
  } else {
    // Story NU este în created stories - user NU este owner
    
    // Verifică dacă este seeded:
    const coverImageUrl = currentStory?.coverImageUrl?.toLowerCase() || '';
    const isSeedByEmail = !ownerEmail || ownerEmail === 'seed@alchimalia.com';
    const isSeedByImagePath = coverImageUrl.includes('seed@alchimalia.com');
    this.isSeedStory.set(isSeedByEmail || isSeedByImagePath);
  }
}
```

**Condiții pentru isSeedStory = true**:
1. Story-ul **NU** este în lista de created stories a user-ului curent
2. ȘI:
   - `ownerEmail` este `null`, `empty`, sau `'seed@alchimalia.com'`
   - SAU `coverImageUrl` conține `'seed@alchimalia.com'`

---

## 3. Tree of Light - Structura și Story-urile Seeded

### 3.1 Arhitectura Tree of Light

**Scop**: Un epic pre-configurat care organizează story-urile în regiuni (planete/lumi) cu reguli de deblocare.

**Entități Principale**:
- `TreeConfiguration` - configurația epic-ului (e.g., "puf-puf-journey-v1")
- `TreeRegion` - regiunile/planetele (e.g., Gateway, Terra, Lunaria)
- `TreeStoryNode` - asociază story-uri cu regiuni
- `TreeUnlockRule` - reguli de deblocare între regiuni

**Locație Seed Data**: `XooCreator.BA/Data/SeedData/TreeOfLight/puf-puf-journey-v1.json`

### 3.2 Story-urile din Tree of Light

#### Lista Story-uri Seeded (din `puf-puf-journey-v1.json`):

| StoryId | Regiune | SortOrder | RewardImageUrl |
|---------|---------|-----------|----------------|
| `intro-pufpuf` | gateway | 1 | images/tol/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png |
| `terra-s1` | terra | 1 | images/tol/stories/seed@alchimalia.com/terra-s1/0.Cover.png |
| `terra-s2` | terra | 2 | images/tol/stories/seed@alchimalia.com/terra-s2/0.Cover.png |
| `lunaria-s1` | lunaria | 1 | images/tol/stories/seed@alchimalia.com/lunaria-s1/0.Cover.png |
| `lunaria-s2` | lunaria | 2 | images/tol/stories/seed@alchimalia.com/lunaria-s2/0.Cover.png |
| `mechanika-s1` | mechanika | 1 | images/worlds/pyron.jpg |
| `sylvaria-s1` | sylvaria | 1 | images/worlds/sylvaria.jpg |
| `crystalia-s1` | crystalia | 1 | images/worlds/crystalia.jpg |

**Total**: **8 story-uri seeded**

#### Unlock Rules (Sample):

1. **Rule 1**: Completează `intro-pufpuf` → deblochează regiunea `terra`
2. **Rule 2**: Completează `terra-s1` ȘI `terra-s2` → deblochează regiunea `lunaria`
3. **Rule 3+**: Alte reguli pentru deblocarea regiunilor următoare

### 3.3 Structura Story-urilor Seeded

**Locație**: `XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/i18n/{locale}/*.json`

**Exemplu**: `intro-pufpuf.json` (ro-ro):
```json
{
  "storyId": "intro-pufpuf",
  "title": "Marea Călătorie",
  "coverImageUrl": "images/tol/stories/seed@alchimalia.com/intro-pufpuf/0.Cover.png",
  "sortOrder": 1,
  "unlockedStoryHeroes": ["puf-puf"],
  "tiles": [
    {
      "tileId": "1",
      "type": "page",
      "caption": "Chemarea",
      "text": "...",
      "imageUrl": "images/tol/stories/seed@alchimalia.com/intro-pufpuf/1.puf-puf-flying.png"
    },
    ...
  ]
}
```

**Caracteristici**:
- **Owner**: `seed@alchimalia.com` (creat prin `SeedUserHelper`)
- **Status**: Published (`StoryDefinition`)
- **Assets**: Stocate în published storage (`images/tol/stories/seed@alchimalia.com/...`)
- **Translations**: Disponibile în ro-ro, en-us, hu-hu

### 3.4 Procesul de Seeding

**Locație**: `XooCreator.BA/Features/Story-Editor/Repositories/StoriesRepository.cs`, linia 215-658

**Metodă**: `SeedStoriesAsync()`

**Proces**:
1. Verifică dacă story-urile principale există deja în `StoryDefinition`
2. Încarcă story-urile din JSON files (`Data/SeedData/Stories/seed@alchimalia.com/i18n/{locale}/*.json`)
3. Mapează din `StorySeedData` la `StoryDefinition` (prin `StoryDefinitionMapper.MapFromSeedData()`)
4. Setează `CreatedBy` = `seedUserId` (obținut prin `SeedUserHelper.GetOrCreateSeedUserIdAsync()`)
5. Adaugă story-urile în `StoryDefinitions` (direct published, fără craft)
6. Procesează translațiile pentru fiecare locale (ro-ro, en-us, hu-hu)

**Key Point**: Story-urile seeded sunt create **direct în StoryDefinition**, fără StoryCraft.

---

## 4. Posibilitatea de Fork pentru Seeded Stories

### 4.1 Analiza Restricțiilor

#### Backend: ✅ NU există restricții
- Fork endpoint acceptă **orice StoryDefinition publicat**
- Nu verifică owner-ul original
- Fork creează un `StoryCraft` nou cu owner-ul = current user
- Assets-urile sunt copiate din published storage la draft storage

#### Frontend: ❌ Există restricții
- Butonul Fork este **ascuns** pentru seeded stories (`!isSeedStory`)
- Restricția este **doar la nivel de UI**
- Dacă se apelează direct endpoint-ul de fork, funcționează fără probleme

### 4.2 Scenariul de Fork pentru Seeded Story

**Presupunem**: User-ul face fork la `intro-pufpuf`

**Proces**:
1. **Backend**: `POST /api/stories/intro-pufpuf/fork` cu `{ "copyAssets": true }`
2. **Backend**: Încarcă `StoryDefinition` pentru `intro-pufpuf` (owner = seed@alchimalia.com)
3. **Backend**: Generează `newStoryId` = `{user-firstname}-{user-lastname}-s1` (e.g., `john-doe-s1`)
4. **Backend**: Creează `StoryForkJob` cu:
   - `SourceStoryId` = `intro-pufpuf`
   - `SourceType` = `Published`
   - `TargetOwnerUserId` = `{current-user-id}`
   - `TargetStoryId` = `john-doe-s1`
5. **Backend**: Job processing:
   - Mapează `StoryDefinition` → `StoryCloneData`
   - Creează `StoryCraft` nou:
     - `StoryId` = `john-doe-s1`
     - `OwnerUserId` = `{current-user-id}`
     - `Status` = `Draft`
     - `AuthorName` = `{current-user-name}` (NU "Alchimalia Seed")
   - Copiază assets-urile:
     - Sursa: `images/tol/stories/seed@alchimalia.com/intro-pufpuf/...`
     - Destinație: `drafts/{user-email}/john-doe-s1/...`
6. **Backend**: Job completed, user-ul are acum un draft `john-doe-s1`
7. **Frontend**: User-ul poate edita draft-ul în Story Editor
8. **Frontend**: User-ul poate publica draft-ul → creează `StoryDefinition` pentru `john-doe-s1`

**Rezultat**: ✅ User-ul are acum o copie editabilă a story-ului seeded.

### 4.3 Testare Manuală (Recomandată)

Pentru a verifica că fork-ul funcționează:
1. **Apelează direct endpoint-ul** (prin Postman/cURL/DevTools):
   ```bash
   POST http://localhost:5000/api/stories/intro-pufpuf/fork
   Authorization: Bearer {token}
   Content-Type: application/json
   
   {
     "copyAssets": true
   }
   ```
2. **Verifică răspunsul**: ar trebui să primești un `jobId`
3. **Monitorizează job-ul**: verifică status-ul job-ului prin `GET /api/stories/fork/jobs/{jobId}`
4. **Verifică rezultatul**: când job-ul este `Completed`, verifică că există un nou `StoryCraft` în DB

---

## 5. Migrarea Tree of Light la Story Epic

### 5.1 Motivația Migrării

**Probleme cu Tree of Light seeded**:
1. **Hard-coded**: Tree of Light este seed-uit din JSON files
2. **Non-editable**: Nu poate fi modificat prin UI
3. **Single configuration**: Doar o singură configurație (puf-puf-journey-v1)
4. **Seed dependency**: Toate story-urile sunt legate de seed@alchimalia.com

**Beneficiile Story Epic**:
1. **Editable**: Story Epic poate fi creat și editat în UI
2. **Multiple epics**: Pot fi create multiple epics de către utilizatori
3. **Ownership**: Fiecare epic are un owner (user sau admin)
4. **Publishing**: Story Epic poate fi publicat/unpublished prin UI
5. **Versioning**: Story Epic suportă versioning (draft → published)

### 5.2 Arhitectura Story Epic vs Tree of Light

#### Comparație Entități:

| Tree of Light | Story Epic | Scop |
|---------------|------------|------|
| `TreeConfiguration` | `StoryEpicCraft` / `StoryEpicDefinition` | Configurația epic-ului |
| `TreeRegion` | `StoryRegionCraft` / `StoryRegionDefinition` | Regiuni/Planete |
| `TreeStoryNode` | `EpicStoryNodeCraft` / `EpicStoryNodeDefinition` | Story nodes în regiuni |
| `TreeUnlockRule` | `EpicUnlockRuleCraft` / `EpicUnlockRuleDefinition` | Reguli de deblocare |

**Similarități**:
- Ambele organizează story-uri în regiuni
- Ambele au reguli de deblocare
- Ambele pot fi vizualizate într-un graph/tree network

**Diferențe**:
- **Tree of Light**: seeded, global, read-only, single config
- **Story Epic**: editable, owned by user, versionable, multiple configs

### 5.3 Procesul de Migrare - Plan Recomandat

#### Faza 1: Fork Seeded Stories
1. **Elimină restricția UI** pentru fork-ul seeded stories:
   - Modifică `story-controls.component.html`:
     ```html
     <!-- BEFORE -->
     <button *ngIf="canEdit() && isPublished && !isSeedStory" ...>
     
     <!-- AFTER -->
     <button *ngIf="canEdit() && isPublished" ...>
     ```
2. **Fork fiecare story** din Tree of Light:
   - `intro-pufpuf` → `your-name-intro-pufpuf-s1`
   - `terra-s1` → `your-name-terra-s1-s1`
   - `terra-s2` → `your-name-terra-s2-s1`
   - `lunaria-s1` → `your-name-lunaria-s1-s1`
   - `lunaria-s2` → `your-name-lunaria-s2-s1`
   - `mechanika-s1` → `your-name-mechanika-s1-s1`
   - `sylvaria-s1` → `your-name-sylvaria-s1-s1`
   - `crystalia-s1` → `your-name-crystalia-s1-s1`

#### Faza 2: Editare și Publicare Story-uri
1. **Editează fiecare draft** în Story Editor (opțional):
   - Modifică conținutul
   - Adaugă/modifică tile-uri
   - Ajustează traducerile
2. **Publică fiecare story**:
   - Status: Published
   - Assets-uri copiate în published storage

#### Faza 3: Crearea Story Epic-ului
1. **Creează un nou Story Epic** în Story Epic Editor:
   - Epic ID: `tree-of-light-v2` (sau alt nume)
   - Epic Name: `Tree of Light V2`
2. **Adaugă regiunile** (copiind din Tree of Light):
   - Gateway
   - Terra
   - Lunaria
   - Mechanika
   - Sylvaria
   - Crystalia
   - Pyron
   - Vulkara
   - Aqualia
   - Aetherion
   - Umbra
3. **Adaugă story nodes**:
   - Asociază fiecare story publicat cu regiunea corespunzătoare
   - Setează `sortOrder` pentru fiecare
4. **Adaugă unlock rules**:
   - Copiază regulile din Tree of Light
   - Ajustează `storyId` pentru a referenția story-urile noi publicate

#### Faza 4: Publicarea Story Epic-ului
1. **Validează epic-ul** în preview:
   - Verifică că toate story-urile sunt published
   - Verifică că toate regiunile sunt definite
   - Verifică că toate unlock rules sunt corecte
2. **Publică epic-ul**:
   - Status: Published
   - Assets-uri copiate în published storage
   - `StoryEpicDefinition` creat în DB

#### Faza 5: Migrarea Frontend-ului
1. **Modifică Tree of Light component** pentru a încărca din Story Epic:
   - În loc de `/api/{locale}/tree-of-light/state`
   - Apelează `/api/{locale}/story-epic/{epicId}/state`
2. **Păstrează backward compatibility**:
   - Dacă nu există Story Epic publicat, folosește Tree of Light seeded
   - Dacă există Story Epic publicat, folosește Story Epic

### 5.4 Avantajele Migrării

1. **Editabilitate**: Poți edita story-urile și epic-ul prin UI
2. **Versioning**: Poți crea versiuni noi ale epic-ului
3. **Multiple Epics**: Poți crea multiple tree-uri (e.g., "Journey 1", "Journey 2")
4. **Ownership**: Fiecare epic are un owner care poate controla publicarea
5. **Unpublishing**: Poți unpublica un epic dacă nu mai este relevant
6. **Testing**: Poți testa epic-ul în draft mode înainte de a-l publica

---

## 6. Provocări și Soluții

### 6.1 Provocare 1: Assets-urile Seeded

**Problemă**: Assets-urile seeded sunt în `images/tol/stories/seed@alchimalia.com/...`

**Soluție**:
- La fork, assets-urile sunt copiate automat în draft storage (`drafts/{user-email}/{storyId}/...`)
- La publicare, assets-urile sunt copiate în published storage (`images/tol/stories/{user-email}/{storyId}/...`)
- Originalele rămân neatinse în seed storage

### 6.2 Provocare 2: Story IDs

**Problemă**: Story-urile forked vor avea ID-uri noi (e.g., `john-doe-s1` în loc de `intro-pufpuf`)

**Soluție**:
- În Story Epic, referențiază story-urile publicate prin ID-ul lor nou
- Păstrează un mapping între story-urile originale și cele forked (opțional, pentru documentație)

### 6.3 Provocare 3: Translațiile

**Problemă**: Seeded stories au 3 traduceri (ro-ro, en-us, hu-hu)

**Soluție**:
- La fork, toate traducerile sunt copiate în `StoryCraft`
- User-ul poate edita fiecare traducere în Story Editor
- La publicare, toate traducerile sunt copiate în `StoryDefinition`

### 6.4 Provocare 4: Heroes Unlocked

**Problemă**: Seeded stories deblochează eroi (e.g., `intro-pufpuf` deblochează `puf-puf`)

**Soluție**:
- La fork, `unlockedStoryHeroes` este copiat în `StoryCraft`
- User-ul poate modifica lista de eroi deblocați în Story Editor
- În Story Epic, poți referi eroii prin `EpicHeroReference`

### 6.5 Provocare 5: Backward Compatibility

**Problemă**: Utilizatorii existenți au progres în Tree of Light seeded

**Soluție**:
- **Opțiunea 1**: Păstrează Tree of Light seeded și adaugă Story Epic paralel
  - Frontend-ul verifică dacă există Story Epic publicat
  - Dacă DA, folosește Story Epic
  - Dacă NU, folosește Tree of Light seeded
- **Opțiunea 2**: Migrează progresul utilizatorilor
  - Creează un script de migrare care copiază progresul din Tree of Light în Story Epic
  - Mapează story-urile vechi la cele noi (prin `storyId` mapping)

---

## 7. Concluzie și Recomandări

### 7.1 Răspuns la Întrebări

#### 1. **Cum se face momentan Story Fork?**
- Fork este disponibil pentru orice StoryDefinition publicat
- Creează un StoryCraft nou (draft) cu owner-ul = current user
- Assets-urile sunt copiate automat
- ❌ **Blocat la nivel de UI pentru seeded stories** (dar funcționează la nivel de backend)

#### 2. **Putem face Story Fork pentru Tree of Light stories?**
- ✅ **DA**, backend-ul permite fork pentru seeded stories
- ❌ **NU**, frontend-ul blochează butonul Fork pentru seeded stories
- **Soluție**: Elimină restricția `!isSeedStory` din frontend

#### 3. **Putem crea Story Craft din seeded stories?**
- ✅ **DA**, prin fork se creează automat StoryCraft
- Craft-ul poate fi editat în Story Editor
- Craft-ul poate fi publicat → StoryDefinition nou

#### 4. **Putem migra seeded data la Story Epic?**
- ✅ **DA**, procesul recomandat:
  1. Fork toate story-urile seeded
  2. Editează și publică story-urile forked
  3. Creează Story Epic în editor
  4. Adaugă regiuni, story nodes, și unlock rules
  5. Publică Story Epic

### 7.2 Recomandări Prioritizate

#### Prioritate 1: Permite Fork pentru Seeded Stories
- **Task**: Elimină restricția `!isSeedStory` din `story-controls.component.html`
- **Effort**: 1 linie de cod
- **Beneficiu**: Permite fork-ul immediate pentru seeded stories

#### Prioritate 2: Testare Manuală
- **Task**: Testează fork pentru `intro-pufpuf`
- **Effort**: 30 minute
- **Beneficiu**: Validează că procesul funcționează end-to-end

#### Prioritate 3: Fork Toate Story-urile Tree of Light
- **Task**: Fork cele 8 story-uri din Tree of Light
- **Effort**: 1-2 ore (cu editare opțională)
- **Beneficiu**: Story-uri editable în Story Editor

#### Prioritate 4: Creează Story Epic pentru Tree of Light
- **Task**: Creează Story Epic în editor cu story-urile forked
- **Effort**: 2-3 ore
- **Beneficiu**: Tree of Light editable și versionable

#### Prioritate 5: Publică Story Epic
- **Task**: Publică Story Epic-ul
- **Effort**: 30 minute
- **Beneficiu**: Story Epic disponibil pentru utilizatori

#### Prioritate 6 (Opțional): Migrează Frontend
- **Task**: Modifică Tree of Light component pentru a încărca din Story Epic
- **Effort**: 2-4 ore
- **Beneficiu**: Elimină dependența de seed data

### 7.3 Avantaje Finale

1. **Flexibilitate**: Poți edita story-urile și epic-ul prin UI
2. **Multiple Versions**: Poți crea versiuni noi ale Tree of Light
3. **Community**: Alți utilizatori pot crea propriile epic-uri (inspirate din Tree of Light)
4. **Maintenance**: Mai ușor de întreținut (fără seed data hard-coded)
5. **Evolution**: Poți evolua Tree of Light fără rebuild și redeploy

### 7.4 Risc Minim

- **Backend-ul deja suportă fork** pentru published stories
- **Nu sunt necesare modificări de backend** (doar eliminarea restricției UI)
- **Procesul este reversibil** (poți păstra seed data ca fallback)
- **Nu afectează utilizatorii existenți** (progresul lor rămâne intact)

---

## 8. Anexe

### 8.1 Cod de Modificat (Frontend)

**Fișier**: `XooCreator/xoo-creator/src/app/story/story-reading/components/story-controls/story-controls.component.html`

**Linia**: 33-38

```html
<!-- BEFORE -->
<button 
  *ngIf="canEdit() && isPublished && !isSeedStory" 
  class="fork-button" 
  type="button" 
  (click)="onFork()" 
  [title]="'storyreading_fork' | translate">
  <!-- SVG icon -->
</button>

<!-- AFTER -->
<button 
  *ngIf="canEdit() && isPublished" 
  class="fork-button" 
  type="button" 
  (click)="onFork()" 
  [title]="'storyreading_fork' | translate">
  <!-- SVG icon -->
</button>
```

**Motivație**: Permite fork-ul pentru toate story-urile published, inclusiv cele seeded.

### 8.2 Story-uri de Forked

| Original StoryId | Forked StoryId (Example) | Region | Description |
|------------------|--------------------------|--------|-------------|
| `intro-pufpuf` | `john-doe-intro-pufpuf-s1` | gateway | Intro story with Puf-Puf |
| `terra-s1` | `john-doe-terra-s1-s1` | terra | First story in Terra |
| `terra-s2` | `john-doe-terra-s2-s1` | terra | Second story in Terra |
| `lunaria-s1` | `john-doe-lunaria-s1-s1` | lunaria | First story in Lunaria |
| `lunaria-s2` | `john-doe-lunaria-s2-s1` | lunaria | Second story in Lunaria |
| `mechanika-s1` | `john-doe-mechanika-s1-s1` | mechanika | First story in Mechanika |
| `sylvaria-s1` | `john-doe-sylvaria-s1-s1` | sylvaria | First story in Sylvaria |
| `crystalia-s1` | `john-doe-crystalia-s1-s1` | crystalia | First story in Crystalia |

### 8.3 Referințe Codul Sursă

#### Backend:
- **Fork Endpoint**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Endpoints/ForkStoryEndpoint.cs`
- **Fork Job Processing**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Endpoints/ForkStoryEndpoint.JobProcessing.cs`
- **Story Copy Service**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/StoryCopyService.cs`
- **Story Cloner**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Services/Cloning/StoryCloner.cs`
- **Seed Stories**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Repositories/StoriesRepository.cs` (linia 215-658)

#### Frontend:
- **Story Controls**: `XooCreator/xoo-creator/src/app/story/story-reading/components/story-controls/story-controls.component.html`
- **Story Reading**: `XooCreator/xoo-creator/src/app/story/story-reading/story-reading.component.ts`
- **Story Editor List**: `XooCreator/xoo-creator/src/app/story/story-editor-list/story-editor-list.component.ts`
- **Story Editor Service**: `XooCreator/xoo-creator/src/app/story/story-editor/story-editor.service.ts`

#### Story Epic:
- **Story Epic Service**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Story-Epic/Services/StoryEpicService.cs`
- **Story Epic Publishing**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Story-Epic/Services/StoryEpicPublishingService.cs`
- **Publish Endpoint**: `XooCreator.BA/XooCreator.BA/Features/Story-Editor/Story-Epic/Endpoints/PublishStoryEpicEndpoint.cs`

#### Tree of Light:
- **Seed Data**: `XooCreator.BA/XooCreator.BA/Data/SeedData/TreeOfLight/puf-puf-journey-v1.json`
- **Seeded Stories**: `XooCreator.BA/XooCreator.BA/Data/SeedData/Stories/seed@alchimalia.com/i18n/{locale}/*.json`
- **Analysis Doc**: `XooCreator/xoo-creator/002.Documentation/Story-Epic/02_TREE_OF_LIGHT_ANALYSIS.md`

### 8.4 Documentație Relevantă

- **Fork Story Bug Analysis**: `XooCreator.BA/XooCreator.BA/Documentation/FORK_STORY_BUG_ANALYSIS.md`
- **Fork Author Issue**: `XooCreator/xoo-creator/002.Documentation/StoryEditorLatestVersion/10_FORK_AUTHOR_ISSUE.md`
- **Story Copy/Fork/Version Plan**: `XooCreator/xoo-creator/002.Documentation/StoryEditor/STORY_COPY_FORK_VERSION_PLAN.md`
- **Tree of Light Analysis**: `XooCreator/xoo-creator/002.Documentation/Story-Epic/02_TREE_OF_LIGHT_ANALYSIS.md`

---

**Autor**: AI Analysis  
**Status**: Draft  
**Următorii Pași**: Testare manuală și implementare incrementală

