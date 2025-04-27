namespace MetroGid.Core.Models
{
    public abstract class TemporaryAvailability(AccessType type, TimeOnly opentime, TimeOnly closetime)
    {
        public AccessType Type { get; set; } = type;
        public TimeOnly OpenTime { get; set; } = opentime;
        public TimeOnly CloseTime { get; set; } = closetime;

        public bool IsAccessible() => Type == AccessType.ACCESSIBLE;

        public bool IsOpenAt(TimeOnly curTime) =>
            OpenTime < CloseTime ?
            OpenTime <= curTime && curTime <= CloseTime :
            CloseTime >= curTime || curTime >= OpenTime;
    }
}