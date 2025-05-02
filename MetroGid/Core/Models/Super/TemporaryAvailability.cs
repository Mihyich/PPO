namespace MetroGid.Core.Models.Super;

public abstract class TemporaryAvailability(TimeOnly opentime, TimeOnly closetime)
{
    public TimeOnly OpenTime { get; set; } = opentime;
    public TimeOnly CloseTime { get; set; } = closetime;

    public bool IsOpenAt(TimeOnly curTime) =>
        OpenTime < CloseTime ?
        OpenTime <= curTime && curTime <= CloseTime :
        CloseTime >= curTime || curTime >= OpenTime;
}