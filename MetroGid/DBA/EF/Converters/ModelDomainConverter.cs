using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MDEME = MetroGid.DBA.EF.Models.UserDefinedTypes;

namespace MetroGid.DBA.EF.Converters;

public static class ModelDomainConverter
{
    public static MCMT.AccessType Convert(MDEME.AccessType type) =>
        type switch
        {
            MDEME.AccessType.ACCESSIBLE => MCMT.AccessType.ACCESSIBLE,
            MDEME.AccessType.INACCESSIBLE => MCMT.AccessType.INACCESSIBLE,
            _ => MCMT.AccessType.INACCESSIBLE
        };

    public static MCMT.WayType Convert(MDEME.NexusType type) =>
        type switch
        {
            MDEME.NexusType.STATION => MCMT.WayType.STATION,
            MDEME.NexusType.RAILWAY => MCMT.WayType.RAILWAY,
            MDEME.NexusType.TRANSITION => MCMT.WayType.TRANSITION,
            _ => MCMT.WayType.STATION
        };

    public static MCMT.RoleType Convert(MDEME.RoleType type) =>
        type switch
        {
            MDEME.RoleType.UNSIGNED => MCMT.RoleType.UNSIGNED,
            MDEME.RoleType.SIGNED => MCMT.RoleType.SIGNED,
            MDEME.RoleType.DUTY => MCMT.RoleType.DUTY,
            _ => MCMT.RoleType.UNSIGNED
        };

    public static MCMC.Chart Convert(MDEMT.Chart chart) =>
        new(chart.Title, chart.City, chart.SvgContent ?? "");

    public static MCMC.Branch Convert(MDEMT.Branch branch) =>
        new(branch.Title, branch.Color ?? 0, Convert(branch.Access));

    public static MCMC.Station Convert(MDEMT.Station station) =>
        new(station.Title, station.Occupancy ?? 0, Convert(station.Access),
            station.OpenTime, station.CloseTime);

    public static MCMC.Railway Convert(MDEMT.Railway railway) =>
        new(railway.Duration);

    public static MCMC.Transition Convert(MDEMT.Transition transition) =>
        new(transition.Occupancy ?? 0, Convert(transition.Access),
            transition.Duration, transition.OpenTime, transition.CloseTime);

    public static MCMC.Client Convert(MDEMT.Client client) =>
        new(client.ClientLogin, client.ClientPassword, client.Mail, Convert(client.Privilege));
}