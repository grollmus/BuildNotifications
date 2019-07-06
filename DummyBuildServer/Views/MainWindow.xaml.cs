namespace DummyBuildServer.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = Kernel.MainViewModel();
        }
    }
}