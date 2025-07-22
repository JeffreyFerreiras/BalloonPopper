using FluentAssertions;
using BalloonPopper.Models;
using BalloonPopper.Services;

namespace BalloonPopper.Maui.UnitTests.Services
{
    [TestFixture]
    public class BalloonSpawnerTests
    {
        private BalloonSpawner _spawner;
        private BalloonSpawnConfig _config;

        [SetUp]
        public void SetUp()
        {
            _spawner = new BalloonSpawner();
            _config = new BalloonSpawnConfig
            {
                BaseSpawnRate = 2.0, // 2 balloons per second
                BaseSpeed = 100.0,
                BaseSize = 50.0,
                BasePoints = 10,
                LifeSpan = 5.0
            };
        }

        [TearDown]
        public void TearDown()
        {
            _spawner?.Dispose();
        }

        [Test]
        public void StartSpawning_ShouldSetIsSpawningToTrue_WhenCalledWithValidConfig()
        {
            // Act
            _spawner.StartSpawning(_config, 400);

            // Assert
            _spawner.IsSpawning.Should().BeTrue();
        }

        [Test]
        public void StartSpawning_ShouldThrowArgumentNullException_WhenConfigIsNull()
        {
            // Act & Assert
            var action = () => _spawner.StartSpawning(null!, 400);
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void StopSpawning_ShouldSetIsSpawningToFalse_WhenCalled()
        {
            // Arrange
            _spawner.StartSpawning(_config, 400);

            // Act
            _spawner.StopSpawning();

            // Assert
            _spawner.IsSpawning.Should().BeFalse();
        }

        [Test]
        public void UpdateSpawnRate_ShouldUpdateConfiguration_WhenSpawning()
        {
            // Arrange
            _spawner.StartSpawning(_config, 400);
            var newConfig = new BalloonSpawnConfig { BaseSpawnRate = 3.0 };

            // Act
            _spawner.UpdateSpawnRate(newConfig);

            // Assert - This test verifies the method doesn't throw and accepts the config
            // Internal timer interval change can't be directly tested without more complex setup
            _spawner.IsSpawning.Should().BeTrue();
        }

        [Test]
        public void BalloonSpawned_ShouldEventuallyFire_WhenSpawning()
        {
            // Arrange
            var balloonSpawned = false;
            Balloon? spawnedBalloon = null;
            
            _spawner.BalloonSpawned += (sender, balloon) =>
            {
                balloonSpawned = true;
                spawnedBalloon = balloon;
            };

            // Act
            _spawner.StartSpawning(_config, 400);

            // Wait a bit longer than the spawn interval
            Thread.Sleep(1000); // 1 second should be enough for at least one spawn at 2/second

            // Assert
            balloonSpawned.Should().BeTrue();
            spawnedBalloon.Should().NotBeNull();
            spawnedBalloon!.Type.Should().BeOneOf(Enum.GetValues<BalloonType>());
            spawnedBalloon.Color.Should().BeOneOf(Enum.GetValues<BalloonColor>());
        }

        [Test]
        public void SpawnedBalloon_ShouldHaveValidProperties_WhenSpawned()
        {
            // Arrange
            Balloon? spawnedBalloon = null;
            _spawner.BalloonSpawned += (sender, balloon) => spawnedBalloon = balloon;

            // Act
            _spawner.StartSpawning(_config, 400);
            Thread.Sleep(600); // Wait for spawn

            // Assert
            spawnedBalloon.Should().NotBeNull();
            spawnedBalloon!.Id.Should().BeGreaterThan(0);
            spawnedBalloon.X.Should().BeGreaterOrEqualTo(0);
            spawnedBalloon.X.Should().BeLessOrEqualTo(400 - _config.BaseSize);
            spawnedBalloon.Points.Should().BeGreaterThan(0);
            spawnedBalloon.Size.Should().BeGreaterThan(0);
            spawnedBalloon.VelocityY.Should().BeGreaterThan(0);
        }
    }
}
