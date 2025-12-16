# Analiză Bug-uri Epic Publish și New Version

## Data analizei
2025-01-XX

## Probleme Raportate

1. **Bug 1:** După "new version", când se încearcă publish din nou, draft-ul nu se șterge și versiunea nu se publică corect
2. **Bug 2:** Când se dă "new version", se șterge versiunea publicată sau se face update aiurea (poate a rămas logica veche)

---

## 🔍 Analiză Flow-ul Complet

### Flow 1: Epic Nou → Approve → Publish (✅ FUNCȚIONEAZĂ)

**Pași:**
1. Se creează `StoryEpicCraft` cu status = "draft"
2. Se aprobă (status = "approved")
3. Se publică:
   - `PublishStoryEpicEndpoint.HandlePost()` creează `EpicPublishJob`
   - `EpicPublishQueueJob` procesează job-ul
   - `PublishFromCraftAsync()` este apelat:
     - `definition == null` → `isNew = true`
     - Creează `StoryEpicDefinition` nou (Version = 1)
     - `forceFull || isNew` = true → șterge conținuturile existente (linia 408)
     - Copiază regions, nodes, rules, translations din craft
   - Worker-ul șterge `StoryEpicCraft` după publish (linia 157)

**Rezultat:** ✅ Epic publicat corect, craft șters

---

### Flow 2: New Version (✅ FUNCȚIONEAZĂ)

**Pași:**
1. Există `StoryEpicDefinition` cu status = "published", Version = 1
2. Se apelează `CreateVersionFromPublishedAsync()`:
   - Verifică că definition există și este published
   - Verifică că NU există deja un craft (linia 343-347)
   - Creează `StoryEpicCraft` nou cu:
     - `Id = definition.Id` (același ID)
     - `Status = "draft"`
     - `BaseVersion = definition.Version` (linia 361)
     - `LastDraftVersion = 0` (linia 362)
   - Copiază toate datele din definition în craft

**Rezultat:** ✅ Craft nou creat, definition rămâne neschimbat

---

### Flow 3: Publish După New Version (❌ BUG AICI)

**Pași:**
1. Există `StoryEpicDefinition` (Version = 1, published)
2. Există `StoryEpicCraft` (draft, creat din new version)
3. Se aprobă craft-ul (status = "approved")
4. Se publică:
   - `PublishFromCraftAsync()` este apelat:
     - `definition != null` → `isNew = false` (linia 369)
     - Se face UPDATE la definition existent:
       - `definition.BaseVersion = definition.Version` (linia 394)
       - `definition.Version = definition.Version + 1` (linia 395)
     - **PROBLEMA:** `forceFull || isNew` = `false || false` = **false** (linia 408)
       - **NU se șterg conținuturile existente!**
       - Regions, nodes, rules, translations rămân în definition
     - Se adaugă NOI regions, nodes, rules, translations (liniile 428-482)
     - **REZULTAT: DUPLICARE!** Conținuturile vechi rămân + se adaugă cele noi
   - Worker-ul încearcă să șteargă craft-ul (linia 152-159)

**Rezultat:** ❌ Definition are conținut duplicat, craft poate să nu se șteargă corect

---

## 🐛 Bug-uri Identificate

### Bug 1: Duplicare Conținut la Re-Publish

**Locație:** `StoryEpicPublishingService.PublishFromCraftAsync()` linia 407-425

**Problema:**
```csharp
// Remove existing content if full publish
if (forceFull || isNew)  // ❌ Când isNew = false și forceFull = false, NU se șterge nimic!
{
    // Remove existing regions
    _context.StoryEpicDefinitionRegions.RemoveRange(definition.Regions);
    // ...
}
```

**Când se întâmplă:**
- După "new version", când se publică din nou
- `isNew = false` (definition există deja)
- `forceFull = false` (default)
- Conținuturile vechi NU se șterg
- Se adaugă conținuturi noi peste cele vechi → **DUPLICARE**

**Fix necesar:**
- Când se face re-publish (isNew = false), TREBUIE să se șteargă conținuturile existente
- Sau să se verifice dacă există deja înainte de a adăuga

---

### Bug 2: Logica de Cleanup Craft Poate Să Eșueze

**Locație:** `EpicPublishQueueJob` linia 145-170

**Problema:**
```csharp
// Reload craft in current context to ensure it's tracked
var craftToDelete = await db.StoryEpicCrafts
    .FirstOrDefaultAsync(c => c.Id == craft.Id, stoppingToken);

if (craftToDelete != null)
{
    db.StoryEpicCrafts.Remove(craftToDelete);
    await db.SaveChangesAsync(stoppingToken);
}
```

**Probleme potențiale:**
1. Craft-ul poate fi deja detașat din context după `PublishFromCraftAsync()`
2. Dacă `PublishFromCraftAsync()` folosește un context diferit, craft-ul poate să nu fie găsit
3. Dacă există erori în cleanup, craft-ul rămâne în DB

**Comparație cu Stories:**
- Stories folosesc `crafts.DeleteAsync()` care probabil gestionează mai bine context-ul
- Epic folosește direct `Remove()` + `SaveChangesAsync()`

---

### Bug 3: Lipsă Copiere Assets la Re-Publish

**Locație:** `StoryEpicPublishingService.PublishFromCraftAsync()` - NU copiază assets!

**Problema:**
- `PublishFromCraftAsync()` NU copiază assets (cover image, reward images)
- Doar copiază structura (regions, nodes, etc.)
- Assets-urile rămân în draft container
- Nu se actualizează paths în definition

**Observație:**
- Metoda `CopyEpicAssetsAsync()` există, dar NU este apelată în `PublishFromCraftAsync()`
- Metoda `CollectEpicAssets()` primește `DbStoryEpic`, nu `StoryEpicCraft`
- Trebuie să se copieze assets din craft în published container

---

## 📊 Comparație cu Stories

### Stories - Cum Funcționează Corect

**Publish după New Version:**
1. `StoryPublishingService.UpsertFromCraftAsync()`:
   - Verifică dacă definition există
   - Dacă există, face UPDATE (incrementează Version)
   - **Șterge conținuturile existente** (tiles, translations, etc.)
   - Adaugă conținuturi noi din craft
   - **Copiază assets** (cover, tiles, audio, video)
2. Worker-ul șterge craft-ul după publish

**Diferențe:**
- Stories au logica de cleanup conținuturi în `ApplyFullPublishAsync()`
- Stories copiază assets în `UpsertFromCraftAsync()`
- Stories folosesc repository pentru ștergere craft

---

## 🔧 Fix-uri Necesare

### Fix 1: Ștergere Conținuturi Existente la Re-Publish

**Fișier:** `StoryEpicPublishingService.PublishFromCraftAsync()`

**Modificare:**
```csharp
// Remove existing content if full publish OR re-publish
if (forceFull || isNew || !isNew)  // Sau mai simplu: întotdeauna șterge la re-publish
{
    // Remove existing regions
    _context.StoryEpicDefinitionRegions.RemoveRange(definition.Regions);
    definition.Regions.Clear();
    // ... rest of cleanup
}
```

**Sau mai bine:**
```csharp
// Always remove existing content before adding new (for re-publish)
if (!isNew)
{
    // Remove existing regions
    _context.StoryEpicDefinitionRegions.RemoveRange(definition.Regions);
    definition.Regions.Clear();
    // ... rest of cleanup
}
```

---

### Fix 2: Copiere Assets la Publish

**Fișier:** `StoryEpicPublishingService.PublishFromCraftAsync()`

**Modificare:**
- Adaugă apel la `CopyEpicAssetsAsync()` înainte de a salva definition
- Trebuie să colecteze assets din craft (nu din epic)
- Trebuie să actualizeze paths în definition după copiere

**Implementare:**
```csharp
// Collect assets from craft
var assets = CollectEpicAssetsFromCraft(craft);

// Copy assets
var copyResult = await CopyEpicAssetsAsync(assets, requestedByEmail, craft.Id, ct);
if (copyResult.HasError)
{
    throw new InvalidOperationException(copyResult.ErrorMessage);
}

// Update definition with published paths
UpdateDefinitionAssets(definition, assets);
```

---

### Fix 3: Îmbunătățire Cleanup Craft

**Fișier:** `EpicPublishQueueJob`

**Modificare:**
- Folosește același context pentru craft și pentru ștergere
- Sau folosește un repository pattern similar cu stories
- Adaugă verificare că craft-ul este șters cu succes

---

## 📝 Flow Corect (După Fix-uri)

### Publish După New Version (Corect)

1. Există `StoryEpicDefinition` (Version = 1, published)
2. Există `StoryEpicCraft` (draft, creat din new version)
3. Se aprobă craft-ul (status = "approved")
4. Se publică:
   - `PublishFromCraftAsync()`:
     - `isNew = false`
     - Incrementează Version în definition
     - **Șterge conținuturile existente** (regions, nodes, rules, translations)
     - **Colectează assets din craft**
     - **Copiază assets în published container**
     - Adaugă conținuturi noi din craft
     - Actualizează paths în definition
   - Worker-ul șterge craft-ul după publish

**Rezultat:** ✅ Definition actualizat corect, craft șters, assets copiate

---

## ✅ Checklist Fix-uri

- [ ] Fix 1: Ștergere conținuturi existente la re-publish (linia 407-425)
- [ ] Fix 2: Adăugare copiere assets în `PublishFromCraftAsync()`
- [ ] Fix 3: Îmbunătățire cleanup craft în worker
- [ ] Test: Publish epic nou → funcționează
- [ ] Test: New version → funcționează
- [ ] Test: Publish după new version → funcționează (fără duplicare)
- [ ] Test: Assets copiate corect la re-publish
- [ ] Test: Craft șters după publish

---

## 🔍 Observații Suplimentare

1. **Metoda `PublishAsync()` (linia 246):**
   - Această metodă folosește `DbStoryEpic` (vechea arhitectură)
   - Nu este folosită în flow-ul actual (care folosește `PublishFromCraftAsync()`)
   - Poate fi deprecată sau folosită pentru migrare

2. **Metoda `UpdateEpicAfterPublish()` (linia 207):**
   - Folosește `DbStoryEpic` (vechea arhitectură)
   - Nu este folosită în `PublishFromCraftAsync()`
   - Poate fi deprecată

3. **Logica de versioning:**
   - În `PublishFromCraftAsync()` se incrementează Version corect
   - `BaseVersion` este setat corect
   - Nu pare să fie probleme aici

---

## 📚 Referințe

- `StoryEpicPublishingService.PublishFromCraftAsync()` - linia 343-489
- `EpicPublishQueueJob` - linia 105-178
- `StoryEpicService.CreateVersionFromPublishedAsync()` - linia 314-431
- `StoryPublishQueueWorker` - pentru comparație cu stories
