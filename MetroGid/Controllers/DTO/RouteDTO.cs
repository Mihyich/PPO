namespace MetroGid.Controllers.DTO;

public abstract record StationConnectionDTO;
public record RailwayConnectionDTO(RailwayDTO Railway) : StationConnectionDTO;
public record TransitionConnectionDTO(TransitionDTO Transition) : StationConnectionDTO;

public abstract record RouteItemDTO;
public record RouteStationItemDTO(StationDTO Station) : RouteItemDTO;
public record RouteConnectionItemDTO(StationConnectionDTO Connection) : RouteItemDTO;

public class RouteDTO(string title, string city, string chartTitle)
{
    public string Title { get; set; } = title;
    public string City { get; set; } = city;
    public string ChartTitle { get; set; } = chartTitle;

    public List<RouteItemDTO> Path = [];
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;

    public override bool Equals(object? obj)
    {
        if (obj is RouteDTO other &&
            Title == other.Title &&
            City == other.City &&
            ChartTitle == other.ChartTitle &&
            Duration == other.Duration &&
            Path.Count == other.Path.Count)
        {
            for (int i = 0; i < Path.Count; ++i)
                if (!Path[i].Equals(other.Path[i]))
                    return false;

            return true;
        }
        
        return false;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Title, City, ChartTitle, Path, Duration);
}