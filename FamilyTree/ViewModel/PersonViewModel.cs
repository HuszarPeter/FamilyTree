﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
                NotifyRelationsChanged();
            }
        }

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
            if(p == null) return;
            Person = p;
        }

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

        public void NotifyRelationsChanged()
        {
            OnPropertyChanged("Childs");
            OnPropertyChanged("Parents");
            OnPropertyChanged("Spouses");
            OnPropertyChanged("Siblings");
        }
    }
}
