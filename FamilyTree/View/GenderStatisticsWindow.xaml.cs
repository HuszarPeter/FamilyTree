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
    /// Interaction logic for GenderStatisticsWindow.xaml
    /// </summary>
    public partial class GenderStatisticsWindow : Window
    {
        private GenderStatisticsViewModel _model;

        public GenderStatisticsWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (_model != null) return;

                _model = new GenderStatisticsViewModel();
                DataContext = _model;

                _model.DownloadData();
            };

        }
    }
}
