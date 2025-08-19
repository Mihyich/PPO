using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Interfaces;

public interface IRouteRepository
{
    Task<int> AddAsync(int clientId, int chartId, Route route);

    Task<int> GetIdAsync(string title, int clientId);
    
    Task<Route?> GetByIdAsync(int id);

    Task<List<string>> GetAllTitlesForClientAsync(int clientId);
    Task<List<string>> GetAllTitlesForClientOfChartAsync(int clientId, int chartId);

    Task<List<Route>> GetAllForClientIdAsync(int clientId);
    Task<List<Route>> GetAllForClientOfChartIdAsync(int clientId, int chartId);

    Task<int> UpdateAsync(int clientId, int chartId, Route route);

    Task<int> DeleteAsync(int id);

    Task<bool> IsTitleExistsAsync(string title, int clientId);
}