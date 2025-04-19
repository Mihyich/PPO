using MetroGid.Core.Models;
using MetroGid.Core.Utilities;

class Program
{
    static void Main(string[] args)
    {
        BuilderChart builder = new();
        // DirectorChartJson director = new(builder, "/home/mihail/Рабочий стол/BMSTU/PPO/temp_cities/Minsk/init.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO/temp_cities/Sankt-Peterburg/init.json"));
        Chart? chart = director.Construct();

        if (chart != null)
        {
            // chart.Searcher = new StrategySearchRouteBFS();
            chart.Searcher = new StrategySearchRouteDijkstra();
            Station? stationA = chart.GetStation("Невско-Василеостровская", "Василеостровская");
            Station? stationB = chart.GetStation("Фрунзенско-Приморская", "Бухарестская");

            if (stationA != null && stationB != null)
            {
                Route? route = chart.Search(stationA, stationB);
                route?.Output();
            }
        }
    }
}