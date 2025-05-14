using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Services;

public class ChartService(IChartRepository chartRepo) : IChartService
{
    private readonly IChartRepository ChartRepo = chartRepo;



    public async Task<ChartDTO> GetChart(int chartId) =>
        DomainDtoConverter.Convert(await ChartRepo.GetChartWeakByIdAsync(chartId));

    public async Task<int> GetChartId(string city, string title) =>
        await ChartRepo.GetChartIdAsync(city, title);

    public async Task<List<ValueTuple<string, string>>> GetChartsCitiesTitles() =>
        await ChartRepo.GetAllChartCityTitleAsync();



    public async Task<BranchDTO> GetBranch(int branchId) =>
        DomainDtoConverter.Convert(await ChartRepo.GetBranchWeakByIdAsync(branchId));

    public async Task<int> GetChartBranchId(string title, int chartId) =>
        await ChartRepo.GetBranchIdAsync(title, chartId);

    public async Task<List<string>> GetChartBranchTitles(int chartId) =>
        await ChartRepo.GetAllChartBranchTitleAsync(chartId);



    public async Task<StationDTO> GetStation(int stationId) =>
        DomainDtoConverter.Convert(await ChartRepo.GetStationWeakByIdAsync(stationId));

    public async Task<int> GetBranchStationId(string title, int branchId) =>
        await ChartRepo.GetStationIdAsync(title, branchId);

    public async Task<List<string>> GetBranchStationTitles(int branchId) =>
        await ChartRepo.GetAllBranchStationTitleAsync(branchId);



    public async Task UpdateChart(int chartId, ChartDTO chart) =>
        await ChartRepo.UpdateChartByIdAsync(chartId, DtoDomainConverter.Convert(chart));

    public async Task UpdateBranch(int branchId, BranchDTO branch) =>
        await ChartRepo.UpdateBranchByIdAsync(branchId, DtoDomainConverter.Convert(branch));

    public async Task UpdateStation(int stationId, StationDTO station) =>
        await ChartRepo.UpdateStationByIdAsync(stationId, DtoDomainConverter.Convert(station));

    public async Task UpdateTransition(int transitionId, TransitionDTO transition) =>
        await ChartRepo.UpdateTransitionByIdAsync(transitionId, DtoDomainConverter.Convert(transition));
}