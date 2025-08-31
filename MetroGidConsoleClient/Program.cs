using System.Text;
using ConsoleClient.Commands.Concrete;
using ConsoleClient.Registrar;
using ConsoleClient.Services.Concrete;
using ConsoleClient.Services.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

static string[] ParseCommandLine(string line)
{
    var parts = new List<string>();
    var current = new StringBuilder();
    bool inQuotes = false;

    for (int i = 0; i < line.Length; i++)
    {
        char c = line[i];

        if (c == '"')
        {
            inQuotes = !inQuotes;
        }
        else if (c == ' ' && !inQuotes)
        {
            if (current.Length > 0)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
        }
        else
        {
            current.Append(c);
        }
    }

    if (current.Length > 0)
        parts.Add(current.ToString());

    return parts.ToArray();
}

string[] suggestions =
{
    "reg",
    "login",
    "logout",
    "unreg",
    "add_chart",
    "patch_branch",
    "patch_station",
    "patch_transition",
    "patch_railway",
    "get_chart_cities_titles",
    "get_chart_scheme",
    "search_route",
    "save_route",
    "get_route_titles",
    "get_saved_route",
    "delete_route",
    "help",
    "exit",
    "clear"
};

Table help_table = new Table()
    .RoundedBorder()
    .AddColumn("Команда")
    .AddColumn("Параметры")
    .AddColumn("Описание")
    .AddRow("help", "Нет обязательных параметров", "Показать эту таблицу")
    .AddEmptyRow()
    .AddRow("reg", "--login,-l <Логин> --password,-p <пароль> --mail,-m <почта>", "Регистрация аккаунта")
    .AddEmptyRow()
    .AddRow("login", "--login,-l <Логин> --password,-p <пароль>", "Войти в аккаунт")
    .AddEmptyRow()
    .AddRow("logout", "Нет обязательных параметров", "Выйти из аккаунта")
    .AddEmptyRow()
    .AddRow("unreg", "--password,-p <пароль>", "Удаление активного аккаунта")
    .AddEmptyRow()
    .AddRow("add_chart", "--chart,-c <json схема метро>", "Создать новую схему")
    .AddEmptyRow()
    .AddRow("patch_branch", "--city,-c <Город> --chart <Схема> --branch,-b <Ветка> --new-title <Новое название ветки> --new-color <Новый цвет> --new-access <Новый тип доступа>", "Обновить параметры ветки схемы")
    .AddEmptyRow()
    .AddRow("patch_station", "--city,-c <Город> --chart <Схема> --branch,-b <Ветка> --station,-s <Станция> --new-title <Новое название станции> --new-occupany <Новая загруженность> --new-access <Новый тип доступа> --new-opentime <Новое время открытия> --new-closetime <Новое врямя закрытия>", "Обновить параметры станции")
    .AddEmptyRow()
    .AddRow("patch_transition", "--city,-c <Город> --chart <Схема> --from-branch,-f-b <Ветка <C>> --from-station,-f-s <Станция <С>> --to-branch,-t-b <Ветка <На>> --to-station,-t-s <Станция <На>> --new-occupany <Новая загруженность> --new-access <Новый тип доступа> --new-duration <Новая средняя продолжительность> --new-opentime <Новое время открытия> --new-closetime <Новое врямя закрытия>", "Обновить параметры перехода")
    .AddEmptyRow()
    .AddRow("patch_railway", "--city,-c <Город> --chart <Схема> --branch,-b <Ветка> --duty-station,-d-s <Дежурная станция <С>> --to-station,-t-s <Станция <К>> --new-duration <Новая средняя продолжительность>", "Обновить параметры переезда")
    .AddEmptyRow()
    .AddRow("get_chart_cities_titles", "Нет обязательных параметров", "Получить все пары <Город, Схема>")
    .AddEmptyRow()
    .AddRow("get_chart_scheme", "--city,-c <Город> --chart <Схема>", "Получить SVG представление схемы")
    .AddEmptyRow()
    .AddRow("search_route", "--city,-c <Город> --chart <Схема> --from-branch <Ветка <С>> --from-station <Станция <C>> --to-branch <Ветка <На> --to-station <Станция <На> --time <Текущее время>>", "Поиск маршрута")
    .AddEmptyRow()
    .AddRow("save_route", "--route,-r <Json марщрут>", "Сохранить маршрут")
    .AddEmptyRow()
    .AddRow("get_route_titles", "--city,-c <Город> --chart <Схема>", "Получить названия сохраненных маршрутов")
    .AddEmptyRow()
    .AddRow("get_saved_route", "--city,-c <Город> --chart <Схема> --title,-t <Название маршрута>", "Получить сохраненный маршрут")
    .AddEmptyRow()
    .AddRow("delete_route", "--city,-c <Город> --chart <Схема> --title,-t <Название маршрута>", "Удалить сохраненный маршрут")
    .AddEmptyRow()
    .AddRow("exit | quit", "Нет обязательных параметров", "Выход из приложения");

TypeRegistrar registrar = new();

ApiService apiService = new(
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000")
    }
);

registrar.RegisterInstance<IApiService>(apiService);

registrar.RegisterInstance(
    typeof(IEnumerable<>).MakeGenericType(Type.GetType("Spectre.Console.Cli.Help.IHelpProvider, Spectre.Console.Cli")!),
    Array.CreateInstance(Type.GetType("Spectre.Console.Cli.Help.IHelpProvider, Spectre.Console.Cli")!, 0)
);

var commandInterceptors = Array.Empty<Spectre.Console.Cli.ICommandInterceptor>();
registrar.RegisterInstance(
    typeof(IEnumerable<Spectre.Console.Cli.ICommandInterceptor>),
    commandInterceptors
);

// И аналогично для IHelpProvider, если нужно
var helpProviders = Array.Empty<Spectre.Console.Cli.Help.IHelpProvider>();
registrar.RegisterInstance(
    typeof(IEnumerable<Spectre.Console.Cli.Help.IHelpProvider>),
    helpProviders
);

CommandApp app = new(registrar);

// Настраиваем команды
// Настраиваем команды
app.Configure(config =>
    {
        config.SetApplicationName("MetroGid");

        config.AddCommand<RegCommand>("reg")
            .WithDescription("Регистрация аккаунта");

        config.AddCommand<LoginCommand>("login")
            .WithDescription("Войти в аккаунт");

        config.AddCommand<LogoutCommand>("logout")
            .WithDescription("Выйти из аккаунта");

        config.AddCommand<UnRegCommand>("unreg")
            .WithDescription("Удаление аккаунта");

        config.AddCommand<PatchBranchCommand>("patch_branch")
            .WithDescription("Обновить параметры ветки");

        config.AddCommand<PatchStationCommand>("patch_station")
            .WithDescription("Обновить параметры станции");

        config.AddCommand<PatchTransitionCommand>("patch_transition")
            .WithDescription("Обновить параметры перехода");

        config.AddCommand<PatchRailwayCommand>("patch_railway")
            .WithDescription("Обновить параметры переезда");

        config.AddCommand<GetChartCitiesTitlesCommand>("get_chart_cities_titles")
            .WithDescription("Получить все пары <Город, Схема>");

        config.AddCommand<GetChartSchemeCommand>("get_chart_scheme")
            .WithDescription("Получить SVG схему");

        config.AddCommand<SearchRouteCommand>("search_route")
            .WithDescription("Поиск маршрута");

        config.AddCommand<SaveRouteCommand>("save_route")
            .WithDescription("Сохранение маршрута");

        config.AddCommand<GetRouteTitlesCommand>("get_route_titles")
            .WithDescription("Получить названия сохраненных маршрутов");

        config.AddCommand<GetSavedRouteCommand>("get_saved_route")
            .WithDescription("Получить сохраненный маршрут");

        config.AddCommand<DeleteRouteCommand>("delete_route")
            .WithDescription("Удалить сохраненный маршрут");
    }
);

while (true)
{
    AnsiConsole.Markup("[grey]MetroGid>[/] ");
    
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    var arguments = ParseCommandLine(input);
    var command = arguments[0].ToLowerInvariant();

    if (command == "help")
    {
        AnsiConsole.Write(help_table);
        continue;
    }

    if (command is "exit" or "quit")
            break;

    if (command == "clear")
    {
        AnsiConsole.Clear();
        continue;
    }

    if (!suggestions.Contains(command))
    {
        AnsiConsole.MarkupLine($"[red]Неизвестная команда: {input}[/]");
        continue;
    }

    await app.RunAsync(arguments);
}
