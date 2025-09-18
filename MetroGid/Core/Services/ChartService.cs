using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Classification;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Exceptions.Truistic;

namespace MetroGid.Core.Services;

public class ChartService(
    IChartRepository chartRepo,
    SuperExceptionHandler handler, IExceptionVisitor? logger = null
) : IChartService
{
    private readonly IChartRepository ChartRepo = chartRepo;

    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    public async Task<MCMA.IdRow> AddChartAsync(string chartJson) =>
        await ChartRepo.AddAsync(chartJson);

    public async Task<MCMA.IdRow> GetChartIdAsync(string cityTitle, string chartTitle)
    {
        MCMA.IdRow idRow = await ChartRepo.GetChartIdAsync(cityTitle, chartTitle);

        if (idRow.id == 0)
            throw new UnknownChartCredentialsException(chartTitle, cityTitle);

        return idRow;
    }

    public async Task<MCMA.FileRow> GetChartSchemeAsync(string cityTitle, string chartTitle)
    {
        MCMA.IdRow chartIdRow = await GetChartIdAsync(cityTitle, chartTitle);
        return await ChartRepo.GetChartSchemeByIdAsync(chartIdRow.id) ?? throw new NotFoundByIdException("chart", chartIdRow.id);
    }

    private async Task<MCMA.IdRow> GetBranchIdAsync(string cityTitle, string chartTitle, string branchTitle)
    {
        MCMA.IdRow chartIdRow = await GetChartIdAsync(cityTitle, chartTitle);
        MCMA.IdRow branchIdRow = await ChartRepo.GetBranchIdAsync(branchTitle, chartIdRow.id);

        if (branchIdRow.id == 0)
            throw new UnknownBranchCredentialsException(chartTitle, cityTitle, branchTitle);

        return branchIdRow;
    }

    private async Task<MCMA.IdRow> GetStationIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle)
    {
        MCMA.IdRow branchIdRow = await GetBranchIdAsync(cityTitle, chartTitle, branchTitle);
        MCMA.IdRow stationIdRow = await ChartRepo.GetStationIdAsync(stationTitle, branchIdRow.id);

        if (stationIdRow.id == 0)
            throw new UnknownStationCredentialsException(stationTitle, branchIdRow.id);

        return stationIdRow;
    }

    private async Task<MCMA.IdRow> GetRailwayIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle,
        string fromStationTitle, string toStationTitle)
    {
        MCMA.IdRow fromStationIdRow = await GetStationIdAsync(
            cityTitle, chartTitle,
            branchTitle, fromStationTitle
        );

        MCMA.IdRow toStationIdRow = await GetStationIdAsync(
            cityTitle, chartTitle,
            branchTitle, toStationTitle
        );

        MCMA.IdRow railwayIdRow = await ChartRepo.GetRailwayIdAsync(fromStationIdRow.id, toStationIdRow.id);

        if (railwayIdRow.id == 0)
            throw new NotFoundByIdException("railway", railwayIdRow.id);

        return railwayIdRow;
    }

    private async Task<MCMA.IdRow> GetTransitionIdAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle)
    {
        MCMA.IdRow fromStationIdRow = await GetStationIdAsync(
            cityTitle, chartTitle,
            fromBranchTitle, fromStationTitle
        );

        MCMA.IdRow toStationIdRow = await GetStationIdAsync(
            cityTitle, chartTitle,
            toBranchTitle, toStationTitle
        );

        MCMA.IdRow transitionIdRow = await ChartRepo.GetTransitionIdAsync(fromStationIdRow.id, toStationIdRow.id);

        if (transitionIdRow.id == 0)
            throw new NotFoundByIdException("transition", transitionIdRow.id);

        return transitionIdRow;
    }

    public async Task<MCMA.ChartIdentifiers> GetChartsCitiesTitlesAsync() =>
        await ChartRepo.GetAllChartCityTitleAsync();

    public async Task<MCMA.IdRow> GetStationDutyIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle
    )
    {
        MCMA.IdRow chartIdRow = await ChartRepo.GetChartIdAsync(
            cityTitle,
            chartTitle
        );

        MCMA.IdRow branchIdRow = await ChartRepo.GetBranchIdAsync(
            branchTitle,
            chartIdRow.id
        );

        MCMA.IdRow stationIdRow = await ChartRepo.GetStationIdAsync(
            stationTitle,
            branchIdRow.id
        );

        return await ChartRepo.GetStationDutyIdAsync(stationIdRow.id) ?? new(0);
    }

    public async Task<MCMA.IdRow> GetTransitionDutyIdAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle
    )
    {
        MCMA.IdRow chartIdRow = await ChartRepo.GetChartIdAsync(
            cityTitle,
            chartTitle
        );

        MCMA.IdRow fromBranchIdRow = await ChartRepo.GetBranchIdAsync(
            fromBranchTitle,
            chartIdRow.id
        );

        MCMA.IdRow toBranchIdRow = await ChartRepo.GetBranchIdAsync(
            toBranchTitle,
            chartIdRow.id
        );

        MCMA.IdRow fromStationIdRow = await ChartRepo.GetStationIdAsync(
            fromStationTitle,
            fromBranchIdRow.id
        );

        MCMA.IdRow toStationIdRow = await ChartRepo.GetStationIdAsync(
            toStationTitle,
            toBranchIdRow.id
        );

        MCMA.IdRow transitionIdRow = await ChartRepo.GetTransitionIdAsync(
            fromStationIdRow.id,
            toStationIdRow.id
        );

        return await ChartRepo.GetTransitionDutyIdAsync(transitionIdRow.id) ?? new(0);
    }

    public async Task<MCMA.ChangedRowCount> UpdateChartSchemeAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string scheme
    )
    {
        if (role != MCMT.RoleType.DUTY)
            return new (0);

        MCMA.IdRow chartIdRow = await GetChartIdAsync(chartCity, chartTitle);
        
        return await ChartRepo.UpdateChartSchemeByIdAsync(
            chartIdRow.id,
            scheme
        );
    }

    public async Task<MCMA.ChangedRowCount> UpdateBranchAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string branchTitle,
        MCMC.Branch branch
    )
    {
        if (role != MCMT.RoleType.DUTY)
            return new (0);

        MCMA.IdRow branchIdRow = await GetBranchIdAsync(chartCity, chartTitle, branchTitle);

        return await ChartRepo.UpdateBranchByIdAsync(
            branchIdRow.id,
            branch
        );
    }

    public async Task<MCMA.ChangedRowCount> UpdateStationAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string branchTitle, string stationTitle,
        MCMC.Station station)
    {
        if (role != MCMT.RoleType.DUTY)
            return new (0);

        MCMA.IdRow stationIdRow = await GetStationIdAsync(
            chartCity, chartTitle,
            branchTitle, stationTitle
        );

        return await ChartRepo.UpdateStationByIdAsync(
            stationIdRow.id,
            station
        );
    }

    public async Task<MCMA.ChangedRowCount> UpdateRailwayAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string BranchTitle,
        string fromStationTitle, string toStationTitle,
        MCMC.Railway railway)
    {
        if (role != MCMT.RoleType.DUTY)
            return new (0);

        MCMA.IdRow railwayIdRow = await GetRailwayIdAsync(
            chartCity, chartTitle,
            BranchTitle,
            fromStationTitle, toStationTitle
        );

        return await ChartRepo.UpdateRailwayByIdAsync(
            railwayIdRow.id,
            railway
        );
    }

    public async Task<MCMA.ChangedRowCount> UpdateTransitionAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle,
        MCMC.Transition transition)
    {
        if (role != MCMT.RoleType.DUTY)
            return new (0);

        MCMA.IdRow transitionIdRow = await GetTransitionIdAsync(
            chartCity, chartTitle,
            fromBranchTitle, fromStationTitle,
            toBranchTitle, toStationTitle
        );

        return await ChartRepo.UpdateTransitionByIdAsync(
            transitionIdRow.id,
            transition
        );
    }
}