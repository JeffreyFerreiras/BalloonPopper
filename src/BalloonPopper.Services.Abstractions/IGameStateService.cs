using BalloonPopper.Models;

namespace BalloonPopper.Services.Abstractions;

public interface IGameStateService
{
    GameState CurrentState { get; }
    event EventHandler<GameState>? GameStateChanged;
    void StartNewGame();
    void EndGame();
    void PauseGame();
    void ResumeGame();
    void UpdateScore(int points);
    void UpdateLives(int lives);
    void PopBalloon();
}
