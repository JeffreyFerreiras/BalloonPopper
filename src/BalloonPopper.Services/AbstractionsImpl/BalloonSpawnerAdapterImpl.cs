using BalloonPopper.Models;

namespace BalloonPopper.Services.AbstractionsImpl
{
    /// <summary>
    /// Implementation of Abstractions IBalloonSpawner that wraps the internal BalloonSpawner
    /// to match the contract differences.
    /// </summary>
    public class BalloonSpawnerAbstractionsImpl : BalloonPopper.Services.Abstractions.IBalloonSpawner, IDisposable
    {
        private readonly BalloonPopper.Services.IBalloonSpawner _impl;
        private BalloonSpawnConfig _config = new();
        private double _width = 360; // default fallback width

        public event EventHandler<Balloon>? BalloonSpawned;

        public BalloonSpawnerAbstractionsImpl(BalloonPopper.Services.IBalloonSpawner impl)
        {
            _impl = impl;
            _impl.BalloonSpawned += (s, b) => BalloonSpawned?.Invoke(this, b);
        }

        public Balloon SpawnBalloon()
        {
            // Create a single balloon using current config as a baseline
            var balloon = new Balloon
            {
                Id = 0,
                Type = BalloonType.Normal,
                Color = BalloonColor.Red,
                Points = _config.BasePoints,
                Size = _config.BaseSize,
                X = 0,
                Y = 0,
                VelocityY = _config.BaseSpeed,
                SpawnTime = DateTime.Now,
                LifeSpan = _config.LifeSpan,
            };
            BalloonSpawned?.Invoke(this, balloon);
            return balloon;
        }

        public void UpdateSpawnConfig(BalloonSpawnConfig config)
        {
            _config = config;
            _impl.UpdateSpawnRate(config);
        }

        public void StopSpawning() => _impl.StopSpawning();

        public void StartSpawning()
        {
            _impl.StartSpawning(_config, _width);
        }

        public void Dispose()
        {
            if (_impl is IDisposable d) d.Dispose();
        }
    }
}
