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
}