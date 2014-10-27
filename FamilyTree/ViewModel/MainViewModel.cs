using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FamilyTree.Dal;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.ViewModel
{
    public class MainViewModel : ModelBase
    {
        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new ActionCommand(this, RefreshExecute, null)); }
        }

        private void RefreshExecute(object obj)
        {
            DownloadData();        
        }

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
            Console.WriteLine("Downloading data from db...");
            var s = SelectedPersonViewModel.Person != null ? SelectedPersonViewModel.Person.Id : -1;
            LocalDataStorage.Instance.DownloadData();
            if (s > 0)
                SelectedPersonViewModel.Person = Persons.FirstOrDefault(p => p.Id == s);
            if (SelectedPersonViewModel.Person == null)
                SelectedPersonViewModel.Person = Persons.FirstOrDefault();
        }
    }
}
