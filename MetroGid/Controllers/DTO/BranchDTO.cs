namespace MetroGid.Controllers.DTO;

public class BranchDTO(string title, int color, AccessTypeDTO type)
{
    public string Title { get; } = title;
    public int Color { get; } = color;
    public AccessTypeDTO Type { get; } = type;
}