using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
using Person = FamilyTree.ViewModel.Model.Person;
using Relation = FamilyTree.ViewModel.Model.Relation;

namespace FamilyTree.ViewModel.Extensions
{
    public static class RelationExtensions
    {
        public static Relation ConvertToViewRelation(this Dal.Model.Relation r, IEnumerable<Person> persons )
        {
            var result = new Relation
            {
                Person = persons.FirstOrDefault(p => p.Id == r.ChildId),
                RelationType = r.RelationType
            };
            var rootPerson = persons.FirstOrDefault(p => p.Id == r.ParentId);
            rootPerson.Relations.Add(result);

            result.Person.Relations.Add(new Relation
            {
                Person = rootPerson,
                RelationType = r.RelationType.GetReverseType(),
            });
            return result;
        }

        private static RelationType GetReverseType(this RelationType rt)
        {
            if(rt == RelationType.Child)
                return RelationType.Parent;
            if(rt == RelationType.Parent)
                return RelationType.Child;
            return RelationType.Spouse;
        }
    }
}
