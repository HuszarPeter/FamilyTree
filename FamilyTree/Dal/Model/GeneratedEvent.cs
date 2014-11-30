namespace FamilyTree.Dal.Model
{
    public class GeneratedEvent : Event
    {
        [DatabaseField(Name = "tipus")]
        public long GeneratedEventType { get; set; }
    }
}