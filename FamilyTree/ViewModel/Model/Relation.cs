using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class Relation : ModelBase
    {
        public Person Person { get; set; }

        public RelationType RelationType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Person, RelationType);
        }
    }
}
