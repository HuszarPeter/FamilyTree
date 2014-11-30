using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class EventParticipator : ModelBase
    {
        private Person _person;
        private bool _isParticipating;

        public Person Person
        {
            get { return _person; }
            set
            {
                _person = value;
                OnPropertyChanged();
            }
        }

        public bool IsParticipating
        {
            get { return _isParticipating; }
            set
            {
                _isParticipating = value;
                OnPropertyChanged();
            }
        }
    }
}
