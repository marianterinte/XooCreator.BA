# StoryHeroes Refactoring Plan

## 📋 Overview

Acest document descrie refactoring-ul complet al sistemului StoryHeroes pentru a îmbunătăți organizarea și scalabilitatea.

## 🎯 Objetive

1. **Separare JSON-uri**: Fiecare erou va avea propriul fișier JSON în loc de un singur fișier centralizat
2. **Independență eroi**: Eroii sunt independenți de stories - story-ul decide ce eroi deblochează prin `unlockedStoryHeroes`
3. **Eliminare unlockConditions**: Nu mai avem nevoie de `UnlockConditionJson` în StoryHero pentru că deblocarea se face direct din story
4. **Endpoint dedicat**: Creez endpoint nou pentru story editor să încarce toți eroii disponibili

## 📁 Structura Nouă

### Fișiere JSON Separate
```
Data/SeedData/StoryHeroes/
  ├── grubot.json
  ├── linkaro.json
  └── puf-puf.json
```

### Format JSON pentru fiecare erou:
```json
{
  "heroId": "grubot",
  "imageUrl": "images/tol/stories/mechanika-s1/heroes/grubot.png"
}
```

### Traduceri (rămân neschimbate):
```
Data/SeedData/Translations/{locale}/story-heroes.json
```

## 🔄 Modificări în Cod

### 1. StoryHero Entity
**Fișier**: `Data/Entities/StoryHero.cs`
- `UnlockConditionJson` → devine nullable/optional (pentru backward compatibility)
- Eliminăm sau comentăm logica legată de `UnlockConditionJson`

### 2. StoryHeroSeedData DTO
**Fișier**: `Data/SeedData/DTOs/StoryHeroSeedData.cs`
- Eliminăm `UnlockConditions` din DTO
- Simplificăm structura

### 3. SeedDataService
**Fișier**: `Data/SeedDataService.cs`
- Modificăm `LoadStoryHeroesAsync()` să scaneze folderul `StoryHeroes/` și să încarce fiecare JSON
- Eliminăm `LoadStoryHeroUnlocksAsync()` - nu mai e nevoie

### 4. TreeOfLightService
**Fișier**: `Features/TreeOfLight/TreeOfLightService.cs`
- Simplificăm `CheckAndUnlockHeroesAsync()` - eliminăm logica cu `UnlockConditionJson`
- Folosim doar `unlockedStoryHeroes` din story JSON (liniile 177-199)

### 5. TreeModelService
**Fișier**: `Features/TreeOfLight/TreeModelService.cs`
- Simplificăm `EvaluateUnlockedHeroesAsync()` - eliminăm logica cu `UnlockConditionJson`
- Folosim doar `unlockedStoryHeroes` din story JSON

### 6. XooDbContext
**Fișier**: `Data/XooDbContext.cs`
- Eliminăm seeding-ul pentru `StoryHeroUnlock` (nu mai e nevoie)
- Păstrăm doar seeding-ul pentru `StoryHero`

### 7. Endpoint Nou pentru Story Editor
**Fișier nou**: `Features/Stories/Endpoints/GetStoryEditorHeroesEndpoint.cs`
- Endpoint: `GET /api/{locale}/story-editor/heroes`
- Returnează toți eroii disponibili din DB cu informații complete (inclusiv traduceri)

## 📝 Fișiere Afectate

### Entities
- ✅ `Data/Entities/StoryHero.cs` - modificare
- ❌ `Data/Entities/StoryHeroUnlock.cs` - poate rămâne pentru viitor, dar nu mai e folosit la seeding

### DTOs
- ✅ `Data/SeedData/DTOs/StoryHeroSeedData.cs` - modificare
- ❌ `Data/SeedData/DTOs/StoryHeroSeedData.cs` - eliminăm `UnlockConditions`

### Services
- ✅ `Data/SeedDataService.cs` - modificare
- ✅ `Features/TreeOfLight/TreeOfLightService.cs` - simplificare
- ✅ `Features/TreeOfLight/TreeModelService.cs` - simplificare

### Database Context
- ✅ `Data/XooDbContext.cs` - modificare seeding

### Endpoints (nou)
- ✅ `Features/Stories/Endpoints/GetStoryEditorHeroesEndpoint.cs` - nou

### Seed Data
- ✅ `Data/SeedData/StoryHeroes/*.json` - nou (fișiere separate)
- ❌ `Data/SeedData/SharedConfigs/story-heroes.json` - șters (după migrare)

## 🔍 Locuri unde StoryHero este folosit

### Seeding
1. `XooDbContext.SeedStoryHeroesDataFromJson()` - modificat
2. `SeedDataService.LoadStoryHeroesAsync()` - modificat
3. `SeedDataService.LoadStoryHeroUnlocksAsync()` - eliminat

### Business Logic
1. `TreeOfLightService.CheckAndUnlockHeroesAsync()` - simplificat
2. `TreeModelService.EvaluateUnlockedHeroesAsync()` - simplificat
3. `TreeOfLightRepository.GetStoryHeroesAsync()` - rămâne neschimbat

### Endpoints
1. `GetUserBestiaryEndpoint` - folosește StoryHero pentru imagini (rămâne neschimbat)

## 🚀 Pași de Implementare

1. ✅ Creez documentația
2. ⏳ Creez folderul `StoryHeroes/` și JSON-urile separate
3. ⏳ Modific StoryHero entity
4. ⏳ Actualizez DTOs
5. ⏳ Modific SeedDataService
6. ⏳ Simplific logica în TreeOfLightService
7. ⏳ Simplific logica în TreeModelService
8. ⏳ Actualizez XooDbContext
9. ⏳ Creez endpoint pentru story editor
10. ⏳ Șterg fișierul vechi story-heroes.json

## 📌 Note Importante

- **Backward Compatibility**: Păstrăm `UnlockConditionJson` nullable pentru a nu afecta migrațiile existente
- **StoryHeroUnlock**: Entitatea rămâne în DB dar nu mai e folosită la seeding
- **Traduceri**: Rămân neschimbate în `Translations/{locale}/story-heroes.json`
- **Stories**: Continuă să folosească `unlockedStoryHeroes` array pentru a specifica ce eroi deblochează

## ✅ Status Refactoring

- [x] Documentația creată
- [x] JSON-uri separate create
- [x] StoryHero entity modificat
- [x] DTOs actualizate
- [x] SeedDataService modificat
- [x] TreeOfLightService simplificat
- [x] TreeModelService simplificat
- [x] XooDbContext actualizat
- [x] Endpoint story editor creat
- [x] Fișier vechi șters
- [ ] Testat și validat (necesită testare manuală)

## 📅 Data: 2025-01-27

## 🎉 Implementare Completă

Refactoring-ul a fost completat cu succes! Toate modificările au fost implementate:

1. ✅ JSON-uri separate create pentru fiecare erou în `StoryHeroes/`
2. ✅ StoryHero entity actualizat (UnlockConditionJson nullable)
3. ✅ SeedDataService modificat să încarce din JSON-uri separate
4. ✅ Logica simplificată în TreeOfLightService și TreeModelService
5. ✅ Endpoint nou creat pentru story editor: `/api/{locale}/story-editor/heroes`
6. ✅ Fișier vechi șters: `SharedConfigs/story-heroes.json`

### Endpoint Nou

**GET** `/api/{locale}/story-editor/heroes`

Returnează lista tuturor eroilor disponibili pentru story editor cu informații complete:
- `heroId`: ID-ul eroului
- `name`: Numele tradus
- `description`: Descrierea tradusă
- `story`: Povestea tradusă
- `imageUrl`: URL-ul imaginii

### Note Importante

- **Backward Compatibility**: `UnlockConditionJson` este nullable pentru a nu afecta migrațiile existente
- **Traduceri**: Rămân în `Translations/{locale}/story-heroes.json` (neschimbate)
- **Stories**: Continuă să folosească `unlockedStoryHeroes` array pentru a specifica ce eroi deblochează

---

**Ultima actualizare**: 2025-01-27
**Autor**: Refactoring pentru StoryHeroes System
**Status**: ✅ Implementat - Necesită testare

