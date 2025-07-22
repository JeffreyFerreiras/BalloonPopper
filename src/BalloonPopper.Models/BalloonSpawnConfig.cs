namespace BalloonPopper.Models
{
    /// <summary>
    /// Configuration class for balloon spawning mechanics
    /// </summary>
    public class BalloonSpawnConfig
    {
        public double BaseSpawnRate { get; set; } = 1.0; // balloons per second
        public double SpawnRateIncrease { get; set; } = 0.1; // increase per level
        public double MaxSpawnRate { get; set; } = 5.0;

        public double BaseSpeed { get; set; } = 100.0; // pixels per second
        public double SpeedIncrease { get; set; } = 10.0; // increase per level
        public double MaxSpeed { get; set; } = 400.0;

        public double BaseSize { get; set; } = 50.0;
        public double SizeVariation { get; set; } = 20.0; // +/- variation

        public int BasePoints { get; set; } = 10;
        public double LifeSpan { get; set; } = 8.0; // seconds before balloon auto-pops

        // Special balloon probabilities (0.0 to 1.0)
        public double SpeedBalloonChance { get; set; } = 0.1;
        public double GiantBalloonChance { get; set; } = 0.08;
        public double BonusBalloonChance { get; set; } = 0.05;
        public double MultiplierBalloonChance { get; set; } = 0.03;
        public double BombBalloonChance { get; set; } = 0.02;
        public double ShieldBalloonChance { get; set; } = 0.02;
        public double TimeFreezeBalloonChance { get; set; } = 0.015;
        public double DoublePointsBalloonChance { get; set; } = 0.025;

        public BalloonSpawnConfig GetConfigForLevel(int level)
        {
            return new BalloonSpawnConfig
            {
                BaseSpawnRate = Math.Min(
                    BaseSpawnRate + (SpawnRateIncrease * (level - 1)),
                    MaxSpawnRate
                ),
                SpawnRateIncrease = SpawnRateIncrease,
                MaxSpawnRate = MaxSpawnRate,

                BaseSpeed = Math.Min(BaseSpeed + (SpeedIncrease * (level - 1)), MaxSpeed),
                SpeedIncrease = SpeedIncrease,
                MaxSpeed = MaxSpeed,

                BaseSize = BaseSize,
                SizeVariation = SizeVariation,
                BasePoints = BasePoints,
                LifeSpan = Math.Max(LifeSpan - (level * 0.1), 3.0), // Minimum 3 seconds

                // Increase special balloon chances slightly with level
                SpeedBalloonChance = Math.Min(SpeedBalloonChance + (level * 0.005), 0.2),
                GiantBalloonChance = Math.Min(GiantBalloonChance + (level * 0.003), 0.15),
                BonusBalloonChance = Math.Min(BonusBalloonChance + (level * 0.002), 0.1),
                MultiplierBalloonChance = Math.Min(MultiplierBalloonChance + (level * 0.001), 0.06),
                BombBalloonChance = Math.Min(BombBalloonChance + (level * 0.001), 0.05),
                ShieldBalloonChance = Math.Min(ShieldBalloonChance + (level * 0.001), 0.04),
                TimeFreezeBalloonChance = Math.Min(
                    TimeFreezeBalloonChance + (level * 0.0005),
                    0.03
                ),
                DoublePointsBalloonChance = Math.Min(
                    DoublePointsBalloonChance + (level * 0.001),
                    0.05
                ),
            };
        }
    }
}
