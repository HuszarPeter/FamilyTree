using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    public class StringAndCounter
    {
        [DatabaseField(Name = "megnevezes")]
        public string Text { get; set; }

        [DatabaseField(Name = "count")]
        public long Count { get; set; }
    }
}
