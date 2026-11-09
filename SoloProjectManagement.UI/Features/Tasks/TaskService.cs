using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.UI.Features.Tasks
{
	public class TaskService(string connectionString) : ITaskService
	{
		public async Task<ProjectTask> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = new ProjectTask
			{
				ProjectId = request.ProjectId,
				Name = request.Name,
				Description = request.Description,
				Status = request.Status,
				Priority = request.Priority,
				DueDate = request.DueDate
			};

			db.ProjectTasks.Add(task);
			await db.SaveChangesAsync(ct);

			return task;
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = await GetByIdAsync(id, ct);

			if (task is null) return false;

			db.ActivityLogs.Add(new ActivityLog
			{
				Name = $"You deleted task: {task.Name}",
				EntityType = ActivityEntityType.Task,
				ActionType = ActivityActionType.Deleted ,
				ProjectId = task.ProjectId,
				ProjectTaskId = task.Id
			});

			db.ProjectTasks.Remove(task);
			await db.SaveChangesAsync(ct);

			return true;
		}

		public async Task<PagedResult<ProjectTaskDto>> GetAllAsync(
			Guid? projectId,
			int pageNumber = 1,
			int pageSize = 10,
			CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var query = db.ProjectTasks.AsNoTracking();

			var totalCount = await query.CountAsync(ct);

			if (projectId.HasValue)
			{
				query = query.Where(t => t.ProjectId == projectId.Value);
			}

			var tasks = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Select(t => new ProjectTaskDto(
					Id: t.Id,
					ProjectId: t.ProjectId,
					Name: t.Name,
					Description: t.Description ?? "No description",
					Status: t.Status,
					Priority: t.Priority,
					DueDate: t.DueDate
					))
				.ToListAsync(ct);

			return new PagedResult<ProjectTaskDto>(
				Items: tasks,
				PageNumber: pageNumber,
				PageSize: pageSize,
				TotalCount: totalCount);
		}

		public async Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
				.FirstOrDefaultAsync(t => t.Id == id, ct);
		}

		public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = await GetByIdAsync(id, ct);

			if (task is null) return false;

			task.ProjectId = request.ProjectId;
			task.Name = request.Name;
			task.Description = request.Description;
			task.Status = request.Status;
			task.Priority = request.Priority;
			task.DueDate = request.DueDate;
			task.UpdatedAt = DateTime.UtcNow;

			await db.SaveChangesAsync(ct);
			return true;
		}
	}
}
