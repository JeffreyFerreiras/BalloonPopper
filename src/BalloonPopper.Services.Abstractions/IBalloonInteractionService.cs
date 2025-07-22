using BalloonPopper.Models;

namespace BalloonPopper.Services.Abstractions;

public interface IBalloonInteractionService
{
    event EventHandler<Balloon>? BalloonPopped;
    event EventHandler<int>? ScoreUpdated;
    bool PopBalloon(Balloon balloon, Point touchPoint);
    int CalculateScore(Balloon balloon);
}
