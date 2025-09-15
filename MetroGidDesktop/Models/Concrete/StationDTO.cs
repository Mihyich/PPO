using System;

namespace MetroGid.Controllers.Utility.DTO.Concrete;

public record StationDTO(
    string Title,
    string BranchTitle,
    int Occupancy,
    AccessTypeDTO Type,
    TimeOnly OpenTime,
    TimeOnly CloseTime
);