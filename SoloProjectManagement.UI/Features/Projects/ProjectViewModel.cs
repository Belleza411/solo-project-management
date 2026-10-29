using CommunityToolkit.Mvvm.ComponentModel;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using INavigationAware = Wpf.Ui.Abstractions.Controls.INavigationAware;

namespace SoloProjectManagement.UI.Features.Projects
{
	public partial class ProjectViewModel(
		ProjectService projectService,
		INavigationService navigationService) : ObservableObject, INavigationAware
	{
		public ObservableCollection<ProjectCardDto> Projects { get; } = [];

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ProjectsFooter))]
		private int _totalProjects;

		[ObservableProperty]
		private string? _errorMessage;

		public string ProjectsFooter => $"Showing total of {TotalProjects} projects";
		public Task OnNavigatedFromAsync() => Task.CompletedTask;

		public Task OnNavigatedToAsync() => LoadAsync();

		private async Task LoadAsync()
		{
			try
			{
				ErrorMessage = null;
				var projects = await projectService.GetAllAsync();

				Replace(Projects, projects.Items);
				TotalProjects = projects.TotalCount;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				System.Diagnostics.Debug.WriteLine(ex.Message);  
			}
		}

		private static void Replace<T>(ObservableCollection<T> source, IEnumerable<T> items)
		{
			source.Clear();
			foreach (var item in items) source.Add(item);
		}
	}
}
