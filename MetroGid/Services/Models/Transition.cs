namespace MetroGid.Services.Models
{
    public class Transition
    {
        public int Occupancy {get; set;} = 0;
        public AccessType Type {get; set;} = AccessType.ACCESSIBLE;
        public TimeOnly OpenTime {get; set;} = new TimeOnly(0, 0, 0);
        public TimeOnly CloseTime {get; set;} = new TimeOnly(0, 0, 0);
        public List<Station> Stations {get; set;} = [];
    }
}