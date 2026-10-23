using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Dtos
{
	public sealed record CreateTaskRequest(
		Guid ProjectId,
		string Name,
		string Description,
		TaskStatus Status,
		TaskPriority Priority,
		DateTime DueDate);
}
