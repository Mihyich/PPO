using System.Net;

namespace MetroGid.Core.Exceptions
{
    public class NotFoundException(NotFoundException.ErrorType reason, string message) : DomainException(message, HttpStatusCode.NotFound)
    {
        public enum ErrorType
        {
            ClientLost, // Пользователь не найден
            ChartIdMisMatch, // Неверный айди Chart
            BranchIdMisMatch, // Неверный айди Branch
            StationIdMisMatch, // Неверный айди Station
            TransitionIdMisMatch, // Неверный айди Transition
            ChartCityTitleMisMatch, // Не существует схемы в таком городе с таким названием
            BranchTitleChartIdMisMatch, // Не найдена ветка с таким названием в схеме с таким айди
            StationTitleBranchIdMisMatch // Не найдена станция с таким названием на ветка с таким айди
        }

        public ErrorType Reason { get; } = reason;
    }
}