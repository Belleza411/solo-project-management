using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Dtos
{
	public sealed record CreateProjectRequest(
		string Title,
		string? Description,
		ProjectCategory Category,
		ProjectStatus Status,
		DateTime? DueDate);
}
