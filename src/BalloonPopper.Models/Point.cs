namespace BalloonPopper.Models;

/// <summary>
/// Represents a point in 2D space with X and Y coordinates
/// </summary>
public readonly struct Point(double x, double y)
{
    public double X { get; } = x;
    public double Y { get; } = y;

    public static bool operator ==(Point left, Point right) =>
        left.X.Equals(right.X) && left.Y.Equals(right.Y);

    public static bool operator !=(Point left, Point right) => !(left == right);

    public override bool Equals(object? obj) => obj is Point point && this == point;

    public override int GetHashCode() => HashCode.Combine(X, Y);
}
