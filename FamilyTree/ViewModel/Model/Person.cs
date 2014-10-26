using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class Person : ModelBase
    {
        private int _id;
        private string _firstName;
        private string _lastName;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string BirthFirstName
        {
            get { return _birthFirstName; }
            set
            {
                _birthFirstName = value; 
                OnPropertyChanged();
            }
        }

        public string BirthLastName
        {
            get { return _birthLastName; }
            set
            {
                _birthLastName = value; 
                OnPropertyChanged();
            }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DateOfDeath
        {
            get { return _dateOfDeath; }
            set
            {
                _dateOfDeath = value;
                OnPropertyChanged();
            }
        }

        public byte[] Picture
        {
            get { return _picture; }
            set
            {
                _picture = value;
                OnPropertyChanged();
            }
        }

        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Person> _childs;
        private DateTime _dateOfBirth;
        private DateTime? _dateOfDeath;
        private byte[] _picture;
        private Gender _gender;
        private ObservableCollection<Relation> _relations;
        private string _birthFirstName;
        private string _birthLastName;

        public ObservableCollection<Person> Childs
        {
            get { return _childs ?? (_childs = new ObservableCollection<Person>()); }
        }

        
        public ObservableCollection<Relation> Relations
        {
            get { return _relations ?? (_relations = new ObservableCollection<Relation>()); }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        }
    }


    public enum Gender
    {
        Male,
        Female
    }
}
