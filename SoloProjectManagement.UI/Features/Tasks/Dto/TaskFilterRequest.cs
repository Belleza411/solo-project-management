using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Tasks.Dto
{
	public sealed record TaskFilterRequest(
		Guid? ProjectId,
		string? SearchQuery,
		TaskStatus? Status,
		TaskPriority? Priority);
}
