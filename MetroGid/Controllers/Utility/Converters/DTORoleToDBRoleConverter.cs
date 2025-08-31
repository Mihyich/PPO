using MetroGid.Controllers.Utility.DTO.Concrete;

namespace MetroGid.Controllers.Utility.Converters;

public static class DTORoleToDBRoleConverter
{
    public static string Convert(RoleTypeDTO role) =>
        role switch
        {
            RoleTypeDTO.UNSIGNED => "unsigned_client",
            RoleTypeDTO.SIGNED => "signed_client",
            RoleTypeDTO.DUTY => "duty",
            _ => "unknown"
        };
}