using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Dtos
{
	public sealed record ProjectDto(
		Guid Id,
		ProjectCategory Category,
		string Title,
		string Description,
		ProjectStatus Status,
		DateTime? DueDate,
		int TotalTasks,
		int CompletedTasks,
		string? MarkedTask)
	{
		public int ProgressPercent { get; } = TotalTasks == 0
			? 0
			: (int)(Math.Round(CompletedTasks * 100.0 / TotalTasks));

		public string DueDateText { get; } = DueDate?.ToString("MMM d") ?? "N/A";
		public string TasksFraction { get; } = TotalTasks == 0
			? "No tasks yet"
			: $"{CompletedTasks} of {TotalTasks} tasks complete · {TotalTasks - CompletedTasks} remaining";
	}
}
