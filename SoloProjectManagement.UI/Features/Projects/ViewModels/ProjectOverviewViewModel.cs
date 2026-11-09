using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Tasks;
using SoloProjectManagement.UI.Models;
using System.Collections.ObjectModel;
using Wpf.Ui.Abstractions.Controls;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Projects.ViewModels
{
	public partial class ProjectOverviewViewModel(
		ProjectService projectService,
		TaskService taskService,
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

		private static void Replace<T>(ObservableCollection<T> source, IEnumerable<T> items)
		{
			source.Clear();
			foreach (var item in items) source.Add(item);
		}
	}
}