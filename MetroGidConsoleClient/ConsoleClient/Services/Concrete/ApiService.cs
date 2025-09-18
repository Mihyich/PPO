using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO;
using ConsoleClient.SharedDTO.Auth;
using ConsoleClient.SharedDTO.Chart;
using ConsoleClient.SharedDTO.Concrete;
using ConsoleClient.SharedDTO.Route;
using MetroGid.Controllers.Utility.DTO.Concrete;

namespace ConsoleClient.Services.Concrete;

public class ApiService(
    HttpClient httpClient
) : IApiService
{
    private readonly HttpClient _httpClient = httpClient;
    private string? _token;

    private async Task PucPucAsync(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        throw new HttpRequestException($"Ошибка {response.StatusCode}: {errorResponse?.Message ?? "Неизвестная ошибка"}");
    }

    public async Task<AuthResponseDTO?> RegAsync(string login, string password, string mail)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequestDTO(login, password, mail));

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
    }

    public async Task<AuthResponseDTO?> LogInAsync(string login, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequestDTO(login, password));

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

        if (result != null)
        {
            _token = result.Token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        return result;
    }

    public async Task LogOutAsync()
    {
        var response = await _httpClient.PostAsync("api/auth/logout", null);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        if (_token != null)
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task UnRegAsync(string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "api/auth/unregister")
        {
            Content = JsonContent.Create(new UnregisterRequestDto(password))
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        if (_token != null)
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task AddChart(
        string ChartJson
    )
    {
        var response = await _httpClient.PostAsJsonAsync("api/chart/add", new AddChartRequest(ChartJson));

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);
    }

    public async Task PatchChartSceme(
        string CityTitle,
        string ChartTitle,
        string Scheme
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "api/chart/scheme")
        {
            Content = JsonContent.Create(new PatchChartSchemeRequest(CityTitle, ChartTitle, Scheme))
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);
    }

    public async Task PatchBranch(
        string CityTitle,
        string ChartTitle,
        string BranchTitle,
        string NewTitle,
        string NewColor, // hex формат
        string NewAccess
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "api/chart/branch")
        {
            Content = JsonContent.Create(
                new PatchBranchRequest(
                    CityTitle,
                    ChartTitle,
                    BranchTitle,
                    NewTitle,
                    NewColor,
                    NewAccess
                )
            )
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);
    }

    public async Task PatchStation(
        string CityTitle,
        string ChartTitle,
        string BranchTitle,
        string StationTitle,
        string NewTitle,
        int NewOccupancy,
        string NewAccess,
        string NewOpenTime,
        string NewCloseTime
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "api/chart/station")
        {
            Content = JsonContent.Create(
                new PatchStationRequest(
                    CityTitle,
                    ChartTitle,
                    BranchTitle,
                    StationTitle,
                    NewTitle,
                    NewOccupancy,
                    NewAccess,
                    NewOpenTime,
                    NewCloseTime
                )
            )
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);
    }

    public async Task PatchTransition(
        string CityTitle,
        string ChartTitle,
        string FromBranchTitle,
        string FromStationTitle,
        string ToBranchTitle,
        string ToStationTitle,
        int NewOccupancy,
        string NewAccess,
        string NewDuration,
        string NewOpenTime,
        string NewCloseTime
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "api/chart/transition")
        {
            Content = JsonContent.Create(
                new PatchTransitionRequest(
                    CityTitle,
                    ChartTitle,
                    FromBranchTitle,
                    FromStationTitle,
                    ToBranchTitle,
                    ToStationTitle,
                    NewOccupancy,
                    NewAccess,
                    NewDuration,
                    NewOpenTime,
                    NewCloseTime
                )
            )
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);
    }

    public async Task PatchRailway(
        string CityTitle,
        string ChartTitle,

        string BranchTitle,
        string DutyStationTitle,
        string ToStationTitle,

        string NewDuration
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "api/chart/railway")
        {
            Content = JsonContent.Create(
                new PatchRailwayRequest(
                    CityTitle,
                    ChartTitle,
                    BranchTitle,
                    DutyStationTitle,
                    ToStationTitle,
                    NewDuration
                )
            )
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);
    }

    public async Task<List<ChartDTO>> GetChartsCitiesTitles()
    {
        var response = await _httpClient.PostAsync("api/chart/cities_titles", null);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<List<ChartDTO>>() ?? new List<ChartDTO>();
    }

    public async Task<string?> GetScheme(
        string CityTitle,
        string ChartTitle
    )
    {
        var response = await _httpClient.PostAsJsonAsync("api/chart/scheme", new GetChartSchemeRequest(CityTitle, ChartTitle));

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        var dto = await response.Content.ReadFromJsonAsync<GetChartSchemeResponse>();
        return dto.content;
    }

    public async Task<RouteDTO?> SearchRoute(
        string CityTitle,
        string ChartTitle,
        string FromBranchTitle,
        string FromStationTitle,
        string ToBranchTitle,
        string ToStationTitle,
        string curTime
    )
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/routes/search",
            new SearchRouteRequest(
                CityTitle, ChartTitle,
                FromBranchTitle, FromStationTitle,
                ToBranchTitle, ToStationTitle,
                curTime
            )
        );

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<RouteDTO>();
    }

    public async Task<int> SaveRoute(
        string routeJson
    )
    {
        var content = new StringContent(routeJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/routes/save", content);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<List<string>> GetRouteTitles(
        string CityTitle,
        string ChartTitle
    )
    {
        var response = await _httpClient.PostAsJsonAsync("api/routes/get/titles", new GetRouteCredentialsRequest(CityTitle, ChartTitle));

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
    }

    public async Task<RouteDTO?> GetSavedRoute(
        string CityTitle,
        string ChartTitle,
        string RouteTitle
    )
    {
        var response = await _httpClient.PostAsJsonAsync("api/routes/get/saved", new GetSavedRouteRequest(CityTitle, ChartTitle, RouteTitle));

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<RouteDTO>();
    }

    public async Task<int> DeleteRoute(
        string CityTitle,
        string ChartTitle,
        string RouteTitle
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "api/routes/delete")
        {
            Content = JsonContent.Create(new DeleteRouteRequest(CityTitle, ChartTitle, RouteTitle))
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            await PucPucAsync(response);

        return await response.Content.ReadFromJsonAsync<int>();
    }
}