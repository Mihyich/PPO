namespace MetroGid.Controllers.DTO;

public class StationDTO(string title, int occupancy, AccessTypeDTO type, TimeOnly open_time, TimeOnly close_time)
{
    public string Title { get; set; } = title;
    public int Occupancy { get; set; } = occupancy;
    public AccessTypeDTO Type { get; set; } = type;
    public TimeOnly OpenTime { get; set; } = open_time;
    public TimeOnly CloseTime { get; set; } = close_time;
}