namespace MetroGid.Core.Utilities.Validators.Predicators;

public static class OccupancyP
{
    public static readonly int MaxValue = 10;
    public static readonly int MinValue = 0;

    public static bool IsOutOfRange(int occupancy) =>
        occupancy < MinValue || occupancy > MaxValue;
}