using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	/// <summary>
	/// Interaction logic for DashboardPage.xaml
	/// </summary>
	public partial class DashboardPage : Page, INavigableView<DashboardViewModel>
	{
		public DashboardViewModel ViewModel { get; }
		public DashboardPage(DashboardViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = viewModel;
			InitializeComponent();
		}
	}
}
