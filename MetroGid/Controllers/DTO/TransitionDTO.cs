namespace MetroGid.Controllers.DTO;

public class TransitionDTO(
    int occupancy, AccessTypeDTO type, TimeOnly duration,
    TimeOnly open_time, TimeOnly close_time,
    string from_station_title, string from_branch_title,
    string to_station_title, string to_branch_title
)
{
    public int Occupancy { get; set; } = occupancy;
    public AccessTypeDTO Type { get; set; } = type;
    public TimeOnly Duration { get; set; } = duration;
    public TimeOnly OpenTime { get; set; } = open_time;
    public TimeOnly CloseTime { get; set; } = close_time;

    public string FromStationTitle { get; set; } = from_station_title;
    public string FromBranchTitle { get; set; } = from_branch_title;

    public string ToStationTitle { get; set; } = to_station_title;
    public string ToBranchTitle { get; set; } = to_branch_title;


    public override bool Equals(object? obj) =>
        obj is TransitionDTO other &&
        Occupancy == other.Occupancy &&
        Type == other.Type &&
        Duration == other.Duration &&
        OpenTime == other.OpenTime &&
        CloseTime == other.CloseTime &&
        FromStationTitle == other.FromStationTitle &&
        FromBranchTitle == other.FromBranchTitle &&
        ToStationTitle == other.ToStationTitle &&
        ToBranchTitle == other.ToBranchTitle;


    public override int GetHashCode() =>
        HashCode.Combine(Occupancy, Type, Duration, OpenTime, CloseTime, FromStationTitle + FromBranchTitle + ToStationTitle + ToBranchTitle);
}