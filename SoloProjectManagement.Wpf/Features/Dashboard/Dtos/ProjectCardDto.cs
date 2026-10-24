using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Dashboard.Dtos
{
	public record ProjectCardDto(
		Guid Id,
		string Title,
		string Description,
		ProjectStatus Status,
		DateTime? DueDate,
		int CompletedTasks,
		int TotalTasks)
	{
		public int ProgressPercent =>
			TotalTasks == 0 ? 0 : (int)Math.Round(CompletedTasks * 100.0 / TotalTasks);
	}
}
