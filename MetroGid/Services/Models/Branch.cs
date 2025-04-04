namespace MetroGid.Services.Models
{
    public class Branch
    {
        public string Title {get; set;} = string.Empty;
        public int Color {get; set;} = 0; // Черный
        public AccessType Type {get; set;} = AccessType.ACCESSIBLE;
        public List<Station> Stations {get; set;} = [];
        public Chart? Chart {get; set;}
    }
}