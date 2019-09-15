using System.ComponentModel;
using System.Windows;
using BuildNotifications.ViewModel;

namespace BuildNotifications
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}