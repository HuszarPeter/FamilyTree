using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    [DatabaseTable(Name = "esemeny")]
    public class Event
    {
        [DatabaseField(Name = "esemeny_id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [DatabaseField(Name = "idopont")]
        public DateTime Date { get; set; }

        [DatabaseField(Name = "megnevezes")]
        public string Description { get; set; }

        [DatabaseField(Name = "szemely_id")]
        public int PersonId { get; set; }
    }

    public enum GeneratedEventType
    {
        Birth = 1,
        Death = 2,
    }
}
