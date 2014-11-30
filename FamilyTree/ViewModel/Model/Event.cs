using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public virtual string Text
        {
            get { return Description; }
        }
    }
}
