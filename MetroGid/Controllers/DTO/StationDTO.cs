namespace MetroGid.Controllers.DTO;

public class StationDTO(
    string title, string branchTitle,
    int occupancy, AccessTypeDTO type,
    TimeOnly open_time, TimeOnly close_time
)
{
    public string Title { get; } = title;
    public string BranchTitle { get; } = branchTitle;
    public int Occupancy { get; } = occupancy;
    public AccessTypeDTO Type { get; } = type;
    public TimeOnly OpenTime { get; } = open_time;
    public TimeOnly CloseTime { get; } = close_time;


    public override bool Equals(object? obj) =>
        obj is StationDTO other &&
        Title == other.Title &&
        BranchTitle == other.BranchTitle &&
        Occupancy == other.Occupancy &&
        Type == other.Type &&
        OpenTime == other.OpenTime &&
        CloseTime == other.CloseTime;


    public override int GetHashCode() =>
        HashCode.Combine(Title, BranchTitle, Occupancy, Type, OpenTime, CloseTime);
}