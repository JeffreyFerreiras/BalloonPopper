namespace BalloonPopper.Services.Abstractions;

public interface IGameEngine
{
    void StartGame();
    void EndGame();
    void PauseGame();
    void ResumeGame();
    void Update(double deltaTime);
    bool IsRunning { get; }
}
