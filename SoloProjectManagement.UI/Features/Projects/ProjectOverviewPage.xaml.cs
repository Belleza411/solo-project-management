using SoloProjectManagement.UI.Features.Projects.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace SoloProjectManagement.UI.Features.Projects
{
	/// <summary>
	/// Interaction logic for ProjectOverviewPage.xaml
	/// </summary>
	public partial class ProjectOverviewPage : Page, INavigableView<ProjectOverviewViewModel>
	{
		public ProjectOverviewViewModel ViewModel { get; }
		public ProjectOverviewPage(ProjectOverviewViewModel viewModel)	
		{
			InitializeComponent();

			ViewModel = viewModel;
			DataContext = viewModel;
		}
	}
}
