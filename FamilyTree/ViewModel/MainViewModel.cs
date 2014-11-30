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

        private Func<Person, bool> _editPersonFunc;
        public Func<Person, bool> EditPersonFunc
        {
            get { return _editPersonFunc; }
            set
            {
                _editPersonFunc = value;
                SelectedPersonViewModel.EditAction = value;
            }
        }

        private Func<Event, bool> _editEventFunc; 
        public Func<Event, bool> EditEventFunc
        {
            get { return _editEventFunc; }
            set
            {
                _editEventFunc = value;
                SelectedPersonViewModel.AddNewEventFunc = value;
            }
        }

        private Func<Func<Person, bool>, Person> _selectPersonFunc;
        public Func<Func<Person, bool>, Person> SelectPersonFunc
        {
            get { return _selectPersonFunc; }
            set
            {
                _selectPersonFunc = value;
                SelectedPersonViewModel.SelectPersonFunc = value;
            }
        }

        private bool CheckSelectedPerson(object obj)
        {
            return SelectedPersonViewModel.Person != null;
        }

        #region Add Father Command
        public ICommand AddFatherCommand
        {
            get { return _selectedPersonViewModel.AddFatherCommand; }
        }
        #endregion

        #region Add Mother command
        public ICommand AddMotherCommand
        {
            get { return _selectedPersonViewModel.AddMotherCommand; }
        }

        #endregion
        
        #region Add Child Command
        public ICommand AddChildCommand
        {
            get { return SelectedPersonViewModel.AddChildCommand; }
        }
        #endregion

        #region Add Sibling command
        public ICommand AddSiblingCommand
        {
            get { return _selectedPersonViewModel.AddSiblingCommand; }
        }
        #endregion

        #region Add Spouse Command
        public ICommand AddSpouseCommand
        {
            get { return SelectedPersonViewModel.AddSpouseCommand; }
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

        private PersonViewModel _selectedPersonViewModel;
        private ICommand _exitCommand;

        public PersonViewModel SelectedPersonViewModel
        {
            get
            {
                return _selectedPersonViewModel ?? (_selectedPersonViewModel = new PersonViewModel
                {
                    DeletePersonCommand = RemoveCurrentPersonCommand,
                });
            }
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
