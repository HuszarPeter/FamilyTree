namespace FamilyTree.Dal.Model
{
    public class GeneratedEvent : Event
    {
        [DatabaseField(Name = "szemely_id")]
        public int PersonId { get; set; }

        [DatabaseField(Name = "tipus")]
        public long GeneratedEventType { get; set; }
    }
}