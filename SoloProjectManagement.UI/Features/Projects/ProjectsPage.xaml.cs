using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace SoloProjectManagement.UI.Features.Projects
{
	/// <summary>
	/// Interaction logic for ProjectPage.xaml
	/// </summary>
	public partial class ProjectsPage : Page, INavigableView<ProjectViewModel>
	{
		public ProjectViewModel ViewModel { get; }
		public ProjectsPage(ProjectViewModel viewModel)
		{
			InitializeComponent();

			ViewModel = viewModel;
			DataContext = viewModel;
		}
	}
}
