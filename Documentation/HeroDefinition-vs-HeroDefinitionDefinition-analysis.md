# Analiză: HeroDefinition vs HeroDefinitionDefinition și ce folosește Story Creator

## 1. Clarificare denumiri

- **HeroDefinitions** (entitatea `HeroDefinition`, tabela `HeroDefinitions`) **nu** se folosesc la epicuri sau stories. Sunt folosite doar în **Alchimalia Universe** pentru draft-urile de eroi (create/update prin `HeroDefinitionRepository`).
- **HeroDefinitionDefinitions** (entitatea `HeroDefinitionDefinition`, tabela `HeroDefinitionDefinitions`) sunt din **Alchimalia Universe** (versiunea publicată a eroilor din Tree of Heroes). Denumirea este confuză, dar sursa este Alchimalia Universe (publicat).

---

## 2. Ce folosește `http://localhost:4200/story-creator/heroes/in-progress`

Această pagină **nu** folosește nici `HeroDefinition`, nici `HeroDefinitionDefinition`.

**Flux:**

- **FE:** `StoryEditorListComponent` (viewType=`heroes`, tab=`in-progress`) → `StoryHeroService.listHeroes()` → **GET** `/api/story-editor/heroes`.
- **BE:** `ListEpicHeroesEndpoint` → `IEpicHeroService.ListHeroesForEditorAsync` / `ListAllHeroesAsync` → **EpicHeroService** → **IEpicHeroRepository** (craft + definition pentru **Epic**).

Pe **story-creator/heroes/in-progress** sunt folosiți:

- **EpicHeroCraft** – draft-urile de eroi (in-progress, in_review, approved).
- **EpicHeroDefinition** – versiunea publicată a acelorași eroi.

Aceștia sunt eroii din **Story Creator** (Epic / Region / Heroes), folosiți în epicuri și stories, **nu** eroii din Alchimalia Universe (Tree of Heroes).

---

## 3. Rezumat: ce e folosit unde

| Ce vezi în UI | Ruta / context | Entități / tabele folosite |
|---------------|----------------|----------------------------|
| **Story Creator → Heroes → In progress** | `/story-creator/heroes/in-progress` | **EpicHeroCraft** + **EpicHeroDefinition** (tabele pentru Epic Heroes). **Nu** HeroDefinition / HeroDefinitionDefinition. |
| **Alchimalia Universe** (editor eroi Tree of Heroes, draft) | Alchimalia Universe editor | **HeroDefinition** (draft), **HeroDefinitionCraft** (draft alternativ), apoi la publish → **HeroDefinitionDefinition** (publicat). |
| **Tree of Heroes** (joc, transformări, config) | API-uri Tree of Heroes, bestiar | **HeroDefinitionDefinition** (citire publicată). |

---

## 4. Detalii pe entități

### HeroDefinition (tabela `HeroDefinitions`)

- **Rol:** Draft în editorul Alchimalia Universe (eroi Tree of Heroes).
- **Unde:** `IHeroDefinitionRepository` / `HeroDefinitionRepository` – GetAsync, GetWithTranslationsAsync, CreateAsync, SaveAsync, ListAsync, CountAsync, DeleteAsync. `HeroDefinitionService` – CreateAsync, UpdateAsync (lucrează pe draft).
- **Câmpuri relevante:** Id, costuri (Courage, Curiosity etc.), PrerequisitesJson, RewardsJson, Image, Status (draft/in_review etc.), CreatedByUserId, ReviewedByUserId, ReviewNotes, Version, ParentVersionId, Translations (HeroDefinitionTranslation).

### HeroDefinitionDefinition (tabela `HeroDefinitionDefinitions`)

- **Rol:** Versiune publicată a eroilor Tree of Heroes; sursă la runtime pentru joc și bestiar.
- **Unde:** `TreeOfHeroesRepository` (GetHeroDefinitionsAsync, GetHeroDefinitionByIdAsync, GetTreeOfHeroesConfigAsync), `HeroDefinitionService` (GetAsync, ListAsync citesc de aici), `HeroDefinitionCraftService` (la publish scrie aici), `GetUserBestiaryEndpoint`, `TreeOfHeroesConfigCraftService`, `CreateHeroDefinitionVersionEndpoint`, seed/migrări.
- **Câmpuri relevante:** Id, Version, BaseVersion, LastPublishedVersion, costuri, PrerequisitesJson, RewardsJson, Image, Status (ex. "published"), PublishedByUserId, PublishedAtUtc, Translations (HeroDefinitionDefinitionTranslation). Config-ul arborelui (TreeOfHeroesConfig) are FK către `HeroDefinitionDefinitions`.

### EpicHeroCraft / EpicHeroDefinition

- **Rol:** Eroii din Story Creator (epic-uri, regiuni, stories) – draft (Craft) și publicat (Definition).
- **Unde:** `/story-creator/heroes/in-progress` → GET `/api/story-editor/heroes` → `IEpicHeroService` → `IEpicHeroRepository`. Toate acțiunile din lista de heroes (submit, review, publish, retract, create-version, unpublish, delete) lucrează pe EpicHero.

---

## 5. Concluzie

Pentru **story-creator/heroes/in-progress** contează doar **EpicHero** (EpicHeroCraft și EpicHeroDefinition). **HeroDefinition** și **HeroDefinitionDefinition** sunt doar pentru Alchimalia Universe / Tree of Heroes.
