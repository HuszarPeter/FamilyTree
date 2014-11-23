using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Utils;

namespace FamilyTree.Dal.Model
{
    public class AgeStatistic : ModelBase
    {
        [DatabaseField(Name = "k")]
        public Int64 K { get; set; }

        public AgeKey Key
        {
            get { return (AgeKey) K; }
        }

        [DatabaseField(Name = "num")]
        public Int64 Number { get; set; }
    }

    public enum AgeKey
    {
        Child = 0,
        Teen = 1,
        Adult = 2,
        UnknownAge = 3,
        Sum = 100
    };
}
