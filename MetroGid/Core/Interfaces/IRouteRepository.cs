using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;

namespace MetroGid.Core.Interfaces;

public interface IRouteRepository
{
    Task<MCMA.IdRow> AddAsync(int clientId, int chartId, MCMC.Route route);

    Task<MCMA.IdRow> GetIdAsync(string title, int clientId);
    
    Task<MCMC.Route?> GetByIdAsync(int id);

    Task<MCMA.TitlesRow> GetAllTitlesForClientAsync(int clientId);
    Task<MCMA.TitlesRow> GetAllTitlesForClientOfChartAsync(int clientId, int chartId);

    Task<MCMA.TitlesRow> GetAllForClientIdAsync(int clientId);
    Task<MCMA.TitlesRow> GetAllForClientOfChartIdAsync(int clientId, int chartId);

    Task<MCMC.Route?> GetChartRouteOfClient(int clientId, int chartId, string title);

    Task<MCMA.ChangedRowCount> UpdateAsync(int clientId, int chartId, MCMC.Route route);

    Task<MCMA.DeletedRowCount> DeleteAsync(int id);

    Task<bool> IsTitleExistsAsync(string title, int clientId);
}