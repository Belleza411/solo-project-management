using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Dtos
{
	public sealed record UpdateProjectRequest(
		Guid Id,
		string Title,
		string? Description,
		ProjectCategory Category,
		ProjectStatus Status,
		DateTime? DueDate);
}
