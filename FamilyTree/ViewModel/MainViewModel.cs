using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FamilyTree.Dal;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.ViewModel
{
    public class MainViewModel : ModelBase
    {
        private Person _selectedPerson;
        private ObservableCollection<Person> _persons;

        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value; 
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Person> Persons
        {
            get { return _persons ?? (_persons = new ObservableCollection<Person>()); }
        }

        public MainViewModel()
        {
        }

        public async void DownloadData()
        {
            using (var context = new DataContext())
            {
                var pp = await DownloadPersonsAsync(context);
                pp.ForEach(Persons.Add);
                var rr = await DownloadRelationsAsync(context);

                Console.WriteLine("P:{0}, R:{1}", pp.Count, rr.Count);

                SelectedPerson = Persons.FirstOrDefault();
            }
        }

        async Task<List<Person>> DownloadPersonsAsync(DataContext context )
        {
            return context.GetAllPersons().Select(p => p.ConvertToViewPerson()).ToList();
        }

        async Task<List<Relation>> DownloadRelationsAsync(DataContext context)
        {
            return context.GetAllRelations().Select(r => r.ConvertToViewRelation(Persons)).ToList();
        }

    }
}
