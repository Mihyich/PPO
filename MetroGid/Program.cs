using MetroGid.Services.Models;
using MetroGid.Services.Utilities;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        BuilderChart builder = new();
        DirectorChartJson director = new(builder, "/home/mihail/Рабочий стол/BMSTU/PPO/temp_cities/Minsk/init.json");
        Chart? chart = director.Construct();

        if (chart != null)
        {
            chart.Searcher = new StrategySearchRouteBFS();
            Station? stationA = chart.GetStation("Московская линия", "Московская");
            Station? stationB = chart.GetStation("Зеленолужская линия", "Площадь Франтишка Богушевича");

            if (stationA != null && stationB != null)
            {
                Route? route = chart.Search(stationA, stationB);
                route?.Output();
            }
        }
    }
}