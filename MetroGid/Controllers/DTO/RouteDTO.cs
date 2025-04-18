namespace MetroGid.Controllers.DTO
{
    public abstract record StationConnectionDTO;
    public record RailwayConnectionDTO(RailwayDTO Railway) : StationConnectionDTO;
    public record TransitionConnectionDTO(TransitionDTO Transition) : StationConnectionDTO;

    public abstract record RouteItemDTO;
    public record RouteStationItemDTO(StationDTO Station) : RouteItemDTO;
    public record RouteConnectionItemDTO(StationConnectionDTO Connection) : RouteItemDTO;

    public class RouteDTO
    {
        public string Title { get; set; } = string.Empty;
        public List<RouteItemDTO> Path = [];
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public ChartDTO? chart;
    }
}