using System;
using System.Reflection;

namespace FamilyTree.Dal
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsPrimaryKey { get; set; }
    }

    public class PropertyMappedDatabaseField
    {
        public PropertyInfo PropertyInfo { get; set; }

        public DatabaseFieldAttribute DatabaseFieldAttribute { get; set; }
    }
}
