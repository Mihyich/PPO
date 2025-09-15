using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MetroGid.Controllers.Utility.DTO.Concrete;

[JsonDerivedType(typeof(RailwayConnectionDTO), "railway")]
[JsonDerivedType(typeof(TransitionConnectionDTO), "transition")]
// [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
public abstract record StationConnectionDTO;
public record RailwayConnectionDTO(RailwayDTO Railway) : StationConnectionDTO;
public record TransitionConnectionDTO(TransitionDTO Transition) : StationConnectionDTO;

[JsonDerivedType(typeof(RouteStationItemDTO), "station")]
[JsonDerivedType(typeof(RouteConnectionItemDTO), "connection")]
// [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
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
        FindFirstMismatchIndex(Path, other.Path) == null;

    public static int? FindFirstMismatchIndex<T>(IList<T> list1, IList<T> list2) where T : IEquatable<T>
    {
        if (list1 == null || list2 == null)
            return null;

        int minCount = Math.Min(list1.Count, list2.Count);

        for (int i = 0; i < minCount; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                return i; // ← возвращаем индекс первого несовпадения
        }

        // Если длины разные — возвращаем индекс первого "лишнего" элемента
        if (list1.Count != list2.Count)
            return minCount;

        return null; // всё совпало
    }
    
    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(Title);
        hash.Add(City);
        hash.Add(ChartTitle);
        hash.Add(Duration);
        foreach (var item in Path)
            hash.Add(item);
        return hash.ToHashCode();
    }
}