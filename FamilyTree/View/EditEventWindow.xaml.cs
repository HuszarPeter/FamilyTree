using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FamilyTree.Annotations;
using FamilyTree.Utils;
using FamilyTree.ViewModel;
using FamilyTree.ViewModel.Model;
using Microsoft.Win32;

namespace FamilyTree.View
{
    /// <summary>
    /// Interaction logic for EditEventWindow.xaml
    /// </summary>
    public partial class EditEventWindow : Window, INotifyPropertyChanged
    {
        private Event _event;
        public Event Event
        {
            get { return _event; }
            set
            {
                _event = value;
                OnPropertyChanged();
            }
        }

        #region OK Command
        private ICommand _OkCommand;

        public ICommand OkCommand
        {
            get { return _OkCommand ?? (_OkCommand = new ActionCommand(this, OkCommandExecute, CanOkCommandExecute)); }
        }

        private bool CanOkCommandExecute(Object param)
        {
            return Event.Date < DateTime.Now && Event.Date > DateTime.MinValue &&
                   !String.IsNullOrWhiteSpace(Event.Description);
        }

        private void OkCommandExecute(Object param)
        {
            DialogResult = true;
            Close();
        } 
        #endregion

        private List<EventParticipator> _persons;
        private ObservableCollection<EventDocument> _eventDocuments = new ObservableCollection<EventDocument>();

        public List<EventParticipator> Persons
        {
            get { return _persons; }
            set
            {
                _persons = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EventDocument> EventDocuments
        {
            get { return _eventDocuments; }
            set
            {
                _eventDocuments = value;
                OnPropertyChanged();
            }
        }

        public EventDocument SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                OnPropertyChanged();
            }
        }

        #region Add Document Command
        private ICommand _addDocumentCommand;
        private EventDocument _selectedDocument;

        public ICommand AddDocumentCommand
        {
            get { return _addDocumentCommand ?? (_addDocumentCommand = new ActionCommand(this, AddDocumentCommandExecute, CanAddDocumentCommandExecute)); }
        }

        private bool CanAddDocumentCommandExecute(Object param)
        {
            return true;
        }

        private void AddDocumentCommandExecute(Object param)
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true
            };
            var dlgRes = dlg.ShowDialog(this);
            if (!dlgRes.Value) return;
            
            var fi = new FileInfo(dlg.FileName);
            
            EventDocuments.Add(new EventDocument
            {
                Data = File.ReadAllBytes(dlg.FileName),
                EventId = Event.Id,
                FileName = fi.Name,
                Id = -1,
                FileType = fi.Extension
            });
        } 
        #endregion

        #region Delete documents command
        private ICommand _deleteDocumentCommand;
        public ICommand DeleteDocumentCommand
        {
            get { return _deleteDocumentCommand ?? (_deleteDocumentCommand = new ActionCommand(this, DeleteDocumentCommandExecute, CanDeleteDocumentCommandExecute)); }
        }

        private bool CanDeleteDocumentCommandExecute(Object param)
        {
            return SelectedDocument != null;
        }

        private void DeleteDocumentCommandExecute(Object param)
        {
            EventDocuments.Remove(SelectedDocument);
            SelectedDocument = null;
        }
        #endregion
        
        public EditEventWindow()
        {
            InitializeComponent();

        }

        public bool? ShowDialog(Event evt)
        {
            Event = evt;
            DataContext = this;
            Persons = LocalDataStorage.Instance.DownloadEventPersons(Event);
            LocalDataStorage.Instance.DownloadEventDocuments(Event)
                .ForEach(d => EventDocuments.Add(d));

            return ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
