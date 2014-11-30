using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    public class PersonIdAndCounter
    {
        [DatabaseField(Name = "szemely_id")]
        public long PersonId { get; set; }

        [DatabaseField(Name = "count")]
        public long Counter { get; set; }
    }
}
