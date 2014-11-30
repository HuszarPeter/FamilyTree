using System.Linq;
using FamilyTree.Dal.Model;

namespace FamilyTree.ViewModel.Model
{
    public class GeneratedEvent : Event
    {
        private GeneratedEventType _eventType;
        public GeneratedEventType EventType
        {
            get { return _eventType; }
            set
            {
                _eventType = value;
                OnPropertyChanged();
            }
        }

        public override string Text
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