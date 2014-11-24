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
                    AskToDeleteFunc = AskDeletePerson,
                    ShowGenderStatisticsAction = ShowGenderStats,
                    EditPersonFunc = EditPerson
                };

                DataContext = _Model;

                _Model.DownloadData();
            };
        }

        private bool EditPerson(Person person)
        {
            if (person == null) return false;

            var form = new EditPersonWindow
            {
                Owner = this
            };
            var result = form.ShowDialog(person);
            return result.HasValue && result.Value;
        }

        private void ShowGenderStats()
        {
            var win = new GenderStatisticsWindow();
            win.Owner = this;
            win.ShowDialog();

        }

        private bool AskDeletePerson(Person person)
        {
            var result = MessageBox.Show(this, Properties.Resources.MainWindow_AskDeletePerson_Are_you_sure_, Properties.Resources.MainWindow_AskDeletePerson_Delete_, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }
    }
}
