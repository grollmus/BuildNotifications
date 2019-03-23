using System.Windows;

namespace BuildNotifications
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}