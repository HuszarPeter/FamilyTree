using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using Person = FamilyTree.ViewModel.Model.Person;
using Relation = FamilyTree.ViewModel.Model.Relation;

namespace FamilyTree.ViewModel
{
    public class LocalDataStorage : ModelBase
    {
        private static readonly LocalDataStorage _instance = new LocalDataStorage();
        public static LocalDataStorage Instance
        {
            get { return _instance; }
        }

        private readonly ObservableCollection<Person> _persons = new ObservableCollection<Person>();
        public ObservableCollection<Person> Persons
        {
            get { return _persons; }
        }

        private readonly  ObservableCollection<Relation> _relations = new ObservableCollection<Relation>();
        public ObservableCollection<Relation> Relations
        {
            get { return _relations; }
        }

        static LocalDataStorage()
        {
        }

        public async void DownloadData()
        {
            Persons.Clear();
            Relations.Clear();
            using (var context = new DataContext())
            {
                var pp = await DownloadPersonsAsync(context);
                pp.ForEach(Persons.Add);
                var rr = await DownloadRelationsAsync(context);
                rr.ForEach(Relations.Add);
            }
        }

        async Task<List<Person>> DownloadPersonsAsync(DataContext context)
        {
            return context.GetAllPersons().Select(p => p.ConvertToViewPerson()).ToList();
        }

        async Task<List<Relation>> DownloadRelationsAsync(DataContext context)
        {
            var result1 = context.GetAllRelations().Select(p => p.ConvertToViewRelation(
                id => Persons.FirstOrDefault(person => person.Id == id))
                ).ToList();

            /*we have to make the other end of relations if they aren't exists!*/
            /*we also need to make the siblings and nephews if they aren't exists*/
            var result2 = new List<Relation>();
            result1.ForEach(relation =>
            {
                if (!result1.IsReverseRelationExists(relation))
                {
                    result2.Add(relation.GetReverseRelation());
                }
            });

            return result1.Union(result2).Distinct().ToList();
        }

        public void RemoveRelation(Person person, Person person1)
        {
            var relationsToRemove = Relations
                .Where(
                    r =>
                        (r.SourcePerson == person && r.DestinationPerson == person1) ||
                        (r.SourcePerson == person1 && r.DestinationPerson == person))
                .ToList();

            relationsToRemove
                .ForEach(relation => Relations.Remove(relation));
        }

        public void AddChild(Person person, Person child)
        {
            using (var context = new DataContext())
            {
                if (Persons.FirstOrDefault(p => p == child) == null)
                {
                    Persons.Add(child);
                    var dalPerson = child.ConvertToDalPerson();
                    context.AddPerson(dalPerson);
                    child.Id = dalPerson.Id;
                }

                var personToChild = new Relation
                {
                    SourcePerson = person,
                    DestinationPerson = child,
                    RelationType = RelationType.Child
                };
                var childToPerson = new Relation
                {
                    SourcePerson = child,
                    DestinationPerson = person,
                    RelationType = RelationType.Parent
                };

                if (Relations.FirstOrDefault(r => r == personToChild) == null)
                {
                    Relations.Add(personToChild);
                    var dalRelation = personToChild.ConvertToDalRelation();
                    context.AddRelation(dalRelation);
                }

                if (Relations.FirstOrDefault(r => r == childToPerson) == null)
                {
                    Relations.Add(childToPerson);
                    context.AddRelation(childToPerson.ConvertToDalRelation());
                }
            }
        }
    }
}
