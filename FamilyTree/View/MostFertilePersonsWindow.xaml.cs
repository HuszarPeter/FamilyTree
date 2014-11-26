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

namespace FamilyTree.View
{
    /// <summary>
    /// Interaction logic for MostFertilePersonsWindow.xaml
    /// </summary>
    public partial class MostFertilePersonsWindow : Window
    {
        private PersonStatisticsViewModel _Model;

        public MostFertilePersonsWindow()
        {
            InitializeComponent();
        }


        public void ShowFertilityList()
        {
            _Model = new PersonStatisticsViewModel();
            DataContext = _Model;
            _Model.DownloadMostFertilyPersons();
            ShowDialog();
        }
    }
}
