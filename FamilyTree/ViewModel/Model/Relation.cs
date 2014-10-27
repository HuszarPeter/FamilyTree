using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class Relation : ModelBase, IEquatable<Relation>
    {
        private int _relationId;
        private Person _sourcePerson;
        private Person _destinationPerson;
        private RelationType _relationType;

        public int RelationId
        {
            get { return _relationId; }
            set
            {
                _relationId = value; 
                OnPropertyChanged();
            }
        }

        public Person SourcePerson
        {
            get { return _sourcePerson; }
            set
            {
                _sourcePerson = value; 
                OnPropertyChanged();
            }
        }

        public Person DestinationPerson
        {
            get { return _destinationPerson; }
            set
            {
                _destinationPerson = value; 
                OnPropertyChanged();
            }
        }

        public RelationType RelationType
        {
            get { return _relationType; }
            set
            {
                _relationType = value;
                OnPropertyChanged();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Relation) obj);
        }

        public bool Equals(Relation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SourcePerson, other.SourcePerson) && Equals(DestinationPerson, other.DestinationPerson) && RelationType == other.RelationType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SourcePerson != null ? SourcePerson.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DestinationPerson != null ? DestinationPerson.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)RelationType;
                return hashCode;
            }
        }

        public static bool operator ==(Relation left, Relation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Relation left, Relation right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1} | ({2})", SourcePerson, DestinationPerson, RelationType);
        }
    }
}
