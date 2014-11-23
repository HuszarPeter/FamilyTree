using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FamilyTree.Dal;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using Person = FamilyTree.ViewModel.Model.Person;

namespace FamilyTree.ViewModel
{
    public class MainViewModel : ModelBase
    {
        #region Refresh Command
        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new ActionCommand(this, RefreshExecute, null)); }
        }

        private void RefreshExecute(object obj)
        {
            DownloadData();
        } 
        #endregion

        #region Add Child Command
        private ICommand _addChildCommand;
        public ICommand AddChildCommand
        {
            get { return _addChildCommand ?? (_addChildCommand = new ActionCommand(this, AddChildExecute, null)); }
        }

        private void AddChildExecute(object obj)
        {
            // Ask the user about the name of the new child and her/his data
            LocalDataStorage.Instance.AddChild(SelectedPersonViewModel.Person, new Person
            {
                FirstName = "Child of",
                LastName = SelectedPersonViewModel.Person.FirstName + " " + SelectedPersonViewModel.Person.LastName
            });
            SelectedPersonViewModel.NotifyRelationsChanged();
        }
        #endregion

        #region Add Sibling command
        private ICommand _addSiblingCommand;
        public ICommand AddSiblingCommand
        {
            get
            {
                return _addSiblingCommand ??
                       (_addSiblingCommand = new ActionCommand(this, AddSiblingCommandExecute, null));
            }
        }

        private void AddSiblingCommandExecute(object obj)
        {
            LocalDataStorage.Instance.AddNewPersonWithRelation(SelectedPersonViewModel.Person, new Person
            {
                FirstName = "Sibling of",
                LastName = SelectedPersonViewModel.Person.FirstName + " " + SelectedPersonViewModel.Person.LastName
            }, RelationType.Sibling);
            SelectedPersonViewModel.NotifyRelationsChanged();
        }

        #endregion

        #region Remove current person Command

        public Func<Person, bool> AskToDeleteFunc { get; set; } 

        private ICommand _removeCurrentPersonCommand;
        public ICommand RemoveCurrentPersonCommand
        {
            get
            {
                return _removeCurrentPersonCommand ??
                       (_removeCurrentPersonCommand = new ActionCommand(this, RemoveCurrentPersonCommandExecute, null));
            }
        }

        private void RemoveCurrentPersonCommandExecute(object obj)
        {
            var doDelete = AskToDeleteFunc != null && AskToDeleteFunc(SelectedPersonViewModel.Person);

            if (doDelete)
            {
                LocalDataStorage.Instance.RemovePerson(SelectedPersonViewModel.Person);
                SelectedPersonViewModel.Person = Persons.FirstOrDefault();
            }
        }

        #endregion
        
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
