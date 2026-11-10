using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Common
{
	public sealed record ProjectStatusFilterOption(string Label, ProjectStatus? Value);
	public sealed record TaskStatusFilterOption(string Label, TaskStatus? Value);
	public sealed record TaskPriorityFilterOption(string Label, TaskPriority? Value);

}
