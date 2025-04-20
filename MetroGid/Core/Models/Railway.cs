namespace MetroGid.Core.Models
{
    public class Railway(TimeOnly duration)
    {
        public Station? Prev { get; set; }
        public Station? Next { get; set; }
        public TimeOnly Duration { get; set; } = duration;
    }
}