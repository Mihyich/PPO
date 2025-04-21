using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities.Validators.Predicators
{
    public static class AccessTypeP
    {
        public static bool IsOutOfRange(int type) => !Enum.IsDefined(typeof(AccessType), type);
    }
}