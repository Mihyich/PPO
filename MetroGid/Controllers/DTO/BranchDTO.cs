namespace MetroGid.Controllers.DTO
{
    public class BranchDTO(string title, int color, AccessTypeDTO type)
    {
        public string Title { get; set; } = title;
        public int Color { get; set; } = color;
        public AccessTypeDTO Type { get; set; } = type;
    }
}