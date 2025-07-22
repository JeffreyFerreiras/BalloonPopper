using BalloonPopper.Models;
using FluentAssertions;

namespace BalloonPopper.Services.UnitTests
{
    [TestFixture]
    public class GameStateManagerTests
    {
        private GameStateManager _gameStateManager;
        private DifficultyConfig _difficultyConfig;

        [SetUp]
        public void SetUp()
        {
            _difficultyConfig = new DifficultyConfig
            {
                BalloonsToAdvanceLevel = 10,
                LivesLostPerEscape = 1,
                MaxLivesAtStart = 3,
                ComboThreshold = 3,
            };
            _gameStateManager = new GameStateManager(_difficultyConfig);
        }

        [Test]
        public void StartNewGame_ShouldResetGameState_WhenCalled()
        {
            // Arrange
            _gameStateManager.CurrentState.Score = 1000;

            // Act
            _gameStateManager.StartNewGame();

            // Assert
            _gameStateManager.CurrentState.Score.Should().Be(0);
            _gameStateManager.CurrentState.Level.Should().Be(1);
            _gameStateManager.CurrentState.Lives.Should().Be(3);
            _gameStateManager.CurrentState.Status.Should().Be(GameStatus.Playing);
        }

        [Test]
        public void StartNewGame_ShouldFireEvents_WhenCalled()
        {
            // Arrange
            var gameStateChanged = false;
            var scoreChanged = false;
            var levelChanged = false;
            var livesChanged = false;

            _gameStateManager.GameStateChanged += (s, e) => gameStateChanged = true;
            _gameStateManager.ScoreChanged += (s, e) => scoreChanged = true;
            _gameStateManager.LevelChanged += (s, e) => levelChanged = true;
            _gameStateManager.LivesChanged += (s, e) => livesChanged = true;

            // Act
            _gameStateManager.StartNewGame();

            // Assert
            gameStateChanged.Should().BeTrue();
            scoreChanged.Should().BeTrue();
            levelChanged.Should().BeTrue();
            livesChanged.Should().BeTrue();
        }

        [Test]
        public void PauseGame_ShouldChangeStatusToPaused_WhenGameIsPlaying()
        {
            // Arrange
            _gameStateManager.StartNewGame(); // Sets status to Playing

            // Act
            _gameStateManager.PauseGame();

            // Assert
            _gameStateManager.CurrentState.Status.Should().Be(GameStatus.Paused);
        }

        [Test]
        public void ResumeGame_ShouldChangeStatusToPlaying_WhenGameIsPaused()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            _gameStateManager.PauseGame();

            // Act
            _gameStateManager.ResumeGame();

            // Assert
            _gameStateManager.CurrentState.Status.Should().Be(GameStatus.Playing);
        }

        [Test]
        public void ProcessBalloonPop_ShouldIncreaseScore_WhenBalloonIsPopped()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            var balloon = new Balloon { Points = 10, Type = BalloonType.Normal };

            // Act
            _gameStateManager.ProcessBalloonPop(balloon);

            // Assert
            _gameStateManager.CurrentState.Score.Should().BeGreaterThan(0);
            _gameStateManager.CurrentState.BalloonsPopped.Should().Be(1);
            balloon.IsPopped.Should().BeTrue();
        }

        [Test]
        public void ProcessBalloonPop_ShouldIncreaseCombo_WhenBalloonsArePoppedQuickly()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            var balloon1 = new Balloon { Points = 10, Type = BalloonType.Normal };
            var balloon2 = new Balloon { Points = 10, Type = BalloonType.Normal };

            // Act
            _gameStateManager.ProcessBalloonPop(balloon1);
            _gameStateManager.ProcessBalloonPop(balloon2); // Within combo window

            // Assert
            _gameStateManager.CurrentState.Combo.Should().Be(2);
        }

        [Test]
        public void ProcessBalloonEscape_ShouldDecreaseLives_WhenNotInvincible()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            var initialLives = _gameStateManager.CurrentState.Lives;

            // Act
            _gameStateManager.ProcessBalloonEscape();

            // Assert
            _gameStateManager.CurrentState.Lives.Should().Be(initialLives - 1);
            _gameStateManager.CurrentState.BalloonsEscaped.Should().Be(1);
            _gameStateManager.CurrentState.Combo.Should().Be(0); // Reset combo
        }

        [Test]
        public void ProcessBalloonEscape_ShouldNotDecreaseLives_WhenInvincible()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            _gameStateManager.CurrentState.IsInvincible = true;
            var initialLives = _gameStateManager.CurrentState.Lives;

            // Act
            _gameStateManager.ProcessBalloonEscape();

            // Assert
            _gameStateManager.CurrentState.Lives.Should().Be(initialLives);
        }

        [Test]
        public void ProcessBalloonEscape_ShouldEndGame_WhenLivesReachZero()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            _gameStateManager.CurrentState.Lives = 1;
            var gameOverFired = false;
            _gameStateManager.GameOver += (s, e) => gameOverFired = true;

            // Act
            _gameStateManager.ProcessBalloonEscape();

            // Assert
            _gameStateManager.CurrentState.Status.Should().Be(GameStatus.GameOver);
            gameOverFired.Should().BeTrue();
        }

        [Test]
        public void ActivatePowerUp_ShouldSetActivePowerUp_WhenCalled()
        {
            // Arrange
            _gameStateManager.StartNewGame();

            // Act
            _gameStateManager.ActivatePowerUp(PowerUpType.DoublePoints);

            // Assert
            _gameStateManager.CurrentState.ActivePowerUp.Should().Be(PowerUpType.DoublePoints);
            _gameStateManager.CurrentState.ScoreMultiplier.Should().Be(2.0);
        }

        [Test]
        public void ActivatePowerUp_ShouldSetShieldInvincibility_WhenShieldActivated()
        {
            // Arrange
            _gameStateManager.StartNewGame();

            // Act
            _gameStateManager.ActivatePowerUp(PowerUpType.Shield);

            // Assert
            _gameStateManager.CurrentState.IsInvincible.Should().BeTrue();
        }

        [Test]
        public void CheckLevelProgress_ShouldAdvanceLevel_WhenRequiredBalloonsPopped()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            _gameStateManager.CurrentState.BalloonsPopped =
                _difficultyConfig.BalloonsToAdvanceLevel;
            var levelCompleteFired = false;
            _gameStateManager.LevelComplete += (s, e) => levelCompleteFired = true;

            // Act
            _gameStateManager.CheckLevelProgress();

            // Assert
            _gameStateManager.CurrentState.Status.Should().Be(GameStatus.LevelComplete);
            levelCompleteFired.Should().BeTrue();
        }

        [Test]
        public void AddBonusScore_ShouldIncreaseScore_WhenGameIsPlaying()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            var initialScore = _gameStateManager.CurrentState.Score;

            // Act
            _gameStateManager.AddBonusScore(100);

            // Assert
            _gameStateManager.CurrentState.Score.Should().Be(initialScore + 100);
        }

        [Test]
        public void UpdateGameTime_ShouldIncreaseGameTime_WhenGameIsPlaying()
        {
            // Arrange
            _gameStateManager.StartNewGame();
            var deltaTime = TimeSpan.FromSeconds(1);

            // Act
            _gameStateManager.UpdateGameTime(deltaTime);

            // Assert
            _gameStateManager.CurrentState.GameTime.Should().Be(deltaTime);
        }
    }
}
