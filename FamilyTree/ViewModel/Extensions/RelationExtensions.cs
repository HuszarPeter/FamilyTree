﻿using System;
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
                RelationType = r.RelationType
            };
            
            return result;
        }

        public static RelationType GetReverseType(this RelationType rt)
        {
            if(rt == RelationType.Child)
                return RelationType.Parent;
            if(rt == RelationType.Parent)
                return RelationType.Child;
            return RelationType.Spouse;
        }
    }
}
