using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Projects
{
	public class ProjectService(string connectionString) : IProjectService
	{
		public async Task<Project> CreateAsync(
			string title,
			string? description,
			ProjectCategory category,
			ProjectStatus status,
			DateTime? dueDate,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = new Project
			{
				Category = category,
				Title = title,
				Description = description,
				Status = status,
				DueDate = dueDate
			};

			db.ActivityLogs.Add(new ActivityLog
			{
				Name = $"You added new project: {title}",
				EntityType = ActivityEntityType.Project,
				ActionType = ActivityActionType.Created,
				ProjectId = project.Id
			});

			db.Projects.Add(project);
			await db.SaveChangesAsync(ct);

			return project;
		}

		public async Task<bool> DeleteAsync(Guid Id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(Id, ct);

			if (project is null) return false;

			db.ActivityLogs.Add(new ActivityLog
			{
				Name = $"You deleted project: {project.Title}",
				EntityType = ActivityEntityType.Project,
				ActionType = ActivityActionType.Deleted,
				ProjectId = project.Id
			});

			db.Projects.Remove(project);
			await db.SaveChangesAsync(ct);

			return true;
		}

		public async Task<PagedResult<ProjectDto>> GetAllAsync(
			int pageNumber = 1, int pageSize = 6, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var totalCount = await db.Projects.CountAsync(ct);

			var items = await db.Projects
				.AsNoTracking()
				.OrderBy(p => p.Title)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Select(p => new ProjectDto(
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
						.FirstOrDefault() ?? "No active task"
				))
				.ToListAsync(ct);

			return new PagedResult<ProjectDto>(items, pageNumber, pageSize, totalCount);
		}

		public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.Projects
				.Include(p => p.Tasks)
				.FirstOrDefaultAsync(p => p.Id == id, ct);
		}

		public async Task<bool> UpdateAsync(
			Guid id,
			string title,
			string? description,
			ProjectCategory category,
			DateTime? dueDate,
			ProjectStatus status,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(id, ct);

			if (project is null) return false;

			project.Title = title;
			project.Description = description;
			project.Category = category;
			project.Status = status;
			project.DueDate = dueDate;
			project.UpdatedAt = DateTime.UtcNow;

			db.Projects.Update(project);
			await db.SaveChangesAsync(ct);
			return true;
		}
	}
}
