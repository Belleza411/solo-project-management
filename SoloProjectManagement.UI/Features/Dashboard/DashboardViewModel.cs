using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using SoloProjectManagement.UI.Features.Projects;
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

		[ObservableProperty]
		private DashboardOverviewDto _overview;

		[ObservableProperty]
		private int _totalTasksCount;
		[ObservableProperty]
		private int _completedTasksCount;
		 
		public static string DateText => DateTime.Today.ToString("dddd, MMMM d, yyyy").ToUpper();
		public static string Greeting => DateTime.Now.Hour switch
		{
			< 12 => "Good morning",
			< 18 => "Good afternoon",
			_ => "Good evening"
		};
		public string TasksFooter => $"Showing {RecentTasks.Count} of {TotalTasksCount} tasks";

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private string? _errorMessage;

		public Task OnNavigatedToAsync() => LoadAsync();
		public Task OnNavigatedFromAsync() => Task.CompletedTask;

		private async Task LoadAsync()
		{
			IsBusy = true;
			ErrorMessage = null;
			try
			{
				var overview = dashboardService.GetOverviewAsync();
				var recentProjects = dashboardService.GetRecentProjectsAsync();
				var recentTasks = dashboardService.GetRecentTasksAsync();
				await Task.WhenAll(overview, recentProjects, recentTasks);

				Overview = overview.Result;
				Replace(RecentProjects, recentProjects.Result);
				Replace(RecentTasks, recentTasks.Result);

				TotalTasksCount = recentTasks.Result.Count;

				OnPropertyChanged(nameof(TasksFooter));

			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private void ViewAllProjects() => navigation.Navigate(typeof(ProjectsPage));

		private static void Replace<T>(ObservableCollection<T> source, IEnumerable<T> items)
		{
			source.Clear();
			foreach (var item in items) source.Add(item);
		}
	};

}
