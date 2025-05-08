namespace MetroGid.Controllers.DTO;

public class ChartDTO(string title, string city, string svg_inst)
{
    public string Title { get; } = title;
    public string City { get; } = city;
    public string SvgInst { get; } = svg_inst;
}