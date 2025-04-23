using System.Text.Json;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Builders;

namespace MetroGid.Core.Utilities.Directors
{
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
                        branch.Title, station.Title, station.Occupancy, AccessTypeConverter.FromString(station.AccesType),
                        TimeConverter.FromString(station.OpenTime), TimeConverter.FromString(station.CloseTime)
                    );
                }

                foreach (var railway in branch.Railways)
                {
                    Builder.BuildRailway(
                        branch.Title, railway.From, railway.To, TimeConverter.FromString(railway.Duration)
                    );
                }
            }

            foreach (var transition in chartDto.Transitions)
            {
                Builder.BuildTransition(
                    transition.BranchSrc, transition.StationSrc,
                    transition.BranchDst, transition.StationDst,
                    transition.Occupancy, AccessTypeConverter.FromString(transition.AccessType),
                    TimeConverter.FromString(transition.Duration),
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

        private class BranchJsonDto
        {
            public string Title { get; set; } = string.Empty;
            public string Color { get; set; } = string.Empty;
            public string AccessType { get; set; } = string.Empty;
            public List<StationJsonDto> Stations { get; set; } = [];
            public List<RailwayJsonDto> Railways { get; set; } = [];
        }

        private class StationJsonDto
        {
            public string Title { get; set; } = string.Empty;
            public int Occupancy { get; set; }
            public string AccesType { get; set; } = string.Empty;
            public string OpenTime { get; set; } = string.Empty;
            public string CloseTime { get; set; } = string.Empty;
        }

        private class RailwayJsonDto
        {
            public string From { get; set; } = string.Empty;
            public string To { get; set; } = string.Empty;
            public string Duration { get; set; } = string.Empty;
        }

        private class TransitionJsonDto
        {
            public int Occupancy { get; set; }
            public string AccessType { get; set; } = string.Empty;
            public string Duration { get; set; } = string.Empty;
            public string OpenTime { get; set; } = string.Empty;
            public string CloseTime { get; set; } = string.Empty;
            public string BranchSrc { get; set; } = string.Empty;
            public string StationSrc { get; set; } = string.Empty;
            public string BranchDst { get; set; } = string.Empty;
            public string StationDst { get; set; } = string.Empty;
        }

        private class ChartJsonDto
        {
            public string Title { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public List<BranchJsonDto> Branches { get; set; } = [];
            public List<TransitionJsonDto> Transitions { get; set; } = [];
        }
    }
}