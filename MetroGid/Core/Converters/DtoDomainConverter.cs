using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

namespace MetroGid.Core.Converters;

public static class DtoDomainConverter
{
    public static MCMT.AccessType Convert(MCUD.AccessTypeDTO type) =>
        type switch
        {
            MCUD.AccessTypeDTO.ACCESSIBLE => MCMT.AccessType.ACCESSIBLE,
            MCUD.AccessTypeDTO.INACCESSIBLE => MCMT.AccessType.INACCESSIBLE,
            _ => MCMT.AccessType.INACCESSIBLE
        };

    public static MCMT.RoleType Convert(MCUD.RoleTypeDTO type) =>
        type switch
        {
            MCUD.RoleTypeDTO.UNSIGNED => MCMT.RoleType.UNSIGNED,
            MCUD.RoleTypeDTO.SIGNED => MCMT.RoleType.SIGNED,
            MCUD.RoleTypeDTO.DUTY => MCMT.RoleType.DUTY,
            _ => MCMT.RoleType.UNSIGNED
        };

    public static MCMC.Chart Convert(MCUD.ChartDTO chart) =>
        new(chart.Title, chart.City, chart.SvgInst);

    public static MCMC.Branch Convert(MCUD.BranchDTO branch) =>
        new(branch.Title, branch.Color, Convert(branch.Type));

    public static MCMC.Station Convert(MCUD.StationDTO station) =>
        new(station.Title, station.Occupancy, Convert(station.Type),
            station.OpenTime, station.CloseTime);

    public static MCMC.Railway Convert(MCUD.RailwayDTO railway) =>
        new(railway.Duration);

    public static MCMC.Transition Convert(MCUD.TransitionDTO transition) =>
        new(transition.Occupancy, Convert(transition.Type), transition.Duration,
            transition.OpenTime, transition.CloseTime);

    public static MCMC.Client Convert(MCUD.ClientDTO client) =>
        new(client.Login, client.Password, client.Mail, Convert(client.Role));
}