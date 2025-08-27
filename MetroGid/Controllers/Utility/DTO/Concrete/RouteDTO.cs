namespace MetroGid.Controllers.Utility.DTO;

public abstract record StationConnectionDTO;
public record RailwayConnectionDTO(RailwayDTO Railway) : StationConnectionDTO;
public record TransitionConnectionDTO(TransitionDTO Transition) : StationConnectionDTO;

public abstract record RouteItemDTO;
public record RouteStationItemDTO(StationDTO Station) : RouteItemDTO;
public record RouteConnectionItemDTO(StationConnectionDTO Connection) : RouteItemDTO;

public record RouteDTO(
    string Title,
    string City,
    string ChartTitle,
    List<RouteItemDTO> Path,
    TimeSpan Duration
)
{
    public virtual bool Equals(RouteDTO? other) =>
        other != null &&
        Title == other.Title &&
        City == other.City &&
        ChartTitle == other.ChartTitle &&
        Duration == other.Duration &&
        Path.SequenceEqual(other.Path);
    
    public override int GetHashCode() =>
        HashCode.Combine(Title, City, ChartTitle, Path, Duration);
}