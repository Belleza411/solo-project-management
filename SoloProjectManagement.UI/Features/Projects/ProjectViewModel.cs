using CommunityToolkit.Mvvm.ComponentModel;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;

namespace SoloProjectManagement.UI.Features.Projects
{
	public partial class ProjectViewModel(
		ProjectService projectService,
		INavigationService navigationService) : ObservableObject, INavigationAware
	{
		public ObservableCollection<ProjectCardDto> Projects { get; } = [];
		public Task OnNavigatedFromAsync() => Task.CompletedTask;

		public Task OnNavigatedToAsync() => RunAsync(LoadAsync);

		private async Task LoadAsync()
		{
			// Call service and fill Projects
		}
		private async Task RunAsync(Func<Task> action)
		{
			try
			{
				//ErrorMessage = null;
				await action();
			}
			catch (Exception ex)
			{
				//ErrorMessage = ex.Message;
			}
		}
	}
}
