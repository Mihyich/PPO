using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

using MetroGid.Core.Services;
using MetroGid.Core.Interfaces;
using MetroGid.DBA.EF.Repositories;
using MetroGid.DBA.EF.Context;
using MetroGid.DBA.EF.Converters;

class Program
{
    static void Main(string[] args)
    {
        SuperExceptionHandler handler = new WarningHandlerException();
        IExceptionVisitor logger = new ExceptionMessenger();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler, logger);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler, logger);

        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        // DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO_BACKUP/cities/Adana/chart.json"));
        // DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO/temp_cities/Sankt-Peterburg/init.json"));
        DirectorChartJson director = new(builder, FileReader.ReadAll("/home/mihail/Рабочий стол/BMSTU/PPO_BACKUP/cities/Moscow/chart.json"));
        Chart chart = director.Construct();

        // chart.Searcher = new StrategySearchRouteDijkstra();
        // Station? stationA = chart.GetStation("Арбатско-Покровская линия", "Щёлковская");
        // Station? stationB = chart.GetStation("Солнцевская линия", "Аэропорт Внуково");
        // Station? stationA = chart.GetStation("Линия 1", "Больница");
        // Station? stationB = chart.GetStation("Линия 1", "Акынджилар");
        Station? stationA = chart.GetStation("МЦД-2", "Нахабино");
        Station? stationB = chart.GetStation("Замоскворецкая линия", "Алма-Атинская");
        TimeOnly timeStart = new(18, 30);

        if (stationA != null && stationB != null)
        {
            Route? route = chart.Search(stationA, stationB, timeStart, new StrategySearchRouteBFS());
            route?.Validate(domainAttribsValidator);
            route?.Validate(domainReferentialityValidator);
        }
        else
        {
            Console.WriteLine($"Не все целевые станции были найдены");
        }



        // MetroContext metroContext = new();
        // IClientRepository clientRepository = new EFClientRepository(metroContext);
        // ClientService clientService = new(clientRepository, handler, logger);

        // int clientId1 = clientService.Reg("mihail", "qwerASDF1234", "michail.zevahin@gmail.com").GetAwaiter().GetResult();
        // Console.WriteLine($"Айди нового пользователя {clientId1}");

        // int clientId2 = clientService.Reg("GrizlyBear", "GrizlyBearGoyda2004", "berloga.taiga@gmail.com").GetAwaiter().GetResult();
        // Console.WriteLine($"Айди нового пользователя {clientId2}");

        // int clientId1 = 1;
        // int clientId2 = 2;
        // int stationId = 1;
        // int transitionId = 1;

        // int dutyId = clientRepository.GetStationDuty(stationId).GetAwaiter().GetResult();
        // Console.WriteLine($"Айди дежурного у станции {stationId}: {dutyId}");

        // int changes = clientRepository.MakeDuty(clientId1, stationId, transitionId).GetAwaiter().GetResult();
        // Console.WriteLine($"Проделанные изменения: {changes}");

        // dutyId = clientRepository.GetStationDuty(stationId).GetAwaiter().GetResult();
        // Console.WriteLine($"Айди дежурного у станции {stationId}: {dutyId}");
    }
}