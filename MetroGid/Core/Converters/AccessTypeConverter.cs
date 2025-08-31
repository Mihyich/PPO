using MCMT = MetroGid.Core.Models.Types;

namespace MetroGid.Core.Converters;

public static class AccessTypeConverter
{
    public static MCMT.AccessType FromString(string accessType)
    {
        return accessType?.ToUpperInvariant() switch
        {
            "ACCESSIBLE" => MCMT.AccessType.ACCESSIBLE,
            "INACCESSIBLE" => MCMT.AccessType.INACCESSIBLE,
            _ => MCMT.AccessType.INACCESSIBLE
        };
    }
}