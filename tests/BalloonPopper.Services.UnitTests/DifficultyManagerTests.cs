using BalloonPopper.Models;
using FluentAssertions;

namespace BalloonPopper.Services.UnitTests
{
    [TestFixture]
    public class DifficultyManagerTests
    {
        private DifficultyManager _difficultyManager;
        private BalloonSpawnConfig _baseConfig;
        private DifficultyConfig _difficultyConfig;

        [SetUp]
        public void SetUp()
        {
            _baseConfig = new BalloonSpawnConfig
            {
                BaseSpawnRate = 1.0,
                SpawnRateIncrease = 0.1,
                BaseSpeed = 100.0,
                SpeedIncrease = 10.0,
            };

            _difficultyConfig = new DifficultyConfig { BalloonsToAdvanceLevel = 20, MaxLevel = 50 };

            _difficultyManager = new DifficultyManager(_baseConfig, _difficultyConfig);
        }

        [Test]
        public void GetSpawnConfigForLevel_ShouldReturnConfigWithIncreasedDifficulty_ForHigherLevels()
        {
            // Act
            var level1Config = _difficultyManager.GetSpawnConfigForLevel(1);
            var level5Config = _difficultyManager.GetSpawnConfigForLevel(5);

            // Assert
            level5Config.BaseSpawnRate.Should().BeGreaterThan(level1Config.BaseSpawnRate);
            level5Config.BaseSpeed.Should().BeGreaterThan(level1Config.BaseSpeed);
        }

        [Test]
        public void ShouldIncreaseDifficulty_ShouldReturnTrue_WhenRequiredBalloonsPopped()
        {
            // Arrange
            var gameState = new GameState
            {
                Level = 1,
                BalloonsPopped = 20, // Equal to required balloons for level 1
            };

            // Act
            var shouldIncrease = _difficultyManager.ShouldIncreaseDifficulty(gameState);

            // Assert
            shouldIncrease.Should().BeTrue();
        }

        [Test]
        public void ShouldIncreaseDifficulty_ShouldReturnFalse_WhenNotEnoughBalloonsPopped()
        {
            // Arrange
            var gameState = new GameState
            {
                Level = 1,
                BalloonsPopped = 10, // Less than required
            };

            // Act
            var shouldIncrease = _difficultyManager.ShouldIncreaseDifficulty(gameState);

            // Assert
            shouldIncrease.Should().BeFalse();
        }

        [Test]
        public void ShouldIncreaseDifficulty_ShouldReturnFalse_WhenAtMaxLevel()
        {
            // Arrange
            var gameState = new GameState
            {
                Level = _difficultyConfig.MaxLevel,
                BalloonsPopped = 1000,
            };

            // Act
            var shouldIncrease = _difficultyManager.ShouldIncreaseDifficulty(gameState);

            // Assert
            shouldIncrease.Should().BeFalse();
        }

        [Test]
        public void GetTimeScaleForLevel_ShouldIncreaseWithLevel_ButBeCapped()
        {
            // Act
            var timeScale1 = _difficultyManager.GetTimeScaleForLevel(1);
            var timeScale5 = _difficultyManager.GetTimeScaleForLevel(5);
            var timeScale20 = _difficultyManager.GetTimeScaleForLevel(20);

            // Assert
            timeScale5.Should().BeGreaterThan(timeScale1);
            timeScale20.Should().BeLessThanOrEqualTo(1.5); // Capped at 50% increase
        }

        [Test]
        public void CalculateLevelScore_ShouldIncreaseWithLevel()
        {
            // Arrange
            var baseScore = 100;

            // Act
            var level1Score = _difficultyManager.CalculateLevelScore(1, baseScore);
            var level5Score = _difficultyManager.CalculateLevelScore(5, baseScore);

            // Assert
            level5Score.Should().BeGreaterThan(level1Score);
            level5Score.Should().Be((int)(baseScore * 1.4)); // 1 + (5-1) * 0.1
        }

        [Test]
        public void GetBalloonSpeedMultiplier_ShouldIncreaseWithLevel_ButBeCapped()
        {
            // Act
            var multiplier1 = _difficultyManager.GetBalloonSpeedMultiplier(1);
            var multiplier5 = _difficultyManager.GetBalloonSpeedMultiplier(5);
            var multiplier20 = _difficultyManager.GetBalloonSpeedMultiplier(20);

            // Assert
            multiplier5.Should().BeGreaterThan(multiplier1);
            multiplier20.Should().BeLessThanOrEqualTo(2.0); // Capped at 200%
        }

        [Test]
        public void GetSpawnRateMultiplier_ShouldIncreaseWithLevel_ButBeCapped()
        {
            // Act
            var multiplier1 = _difficultyManager.GetSpawnRateMultiplier(1);
            var multiplier5 = _difficultyManager.GetSpawnRateMultiplier(5);
            var multiplier50 = _difficultyManager.GetSpawnRateMultiplier(50);

            // Assert
            multiplier5.Should().BeGreaterThan(multiplier1);
            multiplier50.Should().BeLessThanOrEqualTo(3.0); // Capped at 300%
        }
    }

    [TestFixture]
    public class ScoringServiceTests
    {
        private ScoringService _scoringService;
        private DifficultyConfig _difficultyConfig;

        [SetUp]
        public void SetUp()
        {
            _difficultyConfig = new DifficultyConfig { ComboThreshold = 3, MaxComboCount = 20 };
            _scoringService = new ScoringService(_difficultyConfig);
        }

        [Test]
        public void CalculateScore_ShouldApplyLevelMultiplier()
        {
            // Arrange
            var balloon = new Balloon { Points = 10 };
            var gameState = new GameState
            {
                Level = 3,
                ScoreMultiplier = 1.0,
                Combo = 1,
            };

            // Act
            var score = _scoringService.CalculateScore(balloon, gameState);

            // Assert
            score.Should().BeGreaterThan(balloon.Points);
        }

        [Test]
        public void CalculateScore_ShouldApplyStateMultiplier()
        {
            // Arrange
            var balloon = new Balloon { Points = 10 };
            var gameState = new GameState
            {
                Level = 1,
                ScoreMultiplier = 2.0, // Double points power-up
                Combo = 1,
            };

            // Act
            var score = _scoringService.CalculateScore(balloon, gameState);

            // Assert
            score.Should().Be(20); // 10 * 1.0 (level) * 2.0 (state) * 1.0 (combo)
        }

        [Test]
        public void CalculateScore_ShouldApplyComboMultiplier()
        {
            // Arrange
            var balloon = new Balloon { Points = 10 };
            var gameState = new GameState
            {
                Level = 1,
                ScoreMultiplier = 1.0,
                Combo = 5, // High combo
            };

            // Act
            var score = _scoringService.CalculateScore(balloon, gameState);

            // Assert
            score.Should().BeGreaterThan(10); // Base score should be multiplied by combo
        }

        [Test]
        public void CalculateComboBonus_ShouldReturnZero_WhenComboBelowThreshold()
        {
            // Act
            var bonus = _scoringService.CalculateComboBonus(2, 100);

            // Assert
            bonus.Should().Be(0);
        }

        [Test]
        public void CalculateComboBonus_ShouldReturnBonus_WhenComboAboveThreshold()
        {
            // Act
            var bonus = _scoringService.CalculateComboBonus(5, 100);

            // Assert
            bonus.Should().BeGreaterThan(0);
            bonus.Should().Be(30); // (5 - 3 + 1) * 0.1 * 100
        }

        [Test]
        public void CalculateTimeBonus_ShouldReturnBonus_ForFastReaction()
        {
            // Arrange
            var fastReaction = TimeSpan.FromSeconds(0.2);

            // Act
            var bonus = _scoringService.CalculateTimeBonus(fastReaction, 100);

            // Assert
            bonus.Should().BeGreaterThan(0);
        }

        [Test]
        public void CalculateTimeBonus_ShouldReturnZero_ForSlowReaction()
        {
            // Arrange
            var slowReaction = TimeSpan.FromSeconds(1.0);

            // Act
            var bonus = _scoringService.CalculateTimeBonus(slowReaction, 100);

            // Assert
            bonus.Should().Be(0);
        }

        [Test]
        public void CalculateLevelCompletionBonus_ShouldIncreaseWithLevel()
        {
            // Act
            var level1Bonus = _scoringService.CalculateLevelCompletionBonus(
                1,
                3,
                TimeSpan.FromMinutes(1)
            );
            var level5Bonus = _scoringService.CalculateLevelCompletionBonus(
                5,
                3,
                TimeSpan.FromMinutes(1)
            );

            // Assert
            level5Bonus.Should().BeGreaterThan(level1Bonus);
        }

        [Test]
        public void CalculateLevelCompletionBonus_ShouldIncreaseWithRemainingLives()
        {
            // Act
            var bonus1Life = _scoringService.CalculateLevelCompletionBonus(
                1,
                1,
                TimeSpan.FromMinutes(1)
            );
            var bonus3Lives = _scoringService.CalculateLevelCompletionBonus(
                1,
                3,
                TimeSpan.FromMinutes(1)
            );

            // Assert
            bonus3Lives.Should().BeGreaterThan(bonus1Life);
        }

        [Test]
        public void GetAccuracy_ShouldReturnCorrectRatio()
        {
            // Act
            var accuracy = _scoringService.GetAccuracy(80, 20);

            // Assert
            accuracy.Should().Be(0.8); // 80 out of 100
        }

        [Test]
        public void GetAccuracy_ShouldReturnOne_WhenNoBalloons()
        {
            // Act
            var accuracy = _scoringService.GetAccuracy(0, 0);

            // Assert
            accuracy.Should().Be(1.0);
        }

        [Test]
        public void GetRankForScore_ShouldReturnCorrectRank()
        {
            // Act
            var lowRank = _scoringService.GetRankForScore(500); // Below first threshold
            var midRank = _scoringService.GetRankForScore(10000); // Between thresholds
            var highRank = _scoringService.GetRankForScore(200000); // Above all thresholds

            // Assert
            lowRank.Should().Be(0); // Unranked
            midRank.Should().BeGreaterThan(0);
            highRank.Should().Be(7); // Highest rank
        }
    }
}
