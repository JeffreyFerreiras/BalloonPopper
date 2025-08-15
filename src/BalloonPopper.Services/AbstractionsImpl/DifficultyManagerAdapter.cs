using BalloonPopper.Models;

namespace BalloonPopper.Services.AbstractionsImpl
{
    /// <summary>
    /// Adapter implementing Abstractions IDifficultyManager using internal DifficultyManager.
    /// </summary>
    public class DifficultyManagerAdapter : BalloonPopper.Services.Abstractions.IDifficultyManager
    {
        private readonly BalloonPopper.Services.DifficultyManager _impl = new BalloonPopper.Services.DifficultyManager();
        private DifficultyConfig _current = new();

        public DifficultyConfig CurrentDifficulty => _current;

        public event EventHandler<DifficultyConfig>? DifficultyChanged;

        public void IncreaseDifficulty()
        {
            _current = _impl.GetDifficultyConfig();
            DifficultyChanged?.Invoke(this, _current);
        }

        public void SetDifficulty(int level)
        {
            _impl.GetSpawnConfigForLevel(level);
            _current = _impl.GetDifficultyConfig();
            DifficultyChanged?.Invoke(this, _current);
        }

        public void ResetDifficulty()
        {
            _current = new DifficultyConfig();
            DifficultyChanged?.Invoke(this, _current);
        }
    }
}
