using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utilities.TimeMeter.Super;

public abstract class TimeSuper
{
    public abstract TimeSpan Measure(RouteItem from, RouteItem to);


    public abstract TimeSpan Measure(Station s, Railway r);

    public abstract TimeSpan Measure(Railway r, Station s);

    public abstract TimeSpan Measure(Station s1, Railway r, Station s2);


    public abstract TimeSpan Measure(Station s, Transition t);

    public abstract TimeSpan Measure(Transition t, Station s);

    public abstract TimeSpan Measure(Station s1, Transition t, Station s2);
}