# Fix: Eroii Unlocked din StoryCraft nu se păstrează la Publish Epic

## Problema Identificată

Când se salvează eroii unlocked pentru un `StoryCraft` în editorul de epic (tab-ul "Tree Logic"), eroii sunt salvați corect în `StoryCraftUnlockedHeroes`. Totuși, când se publică epic-ul:

1. **Eroii unlocked nu se copiază la `StoryDefinition`** - La publish, eroii unlocked din `StoryCraft` nu sunt copiați la `StoryDefinition`, deci nu sunt disponibili în versiunea publicată.

2. **Eroii nu apar în marketplace** - După ce utilizatorii completează o poveste în epic, eroii unlocked nu apar în containerul "unlocked heroes" din stânga, deoarece sistemul verifică doar `StoryEpicHeroReferences`, nu și eroii unlocked de povestea în sine.

## Analiza Fluxului

### Flux Actual (Înainte de Fix)

1. **Editor Epic → Tree Logic Tab:**
   - Utilizatorul salvează eroii unlocked pentru un `StoryCraft`
   - Eroii sunt salvați în `StoryCraftUnlockedHeroes` ✅

2. **Publish Epic:**
   - `StoryEpicPublishingService.PublishFromCraftAsync` copiază:
     - Regions ✅
     - StoryNodes ✅
     - UnlockRules ✅
     - Translations ✅
   - **NU copiază eroii unlocked din StoryCraft** ❌

3. **Publish Story (individual):**
   - `StoryPublishingService.ApplyFullPublishAsync` copiază:
     - Translations ✅
     - Tiles ✅
     - Topics ✅
     - AgeGroups ✅
   - **NU copiază eroii unlocked din StoryCraft** ❌

4. **Completare Poveste în Epic:**
   - `StoryEpicProgressService.CompleteStoryAsync` verifică doar:
     - `StoryEpicHeroReferences` (eroii asociați cu epic-ul) ✅
   - **NU verifică eroii unlocked din StoryDefinition** ❌

## Soluția Implementată

### 1. Creat Tabelul `StoryDefinitionUnlockedHeroes`

**Fișier:** `Database/Scripts/V0041__add_story_definition_unlocked_heroes.sql`

```sql
CREATE TABLE IF NOT EXISTS "alchimalia_schema"."StoryDefinitionUnlockedHeroes" (
    "StoryDefinitionId" uuid NOT NULL,
    "HeroId" character varying(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    CONSTRAINT "PK_StoryDefinitionUnlockedHeroes" PRIMARY KEY ("StoryDefinitionId", "HeroId"),
    CONSTRAINT "FK_StoryDefinitionUnlockedHeroes_StoryDefinitions_StoryDefinitionId" 
        FOREIGN KEY ("StoryDefinitionId") REFERENCES "alchimalia_schema"."StoryDefinitions" ("Id") ON DELETE CASCADE
);
```

**Entitate:** `Data/Entities/StoryDefinition.cs`
- Adăugat `List<StoryDefinitionUnlockedHero> UnlockedHeroes` la `StoryDefinition`
- Creat clasa `StoryDefinitionUnlockedHero` (similar cu `StoryCraftUnlockedHero`)

**Configurație EF Core:** `Data/Configurations/StoryDefinitionUnlockedHeroConfiguration.cs`
- Configurare pentru cheie compusă `(StoryDefinitionId, HeroId)`
- Foreign key cu `ON DELETE CASCADE`

**DbSet:** `Data/XooDbContext.cs`
- Adăugat `DbSet<StoryDefinitionUnlockedHero> StoryDefinitionUnlockedHeroes`

### 2. Modificat `StoryPublishingService` pentru Publish Story

**Fișier:** `Features/Story-Editor/Services/StoryPublishingService.cs`

**Metodă nouă:** `ReplaceDefinitionUnlockedHeroesAsync`
- Șterge eroii unlocked existenți pentru `StoryDefinition`
- Copiază eroii unlocked din `StoryCraft.UnlockedHeroes` la `StoryDefinitionUnlockedHeroes`

**Apelată în:**
- `ApplyFullPublishAsync` (după `ReplaceDefinitionAgeGroupsAsync`)
- `TryApplyDeltaPublishAsync` (după `ReplaceDefinitionAgeGroupsAsync`)

### 3. Modificat `StoryEpicPublishingService` pentru Publish Epic

**Fișier:** `Features/Story-Editor/Story-Epic/Services/StoryEpicPublishingService.cs`

**Modificare în `PublishFromCraftAsync`:**
- După copierea Translations, pentru fiecare `StoryNode` din epic:
  1. Se obține `StoryCraft`-ul corespunzător (prin `StoryId`)
  2. Se obține `StoryDefinition`-ul corespunzător
  3. Se copiază eroii unlocked din `StoryCraft.UnlockedHeroes` la `StoryDefinitionUnlockedHeroes`

**Logica:**
```csharp
foreach (var craftNode in craft.StoryNodes)
{
    var storyCraft = await _context.StoryCrafts
        .Include(sc => sc.UnlockedHeroes)
        .FirstOrDefaultAsync(sc => sc.StoryId == craftNode.StoryId, ct);

    if (storyCraft != null && storyCraft.UnlockedHeroes != null && storyCraft.UnlockedHeroes.Count > 0)
    {
        var storyDefinition = await _context.StoryDefinitions
            .FirstOrDefaultAsync(sd => sd.StoryId == craftNode.StoryId, ct);

        if (storyDefinition != null)
        {
            // Remove existing unlocked heroes
            // Copy unlocked heroes from StoryCraft to StoryDefinition
        }
    }
}
```

### 4. Modificat `StoryEpicProgressService` pentru Completare Poveste

**Fișier:** `Features/Story-Editor/Story-Epic/Services/StoryEpicProgressService.cs`

**Dependency nou:** `XooDbContext _context` (injectat în constructor)

**Metodă nouă:** `GetUnlockedHeroesFromStoriesAsync`
- Obține eroii unlocked din `StoryDefinitionUnlockedHeroes` pentru poveștile completate
- Returnează `List<UnlockedHeroDto>` cu eroii și URL-urile imaginilor

**Modificat `GetEpicStateWithProgressAsync`:**
- După evaluarea eroilor din `StoryEpicHeroReferences`, se obțin și eroii unlocked din `StoryDefinitionUnlockedHeroes`
- Se combină ambele liste, evitând duplicatele

**Modificat `EvaluateAndUnlockHeroesAsync`:**
- După verificarea eroilor din `StoryEpicHeroReferences`, se verifică și eroii unlocked din `StoryDefinitionUnlockedHeroes` pentru povestea completată
- Se returnează toți eroii nou deblocați (din ambele surse)

**Logica:**
```csharp
// Check heroes from StoryEpicHeroReferences
foreach (var heroRef in heroReferences)
{
    if (heroRef.StoryId == completedStoryId)
    {
        // Add to newlyUnlockedHeroes
    }
}

// Also check heroes unlocked by the story itself
var storyDefinition = await _context.StoryDefinitions
    .Include(sd => sd.UnlockedHeroes)
    .FirstOrDefaultAsync(sd => sd.StoryId == completedStoryId && sd.IsActive);

if (storyDefinition != null && storyDefinition.UnlockedHeroes != null)
{
    foreach (var unlockedHero in storyDefinition.UnlockedHeroes)
    {
        // Add to newlyUnlockedHeroes
    }
}
```

### 5. Actualizat `StoriesRepository`

**Fișier:** `Features/Story-Editor/Repositories/StoriesRepository.cs`

**Modificat `GetStoryDefinitionByIdAsync`:**
- Adăugat `.Include(s => s.UnlockedHeroes)` pentru a încărca eroii unlocked când se obține `StoryDefinition`

## Fișiere Modificate

### Backend

1. **`Data/Entities/StoryDefinition.cs`**
   - Adăugat `List<StoryDefinitionUnlockedHero> UnlockedHeroes`
   - Creat clasa `StoryDefinitionUnlockedHero`

2. **`Data/Configurations/StoryDefinitionUnlockedHeroConfiguration.cs`** (nou)
   - Configurare EF Core pentru `StoryDefinitionUnlockedHero`

3. **`Data/XooDbContext.cs`**
   - Adăugat `DbSet<StoryDefinitionUnlockedHero> StoryDefinitionUnlockedHeroes`

4. **`Features/Story-Editor/Services/StoryPublishingService.cs`**
   - Adăugat `ReplaceDefinitionUnlockedHeroesAsync`
   - Apelată în `ApplyFullPublishAsync` și `TryApplyDeltaPublishAsync`

5. **`Features/Story-Editor/Story-Epic/Services/StoryEpicPublishingService.cs`**
   - Modificat `PublishFromCraftAsync` pentru a copia eroii unlocked din StoryCraft-urile din epic

6. **`Features/Story-Editor/Story-Epic/Services/StoryEpicProgressService.cs`**
   - Adăugat dependency `XooDbContext _context`
   - Adăugat `GetUnlockedHeroesFromStoriesAsync`
   - Modificat `GetEpicStateWithProgressAsync` și `EvaluateAndUnlockHeroesAsync`

7. **`Features/Story-Editor/Repositories/StoriesRepository.cs`**
   - Modificat `GetStoryDefinitionByIdAsync` pentru a include `UnlockedHeroes`

### Database

8. **`Database/Scripts/V0041__add_story_definition_unlocked_heroes.sql`** (nou)
   - Migrație SQL pentru crearea tabelului `StoryDefinitionUnlockedHeroes`

## Fluxul Complet După Fix

### 1. Editor Epic → Tree Logic Tab
```
Utilizator salvează eroii unlocked pentru StoryCraft
↓
Salvat în StoryCraftUnlockedHeroes ✅
```

### 2. Publish Epic
```
StoryEpicPublishingService.PublishFromCraftAsync
↓
Pentru fiecare StoryNode din epic:
  - Obține StoryCraft (cu UnlockedHeroes)
  - Obține StoryDefinition
  - Copiază eroii unlocked: StoryCraft → StoryDefinition ✅
```

### 3. Publish Story (individual)
```
StoryPublishingService.ApplyFullPublishAsync
↓
ReplaceDefinitionUnlockedHeroesAsync
  - Copiază eroii unlocked: StoryCraft → StoryDefinition ✅
```

### 4. Completare Poveste în Epic
```
StoryEpicProgressService.CompleteStoryAsync
↓
EvaluateAndUnlockHeroesAsync
  - Verifică eroii din StoryEpicHeroReferences ✅
  - Verifică eroii din StoryDefinitionUnlockedHeroes ✅
  - Returnează toți eroii nou deblocați ✅
```

### 5. Afișare în Marketplace
```
EpicProgressService.completeStory
↓
UI-ul primește newlyUnlockedHeroes
↓
Eroii apar în containerul "unlocked heroes" din stânga ✅
```

## Pași pentru Testare

1. **Aplică migrația SQL:**
   ```sql
   -- Rulează V0041__add_story_definition_unlocked_heroes.sql
   ```

2. **Testează Publish Story:**
   - Creează un StoryCraft cu eroii unlocked
   - Publică story-ul
   - Verifică că eroii sunt în `StoryDefinitionUnlockedHeroes`

3. **Testează Publish Epic:**
   - Creează un EpicCraft cu StoryNodes
   - Pentru fiecare StoryCraft, adaugă eroii unlocked
   - Publică epic-ul
   - Verifică că eroii sunt copiați la `StoryDefinitionUnlockedHeroes` pentru fiecare story

4. **Testează Completare Poveste:**
   - Completează o poveste într-un epic publicat
   - Verifică că eroii unlocked din `StoryDefinitionUnlockedHeroes` apar în `newlyUnlockedHeroes`
   - Verifică că eroii apar în UI (containerul "unlocked heroes")

## Note Importante

1. **Duplicate Prevention:** Sistemul evită duplicatele când combină eroii din `StoryEpicHeroReferences` și `StoryDefinitionUnlockedHeroes`.

2. **Cascade Delete:** Când se șterge un `StoryDefinition`, eroii unlocked asociate sunt șterși automat (prin `ON DELETE CASCADE`).

3. **Performance:** Pentru epic-uri cu multe povești, copierea eroilor unlocked se face într-o singură tranzacție, dar poate fi optimizată dacă este necesar.

4. **Backward Compatibility:** Eroii existente din `StoryEpicHeroReferences` continuă să funcționeze. Noua funcționalitate este aditivă.

## Probleme Potențiale și Soluții

### Problemă: Eroii nu se copiază la re-publish
**Cauză:** `ReplaceDefinitionUnlockedHeroesAsync` șterge eroii existenți înainte de a copia cei noi.
**Soluție:** Comportamentul este corect - la re-publish, eroii vechi sunt înlocuiți cu cei noi din draft.

### Problemă: Eroii nu apar în UI după completare
**Cauză:** Frontend-ul nu procesează corect `newlyUnlockedHeroes`.
**Soluție:** Verifică `epic-progress.service.ts` și `story-epic-player.component.ts` pentru procesarea eroilor nou deblocați.

## Referințe

- **Tabel similar:** `StoryCraftUnlockedHeroes` (pentru draft-uri)
- **Servicii similare:** `ReplaceDefinitionTopicsAsync`, `ReplaceDefinitionAgeGroupsAsync`
- **Documentație Epic Publish:** `Documentation/EPIC_PUBLISH_JOBS_ANALYSIS.md`

## Data Implementării

**Data:** 2025-01-XX
**Problema inițială:** Eroii unlocked din StoryCraft nu se păstrează la publish epic și nu apar în marketplace după completare poveste.
