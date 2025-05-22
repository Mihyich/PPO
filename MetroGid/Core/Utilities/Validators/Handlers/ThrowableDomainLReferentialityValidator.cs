using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Utilities.Validators.Handlers;

public class ThrowableDomainReferentialityValidator(
    SuperExceptionHandler handler,
    IExceptionVisitor? logger = null
) : IDomainValidatorVisitor
{
    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    public void Visit(Client client) {}

    public void Visit(Chart chart)
    {
        Handler.Snap(
            () =>
            {
                bool thrown = chart.Branches.Count == 0;

                if (thrown)
                {
                    throw new DomainValidationException(
                        $"Схема '{chart.Title}' в городе '{chart.City}' не имеет ни одной ветки",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
                }

                return thrown;
            }, Logger
        );

        for (int i = 0; i < chart.Branches.Count - 1; ++i)
        {
            for (int j = i + 1; j < chart.Branches.Count; ++j)
            {
                Handler.Snap(
                    () =>
                    {
                        bool thrown = chart.Branches[i].Title == chart.Branches[j].Title;

                        if (thrown)
                            throw new DomainValidationException(
                                $"Схема '{chart.Title}' в городе '{chart.City}' имеет ветки с одинаковыми наименованиями: '{chart.Branches[i].Title}'",
                                ExceptionType.Error,
                                ExceptionReason.ValueDuplicate
                            );

                        return thrown;
                    }, Logger
                );
            }
        }

        Handler.Snap(
            () =>
            {
                bool thrown = false;

                if (chart.Branches.Count > 0)
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

                        if (branch.Chart != chart)
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

        foreach (Branch branch in chart.Branches)
            branch.Validate(this);
    }

    public void Visit(Branch branch)
    {
        Handler.Snap(
            () =>
            {
                bool thrown = branch.Stations.Count == 0;

                if (thrown)
                {
                    throw new DomainValidationException(
                        $"Ветка '{branch.Title}' схемы '{branch.Chart?.Title ?? "Неизвестно"}' в городе '{branch.Chart?.City ?? "Неизвестно"}' не имеет ни одной станции",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
                }

                return thrown;
            }, Logger
        );

        for (int i = 0; i < branch.Stations.Count - 1; ++i)
        {
            for (int j = i + 1; j < branch.Stations.Count; ++j)
            {
                Handler.Snap(
                    () =>
                    {
                        bool thrown = branch.Stations[i].Title == branch.Stations[j].Title;

                        if (thrown)
                            throw new DomainValidationException(
                                $"Ветка схемы '{branch.Chart?.Title ?? "Неизвестно"}' имеет станции с одинаковыми наименованиями: '{branch.Stations[i].Title}'",
                                ExceptionType.Error,
                                ExceptionReason.ValueDuplicate
                            );

                        return thrown;
                    }, Logger
                );
            }
        }

        Handler.Snap(
            () =>
            {
                bool thrown = false;

                if (branch.Stations.Count > 0)
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

                        if (station.Branch != branch)
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

        foreach (Station station in branch.Stations)
            station.Validate(this);
    }

    public void Visit(Station station)
    {
        Handler.Snap(
            () =>
            {
                bool thrown = false;

                Station? prevStation = station.Prev?.Prev ?? null;
                Station? nextStation = station.Next?.Next ?? null;

                Branch? prevBranchHead = prevStation?.Branch ?? null;
                Branch? nextBranchHead = nextStation?.Branch ?? null;

                Branch? curBranchHead = station.Branch;

                if (thrown = curBranchHead == null)
                    throw new DomainValidationException(
                        $"Станция '{station.Title}' ветки '{curBranchHead?.Title ?? "Неизвестно"}' схемы '{curBranchHead?.Chart?.Title ?? "Неизвестно"}' в городе '{curBranchHead?.Chart?.City ?? "Неизвестно"}' не связана с родной веткой",
                        ExceptionType.Error,
                        ExceptionReason.NullArgument
                    );

                if (prevStation != null)
                {
                    if (thrown = prevBranchHead == null)
                        throw new DomainValidationException(
                            $"Станция '{station.Title}' ветки '{curBranchHead?.Title ?? "Неизвестно"}' схемы '{curBranchHead?.Chart?.Title ?? "Неизвестно"}' в городе '{curBranchHead?.Chart?.City ?? "Неизвестно"}' ведет на станцию '{prevStation.Title}' не связанной с родной веткой",
                            ExceptionType.Error,
                            ExceptionReason.NullArgument
                        );

                    if (thrown = prevBranchHead != curBranchHead)
                        throw new DomainValidationException(
                            $"Станция '{station.Title}' ветки '{curBranchHead?.Title ?? "Неизвестно"}' схемы '{curBranchHead?.Chart?.Title ?? "Неизвестно"}' в городе '{curBranchHead?.Chart?.City ?? "Неизвестно"}' ведет на станцию '{prevStation.Title}' связанной с другой веткой",
                            ExceptionType.Error,
                            ExceptionReason.IncorrectLink
                        );
                }

                if (nextStation != null)
                {
                    if (thrown = nextBranchHead == null)
                        throw new DomainValidationException(
                            $"Станция '{station.Title}' ветки '{curBranchHead?.Title ?? "Неизвестно"}' схемы '{curBranchHead?.Chart?.Title ?? "Неизвестно"}' в городе '{curBranchHead?.Chart?.City ?? "Неизвестно"}' ведет на станцию '{nextStation.Title ?? "Неизвестно"}' не связанной с родной веткой",
                            ExceptionType.Error,
                            ExceptionReason.NullArgument
                        );

                    if (thrown = nextBranchHead != curBranchHead)
                        throw new DomainValidationException(
                            $"Станция '{station.Title}' ветки '{curBranchHead?.Title ?? "Неизвестно"}' схемы '{curBranchHead?.Chart?.Title ?? "Неизвестно"}' в городе '{curBranchHead?.Chart?.City ?? "Неизвестно"}' ведет на станцию '{nextStation.Title ?? "Неизвестно"}' связанной с другой веткой",
                            ExceptionType.Error,
                            ExceptionReason.IncorrectLink
                        );
                }

                return thrown;
            }, Logger
        );

        station.Prev?.Validate(this);
        station.Next?.Validate(this);

        foreach (Transition transition in station.Transitions)
            transition.Validate(this);
    }

    public void Visit(Railway railway)
    {
        Handler.Snap(
            () =>
            {
                bool thrown = false;

                if (thrown = railway.Prev == null)
                    throw new DomainValidationException(
                        $"Переезд (Prev) со станции '{railway?.Next?.Title ?? "Неизвестно"}' ветки '{railway?.Next?.Branch?.Title ?? "Неизвестно"}' схемы '{railway?.Next?.Branch?.Chart?.Title ?? "Неизвестно"}' в городе '{railway?.Next?.Branch?.Chart?.City ?? "Неизвестно"}' не имеет Prev ссылки",
                        ExceptionType.Error,
                        ExceptionReason.NullArgument
                    );

                if (thrown = railway.Next == null)
                    throw new DomainValidationException(
                        $"Переезд (Next) со станции '{railway?.Prev?.Title ?? "Неизвестно"}' ветки '{railway?.Prev?.Branch?.Title ?? "Неизвестно"}' схемы '{railway?.Prev?.Branch?.Chart?.Title ?? "Неизвестно"}' в городе '{railway?.Prev?.Branch?.Chart?.City ?? "Неизвестно"}' не имеет Next ссылки",
                        ExceptionType.Error,
                        ExceptionReason.NullArgument
                    );

                return thrown;
            }, Logger
        );
    }

    public void Visit(Transition transition)
    {
        Handler.Snap(
            () =>
            {
                bool thrown = false;

                if (thrown = transition.From == null)
                    throw new DomainValidationException(
                        $"Переход ветки '{transition?.To?.Title ?? "Неизвестно"}' схемы '{transition?.To?.Branch?.Chart?.Title ?? "Неизвестно"}' в городе '{transition?.To?.Branch?.Chart?.City ?? "Неизвестно"}' не имеет From ссылки",
                        ExceptionType.Error,
                        ExceptionReason.NullArgument
                    );

                if (thrown = transition.To == null)
                    throw new DomainValidationException(
                        $"Переход ветки '{transition?.From?.Title ?? "Неизвестно"}' схемы '{transition?.From?.Branch?.Chart?.Title ?? "Неизвестно"}' в городе '{transition?.From?.Branch?.Chart?.City ?? "Неизвестно"}' не имеет To ссылки",
                        ExceptionType.Error,
                        ExceptionReason.NullArgument
                    );

                return thrown;
            }, Logger
        );
    }

    public void Visit(Route route)
    {
        Handler.Snap(
            () =>
            {
                bool thrown = route.Chart == null;

                if (thrown)
                    throw new DomainValidationException(
                        $"Маршрут '{route.Title}' схемы '{route.Chart?.Title ?? "Неизвестно"}' в городе '{route.Chart?.City ?? "Неизвестно"}' не связан с родной схемой",
                        ExceptionType.Error,
                        ExceptionReason.NullArgument
                    );

                return thrown;
            }, Logger
        );

        foreach (var item in route.Path)
        {
            if (item is RouteStationItem { Station: var station })
            {
                Handler.Snap(
                    () =>
                    {
                        bool thrown = route.Chart != station.Branch?.Chart;

                        if (thrown)
                            throw new DomainValidationException(
                                $"Маршрут '{route.Title}' схемы '{route.Chart?.Title ?? "Неизвестно"}' в городе '{route.Chart?.City ?? "Неизвестно"}' содержит станцию '{station.Title}' ветки '{station.Branch?.Title ?? "Неизвестно"}' с отличающейся ссылкой на схему",
                                ExceptionType.Error,
                                ExceptionReason.IncorrectLink
                            );

                        return thrown;
                    }, Logger
                );
            }
            else if (item is RouteConnectionItem { Connection: var connection })
            {
                if (connection is RailwayConnection { Railway: var railway })
                {
                    Handler.Snap(
                        () =>
                        {
                            bool thrown = route.Chart != railway.Prev?.Branch?.Chart;

                            if (thrown)
                                throw new DomainValidationException(
                                    $"Маршрут '{route.Title}' схемы '{route.Chart?.Title ?? "Неизвестно"}' в городе '{route.Chart?.City ?? "Неизвестно"}' содержит переезд, ведущий со станции '{railway.Next?.Title ?? "Неизвестно"}' на станцию '{railway.Prev?.Title ?? "Неизвестно"}' ветки '{railway.Prev?.Branch?.Title ?? "Неизвестно"}' с отличающейся ссылкой на схему",
                                    ExceptionType.Error,
                                    ExceptionReason.IncorrectLink
                                );

                            return thrown;
                        }, Logger
                    );

                    Handler.Snap(
                        () =>
                        {
                            bool thrown = route.Chart != railway.Next?.Branch?.Chart;

                            if (thrown)
                                throw new DomainValidationException(
                                    $"Маршрут '{route.Title}' схемы '{route.Chart?.Title ?? "Неизвестно"}' в городе '{route.Chart?.City ?? "Неизвестно"}' содержит переезд, ведущий со станции '{railway.Prev?.Title ?? "Неизвестно"}' на станцию '{railway.Next?.Title ?? "Неизвестно"}' ветки '{railway.Next?.Branch?.Title ?? "Неизвестно"}' с отличающейся ссылкой на схему",
                                    ExceptionType.Error,
                                    ExceptionReason.IncorrectLink
                                );

                            return thrown;
                        }, Logger
                    );
                }
                else if (connection is TransitionConnection { Transition: var transition })
                {
                    Handler.Snap(
                        () =>
                        {
                            bool thrown = route.Chart != transition.From?.Branch?.Chart;

                            if (thrown)
                                throw new DomainValidationException(
                                    $"Маршрут '{route.Title}' схемы '{route.Chart?.Title ?? "Неизвестно"}' в городе '{route.Chart?.City ?? "Неизвестно"}' содержит переход, ведущий со станции '{transition.To?.Title ?? "Неизвестно"}' ветки '{transition.To?.Branch?.Title ?? "Неизвестно"}' на станцию '{transition.From?.Title ?? "Неизвестно"}' ветки '{transition.From?.Branch?.Title ?? "Неизвестно"}' с отличающейся ссылкой на схему",
                                    ExceptionType.Error,
                                    ExceptionReason.IncorrectLink
                                );

                            return thrown;
                        }, Logger
                    );

                    Handler.Snap(
                        () =>
                        {
                            bool thrown = route.Chart != transition.To?.Branch?.Chart;

                            if (thrown)
                                throw new DomainValidationException(
                                    $"Маршрут '{route.Title}' схемы '{route.Chart?.Title ?? "Неизвестно"}' в городе '{route.Chart?.City ?? "Неизвестно"}' содержит переход, ведущий со станции '{transition.From?.Title ?? "Неизвестно"}' ветки '{transition.From?.Branch?.Title ?? "Неизвестно"}' на станцию '{transition.To?.Title ?? "Неизвестно"}' ветки '{transition.To?.Branch?.Title ?? "Неизвестно"}' с отличающейся ссылкой на схему",
                                    ExceptionType.Error,
                                    ExceptionReason.IncorrectLink
                                );

                            return thrown;
                        }, Logger
                    );
                }
            }
        }
    }
}