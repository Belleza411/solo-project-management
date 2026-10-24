using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Projects.Dtos
{
	public sealed record ProjectDto(
		Guid Id,
		string Title,
		string Description,
		ProjectStatus Status,
		DateTime? DueDate,
		int CompletedTasks,
		int TotalTasks,
		DateTime? UpdatedAt)
	{
		public int ProgressPercent => 
			TotalTasks == 0 ? 0 : (int)Math.Round(CompletedTasks * 100.0 / TotalTasks);
	}
}
