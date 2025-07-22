using BalloonPopper.Models;
using FluentAssertions;

namespace BalloonPopper.Services.UnitTests
{
    [TestFixture]
    public class BalloonInteractionServiceTests
    {
        private BalloonInteractionService _interactionService;

        [SetUp]
        public void SetUp()
        {
            _interactionService = new BalloonInteractionService();
        }

        [Test]
        public void TryPopBalloon_ShouldReturnTrue_WhenTapIsInsideBalloon()
        {
            // Arrange
            var balloon = new Balloon
            {
                X = 100,
                Y = 100,
                Size = 50,
                Points = 10,
            };
            var tapLocation = new Point(125, 125); // Center of balloon

            // Act
            var result = _interactionService.TryPopBalloon(balloon, tapLocation);

            // Assert
            result.Should().BeTrue();
            balloon.IsPopped.Should().BeTrue();
        }

        [Test]
        public void TryPopBalloon_ShouldReturnFalse_WhenTapIsOutsideBalloon()
        {
            // Arrange
            var balloon = new Balloon
            {
                X = 100,
                Y = 100,
                Size = 50,
                Points = 10,
            };
            var tapLocation = new Point(200, 200); // Far from balloon

            // Act
            var result = _interactionService.TryPopBalloon(balloon, tapLocation);

            // Assert
            result.Should().BeFalse();
            balloon.IsPopped.Should().BeFalse();
        }

        [Test]
        public void TryPopBalloon_ShouldReturnFalse_WhenBalloonIsAlreadyPopped()
        {
            // Arrange
            var balloon = new Balloon
            {
                X = 100,
                Y = 100,
                Size = 50,
                Points = 10,
                IsPopped = true,
            };
            var tapLocation = new Point(125, 125);

            // Act
            var result = _interactionService.TryPopBalloon(balloon, tapLocation);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryPopBalloon_ShouldFireBalloonPoppedEvent_WhenBalloonIsPopped()
        {
            // Arrange
            var balloon = new Balloon
            {
                X = 100,
                Y = 100,
                Size = 50,
                Points = 10,
            };
            var tapLocation = new Point(125, 125);
            var eventFired = false;
            BalloonPoppedEventArgs? eventArgs = null;

            _interactionService.BalloonPopped += (sender, args) =>
            {
                eventFired = true;
                eventArgs = args;
            };

            // Act
            _interactionService.TryPopBalloon(balloon, tapLocation);

            // Assert
            eventFired.Should().BeTrue();
            eventArgs.Should().NotBeNull();
            eventArgs!.Balloon.Should().Be(balloon);
            eventArgs.ScoreEarned.Should().Be(10);
            eventArgs.TapLocation.Should().Be(tapLocation);
        }

        [Test]
        public void IsPointInsideBalloon_ShouldReturnTrue_WhenPointIsInsideBalloon()
        {
            // Arrange
            var balloon = new Balloon
            {
                X = 100,
                Y = 100,
                Size = 50,
            };
            var point = new Point(125, 125); // Center of balloon

            // Act
            var result = _interactionService.IsPointInsideBalloon(point, balloon);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsPointInsideBalloon_ShouldReturnFalse_WhenPointIsOutsideBalloon()
        {
            // Arrange
            var balloon = new Balloon
            {
                X = 100,
                Y = 100,
                Size = 50,
            };
            var point = new Point(200, 200); // Far from balloon

            // Act
            var result = _interactionService.IsPointInsideBalloon(point, balloon);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void CalculateDistance_ShouldReturnCorrectDistance_BetweenTwoPoints()
        {
            // Arrange
            var point1 = new Point(0, 0);
            var point2 = new Point(3, 4);
            var expectedDistance = 5.0; // 3-4-5 triangle

            // Act
            var distance = _interactionService.CalculateDistance(point1, point2);

            // Assert
            distance.Should().BeApproximately(expectedDistance, 0.001);
        }

        [Test]
        public void GetBalloonsInExplosionRadius_ShouldReturnBalloonsWithinRadius()
        {
            // Arrange
            var center = new Point(100, 100);
            var radius = 50.0;
            var balloons = new List<Balloon>
            {
                new()
                {
                    X = 90,
                    Y = 90,
                    Size = 20,
                }, // Within radius
                new()
                {
                    X = 200,
                    Y = 200,
                    Size = 20,
                }, // Outside radius
                new()
                {
                    X = 110,
                    Y = 110,
                    Size = 20,
                }, // Within radius
                new()
                {
                    X = 120,
                    Y = 120,
                    Size = 20,
                    IsPopped = true,
                }, // Within but popped
            };

            // Act
            var balloonsInRadius = _interactionService.GetBalloonsInExplosionRadius(
                center,
                radius,
                balloons
            );

            // Assert
            balloonsInRadius.Should().HaveCount(2); // Only unpopped balloons within radius
        }

        [Test]
        public void ProcessBombExplosion_ShouldPopBalloonsInRadius_WhenBombExplodes()
        {
            // Arrange
            var bombBalloon = new Balloon
            {
                Type = BalloonType.Bomb,
                X = 100,
                Y = 100,
                Size = 50,
                Points = 25,
            };
            var nearbyBalloons = new List<Balloon>
            {
                new()
                {
                    X = 90,
                    Y = 90,
                    Size = 20,
                    Points = 10,
                },
                new()
                {
                    X = 110,
                    Y = 110,
                    Size = 20,
                    Points = 10,
                },
                new()
                {
                    X = 300,
                    Y = 300,
                    Size = 20,
                    Points = 10,
                }, // Too far
            };
            var eventFired = false;
            BombExplosionEventArgs? eventArgs = null;

            _interactionService.BombExploded += (sender, args) =>
            {
                eventFired = true;
                eventArgs = args;
            };

            // Act
            _interactionService.ProcessBombExplosion(bombBalloon, nearbyBalloons);

            // Assert
            eventFired.Should().BeTrue();
            eventArgs.Should().NotBeNull();
            eventArgs!.BombBalloon.Should().Be(bombBalloon);
            eventArgs.AffectedBalloons.Should().HaveCount(2); // Two balloons within explosion radius
            eventArgs.TotalScoreEarned.Should().Be(20); // 2 * 10 points
        }

        [Test]
        public void TryPopBalloon_ShouldFirePowerUpActivatedEvent_WhenSpecialBalloonIsPopped()
        {
            // Arrange
            var multiplierBalloon = new Balloon
            {
                Type = BalloonType.Multiplier,
                X = 100,
                Y = 100,
                Size = 50,
                Points = 20,
            };
            var tapLocation = new Point(125, 125);
            var powerUpEventFired = false;
            PowerUpActivatedEventArgs? powerUpArgs = null;

            _interactionService.PowerUpActivated += (sender, args) =>
            {
                powerUpEventFired = true;
                powerUpArgs = args;
            };

            // Act
            _interactionService.TryPopBalloon(multiplierBalloon, tapLocation);

            // Assert
            powerUpEventFired.Should().BeTrue();
            powerUpArgs.Should().NotBeNull();
            powerUpArgs!.PowerUpType.Should().Be(PowerUpType.DoublePoints);
            powerUpArgs.SourceBalloon.Should().Be(multiplierBalloon);
        }
    }
}
