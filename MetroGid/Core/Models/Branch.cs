namespace MetroGid.Core.Models
{
    public class Branch(string title, int color, AccessType type)
    {
        public string Title { get; set; } = title;
        public int Color { get; set; } = color;
        public AccessType Type { get; set; } = type;
        public List<Station> Stations { get; set; } = [];
        public Chart? Chart { get; set; }
    }
}