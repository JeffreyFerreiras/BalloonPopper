using System.Collections.ObjectModel;
using BalloonPopper.Models;
using BalloonPopper.Services.Abstractions;

namespace BalloonPopper.Services
{
    /// <summary>
    /// Minimal implementation of IGameEngine (Abstractions) that runs a basic loop.
    /// </summary>
    public class SimpleGameEngine : BalloonPopper.Services.Abstractions.IGameEngine, IDisposable
    {
        private readonly BalloonPopper.Services.Abstractions.IBalloonSpawner _spawner;
        private readonly BalloonPopper.Services.Abstractions.IGameStateService _state;
        private readonly BalloonPopper.Services.Abstractions.IBalloonInteractionService _interaction;
        private readonly BalloonPopper.Services.Abstractions.IDifficultyManager _difficulty;

        private readonly ObservableCollection<Balloon> _active = new();
        private readonly System.Timers.Timer _tick;
        private DateTime _lastUpdate;

        public bool IsRunning { get; private set; }

        public SimpleGameEngine(
            BalloonPopper.Services.Abstractions.IBalloonSpawner spawner,
            BalloonPopper.Services.Abstractions.IGameStateService state,
            BalloonPopper.Services.Abstractions.IBalloonInteractionService interaction,
            BalloonPopper.Services.Abstractions.IDifficultyManager difficulty)
        {
            _spawner = spawner;
            _state = state;
            _interaction = interaction;
            _difficulty = difficulty;

            _tick = new System.Timers.Timer(16); // ~60 FPS
            _tick.Elapsed += (_, __) =>
            {
                var now = DateTime.Now;
                var dt = (now - _lastUpdate).TotalSeconds;
                _lastUpdate = now;
                Update(dt);
            };

            _spawner.BalloonSpawned += (_, b) =>
            {
                if (b != null)
                {
                    _active.Add(b);
                }
            };

            _interaction.BalloonPopped += (_, balloon) =>
            {
                _state.PopBalloon();
                _state.UpdateScore(balloon.Points);
            };
        }

        public void StartGame()
        {
            if (IsRunning) return;

            // Reset state
            _state.StartNewGame();

            // Configure spawner based on starting difficulty
            var spawnConfig = new BalloonSpawnConfig().GetConfigForLevel(1);
            if (_spawner is BalloonPopper.Services.AbstractionsImpl.BalloonSpawnerAbstractionsImpl impl)
            {
                impl.UpdateSpawnConfig(spawnConfig);
                impl.StartSpawning();
            }
            else
            {
                // Fallback if adapter not used
                _spawner.UpdateSpawnConfig(spawnConfig);
                _spawner.StartSpawning();
            }

            _lastUpdate = DateTime.Now;
            IsRunning = true;
            _tick.Start();
        }

        public void PauseGame()
        {
            if (!IsRunning) return;
            _state.PauseGame();
            IsRunning = false;
            _tick.Stop();
            _spawner.StopSpawning();
        }

        public void ResumeGame()
        {
            if (IsRunning) return;
            if (_state.CurrentState.Status != GameStatus.Paused) return;

            _state.ResumeGame();
            _spawner.StartSpawning();
            _lastUpdate = DateTime.Now;
            IsRunning = true;
            _tick.Start();
        }

        public void EndGame()
        {
            if (!IsRunning && _state.CurrentState.Status == GameStatus.GameOver) return;
            IsRunning = false;
            _tick.Stop();
            _spawner.StopSpawning();
            _active.Clear();
            _state.EndGame();
        }

        public void Update(double deltaTime)
        {
            if (!IsRunning) return;
            if (_state.CurrentState.Status != GameStatus.Playing) return;

            // Move balloons upward and remove expired or escaped
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var b = _active[i];
                if (!b.IsPopped)
                {
                    b.Update(deltaTime);
                }

                if (b.ShouldBeRemoved)
                {
                    // If escaped off the top, lose life
                    if (!b.IsPopped && b.Y < -b.Size)
                    {
                        var newLives = Math.Max(0, _state.CurrentState.Lives - 1);
                        _state.UpdateLives(newLives);
                        if (newLives <= 0)
                        {
                            EndGame();
                            return;
                        }
                    }
                    _active.RemoveAt(i);
                }
            }
        }

        public void Dispose()
        {
            _tick?.Dispose();
        }
    }
}
