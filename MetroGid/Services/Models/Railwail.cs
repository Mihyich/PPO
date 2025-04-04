namespace MetroGid.Services.Models
{
    public class Railway
    {
        public Station? Src {get; set;}
        public Station? Dst {get; set;}
        public TimeOnly Duration {get; set;} = new TimeOnly(0, 3, 0);
    }
}