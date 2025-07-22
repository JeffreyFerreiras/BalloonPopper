using BalloonPopper.Models;

namespace BalloonPopper.Services
{
    /// <summary>
    /// Interface for balloon spawning logic
    /// </summary>
    public interface IBalloonSpawner
    {
        event EventHandler<Balloon> BalloonSpawned;
        void StartSpawning(BalloonSpawnConfig config, double gameAreaWidth);
        void StopSpawning();
        void UpdateSpawnRate(BalloonSpawnConfig config);
        bool IsSpawning { get; }
    }

    /// <summary>
    /// Service responsible for spawning balloons with configurable parameters
    /// </summary>
    public class BalloonSpawner : IBalloonSpawner, IDisposable
    {
        private readonly Random _random;
        private readonly System.Timers.Timer _spawnTimer;
        private BalloonSpawnConfig? _currentConfig;
        private double _gameAreaWidth;
        private int _balloonIdCounter;
        private bool _isSpawning;

        public event EventHandler<Balloon>? BalloonSpawned;

        public bool IsSpawning => _isSpawning;

        public BalloonSpawner()
        {
            _random = new Random();
            _spawnTimer = new System.Timers.Timer();
            _spawnTimer.Elapsed += OnSpawnTimerElapsed;
            _balloonIdCounter = 0;
        }

        public void StartSpawning(BalloonSpawnConfig config, double gameAreaWidth)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _currentConfig = config;
            _gameAreaWidth = gameAreaWidth;
            _isSpawning = true;

            // Convert spawn rate to timer interval (milliseconds)
            var intervalMs = 1000.0 / config.BaseSpawnRate;
            _spawnTimer.Interval = intervalMs;
            _spawnTimer.Start();
        }

        public void StopSpawning()
        {
            _isSpawning = false;
            _spawnTimer.Stop();
        }

        public void UpdateSpawnRate(BalloonSpawnConfig config)
        {
            if (config == null || !_isSpawning)
                return;

            _currentConfig = config;
            var intervalMs = 1000.0 / config.BaseSpawnRate;
            _spawnTimer.Interval = intervalMs;
        }

        private void OnSpawnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_isSpawning || _currentConfig == null)
                return;

            var balloon = CreateBalloon();
            BalloonSpawned?.Invoke(this, balloon);
        }

        private Balloon CreateBalloon()
        {
            if (_currentConfig == null)
                throw new InvalidOperationException("Spawn config not set");

            var balloonType = DetermineBalloonType();
            var balloon = new Balloon
            {
                Id = ++_balloonIdCounter,
                Type = balloonType,
                Color = GetRandomColor(),
                X = _random.NextDouble() * (_gameAreaWidth - _currentConfig.BaseSize),
                Y = 100, // Start below the visible area
                VelocityY = CalculateVelocity(balloonType),
                Size = CalculateSize(balloonType),
                Points = CalculatePoints(balloonType),
                SpawnTime = DateTime.Now,
                LifeSpan = _currentConfig.LifeSpan,
            };

            return balloon;
        }

        private BalloonType DetermineBalloonType()
        {
            if (_currentConfig == null)
                return BalloonType.Normal;

            var roll = _random.NextDouble();
            var cumulativeProbability = 0.0;

            // Check for special balloons in order of rarity
            cumulativeProbability += _currentConfig.TimeFreezeBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.TimeFreeze;

            cumulativeProbability += _currentConfig.BombBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.Bomb;

            cumulativeProbability += _currentConfig.ShieldBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.Shield;

            cumulativeProbability += _currentConfig.MultiplierBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.Multiplier;

            cumulativeProbability += _currentConfig.DoublePointsBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.DoublePoints;

            cumulativeProbability += _currentConfig.BonusBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.Bonus;

            cumulativeProbability += _currentConfig.GiantBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.Giant;

            cumulativeProbability += _currentConfig.SpeedBalloonChance;
            if (roll < cumulativeProbability)
                return BalloonType.Speed;

            return BalloonType.Normal;
        }

        private BalloonColor GetRandomColor()
        {
            var colors = Enum.GetValues<BalloonColor>();
            // Rainbow balloons are rarer
            var colorPool = colors.Where(c => c != BalloonColor.Rainbow).ToArray();

            if (_random.NextDouble() < 0.05) // 5% chance for rainbow
                return BalloonColor.Rainbow;

            return colorPool[_random.Next(colorPool.Length)];
        }

        private double CalculateVelocity(BalloonType type)
        {
            if (_currentConfig == null)
                return 100.0;

            var baseVelocity = _currentConfig.BaseSpeed;
            var variation = baseVelocity * 0.3; // 30% variation

            var velocity = baseVelocity + (_random.NextDouble() - 0.5) * variation;

            return type switch
            {
                BalloonType.Speed => velocity * 1.8,
                BalloonType.Giant => velocity * 0.7,
                BalloonType.Bomb => velocity * 1.2,
                _ => velocity,
            };
        }

        private double CalculateSize(BalloonType type)
        {
            if (_currentConfig == null)
                return 50.0;

            var baseSize = _currentConfig.BaseSize;
            var variation = _currentConfig.SizeVariation;

            var size = baseSize + (_random.NextDouble() - 0.5) * variation;

            return type switch
            {
                BalloonType.Giant => size * 1.5,
                BalloonType.Speed => size * 0.8,
                BalloonType.Bomb => size * 1.2,
                _ => size,
            };
        }

        private int CalculatePoints(BalloonType type)
        {
            if (_currentConfig == null)
                return 10;

            var basePoints = _currentConfig.BasePoints;

            return type switch
            {
                BalloonType.Normal => basePoints,
                BalloonType.Speed => basePoints + 5,
                BalloonType.Giant => basePoints + 15,
                BalloonType.Bonus => basePoints * 2,
                BalloonType.Multiplier => basePoints + 20,
                BalloonType.Bomb => basePoints + 25,
                BalloonType.Shield => basePoints + 30,
                BalloonType.TimeFreeze => basePoints + 35,
                BalloonType.DoublePoints => basePoints + 20,
                _ => basePoints,
            };
        }

        public void Dispose()
        {
            _spawnTimer?.Dispose();
        }
    }
}
