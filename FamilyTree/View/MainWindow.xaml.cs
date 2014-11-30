using System;
using System.Linq;
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
                    ShowPeoplesWithoutChildsAction = ShowChildlessPeoples,
                    ShowFertilityAction = ShowFertilityStat,
                    ShowTimelineAction = ShowTimeline,
                    EditPersonFunc = EditPerson,
                    EditEventFunc = EditEvent,
                    SelectPersonFunc = SelectPerson,
                };

                DataContext = _Model;

                _Model.DownloadData();
            };
        }

        private Person SelectPerson(Func<Person, bool> predicate)
        {
            var form = new SelectPerson
            {
                Owner = this
            };

            var result = form.ShowSelector(LocalDataStorage.Instance.Persons.Where(predicate));
            return result;
        }

        private bool EditEvent(Event arg)
        {
            var form = new EditEventWindow
            {
                Owner = this
            };
            var r = form.ShowDialog(arg);
            var result = r.HasValue && r.Value;
            arg.Participators = form.Persons
                .Where(p => p.IsParticipating)
                .Select(p => p.Person.Id)
                .ToList();

            return result;
        }

        private void ShowTimeline()
        {
            var win = new TimelineWindow
            {
                Owner = this

            };
            win.ShowDialog();
        }

        private void ShowFertilityStat()
        {
            var frm = new MostFertilePersonsWindow
            {
                Owner = this
            };
            frm.ShowFertilityList();
        }

        private void ShowChildlessPeoples()
        {
            var frm = new PeoplesWithutChildsWindow
            {
                Owner = this
            };
            frm.ShowPersonsWithoutChilds();
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
            var win = new GeneralStatisticsWindow();
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
