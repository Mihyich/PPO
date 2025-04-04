namespace MetroGid.Services.Models
{
    public class Station
    {
        public string Title {get; set;} = string.Empty;
        public int Occupancy {get; set;} = 0;
        public AccessType Type {get; set;} = AccessType.ACCESSIBLE;
        public TimeOnly OpenTime {get; set;} = new TimeOnly(0, 0, 0);
        public TimeOnly CloseTime {get; set;} = new TimeOnly(0, 0, 0);
        public List<Transition> Transitions {get; set;} = [];
        public Railway? Rail1 {get; set;}
        public Railway? Rail2 {get; set;}
        public Branch? Branch {get; set;}
    }
}