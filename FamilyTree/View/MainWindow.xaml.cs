using System;
using System.Windows;
using System.Windows.Controls;
using FamilyTree.ViewModel;

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

                _Model = new MainViewModel();
                DataContext = _Model;

                _Model.DownloadData();
            };
        }

    }
}
