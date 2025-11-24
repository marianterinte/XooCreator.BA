# Backend Refactoring Plan

Acest document detaliază candidații identificați pentru refactoring în backend-ul XooCreator.BA, cu scopul de a îmbunătăți mentenanța, testabilitatea și claritatea codului.

> [!IMPORTANT]
> **NO BREAKING CHANGES**: Toate modificările trebuie să fie strict additive sau refactorizări interne care nu schimbă contractul API existent sau comportamentul bazei de date.

## 1. Program.cs - Startup Decomposition

**Obiectiv:** Spargerea monolitului `Program.cs` în clase de configurare granulare.

**Clase Noi:**
- `Infrastructure/Configuration/SwaggerConfiguration.cs`: Configurare OpenApi/Swagger.
- `Infrastructure/Configuration/AuthConfiguration.cs`: Configurare JWT și Auth0.
- `Infrastructure/Configuration/DatabaseConfiguration.cs`: Parsing connection string și configurare DbContext.
- `Infrastructure/Configuration/CorsConfiguration.cs`: Politici CORS.
- `Infrastructure/Initialization/DbInitializer.cs`: Logica de migrare și seeding (mutată din main).

## 2. StoryEditorService.cs - Granular Decomposition

**Obiectiv:** Spargerea `StoryEditorService` (God Class) în servicii mici, specializate (Single Responsibility Principle).

**Clase Noi:**
- `Features/Story-Editor/Services/Content/StoryDraftManager.cs`: Creare/Verificare draft-uri.
- `Features/Story-Editor/Services/Content/StoryTranslationManager.cs`: Gestionare traduceri (adăugare, ștergere).
- `Features/Story-Editor/Services/Content/StoryTileUpdater.cs`: Logică specifică pentru actualizarea Tiles.
- `Features/Story-Editor/Services/Content/StoryAnswerUpdater.cs`: Logică specifică pentru actualizarea Answers și Tokens.
- `Features/Story-Editor/Services/Content/StoryOwnershipService.cs`: Verificare permisiuni și ownership.

## 3. StoryCopyService.cs - Unification & Abstraction

**Obiectiv:** Unificarea logicii de copiere folosind un model intermediar sau strategii.

**Clase Noi:**
- `Features/Story-Editor/Services/Cloning/StoryCloner.cs`: Serviciul principal de clonare.
- `Features/Story-Editor/Services/Cloning/StorySourceMapper.cs`: Mapare din `StoryCraft` sau `StoryDefinition` către un model comun de clonare.
- `Features/Story-Editor/Services/Cloning/StoryCloneStrategy.cs`: Strategii pentru "Copy" vs "New Version".

## 4. Seeding Services - Generic Utilities

**Obiectiv:** Crearea unor utilitare generice pentru seeding.

**Clase Noi:**
- `Infrastructure/Seeding/JsonFileSeeder<T>.cs`: Clasă generică pentru citire/deserializare JSON.
- `Infrastructure/Seeding/SeedingUtils.cs`: Metode helper (ex: `GenerateAuthorId`).
- `Features/Story-Editor/Services/Seeding/AuthorSeeder.cs`: Implementare specifică pentru autori.
- `Features/Story-Editor/Services/Seeding/TopicSeeder.cs`: Implementare specifică pentru topics.
- `Features/Story-Editor/Services/Seeding/AgeGroupSeeder.cs`: Implementare specifică pentru age groups.

## Plan de Execuție (Safe Refactoring)

1.  **Phase 1: Setup** - Crearea structurii de directoare și a claselor goale (folosind `refactoring_setup.cmd`).
2.  **Phase 2: Move Logic** - Mutarea logicii bucată cu bucată în noile clase, păstrând `StoryEditorService` ca un "Facade" care apelează noile servicii.
3.  **Phase 3: Cleanup** - După ce totul este testat, `StoryEditorService` poate fi simplificat sau eliminat (dacă se schimbă injecția în controllere).
