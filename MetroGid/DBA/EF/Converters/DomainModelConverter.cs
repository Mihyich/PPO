using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MDEME = MetroGid.DBA.EF.Models.UserDefinedTypes;

namespace MetroGid.DBA.EF.Converters;

public static class DomainModelConverter
{
    public static string Convert(MCMT.AccessType type) =>
        type switch
        {
            MCMT.AccessType.ACCESSIBLE => MDEME.AccessType.ACCESSIBLE.ToString(),
            MCMT.AccessType.INACCESSIBLE => MDEME.AccessType.INACCESSIBLE.ToString(),
            _ => MDEME.AccessType.INACCESSIBLE.ToString()
        };

    public static string Convert(MCMT.WayType type) =>
        type switch
        {
            MCMT.WayType.STATION => MDEME.NexusType.STATION.ToString(),
            MCMT.WayType.RAILWAY => MDEME.NexusType.RAILWAY.ToString(),
            MCMT.WayType.TRANSITION => MDEME.NexusType.TRANSITION.ToString(),
            _ => MDEME.NexusType.STATION.ToString()
        };

    public static string Convert(MCMT.RoleType type) =>
        type switch
        {
            MCMT.RoleType.UNSIGNED => MDEME.RoleType.UNSIGNED.ToString(),
            MCMT.RoleType.SIGNED => MDEME.RoleType.SIGNED.ToString(),
            MCMT.RoleType.DUTY => MDEME.RoleType.DUTY.ToString(),
            _ => MDEME.RoleType.UNSIGNED.ToString()
        };

    public static MDEMT.Chart Convert(MCMC.Chart chart) =>
        new()
        {
            City = chart.City,
            Title = chart.Title,
            SvgContent = chart.SvgInst
        };

    public static MDEMT.Branch Convert(MCMC.Branch branch) =>
        new()
        {
            Title = branch.Title,
            Color = branch.Color,
            Access = Convert(branch.Type)
        };

    public static MDEMT.Station Convert(MCMC.Station station) =>
        new()
        {
            Title = station.Title,
            Occupancy = (short)station.Occupancy,
            Access = Convert(station.Type),
            OpenTime = station.OpenTime,
            CloseTime = station.CloseTime
        };

    public static MDEMT.Railway Convert(MCMC.Railway railway) =>
        new()
        {
            // FromId,
            // ToId,
            Duration = railway.Duration
        };

    public static MDEMT.Transition Convert(MCMC.Transition transition) =>
        new()
        {
            Occupancy = (short)transition.Occupancy,
            Access = Convert(transition.Type),
            Duration = transition.Duration,
            OpenTime = transition.OpenTime,
            CloseTime = transition.CloseTime
        };

    public static MDEMT.Client Convert(MCMC.Client client) =>
        new()
        {
            ClientLogin = client.Login,
            ClientPassword = client.Password,
            Mail = client.Mail,
            Privilege = Convert(client.Role)  
        };
}