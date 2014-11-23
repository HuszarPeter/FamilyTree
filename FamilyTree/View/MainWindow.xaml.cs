using System;
using System.Windows;
using System.Windows.Controls;
using FamilyTree.ViewModel;
using FamilyTree.ViewModel.Model;

namespace FamilyTree.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _Model;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if(_Model != null)
                    return;

                _Model = new MainViewModel
                {
                    AskToDeleteFunc = AskDeletePerson
                };

                DataContext = _Model;

                _Model.DownloadData();
            };
        }

        private bool AskDeletePerson(Person person)
        {
            var result = MessageBox.Show(this, Properties.Resources.MainWindow_AskDeletePerson_Are_you_sure_, Properties.Resources.MainWindow_AskDeletePerson_Delete_, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }
    }
}
