namespace MetroGid.Controllers.DTO
{
    public class ChartDTO(string city, string title, string svg_inst)
    {
        public string City { get; set; } = city;
        public string Title { get; set; } = title;
        public string SvgInst { get; set; } = svg_inst;
    }
}