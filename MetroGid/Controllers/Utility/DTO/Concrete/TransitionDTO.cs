namespace MetroGid.Controllers.Utility.DTO.Concrete;

public record TransitionDTO(
    int Occupancy,
    AccessTypeDTO Type,
    TimeSpan Duration,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    string FromStationTitle,
    string FromBranchTitle,
    string ToStationTitle,
    string ToBranchTitle
);