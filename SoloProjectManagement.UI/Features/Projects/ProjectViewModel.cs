using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.ViewModels;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using INavigationAware = Wpf.Ui.Abstractions.Controls.INavigationAware;

namespace SoloProjectManagement.UI.Features.Projects
{
	public partial class ProjectViewModel(
		ProjectService projectService,
		INavigationService navigationService,
		IContentDialogService dialogService) : ObservableObject, INavigationAware
	{
		public ObservableCollection<ProjectCardDto> Projects { get; } = [];

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ProjectsFooter))]
		private int _totalProjects;

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private string? _errorMessage;

		public string ProjectsFooter => $"Showing total of {TotalProjects} projects";
		public Task OnNavigatedFromAsync() => Task.CompletedTask;

		public Task OnNavigatedToAsync() => LoadAsync();

		private async Task LoadAsync()
		{
			IsBusy = true;
			ErrorMessage = null;

			try
			{
				var projects = await projectService.GetAllAsync();

				Replace(Projects, projects.Items);
				TotalProjects = projects.TotalCount;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private async Task AddProjectAsync(CancellationToken ct)
		{
			if (dialogService is null) return;

			var form = new AddProjectViewModel();
			var dialog = new AddProjectDialog(dialogService.GetDialogHost(), form);

			var result = await dialogService.ShowAsync(dialog, ct);
			if (result != ContentDialogResult.Primary) return;

			try
			{
				await projectService.CreateAsync(
					form.Title,
					form.Description,
					form.Category,
					form.Status,
					form.DueDate, ct);

				await LoadAsync();
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		private static void Replace<T>(ObservableCollection<T> source, IEnumerable<T> items)
		{
			source.Clear();
			foreach (var item in items) source.Add(item);
		}
	}
}
