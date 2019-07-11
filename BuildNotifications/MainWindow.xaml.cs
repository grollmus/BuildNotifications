using BuildNotifications.Core.Text;
using BuildNotifications.ViewModel;

namespace BuildNotifications
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();

            var text = StringLocalizer.Instance["Test"];
        }
    }
}