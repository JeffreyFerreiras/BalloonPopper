using BalloonPopper.Models;

namespace BalloonPopper.Services.Abstractions;

public interface IBalloonSpawner
{
    event EventHandler<Balloon>? BalloonSpawned;
    Balloon SpawnBalloon();
    void UpdateSpawnConfig(BalloonSpawnConfig config);
    void StopSpawning();
    void StartSpawning();
}
