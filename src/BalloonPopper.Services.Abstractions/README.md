# BalloonPopper.Services.Abstractions

This project contains all the service interface definitions for the Balloon Popper game, following SOLID principles and enabling dependency injection.

## Interfaces Defined

### Core Game Services

- **IGameEngine**: Main game loop and lifecycle management
- **IGameStateService**: Game state management with events
- **IBalloonSpawner**: Balloon creation and spawning logic
- **IDifficultyManager**: Difficulty progression and configuration
- **IBalloonInteractionService**: User interaction with balloons

### External Services

- **IAdService**: Advertisement integration and premium purchase handling

## Architecture Notes

This separate assembly ensures:

- **Dependency Inversion**: High-level modules depend on abstractions, not concretions
- **Interface Segregation**: Each interface has a specific, focused responsibility
- **Testability**: All concrete implementations can be easily mocked for unit testing
- **Modularity**: Clear separation between contracts and implementations

## Usage

Projects that need to consume services should reference this abstractions project and use dependency injection to get concrete implementations.

Example:
```csharp
public class GamePageModel
{
    private readonly IGameStateService _gameStateService;
    
    public GamePageModel(IGameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }
}
```

## Implementation Notes

- All interfaces include appropriate events for reactive programming
- Async methods are used where appropriate for non-blocking operations
- Services follow the Single Responsibility Principle
- Error handling is encapsulated within implementations
