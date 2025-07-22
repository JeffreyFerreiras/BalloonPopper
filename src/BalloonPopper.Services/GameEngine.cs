using System.Collections.ObjectModel;
using BalloonPopper.Models;

namespace BalloonPopper.Services
{
    /// <summary>
    /// Interface for the main game engine that orchestrates all game mechanics
    /// </summary>
    public interface IGameEngine
    {
        event EventHandler<GameState> GameStateChanged;
        event EventHandler<Balloon> BalloonAdded;
        event EventHandler<Balloon> BalloonRemoved;
        event EventHandler<BombExplosionEventArgs> BombExploded;

        ObservableCollection<Balloon> ActiveBalloons { get; }
        GameState CurrentGameState { get; }
        bool IsRunning { get; }

        void StartGame(double gameAreaWidth, double gameAreaHeight);
        void PauseGame();
        void ResumeGame();
        void EndGame();
        void ProcessTap(Point tapLocation);
        void Update(double deltaTime);
        void SetGameAreaSize(double width, double height);
    }

    /// <summary>
    /// Main game engine service that orchestrates all game mechanics
    /// Following SOLID principles with pure functions and dependency injection
    /// </summary>
    public class GameEngine : IGameEngine, IDisposable
    {
        private readonly IBalloonSpawner _balloonSpawner;
        private readonly IGameStateManager _gameStateManager;
        private readonly IBalloonInteractionService _interactionService;
        private readonly IDifficultyManager _difficultyManager;
        private readonly IScoringService _scoringService;

        private readonly ObservableCollection<Balloon> _activeBalloons;
        private readonly System.Timers.Timer _gameTimer;
        private DateTime _lastUpdateTime;
        private double _gameAreaWidth;
        private double _gameAreaHeight;
        private bool _isRunning;

        public event EventHandler<GameState>? GameStateChanged;
        public event EventHandler<Balloon>? BalloonAdded;
        public event EventHandler<Balloon>? BalloonRemoved;
        public event EventHandler<BombExplosionEventArgs>? BombExploded;

        public ObservableCollection<Balloon> ActiveBalloons => _activeBalloons;
        public GameState CurrentGameState => _gameStateManager.CurrentState;
        public bool IsRunning => _isRunning;

        public GameEngine(
            IBalloonSpawner balloonSpawner,
            IGameStateManager gameStateManager,
            IBalloonInteractionService interactionService,
            IDifficultyManager difficultyManager,
            IScoringService scoringService
        )
        {
            _balloonSpawner =
                balloonSpawner ?? throw new ArgumentNullException(nameof(balloonSpawner));
            _gameStateManager =
                gameStateManager ?? throw new ArgumentNullException(nameof(gameStateManager));
            _interactionService =
                interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _difficultyManager =
                difficultyManager ?? throw new ArgumentNullException(nameof(difficultyManager));
            _scoringService =
                scoringService ?? throw new ArgumentNullException(nameof(scoringService));

            _activeBalloons = new ObservableCollection<Balloon>();
            _gameTimer = new System.Timers.Timer(16); // ~60 FPS
            _gameTimer.Elapsed += OnGameTimerElapsed;
            _lastUpdateTime = DateTime.Now;

            SubscribeToEvents();
        }

        public void StartGame(double gameAreaWidth, double gameAreaHeight)
        {
            SetGameAreaSize(gameAreaWidth, gameAreaHeight);

            _gameStateManager.StartNewGame();
            _activeBalloons.Clear();

            var spawnConfig = _difficultyManager.GetSpawnConfigForLevel(1);
            _balloonSpawner.StartSpawning(spawnConfig, _gameAreaWidth);

            _isRunning = true;
            _lastUpdateTime = DateTime.Now;
            _gameTimer.Start();
        }

        public void PauseGame()
        {
            if (!_isRunning)
                return;

            _gameStateManager.PauseGame();
            _balloonSpawner.StopSpawning();
            _gameTimer.Stop();
            _isRunning = false;
        }

        public void ResumeGame()
        {
            if (_isRunning || CurrentGameState.Status != GameStatus.Paused)
                return;

            _gameStateManager.ResumeGame();

            var spawnConfig = _difficultyManager.GetSpawnConfigForLevel(CurrentGameState.Level);
            _balloonSpawner.StartSpawning(spawnConfig, _gameAreaWidth);

            _isRunning = true;
            _lastUpdateTime = DateTime.Now;
            _gameTimer.Start();
        }

        public void EndGame()
        {
            _isRunning = false;
            _gameTimer.Stop();
            _balloonSpawner.StopSpawning();
            _gameStateManager.EndGame();
            _activeBalloons.Clear();
        }

        public void ProcessTap(Point tapLocation)
        {
            if (!_isRunning || CurrentGameState.Status != GameStatus.Playing)
                return;

            // Find the topmost balloon at the tap location
            var tappedBalloon = FindBalloonAtLocation(tapLocation);
            if (tappedBalloon == null)
                return;

            // Handle special balloon types
            if (tappedBalloon.Type == BalloonType.Bomb)
            {
                ProcessBombExplosion(tappedBalloon);
            }
            else
            {
                // Regular balloon pop
                if (_interactionService.TryPopBalloon(tappedBalloon, tapLocation))
                {
                    _gameStateManager.ProcessBalloonPop(tappedBalloon);
                }
            }
        }

        public void Update(double deltaTime)
        {
            if (!_isRunning || CurrentGameState.Status != GameStatus.Playing)
                return;

            var adjustedDeltaTime = CurrentGameState.IsTimeFrozen ? deltaTime * 0.1 : deltaTime;

            // Update game time
            _gameStateManager.UpdateGameTime(TimeSpan.FromSeconds(deltaTime));

            // Update balloon positions
            UpdateBalloonPositions(adjustedDeltaTime);

            // Remove balloons that should be removed
            RemoveExpiredBalloons();

            // Update difficulty if needed
            UpdateDifficultyForCurrentLevel();
        }

        public void SetGameAreaSize(double width, double height)
        {
            _gameAreaWidth = width;
            _gameAreaHeight = height;
        }

        private void SubscribeToEvents()
        {
            _balloonSpawner.BalloonSpawned += OnBalloonSpawned;
            _gameStateManager.GameStateChanged += OnGameStateChanged;
            _gameStateManager.GameOver += OnGameOver;
            _gameStateManager.LevelChanged += OnLevelChanged;
            _interactionService.BalloonPopped += OnBalloonPopped;
            _interactionService.BombExploded += OnBombExploded;
            _interactionService.PowerUpActivated += OnPowerUpActivated;
        }

        private void OnGameTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var deltaTime = (now - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = now;

            Update(deltaTime);
        }

        private void OnBalloonSpawned(object? sender, Balloon balloon)
        {
            if (balloon != null)
            {
                _activeBalloons.Add(balloon);
                BalloonAdded?.Invoke(this, balloon);
            }
        }

        private void OnGameStateChanged(object? sender, GameState gameState)
        {
            GameStateChanged?.Invoke(this, gameState);
        }

        private void OnGameOver(object? sender, EventArgs e)
        {
            EndGame();
        }

        private void OnLevelChanged(object? sender, int newLevel)
        {
            UpdateDifficultyForCurrentLevel();
        }

        private void OnBalloonPopped(object? sender, BalloonPoppedEventArgs e)
        {
            // Additional balloon popped logic if needed
        }

        private void OnBombExploded(object? sender, BombExplosionEventArgs e)
        {
            BombExploded?.Invoke(this, e);

            // Process all affected balloons
            foreach (var balloon in e.AffectedBalloons)
            {
                _gameStateManager.ProcessBalloonPop(balloon);
            }
        }

        private void OnPowerUpActivated(object? sender, PowerUpActivatedEventArgs e)
        {
            _gameStateManager.ActivatePowerUp(e.PowerUpType);
        }

        private Balloon? FindBalloonAtLocation(Point location)
        {
            // Find the first (topmost) balloon that contains the tap location
            return _activeBalloons.FirstOrDefault(b =>
                !b.IsPopped && _interactionService.IsPointInsideBalloon(location, b)
            );
        }

        private void ProcessBombExplosion(Balloon bombBalloon)
        {
            var nearbyBalloons = _activeBalloons.Where(b => !b.IsPopped && b != bombBalloon);
            _interactionService.ProcessBombExplosion(bombBalloon, nearbyBalloons);
        }

        private void UpdateBalloonPositions(double deltaTime)
        {
            foreach (var balloon in _activeBalloons.ToList())
            {
                if (!balloon.IsPopped)
                {
                    balloon.Update(deltaTime);
                }
            }
        }

        private void RemoveExpiredBalloons()
        {
            var balloonsToRemove = _activeBalloons.Where(b => b.ShouldBeRemoved).ToList();

            foreach (var balloon in balloonsToRemove)
            {
                _activeBalloons.Remove(balloon);
                BalloonRemoved?.Invoke(this, balloon);

                // If balloon escaped (went off screen without being popped)
                if (!balloon.IsPopped && balloon.Y < -balloon.Size)
                {
                    _gameStateManager.ProcessBalloonEscape();
                }
            }
        }

        private void UpdateDifficultyForCurrentLevel()
        {
            if (_difficultyManager.ShouldIncreaseDifficulty(CurrentGameState))
            {
                var newConfig = _difficultyManager.GetSpawnConfigForLevel(CurrentGameState.Level);
                _balloonSpawner.UpdateSpawnRate(newConfig);
            }
        }

        public void Dispose()
        {
            _gameTimer?.Dispose();
            if (_balloonSpawner is IDisposable disposableSpawner)
                disposableSpawner.Dispose();
        }
    }
}
