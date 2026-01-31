# Story Epic - Backend Documentație

## Overview

Acest folder conține documentația backend pentru funcționalitatea **Story Epic**.

## Status Implementare

**Ultima actualizare**: 2025-01-XX

| Componentă | Status | Locație |
|------------|--------|---------|
| Entități | ✅ COMPLETĂ | `Data/Entities/` |
| DbContext | ✅ COMPLETĂ | `Data/XooDbContext.cs` |
| Migrație | ✅ COMPLETĂ | `Database/Scripts/V0023__add_story_epic.sql` |
| Repository | ⏳ PENDING | `Features/Story-Editor/Story-Epic/Repositories/` |
| Service | ⏳ PENDING | `Features/Story-Editor/Story-Epic/Services/` |
| DTOs | ⏳ PENDING | `Features/Story-Editor/Story-Epic/DTOs/` |
| Endpoints | ⏳ PENDING | `Features/Story-Editor/Story-Epic/Endpoints/` |

## Structura

Documentația completă se află în:
- **Frontend**: `FE/XooCreator/xoo-creator/002.Documentation/Story-Epic/`
- **Backend**: `BA/XooCreator.BA/XooCreator.BA/Documentation/StoryEpic/` (acest folder)

## Referințe

Pentru analiza completă și planul de implementare, vezi:
- `FE/XooCreator/xoo-creator/002.Documentation/Story-Epic/00_README.md`
- `FE/XooCreator/xoo-creator/002.Documentation/Story-Epic/01_STORY_EDITOR_ANALYSIS.md`
- `FE/XooCreator/xoo-creator/002.Documentation/Story-Epic/02_TREE_OF_LIGHT_ANALYSIS.md`
- `FE/XooCreator/xoo-creator/002.Documentation/Story-Epic/03_STORY_EPIC_IMPLEMENTATION_PLAN.md`
- `FE/XooCreator/xoo-creator/002.Documentation/Story-Epic/05_IMPLEMENTATION_STATUS.md` - Status actual

## Structura Backend Propusă

```
Features/
└── Story-Editor/
    └── Story-Epic/
        ├── DTOs/
        │   └── StoryEpicDtos.cs
        ├── Endpoints/
        │   ├── CreateStoryEpicEndpoint.cs
        │   ├── GetStoryEpicEndpoint.cs
        │   ├── SaveStoryEpicEndpoint.cs
        │   ├── GetStoryEpicStateEndpoint.cs
        │   ├── ListStoryEpicsEndpoint.cs
        │   └── DeleteStoryEpicDraftEndpoint.cs
        ├── Repositories/
        │   ├── IStoryEpicRepository.cs
        │   └── StoryEpicRepository.cs
        └── Services/
            ├── IStoryEpicService.cs
            └── StoryEpicService.cs
```

## Entități ✅ COMPLETĂ

**Locație**: `BA/XooCreator.BA/XooCreator.BA/Data/Entities/`

- ✅ `StoryEpic.cs` - Epic principal cu ownership
- ✅ `StoryEpicRegion.cs` - Regiuni/planete în epic
- ✅ `StoryEpicStoryNode.cs` - Asocieri story-regiune
- ✅ `StoryEpicUnlockRule.cs` - Reguli de deblocare

**Caracteristici**:
- StoryEpic.Id: string (max 100 chars), unique per owner
- StoryEpicRegion: composite key (EpicId, RegionId)
- StoryEpicStoryNode: composite unique (EpicId, StoryId, RegionId)
- StoryEpicStoryNode poate referi fie StoryCraft (draft) fie StoryDefinition (published)
- Toate FK-urile au cascade delete pentru consistență

## Migrație ✅ COMPLETĂ

**Locație**: `BA/XooCreator.BA/Database/Scripts/V0023__add_story_epic.sql`

**Caracteristici**:
- ✅ Idempotentă (folosește `IF NOT EXISTS`, `ON CONFLICT`)
- ✅ 4 tabele create: `StoryEpics`, `StoryEpicRegions`, `StoryEpicStoryNodes`, `StoryEpicUnlockRules`
- ✅ Indexuri create pentru performanță
- ✅ Constraint-uri unice pentru integritate datelor
- ✅ Cascade delete configurat pentru toate relațiile

## Endpoints

Toate endpoint-urile vor fi sub `/api/{locale}/story-editor/epics`:
- `POST /api/{locale}/story-editor/epics` - Create epic
- `GET /api/{locale}/story-editor/epics/{epicId}` - Get epic
- `PUT /api/{locale}/story-editor/epics/{epicId}` - Save epic
- `GET /api/{locale}/story-editor/epics/{epicId}/state` - Get epic state (for preview)
- `GET /api/{locale}/story-editor/epics` - List epics
- `DELETE /api/{locale}/story-editor/epics/{epicId}` - Delete epic

## Note

- Implementarea va urma pattern-urile existente din Story-Editor și TreeOfLight
- Epicul este „lightweight” la publish (doar referințe), pentru că **stories, heroes și (viitoarele) global regions** vor avea propriul lor flux de draft → review → publish, cu același mecanism de approval ca stories.
- Vezi `03_STORY_EPIC_IMPLEMENTATION_PLAN.md` pentru detalii complete

