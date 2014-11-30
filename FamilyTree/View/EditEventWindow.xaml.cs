using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using FamilyTree.ViewModel.Model;

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

        public EditEventWindow()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(Event evt)
        {
            Event = evt;
            DataContext = this;

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
