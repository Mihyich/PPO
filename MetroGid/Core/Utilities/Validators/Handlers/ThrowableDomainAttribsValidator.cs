using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Validators.Interfaces;
using MetroGid.Core.Utilities.Validators.Predicators;

namespace MetroGid.Core.Utilities.Validators.Handlers
{
    public class ThrowableDomainAttribsValidator : IDomainValidatorVisitor
    {
        private readonly WarningHandlerException handler = new();

        public void Visit(Chart chart)
        {
            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsEmpty(chart.Title);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            "Наименование схемы пустое",
                            ExceptionType.Warning,
                            ExceptionReason.EmptyString
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsOutOfRange(chart.Title);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Наименование схемы превышает допустимый предел длины: {chart.Title.Length} > {TitleP.MaxLength}",
                            ExceptionType.Warning,
                            ExceptionReason.StringLenghtOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsEmpty(chart.City);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            "Наименование города схемы пустое",
                            ExceptionType.Warning,
                            ExceptionReason.EmptyString
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsOutOfRange(chart.City);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Наименование города схемы превышает допустимый предел длины: {chart.City.Length} > {TitleP.MaxLength}",
                            ExceptionType.Warning,
                            ExceptionReason.StringLenghtOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            // Что делать с svgInst???
        }
        
        public void Visit(Branch branch)
        {
            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsEmpty(branch.Title);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            "Наименование ветки пустое",
                            ExceptionType.Warning,
                            ExceptionReason.EmptyString
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsOutOfRange(branch.Title);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Наименование ветки превышает допустимый предел длины: {branch.Title.Length} > {TitleP.MaxLength}",
                            ExceptionType.Warning,
                            ExceptionReason.StringLenghtOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = ColorP.IsOutOfRange(branch.Color);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"10-ое представление значения цвета ветки {branch.Color} не принадлежит отрезку: [0, {ColorP.MaxValue}]",
                            ExceptionType.Warning,
                            ExceptionReason.ValueOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = AccessTypeP.IsOutOfRange((int)branch.Type);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Тип доступа ветки {(int)branch.Type} не принадлежит перечислению AccessType",
                            ExceptionType.Warning,
                            ExceptionReason.ValueOutOfRange
                        );
                    }

                    return thrown;
                }
            );
        }

        public void Visit(Station station)
        {
            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsEmpty(station.Title);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            "Наименование станции пустое",
                            ExceptionType.Warning,
                            ExceptionReason.EmptyString
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = TitleP.IsOutOfRange(station.Title);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Наименование станции превышает допустимый предел длины: {station.Title.Length} > {TitleP.MaxLength}",
                            ExceptionType.Warning,
                            ExceptionReason.StringLenghtOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = OccupancyP.IsOutOfRange(station.Occupancy);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Уровень загруженности станции {station.Occupancy} не принадлежит отрезку: [{OccupancyP.MinValue}, {OccupancyP.MaxValue}]",
                            ExceptionType.Warning,
                            ExceptionReason.ValueOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = AccessTypeP.IsOutOfRange((int)station.Type);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Тип доступа станции {(int)station.Type} не принадлежит перечислению AccessType",
                            ExceptionType.Warning,
                            ExceptionReason.ValueOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = station.OpenTime == station.CloseTime;

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Время открытия и закрытия станции совпадают: {station.OpenTime}",
                            ExceptionType.Warning,
                            ExceptionReason.NotLogicValue
                        );
                    }

                    return thrown;
                }
            );
        }

        public void Visit(Railway railway)
        {
            // Вроде пока нечего проверять, но пусть будет...
        }

        public void Visit(Transition transition)
        {
            handler.Snap(
                () =>
                {
                    bool thrown = OccupancyP.IsOutOfRange(transition.Occupancy);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Уровень загруженности перехода {transition.Occupancy} не принадлежит отрезку: [{OccupancyP.MinValue}, {OccupancyP.MaxValue}]",
                            ExceptionType.Warning,
                            ExceptionReason.ValueOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = AccessTypeP.IsOutOfRange((int)transition.Type);

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Тип доступа перехода {(int)transition.Type} не принадлежит перечислению AccessType",
                            ExceptionType.Warning,
                            ExceptionReason.ValueOutOfRange
                        );
                    }

                    return thrown;
                }
            );

            handler.Snap(
                () =>
                {
                    bool thrown = transition.OpenTime == transition.CloseTime;

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Время открытия и закрытия перехода совпадают: {transition.OpenTime}",
                            ExceptionType.Warning,
                            ExceptionReason.NotLogicValue
                        );
                    }

                    return thrown;
                }
            );
        }
    }
}