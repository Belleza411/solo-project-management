using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Dtos
{
	public sealed record ProjectDto(
		Guid Id,
		string Title,
		string Description,
		ProjectStatus Status);
}
