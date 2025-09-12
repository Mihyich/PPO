using MetroGid.Controllers.Utility.DTO.Concrete;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IChartService
{
    Task<MCMA.IdRow> AddChartAsync(string chartJson);

    Task<MCMA.IdRow> GetChartIdAsync(string cityTitle, string chartTitle);
    Task<MCMA.FileRow> GetChartSchemeAsync(string cityTitle, string chartTitle);
    Task<MCMA.ChartIdentifiers> GetChartsCitiesTitlesAsync();

    Task<MCMA.IdRow> GetStationDutyIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle
    );

    Task<MCMA.IdRow> GetTransitionDutyIdAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle
    );

    // Изменение атрибутов таблиц
    Task<MCMA.ChangedRowCount> UpdateChartSchemeAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string scheme
    );
    Task<MCMA.ChangedRowCount> UpdateBranchAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string branchTitle,
        MCMC.Branch branch
    );
    Task<MCMA.ChangedRowCount> UpdateStationAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string branchTitle, string stationTitle,
        MCMC.Station station
    );
    Task<MCMA.ChangedRowCount> UpdateRailwayAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string BranchTitle,
        string fromStationTitle, string toStationTitle,
        MCMC.Railway railway
    );
    Task<MCMA.ChangedRowCount> UpdateTransitionAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle,
        MCMC.Transition transition
    );
}