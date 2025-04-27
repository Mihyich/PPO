using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

class Program
{
    static void Main(string[] args)
    {
        SuperHandlerException handler = new WarningHandlerException();
        IExceptionVisitor logger = new ExceptionMessenger();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler, logger);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler, logger);

        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        // DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO/temp_cities/Sankt-Peterburg/init.json"));
        DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO/cities/Sankt-Peterburg/chart.json"));
        Chart chart = director.Construct();

        chart.Searcher = new StrategySearchRouteBFS();
        // chart.Searcher = new StrategySearchRouteDijkstra();
        // Station? stationA = chart.GetStation("Арбатско-Покровская линия", "Щёлковская");
        // Station? stationB = chart.GetStation("Солнцевская линия", "Аэропорт Внуково");
        // Station? stationA = chart.GetStation("Линия 1", "Больница");
        // Station? stationB = chart.GetStation("Линия 1", "Акынджилар");
        Station? stationA = chart.GetStation("Кировско-Выборгская", "Проспект Ветеранов");
        Station? stationB = chart.GetStation("Правобережная", "Улица Дыбенко");
        TimeOnly timeStart = new(18, 30);

        if (stationA != null && stationB != null)
        {
            Route? route = chart.Search(stationA, stationB, timeStart);
            route?.Output();
        }
        else
        {
            Console.WriteLine($"Не все целевые станции были найдены");
        }
    }
}