namespace Uno.Diagnostics
{
    public class AllocateEvent : ProfileEvent
    {
        public readonly int Class;
        public readonly int Id;
        public readonly int Weight;

        public override EventType Type { get {  return EventType.Allocate;} }

        public AllocateEvent(int cls, int id, int weight)
        {
            Class = cls;
            Id = id;
            Weight = weight;
        }
    }
}
