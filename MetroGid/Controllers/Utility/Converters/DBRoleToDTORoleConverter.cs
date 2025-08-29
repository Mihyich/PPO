using MetroGid.Controllers.Utility.DTO.Concrete;

namespace MetroGid.Controllers.Utility.Converters;

public static class DBRoleToDTORoleConverter
{
    public static RoleTypeDTO Convert(string role) =>
        role switch
        {
            "unsigned_client" => RoleTypeDTO.UNSIGNED,
            "signed_client" => RoleTypeDTO.SIGNED,
            "duty" => RoleTypeDTO.DUTY,
            _ => RoleTypeDTO.UNSIGNED
        };
}