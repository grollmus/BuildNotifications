using System.ComponentModel;
using System.Windows;
using BuildNotifications.ViewModel;
using NLog.Fluent;

namespace BuildNotifications
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            Visibility = App.StartMinimized ? Visibility.Hidden : Visibility.Visible;
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Log.Info().Message("Hiding window.").Write();
            Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}