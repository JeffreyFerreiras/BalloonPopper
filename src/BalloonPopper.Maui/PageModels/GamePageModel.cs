using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalloonPopper.Models;
using BalloonPopper.Services.Abstractions;

namespace BalloonPopper.Maui.PageModels;

public class GamePageModel : INotifyPropertyChanged
{
    private readonly IGameEngine? _gameEngine;
    private readonly IGameStateService? _gameStateService;

    private string _gameMode = "classic";
    private int _currentScore = 0;
    private string _livesOrTimeDisplay = "Lives: 3";
    private bool _isPaused = false;

    public GamePageModel(IGameEngine? gameEngine = null, IGameStateService? gameStateService = null)
    {
        _gameEngine = gameEngine;
        _gameStateService = gameStateService;

        // Subscribe to game events when services are available
        if (_gameStateService != null)
        {
            _gameStateService.GameStateChanged += OnGameStateChanged;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int CurrentScore
    {
        get => _currentScore;
        set
        {
            if (_currentScore != value)
            {
                _currentScore = value;
                OnPropertyChanged();
            }
        }
    }

    public string LivesOrTimeDisplay
    {
        get => _livesOrTimeDisplay;
        set
        {
            if (_livesOrTimeDisplay != value)
            {
                _livesOrTimeDisplay = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (_isPaused != value)
            {
                _isPaused = value;
                OnPropertyChanged();
            }
        }
    }

    public void SetGameMode(string mode)
    {
        _gameMode = mode?.ToLower() ?? "classic";

        // Update display based on game mode
        switch (_gameMode)
        {
            case "timeattack":
                LivesOrTimeDisplay = "Time: 60s";
                break;
            case "endless":
                LivesOrTimeDisplay = "Lives: ∞";
                break;
            case "precision":
                LivesOrTimeDisplay = "Precision: 100%";
                break;
            default: // classic
                LivesOrTimeDisplay = "Lives: 3";
                break;
        }
    }

    public void StartGame()
    {
        try
        {
            // Reset game state
            CurrentScore = 0;
            IsPaused = false;

            // Start the game engine if available
            if (_gameEngine != null)
            {
                _gameEngine.StartGame();
            }
            else
            {
                // Placeholder logic when game engine is not yet implemented
                // This will be replaced when Brian Bot implements the game engine
                System.Diagnostics.Debug.WriteLine(
                    $"Starting game in {_gameMode} mode (Game Engine not yet implemented)"
                );
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting game: {ex.Message}");
        }
    }

    public void PauseGame()
    {
        try
        {
            IsPaused = true;

            if (_gameEngine != null)
            {
                _gameEngine.PauseGame();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error pausing game: {ex.Message}");
        }
    }

    public void ResumeGame()
    {
        try
        {
            IsPaused = false;

            if (_gameEngine != null)
            {
                _gameEngine.ResumeGame();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resuming game: {ex.Message}");
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void RestartGame()
    {
        try
        {
            StartGame();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error restarting game: {ex.Message}");
        }
    }

    public async void NavigateToMainMenu()
    {
        try
        {
            // Stop the game
            if (_gameEngine != null)
            {
                _gameEngine.EndGame();
            }

            // Navigate back to menu
            await Shell.Current.GoToAsync("//MenuPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error navigating to main menu: {ex.Message}");
        }
    }

    private void OnGameStateChanged(object? sender, GameState newState)
    {
        // Handle game state changes
        switch (newState.Status)
        {
            case GameStatus.Paused:
                IsPaused = true;
                break;
            case GameStatus.Playing:
                IsPaused = false;
                break;
            case GameStatus.GameOver:
                // Handle game over
                HandleGameOver();
                break;
        }

        // Update score from the game state
        CurrentScore = newState.Score;
    }

    private async void HandleGameOver()
    {
        try
        {
            if (Application.Current?.Windows.Count > 0)
            {
                var playAgain = await Application
                    .Current.Windows[0]
                    .Page!.DisplayAlert(
                        "Game Over!",
                        $"Final Score: {CurrentScore}\n\nWould you like to play again?",
                        "Play Again",
                        "Main Menu"
                    );

                if (playAgain)
                {
                    RestartGame();
                }
                else
                {
                    NavigateToMainMenu();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling game over: {ex.Message}");
        }
    }
}
