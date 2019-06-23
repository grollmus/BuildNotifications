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

            Style = (Style) FindResource(typeof(Window));
        }
    }
}