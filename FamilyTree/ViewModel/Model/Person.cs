using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Utils;
using NodaTime;

namespace FamilyTree.ViewModel.Model
{
    public class Person : ModelBase, IEquatable<Person>
    {
        private int _id;
        private string _firstName;
        private string _lastName;
        private DateTime _dateOfBirth;
        private DateTime? _dateOfDeath;
        private byte[] _picture;
        private Gender _gender;
        private string _birthFirstName;
        private string _birthLastName;

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

        public long Age
        {
            get
            {
                var now = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var birth = new LocalDate(DateOfBirth.Year, DateOfBirth.Month, DateOfBirth.Day);
                var p = Period.Between(birth, now, PeriodUnits.Years);

                return p.Years;
            }
        }

        public bool Equals(Person other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(Person left, Person right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Person left, Person right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Person) obj);
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
