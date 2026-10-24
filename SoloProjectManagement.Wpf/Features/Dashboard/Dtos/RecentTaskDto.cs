using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard.Dtos
{
	public sealed record RecentTaskDto(
		string Name,
		string ProjectTitle,
		TaskStatus Status,
		TaskPriority Priority,
		DateTime DueDate);
}
