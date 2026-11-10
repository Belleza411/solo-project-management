using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard.Services
{
	public class DashboardService(string connectionString, TimeProvider? clock = null) : IDashboardService
	{
		private readonly TimeProvider _clock = clock ?? TimeProvider.System;
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

			var (thisWeekStartUtc, nextWeekStartUtc, lastWeekStartUtc) = GetWeekBoundsUtc(_clock);
			var today = _clock.GetLocalNow().Date;  

			var taskMetrics = await db.ProjectTasks
			   .GroupBy(_ => 1)
			   .Select(g => new
			   {
				   Total = g.Count(),
				   InProgress = g.Count(t => t.Status == TaskStatus.InProgress),
				   CompletedThisWeek = g.Count(t => t.Status == TaskStatus.Completed
										  && t.UpdatedAt >= thisWeekStartUtc && t.UpdatedAt < nextWeekStartUtc),
				   CompletedLastWeek = g.Count(t => 
						t.Status == TaskStatus.Completed
						&& t.UpdatedAt >= lastWeekStartUtc && t.UpdatedAt < thisWeekStartUtc),
				   Overdue = g.Count(t =>
						t.Status == TaskStatus.Overdue ||
						(t.Status != TaskStatus.Completed && t.Status != TaskStatus.Overdue && t.DueDate < today)),
				   HighPriorityOverdue = g.Count(t => t.Status != TaskStatus.Completed
					   && t.DueDate < today && t.Priority == TaskPriority.High)
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
				.OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
				.ThenByDescending(p => p.Id)
				.Take(3)
				.ToListAsync(ct);

			return projects.Select(p => p.ToDto()).ToList();
		}

		public async Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
			   .AsNoTracking()
			   .Where(t => t.Status != TaskStatus.Completed)
			   .OrderBy(t => t.Priority == TaskPriority.High 
					? 0
					: t.Priority == TaskPriority.Medium ? 1 : 2)
			   .ThenBy(t => t.DueDate)
			   .ThenBy(t => t.Id)
			   .Take(5)
			   .Select(t => new RecentTaskDto(
				   Name: t.Name,
				   ProjectTitle: t.Project != null ? t.Project.Title : "No Project",
				   Status: t.Status,
				   Priority: t.Priority,
				   DueDate: t.DueDate))
			   .ToListAsync(ct);
		}

		internal static (DateTime ThisWeekStartUtc, DateTime NextWeekStartUtc, DateTime LastWeekStartUtc) GetWeekBoundsUtc(TimeProvider clock)
		{
			var local = clock.GetLocalNow();
			var daysSinceMonday = ((int)local.DayOfWeek + 6) % 7;    
			var thisMonday = local.Date.AddDays(-daysSinceMonday);

			DateTime ToUtc(DateTime localMidnight) =>
				TimeZoneInfo.ConvertTimeToUtc(
					DateTime.SpecifyKind(localMidnight, DateTimeKind.Unspecified), clock.LocalTimeZone);

			return (ToUtc(thisMonday), ToUtc(thisMonday.AddDays(7)), ToUtc(thisMonday.AddDays(-7)));
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