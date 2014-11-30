using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class PersonAndCounter : ModelBase
    {
        private Person _person;
        private long _counter;

        public Person Person
        {
            get { return _person; }
            set
            {
                _person = value;
                OnPropertyChanged();
            }
        }

        public long Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnPropertyChanged();
            }
        }
    }
}
