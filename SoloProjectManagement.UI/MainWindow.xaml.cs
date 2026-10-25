using Wpf.Ui.Controls;

namespace SoloProjectManagement.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : FluentWindow
	{
		public INavigationView Navigation => RootNavigation;
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}