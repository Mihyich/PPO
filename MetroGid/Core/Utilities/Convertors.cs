using MetroGid.Controllers.DTO;
using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public static class ColorConverter
    {
        public static int FromHex(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                return 0;

            hexColor = hexColor.TrimStart('#');

            return hexColor.Length switch
            {
                3 => Convert.ToInt32($"{hexColor[0]}{hexColor[0]}{hexColor[1]}{hexColor[1]}{hexColor[2]}{hexColor[2]}", 16),
                6 => Convert.ToInt32(hexColor, 16),
                _ => 0
            };
        }
    }

    public static class AccessTypeConverter
    {
        public static AccessType FromString(string accessType)
        {
            return accessType?.ToUpperInvariant() switch
            {
                "ACCESSIBLE" => AccessType.ACCESSIBLE,
                "INACCESSIBLE" => AccessType.INACCESSIBLE,
                _ => AccessType.INACCESSIBLE
            };
        }
    }

    public static class TimeConverter
    {
        public static TimeOnly FromString(string timeString)
        {
            if (TimeOnly.TryParse(timeString, out var result))
                return result;

            return new TimeOnly(0, 0);
        }
    }

    public static class CntRoleTypeDTO
    {
        public static RoleTypeDTO Convert(RoleType type) =>
            type switch
            {
                RoleType.UNSIGNED => RoleTypeDTO.UNSIGNED,
                RoleType.SIGNED => RoleTypeDTO.SIGNED,
                RoleType.DUTY => RoleTypeDTO.DUTY,
                _ => RoleTypeDTO.UNSIGNED
            };
    }

    public static class CntAccessTypeDTO
    {
        public static AccessTypeDTO Convert(AccessType type) =>
            type switch
            {
                AccessType.ACCESSIBLE => AccessTypeDTO.ACCESSIBLE,
                AccessType.INACCESSIBLE => AccessTypeDTO.INACCESSIBLE,
                _ => AccessTypeDTO.INACCESSIBLE
            };
    }

    public static class CntChartDTO
    {
        public static ChartDTO Convert(Chart chart) =>
            new(chart.City, chart.Title, chart.SvgInst);
    }

    public static class CntBranchDTO
    {
        public static BranchDTO Convert(Branch branch) =>
            new (branch.Title, branch.Color, CntAccessTypeDTO.Convert(branch.Type));
    }

    public static class CntStationDTO
    {
        public static StationDTO Convert(Station station) =>
            new (station.Title, station.Occupancy, CntAccessTypeDTO.Convert(station.Type),
                station.OpenTime, station.CloseTime);
    }
}