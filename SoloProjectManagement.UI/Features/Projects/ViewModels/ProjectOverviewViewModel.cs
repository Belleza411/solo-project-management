using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Services;
using SoloProjectManagement.UI.Features.Tasks;
using SoloProjectManagement.UI.Features.Tasks.Services;
using SoloProjectManagement.UI.Features.Tasks.ViewModel;
using SoloProjectManagement.UI.Models;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Projects.ViewModels
{
	public partial class ProjectOverviewViewModel(
		ProjectService projectService,
		TaskService taskService,
		IContentDialogService dialogService,
		ProjectContext context) : ObservableObject, INavigationAware
	{
		[ObservableProperty]
		private Project? _project;
		public ObservableCollection<ProjectTaskDto> Tasks { get; } = [];

		[ObservableProperty]
		private int _remaining;
		[ObservableProperty] 
		private int _completedCount;
		[ObservableProperty] 
		private int _inProgressCount;
		[ObservableProperty] 
		private int _todoCount;
		[ObservableProperty] 
		private double _progressPercent;
		[ObservableProperty]
		private string _percentText = "0%";
		[ObservableProperty]
		private string _progressText = "";
		[ObservableProperty]
		public string _dueDateText = "";
		[ObservableProperty]
		private string _updatedText = "";

		[ObservableProperty]
		private bool _isBusy;
		[ObservableProperty]
		private string? _errorMessage;

		public IEnumerable<TaskStatus> Statuses { get; } = Enum.GetValues<TaskStatus>();

		public Task OnNavigatedFromAsync() => Task.CompletedTask;
		public Task OnNavigatedToAsync() => LoadAsync();

		private async Task LoadAsync()
		{
			IsBusy = true;
			ErrorMessage = null;

			if (context.SelectedProjectId is not Guid id)
			{
				ErrorMessage = "No project selected.";
				IsBusy = false;
				return;
			}

			try
			{
				var project = await projectService.GetByIdAsync(id);

				if(project is null)
				{
					ErrorMessage = "Project not found";
					Project = null;
					return;
				}

				var tasks = await taskService.GetAllAsync(project.Id);

				Replace(Tasks, tasks.Items);

				var total = project.Tasks.Count;

				Project = project;
				CompletedCount = project.Tasks.Count(t => t.Status == TaskStatus.Completed);
				InProgressCount = project.Tasks.Count(t => t.Status == TaskStatus.InProgress);
				TodoCount = project.Tasks.Count(t => t.Status == TaskStatus.Todo);
				Remaining = total - CompletedCount;

				var percent = total == 0
					? 0
					: Math.Round(CompletedCount * 100.0 / total);

				PercentText = $"{percent:0}%";
				ProgressPercent = percent;

				ProgressText = total == 0 
					? "No tasks yet"
					: $"{CompletedCount} of {total} tasks complete · {Remaining} remaining";

				var latest = project.Tasks
					.Select(t => t.UpdatedAt ?? t.CreatedAt)
					.Append(project.UpdatedAt ?? project.CreatedAt)
					.Max();

				DueDateText = project.DueDate?.ToString("MMM d") ?? "N/A";

				UpdatedText = $"Updated {latest.Humanize()}";

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
		private async Task AddTaskAsync(CancellationToken ct)
		{
			if (dialogService is null) return;

			var form = new AddTaskViewModel();
			var dialog = new AddTaskDialog(form);

			var result = await dialogService.ShowAsync(dialog, ct);
			if (result != ContentDialogResult.Primary) return;

			if (Project is null) return;

			try
			{
				var request = new CreateTaskRequest(
					ProjectId: Project.Id,
					Name: form.Name,
					Description: form.Description,
					Status: form.Status,
					Priority: form.Priority,
					DueDate: form.DueDate);

				await taskService.CreateAsync(request, ct);

				await LoadAsync();
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		[RelayCommand]
		private async Task DeleteTaskAsync(ProjectTaskDto task, CancellationToken ct)
		{
			var result = await dialogService.ShowSimpleDialogAsync(
				new SimpleContentDialogCreateOptions
				{
					Title = "Delete project?",
					Content = $"\"{task.Name}\" and its tasks will be permanently deleted.",
					PrimaryButtonText = "Delete",
					CloseButtonText = "Cancel"
				}, ct);

			if (result != ContentDialogResult.Primary) return;

			await taskService.DeleteAsync(task.Id, ct);
			await LoadAsync();
		}

		[RelayCommand] 
		private async Task EditTaskAsync(ProjectTaskDto task, CancellationToken ct)
		{
			if (dialogService is null) return;

			var form = new EditTaskViewModel(task);
			var dialog = new EditTaskDialog(form);

			var result = await dialogService.ShowAsync(dialog, ct);
			if (result != ContentDialogResult.Primary) return;

			if (Project is null) return;

			try
			{
				var request = new UpdateTaskRequest(
					ProjectId: Project.Id,
					Name: form.Name,
					Description: form.Description,
					Status: form.Status,
					Priority: form.Priority,
					DueDate: form.DueDate);

				await taskService.UpdateAsync(task.Id, request, ct);

				await LoadAsync();
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		[RelayCommand]
		private async Task EditProjectAsync(Project project, CancellationToken ct)
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
		private async Task DeleteProjectAsync(Project project, CancellationToken ct)
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
		private async Task ChangeStatusAsync(StatusChange change, CancellationToken ct)
		{
			if (Project is null || change is null || change.Task.Status == change.Status)
				return;

			try
			{
				var request = new UpdateTaskRequest(
					ProjectId: Project.Id,
					Name: change.Task.Name,
					Description: change.Task.Description,
					Status: change.Status,                       
					Priority: change.Task.Priority,
					DueDate: change.Task.DueDate);

				await taskService.UpdateAsync(change.Task.Id, request, ct);

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