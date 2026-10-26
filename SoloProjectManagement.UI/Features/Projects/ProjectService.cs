using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
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

		public async Task<PagedResult<ProjectDto>> GetAllAsync(int pageNumber = 1, int pageSize = 6, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var projects = await db.Projects
				.Include(p => p.Tasks)  
				.AsNoTracking()
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Select(p => new ProjectDto(
					Id: p.Id,
					Category: p.Category,
					Title: p.Title,
					Description: p.Description ?? "",
					Status: p.Status,
					DueDate: p.DueDate,
					CompletedTasks: p.Tasks.Count(t => t.Status == TaskStatus.Completed),
					TotalTasks: p.Tasks.Count,
					UpdatedAt: p.UpdatedAt))
				.ToListAsync(ct);
			 
			var totalCount = projects.Count;
			 
			return new PagedResult<ProjectDto>(
				Items: projects,
				PageNumber: pageNumber,
				PageSize: pageSize,
				TotalCount: totalCount);
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
