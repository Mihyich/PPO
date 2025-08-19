using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Super;

/*
Алгоритм Дейкстры: https://ru.wikipedia.org/wiki/%D0%90%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC_%D0%94%D0%B5%D0%B9%D0%BA%D1%81%D1%82%D1%80%D1%8B

Adj - словарь станций, для каждой из которых имеется список ближайших
маршрутов к ближайшим станциям, гарантированно состоящий из 3 элементов:
{ Станция --> Переезд/Переход --> Станция }
Время каждого такого маршрута предварительно вычисляется.

dist - динамически изменяемый словарь станций, которые содержат маршруты,
не только хранящие конечную стоимость пути (как это реализовано в стандартном алгоритме),
но и непосредственно сам способ - маршрут, описывающий, как добраться из станции src к текущей.
По завершении алгоритма, когда будут посещены все станции и цикл закончится, этот словарь
будет хранить наименьшие по времени маршруты от src до каждой из этих станций. То есть,
ответ будет dist[dst], где src - начальная станция, dst - конечная станция.
Замечание: изначально маршрута нет. Иными словами, время = бесконечность.

visited - хеш-список, хранящий посещенные узлы графа по стандартному алгоритму.

pq - приоритетная оцередь. Используется для корректной работы алгоритма. Продолжать поиск
новых маршрутов необходимо в определенном порядке - выбирать наикратчайший по затратам текущий
найденный маршрут. (В общем как это сделано в стандартном алгоритме)

Помимо проделанных нововведений, динамически контролируется время в движении:
каждый переход, станция могут закрыться в определенный момент времени. Таким образом,
требуется знать не только начальную и конечную станции, но и время отправки - timeStart.
*/

namespace MetroGid.Core.Utilities.Strategies;

public class StrategySearchRouteDijkstra(TimeSuper? timerSuper = null) : StrategySearchRouteBase(timerSuper ?? new TimeFast())
{
    public override Route? Search(Chart chart, Station src, Station dst, TimeOnly timeStart)
    {
        if ((!src.Branch?.IsAccessible() ?? true) || !src.IsAccessible() || !src.IsOpenAt(timeStart) ||
            (!dst.Branch?.IsAccessible() ?? true) || !dst.IsAccessible())
            return null;

        Dictionary<Station, List<Route>> Adj = GenAdj(chart.Branches); // Аналог матрицы смежностей
        Dictionary<Station, Route> dist = GenDist(chart.Branches); // Поиск маршрутов к каждому из узлов графа
        HashSet<Station> visited = []; // Посещенные станции
        PriorityQueue<Route, TimeSpan> pq = new(); // Приоритетная очередь по времени маршрутов
        Station? lstation;

        // Создание отправной точки
        Route route = new Route(string.Empty, [], TimeSpan.Zero, ts).Append(src);

        // По умолчанию минимальный способ добраться до src это route.
        dist[src] = route;

        // Обновить очередь
        pq.Enqueue(route, route.Duration);

        // Основной цикл
        while (pq.Count > 0)
        {
            route = pq.Dequeue();
            lstation = route.GetLastStation();

            if (lstation == null || route.Duration > dist[lstation].Duration)
                continue;

            foreach (Route r in Adj[lstation])
            {
                Station? neighbor = r.GetLastStation();
                TimeSpan newTime = dist[lstation].PredictDurationAfterAddAsOrphan(r);
                Route newRoute;

                if (neighbor != null &&
                    neighbor.IsAccessible() &&
                    (neighbor.Branch?.IsAccessible() ?? false) &&
                    dist[neighbor].Duration > newTime &&
                    !visited.Contains(neighbor))
                {
                    newRoute = route.SemiShallowClone().AppendAsOrphan(r);

                    dist[neighbor] = newRoute;
                    pq.Enqueue(newRoute, newRoute.Duration);
                }
            }

            visited.Add(lstation);
        }

        return dist[dst].Duration == TimeSpan.MaxValue ? null : dist[dst];
    }

    private Dictionary<Station, List<Route>> GenAdj(List<Branch> branches)
    {
        Dictionary<Station, List<Route>> Adj = [];

        foreach (var branch in branches)
        {
            foreach (var station in branch.Stations)
            {
                Route route;
                List<Route> routes = [];

                Station? neighbor;
                Railway? railPrev = station.Prev;
                Railway? railNext = station.Next;

                if (railPrev != null && (neighbor = railPrev.Prev) != null)
                {
                    route = new Route(string.Empty, [], TimeSpan.Zero, ts).Append(station).Append(railPrev).Append(neighbor);
                    routes.Add(route);
                }

                if (railNext != null && (neighbor = railNext.Next) != null)
                {
                    route = new Route(string.Empty, [], TimeSpan.Zero, ts).Append(station).Append(railNext).Append(neighbor);
                    routes.Add(route);
                }

                foreach (var transition in station.Transitions)
                {
                    if ((neighbor = transition.ToFrom(station)) != null)
                    {
                        route = new Route(string.Empty, [], TimeSpan.Zero, ts).Append(station).Append(transition).Append(neighbor);
                        routes.Add(route);
                    }
                }

                Adj[station] = routes;
            }
        }

        return Adj;
    }

    private Dictionary<Station, Route> GenDist(List<Branch> branches)
    {
        Dictionary<Station, Route> dist = [];

        foreach (var branch in branches)
            foreach (var station in branch.Stations)
                dist[station] = new Route(string.Empty, [], TimeSpan.MaxValue, ts);

        return dist;
    }
}