using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Dtos;
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
			ProjectStatus status,
			DateTime dueDate,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = new Project
			{
				Title = title,
				Description = description,
				Status = status,
				DueDate = dueDate
			};

			db.Projects.Add(project);
			await db.SaveChangesAsync(ct);

			return project;
		}

		public async Task<bool> DeleteAsync(Guid Id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(Id, ct);

			if (project is null) return false;

			db.Projects.Remove(project);
			await db.SaveChangesAsync(ct);

			return true;
		}

		public async Task<PagedResult<ProjectCardDto>> GetAllAsync(
			int pageNumber = 1, int pageSize = 6, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var totalCount = await db.Projects.CountAsync(ct);

			var rows = await db.Projects
				.AsNoTracking()
				.OrderBy(p => p.Title)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Select(p => new
				{
					p.Id,
					p.Category,
					p.Title,
					p.Description,
					p.Status,
					p.DueDate,
					TotalTasks = p.Tasks.Count(),
					CompletedTasks = p.Tasks.Count(t => t.Status == TaskStatus.Completed),
					CurrentTask = p.Tasks
						.Where(t => t.Status == TaskStatus.InProgress
								 && t.Priority == TaskPriority.High)
						.Select(t => t.Name)
						.FirstOrDefault()
				})
				.ToListAsync(ct);

			var items = rows.Select(r =>
			{
				var progressPercent = r.TotalTasks == 0
					? 0
					: (int)Math.Round(r.CompletedTasks * 100.0 / r.TotalTasks);

				return new ProjectCardDto(
					r.Id,
					r.Category,
					r.Title,
					r.Description ?? "",
					r.CurrentTask ?? "No task",
					r.Status,
					r.DueDate,
					$"{r.CompletedTasks} / {r.TotalTasks} tasks",
					progressPercent);
			}).ToList();

			return new PagedResult<ProjectCardDto>(items, pageNumber, pageSize, totalCount);
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
			DateTime dueDate,
			ProjectStatus status,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(id, ct);

			if (project is null) return false;

			project.Title = title;
			project.Description = description;
			project.Status = status;
			project.DueDate = dueDate;
			project.UpdatedAt = DateTime.UtcNow;

			await db.SaveChangesAsync(ct);
			return true;
		}
	}
}
