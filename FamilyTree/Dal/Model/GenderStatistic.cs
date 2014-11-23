using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    public class GenderStatistic
    {
        [DatabaseField(Name = "ferfi")]
        public Int64 Male { get; set; }

        public bool IsMale
        {
            get { return Male == 1; }
        }

        [DatabaseField(Name = "cnt")]
        public Int64 Count { get; set; }

        [DatabaseField(Name = "osszes")]
        public Int64 All { get; set; }

        public double Percent
        {
            get { return All > 0 ? 100*(Count/(double) All) : 0.0; }
        }
    }
}
