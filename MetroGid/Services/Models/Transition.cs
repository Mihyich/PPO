namespace MetroGid.Services.Models
{
    public class Transition(int occupancy, AccessType type, TimeOnly duration, TimeOnly opentime, TimeOnly closetime)
    {
        public int Occupancy { get; set; } = occupancy;
        public AccessType Type { get; set; } = type;
        public TimeOnly Duration { get; set; } = duration;
        public TimeOnly OpenTime { get; set; } = opentime;
        public TimeOnly CloseTime { get; set; } = closetime;
        public Station? Station1 {get; set;}
        public Station? Station2 {get; set;}
    }
}