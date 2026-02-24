# Publish JSON constraint matrix

This document maps JSON payload fields (EditableStoryDto and children) to craft/definition entities and DB constraints. Violations can cause save or publish to fail; publish uses the same constraints when writing to StoryDefinition tables.

## Summary: what can break publish

| Category | Failure mode | Constraint / index |
|----------|--------------|--------------------|
| **Answer tokens** | Type &gt; 64 or Value &gt; 128 | StoryAnswerToken: Type(64), Value(128) |
| **Dialog edge tokens** | Type &gt; 100 or Value &gt; 200 | StoryDialogEdgeToken: Type(100), Value(200) |
| **Duplicate tile IDs** | Same TileId twice in Tiles | Unique(StoryCraftId, TileId) / (StoryDefinitionId, TileId) |
| **Duplicate answer IDs** | Same Answer Id twice per tile | Unique(StoryCraftTileId, AnswerId) / (StoryTileId, AnswerId) |
| **Duplicate dialog node IDs** | Same NodeId twice per dialog tile | Unique(StoryCraftDialogTileId, NodeId) / (StoryDialogTileId, NodeId) |
| **Duplicate dialog edge IDs** | Same EdgeId twice per node | Unique(StoryCraftDialogNodeId, EdgeId) / (StoryDialogNodeId, EdgeId) |
| **Duplicate (entity, LanguageCode)** | Same language twice per tile/answer/node/edge | Unique on *Translation tables |
| **Branch/reference length** | BranchId, JumpToTileId, SetBranchId, HideIfBranchSet &gt; 100 | HasMaxLength(100) on tile/dialog edge |
| **LanguageCode** | &gt; 10 chars | HasMaxLength(10) on translation entities |
| **Dialog hero ref** | SpeakerHeroId not in DialogParticipants | Business rule in StoryTileUpdater (throws) |
| **Null token fields** | Type/Value null at publish | NOT NULL on StoryAnswerToken/StoryDialogEdgeToken |

---

## 1. Story-level (EditableStoryDto → StoryCraft / StoryDefinition)

| DTO field | Craft / Definition field | DB constraint | Notes |
|-----------|--------------------------|---------------|--------|
| Id | StoryCraft.StoryId / StoryDefinition.StoryId | StoryId max 200 (craft), unique | Set from route or generated |
| Title, Summary | StoryCraftTranslation / StoryDefinitionTranslation | — | Per language |
| Language | — | LanguageCode max 10 on all translation tables | Required in payload |
| TopicIds | StoryCraftTopic / StoryDefinitionTopic | FK to StoryTopic | Missing topic ID = row not added |
| AgeGroupIds | StoryCraftAgeGroup / StoryDefinitionAgeGroup | FK to StoryAgeGroup | Same |
| DialogParticipants | StoryCraftDialogParticipant / StoryDialogParticipant | HeroId max 100, Unique(StoryCraftId, HeroId) | Duplicate hero ID in list → unique violation |
| UnlockedStoryHeroes | StoryCraftUnlockedHero / StoryDefinitionUnlockedHero | HeroId max 100 | — |
| CoAuthors | StoryCraftCoAuthor / StoryDefinitionCoAuthor | DisplayName max 500 | — |

---

## 2. Tiles (EditableTileDto → StoryCraftTile / StoryTile)

| DTO field | Craft / Definition field | DB constraint | Notes |
|-----------|--------------------------|---------------|--------|
| Id | TileId | Unique(StoryCraftId, TileId) / (StoryDefinitionId, TileId) | Duplicate tile IDs in same story → unique violation |
| Type | Type | — | e.g. "page", "quiz", "dialog" |
| BranchId | BranchId | HasMaxLength(100) | Null/empty stored as null |
| Caption, Text, Question | Tile translation | — | Per language |
| ImageUrl, AudioUrl, VideoUrl | ImageUrl (tile); AudioUrl/VideoUrl (translation) | Filename only stored | Length from path not constrained on entity |
| Answers | StoryCraftAnswer / StoryAnswer | See Answers section | Duplicate Answer.Id per tile → unique violation |
| DialogRootNodeId | DialogTile.RootNodeId | — | No max in config; keep &lt;= 100 for consistency with other IDs |
| DialogNodes | StoryCraftDialogNode / StoryDialogNode | Unique(StoryCraftDialogTileId, NodeId) | Duplicate NodeId in same dialog → unique violation |
| AvailableHeroIds | AvailableHeroIdsJson | JSON stored as string | No length in config; large arrays could be an issue |

---

## 3. Quiz answers (EditableAnswerDto → StoryCraftAnswer / StoryAnswer)

| DTO field | Craft / Definition field | DB constraint | Notes |
|-----------|--------------------------|---------------|--------|
| Id | AnswerId | Unique(StoryCraftTileId, AnswerId) / (StoryTileId, AnswerId) | Duplicate answer IDs per tile → unique violation |
| Text | StoryCraftAnswerTranslation.Text / StoryAnswerTranslation.Text | — | Per language; Unique(StoryCraftAnswerId, LanguageCode) |
| IsCorrect | IsCorrect | — | — |
| **Tokens** | StoryCraftAnswerToken / StoryAnswerToken | **Type max 64, Value max 128, NOT NULL** | **Preselect/admin tokens must not exceed lengths; null coalesced to "Personality"/"" on save but copied 1:1 at publish** |

---

## 4. Dialog nodes (EditableDialogNodeDto → StoryCraftDialogNode / StoryDialogNode)

| DTO field | Craft / Definition field | DB constraint | Notes |
|-----------|--------------------------|---------------|--------|
| NodeId | NodeId | MaxLength(100), Unique(StoryCraftDialogTileId, NodeId) | Duplicate NodeId in same tile → unique violation |
| SpeakerType | SpeakerType | MaxLength(20) | "reader" \| "hero" |
| SpeakerHeroId | SpeakerHeroId | MaxLength(100) | Required when SpeakerType = hero; must be in DialogParticipants (business rule) |
| Text, AudioUrl | Node translation | LanguageCode max 10, AudioUrl max 500 | Unique(StoryCraftDialogNodeId, LanguageCode) |
| Options | OutgoingEdges | See Dialog edges | EdgeId unique per node |

---

## 5. Dialog options / edges (EditableDialogOptionDto → StoryCraftDialogEdge / StoryDialogEdge)

| DTO field | Craft / Definition field | DB constraint | Notes |
|-----------|--------------------------|---------------|--------|
| Id | EdgeId | MaxLength(100), Unique(StoryCraftDialogNodeId, EdgeId) | Duplicate option Id per node → unique violation |
| NextNodeId | ToNodeId | MaxLength(100) | — |
| JumpToTileId | JumpToTileId | HasMaxLength(100) | Must reference existing TileId in same story |
| SetBranchId | SetBranchId | HasMaxLength(100) | — |
| HideIfBranchSet | HideIfBranchSet | HasMaxLength(100) | — |
| ShowOnlyIfBranchesSet | ShowOnlyIfBranchesSet | Stored as JSON string | No max in config |
| **Tokens** | StoryCraftDialogEdgeToken / StoryDialogEdgeToken | **Type max 100, Value max 200** | **Stricter than answer tokens; null/empty trimmed to "" in StoryTileUpdater** |

---

## 6. Token DTO (EditableTokenDto) – used in both answers and dialog options

| DTO field | Answer token (published) | Dialog edge token (published) | Notes |
|-----------|---------------------------|-------------------------------|--------|
| Type | StoryAnswerToken.Type max **64** | StoryDialogEdgeToken.Type max **100** | Answer tokens stricter; admin-preselected values must fit 64/128 |
| Value | StoryAnswerToken.Value max **128** | StoryDialogEdgeToken.Value max **200** | Same |
| Quantity | Quantity (int) | Quantity (int) | — |

At **save** (craft): StoryAnswerUpdater uses `Type ?? "Personality"`, `Value ?? ""`. StoryTileUpdater uses `Type?.Trim() ?? ""`, `Value?.Trim() ?? ""` for dialog edge tokens.  
At **publish**: StoryPublishingService copies craft token Type/Value 1:1; no truncation. So craft can contain strings within craft limits; if they exceed **published** limits (64/128 for answers, 100/200 for edges), SaveChangesAsync on definition tables fails.

---

## 7. Uniqueness indexes (duplicate-ID failure modes)

- **StoryCraft** / **StoryDefinition**: StoryId unique.
- **StoryCraftTile** / **StoryTile**: (StoryCraftId / StoryDefinitionId, TileId) unique.
- **StoryCraftAnswer** / **StoryAnswer**: (StoryCraftTileId / StoryTileId, AnswerId) unique.
- **StoryCraftAnswerTranslation** / **StoryAnswerTranslation**: (StoryCraftAnswerId / StoryAnswerId, LanguageCode) unique.
- **StoryCraftTileTranslation** / **StoryTileTranslation**: (StoryCraftTileId / StoryTileId, LanguageCode) unique.
- **StoryCraftDialogParticipant** / **StoryDialogParticipant**: (StoryCraftId / StoryDefinitionId, HeroId) unique.
- **StoryCraftDialogTile** / **StoryDialogTile**: StoryCraftTileId / StoryTileId unique (1:1).
- **StoryCraftDialogNode** / **StoryDialogNode**: (StoryCraftDialogTileId / StoryDialogTileId, NodeId) unique.
- **StoryCraftDialogNodeTranslation** / **StoryDialogNodeTranslation**: (StoryCraftDialogNodeId / StoryDialogNodeId, LanguageCode) unique.
- **StoryCraftDialogEdge** / **StoryDialogEdge**: (StoryCraftDialogNodeId / StoryDialogNodeId, EdgeId) unique.
- **StoryCraftDialogEdgeTranslation** / **StoryDialogEdgeTranslation**: (StoryCraftDialogEdgeId / StoryDialogEdgeId, LanguageCode) unique.

---

## 8. References

- **EditableStoryDtos**: `Features/Story-Editor/Services/Content/EditableStoryDtos.cs`
- **StoryEditorService**: `Features/Story-Editor/Services/StoryEditorService.cs`
- **StoryTileUpdater**: `Features/Story-Editor/Services/Content/StoryTileUpdater.cs`
- **StoryAnswerUpdater**: `Features/Story-Editor/Services/Content/StoryAnswerUpdater.cs`
- **StoryPublishingService**: `Features/Story-Editor/Services/StoryPublishingService.cs`
- **Configurations**: `Data/Configurations/` (StoryCraftConfiguration, StoryTileConfiguration, StoryAnswerTokenConfiguration, StoryDialogConfiguration, etc.)
