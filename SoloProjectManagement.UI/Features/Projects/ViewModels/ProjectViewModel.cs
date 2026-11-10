using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Dtos;
using SoloProjectManagement.UI.Features.Projects.Services;
using SoloProjectManagement.UI.Features.Projects.ViewModels;
using SoloProjectManagement.UI.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
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
		private static readonly StatusFilterOption AllStatuses = new("All statuses", null);

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
		[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
		private bool _isBusy;

		[ObservableProperty]
		private string? _errorMessage;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(EmptyMessage))]
		private string _searchText = "";

		[ObservableProperty]
		private string? latestTask;

		[ObservableProperty]
		private bool _hasNoProjects;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(HasActiveFilter))]
		[NotifyPropertyChangedFor(nameof(EmptyMessage))]
		[NotifyCanExecuteChangedFor(nameof(ClearFiltersCommand))]
		private StatusFilterOption _selectedStatusFilter = AllStatuses;

		private CancellationTokenSource? _searchCts;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TotalPages))]
		[NotifyPropertyChangedFor(nameof(HasPages))]
		[NotifyPropertyChangedFor(nameof(PageText))]
		[NotifyPropertyChangedFor(nameof(RangeText))]
		[NotifyPropertyChangedFor(nameof(CanGoNext))]
		[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
		private int _totalCount;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(PageText))]
		[NotifyPropertyChangedFor(nameof(RangeText))]
		[NotifyPropertyChangedFor(nameof(CanGoPrevious))]
		[NotifyPropertyChangedFor(nameof(CanGoNext))]
		[NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
		[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
		private int _currentPage = 1;

		[ObservableProperty]
		private int _pageSize = 6;

		public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
		public bool HasPages => TotalPages > 1;
		public bool CanGoPrevious => CurrentPage > 1 && !IsBusy;
		public bool CanGoNext => CurrentPage < TotalPages && !IsBusy;
		public string PageText => $"Page {CurrentPage} of {TotalPages}";
		public string RangeText
		{
			get
			{
				if (TotalCount == 0) return "";
				var first = (CurrentPage - 1) * PageSize + 1;
				var last = Math.Min(CurrentPage * PageSize, TotalCount);
				return $"{first}–{last} of {TotalCount}";
			}
		}

		public bool HasActiveFilter => !string.IsNullOrWhiteSpace(SearchText) || SelectedStatusFilter.Value is not null;
		public string EmptyMessage => string.IsNullOrWhiteSpace(SearchText)
			? "No projects yet. Create one to get started."
			: $"No projects match \"{SearchText.Trim()}\".";

		public IReadOnlyList<StatusFilterOption> StatusFilters { get; } =
			[AllStatuses, .. Enum.GetValues<ProjectStatus>().Select(s => new StatusFilterOption(s.Humanize(), s))];
		public Task OnNavigatedFromAsync() => Task.CompletedTask;

		public Task OnNavigatedToAsync() => LoadAsync();

		private async Task LoadAsync(CancellationToken ct = default)
		{
			IsBusy = true;
			ErrorMessage = null;

			try
			{
				await LoadProjectsAsync(CurrentPage, ct);
			}
			catch (OperationCanceledException)
			{
				throw;
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

		private async Task ChangePageAsync(int page)
		{
			IsBusy = true;
			ErrorMessage = null;
			try
			{
				await LoadProjectsAsync(page);
			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				ErrorMessage = "Could not load that page. Please try again.";
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task LoadProjectsAsync(int page, CancellationToken ct = default)
		{
			page = Math.Max(1, page);

			var search = string.IsNullOrEmpty(SearchText) ? null : SearchText.Trim();
			var status = SelectedStatusFilter.Value;

			var result = await projectService.GetAllAsync(
				new ProjectsFilterRequest(search, status),
				page,
				PageSize,
				ct);

			var lastPage = Math.Max(1, (int)Math.Ceiling(result.TotalCount / (double)PageSize));

			if(page > lastPage)
			{
				page = lastPage;
				result = await projectService.GetAllAsync(
					new ProjectsFilterRequest(search, status),
					page,
					PageSize,
					ct);
			}

			ct.ThrowIfCancellationRequested();

			Replace(Projects, result.Items);
			TotalCount = result.TotalCount;
			HasNoProjects = result.TotalCount == 0;
			CurrentPage = page;
		}

		partial void OnSearchTextChanged(string value) => _ = RefreshAsync(TimeSpan.FromMilliseconds(300));
		partial void OnSelectedStatusFilterChanged(StatusFilterOption value) => _ = RefreshAsync(TimeSpan.Zero);

		private async Task RefreshAsync(TimeSpan delay)
		{
			_searchCts?.Cancel();
			var cts = _searchCts = new CancellationTokenSource();

			try
			{
				if (delay > TimeSpan.Zero)
					await Task.Delay(delay, cts.Token);

				await LoadAsync(cts.Token);
			}
			catch (OperationCanceledException)
			{
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

		[RelayCommand]
		private void ClearFilters()
		{
			SearchText = "";
			SelectedStatusFilter = AllStatuses;
		}

		[RelayCommand(CanExecute = nameof(CanGoPrevious))]
		private Task PreviousPageAsync() => ChangePageAsync(CurrentPage - 1);

		[RelayCommand(CanExecute = nameof(CanGoNext))]
		private Task NextPageAsync() => ChangePageAsync(CurrentPage + 1);

		private static void Replace<T>(ObservableCollection<T> source, IEnumerable<T> items)
		{
			source.Clear();
			foreach (var item in items) source.Add(item);
		}
	}
}
