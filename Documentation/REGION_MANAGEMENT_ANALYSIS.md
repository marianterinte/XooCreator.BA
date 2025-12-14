# Analiză Completă - Region Management pentru Heroes Implementation

## 📋 Overview

Acest document oferă o analiză completă a sistemului de **Region Management** pentru a servi ca blueprint pentru implementarea **Heroes Management**. Sistemul de regiuni este o entitate independentă, reutilizabilă, cu workflow complet de review și publish.

---

## 🏗️ Arhitectură Generală

### Concepte Cheie

1. **Entități Independente**: Regions și Heroes sunt entități separate, nu sunt legate direct de Epic-uri
2. **Reutilizare**: O regiune/hero publicată poate fi folosită în multiple epic-uri
3. **Workflow Similar cu StoryCraft**: `draft → sent_for_approval → in_review → approved → published`
4. **Publish Sincron**: Nu folosesc background jobs (lightweight entities, doar poze și conținut media)
5. **Multi-limbă**: Suport pentru traduceri în multiple limbi

---

## 📊 Database Schema

### StoryRegion Entity

```sql
CREATE TABLE "StoryRegions" (
    "Id" VARCHAR(100) PRIMARY KEY,                    -- e.g., "terra-region-20250115"
    "Name" VARCHAR(200) NOT NULL,                      -- Label/Display name
    "ImageUrl" VARCHAR(500),                          -- Azure Blob path
    "OwnerUserId" UUID NOT NULL,                       -- FK to AlchimaliaUsers
    "Status" VARCHAR(20) NOT NULL DEFAULT 'draft',    -- Workflow status
    "CreatedAt" TIMESTAMP NOT NULL,
    "UpdatedAt" TIMESTAMP NOT NULL,
    "PublishedAtUtc" TIMESTAMP,
    
    -- Review workflow fields
    "AssignedReviewerUserId" UUID,                     -- FK to AlchimaliaUsers
    "ReviewNotes" TEXT,
    "ReviewStartedAt" TIMESTAMP,
    "ReviewEndedAt" TIMESTAMP,
    "ReviewedByUserId" UUID,                          -- FK to AlchimaliaUsers
    "ApprovedByUserId" UUID,                          -- FK to AlchimaliaUsers
    
    -- Constraints
    UNIQUE ("OwnerUserId", "Id"),
    FOREIGN KEY ("OwnerUserId") REFERENCES "AlchimaliaUsers" ("Id") ON DELETE CASCADE
);
```

### StoryRegionTranslation Entity

```sql
CREATE TABLE "StoryRegionTranslations" (
    "Id" INTEGER PRIMARY KEY,
    "RegionId" VARCHAR(100) NOT NULL,                 -- FK to StoryRegions
    "LanguageCode" VARCHAR(10) NOT NULL,              -- e.g., "ro-ro", "en-us"
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    
    UNIQUE ("RegionId", "LanguageCode"),
    FOREIGN KEY ("RegionId") REFERENCES "StoryRegions" ("Id") ON DELETE CASCADE
);
```

### StoryEpicRegionReference (Junction Table)

```sql
CREATE TABLE "StoryEpicRegionReferences" (
    "Id" INTEGER PRIMARY KEY,
    "EpicId" VARCHAR(100) NOT NULL,                   -- FK to StoryEpicDefinitions
    "RegionId" VARCHAR(100) NOT NULL,                 -- FK to StoryRegions
    "X" DOUBLE PRECISION,                             -- Position in tree logic
    "Y" DOUBLE PRECISION,
    "SortOrder" INTEGER DEFAULT 0,
    "IsLocked" BOOLEAN DEFAULT false,
    "IsStartupRegion" BOOLEAN DEFAULT false,
    
    UNIQUE ("EpicId", "RegionId"),
    FOREIGN KEY ("RegionId") REFERENCES "StoryRegions" ("Id") ON DELETE CASCADE
);
```

**Note pentru Heroes**: Va fi similar, dar cu `StoryEpicHeroReference` și câmpuri specifice pentru heroes (ex: `GreetingText`, `GreetingAudioUrl`).

---

## 🔧 Backend Architecture

### 1. Entity (C#)

**Locație**: `XooCreator.BA/Data/Entities/StoryRegion.cs`

```csharp
public class StoryRegion
{
    [MaxLength(100)]
    public required string Id { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
    
    public Guid OwnerUserId { get; set; }
    
    [MaxLength(20)]
    public required string Status { get; set; } = "draft";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
    
    // Review workflow
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    
    // Navigation
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<StoryEpicRegionReference> EpicReferences { get; set; }
    public ICollection<StoryRegionTranslation> Translations { get; set; }
}
```

**Diferențe pentru Heroes**:
- Adăugare: `GreetingText` (TEXT), `GreetingAudioUrl` (VARCHAR(500))
- Navigation: `StoryEpicHeroReference` în loc de `StoryEpicRegionReference`

### 2. Repository Interface

**Locație**: `XooCreator.BA/Features/Story-Editor/Story-Epic/Repositories/IStoryRegionRepository.cs`

```csharp
public interface IStoryRegionRepository
{
    Task<StoryRegion?> GetAsync(string regionId, CancellationToken ct = default);
    Task<StoryRegion> CreateAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default);
    Task SaveAsync(StoryRegion region, CancellationToken ct = default);
    Task DeleteAsync(string regionId, CancellationToken ct = default);
    Task<List<StoryRegion>> ListByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default);
    Task<List<StoryRegion>> ListForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default);
}
```

### 3. Service Interface

**Locație**: `XooCreator.BA/Features/Story-Editor/Story-Epic/Services/IStoryRegionService.cs`

```csharp
public interface IStoryRegionService
{
    // CRUD Operations
    Task<StoryRegionDto?> GetRegionAsync(string regionId, CancellationToken ct = default);
    Task<StoryRegionDto> CreateRegionAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default);
    Task SaveRegionAsync(Guid ownerUserId, string regionId, StoryRegionDto dto, CancellationToken ct = default);
    Task DeleteRegionAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
    
    // Listing
    Task<List<StoryRegionListItemDto>> ListRegionsByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default);
    Task<List<StoryRegionListItemDto>> ListRegionsForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default);
    
    // Workflow
    Task SubmitForReviewAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerUserId, string regionId, bool approve, string? notes, CancellationToken ct = default);
    Task PublishAsync(Guid ownerUserId, string regionId, string ownerEmail, CancellationToken ct = default);
    Task RetractAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
}
```

**Note pentru Heroes**: Același pattern, dar cu `IStoryHeroService` și metodele corespunzătoare.

### 4. Endpoints (Minimal API)

#### Create Region
- **Route**: `POST /api/story-editor/regions`
- **Request**: `{ regionId?: string, name: string }`
- **Response**: `{ regionId: string }`
- **Auth**: Creator role required
- **Logic**: Auto-generate ID dacă nu e furnizat: `{slug}-region-{YYYYMMDD}`

#### List Regions
- **Route**: `GET /api/story-editor/regions?status={status}`
- **Response**: `StoryRegionListItemDto[]`
- **Auth**: Authenticated user
- **Logic**: Returnează own + published regions

#### Get Region
- **Route**: `GET /api/story-editor/regions/{regionId}`
- **Response**: `StoryRegionDto`
- **Auth**: Authenticated user

#### Update Region
- **Route**: `PUT /api/story-editor/regions/{regionId}`
- **Request**: `StoryRegionDto` (partial)
- **Response**: `{ ok: boolean }`
- **Auth**: Owner or Admin

#### Delete Region
- **Route**: `DELETE /api/story-editor/regions/{regionId}`
- **Response**: `{ ok: boolean }`
- **Auth**: Owner or Admin

#### Submit for Review
- **Route**: `POST /api/story-editor/regions/{regionId}/submit`
- **Response**: `{ ok: boolean, status: string }`
- **Auth**: Owner
- **Logic**: `draft` → `sent_for_approval`

#### Claim Review
- **Route**: `POST /api/story-editor/regions/{regionId}/claim`
- **Response**: `{ ok: boolean, status: string }`
- **Auth**: Reviewer role
- **Logic**: `sent_for_approval` → `in_review`, setează `AssignedReviewerUserId`

#### Review (Approve/Reject)
- **Route**: `POST /api/story-editor/regions/{regionId}/review`
- **Request**: `{ approve: boolean, notes?: string }`
- **Response**: `{ ok: boolean, status: string }`
- **Auth**: Assigned Reviewer
- **Logic**: 
  - `approve = true`: `in_review` → `approved`
  - `approve = false`: `in_review` → `changes_requested`

#### Publish
- **Route**: `POST /api/story-editor/regions/{regionId}/publish`
- **Response**: `{ ok: boolean, status: string, publishedAtUtc?: DateTime }`
- **Auth**: Owner (doar dacă status = `approved`)
- **Logic**: 
  - `approved` → `published`
  - Copiază assets din container privat în container public
  - Actualizează `PublishedAtUtc`

#### Retract
- **Route**: `POST /api/story-editor/regions/{regionId}/retract`
- **Response**: `{ ok: boolean, status: string }`
- **Auth**: Owner
- **Logic**: `sent_for_approval`/`in_review`/`approved` → `draft`

#### Image Upload
- **Route**: `POST /api/story-editor/regions/{regionId}/image-upload`
- **Response**: `{ sasUrl: string, blobPath: string }`
- **Auth**: Owner
- **Logic**: Generează SAS token pentru upload direct în Azure Blob Storage

---

## 🎨 Frontend Architecture

### 1. Types (TypeScript)

**Locație**: `xoo-creator/src/app/types/story-epic.types.ts`

```typescript
// Full region DTO (with translations)
export interface StoryRegionDto {
  id: string;
  imageUrl?: string | null;
  status: string; // "draft" | "sent_for_approval" | "in_review" | "approved" | "changes_requested" | "published" | "archived"
  createdAt: string; // ISO date
  updatedAt: string; // ISO date
  publishedAtUtc?: string | null;
  translations: StoryRegionTranslationDto[];
  // Review workflow fields
  assignedReviewerUserId?: string | null;
  reviewNotes?: string | null;
  reviewStartedAt?: string | null;
  reviewEndedAt?: string | null;
  reviewedByUserId?: string | null;
  approvedByUserId?: string | null;
}

// List item (simplified)
export interface StoryRegionListItemDto {
  id: string;
  name: string; // Name in requested/default language
  imageUrl?: string | null;
  status: string;
  createdAt: string;
  updatedAt: string;
  publishedAtUtc?: string | null;
  assignedReviewerUserId?: string | null;
  isAssignedToCurrentUser?: boolean;
  isOwnedByCurrentUser?: boolean;
}

// Translation
export interface StoryRegionTranslationDto {
  languageCode: string;
  name: string;
  description?: string | null;
}

// Workflow responses
export interface SubmitStoryRegionResponse {
  ok: boolean;
  status: string;
}

export interface ReviewStoryRegionRequest {
  approve: boolean;
  notes?: string | null;
}

export interface ReviewStoryRegionResponse {
  ok: boolean;
  status: string;
}

export interface PublishStoryRegionResponse {
  ok: boolean;
  status: string;
  publishedAtUtc?: string | null;
}
```

**Note pentru Heroes**: Similar, dar cu `StoryHeroDto`, `StoryHeroListItemDto`, etc., și câmpuri specifice (`greetingText`, `greetingAudioUrl`).

### 2. Service (Angular)

**Locație**: `xoo-creator/src/app/services/story-region.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class StoryRegionService {
  // CRUD
  getRegion(regionId: string): Observable<StoryRegionDto>
  listRegions(status?: string): Observable<StoryRegionListItemDto[]>
  createRegion(regionId: string, name: string): Observable<{ regionId: string }>
  updateRegion(regionId: string, dto: Partial<StoryRegionDto>): Observable<{ ok: boolean }>
  deleteRegion(regionId: string): Observable<{ ok: boolean }>
  
  // Workflow
  submit(regionId: string): Observable<SubmitStoryRegionResponse>
  claim(regionId: string): Observable<{ ok: boolean; status: string }>
  review(regionId: string, approve: boolean, notes?: string): Observable<ReviewStoryRegionResponse>
  publish(regionId: string): Observable<PublishStoryRegionResponse>
  retract(regionId: string): Observable<{ ok: boolean; status: string }>
}
```

**Note**: Toate endpoint-urile sunt language-agnostic (nu trimit `Accept-Language` header, backend returnează toate traducerile).

### 3. UI Components

#### Main List Component
**Locație**: `xoo-creator/src/app/story/story-editor-list/story-editor-list.component.ts`

**Funcționalități**:
- Dropdown pentru selectarea entității: "Povești", "Epicuri", "Regiuni", "Eroi"
- Tabs pentru filtrare: "În Lucru", "Publicate", "Sarcinile Mele", "Disponibile pentru Revendicare"
- Card-uri pentru fiecare regiune cu:
  - Image preview
  - Title și ID
  - Status badge (DRAFT, PUBLISHED, etc.)
  - Ownership badge ("A mea")
  - Action buttons: Edit, Publish, Delete
- Modal pentru creare/editare regiune
- Modal pentru review (approve/reject)
- Modal pentru confirmare delete

**State Management**:
- `regions = signal<StoryRegionListItemDto[]>([])`
- `selectedEntityType = signal<'stories' | 'epics' | 'regions' | 'heroes'>('regions')`
- `selectedStatusTab = signal<string>('draft')` // "draft", "published", "my-tasks", "available-for-claim"

#### Region Form Component
**Locație**: `xoo-creator/src/app/story/story-epic-editor/components/story-epic-region-form/`

**Funcționalități**:
- Wizard în 2 pași:
  1. **Step 1: Region Name**
     - Language selector (reusable component)
     - Region Label input
     - Auto-generated Region ID
  2. **Step 2: Image Upload**
     - Image preview
     - Upload button
     - Remove button
- Language management cu componenta reutilizabilă `LanguageSelectorComponent`
- Image upload cu refresh automat al UI-ului

**Note pentru Heroes**: Similar, dar cu Step 2 pentru Greeting Text și Audio Upload.

#### Language Selector Component (Reusable)
**Locație**: `xoo-creator/src/app/story/story-epic-editor/components/language-selector/`

**Funcționalități**:
- Afișează limbile disponibile ca badges
- Dropdown pentru adăugare limbă ("+ Add")
- Dropdown pentru selectarea limbii de editare
- Remove language (dacă mai mult de 1 limbă)

---

## 🔄 Workflow States

### Status Flow

```
draft
  ↓ (Submit)
sent_for_approval
  ↓ (Claim by Reviewer)
in_review
  ↓ (Review)
  ├─→ approved → (Publish) → published
  └─→ changes_requested → (Retract) → draft
```

### Status Meanings

- **draft**: Work in progress, owner poate edita
- **sent_for_approval**: Trimis pentru review, așteaptă reviewer
- **in_review**: Reviewer a preluat review-ul
- **approved**: Aprobat, gata pentru publish
- **changes_requested**: Necesită modificări, owner trebuie să retracteze și să editeze
- **published**: Publicat, poate fi folosit în epic-uri
- **archived**: Arhivat (nu e folosit în workflow normal)

---

## 📦 Assets Management

### Image Upload Flow

1. **Request SAS Token**: `POST /api/story-editor/regions/{regionId}/image-upload`
   - Backend verifică ownership
   - Generează SAS token pentru container privat: `regions/{epicId}/{regionId}/`
   - Returnează `{ sasUrl: string, blobPath: string }`

2. **Direct Upload**: Frontend upload direct în Azure Blob Storage folosind SAS token

3. **Save Region**: Frontend salvează `imageUrl = blobPath` în region DTO

4. **Publish**: La publish, backend copiază assets din container privat în container public

**Service**: `StoryEpicRegionAssetsService`
- `uploadImage(epicId: string, regionId: string, file: File): Promise<{ blobPath: string, previewUrl: string }>`
- `resolvePreviewUrl(blobPath: string): Promise<string>`

**Note pentru Heroes**: Similar, dar cu `StoryEpicHeroAssetsService` și path-uri `heroes/{epicId}/{heroId}/`.

---

## 🔐 Authorization & Permissions

### Roles

- **Creator**: Poate crea, edita, submit, publish own regions
- **Admin**: Poate face orice
- **Reviewer**: Poate claim și review regions

### Permission Checks

- **Create/Edit/Delete**: Owner sau Admin
- **Submit**: Owner (doar dacă status = `draft` sau `changes_requested`)
- **Claim**: Reviewer (doar dacă status = `sent_for_approval` și nu e deja assigned)
- **Review**: Assigned Reviewer (doar dacă status = `in_review`)
- **Publish**: Owner (doar dacă status = `approved`)
- **Retract**: Owner (doar dacă status = `sent_for_approval`, `in_review`, sau `approved`)

---

## 🎯 Mapping pentru Heroes Implementation

### Naming Conventions

| Region | Hero |
|--------|------|
| `StoryRegion` | `StoryHero` |
| `StoryRegionTranslation` | `StoryHeroTranslation` |
| `StoryEpicRegionReference` | `StoryEpicHeroReference` |
| `IStoryRegionService` | `IStoryHeroService` |
| `StoryRegionService` | `StoryHeroService` |
| `StoryRegionRepository` | `StoryHeroRepository` |
| `/api/story-editor/regions` | `/api/story-editor/heroes` |
| `StoryRegionDto` | `StoryHeroDto` |
| `StoryRegionListItemDto` | `StoryHeroListItemDto` |
| `StoryRegionService` (Angular) | `StoryHeroService` (Angular) |

### Additional Fields pentru Heroes

**Entity**:
- `GreetingText` (TEXT) - Textul de salut al eroului
- `GreetingAudioUrl` (VARCHAR(500)) - URL-ul audio pentru salut

**DTO**:
```typescript
export interface StoryHeroDto {
  // ... same as StoryRegionDto
  greetingText?: string | null;
  greetingAudioUrl?: string | null;
  translations: StoryHeroTranslationDto[]; // Name + Description + GreetingText per language
}
```

**UI Form**:
- Step 1: Hero Name (similar cu Region Name)
- Step 2: Image Upload (similar)
- Step 3: Greeting Text & Audio Upload (nou)

### Database Schema pentru Heroes

```sql
CREATE TABLE "EpicHeroes" (
    -- Same as StoryRegions, plus:
    "GreetingText" TEXT,
    "GreetingAudioUrl" VARCHAR(500),
    -- ... rest same
);

CREATE TABLE "StoryEpicHeroReferences" (
    -- Similar to StoryEpicRegionReferences
    "EpicId" VARCHAR(100),
    "HeroId" VARCHAR(100),
    -- ... position, sort order, etc.
);
```

---

## 📝 Checklist pentru Heroes Implementation

### Backend

- [ ] Create `StoryHero` entity (similar cu `StoryRegion`)
- [ ] Create `StoryHeroTranslation` entity
- [ ] Create `StoryEpicHeroReference` entity (junction table)
- [ ] Create migration script (similar cu `V0025__add_story_regions_and_epic_heroes.sql`)
- [ ] Create `IStoryHeroRepository` interface
- [ ] Create `StoryHeroRepository` implementation
- [ ] Create `IStoryHeroService` interface
- [ ] Create `StoryHeroService` implementation
- [ ] Create all endpoints:
  - [ ] `CreateStoryHeroEndpoint`
  - [ ] `ListStoryHeroesEndpoint`
  - [ ] `GetStoryHeroEndpoint`
  - [ ] `UpdateStoryHeroEndpoint`
  - [ ] `DeleteStoryHeroEndpoint`
  - [ ] `SubmitStoryHeroEndpoint`
  - [ ] `ClaimStoryHeroReviewEndpoint`
  - [ ] `ReviewStoryHeroEndpoint`
  - [ ] `PublishStoryHeroEndpoint`
  - [ ] `RetractStoryHeroEndpoint`
  - [ ] `RequestStoryHeroImageUploadEndpoint`
  - [ ] `RequestStoryHeroAudioUploadEndpoint` (nou)
- [ ] Create `StoryEpicHeroAssetsService` (pentru image și audio upload)
- [ ] Update `StoryEpicService` pentru a include heroes în epic DTO

### Frontend

- [ ] Add types în `story-epic.types.ts`:
  - [ ] `StoryHeroDto`
  - [ ] `StoryHeroListItemDto`
  - [ ] `StoryHeroTranslationDto`
  - [ ] Workflow response types
- [ ] Create `StoryHeroService` (similar cu `StoryRegionService`)
- [ ] Create `StoryEpicHeroAssetsService` (pentru image și audio)
- [ ] Update `story-editor-list.component.ts`:
  - [ ] Add "Eroi" în entity type dropdown
  - [ ] Add heroes state management
  - [ ] Add heroes listing logic
  - [ ] Add heroes card rendering
  - [ ] Add heroes action handlers
- [ ] Create `story-epic-hero-form` component:
  - [ ] Step 1: Hero Name (cu language selector)
  - [ ] Step 2: Image Upload
  - [ ] Step 3: Greeting Text & Audio Upload
- [ ] Create `story-epic-hero-item` component (pentru card display)
- [ ] Update `story-epic-editor` pentru a include heroes tab
- [ ] Update `story-epic-tree-logic-tab` pentru a afișa heroes în tree

### Testing

- [ ] Test CRUD operations
- [ ] Test workflow (submit → claim → review → publish)
- [ ] Test multi-language support
- [ ] Test image upload
- [ ] Test audio upload (nou)
- [ ] Test authorization (owner, reviewer, admin)
- [ ] Test integration cu epic-uri (adăugare hero în epic)

---

## 🚀 Next Steps

1. **Review acest document** cu echipa
2. **Prioritize features** (ex: audio upload poate fi Phase 2)
3. **Create tickets** pentru fiecare componentă
4. **Start cu Backend** (entities, repositories, services)
5. **Apoi Frontend** (types, services, components)
6. **Testing & Integration**

---

## 📚 Referințe

- **Region Implementation**: `XooCreator.BA/Features/Story-Editor/Story-Epic/`
- **Frontend Components**: `xoo-creator/src/app/story/story-epic-editor/components/`
- **Database Scripts**: `XooCreator.BA/Database/Scripts/V0025__add_story_regions_and_epic_heroes.sql`
- **Documentation**: `XooCreator/002.Documentation/Story-Epic/`

---

**Document creat**: 2025-01-14  
**Ultima actualizare**: 2025-01-14
