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
    /// Interaction logic for SelectPerson.xaml
    /// </summary>
    public partial class SelectPerson : Window, INotifyPropertyChanged
    {
        private Person _selectedPerson;
        public IEnumerable<Person> Persons { get; set; }

        private ICommand _OkCommand;
        public ICommand OkCommand
        {
            get { return _OkCommand ?? (_OkCommand = new ActionCommand(this, OkCommandExecute, CanOkCommandExecute)); }
        }

        private bool CanOkCommandExecute(Object param)
        {
            return true;
        }

        private void OkCommandExecute(Object param)
        {
            DialogResult = true;
            Close();
        }

        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                OnPropertyChanged();
            }
        }

        public SelectPerson()
        {
            InitializeComponent();
        }

        public Person ShowSelector(IEnumerable<Person> persons)
        {
            DataContext = this;
            Persons = persons;
            var res = ShowDialog();
            return res.HasValue && res.Value ? SelectedPerson : null;
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
