using MetroGid.Controllers.Utility.DTO.Chart;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Core.Converters;

namespace MetroGid.Controllers.Utility.Converters;

public static class RequestToDTOConverter
{
    public static AccessTypeDTO ConvertAccessType(string type) =>
        type switch
        {
            "ACCESSIBLE" => AccessTypeDTO.ACCESSIBLE,
            "INACCESSIBLE" => AccessTypeDTO.INACCESSIBLE,
            _ => AccessTypeDTO.INACCESSIBLE
        };

    public static BranchDTO Convert(PatchBranchRequest pbr) =>
        new(
            pbr.NewTitle,
            ColorConverter.FromHex(pbr.NewColor),
            ConvertAccessType(pbr.NewAccess)
        );

    public static StationDTO Convert(PatchStationRequest psr) =>
        new(
            psr.NewTitle,
            psr.BranchTitle,
            psr.NewOccupancy,
            ConvertAccessType(psr.NewAccess),
            TimeConverter.FromString(psr.NewOpenTime),
            TimeConverter.FromString(psr.NewCloseTime)
        );
}