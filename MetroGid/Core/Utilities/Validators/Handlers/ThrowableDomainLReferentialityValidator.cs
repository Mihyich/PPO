using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Utilities.Validators.Handlers
{
    public class ThrowableDomainReferentialityValidator(
        SuperHandlerException handler,
        IExceptionVisitor? logger = null
    ) : IDomainValidatorVisitor
    {
        private readonly SuperHandlerException Handler = handler;
        private readonly IExceptionVisitor? Logger = logger;

        public void Visit(Chart chart)
        {
            Handler.Snap(
                () =>
                {
                    bool thrown = chart.Branches.Capacity == 0;

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Схема '{chart.Title}' в городе '{chart.City}' не имеет ни одной ветви",
                            ExceptionType.Warning,
                            ExceptionReason.NotFound
                        );
                    }

                    return thrown;
                }, Logger
            );

            Handler.Snap(
                () =>
                {
                    bool thrown = false;
                    Chart? chartHead;

                    if (chart.Branches.Capacity > 0 && (chartHead = chart.Branches[0].Chart) != null)
                    {
                        foreach (Branch branch in chart.Branches)
                        {
                            if (branch.Chart == null)
                            {
                                thrown = true;

                                throw new DomainValidationException(
                                    $"Ветка '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' не привязана к родной схеме",
                                    ExceptionType.Error,
                                    ExceptionReason.NullArgument
                                );
                            }

                            if (branch.Chart != chartHead)
                            {
                                thrown = true;

                                throw new DomainValidationException(
                                    $"Ветка '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' ссылается не на родную схему",
                                    ExceptionType.Error,
                                    ExceptionReason.IncorrectLink
                                );
                            }
                        }
                    }

                    return thrown;
                }, Logger
            );
        }

        public void Visit(Branch branch)
        {
            Handler.Snap(
                () =>
                {
                    bool thrown = branch.Stations.Capacity == 0;

                    if (thrown)
                    {
                        throw new DomainValidationException(
                            $"Ветка '{branch.Title}' схемы '{branch.Chart?.City ?? "Неизвестно"}' в городе '{branch.Chart?.City ?? "Неизвестно"}' не имеет ни одной станции",
                            ExceptionType.Warning,
                            ExceptionReason.NotFound
                        );
                    }

                    return thrown;
                }, Logger
            );

            Handler.Snap(
                () =>
                {
                    bool thrown = false;
                    Branch? branchHead;

                    if (branch.Stations.Capacity > 0 && (branchHead = branch.Stations[0].Branch) != null)
                    {
                        foreach (Station station in branch.Stations)
                        {
                            if (station.Branch == null)
                            {
                                thrown = true;

                                throw new DomainValidationException(
                                    $"Станция '{station.Title}' ветки '{branch.Title}' схемы '{branch.Chart?.Title ?? "Неизвестно"}' в городе '{branch.Chart?.City ?? "Неизвестно"}' не привязана к родной ветке",
                                    ExceptionType.Error,
                                    ExceptionReason.NullArgument
                                );
                            }

                            if (station.Branch != branchHead)
                            {
                                thrown = true;

                                throw new DomainValidationException(
                                    $"Станция '{station.Title}' ветки '{branch.Title}' схемы '{branch.Chart?.Title ?? "Неизвестно"}' в городе '{branch.Chart?.City ?? "Неизвестно"}' ссылается не на родную ветку",
                                    ExceptionType.Error,
                                    ExceptionReason.IncorrectLink
                                );
                            }
                        }
                    }

                    return thrown;
                }, Logger
            );
        }

        public void Visit(Station station)
        {
            throw new NotImplementedException();
        }

        public void Visit(Railway railway)
        {
            throw new NotImplementedException();
        }

        public void Visit(Transition transition)
        {
            throw new NotImplementedException();
        }
    }
}