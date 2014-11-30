﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Utils;

namespace FamilyTree.ViewModel.Model
{
    public class Event: ModelBase, IEditableObject
    {
        private int _id;
        private DateTime _date;
        private string _description;
        private int _personId;

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

        public int PersonId
        {
            get { return _personId; }
            set
            {
                _personId = value;
                OnPropertyChanged();
            }
        }

        public virtual string Text
        {
            get { return Description; }
        }


        #region IEditableObject

        private bool _isInEditMode = false;
        private Event _cache;

        public void BeginEdit()
        {
            if(_isInEditMode) return;
            _isInEditMode = true;
            _cache = new Event
            {
                Date = Date,
                Description = Description,
                PersonId = PersonId,
                Id = Id
            };
        }

        public void EndEdit()
        {
            _isInEditMode = false;
            _cache = null;
        }

        public void CancelEdit()
        {
            if(!_isInEditMode) return;
            Id = _cache.Id;
            Date = _cache.Date;
            Description = _cache.Description;
            PersonId = _cache.PersonId;
            EndEdit();
        } 
        #endregion
    }
}
