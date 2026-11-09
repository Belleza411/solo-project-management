using Humanizer;
using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard.Services
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
				.FirstOrDefaultAsync(ct) ?? new { Total = 0, Active = 0 };

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

		public async Task<List<ProjectDto>> GetRecentProjectsAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var projects = await db.Projects
				.AsNoTracking()
				.Include(p => p.Tasks)
				.Where(p => p.Status != ProjectStatus.Archived)
				.OrderByDescending(p => p.UpdatedAt)
				.Take(3)
				.ToListAsync(ct);

			return projects.Select(p => p.ToDto()).ToList();
		}

		public async Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
			   .AsNoTracking()
			   .Include(t => t.Project)
			   .Where(t => t.Status != TaskStatus.Completed)
			   .OrderBy(t => t.Priority)
			   .ThenBy(t => t.DueDate)
			   .Take(5)
			   .Select(t => new RecentTaskDto(
				   Name: t.Name,
				   ProjectTitle: t.Project != null ? t.Project.Title : "No Project",
				   Status: t.Status,
				   Priority: t.Priority,
				   DueDate: t.DueDate))
			   .ToListAsync(ct);
		}

		public async Task<LeftOffTaskDto?> GetLeftOffTaskAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var leftOffTask = await db.ProjectTasks
			   .AsNoTracking()
			   .Select(t => new
			   {			   
				   TaskName = t.Name,
				   t.Status,
				   ProjectTitle = t.Project != null ? t.Project.Title : "No Project",
				   LastActivity = t.ActivityLogs.Max(a => (DateTime?)a.CreatedAt)
			   })
			   .Where(x => x.LastActivity != null && x.Status != TaskStatus.Completed)
			   .OrderByDescending(x => x.LastActivity)
			   .FirstOrDefaultAsync(ct);

			return new LeftOffTaskDto(
				Name: leftOffTask?.TaskName ?? "No task",
				ProjectTitle: leftOffTask?.ProjectTitle ?? "No project"
			);
		}

		public async Task<List<ActivityLogDto>> GetRecentActivityAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ActivityLogs
				.AsNoTracking()
				.OrderByDescending(a => a.CreatedAt)
				.Take(5)
				.Join(
					db.Projects,
					log => log.ProjectId,
					project => project.Id,
					(log, project) => new ActivityLogDto(
						log.Name,
						project.Title,
						log.CreatedAt.Humanize())
				)
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