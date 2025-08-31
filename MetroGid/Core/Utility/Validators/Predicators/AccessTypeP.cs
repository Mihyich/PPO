using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Utility.Validators.Predicators;

public static class AccessTypeP
{
    public static bool IsOutOfRange(int type) => !Enum.IsDefined(typeof(AccessType), type);
}