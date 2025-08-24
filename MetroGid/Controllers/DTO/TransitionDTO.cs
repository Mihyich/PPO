namespace MetroGid.Controllers.DTO;

public record TransitionDTO(
    int Occupancy,
    AccessTypeDTO Type,
    TimeOnly Duration,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    string FromStationTitle,
    string FromBranchTitle,
    string ToStationTitle,
    string ToBranchTitle
);