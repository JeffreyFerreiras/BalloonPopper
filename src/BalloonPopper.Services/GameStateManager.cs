using BalloonPopper.Models;

namespace BalloonPopper.Services
{
    /// <summary>
    /// Interface for game state management
    /// </summary>
    public interface IGameStateManager
    {
        event EventHandler<GameState> GameStateChanged;
        event EventHandler<int> ScoreChanged;
        event EventHandler<int> LevelChanged;
        event EventHandler<int> LivesChanged;
        event EventHandler GameOver;
        event EventHandler LevelComplete;

        GameState CurrentState { get; }
        void StartNewGame();
        void PauseGame();
        void ResumeGame();
        void EndGame();
        void UpdateGameTime(TimeSpan deltaTime);
        void ProcessBalloonPop(Balloon balloon);
        void ProcessBalloonEscape();
        void ActivatePowerUp(PowerUpType powerUp);
        void CheckLevelProgress();
        void NextLevel();
        void AddBonusScore(int points);
    }

    /// <summary>
    /// Service responsible for managing the overall game state and progression
    /// </summary>
    public class GameStateManager(DifficultyConfig? difficultyConfig = null) : IGameStateManager
    {
        private readonly GameState _gameState = new GameState();
        private readonly DifficultyConfig _difficultyConfig = difficultyConfig ?? new DifficultyConfig();
        private DateTime _lastComboTime = DateTime.MinValue;
        private readonly TimeSpan _comboTimeWindow = TimeSpan.FromSeconds(2.0);

        public event EventHandler<GameState>? GameStateChanged;
        public event EventHandler<int>? ScoreChanged;
        public event EventHandler<int>? LevelChanged;
        public event EventHandler<int>? LivesChanged;
        public event EventHandler? GameOver;
        public event EventHandler? LevelComplete;

        public GameState CurrentState => _gameState;

        public void StartNewGame()
        {
            _gameState.ResetForNewGame();
            _gameState.Status = GameStatus.Playing;
            _lastComboTime = DateTime.MinValue;
            
            OnGameStateChanged();
            OnScoreChanged();
            OnLevelChanged();
            OnLivesChanged();
        }

        public void PauseGame()
        {
            if (_gameState.Status == GameStatus.Playing)
            {
                _gameState.Status = GameStatus.Paused;
                OnGameStateChanged();
            }
        }

        public void ResumeGame()
        {
            if (_gameState.Status == GameStatus.Paused)
            {
                _gameState.Status = GameStatus.Playing;
                OnGameStateChanged();
            }
        }

        public void EndGame()
        {
            _gameState.Status = GameStatus.GameOver;
            OnGameStateChanged();
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateGameTime(TimeSpan deltaTime)
        {
            if (_gameState.Status == GameStatus.Playing)
            {
                _gameState.GameTime += deltaTime;
                _gameState.UpdatePowerUpStatus();
                OnGameStateChanged();
            }
        }

        public void ProcessBalloonPop(Balloon balloon)
        {
            if (_gameState.Status != GameStatus.Playing || balloon.IsPopped)
                return;

            var baseScore = balloon.Pop();
            if (baseScore <= 0) return;

            // Handle special balloon effects
            HandleSpecialBalloonEffects(balloon);

            // Apply combo logic
            var now = DateTime.Now;
            if (now - _lastComboTime <= _comboTimeWindow)
            {
                _gameState.Combo++;
                if (_gameState.Combo > _gameState.MaxCombo)
                    _gameState.MaxCombo = _gameState.Combo;
            }
            else
            {
                _gameState.Combo = 1;
            }
            _lastComboTime = now;

            // Calculate final score with multipliers
            var comboMultiplier = _difficultyConfig.GetComboMultiplierForCount(_gameState.Combo);
            var finalScore = (int)(baseScore * _gameState.ScoreMultiplier * comboMultiplier);

            _gameState.Score += finalScore;
            _gameState.BalloonsPopped++;

            OnScoreChanged();
            CheckLevelProgress();
        }

        public void ProcessBalloonEscape()
        {
            if (_gameState.Status != GameStatus.Playing)
                return;

            _gameState.BalloonsEscaped++;
            
            // Reset combo on balloon escape
            _gameState.Combo = 0;
            _lastComboTime = DateTime.MinValue;

            // Lose life if not invincible
            if (!_gameState.IsInvincible)
            {
                _gameState.Lives -= _difficultyConfig.LivesLostPerEscape;
                OnLivesChanged();

                if (_gameState.Lives <= 0)
                {
                    EndGame();
                }
            }
        }

        public void ActivatePowerUp(PowerUpType powerUp)
        {
            var duration = powerUp switch
            {
                PowerUpType.DoublePoints => _difficultyConfig.DoublePointsDuration,
                PowerUpType.Shield => _difficultyConfig.ShieldDuration,
                PowerUpType.TimeFreeze => _difficultyConfig.TimeFreezeDuration,
                PowerUpType.SlowMotion => _difficultyConfig.SlowMotionDuration,
                PowerUpType.ExtraLife => 0, // Instant effect
                PowerUpType.BombBlast => 0, // Instant effect
                _ => 0
            };

            _gameState.ActivePowerUp = powerUp;
            _gameState.PowerUpExpiryTime = duration > 0 ? DateTime.Now.AddSeconds(duration) : null;

            ApplyPowerUpEffect(powerUp);
            OnGameStateChanged();
        }

        public void CheckLevelProgress()
        {
            var requiredBalloons = _difficultyConfig.GetRequiredBalloonsForLevel(_gameState.Level);
            
            if (_gameState.BalloonsPopped >= requiredBalloons)
            {
                CompleteLevelAndAdvance();
            }
        }

        public void NextLevel()
        {
            _gameState.Level++;
            _gameState.BalloonsPopped = 0; // Reset for new level
            OnLevelChanged();
            OnGameStateChanged();
        }

        public void AddBonusScore(int points)
        {
            if (_gameState.Status == GameStatus.Playing)
            {
                var finalPoints = (int)(points * _gameState.ScoreMultiplier);
                _gameState.Score += finalPoints;
                OnScoreChanged();
            }
        }

        private void HandleSpecialBalloonEffects(Balloon balloon)
        {
            switch (balloon.Type)
            {
                case BalloonType.Multiplier:
                    ActivatePowerUp(PowerUpType.DoublePoints);
                    break;
                case BalloonType.Shield:
                    ActivatePowerUp(PowerUpType.Shield);
                    break;
                case BalloonType.TimeFreeze:
                    ActivatePowerUp(PowerUpType.TimeFreeze);
                    break;
                case BalloonType.DoublePoints:
                    ActivatePowerUp(PowerUpType.DoublePoints);
                    break;
                case BalloonType.Bomb:
                    // This would trigger bomb explosion logic (handled by caller)
                    break;
            }
        }

        private void ApplyPowerUpEffect(PowerUpType powerUp)
        {
            switch (powerUp)
            {
                case PowerUpType.DoublePoints:
                    _gameState.ScoreMultiplier = 2.0;
                    break;
                case PowerUpType.Shield:
                    _gameState.IsInvincible = true;
                    break;
                case PowerUpType.TimeFreeze:
                    _gameState.IsTimeFrozen = true;
                    _gameState.TimeFreezeExpiry = DateTime.Now.AddSeconds(_difficultyConfig.TimeFreezeDuration);
                    break;
                case PowerUpType.ExtraLife:
                    _gameState.Lives++;
                    OnLivesChanged();
                    break;
                case PowerUpType.BombBlast:
                    // Handled by game engine for nearby balloon destruction
                    break;
            }
        }

        private void CompleteLevelAndAdvance()
        {
            _gameState.Status = GameStatus.LevelComplete;
            LevelComplete?.Invoke(this, EventArgs.Empty);
            
            // Auto-advance after a brief pause
            Task.Delay(2000).ContinueWith(_ =>
            {
                if (_gameState.Status == GameStatus.LevelComplete)
                {
                    NextLevel();
                    _gameState.Status = GameStatus.Playing;
                    OnGameStateChanged();
                }
            });
        }

        private void OnGameStateChanged() => GameStateChanged?.Invoke(this, _gameState);
        private void OnScoreChanged() => ScoreChanged?.Invoke(this, _gameState.Score);
        private void OnLevelChanged() => LevelChanged?.Invoke(this, _gameState.Level);
        private void OnLivesChanged() => LivesChanged?.Invoke(this, _gameState.Lives);
    }
}
