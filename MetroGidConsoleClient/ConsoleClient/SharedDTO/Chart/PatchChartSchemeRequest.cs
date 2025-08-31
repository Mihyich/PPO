namespace ConsoleClient.SharedDTO.Chart;

public record PatchChartSchemeRequest(
    string CityTitle,
    string ChartTitle,
    string Scheme
);