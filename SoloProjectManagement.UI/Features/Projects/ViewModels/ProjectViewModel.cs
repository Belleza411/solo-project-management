using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Services;
using SoloProjectManagement.UI.Features.Projects.ViewModels;
using SoloProjectManagement.UI.Models;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using INavigationAware = Wpf.Ui.Abstractions.Controls.INavigationAware;

namespace SoloProjectManagement.UI.Features.Projects
{
	public partial class ProjectViewModel(
		ProjectService projectService,
		INavigationService navigationService,
		IContentDialogService dialogService,
		ProjectContext context) : ObservableObject, INavigationAware
	{
		public ObservableCollection<ProjectDto> Projects { get; } = [];

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ProjectsFooter))]
		private int _totalProjects;

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private string? _errorMessage;

		[ObservableProperty]
		private string? latestTask;

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
			var dialog = new AddProjectDialog(form);

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

		[RelayCommand]
		private async Task EditProjectAsync(ProjectDto project, CancellationToken ct)
		{
			if (dialogService is null) return;

			var form = new EditProjectViewModel(project.Title, project.Description, project.Category, project.Status, project.DueDate);
			var dialog = new EditProjectDialog(form);

			var result = await dialogService.ShowAsync(dialog, ct);
			if (result != ContentDialogResult.Primary) return;

			try
			{
				await projectService.UpdateAsync(
					project.Id,
					form.Title,
					form.Description,
					form.Category,
					form.DueDate,
					form.Status, ct);

				await LoadAsync();
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}


		[RelayCommand]
		private async Task DeleteProjectAsync(ProjectDto project, CancellationToken ct)
		{
			var result = await dialogService.ShowSimpleDialogAsync(
				new SimpleContentDialogCreateOptions
				{
					Title = "Delete project?",
					Content = $"\"{project.Title}\" and its tasks will be permanently deleted.",
					PrimaryButtonText = "Delete",
					CloseButtonText = "Cancel"
				}, ct);

			if (result != ContentDialogResult.Primary) return;

			await projectService.DeleteAsync(project.Id, ct);
			await LoadAsync();
		}

		[RelayCommand]
		private void GoToOverview(ProjectDto project)
		{
			context.SelectedProjectId = project.Id;
			navigationService.Navigate(typeof(ProjectOverviewPage));
		}

		private static void Replace<T>(ObservableCollection<T> source, IEnumerable<T> items)
		{
			source.Clear();
			foreach (var item in items) source.Add(item);
		}
	}
}
