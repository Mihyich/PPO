using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Converters;

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