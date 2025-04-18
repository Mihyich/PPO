using System.Net;

namespace MetroGid.Core.Exceptions
{
    public class NotFoundException(NotFoundException.ErrorType reason, string message) : DomainException(message, HttpStatusCode.NotFound)
    {
        public enum ErrorType
        {
            ClientLost, // Пользователь не найден
            ChartIdMisMatch, // Неверный айди Chart
            ChartCityTitleMisMatch, // Не существует схемы в таком городе с таким названием
            BranchIdMisMatch, // Неверный айди Branch
            BranchTitleChartIdMisMatch, // Не найдена ветка с таким названием в схеме с таким айди
            StationIdMisMatch, // Неверный айди Station
            StationTitleBranchIdMisMatch, // Не найдена станция с таким названием на ветка с таким айди
        }

        public ErrorType Reason { get; } = reason;
    }
}