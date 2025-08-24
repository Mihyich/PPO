namespace MetroGid.Controllers.DTO;

public record RailwayDTO(
    string BranchTitle,
    string PrevStationTitle,
    string NextStationTitle,
    TimeOnly Duration
);