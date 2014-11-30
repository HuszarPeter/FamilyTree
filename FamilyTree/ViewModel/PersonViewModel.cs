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
using FamilyTree.ViewModel.Model;
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

        private List<Event> _events;
        public List<Event> Events
        {
            get { return _events ?? (_events = DowloadEvents()); }
        }

        private List<Event> DowloadEvents()
        {
            if(Person == null) return new List<Event>();
            using (var context = new DataContext())
            {
                return context.GetPersonEvents(Person.ConvertToDalPerson()).Select(e => e.ConvertToModelEvent()).ToList();
            }
        }
        #endregion

        public ICommand DeletePersonCommand { get; set; }

        private bool CheckSelectedPerson(object arg)
        {
            return Person != null;
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
                LastName = PersonNameConverter.GetFullNameOfPerson(Person)
            };
            var ok = EditAction != null && EditAction(child);
            if (!ok) return;
            // Ask the user about the name of the new child and her/his data
            LocalDataStorage.Instance.AddChild(Person, child);
        }
        #endregion

        #region Add Father Command
        private ICommand _addFatherCommand;
        public ICommand AddFatherCommand
        {
            get
            {
                return _addFatherCommand ?? (_addFatherCommand = new ActionCommand(this, AddFatherCommandExecute, CanAddFatherCommandExecute));
            }
        }

        private bool CanAddFatherCommandExecute(object arg)
        {
            return Person != null && Parents.FirstOrDefault(p => p.IsMale) == null;
        }

        private void AddFatherCommandExecute(object obj)
        {
            AddParent(Gender.Male);
        }
        #endregion

        #region Add Mother Command
        private ICommand _addMotherCommand;
        public ICommand AddMotherCommand
        {
            get
            {
                return _addMotherCommand ?? (_addMotherCommand = new ActionCommand(this, AddMotherCommandExecute, CanAddMotherCommandExecute));
            }
        }

        private bool CanAddMotherCommandExecute(object arg)
        {
            return Person != null && Parents.FirstOrDefault(p => p.IsMale == false) == null;
        }

        private void AddMotherCommandExecute(object obj)
        {
            AddParent(Gender.Female);
        }

        #endregion

        public Func<Func<Person, bool>, Person> SelectPersonFunc; 

        #region Set Father command
        private ICommand _setFatherCommand;
        public ICommand SetFatherCommand
        {
            get { return _setFatherCommand ?? (_setFatherCommand = new ActionCommand(this, SetFatherCommandExecute, CanSetFatherCommandExecute)); }
        }

        private bool CanSetFatherCommandExecute(Object param)
        {
            return Person != null && Parents.FirstOrDefault(p => p.IsMale) == null;
        }

        private void SetFatherCommandExecute(Object param)
        {
            if(SelectPersonFunc == null) return;
            var selected = SelectPersonFunc(p => (p.IsMale && (p.Age > Person.Age || !p.Age.HasValue)));
            if(selected == null) return;
            LocalDataStorage.Instance.AddNewPersonWithRelation(selected, Person, RelationType.Child);
        } 
        #endregion

        #region Set Mother Command
        private ICommand _setMotherCommand;
        public ICommand SetMotherCommand
        {
            get { return _setMotherCommand ?? (_setMotherCommand = new ActionCommand(this, SetMotherCommandExecute, CanSetMotherCommandExecute)); }
        }

        private bool CanSetMotherCommandExecute(Object param)
        {
            return Person != null && Parents.FirstOrDefault(p => !p.IsMale) == null;
        }

        private void SetMotherCommandExecute(Object param)
        {
            if (SelectPersonFunc == null) return;
            var selected = SelectPersonFunc(p => (!p.IsMale && (p.Age > Person.Age || !p.Age.HasValue)));
            if (selected == null) return;
            LocalDataStorage.Instance.AddNewPersonWithRelation(selected, Person, RelationType.Child);
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
                LastName = Person.FullName
            };
            var ok = EditAction != null && EditAction(sibling);
            if (!ok) return;
            LocalDataStorage.Instance.AddNewPersonWithRelation(Person, sibling, RelationType.Sibling);
        }

        #endregion

        #region Add Spouse Command
        private ICommand _addSpouseCommand;
        public ICommand AddSpouseCommand
        {
            get
            {
                return _addSpouseCommand ?? (_addSpouseCommand = new ActionCommand(this, AddSpouseCommandExecute, CanAddSpouseCommandExecute));
            }
        }

        private bool CanAddSpouseCommandExecute(object arg)
        {
            return Person != null && !Spouses.Any();
        }

        private void AddSpouseCommandExecute(object obj)
        {
            var spouse = new Person
            {
                FirstName = "Spouse of",
                LastName = Person.FullName,
                Gender = Person.Gender == Gender.Male ? Gender.Female : Gender.Male
            };
            var ok = EditAction != null && EditAction(spouse);
            if (!ok) return;
            LocalDataStorage.Instance.AddNewPersonWithRelation(Person, spouse, RelationType.Spouse);

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

        private void AddParent(Gender gender)
        {
            var fullName = Person.FullName;

            if (Parents.All(p => p.Gender != gender))
            {
                var parent = new Person
                {
                    FirstName = gender == Gender.Female ? "Mother of" : "Father of",
                    Gender = gender,
                    LastName = fullName
                };
                var ok = EditAction != null && EditAction(parent);
                if (!ok) return;
                LocalDataStorage.Instance.AddNewPersonWithRelation(Person, parent, RelationType.Parent);
            }
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
