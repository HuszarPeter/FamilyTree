using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal.Model
{
    [DatabaseTable(Name = "kapcsolat")]
    public class Relation
    {
        [DatabaseField(Name = "kapcsolat_id", IsPrimaryKey = true)]
        public int RelationId { get; set; }

        [DatabaseField(Name = "tipus")]
        public int RelationType { get; set; }

        [DatabaseField(Name = "szemely_id1")]
        public int ParentId { get; set; }

        [DatabaseField(Name = "szemely_id2")]
        public int ChildId { get; set; }
    }

    public enum RelationType
    {
        Unknown = 0,
        Child = 1,
        Spouse = 2,
        Parent = 3,
        Sibling = 4,
    }
}
