using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard.Dtos
{
	public sealed record LeftOffTaskDto(
		string Name,
		string ProjectTitle);
}
