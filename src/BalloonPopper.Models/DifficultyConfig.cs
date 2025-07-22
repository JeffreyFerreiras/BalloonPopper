namespace BalloonPopper.Models
{
    /// <summary>
    /// Configuration for game difficulty progression
    /// </summary>
    public class DifficultyConfig
    {
        public int BalloonsToAdvanceLevel { get; set; } = 20;
        public int MaxLevel { get; set; } = 50;
        public int LivesLostPerEscape { get; set; } = 1;
        public int MaxLivesAtStart { get; set; } = 3;
        public int ComboThreshold { get; set; } = 5; // balloons needed for combo bonus
        public double ComboMultiplier { get; set; } = 1.5;
        public int MaxComboCount { get; set; } = 20;

        // Power-up durations in seconds
        public double DoublePointsDuration { get; set; } = 10.0;
        public double ShieldDuration { get; set; } = 8.0;
        public double TimeFreezeDuration { get; set; } = 5.0;
        public double SlowMotionDuration { get; set; } = 7.0;

        public int GetRequiredBalloonsForLevel(int level)
        {
            // For level 1, return base amount. For subsequent levels, add progression
            return BalloonsToAdvanceLevel + ((level - 1) * 2);
        }

        public double GetComboMultiplierForCount(int comboCount)
        {
            return 1.0 + (Math.Min(comboCount, MaxComboCount) * 0.1);
        }
    }
}
