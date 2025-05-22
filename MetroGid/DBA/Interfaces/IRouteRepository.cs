using MetroGid.Core.Models.Concrete;

namespace MetroGid.DBA.Interfaces;

public interface IRouteRepository
{
    Task<int> AddAsync(Route route, int clientId, int chartId);

    Task<int> GetIdAsync(string title, int clientId);
    Task<Route> GetByIdAsync(int id);
    Task<List<string>> GetAllTitlesForClientAsync(int clientId);
    Task<List<string>> GetAllTitlesForClientOfChartAsync(int clientId, int chartId);
    Task<List<Route>> GetAllForClientIdAsync(int clientId);
    Task<List<Route>> GetAllForClientOfChartIdAsync(int clientId, int chartId);

    Task UpdateAsync(int id, Route route);

    Task DeleteAsync(int id);

    Task<bool> IsTitleExistsAsync(string title, int clientId);
}