using SoloProjectManagement.UI.Dtos;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;


namespace SoloProjectManagement.UI.Features.Tasks
{
	public sealed record StatusChange(ProjectTaskDto Task, TaskStatus Status);
}
