using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    public class YearStatistics
    {
        [DatabaseField(Name = "year")]
        public long Year { get; set; }

        [DatabaseField(Name = "count")]
        public long Count { get; set; }
    }
}
