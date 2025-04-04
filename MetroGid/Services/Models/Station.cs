namespace MetroGid.Services.Models
{
    public class Station(string title, int occupancy, AccessType type, TimeOnly opentime, TimeOnly closetime)
    {
        public string Title { get; set; } = title;
        public int Occupancy { get; set; } = occupancy;
        public AccessType Type { get; set; } = type;
        public TimeOnly OpenTime { get; set; } = opentime;
        public TimeOnly CloseTime { get; set; } = closetime;
        public List<Transition> Transitions {get; set;} = [];
        public Railway? Railway1 {get; set;}
        public Railway? Railway2 {get; set;}
        public Branch? Branch {get; set;}
    }
}