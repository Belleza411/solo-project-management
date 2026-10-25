using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Projects.Dtos
{
	public sealed record ProjectsFilterRequest(
		string SearchQuery,
		ProjectStatus? Status);
}
