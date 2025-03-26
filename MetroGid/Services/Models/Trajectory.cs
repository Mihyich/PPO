namespace MetroGid.Services.Models
{
    public enum TrajectoryComponentType
    {
        STATION = 0,
        TRANSITION
    }

    public abstract class TrajectoryComponentBase(TrajectoryComponentType type)
    {
        protected TrajectoryComponentType type = type;
        public TrajectoryComponentType GetComponentType() { return type; }
    }

    public class TrajectoryStation : TrajectoryComponentBase
    {
        public TrajectoryStation() : base(TrajectoryComponentType.STATION) {}
    }

    public class TrajectoryTransition : TrajectoryComponentBase
    {
        public TrajectoryTransition() : base(TrajectoryComponentType.TRANSITION) {}
    }

    public class Trajectory
    {
        Trajectory() { components = []; }

        public void AddComponent(TrajectoryComponentBase component) { components.Add(component); }

        public bool RemoveComponent(TrajectoryComponentBase component) { return components.Remove(component); }

        public IReadOnlyList<TrajectoryComponentBase> GetComponents() { return components.AsReadOnly(); }

        private readonly List<TrajectoryComponentBase> components;
    }
}