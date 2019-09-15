using System.ComponentModel;
using System.Windows;
using Anotar.NLog;
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
            LogTo.Info("Hiding window.");
            Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}