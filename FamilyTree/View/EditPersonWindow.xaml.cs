using System;
using System.Collections.Generic;
using System.Linq;
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
using FamilyTree.ViewModel;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.View
{
    /// <summary>
    /// Interaction logic for EditPersonWindow.xaml
    /// </summary>
    public partial class EditPersonWindow : Window
    {
        private PersonViewModel _model = new PersonViewModel();

        public EditPersonWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                DataContext = _model;
            };
        }

        public bool? ShowDialog(Person person)
        {
            _model.Person = person;

            return ShowDialog();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
