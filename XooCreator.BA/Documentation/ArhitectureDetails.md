## XooCreator Backend Architecture and Organization

### Overview
Backend-ul este organizat pe module de business în folderul `Features/`, fiecare modul având aceeași structură internă:
- Endpoints: expunerea API-urilor REST (atribute `[Route]`, `[Authorize]`)
- Services: logica de business (orchestrare, validări, compunere de rezultate)
- Repositories: acces la date/EF Core + transformări către DTO-uri
- DTOs: contracte de date pentru API (request/response) și transfer intern între straturi

### Conventii generale
- Namespace-uri per strat: `XooCreator.BA.Features.<Module>.<Layer>` (ex: `...Stories.Services`)
- Rute API prefixate cu locale: `/api/{locale}/...`
- Modulul determină segmentul de rută (ex: `stories`, `tales-of-alchimalia/market`, `tree-of-heroes`, `tree-of-light`)
- UI labels nu sunt afectate de redenumirile interne—claritatea este în cod, nu în UI

### Module și structuri

#### 1) Stories
- Path: `Features/Stories`
- Straturi:
  - `Endpoints/` (ex: `GetStoryByIdEndpoint.cs`, `MarkTileAsReadEndpoint.cs`)
  - `Services/StoriesService.cs` + interfața `IStoriesService`
  - `Repositories/StoriesRepository.cs` + interfața `IStoriesRepository`
  - `DTOs/StoriesDtos.cs` (ex: `GetStoryByIdResponse`, `StoryContentDto`, `MarkTileAsRead*`)
- Rute API (exemple):
  - `GET /api/{locale}/stories/{storyId}`
  - `POST /api/{locale}/stories/mark-tile-read`
- Observații:
  - Flow-ul de citire (Tree of Light) rămâne pe `/stories` (extragere conținut, progres user)

#### 2) Tales of Alchimalia - Market (TOA-Market)
- Path: `Features/TalesOfAlchimalia/Market`
- Straturi:
  - `Endpoints/` (ex: `GetMarketplaceStoriesEndpoint`, `GetStoryDetailsEndpoint`, `PurchaseStoryEndpoint`, `AcquireFreeStoryEndpoint`)
  - `Services/StoriesMarketplaceService.cs` + interfața `IStoriesMarketplaceService` (namespace: `...Market.Services`)
  - `Repositories/StoriesMarketplaceRepository.cs` + interfața `IStoriesMarketplaceRepository` (namespace: `...Market.Repositories`)
  - `DTOs/StoriesMarketplaceDtos.cs` (marketplace list, details, purchase, filters)
- Rute API (exemple):
  - `GET /api/{locale}/tales-of-alchimalia/market`
  - `GET /api/{locale}/tales-of-alchimalia/market/details/{storyId}`
  - `POST /api/{locale}/tales-of-alchimalia/market/purchase`
  - `POST /api/{locale}/tales-of-alchimalia/market/acquire-free-story` (body: `{ storyId }`)
- Observații:
  - Separă clar marketplace-ul de `stories` (citire). FE folosește acum bază `.../tales-of-alchimalia/market`.

#### 3) Library
- Path: `Features/Library`
- Straturi:
  - `Endpoints/` (ex: `GetUserOwnedStoriesEndpoint`, `GetUserCreatedStoriesEndpoint`, `GetUserPurchasedStoriesEndpoint`)
  - `DTOs/LibraryDtos.cs` (Owned/Created list responses)
- Rute API (exemple):
  - `GET /api/{locale}/stories/owned`
  - `GET /api/{locale}/stories/created`
  - `GET /api/{locale}/stories/marketplace/purchased` (în viitor se poate alinia sub `library` dacă se dorește)

#### 4) Tree of Heroes
- Path: `Features/TreeOfHeroes`
- Straturi:
  - `Endpoints/` (ex: `GetUserTokensEndpoint`, `TransformToHeroEndpoint`, `GetTreeOfHeroesConfigEndpoint`, etc.)
  - `Services/TreeOfHeroesService.cs` + interfața `ITreeOfHeroesService` (namespace: `...TreeOfHeroes.Services`)
  - `Repositories/TreeOfHeroesRepository.cs` + interfața `ITreeOfHeroesRepository` (namespace: `...TreeOfHeroes.Repositories`)
  - `DTOs/TreeOfHeroesDtos.cs` (tokens, hero definitions, config, transform)
- Rute API (exemple):
  - `GET /api/{locale}/tree-of-heroes/tokens`
  - `GET /api/{locale}/tree-of-heroes/definitions`
  - `POST /api/{locale}/tree-of-heroes/transform-hero`

#### 5) Tree of Light
- Path: `Features/TreeOfLight`
- Straturi:
  - `Endpoints/` (ex: `GetTreeStateEndpoint`, `GetTreeProgressEndpoint`, `CompleteStoryEndpoint`, etc.)
  - `Services/` (namespace: `...TreeOfLight.Services`):
    - `TreeOfLightService.cs` (business flow: complete story, tokens, unlocks)
    - `TreeModelService.cs` (model agregat: state/config/regions/stories/rules)
    - `ITreeOfLightTranslationService.cs`, `TreeOfLightTranslationService.cs`
  - `Repositories/` (namespace: `...TreeOfLight.Repositories`):
    - `TreeOfLightRepository.cs` (progres user, mesaje erou)
    - `TreeModelRepository.cs` (config/model/rules + seeding)
  - `DTOs/` (namespace: `...TreeOfLight.DTOs`):
    - `TreeOfLightDtos.cs` (TreeState/Progress, mesaje erou, complete story)
    - `TokenFamily.cs`
- Rute API (exemple):
  - `GET /api/{locale}/tree-of-light/state`
  - `GET /api/{locale}/tree-of-light/progress`
  - `POST /api/{locale}/tree-of-light/complete-story`
  - `POST /api/{locale}/tree-of-light/reset-progress`

#### 6) Payment
- Path: `Features/Payment`
- Straturi:
  - `Endpoints/PaymentEndpoint.cs` (namespace: `...Payment.Endpoints`)
  - `Services/PaymentService.cs` + interfața `IPaymentService` (namespace: `...Payment.Services`)
  - `DTOs/PaymentDtos.cs` (webhook, status, create intent)
- Rute API (exemple):
  - `POST /api/payment/buymeacoffee/webhook`
  - `GET /api/payment/status/{paymentId}`
  - `POST /api/payment/create`

### Rulare și DI (indicativ)
- Endpoints sunt descoperite via `[Endpoint]` + `XooCreator.BA.Infrastructure.Endpoints` registrar (minimal APIs pattern)
- Serviciile și repository-urile sunt înregistrate în DI în `Program.cs`/registrar intern cu mapare interfață → implementare
- Ordin recomandat pentru seeding/init în startup:
  1) `StoriesService.InitializeStoriesAsync()`
  2) `TreeModelService.InitializeTreeModelAsync()`
  3) `StoriesMarketplaceService.InitializeMarketplaceAsync()` (dacă este activ)

### Conventii suplimentare
- Locale management: locale se propagă în rută (`/api/{locale}/...`), iar în servicii/repository se folosesc parametrii `locale` pentru selectarea traducerilor
- DTO-urile sunt orientate pe scenarii (reading, marketplace, progression) și nu expun direct entitățile EF
- Cheile de traducere pentru Bestiary/mesaje sunt salvate ca keys în DB, rezolvate la runtime prin `TreeOfLightTranslationService`

### Schimbări recente (highlights)
- Marketplace mutat din `Stories` în `TalesOfAlchimalia/Market` (rute noi `.../tales-of-alchimalia/market`)
- `AcquireFreeStory` acceptă acum `storyId` doar în body, nu și în rută
- `Library` consolidează `owned/created` (și `purchased` interogabil prin service marketplace)
- `Stories`, `Library`, `Payment`, `TreeOfHeroes`, `TreeOfLight`, `TalesOfAlchimalia/Market` uniformizate pe structura Endpoints/Services/Repositories/DTOs


