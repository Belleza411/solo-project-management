using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Dashboard.Dtos
{
	public record ProjectCardDto(
		Guid Id,
		ProjectCategory Category,
		string Title,
		string Description,
		ProjectStatus Status,
		DateTime? DueDate,
		string TasksFraction,
		int ProgressPercent);
}
