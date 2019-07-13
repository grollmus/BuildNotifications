namespace DummyBuildServer.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainViewModel = Kernel.MainViewModel();
            Closing += (sender, args) => mainViewModel.StopServerCommand.Execute(null);
            DataContext = mainViewModel;
        }
    }
}