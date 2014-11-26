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
    /// Interaction logic for TimelineWindow.xaml
    /// </summary>
    public partial class TimelineWindow : Window
    {
        private TimelineViewModel _Model;
        public TimelineWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (_Model != null) return;
                _Model = new TimelineViewModel();
                DataContext = _Model;
                _Model.DownloadGeneratedEvents();
            };

        }
    }
}
