using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class Event: ModelBase
    {
        private int _id;
        private DateTime _date;
        private string _description;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public String Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
    }

    public class GeneratedEvent : Event
    {
        private int _personId;
        private GeneratedEventType _eventType;

        public int PersonId
        {
            get { return _personId; }
            set
            {
                _personId = value;
                OnPropertyChanged();
            }
        }

        public GeneratedEventType EventType
        {
            get { return _eventType; }
            set
            {
                _eventType = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                {
                    var person = LocalDataStorage.Instance.Persons.FirstOrDefault(p => p.Id == PersonId);
                    return string.Format("{0} of {1}", EventType, person != null ? person.FullName : "??");
                }
                return Description;
            }
        }
    }
}
