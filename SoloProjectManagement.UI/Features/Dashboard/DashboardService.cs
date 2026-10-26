using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public class DashboardService(string connectionString) : IDashboardService
	{
		public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var projectMetrics = await db.Projects
				.GroupBy(_ => 1)
				.Select(g => new
				{
					Total = g.Count(),
					Active = g.Count(p => p.Status == ProjectStatus.Active)
				})
				.FirstOrDefaultAsync(ct) ?? new { Total = 0, Active = 0};

			var (thisWeekStart, thisWeekEnd) = GetThisWeekRange();
			var (lastWeekStart, lastWeekEnd) = GetLastWeekRange();

			var taskMetrics = await db.ProjectTasks
			   .GroupBy(_ => 1)
			   .Select(g => new
			   {
				   Total = g.Count(),
				   InProgress = g.Count(t => t.Status == TaskStatus.InProgress),
				   CompletedThisWeek = g.Count(t => t.Status == TaskStatus.Completed && t.CreatedAt >= thisWeekStart && t.CreatedAt < thisWeekEnd),
				   CompletedLastWeek = g.Count(t => t.Status == TaskStatus.Completed && t.CreatedAt >= lastWeekStart && t.CreatedAt < lastWeekEnd),
				   Overdue = g.Count(t => t.Status == TaskStatus.Overdue),
				   HighPriorityOverdue = g.Count(t => t.Status == TaskStatus.Overdue && t.Priority == TaskPriority.High)
			   })
			   .FirstOrDefaultAsync(ct) ?? new { Total = 0, InProgress = 0, CompletedThisWeek = 0, CompletedLastWeek = 0, Overdue = 0, HighPriorityOverdue = 0 };

			return new DashboardOverviewDto(
				TotalProjects: new MetricCardDto("Total Projects", projectMetrics.Total, $"{projectMetrics.Active} currently active"),
				TotalTasks: new MetricCardDto("Total Tasks", taskMetrics.Total, $"{taskMetrics.InProgress} tasks currently in progress"),
				CompletedTasksThisWeek: new MetricCardDto("Completed Task This Week", taskMetrics.CompletedThisWeek, GetDifference(taskMetrics.CompletedThisWeek, taskMetrics.CompletedLastWeek)),
				OverdueTasks: new MetricCardDto("Overdue Tasks", taskMetrics.Overdue, $"{taskMetrics.HighPriorityOverdue} high priority overdue tasks") 
			);
		}

		public async Task<List<ProjectCardDto>> GetRecentProjectsAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.Projects
				.AsNoTracking()
				.Where(p => p.Status != ProjectStatus.Archived)
				.OrderByDescending(p => p.UpdatedAt)
				.Select(p => new ProjectCardDto(
					p.Id,
					p.Category,
					p.Title,
					p.Description ?? "",
					p.Status,
					p.DueDate,
					p.Tasks.Count(t => t.Status == TaskStatus.Completed),
					p.Tasks.Count()))
				.Take(3)
				.ToListAsync(ct);
		}

		public async Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
				.AsNoTracking()
				.Where(t => 
					t.Status == TaskStatus.InProgress
					&& t.Status == TaskStatus.Overdue)
				.OrderBy(t => t.DueDate)
				.ThenBy(t => t.Priority)
				.Take(10)
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

		private static string GetDifference(int thisWeek, int lastWeek)
		{
			int diff = thisWeek - lastWeek;

			return diff switch
			{
				> 0 => $"+{diff} vs last week",
				< 0 => $"{diff} vs last week",
				_ => "0 vs last week"
			};
		}
	}
}
