using MetroGid.Controllers.DTO;
using MetroGid.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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

    public static class CntRoleType
    {
        public static RoleTypeDTO Convert(RoleType type) =>
            type switch
            {
                RoleType.UNSIGNED => RoleTypeDTO.UNSIGNED,
                RoleType.SIGNED => RoleTypeDTO.SIGNED,
                RoleType.DUTY => RoleTypeDTO.DUTY,
                _ => RoleTypeDTO.UNSIGNED
            };

        public static RoleType Convert(RoleTypeDTO type) =>
            type switch
            {
                RoleTypeDTO.UNSIGNED => RoleType.UNSIGNED,
                RoleTypeDTO.SIGNED => RoleType.SIGNED,
                RoleTypeDTO.DUTY => RoleType.DUTY,
                _ => RoleType.UNSIGNED
            };
    }

    public static class CntAccessType
    {
        public static AccessTypeDTO Convert(AccessType type) =>
            type switch
            {
                AccessType.ACCESSIBLE => AccessTypeDTO.ACCESSIBLE,
                AccessType.INACCESSIBLE => AccessTypeDTO.INACCESSIBLE,
                _ => AccessTypeDTO.INACCESSIBLE
            };

        public static AccessType Convert(AccessTypeDTO type) =>
            type switch
            {
                AccessTypeDTO.ACCESSIBLE => AccessType.ACCESSIBLE,
                AccessTypeDTO.INACCESSIBLE => AccessType.INACCESSIBLE,
                _ => AccessType.INACCESSIBLE
            };
    }

    public static class CntChart
    {
        public static ChartDTO Convert(Chart chart) =>
            new(chart.City, chart.Title, chart.SvgInst);

        public static Chart Convert(ChartDTO chart) =>
            new(chart.Title, chart.City, chart.SvgInst);
    }

    public static class CntBranch
    {
        public static BranchDTO Convert(Branch branch) =>
            new(branch.Title, branch.Color, CntAccessType.Convert(branch.Type));

        public static Branch Convert(BranchDTO branch) =>
            new(branch.Title, branch.Color, CntAccessType.Convert(branch.Type));
    }

    public static class CntStation
    {
        public static StationDTO Convert(Station station) =>
            new(station.Title, station.Occupancy, CntAccessType.Convert(station.Type),
                station.OpenTime, station.CloseTime);

        public static Station Convert(StationDTO station) =>
            new(station.Title, station.Occupancy, CntAccessType.Convert(station.Type),
                station.OpenTime, station.CloseTime);
    }

    public static class CntRailway
    {
        public static RailwayDTO Convert(Railway railway) =>
            new(railway.Prev?.Title ?? string.Empty,
                railway.Next?.Title ?? string.Empty,
                railway.Duration);
    }

    public static class CntTransition
    {
        public static TransitionDTO Convert(Transition transition) =>
            new(transition.Occupancy, CntAccessType.Convert(transition.Type),
                transition.Duration, transition.OpenTime, transition.CloseTime,
                transition.From?.Title ?? string.Empty,
                transition.From?.Branch?.Title ?? string.Empty,
                transition.To?.Title ?? string.Empty,
                transition.To?.Branch?.Title ?? string.Empty);
    }

    public static class CntRoute
    {
        public static RouteDTO Convert(Route route)
        {
            List<RouteItemDTO> CntPath = [];

            foreach (var item in route.Path)
            {
                if (item is RouteStationItem {Station: var station})
                    CntPath.Add(new RouteStationItemDTO(CntStation.Convert(station)));
                else if (item is RouteConnectionItem {Connection: var connection})
                {
                    if (connection is RailwayConnection {Railway: var railway})
                        CntPath.Add(new RouteConnectionItemDTO(new RailwayConnectionDTO(CntRailway.Convert(railway))));
                    else if (connection is TransitionConnection {Transition: var transition})
                        CntPath.Add(new RouteConnectionItemDTO(new TransitionConnectionDTO(CntTransition.Convert(transition))));
                }
            }

            return new()
            {
                Title = route.Title,
                City = route.Chart?.City ?? string.Empty,
                ChartTitle = route.Chart?.Title ?? string.Empty,
                Path = CntPath,
                Duration = route.Duration
            };
        }

        public static Route Convert(RouteDTO route)
        {
            return new();
        }
    }
}