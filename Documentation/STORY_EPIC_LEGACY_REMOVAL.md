# Story Epic Legacy Architecture Removal

## Data: 2025-01-XX

## Context

Sistemul inițial pentru Epic-uri folosea `DbStoryEpic` (similar cu `Story` vechi). Ulterior s-a implementat arhitectura nouă similară cu Stories:
- **Draft**: `StoryEpicCraft` (echivalent cu `StoryCraft`)
- **Published**: `StoryEpicDefinition` (echivalent cu `StoryDefinition`)

## Entități Șterse

### Entities
- `DbStoryEpic.cs` - Epic principal vechi
- `StoryEpicTranslation.cs` - Translations pentru DbStoryEpic
- `StoryEpicRegion.cs` - Regions pentru DbStoryEpic
- `StoryEpicStoryNode.cs` - Story nodes pentru DbStoryEpic
- `StoryEpicUnlockRule.cs` - Unlock rules pentru DbStoryEpic

### Configurations
- `DbStoryEpicConfiguration.cs`
- `StoryEpicTranslationConfiguration.cs`
- `StoryEpicRegionConfiguration.cs`
- `StoryEpicStoryNodeConfiguration.cs`
- `StoryEpicUnlockRuleConfiguration.cs`

### Repositories
- `IStoryEpicRepository.cs`
- `StoryEpicRepository.cs`

## Metode Șterse din StoryEpicService

- `MapToDto(Data.DbStoryEpic epic, string locale)` - Înlocuit cu `MapDefinitionToDto`
- `UpdateTranslationsAsync(Data.DbStoryEpic epic, ...)` - Înlocuit cu `UpdateCraftTranslationsAsync`
- `UpdateRegionsAsync(Data.DbStoryEpic epic, ...)` - Înlocuit cu `UpdateCraftRegionsAsync`
- `UpdateStoryNodesAsync(Data.DbStoryEpic epic, ...)` - Înlocuit cu `UpdateCraftStoryNodesAsync`
- `UpdateUnlockRulesAsync(Data.DbStoryEpic epic, ...)` - Înlocuit cu `UpdateCraftUnlockRulesAsync`

## Metode Șterse din StoryEpicPublishingService

- `ValidatePublishAsync(DbStoryEpic epic, ...)` - Nu mai este necesar, publish-ul se face din StoryEpicCraft
- `CollectEpicAssets(DbStoryEpic epic)` - Înlocuit cu `CollectEpicAssetsFromCraft`
- `UpdateEpicAfterPublish(DbStoryEpic epic, ...)` - Publish-ul se face prin `PublishFromCraftAsync`
- `PublishAsync(Guid ownerUserId, string epicId, ...)` - Nu mai este necesar, folosim `PublishFromCraftAsync`

## Actualizări la Entități Existente

### StoryEpicHeroReference
- **Înainte**: `Epic` navigation property referenția `DbStoryEpic`
- **Acum**: `Epic` navigation property referențiază `StoryEpicDefinition`

### StoryEpicRegionReference
- **Înainte**: `Epic` navigation property referenția `DbStoryEpic`
- **Acum**: `Epic` navigation property referențiază `StoryEpicDefinition`

### EpicReview
- **Înainte**: `Epic` navigation property referenția `DbStoryEpic`
- **Acum**: `Epic` navigation property referențiază `StoryEpicDefinition`

### UserFavoriteEpics
- **Înainte**: `Epic` navigation property referenția `DbStoryEpic`
- **Acum**: `Epic` navigation property referențiază `StoryEpicDefinition`

### EpicReader
- **Înainte**: `Epic` navigation property referenția `DbStoryEpic`
- **Acum**: `Epic` navigation property referențiază `StoryEpicDefinition`

## Scripturi SQL de Șters

Următoarele scripturi SQL creează tabelele pentru sistemul vechi și trebuie șterse:
- `V0023__add_story_epic.sql` - Creează `StoryEpics`, `StoryEpicRegions`, `StoryEpicStoryNodes`, `StoryEpicUnlockRules`
- `V0024__add_story_epic_publish_metadata.sql` - Adaugă `PublishedAtUtc` la `StoryEpics`
- `V0027__add_cover_image_url_to_story_epic.sql` - Adaugă `CoverImageUrl` la `StoryEpics`
- `V0028__add_epic_versioning_and_review_workflow.sql` - Adaugă câmpuri de versioning și review la `StoryEpics`
- `V0033__add_story_epic_translations.sql` - Creează `StoryEpicTranslations`

## Notă Importantă

Aceste scripturi SQL pot fi șterse doar dacă **nu sunt în producție** sau dacă se poate recrea baza de date completă. În producție, aceste scripturi ar trebui să rămână pentru a menține istoricul migrațiilor, dar tabelele nu vor mai fi folosite.

## Arhitectura Nouă

### Draft (Editor)
- `StoryEpicCraft` - Epic în lucru
- `StoryEpicCraftTranslation`
- `StoryEpicCraftRegion`
- `StoryEpicCraftStoryNode`
- `StoryEpicCraftUnlockRule`
- `StoryEpicCraftHeroReference`

### Published (Marketplace)
- `StoryEpicDefinition` - Epic publicat
- `StoryEpicDefinitionTranslation`
- `StoryEpicDefinitionRegion`
- `StoryEpicDefinitionStoryNode`
- `StoryEpicDefinitionUnlockRule`
- `StoryEpicHeroReference` (shared references pentru published heroes)

## Foreign Keys Actualizate

### EpicProgress & EpicStoryProgress
- **Înainte**: Foreign key către `StoryEpics.Id`
- **Acum**: Nu mai au foreign key constraint - `EpicId` poate referi fie `StoryEpics` (legacy) fie `StoryEpicDefinition` (nou)
- **Script**: `V0042__remove_epic_progress_foreign_keys.sql`

## Funcționalități Adăugate pentru StoryEpicDefinition

După eliminarea arhitecturii vechi, s-au adăugat metode helper în `StoryEpicService` pentru lucrul direct cu `StoryEpicDefinition`:

### Metode Noi în IStoryEpicService / StoryEpicService

1. **`GetPublishedEpicAsync(string epicId, CancellationToken ct)`**
   - Returnează un epic publicat (StoryEpicDefinition) ca DTO
   - Nu face fallback la craft - returnează doar epice publicate
   - Folosit pentru citirea epic-urilor publicate din marketplace

2. **`GetStoryEpicDefinitionByIdAsync(string epicId, CancellationToken ct)`**
   - Helper method care returnează entitatea `StoryEpicDefinition` cu toate navigation properties loaded
   - Include: Regions, StoryNodes, UnlockRules, Translations, Owner
   - Folosit intern pentru operații pe epic-uri publicate

3. **`GetAllPublishedEpicsAsync(string locale, CancellationToken ct)`**
   - Returnează lista tuturor epic-urilor publicate
   - Optimizat pentru a evita N+1 queries (toate heroes-urile sunt încărcate într-un singur query)
   - Folosit pentru listări și căutări pe epic-uri publicate

### Metode Existente care Funcționează cu StoryEpicDefinition

- `GetEpicAsync` - face fallback la StoryEpicDefinition dacă nu găsește craft
- `ListEpicsByOwnerAsync` - listează atât crafts cât și definitions
- `DeleteEpicAsync` - șterge atât craft cât și definition
- `CreateVersionFromPublishedAsync` - creează craft nou din definition publicat
- `MapDefinitionToDto` - mapare StoryEpicDefinition → StoryEpicDto

### Marketplace Repository

`EpicsMarketplaceRepository` folosește deja `StoryEpicDefinition` pentru:
- `GetMarketplaceEpicsWithPaginationAsync` - query pe epice publicate
- `GetEpicDetailsAsync` - detalii epic publicat
- `GetStoryIdsInPublishedEpicsAsync` - story IDs din epice publicate

