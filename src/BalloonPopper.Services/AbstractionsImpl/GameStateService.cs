using BalloonPopper.Models;
using BalloonPopper.Services.Abstractions;

namespace BalloonPopper.Services.AbstractionsImpl
{
    public class GameStateService : IGameStateService
    {
        private readonly object _gate = new();
        private readonly GameState _state = new();

        public event EventHandler<GameState>? GameStateChanged;

        public GameState CurrentState
        {
            get { lock (_gate) return _state; }
        }

        public void StartNewGame()
        {
            lock (_gate)
            {
                _state.ResetForNewGame();
                _state.Status = GameStatus.Playing;
            }
            RaiseChanged();
        }

        public void EndGame()
        {
            lock (_gate)
            {
                _state.Status = GameStatus.GameOver;
            }
            RaiseChanged();
        }

        public void PauseGame()
        {
            lock (_gate)
            {
                if (_state.Status == GameStatus.Playing)
                    _state.Status = GameStatus.Paused;
            }
            RaiseChanged();
        }

        public void ResumeGame()
        {
            lock (_gate)
            {
                if (_state.Status == GameStatus.Paused)
                    _state.Status = GameStatus.Playing;
            }
            RaiseChanged();
        }

        public void UpdateScore(int points)
        {
            lock (_gate)
            {
                _state.Score += points;
            }
            RaiseChanged();
        }

        public void UpdateLives(int lives)
        {
            var gameOver = false;
            lock (_gate)
            {
                _state.Lives = lives;
                if (_state.Lives <= 0)
                {
                    _state.Status = GameStatus.GameOver;
                    gameOver = true;
                }
            }
            RaiseChanged();
            if (gameOver)
            {
                // no-op, UI reacts to state change
            }
        }

        public void PopBalloon()
        {
            lock (_gate)
            {
                _state.BalloonsPopped++;
            }
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            GameStateChanged?.Invoke(this, CurrentState);
        }
    }
}
