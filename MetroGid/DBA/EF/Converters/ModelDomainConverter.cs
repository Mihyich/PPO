using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MDEME = MetroGid.DBA.EF.Models.UserDefinedTypes;
using MetroGid.Core.Models.Types;

namespace MetroGid.DBA.EF.Converters;

public static class ModelDomainConverter
{
    public static TEnum Convert<TEnum>(string? type) where TEnum : struct, Enum
    {
        TEnum fallback = Enum.GetValues<TEnum>().Last();

        if (string.IsNullOrWhiteSpace(type))
            return fallback;

        if (Enum.TryParse<TEnum>(type, true, out var parsed) && Enum.IsDefined(parsed))
            return parsed;

        return fallback;
    }

    public static MCMC.Chart Convert(MDEMT.Chart chart) =>
        new(chart.Title, chart.City, chart.SvgContent ?? "");

    public static MCMC.Branch Convert(MDEMT.Branch branch) =>
        new(branch.Title, branch.Color ?? 0, Convert<MCMT.AccessType>(branch.Access));

    public static MCMC.Station Convert(MDEMT.Station station) =>
        new(station.Title, station.Occupancy ?? 0, Convert<MCMT.AccessType>(station.Access),
            station.OpenTime, station.CloseTime);

    public static MCMC.Railway Convert(MDEMT.Railway railway) =>
        new(railway.Duration);

    public static MCMC.Transition Convert(MDEMT.Transition transition) =>
        new(transition.Occupancy ?? 0, Convert<MCMT.AccessType>(transition.Access),
            transition.Duration, transition.OpenTime, transition.CloseTime);

    public static MCMC.Client Convert(MDEMT.Client client) =>
        new(client.ClientLogin, client.ClientPassword, client.Mail, Convert<MCMT.RoleType>(client.Privilege));
}