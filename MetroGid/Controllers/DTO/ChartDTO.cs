namespace MetroGid.Controllers.DTO;

public class ChartDTO(string title, string city, string svg_inst)
{
    public string Title { get; set; } = title;
    public string City { get; set; } = city;
    public string SvgInst { get; set; } = svg_inst;
}