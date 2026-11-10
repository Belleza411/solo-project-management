using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Dtos
{
	public record ProjectTaskDto(
		Guid Id,
		Guid ProjectId,
		string Name,
		string Description,
		TaskStatus Status,
		TaskPriority Priority,
		DateTime? DueDate)
	{
		public IEnumerable<TaskStatus> Statuses { get; } = Enum.GetValues<TaskStatus>();
		public IEnumerable<TaskPriority> Priorities { get; } = Enum.GetValues<TaskPriority>();
		public string DueDateText { get; } = DueDate?.ToString("MMM d") ?? "N/A";
	}
}