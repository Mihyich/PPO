namespace MetroGid.Controllers.Utility.DTO.Chart;

public record PatchBranchRequest(
    string CityTitle,
    string ChartTitle,
    string BranchTitle,

    string NewTitle,
    string NewColor, // hex формат
    string NewAccess
);