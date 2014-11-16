using System;

namespace FamilyTree.Dal
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseTableAttribute : Attribute
    {
        public string Name { get; set; }
    }
}