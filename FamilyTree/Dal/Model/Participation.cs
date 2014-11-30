using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    [DatabaseTable(Name = "resztvevo")]
    public class Participation
    {
        [DatabaseField(Name = "szemely_id")]
        public int PersonId { get; set; }

        [DatabaseField(Name = "resztvesz")]
        public long IsParticipating { get; set; }
    }
}
