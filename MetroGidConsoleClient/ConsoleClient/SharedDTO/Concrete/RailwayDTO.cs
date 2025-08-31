namespace ConsoleClient.SharedDTO.Concrete;

public record RailwayDTO(
    string BranchTitle,
    string PrevStationTitle,
    string NextStationTitle,
    TimeSpan Duration
);