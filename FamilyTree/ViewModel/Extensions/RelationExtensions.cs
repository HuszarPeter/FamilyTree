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
        public static Relation ConvertToViewRelation(this Dal.Model.Relation r, Func<int, Person> findPersonFunc )
        {
            var result = new Relation
            {
                SourcePerson = findPersonFunc(r.ParentId),
                DestinationPerson = findPersonFunc(r.ChildId),
                RelationType = (RelationType)r.RelationType
            };
            
            return result;
        }

        public static Dal.Model.Relation ConvertToDalRelation(this Relation relation)
        {
            return new Dal.Model.Relation
            {
                ParentId = relation.SourcePerson.Id,
                ChildId = relation.DestinationPerson.Id,
                RelationId = relation.RelationId,
                RelationType = (int)relation.RelationType
            };
        }

        public static bool IsReverseRelationExists(this List<Relation> relations, Relation relation)
        {
            return
                relations.FirstOrDefault(
                    r =>
                        r.SourcePerson == relation.DestinationPerson && r.DestinationPerson == relation.SourcePerson &&
                        r.RelationType == relation.RelationType.GetReverseType()) != null;
        }

        public static Relation GetReverseRelation(this Relation relation)
        {
            return new Relation
            {
                SourcePerson = relation.DestinationPerson,
                DestinationPerson = relation.SourcePerson,
                RelationType = relation.RelationType.GetReverseType()
            };
        }

        public static RelationType GetReverseType(this RelationType rt)
        {
            if(rt == RelationType.Child)
                return RelationType.Parent;
            if(rt == RelationType.Parent)
                return RelationType.Child;
            return rt;
        }
    }
}
