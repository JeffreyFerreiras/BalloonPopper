# PLAN.MD

## Project: Balloon Popping Game (.NET MAUI for iOS)

### AI Development Team

* **Alice Bot** (Lead Developer & Architecture)
* **Brian Bot** (Gameplay Mechanics)
* **Catherine Bot** (UI/UX & Assets)
* **David Bot** (Ads Integration)
* **Emily Bot** (Quality Assurance)

---

## Task Assignments

### Alice Bot

* [x] Set up initial .NET MAUI project structure
* [x] **Define core application architecture and service interfaces (e.g., `IGameStateService`, `IAdService`)**
* [x] **Implement dependency injection framework in `MauiProgram.cs`**
* [x] **Implement game navigation (menus, game modes, settings pages)**
* [x] **Configure build settings for iOS deployment**

### Brian Bot

* [x] **Update existing services to implement the new interfaces from `BalloonPopper.Services.Abstractions`** — adapters present in `AbstractionsImpl`
* [x] **Develop core gameplay services (`BalloonSpawner`, `GameEngine`, `DifficultyManager`) implementing interfaces defined by Alice** — implementations exist in `src/BalloonPopper.Services`
* [x] Implement balloon pop interactions and scoring logic within the `BalloonInteractionService` — `BalloonInteractionService` implemented and raises events
* [ ] Create special balloon models and their effects in `BalloonPopper.Models` — basic `Balloon` model exists; special effects wiring partially implemented, some specific effect types may still need assets/logic
* [x] Build the `GameStateManager` to handle the game's state machine — `GameStateManager` implemented and publishes relevant events
* [x] **Focus solely on the `BalloonPopper.Services` and `BalloonPopper.Models` projects** — changes limited to services and models
* [ ] **Uncomment and register your services in `MauiProgram.cs` once implemented** — DI registration still needs verification / may be commented out in `MauiProgram.cs`

Notes:

* Concrete implementations found:
  * `BalloonSpawner` (spawning logic, spawn timer, balloon type selection)
  * `GameEngine` (main loop, balloon lifecycle, event wiring)
  * `DifficultyManager` (spawn configs and multipliers)
  * `BalloonInteractionService` (tap handling, bomb explosion, power-ups)
  * `GameStateManager` (state machine, scoring, power-ups)
* Adapters/bridges for the abstractions live under `src/BalloonPopper.Services/AbstractionsImpl`
* Remaining work: special balloon-specific behaviors and assets; confirm and enable DI registrations in `MauiProgram.cs`; create/expand unit tests for these services if not already present.

### Catherine Bot

* [ ] **Reference the `BalloonPopper.Services.Abstractions` project in PageModels**
* [ ] **Implement all UI pages and views in `BalloonPopper.Maui/Pages` and `BalloonPopper.Maui/Views`**
* [ ] **Consume services (e.g., `IGameStateService`) via dependency injection in PageModels to link UI to game logic**
* [ ] Create responsive UI layouts and styles for cross-platform compatibility
* [ ] Implement UI animations and interaction effects (e.g., balloon pop animation)
* [ ] **Place the `AdBannerView` control (created by David) on the main game page**

### David Bot

* [ ] **Create an `AdService` class that implements `IAdService` from `BalloonPopper.Services.Abstractions`**
* [ ] **Encapsulate all ad logic into an `AdService` that implements an `IAdService` interface**
* [ ] **Create a self-contained `AdBannerView` control for banner ads**
* [ ] Implement banner, interstitial, and rewarded video ad logic within the `AdService`
* [ ] Handle the premium (no-ads) in-app purchase flow
* [ ] **Register the `AdService` and related components in `MauiProgram.cs`**

### Emily Bot

* [ ] **Write unit tests for all services in `BalloonPopper.Services.UnitTests`**
* [ ] **Write unit tests for all models in `BalloonPopper.Models.UnitTests`**
* [ ] **Write UI-layer tests for PageModels in `BalloonPopper.Maui.UnitTests`**
* [ ] Create integration tests to validate the interaction between services
* [ ] Validate ad integration and monetization flows through automated tests

---

## Development Milestones

* **Sprint 1:** Project scaffolding, core architecture, initial gameplay loop
* **Sprint 2:** Complete game mechanics, ad integration, UI implementation
* **Sprint 3:** Testing automation, performance optimization, iOS-specific features
* **Sprint 4:** Final integration testing, deployment preparation, documentation

---

## Code Quality Standards

* All code must follow SOLID principles and dependency injection patterns
* Comprehensive unit test coverage (>90%) required
* Pure functions preferred for game logic calculations
* Interface-based design for all major components
* Clean separation of concerns between UI, business logic, and data layers

---

## Notes

* Each AI bot maintains independent task execution
* Automated testing pipeline validates all changes
* Continuous integration ensures code quality standards
* All dependencies must be injected for testability

## Alice Bot Progress Summary ✅

**Architecture & Interface Design Completed:**

1. ✅ Created `BalloonPopper.Services.Abstractions` as a separate DLL
2. ✅ Defined all core service interfaces following SOLID principles:
   * `IGameEngine` - Game lifecycle management
   * `IGameStateService` - State management with events
   * `IBalloonSpawner` - Balloon creation with events
   * `IDifficultyManager` - Difficulty progression
   * `IBalloonInteractionService` - User interactions
   * `IAdService` - Advertisement and premium services
3. ✅ Implemented dependency injection framework in `MauiProgram.cs`
4. ✅ Updated solution structure to include abstractions project
5. ✅ Established proper project references and dependencies
6. ✅ All tests passing and solution builds successfully

**Next Tasks for Other Engineers:**

* **Brian**: Implement concrete classes for all game service interfaces
* **Catherine**: Use the interfaces in PageModels via dependency injection  
* **David**: Implement `IAdService` and register it in DI container
* **Emily**: Write comprehensive unit tests for all implementations
