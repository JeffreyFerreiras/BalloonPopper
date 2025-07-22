using BalloonPopper.Models;

namespace BalloonPopper.Services.Abstractions;

public interface IDifficultyManager
{
    DifficultyConfig CurrentDifficulty { get; }
    event EventHandler<DifficultyConfig>? DifficultyChanged;
    void IncreaseDifficulty();
    void SetDifficulty(int level);
    void ResetDifficulty();
}
