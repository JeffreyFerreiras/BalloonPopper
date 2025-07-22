using FluentAssertions;

namespace BalloonPopper.Models.UnitTests
{
    [TestFixture]
    public class BalloonTests
    {
        private Balloon _balloon;

        [SetUp]
        public void SetUp()
        {
            _balloon = new Balloon
            {
                Id = 1,
                Type = BalloonType.Normal,
                Color = BalloonColor.Red,
                Points = 10,
                Size = 50,
                X = 100,
                Y = 200,
                VelocityY = 50,
                LifeSpan = 5.0,
                SpawnTime = DateTime.Now.AddSeconds(-1),
            };
        }

        [Test]
        public void Update_ShouldMoveBallon_WhenCalledWithDeltaTime()
        {
            // Arrange
            var initialY = _balloon.Y;
            var deltaTime = 0.1;
            var expectedY = initialY - (_balloon.VelocityY * deltaTime);

            // Act
            _balloon.Update(deltaTime);

            // Assert
            _balloon.Y.Should().Be(expectedY);
        }

        [Test]
        public void Pop_ShouldReturnPoints_WhenBalloonIsNotPopped()
        {
            // Arrange & Act
            var points = _balloon.Pop();

            // Assert
            points.Should().Be(10);
            _balloon.IsPopped.Should().BeTrue();
        }

        [Test]
        public void Pop_ShouldReturnZero_WhenBalloonIsAlreadyPopped()
        {
            // Arrange
            _balloon.Pop(); // Pop it first

            // Act
            var points = _balloon.Pop();

            // Assert
            points.Should().Be(0);
        }

        [Test]
        public void IsExpired_ShouldReturnTrue_WhenLifeSpanHasExceeded()
        {
            // Arrange
            _balloon.SpawnTime = DateTime.Now.AddSeconds(-10); // 10 seconds ago
            _balloon.LifeSpan = 5.0; // 5-second lifespan

            // Act & Assert
            _balloon.IsExpired.Should().BeTrue();
        }

        [Test]
        public void IsExpired_ShouldReturnFalse_WhenWithinLifeSpan()
        {
            // Arrange
            _balloon.SpawnTime = DateTime.Now.AddSeconds(-2); // 2 seconds ago
            _balloon.LifeSpan = 5.0; // 5-second lifespan

            // Act & Assert
            _balloon.IsExpired.Should().BeFalse();
        }

        [Test]
        public void ShouldBeRemoved_ShouldReturnTrue_WhenBalloonIsPopped()
        {
            // Arrange
            _balloon.Pop();

            // Act & Assert
            _balloon.ShouldBeRemoved.Should().BeTrue();
        }

        [Test]
        public void ShouldBeRemoved_ShouldReturnTrue_WhenBalloonIsOffScreen()
        {
            // Arrange
            _balloon.Y = -100; // Above screen
            _balloon.Size = 50;

            // Act & Assert
            _balloon.ShouldBeRemoved.Should().BeTrue();
        }

        [Test]
        public void PropertyChanged_ShouldFire_WhenPositionIsUpdated()
        {
            // Arrange
            var propertyChangedFired = false;
            _balloon.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Balloon.X))
                    propertyChangedFired = true;
            };

            // Act
            _balloon.X = 150;

            // Assert
            propertyChangedFired.Should().BeTrue();
        }
    }
}
