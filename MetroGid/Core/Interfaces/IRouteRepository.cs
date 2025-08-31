using MCMC = MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Interfaces;

public interface IRouteRepository
{
    Task<int> AddAsync(int clientId, int chartId, string routeJson);

    Task<int> GetIdAsync(string title, int clientId);
    
    Task<string?> GetByIdAsync(int id);

    Task<List<string>> GetAllTitlesForClientAsync(int clientId);
    Task<List<string>> GetAllTitlesForClientOfChartAsync(int clientId, int chartId);

    Task<List<string>> GetAllForClientIdAsync(int clientId);
    Task<List<string>> GetAllForClientOfChartIdAsync(int clientId, int chartId);

    Task<string?> GetChartRouteOfClient(int clientId, int chartId, string title);

    Task<int> UpdateAsync(int clientId, int chartId, MCMC.Route route);

    Task<int> DeleteAsync(int id);

    Task<bool> IsTitleExistsAsync(string title, int clientId);
}