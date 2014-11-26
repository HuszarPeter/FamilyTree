namespace FamilyTree.Dal.Model
{
    public class PersonWithCount : Person
    {
        [DatabaseField(Name = "count")]
        public long Count { get; set; }
    }
}