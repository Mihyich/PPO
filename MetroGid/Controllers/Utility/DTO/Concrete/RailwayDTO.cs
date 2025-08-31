namespace MetroGid.Controllers.Utility.DTO.Concrete;

public record RailwayDTO(
    string BranchTitle,
    string PrevStationTitle,
    string NextStationTitle,
    TimeSpan Duration
);