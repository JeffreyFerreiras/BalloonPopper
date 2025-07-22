using BalloonPopper.Models;

namespace BalloonPopper.Services
{
    /// <summary>
    /// Interface for handling balloon interactions and effects
    /// </summary>
    public interface IBalloonInteractionService
    {
        event EventHandler<BalloonPoppedEventArgs> BalloonPopped;
        event EventHandler<BombExplosionEventArgs> BombExploded;
        event EventHandler<PowerUpActivatedEventArgs> PowerUpActivated;

        bool TryPopBalloon(Balloon balloon, Point tapLocation);
        List<Balloon> GetBalloonsInExplosionRadius(
            Point center,
            double radius,
            IEnumerable<Balloon> balloons
        );
        void ProcessBombExplosion(Balloon bombBalloon, IEnumerable<Balloon> nearbyBalloons);
        bool IsPointInsideBalloon(Point point, Balloon balloon);
        double CalculateDistance(Point point1, Point point2);
    }

    /// <summary>
    /// Service responsible for balloon pop interactions and special effects
    /// </summary>
    public class BalloonInteractionService : IBalloonInteractionService
    {
        private const double BombExplosionRadius = 120.0;
        private const double TapTolerance = 10.0; // Additional tolerance for tapping

        public event EventHandler<BalloonPoppedEventArgs>? BalloonPopped;
        public event EventHandler<BombExplosionEventArgs>? BombExploded;
        public event EventHandler<PowerUpActivatedEventArgs>? PowerUpActivated;

        public bool TryPopBalloon(Balloon balloon, Point tapLocation)
        {
            if (balloon == null || balloon.IsPopped)
                return false;

            if (!IsPointInsideBalloon(tapLocation, balloon))
                return false;

            var scoreEarned = balloon.Pop();
            var eventArgs = new BalloonPoppedEventArgs
            {
                Balloon = balloon,
                ScoreEarned = scoreEarned,
                TapLocation = tapLocation,
            };

            BalloonPopped?.Invoke(this, eventArgs);

            // Handle special balloon effects
            HandleSpecialBalloonEffect(balloon);

            return true;
        }

        public List<Balloon> GetBalloonsInExplosionRadius(
            Point center,
            double radius,
            IEnumerable<Balloon> balloons
        )
        {
            var balloonsInRadius = new List<Balloon>();

            foreach (var balloon in balloons)
            {
                if (balloon.IsPopped)
                    continue;

                var balloonCenter = new Point(
                    balloon.X + balloon.Size / 2,
                    balloon.Y + balloon.Size / 2
                );
                var distance = CalculateDistance(center, balloonCenter);

                if (distance <= radius)
                {
                    balloonsInRadius.Add(balloon);
                }
            }

            return balloonsInRadius;
        }

        public void ProcessBombExplosion(Balloon bombBalloon, IEnumerable<Balloon> nearbyBalloons)
        {
            if (bombBalloon.Type != BalloonType.Bomb)
                return;

            var explosionCenter = new Point(
                bombBalloon.X + bombBalloon.Size / 2,
                bombBalloon.Y + bombBalloon.Size / 2
            );

            var affectedBalloons = GetBalloonsInExplosionRadius(
                explosionCenter,
                BombExplosionRadius,
                nearbyBalloons
            );

            var totalScore = 0;
            var poppedBalloons = new List<Balloon>();

            foreach (var balloon in affectedBalloons)
            {
                if (!balloon.IsPopped)
                {
                    totalScore += balloon.Pop();
                    poppedBalloons.Add(balloon);
                }
            }

            var explosionArgs = new BombExplosionEventArgs
            {
                BombBalloon = bombBalloon,
                ExplosionCenter = explosionCenter,
                ExplosionRadius = BombExplosionRadius,
                AffectedBalloons = poppedBalloons,
                TotalScoreEarned = totalScore,
            };

            BombExploded?.Invoke(this, explosionArgs);
        }

        public bool IsPointInsideBalloon(Point point, Balloon balloon)
        {
            var balloonCenter = new Point(
                balloon.X + balloon.Size / 2,
                balloon.Y + balloon.Size / 2
            );

            var distance = CalculateDistance(point, balloonCenter);
            var effectiveRadius = (balloon.Size / 2) + TapTolerance;

            return distance <= effectiveRadius;
        }

        public double CalculateDistance(Point point1, Point point2)
        {
            var deltaX = point1.X - point2.X;
            var deltaY = point1.Y - point2.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        private void HandleSpecialBalloonEffect(Balloon balloon)
        {
            PowerUpType? powerUpType = balloon.Type switch
            {
                BalloonType.Multiplier => PowerUpType.DoublePoints,
                BalloonType.Shield => PowerUpType.Shield,
                BalloonType.TimeFreeze => PowerUpType.TimeFreeze,
                BalloonType.DoublePoints => PowerUpType.DoublePoints,
                BalloonType.Bomb => PowerUpType.BombBlast,
                _ => null,
            };

            if (powerUpType.HasValue)
            {
                var powerUpArgs = new PowerUpActivatedEventArgs
                {
                    PowerUpType = powerUpType.Value,
                    SourceBalloon = balloon,
                    ActivationTime = DateTime.Now,
                };

                PowerUpActivated?.Invoke(this, powerUpArgs);
            }
        }
    }

    // Event argument classes
    public class BalloonPoppedEventArgs : EventArgs
    {
        public required Balloon Balloon { get; set; }
        public int ScoreEarned { get; set; }
        public Point TapLocation { get; set; }
    }

    public class BombExplosionEventArgs : EventArgs
    {
        public required Balloon BombBalloon { get; set; }
        public Point ExplosionCenter { get; set; }
        public double ExplosionRadius { get; set; }
        public required List<Balloon> AffectedBalloons { get; set; }
        public int TotalScoreEarned { get; set; }
    }

    public class PowerUpActivatedEventArgs : EventArgs
    {
        public PowerUpType PowerUpType { get; set; }
        public required Balloon SourceBalloon { get; set; }
        public DateTime ActivationTime { get; set; }
    }
}
