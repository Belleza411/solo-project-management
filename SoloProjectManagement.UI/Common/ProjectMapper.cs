using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Common
{
	public static class ProjectMapper
	{
		public static ProjectDto ToDto(this Project p)
		{
			return new ProjectDto(
				Id: p.Id,
				Category: p.Category,
				Title: p.Title,
				Description: p.Description ?? "No description",
				Status: p.Status,
				DueDate: p.DueDate,
				TotalTasks: p.Tasks.Count,
				CompletedTasks: p.Tasks.Count(t => t.Status == TaskStatus.Completed),
				MarkedTask: p.Tasks
					.Where(t => t.Status != TaskStatus.Completed)
					.OrderByDescending(t => t.UpdatedAt)
					.ThenBy(t => (int)t.Priority)
					.Select(t => t.Name)
					.FirstOrDefault() ?? "No active task");
		}
	}
}
