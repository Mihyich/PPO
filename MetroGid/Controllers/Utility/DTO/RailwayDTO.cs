namespace MetroGid.Controllers.Utility.DTO;

public record RailwayDTO(
    string BranchTitle,
    string PrevStationTitle,
    string NextStationTitle,
    TimeOnly Duration
);