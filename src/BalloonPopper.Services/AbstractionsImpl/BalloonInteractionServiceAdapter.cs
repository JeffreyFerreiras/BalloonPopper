using BalloonPopper.Models;

namespace BalloonPopper.Services.AbstractionsImpl
{
    /// <summary>
    /// Adapter implementing Abstractions IBalloonInteractionService using internal BalloonInteractionService.
    /// </summary>
    public class BalloonInteractionServiceAdapter : BalloonPopper.Services.Abstractions.IBalloonInteractionService
    {
        private readonly BalloonPopper.Services.IBalloonInteractionService _impl = new BalloonPopper.Services.BalloonInteractionService();

        public event EventHandler<Balloon>? BalloonPopped;
        public event EventHandler<int>? ScoreUpdated;

        public BalloonInteractionServiceAdapter()
        {
            _impl.BalloonPopped += (s, e) => BalloonPopped?.Invoke(this, e.Balloon);
            _impl.BombExploded += (s, e) => ScoreUpdated?.Invoke(this, e.TotalScoreEarned);
        }

        public bool PopBalloon(Balloon balloon, Point touchPoint)
        {
            var popped = _impl.TryPopBalloon(balloon, touchPoint);
            if (popped)
            {
                ScoreUpdated?.Invoke(this, balloon.Points);
            }
            return popped;
        }

        public int CalculateScore(Balloon balloon)
        {
            return balloon.Points;
        }
    }
}
