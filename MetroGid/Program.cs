using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities;
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
        DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO/temp_cities/Sankt-Peterburg/init.json"));
        Chart chart = director.Construct();

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