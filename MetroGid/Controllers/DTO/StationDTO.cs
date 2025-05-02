namespace MetroGid.Controllers.DTO;

public class StationDTO(
    string title, string branchTitle,
    int occupancy, AccessTypeDTO type,
    TimeOnly open_time, TimeOnly close_time
)
{
    public string Title { get; set; } = title;
    public string BranchTitle { get; set; } = branchTitle;
    public int Occupancy { get; set; } = occupancy;
    public AccessTypeDTO Type { get; set; } = type;
    public TimeOnly OpenTime { get; set; } = open_time;
    public TimeOnly CloseTime { get; set; } = close_time;


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