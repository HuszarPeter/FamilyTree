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
        private readonly PersonViewModel _selectedPersonViewModel = new PersonViewModel();
        public PersonViewModel SelectedPersonViewModel
        {
            get { return _selectedPersonViewModel; }
        }

        public ObservableCollection<Person> Persons
        {
            get
            {
                return LocalDataStorage.Instance.Persons;
            }
        }

        public MainViewModel()
        {
        }

        public void DownloadData()
        {
            LocalDataStorage.Instance.DownloadData();
            SelectedPersonViewModel.Person = Persons.FirstOrDefault();
        }
    }
}
