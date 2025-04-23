using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Utilities
{
    public class BuilderChart(
        IDomainValidatorVisitor domainAttribsValidator,
        IDomainValidatorVisitor domainReferentialityValidator
    ) : BuilderChartBase(domainAttribsValidator, domainReferentialityValidator)
    {
        protected List<Branch> Branches = [];

        public override void BuildBranch(string title, int color, AccessType type)
        {
            Branch branch = new(title, color, type);
            branch.Validate(DomainAttribsValidator);
            Branches.Add(branch);
        }

        public override void BuildStation(
            string branch_title,
            string title, int occupancy, AccessType type, TimeOnly opentime, TimeOnly closetime
        )
        {
            Branch? branch;

            if ((branch = Branches.FirstOrDefault(b => b.Title == branch_title)) != null)
            {
                Station station = new(title, occupancy, type, opentime, closetime);
                station.Validate(DomainAttribsValidator);
                branch.Stations.Add(station);
                station.Branch = branch;
            }
            else
            {
                throw new BuilderProccessException(
                    $"Создание станции невозможно, поскольку в списке созданных веток не найдена ветка: '{branch_title}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }
        }

        public override void BuildRailway(
            string branch_title, string station_title_src, string station_title_dst,
            TimeOnly duration
        )
        {
            Branch? branch;
            Station? src;
            Station? dst;

            if (station_title_src == station_title_dst)
            {
                throw new BuilderValidationException(
                    $"Попытка создания переезда между одними и теми же станциями - '{station_title_src}'",
                    ExceptionType.Warning,
                    ExceptionReason.ValidationFailed
                );
            }

            if ((branch = Branches.FirstOrDefault(b => b.Title == branch_title)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание переезда невозможно, поскольку в списке созданных веток не найдена ветка: '{branch_title}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            if ((src = branch.Stations.FirstOrDefault(s => s.Title == station_title_src)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание переезда невозможно, поскольку в списке созданных станций ветки '{branch_title}' не найдена станция: '{station_title_src}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            if ((dst = branch.Stations.FirstOrDefault(s => s.Title == station_title_dst)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание переезда невозможно, поскольку в списке созданных станций ветки '{branch_title}' не найдена станция: '{station_title_dst}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            
            Railway railway = new(duration);
            railway.Validate(DomainAttribsValidator);

            if (!src.HasNext() && !dst.HasPrev())
            {
                railway.Prev = src;
                railway.Next = dst;

                src.Next = railway;
                dst.Prev = railway;
            }
            else if (!dst.HasNext() && !src.HasPrev())
            {
                railway.Prev = dst;
                railway.Next = src;

                dst.Next = railway;
                src.Prev = railway;
            }
            else
            {
                throw new BuilderProccessException(
                    "Достигнут предположительно недостижимый фрагмент кода! Иди чини алгоритм!!!",
                    ExceptionType.Critical,
                    ExceptionReason.UnexpectedBehavior
                );
            }
        }

        public override void BuildTransition(
            string branch_title_src, string station_title_src, string branch_title_dst, string station_title_dst,
            int occupancy, AccessType type, TimeOnly duration, TimeOnly opentime, TimeOnly closetime
        )
        {
            Branch? branch_src;
            Branch? branch_dst;
            Station? station_src;
            Station? station_dst;

            if (branch_title_src == branch_title_dst)
            {
                throw new BuilderValidationException(
                    $"Попытка создания перехода между одними и теми же ветками - '{branch_title_src}'",
                    ExceptionType.Warning,
                    ExceptionReason.ValidationFailed
                );
            }

            if ((branch_src = Branches.FirstOrDefault(b => b.Title == branch_title_src)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание перехода невозможно, поскольку в списке созданных веток не найдена ветка: '{branch_title_src}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            if ((branch_dst = Branches.FirstOrDefault(b => b.Title == branch_title_dst)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание перехода невозможно, поскольку в списке созданных веток не найдена ветка: '{branch_title_dst}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            if ((station_src = branch_src.Stations.FirstOrDefault(s => s.Title == station_title_src)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание перехода невозможно, поскольку в списке созданных станций ветки '{branch_title_src}' не найдена станция: '{station_title_src}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            if ((station_dst = branch_dst.Stations.FirstOrDefault(s => s.Title == station_title_dst)) == null)
            {
                throw new BuilderProccessException(
                    $"Создание перехода невозможно, поскольку в списке созданных станций ветки '{branch_title_dst}' не найдена станция: '{station_title_dst}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );
            }

            Transition transition = new(occupancy, type, duration, opentime, closetime)
            {
                From = station_src,
                To = station_dst
            };

            transition.Validate(DomainAttribsValidator);

            station_src.Transitions.Add(transition);
            station_dst.Transitions.Add(transition);
        }

        public override void BuildChart(string title, string city, string svg_inst)
        {
            Chart = new(title, city, svg_inst)
            {
                Branches = Branches
            };

            foreach (Branch branch in Chart.Branches)
                branch.Chart = Chart;

            Chart.Validate(DomainAttribsValidator);
            Chart.Validate(DomainReferentialityValidator);
        }

        public override Chart GetResult() =>
            Chart ??
                throw new BuilderValidationException(
                    "BuilderChart не создал конечный продукт (null)",
                    ExceptionType.Error,
                    ExceptionReason.NullResult);
    }
}