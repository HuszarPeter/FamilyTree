using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Dal
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseFieldAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
