# Analiza: HeroDefinition vs HeroDefinitionDefinition vs EpicHero

Acest document clarifică ce entități/tabele sunt folosite pentru „eroi” în aplicație și unde (inclusiv pentru pagina Story Creator → Heroes → In progress).

---

## 1. Clarificare rapidă

- **HeroDefinitions** (entitatea `HeroDefinition`, tabela `HeroDefinitions`) **nu** se folosesc la epicuri sau stories. Sunt folosite doar în **Alchimalia Universe** pentru draft-urile de eroi (create/update prin `HeroDefinitionRepository`).
- **HeroDefinitionDefinitions** (entitatea `HeroDefinitionDefinition`, tabela `HeroDefinitionDefinitions`) sunt din **Alchimalia Universe** (versiunea publicată a eroilor din Tree of Heroes). Denumirea este confuză, dar sursa este Alchimalia Universe (publicat).

---

## 2. Ce folosește `http://localhost:4200/story-creator/heroes/in-progress`

Această pagină **nu** folosește nici `HeroDefinition`, nici `HeroDefinitionDefinition`.

Fluxul este:

- **FE:** `StoryEditorListComponent` (viewType=`heroes`, tab=`in-progress`) → `StoryHeroService.listHeroes()` → **GET** `/api/story-editor/heroes`.
- **BE:** `ListEpicHeroesEndpoint` → `IEpicHeroService.ListHeroesForEditorAsync` / `ListAllHeroesAsync` → **EpicHeroService** → **IEpicHeroRepository** (craft + definition pentru **Epic**).

Pe **story-creator/heroes/in-progress** sunt folosite:

- **EpicHeroCraft** – draft-urile de eroi (in-progress, in_review, approved).
- **EpicHeroDefinition** – versiunea publicată a acelorași eroi.

Aceștia sunt eroii din **Story Creator** (Epic / Region / Heroes), folosiți în epicuri și stories, **nu** eroii din Alchimalia Universe (Tree of Heroes).

---

## 3. HeroDefinition (tabela `HeroDefinitions`)

**Rol:** Draft în editorul Alchimalia Universe (creare/editare eroi Tree of Heroes).

**Unde e folosit:**

- **IHeroDefinitionRepository / HeroDefinitionRepository**
  - `GetAsync`, `GetWithTranslationsAsync` – citește un draft după id.
  - `CreateAsync`, `SaveAsync` – creează/actualizează draft.
  - `ListAsync`, `CountAsync` – listă draft-uri (filtrate după status, search).
  - `DeleteAsync` – șterge draft.
- **HeroDefinitionService**
  - `CreateAsync` – creează un nou **HeroDefinition** (draft) prin repository.
  - `UpdateAsync` – actualizează **HeroDefinition** (draft) prin repository.
  - Pentru **GetAsync** și **ListAsync** serviciul citește direct din `_db.HeroDefinitionDefinitions` (entitatea publicată), nu din `HeroDefinitions`.

**Câmpuri relevante:** Id, costuri (Courage, Curiosity etc.), PrerequisitesJson, RewardsJson, Image, Status (draft/in_review etc.), CreatedByUserId, ReviewedByUserId, ReviewNotes, Version, ParentVersionId, Translations (HeroDefinitionTranslation).

---

## 4. HeroDefinitionDefinition (tabela `HeroDefinitionDefinitions`)

**Rol:** Versiunea **publicată** a eroilor. Sursa de date la runtime pentru Tree of Heroes (joc, transformări, config arbore) și pentru afișare în Alchimalia Universe (listă/citire „definiții”).

**Unde e folosit:**

- **TreeOfHeroesRepository**
  - `GetHeroDefinitionsAsync(locale)` – listă definiții publicate cu traduceri, mapate la DTO în limba cerută.
  - `GetHeroDefinitionByIdAsync(heroId, locale)` – o definiție publicată după id.
  - `GetTreeOfHeroesConfigAsync()` – încarcă toate definițiile și construiește config-ul arborelui (base heroes, hybrids, costuri, prereqs).
- **HeroDefinitionService** (Alchimalia Universe)
  - `GetAsync(heroId)` – citește din `_db.HeroDefinitionDefinitions`.
  - `ListAsync(...)` – listă din `_db.HeroDefinitionDefinitions` pentru ecranul de listă „Hero Definitions”.
- **HeroDefinitionCraftService**
  - La **publish**: copiază din **HeroDefinitionCraft** în **HeroDefinitionDefinition** (creare/actualizare în `HeroDefinitionDefinitions` și traduceri).
- **GetUserBestiaryEndpoint**
  - Rezolvă nume/imagini pentru bestiar (Tree of Heroes când `ArmsKey` e id de tip hero tree).
- **TreeOfHeroesConfigCraftService**
  - Verifică existența id-urilor de eroi în `HeroDefinitionDefinitions`.
- **CreateHeroDefinitionVersionEndpoint**
  - Citește o definiție publicată pentru versioning.
- **Seed / migrări**
  - Scripturi SQL și migrări care populează `HeroDefinitionDefinitions` și traducerile.

**Câmpuri relevante:** Id, Version, BaseVersion, LastPublishedVersion, costuri, PrerequisitesJson, RewardsJson, Image, Status (ex. "published"), PublishedByUserId, PublishedAtUtc, Translations (HeroDefinitionDefinitionTranslation). Config-ul arborelui (TreeOfHeroesConfig) are FK către `HeroDefinitionDefinitions` (noduri și muchii).

---

## 5. EpicHero (EpicHeroCraft + EpicHeroDefinition)

**Rol:** Eroii din **Story Creator** (Epic / Region / Heroes), folosiți în epicuri și stories. Acesta este fluxul pentru `/story-creator/heroes/in-progress`.

**Unde e folosit:**

- **ListEpicHeroesEndpoint** – GET `/api/story-editor/heroes` → `IEpicHeroService.ListHeroesForEditorAsync` / `ListAllHeroesAsync`.
- **EpicHeroService** – lucrează cu **IEpicHeroRepository** (EpicHeroCraft pentru draft, EpicHeroDefinition pentru publicat).
- Toate endpoint-urile Story Creator pentru heroes: create, get, update, delete, submit, claim, review, publish, retract, create-version, unpublish, topics, regions.

**Nu** sunt folosiți pentru Tree of Heroes (Alchimalia Universe); acolo se folosesc HeroDefinition / HeroDefinitionDefinition.

---

## 6. Rezumat

| Entitate / concept       | Tabelă(e)                    | Rol principal                          | Unde o găsești                                                                 |
|--------------------------|------------------------------|----------------------------------------|-------------------------------------------------------------------------------|
| **HeroDefinition**       | `HeroDefinitions`            | Draft în editor Alchimalia Universe    | HeroDefinitionRepository (CRUD draft), HeroDefinitionService (Create/Update). |
| **HeroDefinitionDefinition** | `HeroDefinitionDefinitions` | Versiune publicată, runtime Tree of Heroes | Tree of Heroes (API definitions, config arbore), HeroDefinitionService (Get/List), publish din Craft, bestiar, versioning. |
| **EpicHero**             | EpicHeroCraft / EpicHeroDefinition | Eroi Story Creator (epicuri/stories)   | `/story-creator/heroes/in-progress`, IEpicHeroService, ListEpicHeroesEndpoint. |

Fluxuri:

- **Story Creator Heroes** (epicuri/stories): EpicHeroCraft (draft) → publish → EpicHeroDefinition (publicat). Folosit pe **story-creator/heroes/in-progress**.
- **Alchimalia Universe Tree of Heroes**: HeroDefinitionCraft (draft) → publish → HeroDefinitionDefinition (publicat). HeroDefinition (draft) este folosit pentru Create/Update în editor; pentru Get/List, HeroDefinitionService citește din `HeroDefinitionDefinitions` (publicat).
