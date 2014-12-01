using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace FamilyTree.View
{
    /// <summary>
    /// Interaction logic for EditPersonWindow.xaml
    /// </summary>
    public partial class EditPersonWindow : Window
    {
        private PersonViewModel _model;

        public EditPersonWindow()
        {
            InitializeComponent();
        }

        private byte[] OpenPicture()
        {
            var dlg = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "Pictures (jpg,jpeg,png)|*.jpg;*.png;*.jpeg"
            };
            var dlgResult = dlg.ShowDialog(this);
            return dlgResult.Value 
                ? File.ReadAllBytes(dlg.FileName) 
                : null;
        }

        public bool? ShowDialog(Person person)
        {
            _model = new PersonViewModel
            {
                BrowseForPicture = OpenPicture,
                Person = person
            };
            DataContext = _model;

            return ShowDialog();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Asd");
        }
    }
}
