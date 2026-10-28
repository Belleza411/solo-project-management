using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Dtos
{
	public record ProjectCardDto(
		Guid Id,
		ProjectCategory Category,
		string Title,
		string Description,
		string CurrentTask,
		ProjectStatus Status,
		DateTime? DueDate,
		string TasksFraction,
		int ProgressPercent);
}
