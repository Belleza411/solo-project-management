using SoloProjectManagement.UI.Persistence;
using SoloProjectManagement.UI.Services;
using System.Windows;

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
		}
	}

}
