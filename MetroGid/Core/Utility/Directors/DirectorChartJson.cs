using System.Text.Json;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Builders;

namespace MetroGid.Core.Utility.Directors;

public class DirectorChartJson(BuilderChartBase builder, string jsonContent) : DirectorChartBase(builder)
{
    private readonly string JsonContent = jsonContent;
    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true // camelCase
    };

    public override Chart Construct()
    {
        ChartJsonDto chartDto = Deserialize(JsonContent);

        foreach (var branch in chartDto.Branches)
        {
            Builder.BuildBranch(
                branch.Title, ColorConverter.FromHex(branch.Color), AccessTypeConverter.FromString(branch.AccessType)
            );

            foreach (var station in branch.Stations)
            {
                Builder.BuildStation(
                    branch.Title, station.Title, station.Occupancy, AccessTypeConverter.FromString(station.AccessType),
                    TimeConverter.FromString(station.OpenTime), TimeConverter.FromString(station.CloseTime)
                );
            }

            foreach (var railway in branch.Railways)
            {
                Builder.BuildRailway(
                    branch.Title, railway.From, railway.To, TimeSpanConverter.FromString(railway.Duration)
                );
            }
        }

        foreach (var transition in chartDto.Transitions)
        {
            Builder.BuildTransition(
                transition.BranchSrc, transition.StationSrc,
                transition.BranchDst, transition.StationDst,
                transition.Occupancy, AccessTypeConverter.FromString(transition.AccessType),
                TimeSpanConverter.FromString(transition.Duration),
                TimeConverter.FromString(transition.OpenTime),
                TimeConverter.FromString(transition.CloseTime)
            );
        }

        Builder.BuildChart(chartDto.Title, chartDto.City, "");

        return Builder.GetResult();
    }

    private ChartJsonDto Deserialize(string jsonContent)
    {
        ChartJsonDto chartDto;

        try
        {
            chartDto = JsonSerializer.Deserialize<ChartJsonDto>(jsonContent, options) ??
                throw new JsonValidationException(
                    "Результат десериализации null",
                    ExceptionType.Error,
                    ExceptionReason.NullResult);
        }
        catch (JsonException ex)
        {
            throw new JsonDeserializeException(
                $"Ошибка формата JSON: {ex.Message}",
                ExceptionType.Error,
                ExceptionReason.FailedJsonDeserializing
            );
        }
        catch (ArgumentNullException)
        {
            throw new JsonValidationException(
                "Отсутствуют данные для десериализации (null)",
                ExceptionType.Error,
                ExceptionReason.NullArgument);
        }
        catch (NotSupportedException)
        {
            throw new JsonValidationException(
                "Неподдерживаемый формат данных в JSON",
                ExceptionType.Error,
                ExceptionReason.IncorrectJsonFormat);
        }

        return chartDto;
    }

    private record BranchJsonDto(
        string Title,
        string Color,
        string AccessType,
        List<StationJsonDto> Stations,
        List<RailwayJsonDto> Railways
    );

    private record StationJsonDto(
        string Title,
        int Occupancy,
        string AccessType,
        string OpenTime,
        string CloseTime
    );

    private record RailwayJsonDto(
        string From,
        string To,
        string Duration
    );

    private record TransitionJsonDto(
        int Occupancy,
        string AccessType,
        string Duration,
        string OpenTime,
        string CloseTime,
        string BranchSrc,
        string StationSrc,
        string BranchDst,
        string StationDst
    );

    private record ChartJsonDto(
        string Title,
        string City,
        List<BranchJsonDto> Branches,
        List<TransitionJsonDto> Transitions
    );
}