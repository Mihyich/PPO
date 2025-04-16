using System.Text.Json;
using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public class DirectorChartJson(BuilderChartBase builder, string file_path) : DirectorChartBase(builder)
    {
        private string FilePath = file_path;
        private readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true // camelCase
        };

        public override Chart? Construct()
        {
            ChartJsonDto chartDto = Deserialize(ReadJson());

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

        private string ReadJson()
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException($"JSON файл не найден: {FilePath}");
            }

            return File.ReadAllText(FilePath);
        }

        private ChartJsonDto Deserialize(string jsonContent)
        {
            ChartJsonDto? chartDto;
            try
            {
                chartDto = JsonSerializer.Deserialize<ChartJsonDto>(jsonContent, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Ошибка при парсинге JSON", ex);
            }

            if (chartDto == null)
            {
                throw new InvalidOperationException("Не удалось десериализовать JSON");
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