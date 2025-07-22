using FluentAssertions;

namespace BalloonPopper.Models.UnitTests;

[TestFixture]
public class GameStateTests
{
    private GameState _gameState;

    [SetUp]
    public void SetUp()
    {
        _gameState = new GameState();
    }

    [Test]
    public void ResetForNewGame_ShouldResetAllProperties_WhenCalled()
    {
        // Arrange
        _gameState.Score = 1000;
        _gameState.Level = 5;
        _gameState.Lives = 1;
        _gameState.Status = GameStatus.GameOver;
        _gameState.BalloonsPopped = 50;
        _gameState.ScoreMultiplier = 2.0;
        _gameState.ActivePowerUp = PowerUpType.DoublePoints;

        // Act
        _gameState.ResetForNewGame();

        // Assert
        _gameState.Score.Should().Be(0);
        _gameState.Level.Should().Be(1);
        _gameState.Lives.Should().Be(3);
        _gameState.Status.Should().Be(GameStatus.NotStarted);
        _gameState.BalloonsPopped.Should().Be(0);
        _gameState.ScoreMultiplier.Should().Be(1.0);
        _gameState.ActivePowerUp.Should().BeNull();
    }

    [Test]
    public void HasActivePowerUp_ShouldReturnTrue_WhenPowerUpIsActiveAndNotExpired()
    {
        // Arrange
        _gameState.ActivePowerUp = PowerUpType.DoublePoints;
        _gameState.PowerUpExpiryTime = DateTime.Now.AddSeconds(5);

        // Act & Assert
        _gameState.HasActivePowerUp.Should().BeTrue();
    }

    [Test]
    public void HasActivePowerUp_ShouldReturnFalse_WhenPowerUpIsExpired()
    {
        // Arrange
        _gameState.ActivePowerUp = PowerUpType.DoublePoints;
        _gameState.PowerUpExpiryTime = DateTime.Now.AddSeconds(-1);

        // Act & Assert
        _gameState.HasActivePowerUp.Should().BeFalse();
    }

    [Test]
    public void UpdatePowerUpStatus_ShouldClearExpiredPowerUp_WhenPowerUpHasExpired()
    {
        // Arrange
        _gameState.ActivePowerUp = PowerUpType.DoublePoints;
        _gameState.PowerUpExpiryTime = DateTime.Now.AddSeconds(-1);
        _gameState.ScoreMultiplier = 2.0;
        _gameState.IsInvincible = true;

        // Act
        _gameState.UpdatePowerUpStatus();

        // Assert
        _gameState.ActivePowerUp.Should().BeNull();
        _gameState.PowerUpExpiryTime.Should().BeNull();
        _gameState.ScoreMultiplier.Should().Be(1.0);
        _gameState.IsInvincible.Should().BeFalse();
    }

    [Test]
    public void UpdatePowerUpStatus_ShouldClearTimeFreeze_WhenTimeFreezeHasExpired()
    {
        // Arrange
        _gameState.IsTimeFrozen = true;
        _gameState.TimeFreezeExpiry = DateTime.Now.AddSeconds(-1);

        // Act
        _gameState.UpdatePowerUpStatus();

        // Assert
        _gameState.IsTimeFrozen.Should().BeFalse();
        _gameState.TimeFreezeExpiry.Should().BeNull();
    }
}
