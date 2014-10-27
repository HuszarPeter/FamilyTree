using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
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
                OnPropertyChanged();
                OnPropertyChanged("Childs");
                OnPropertyChanged("Parents");
                OnPropertyChanged("Spouses");
                OnPropertyChanged("Siblings");
            }
        }

        public IEnumerable<Person> Childs
        {
            get { return GetRelativesByType(RelationType.Child); }
        }

        public IEnumerable<Person> Parents {
            get { return GetRelativesByType(RelationType.Parent); }
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
    }
}
