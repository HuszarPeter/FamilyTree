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
    }
}
