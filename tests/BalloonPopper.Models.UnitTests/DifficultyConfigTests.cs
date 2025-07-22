using FluentAssertions;

namespace BalloonPopper.Models.UnitTests;

public partial class GameConfigTests
{
    [TestFixture]
    public class DifficultyConfigTests
    {
        private DifficultyConfig _config;

        [SetUp]
        public void SetUp()
        {
            _config = new DifficultyConfig
            {
                BalloonsToAdvanceLevel = 20,
                MaxLevel = 50,
                ComboThreshold = 5,
                MaxComboCount = 20,
            };
        }

        [Test]
        public void GetRequiredBalloonsForLevel_ShouldIncreaseWithLevel()
        {
            // Act
            var level1Required = _config.GetRequiredBalloonsForLevel(1);
            var level5Required = _config.GetRequiredBalloonsForLevel(5);

            // Assert
            level5Required.Should().BeGreaterThan(level1Required);
        }

        [Test]
        public void GetRequiredBalloonsForLevel_ShouldReturnBaseAmountForLevel1()
        {
            // Act
            var level1Required = _config.GetRequiredBalloonsForLevel(1);

            // Assert
            level1Required.Should().Be(_config.BalloonsToAdvanceLevel);
        }

        [Test]
        public void GetComboMultiplierForCount_ShouldReturnCorrectMultiplier()
        {
            // Act
            var multiplier = _config.GetComboMultiplierForCount(10);

            // Assert
            multiplier.Should().Be(2.0); // 1.0 + (10 * 0.1)
        }

        [Test]
        public void GetComboMultiplierForCount_ShouldCapAtMaxComboCount()
        {
            // Act
            var multiplier = _config.GetComboMultiplierForCount(30); // Above max

            // Assert
            multiplier.Should().Be(3.0); // 1.0 + (20 * 0.1) - capped at MaxComboCount
        }

        [Test]
        public void GetComboMultiplierForCount_ShouldReturnMinimumForZero()
        {
            // Act
            var multiplier = _config.GetComboMultiplierForCount(0);

            // Assert
            multiplier.Should().Be(1.0);
        }
    }
}
