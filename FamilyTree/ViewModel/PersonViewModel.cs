using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using FamilyTree.Dal;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
using FamilyTree.ViewModel.Extensions;
using Event = FamilyTree.ViewModel.Model.Event;
using Person = FamilyTree.ViewModel.Model.Person;

namespace FamilyTree.ViewModel
{
    public class PersonViewModel : ModelBase
    {
        private Person _person;
        public Person Person
        {
            get { return _person; }
            set
            {
                _person = value;
                _events = null;
                OnPropertyChanged();
                OnPropertyChanged("Events");
                NotifyRelationsChanged();
            }
        }

        #region Events

        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                OnPropertyChanged();
            }
        }

        private List<Event> _events = null;
        public List<Event> Events
        {
            get { return _events ?? (_events = DowloadEvents()); }
        }

        private List<Event> DowloadEvents()
        {
            using (var context = new DataContext())
            {
                return context.GetPersonEvents(Person.ConvertToDalPerson()).Select(e => e.ConvertToModelEvent()).ToList();
            }
        }
        #endregion
        
        #region Select another person Command
        private ICommand _selectPersonCommand;
        public ICommand SelectPersonCommand
        {
            get
            {
                return _selectPersonCommand ??
                       (_selectPersonCommand = new ActionCommand(this, SelectAnotherPerson, null));
            }
        }

        private void SelectAnotherPerson(Object param)
        {
            var p = param as Person;
            if (p == null) return;
            Person = p;
        }

        #endregion

        #region Remove connection command
        private ICommand _removeConnectionCommand;
        public ICommand RemoveConnectionCommand
        {
            get
            {
                return _removeConnectionCommand ??
                       (_removeConnectionCommand = new ActionCommand(this, RemoveConnection, null));
            }
        }

        private void RemoveConnection(object param)
        {
            var p = param as Person;
            if (p == null) return;

            LocalDataStorage.Instance.RemoveRelation(Person, p);
            NotifyRelationsChanged();
        } 
        #endregion

        #region Edit Command
        public Func<Person, bool> EditAction { get; set; } 
        private ICommand _editCommand;
        public ICommand EditCommand
        {
            get { return _editCommand ?? (_editCommand = new ActionCommand(this, EditCommandExecute, null)); }
        }

        private void EditCommandExecute(object obj)
        {
            Person.BeginEdit();
            var ok = EditAction != null && EditAction(Person);
            if (!ok)
            {
                Person.CancelEdit();
                return;
            }
            Person.EndEdit();
            LocalDataStorage.Instance.UpdatePerson(Person);
        }

        #endregion

        #region Browse For Pictures Command
        public Func<byte[]> BrowseForPicture { get; set; }
        private ICommand _browsePictureCommand;
        public ICommand BrowsePictureCommand
        {
            get
            {
                return _browsePictureCommand ??
                       (_browsePictureCommand = new ActionCommand(this, BrowseForPictireCommandExecute, null));
            }
        }

        private void BrowseForPictireCommandExecute(object obj)
        {
            if (BrowseForPicture == null) return;
            var pic = BrowseForPicture();
            if (pic == null) return;
            Person.Picture = pic;
        }

        #endregion

        #region Delete Events
        private ICommand _DeleteEventCommand;
        public ICommand DeleteEventCommand
        {
            get { return _DeleteEventCommand ?? (_DeleteEventCommand = new ActionCommand(this, DeleteEventCommandExecute, CanDeleteEventCommandExecute)); }
        }

        private bool CanDeleteEventCommandExecute(Object param)
        {
            return SelectedEvent != null;
        }

        private void DeleteEventCommandExecute(Object param)
        {
            using (var context = new DataContext())
            {
                context.DeleteEvent(SelectedEvent.ConvertBackToDalModel());
            }
            SelectedEvent = null;
            RefreshEvents();
        } 
        #endregion

        #region New Event

        public Func<Event, bool> AddNewEventFunc; 
        private ICommand _newEventCommand;
        public ICommand NewEventCommand
        {
            get { return _newEventCommand ?? (_newEventCommand = new ActionCommand(this, NewEventExecuted, null)); }
        }

        private void NewEventExecuted(object obj)
        {
            var newEvent = new Event
            {
                Id = -1,
                Description = "<<>>",
                Date = DateTime.Now.Date,
                PersonId = Person.Id
            };
            if(AddNewEventFunc != null && AddNewEventFunc(newEvent))
            {
                // insert new event!
                using (var context = new DataContext())
                {
                    context.AddEvent(newEvent.ConvertBackToDalModel());
                    RefreshEvents();
                }
            }
        } 
        #endregion

        #region Modify Event Command
        private ICommand _ModifyCommand;
        public ICommand ModifyCommand
        {
            get { return _ModifyCommand ?? (_ModifyCommand = new ActionCommand(this, ModifyCommandExecute, CanModifyCommandExecute)); }
        }

        private bool CanModifyCommandExecute(Object param)
        {
            return SelectedEvent != null;
        }

        private void ModifyCommandExecute(Object param)
        {
            SelectedEvent.BeginEdit();
            if (AddNewEventFunc != null && AddNewEventFunc(SelectedEvent))
            {
                using (var context = new DataContext())
                {
                    context.UpdateEvent(SelectedEvent.ConvertBackToDalModel());

                    SelectedEvent.EndEdit();
                    RefreshEvents();
                    return;

                }
            }
            SelectedEvent.CancelEdit();
        } 
        #endregion

        private void RefreshEvents()
        {
            SelectedEvent = null;
            _events = null;
            OnPropertyChanged("Events");
        }

        public IEnumerable<Person> Childs
        {
            get { return GetRelativesByType(RelationType.Child); }
        }

        public IEnumerable<Person> Parents {
            get
            {
                // direct parents
                var result = GetRelativesByType(RelationType.Parent);
                // parents of siblings
                return result;
            }
        }

        public IEnumerable<Person> Spouses
        {
            get { return GetRelativesByType(RelationType.Spouse); }
        }

        public IEnumerable<Person> Siblings
        {
            get { return GetRelativesByType(RelationType.Sibling); }
        }

        private IEnumerable<Person> GetRelativesByType(RelationType t)
        {
            return LocalDataStorage.Instance.Relations.Where(r => r.SourcePerson == Person && r.RelationType == t)
                .Select(r => r.DestinationPerson);
        }

        public PersonViewModel()
        {
            LocalDataStorage.Instance.Persons.CollectionChanged += (s, e) => NotifyRelationsChanged();
            LocalDataStorage.Instance.Relations.CollectionChanged += (s, e) => NotifyRelationsChanged();
        }

        private void NotifyRelationsChanged()
        {
            OnPropertyChanged("Childs");
            OnPropertyChanged("Parents");
            OnPropertyChanged("Spouses");
            OnPropertyChanged("Siblings");
        }
    }
}
