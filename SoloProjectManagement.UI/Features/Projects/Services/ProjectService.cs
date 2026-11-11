using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.UI.Features.Projects.Services
{
	public class ProjectService(string connectionString) : IProjectService
	{
		public async Task<Project> CreateAsync(
			CreateProjectRequest request,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = new Project
			{
				Category = request.Category,
				Title = request.Title,
				Description = request.Description,
				Status = request.Status,
				DueDate = request.DueDate
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

		public async Task<PagedResult<ProjectDto>> GetAllAsync(
			ProjectsFilterRequest? request,
			int pageNumber = 1,
			int pageSize = 6,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var query = db.Projects.AsNoTracking();

			if (request != null)
			{
				if (!string.IsNullOrEmpty(request.SearchQuery))
				{
					string search = request.SearchQuery.Trim();
					query = query.Where(p => p.Title.Contains(search));
				}

				if (request.Status.HasValue && request.Status is ProjectStatus status)
				{
					query = query.Where(p => p.Status == status);
				}
			}

			var projects = await query
				.Include(p => p.Tasks)
				.OrderBy(p => p.Id)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(ct);

			var items = projects.Select(p => p.ToDto()).ToList();

			var totalCount = await query.CountAsync(ct);

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
			UpdateProjectRequest request,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var project = await GetByIdAsync(request.Id, ct);

			if (project is null) return false;

			project.Title = request.Title;
			project.Description = request.Description;
			project.Category = request.Category;
			project.Status = request.Status;
			project.DueDate = request.DueDate;
			project.UpdatedAt = DateTime.UtcNow;

			db.Projects.Update(project);
			await db.SaveChangesAsync(ct);
			return true;
		}
	}
}
