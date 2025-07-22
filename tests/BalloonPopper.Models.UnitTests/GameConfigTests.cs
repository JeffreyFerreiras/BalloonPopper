using FluentAssertions;

namespace BalloonPopper.Models.UnitTests;

[TestFixture]
public partial class GameConfigTests
{
    private BalloonSpawnConfig _spawnConfig;
    private DifficultyConfig _difficultyConfig;

    [SetUp]
    public void SetUp()
    {
        _spawnConfig = new BalloonSpawnConfig();
        _difficultyConfig = new DifficultyConfig();
    }

    [TestFixture]
    public class BalloonSpawnConfigTests
    {
        private BalloonSpawnConfig _config;

        [SetUp]
        public void SetUp()
        {
            _config = new BalloonSpawnConfig
            {
                BaseSpawnRate = 1.0,
                SpawnRateIncrease = 0.1,
                MaxSpawnRate = 5.0,
                BaseSpeed = 100.0,
                SpeedIncrease = 10.0,
                MaxSpeed = 400.0,
                LifeSpan = 8.0,
            };
        }

        [Test]
        public void GetConfigForLevel_ShouldIncreaseSpawnRate_ForHigherLevels()
        {
            // Act
            var level1Config = _config.GetConfigForLevel(1);
            var level5Config = _config.GetConfigForLevel(5);

            // Assert
            level5Config.BaseSpawnRate.Should().BeGreaterThan(level1Config.BaseSpawnRate);
        }

        [Test]
        public void GetConfigForLevel_ShouldIncreaseSpeed_ForHigherLevels()
        {
            // Act
            var level1Config = _config.GetConfigForLevel(1);
            var level5Config = _config.GetConfigForLevel(5);

            // Assert
            level5Config.BaseSpeed.Should().BeGreaterThan(level1Config.BaseSpeed);
        }

        [Test]
        public void GetConfigForLevel_ShouldNotExceedMaximums_ForVeryHighLevels()
        {
            // Act
            var level100Config = _config.GetConfigForLevel(100);

            // Assert
            level100Config.BaseSpawnRate.Should().BeLessThanOrEqualTo(_config.MaxSpawnRate);
            level100Config.BaseSpeed.Should().BeLessThanOrEqualTo(_config.MaxSpeed);
        }

        [Test]
        public void GetConfigForLevel_ShouldDecreaseLifeSpan_ForHigherLevels()
        {
            // Arrange
            _config.LifeSpan = 8.0;

            // Act
            var level5Config = _config.GetConfigForLevel(5);

            // Assert
            level5Config.LifeSpan.Should().BeLessThan(_config.LifeSpan);
            level5Config.LifeSpan.Should().BeGreaterThanOrEqualTo(3.0); // Minimum lifespan
        }

        [Test]
        public void GetConfigForLevel_ShouldIncreaseSpecialBalloonChances_ForHigherLevels()
        {
            // Act
            var level1Config = _config.GetConfigForLevel(1);
            var level10Config = _config.GetConfigForLevel(10);

            // Assert
            level10Config
                .SpeedBalloonChance.Should()
                .BeGreaterThan(level1Config.SpeedBalloonChance);
            level10Config
                .GiantBalloonChance.Should()
                .BeGreaterThan(level1Config.GiantBalloonChance);
            level10Config
                .BonusBalloonChance.Should()
                .BeGreaterThan(level1Config.BonusBalloonChance);
        }
    }
}
