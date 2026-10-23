using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.UI.Services
{
	public class ProjectService(string connectionString) : IProjectService
	{
		public async Task<Project> CreateAsync(string title, string? description, ProjectStatus status, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = new Project
			{
				Title = title,
				Description = description,
				Status = status
			};

			db.Projects.Add(project);
			await db.SaveChangesAsync(ct);

			return project;
		}

		public async Task<bool> DeleteAsync(Guid Id, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(Id, ct);

			if (project is null) return false;

			db.Projects.Remove(project);
			await db.SaveChangesAsync(ct);

			return true;
		}

		public async Task<PagedResult<Project>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var projects = await db.Projects
				.Include(p => p.Tasks)
				.AsNoTracking()
				.ToListAsync(ct);

			var totalCount = projects.Count;

			return new PagedResult<Project>(
				Items: projects,
				PageNumber: pageNumber ?? 1,
				PageSize: pageSize ?? 3,
				TotalCount: totalCount);
		}

		public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.Projects
				.Include(p => p.Tasks)
				.FirstOrDefaultAsync(p => p.Id == id, ct);
		}
		 
		public async Task<bool> UpdateAsync(Guid id, string title, string? description, ProjectStatus status, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(id, ct);

			if (project is null) return false;

			project.Title = title;
			project.Description = description;
			project.Status = status;
			project.UpdatedAt = DateTime.UtcNow;

			await db.SaveChangesAsync(ct);
			return true;
		}
	}
}
