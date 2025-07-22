namespace BalloonPopper.Models
{
    /// <summary>
    /// Represents the current state of the game session
    /// </summary>
    public class GameState
    {
        public int Score { get; set; }
        public int Level { get; set; } = 1;
        public int Lives { get; set; } = 3;
        public TimeSpan GameTime { get; set; }
        public GameStatus Status { get; set; } = GameStatus.NotStarted;
        public int BalloonsPopped { get; set; }
        public int BalloonsEscaped { get; set; }
        public int Combo { get; set; }
        public int MaxCombo { get; set; }
        public double ScoreMultiplier { get; set; } = 1.0;
        public DateTime? PowerUpExpiryTime { get; set; }
        public PowerUpType? ActivePowerUp { get; set; }
        public bool IsInvincible { get; set; }
        public bool IsTimeFrozen { get; set; }
        public DateTime? TimeFreezeExpiry { get; set; }

        public bool HasActivePowerUp =>
            ActivePowerUp.HasValue
            && PowerUpExpiryTime.HasValue
            && DateTime.Now < PowerUpExpiryTime;

        public void ResetForNewGame()
        {
            Score = 0;
            Level = 1;
            Lives = 3;
            GameTime = TimeSpan.Zero;
            Status = GameStatus.NotStarted;
            BalloonsPopped = 0;
            BalloonsEscaped = 0;
            Combo = 0;
            MaxCombo = 0;
            ScoreMultiplier = 1.0;
            PowerUpExpiryTime = null;
            ActivePowerUp = null;
            IsInvincible = false;
            IsTimeFrozen = false;
            TimeFreezeExpiry = null;
        }

        public void UpdatePowerUpStatus()
        {
            if (PowerUpExpiryTime.HasValue && DateTime.Now >= PowerUpExpiryTime)
            {
                ActivePowerUp = null;
                PowerUpExpiryTime = null;
                ScoreMultiplier = 1.0;
                IsInvincible = false;
            }

            if (TimeFreezeExpiry.HasValue && DateTime.Now >= TimeFreezeExpiry)
            {
                IsTimeFrozen = false;
                TimeFreezeExpiry = null;
            }
        }
    }

    public enum GameStatus
    {
        NotStarted,
        Playing,
        Paused,
        GameOver,
        LevelComplete,
    }

    public enum PowerUpType
    {
        DoublePoints,
        Shield,
        TimeFreeze,
        SlowMotion,
        ExtraLife,
        BombBlast,
    }
}
