using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.ViewModel
{
    public class PersonStatisticsViewModel : ModelBase
    {
        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private List<Person> _persons;
        public List<Person> Persons
        {
            get { return _persons; }
            set
            {
                _persons = value;
                OnPropertyChanged();
            }
        }

        private List<PersonWithCount> _personsWithCounts;
        public List<PersonWithCount> PersonsWithCounts
        {
            get { return _personsWithCounts; }
            set
            {
                _personsWithCounts = value;
                OnPropertyChanged();
            }
        }

        public void DownloadPersonswithoutChildsData()
        {
            using (var context = new DataContext())
            {
                Description = "This list shows persons without any childrens.";
                Persons =
                    context.GetAllPersonsWithoutChilds().Select(p => p.ConvertToViewPerson()).ToList();
            }
        }

        public void DownloadMostFertilyPersons()
        {
            using (var context = new DataContext())
            {
                Description = "This list shows the most fertile persons.";
                PersonsWithCounts = context.GetFeritlityList().Select(p => p.ConvertToViewPersonWithCount()).ToList();
            }
        }
    }
}
