using SoloProjectManagement.UI.Features.Dashboard;
using SoloProjectManagement.UI.Features.Projects;
using SoloProjectManagement.UI.Features.Tasks;
using SoloProjectManagement.UI.Persistence;
using SoloProjectManagement.UI.Services;
using SoloProjectManagement.UI.Shell;
using System.Windows;
using NavigationService = Wpf.Ui.NavigationService;

namespace SoloProjectManagement.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var projectService = new ProjectService(DatabaseSettings.ConnectionString);
			var taskService = new TaskService(DatabaseSettings.ConnectionString);
			var dashboardService = new DashboardService(DatabaseSettings.ConnectionString);

			var pages = new PageProvider();
			var navigation = new NavigationService(pages);

			pages.Register(() => new DashboardPage(new DashboardViewModel(dashboardService, navigation)));
			pages.Register(() => new ProjectsPage(new ProjectViewModel(projectService, navigation)));

			var window = new MainWindow { DataContext = new MainViewModel() };
			navigation.SetNavigationControl(window.Navigation);
			window.Show();

			navigation.Navigate(typeof(DashboardPage));
		}
	}

}
