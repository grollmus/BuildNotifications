using BuildNotifications.ViewModel;

namespace BuildNotifications.Views
{
    public partial class InfoPopupDialog
    {
        public InfoPopupDialog()
        {
            DataContext = new InfoPopupViewModel();
            InitializeComponent();
        }
    }
}