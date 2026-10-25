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
		DateTime DueDate);
}