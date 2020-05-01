using System.ComponentModel;
using System.Windows;
using Anotar.NLog;
using BuildNotifications.ViewModel;

namespace BuildNotifications
{
    public partial class MainWindow : IViewProvider
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(this);
            InitializeComponent();
            Visibility = App.StartMinimized ? Visibility.Hidden : Visibility.Visible;
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            LogTo.Info("Hiding window.");
            Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }

        public Window View => this;
    }
}