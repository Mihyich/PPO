namespace MetroGid.Services.Models
{
    public class Chart
    {
        public string Title {get; set;} = string.Empty;
        public string City {get; set;} = string.Empty;
        public string SvgInst {get; set;} = string.Empty;
        public List<Branch> Branches {get; set;} = [];
    }
}