using CommunityToolkit.Mvvm.ComponentModel;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public partial class DashboardViewModel(
		DashboardService dashboardService,
		INavigationService navigation) : ObservableObject, INavigationAware
	{
		public ObservableCollection<ProjectCardDto> RecentProjects { get; } = [];
		public ObservableCollection<RecentTaskDto> RecentTasks { get; } = [];
		public Task OnNavigatedToAsync() => LoadAsync();
		public Task OnNavigatedFromAsync() => Task.CompletedTask;

		private Task LoadAsync()
		{
			// TODO: Call service 
			return Task.CompletedTask;
		}
	};

}
