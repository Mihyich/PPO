namespace MetroGid.Controllers.DTO;

public class RailwayDTO(
    string branchTitle,
    string prev_station_title,
    string next_station_title,
    TimeOnly duration
)
{
    public string BranchTitle { get; set; } = branchTitle;
    public string PrevStationTitle { get; set; } = prev_station_title;
    public string NextStationTile { get; set; } = next_station_title;
    public TimeOnly Duration = duration;


    public override bool Equals(object? obj) =>
        obj is RailwayDTO other &&
        BranchTitle == other.BranchTitle &&
        PrevStationTitle == other.PrevStationTitle &&
        NextStationTile == other.NextStationTile &&
        Duration == other.Duration;


    public override int GetHashCode() =>
        HashCode.Combine(BranchTitle, PrevStationTitle, NextStationTile, Duration);
}