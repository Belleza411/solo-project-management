using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public class DashboardService(string connectionString) : IDashboardService
	{
		public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var totalProjects = await db.Projects.CountAsync(ct);
			var activeProject = await db.Projects
				.Where(p => p.Status == ProjectStatus.Active)
				.CountAsync(ct);

			var totalTasks = await db.ProjectTasks.CountAsync(ct);
			var inProgressTasks = await db.ProjectTasks
				.Where(t => t.Status == TaskStatus.InProgress)
				.CountAsync(ct);

			var (thisWeekStart, thisWeekEnd) = GetThisWeekRange();
			var (lastWeekStart, lastWeekEnd) = GetLastWeekRange();

			var completedTasksThisWeek = await db.ProjectTasks
				.Where(t => t.Status == TaskStatus.Completed
					&& t.CreatedAt >= thisWeekStart
					&& t.CreatedAt < thisWeekEnd)
				.CountAsync(ct);

			var completedTasksLastWeek = await db.ProjectTasks
				.Where(t => t.Status == TaskStatus.Completed
					&& t.CreatedAt >= lastWeekStart
					&& t.CreatedAt < lastWeekEnd)
				.CountAsync(ct);

			var overdueTasks = await db.ProjectTasks
				.Where(t => t.Status == TaskStatus.Overdue)
				.CountAsync(ct);

			var highPriorityOverdueTasks = await db.ProjectTasks
				.Where(t => t.Status == TaskStatus.Overdue
					&& t.Priority == TaskPriority.High)
				.CountAsync(ct);

			return new DashboardOverviewDto(
				TotalProjects: new MetricCardDto("Total Projects", totalProjects, totalProjects),
				TotalTasks: new MetricCardDto("Total Tasks", totalTasks, inProgressTasks),
				CompletedTasksThisWeek: new MetricCardDto("Completed Task This Week", completedTasksThisWeek, completedTasksLastWeek),
				OverdueTasks: new MetricCardDto("Overdue Tasks", overdueTasks, highPriorityOverdueTasks));
		}

		public async Task<List<ProjectCardDto>> GetRecentProjectsAsync(CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.Projects
				.AsNoTracking()
				.Select(p => new ProjectCardDto(
					p.Id,
					p.Title,
					p.Description ?? "",
					p.Status,
					p.DueDate,
					p.Tasks.Count(t => t.Status == TaskStatus.Completed),
					p.Tasks.Count()))
				.ToListAsync(ct);
		}

		public async Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
				.AsNoTracking()
				.OrderBy(t => t.DueDate)
				.ThenBy(t => t.Priority)
				.Select(t => new RecentTaskDto(
					Name: t.Name,
					ProjectTitle: t.Project.Title,
					Status: t.Status,
					Priority: t.Priority,
					DueDate: t.DueDate))
				.ToListAsync(ct);
			
		}

		private static (DateTime start, DateTime end) GetThisWeekRange()
		{
			var today = DateTime.Today;

			int diff = (7 + ((int)today.DayOfWeek - (int)DayOfWeek.Monday)) % 7;

			var startOfTheWeek = today.AddDays(-diff);
			var endOfTheWeek = startOfTheWeek.AddDays(6);

			return (startOfTheWeek, endOfTheWeek);
		}

		private static (DateTime start, DateTime end) GetLastWeekRange()
		{
			var today = DateTime.Today;

			int diff = (7 + ((int)today.DayOfWeek - (int)DayOfWeek.Monday)) % 7;

			var startOfLastWeek = today.AddDays(-diff - 7);
			var endOfLastWeek = startOfLastWeek.AddDays(6);

			return (startOfLastWeek, endOfLastWeek);
		}
	}
}
