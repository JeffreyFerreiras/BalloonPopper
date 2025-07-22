using BalloonPopper.Models;

namespace BalloonPopper.Services
{
    /// <summary>
    /// Interface for game difficulty progression algorithms
    /// </summary>
    public interface IDifficultyManager
    {
        BalloonSpawnConfig GetSpawnConfigForLevel(int level);
        DifficultyConfig GetDifficultyConfig();
        bool ShouldIncreaseDifficulty(GameState gameState);
        double GetTimeScaleForLevel(int level);
        int CalculateLevelScore(int level, int baseScore);
        double GetBalloonSpeedMultiplier(int level);
        double GetSpawnRateMultiplier(int level);
    }

    /// <summary>
    /// Service responsible for implementing difficulty progression algorithms
    /// </summary>
    public class DifficultyManager(
        BalloonSpawnConfig? balloonConfig = null,
        DifficultyConfig? difficultyConfig = null
        ) : IDifficultyManager
    {
        private readonly BalloonSpawnConfig _baseBalloonConfig = balloonConfig ?? new BalloonSpawnConfig();
        private readonly DifficultyConfig _difficultyConfig = difficultyConfig ?? new DifficultyConfig();

        public BalloonSpawnConfig GetSpawnConfigForLevel(int level)
        {
            return _baseBalloonConfig.GetConfigForLevel(level);
        }

        public DifficultyConfig GetDifficultyConfig()
        {
            return _difficultyConfig;
        }

        public bool ShouldIncreaseDifficulty(GameState gameState)
        {
            var requiredBalloons = _difficultyConfig.GetRequiredBalloonsForLevel(gameState.Level);
            return gameState.BalloonsPopped >= requiredBalloons
                && gameState.Level < _difficultyConfig.MaxLevel;
        }

        public double GetTimeScaleForLevel(int level)
        {
            // Gradually increase game speed with level, but cap it
            var speedIncrease = Math.Min(level * 0.05, 0.5); // Max 50% speed increase
            return 1.0 + speedIncrease;
        }

        public int CalculateLevelScore(int level, int baseScore)
        {
            // Award bonus points for higher levels
            var levelMultiplier = 1.0 + (level - 1) * 0.1; // 10% more points per level
            return (int)(baseScore * levelMultiplier);
        }

        public double GetBalloonSpeedMultiplier(int level)
        {
            // Balloons move faster at higher levels
            var baseMultiplier = 1.0;
            var levelIncrease = (level - 1) * 0.15; // 15% speed increase per level
            var maxIncrease = 2.0; // Cap at 200% of base speed

            return Math.Min(baseMultiplier + levelIncrease, maxIncrease);
        }

        public double GetSpawnRateMultiplier(int level)
        {
            // More balloons spawn at higher levels
            var baseMultiplier = 1.0;
            var levelIncrease = (level - 1) * 0.1; // 10% more spawns per level
            var maxIncrease = 3.0; // Cap at 300% of base spawn rate

            return Math.Min(baseMultiplier + levelIncrease, maxIncrease);
        }
    }

    /// <summary>
    /// Interface for scoring calculations and algorithms
    /// </summary>
    public interface IScoringService
    {
        int CalculateScore(Balloon balloon, GameState gameState);
        int CalculateComboBonus(int comboCount, int baseScore);
        int CalculateTimeBonus(TimeSpan reactionTime, int baseScore);
        int CalculateLevelCompletionBonus(int level, int remainingLives, TimeSpan completionTime);
        double GetAccuracy(int balloonsPopped, int balloonsEscaped);
        int GetRankForScore(int score);
    }

    /// <summary>
    /// Service responsible for all scoring calculations and algorithms
    /// </summary>
    public class ScoringService(DifficultyConfig? difficultyConfig = null) : IScoringService
    {
        private readonly DifficultyConfig _difficultyConfig = difficultyConfig ?? new DifficultyConfig();
        private readonly int[] _rankThresholds = { 1000, 5000, 15000, 30000, 50000, 75000, 100000 };

        public int CalculateScore(Balloon balloon, GameState gameState)
        {
            var baseScore = balloon.Points;

            // Apply level multiplier
            var levelMultiplier = 1.0 + (gameState.Level - 1) * 0.1;

            // Apply game state multipliers
            var stateMultiplier = gameState.ScoreMultiplier;

            // Apply combo bonus
            var comboMultiplier = _difficultyConfig.GetComboMultiplierForCount(gameState.Combo);

            var finalScore = (int)(baseScore * levelMultiplier * stateMultiplier * comboMultiplier);

            return Math.Max(finalScore, 1); // Minimum 1 point
        }

        public int CalculateComboBonus(int comboCount, int baseScore)
        {
            if (comboCount < _difficultyConfig.ComboThreshold)
                return 0;

            var bonusMultiplier = (comboCount - _difficultyConfig.ComboThreshold + 1) * 0.1;
            return (int)(baseScore * bonusMultiplier);
        }

        public int CalculateTimeBonus(TimeSpan reactionTime, int baseScore)
        {
            // Bonus for quick reactions (under 0.5 seconds)
            if (reactionTime.TotalSeconds > 0.5)
                return 0;

            var speedBonus = Math.Max(0, 0.5 - reactionTime.TotalSeconds);
            return (int)(baseScore * speedBonus);
        }

        public int CalculateLevelCompletionBonus(
            int level,
            int remainingLives,
            TimeSpan completionTime
        )
        {
            var baseBonus = level * 100;
            var livesBonus = remainingLives * 50;

            // Time bonus (faster completion = more points)
            var targetTime = TimeSpan.FromMinutes(2); // Target 2 minutes per level
            var timeBonus = 0;
            if (completionTime < targetTime)
            {
                var timeRatio = 1.0 - (completionTime.TotalSeconds / targetTime.TotalSeconds);
                timeBonus = (int)(baseBonus * timeRatio);
            }

            return baseBonus + livesBonus + timeBonus;
        }

        public double GetAccuracy(int balloonsPopped, int balloonsEscaped)
        {
            var totalBalloons = balloonsPopped + balloonsEscaped;
            if (totalBalloons == 0)
                return 1.0;

            return (double)balloonsPopped / totalBalloons;
        }

        public int GetRankForScore(int score)
        {
            for (int i = _rankThresholds.Length - 1; i >= 0; i--)
            {
                if (score >= _rankThresholds[i])
                    return i + 1; // Rank 1-7
            }
            return 0; // Unranked
        }
    }
}
