using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FamilyTree.Dal;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using FamilyTree.ViewModel.Model;
using Event = FamilyTree.ViewModel.Model.Event;
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

        #region ExitCommand

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new ActionCommand(this, ExitCommandExecute, null)); }
        }

        private void ExitCommandExecute(object obj)
        {
            Application.Current.Shutdown(0);
        }

        #endregion

        public Func<Person, bool> EditPersonFunc
        {
            get { return _editPersonFunc; }
            set
            {
                _editPersonFunc = value;
                _selectedPersonViewModel.EditAction = value;
            }
        }

        private Func<Event, bool> _editEventFunc; 
        public Func<Event, bool> EditEventFunc
        {
            get { return _editEventFunc; }
            set
            {
                _editEventFunc = value;
                _selectedPersonViewModel.AddNewEventFunc = value;
            }
        }

        private bool CheckSelectedPerson(object obj)
        {
            return SelectedPersonViewModel.Person != null;
        }

        #region Add Father Command
        private ICommand _addFatherCommand;
        public ICommand AddFatherCommand
        {
            get
            {
                return _addFatherCommand ?? (_addFatherCommand = new ActionCommand(this, AddFatherCommandExecute, CheckSelectedPerson));
            }
        }

        private void AddFatherCommandExecute(object obj)
        {
            AddParent(obj, Gender.Male);
        } 
        #endregion

        #region Add Mother command
        private ICommand _addMotherCommand;
        public ICommand AddMotherCommand
        {
            get
            {
                return _addMotherCommand ?? (_addMotherCommand = new ActionCommand(this, AddMotherCommandExecute, CheckSelectedPerson));
            }
        }

        private void AddMotherCommandExecute(object obj)
        {
            AddParent(obj, Gender.Female);
        }
        #endregion
        
        private void AddParent(object obj, Gender gender)
        {
            var cPerson = SelectedPersonViewModel.Person;
            var fullName = PersonNameConverter.GetFullNameOfPerson(SelectedPersonViewModel.Person);

            if (SelectedPersonViewModel.Parents.All(p => p.Gender != gender))
            {
                var parent = new Person
                {
                    FirstName = gender == Gender.Female ? "Mother of" : "Father of",
                    Gender = gender,
                    LastName = fullName
                };
                var ok = EditPersonFunc != null && EditPersonFunc(parent);
                if(!ok) return;
                LocalDataStorage.Instance.AddNewPersonWithRelation(SelectedPersonViewModel.Person, parent, RelationType.Parent);
            }
        }

        #region Add Child Command
        private ICommand _addChildCommand;
        public ICommand AddChildCommand
        {
            get { return _addChildCommand ?? (_addChildCommand = new ActionCommand(this, AddChildExecute, CheckSelectedPerson)); }
        }

        private void AddChildExecute(object obj)
        {
            var child = new Person
            {
                FirstName = "Child of",
                LastName = PersonNameConverter.GetFullNameOfPerson(SelectedPersonViewModel.Person)
            };
            var ok = EditPersonFunc != null && EditPersonFunc(child);
            if(!ok) return;
            // Ask the user about the name of the new child and her/his data
            LocalDataStorage.Instance.AddChild(SelectedPersonViewModel.Person, child);
        }
        #endregion

        #region Add Sibling command
        private ICommand _addSiblingCommand;
        public ICommand AddSiblingCommand
        {
            get
            {
                return _addSiblingCommand ??
                       (_addSiblingCommand = new ActionCommand(this, AddSiblingCommandExecute, CheckSelectedPerson));
            }
        }

        private void AddSiblingCommandExecute(object obj)
        {
            var sibling = new Person
            {
                FirstName = "Sibling of",
                LastName = SelectedPersonViewModel.Person.FirstName + " " + SelectedPersonViewModel.Person.LastName
            };
            var ok = EditPersonFunc != null && EditPersonFunc(sibling);
            if(!ok) return;
            LocalDataStorage.Instance.AddNewPersonWithRelation(SelectedPersonViewModel.Person, sibling, RelationType.Sibling);
        }

        #endregion

        #region Add spouse Command

        private ICommand _addSpouseCommand;
        public ICommand AddSpouseCommand
        {
            get
            {
                return _addSpouseCommand ?? (_addSpouseCommand = new ActionCommand(this, AddSpouseCommandExecute, CheckSelectedPerson));
            }
        }

        private void AddSpouseCommandExecute(object obj)
        {
            var spouse = new Person
            {
                FirstName = "Spouse of",
                LastName = SelectedPersonViewModel.Person.FirstName + " " + SelectedPersonViewModel.Person.LastName,
                Gender = SelectedPersonViewModel.Person.Gender == Gender.Male ? Gender.Female : Gender.Male
            };
            var ok = EditPersonFunc != null && EditPersonFunc(spouse);
            if(!ok) return;
            LocalDataStorage.Instance.AddNewPersonWithRelation(SelectedPersonViewModel.Person, spouse, RelationType.Spouse);

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
                       (_removeCurrentPersonCommand = new ActionCommand(this, RemoveCurrentPersonCommandExecute, CheckSelectedPerson));
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

        #region Show Gender statistics Command
        public Action ShowGenderStatisticsAction { get; set; }
        private ICommand _showGendersStatistics;
        public ICommand ShowGendersStatistics
        {
            get
            {
                return _showGendersStatistics ??
                       (_showGendersStatistics = new ActionCommand(this, ShowGendersStatisticsExecute, null));
            }
        }

        private void ShowGendersStatisticsExecute(object obj)
        {
            if (ShowGenderStatisticsAction != null)
            {
                ShowGenderStatisticsAction();
            }
        }

        #endregion

        #region ShowPersons Without childs command
        public Action ShowPeoplesWithoutChildsAction { get; set; }
        private ICommand _showPeoplesWithoutChildsCommand;

        public ICommand ShowPeoplesWithoutChildsCommand
        {
            get
            {
                return _showPeoplesWithoutChildsCommand ??
                       (_showPeoplesWithoutChildsCommand =
                           new ActionCommand(this, ShowPeoplesWithoutChuldsExecute, null));
            }
        }

        private void ShowPeoplesWithoutChuldsExecute(object obj)
        {
            if (ShowPeoplesWithoutChildsAction != null)
                ShowPeoplesWithoutChildsAction();
        } 
        #endregion

        public Action ShowFertilityAction { get; set; }
        private ICommand _showFertilityCommand;
        public ICommand ShowFertilityCommand
        {
            get
            {
                return _showFertilityCommand ??
                       (_showFertilityCommand = new ActionCommand(this, ShowFertilityCommandExecute, null));
            }
        }

        private void ShowFertilityCommandExecute(object obj)
        {
            if (ShowFertilityAction != null)
                ShowFertilityAction();
        }

        public Action ShowTimelineAction;
        private ICommand _showTimeLineCommand;

        public ICommand ShowTimeLineCommand
        {
            get
            {
                return _showTimeLineCommand ?? (_showTimeLineCommand = new ActionCommand(this, ShowtimeLineCommandExecute, null));
            }
        }

        private void ShowtimeLineCommandExecute(object obj)
        {
            if (ShowTimelineAction != null)
                ShowTimelineAction();
        }

        private readonly PersonViewModel _selectedPersonViewModel = new PersonViewModel();
        private ICommand _exitCommand;
        private Func<Person, bool> _editPersonFunc;

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
