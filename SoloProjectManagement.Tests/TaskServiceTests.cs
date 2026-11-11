using SoloProjectManagement.Tests.Helper;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Tasks.Dto;
using SoloProjectManagement.UI.Features.Tasks.Services;
using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.Tests
{
	public class TaskServiceTests
	{
		private readonly SqliteTestDb _db;
		private static TaskService CreateService(SqliteTestDb db) => new(db.ConnectionString);

		public TaskServiceTests()
		{
			_db = new SqliteTestDb();
		}

		[Fact]
		public async Task GetAll_With_Empty_Database_ReturnsEmptyList()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var result = await service.GetAllAsync(null);

			Assert.NotNull(result);
			Assert.Empty(result.Items);
			Assert.Equal(0, result.TotalCount);
		}

		[Fact]
		public async Task GetAll_WithOneTask_ReturnsCorrectMapping()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var task = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Design Database Schema")
				.RuleFor(t => t.Description, f => null)
				.RuleFor(t => t.Status, f => TaskStatus.InProgress)
				.RuleFor(t => t.Priority, f => TaskPriority.High)
				.Generate();

			context.Projects.Add(project);
			context.ProjectTasks.Add(task);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null);

			Assert.NotNull(result);
			Assert.Single(result.Items)

			var dto = result.Items.First();

			Assert.Equal(task.Id, dto.Id);
			Assert.Equal(task.ProjectId, dto.ProjectId);
			Assert.Equal(task.Name, dto.Name);
			Assert.Equal("No description", dto.Description);
			Assert.Equal(task.Status, dto.Status);
			Assert.Equal(task.Priority, dto.Priority);
			Assert.Equal(task.DueDate, dto.DueDate);
		}

		[Fact]
		public async Task GetAll_NoFiltersApplied_ReturnsAllTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var tasks = Faker.CreateTask(project.Id).Generate(5);

			context.Projects.Add(project);
			context.ProjectTasks.AddRange(tasks);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null);

			Assert.NotNull(result);
			Assert.Equal(5, result.Items.Count);
		}

		[Fact]
		public async Task GetAll_ProjectFilterApplied_ReturnsMatchedTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var projectOne = Faker.CreateProject().Generate();
			var projectTwo = Faker.CreateProject().Generate();
			var matchingTask = Faker.CreateTask(projectOne.Id).Generate();
			var otherTask = Faker.CreateTask(projectTwo.Id).Generate();

			context.Projects.AddRange(projectOne, projectTwo);
			context.ProjectTasks.AddRange(matchingTask, otherTask);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new TaskFilterRequest(projectOne.Id, null, null, null));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(matchingTask.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_SearchQueryApplied_WithWhitespace_ReturnsMatchedTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var matchingTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Design Database Schema")
				.Generate();
			var otherTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Implement Authentication")
				.Generate();

			context.Projects.Add(project);
			context.ProjectTasks.AddRange(matchingTask, otherTask);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new TaskFilterRequest(null, "  Database  ", null, null));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(matchingTask.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_StatusFilterApplied_ReturnsMatchedTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var inProgressTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Status, f => TaskStatus.InProgress)
				.Generate();
			var todoTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Status, f => TaskStatus.Todo)
				.Generate();

			context.Projects.Add(project);
			context.ProjectTasks.AddRange(inProgressTask, todoTask);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new TaskFilterRequest(null, null, TaskStatus.InProgress, null));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(inProgressTask.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_PriorityFilterApplied_ReturnsMatchedTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var highPriorityTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Priority, f => TaskPriority.High)
				.Generate();
			var lowPriorityTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Priority, f => TaskPriority.Low)
				.Generate();

			context.Projects.Add(project);
			context.ProjectTasks.AddRange(highPriorityTask, lowPriorityTask);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new TaskFilterRequest(null, null, null, TaskPriority.High));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(highPriorityTask.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_CombinedFilter_ReturnsMatchedTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var matchingTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Design Database Schema")
				.RuleFor(t => t.Status, f => TaskStatus.InProgress)
				.RuleFor(t => t.Priority, f => TaskPriority.High)
				.Generate();
			var wrongStatusTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Design Database Migration")
				.RuleFor(t => t.Status, f => TaskStatus.Todo)
				.RuleFor(t => t.Priority, f => TaskPriority.High)
				.Generate();

			context.Projects.Add(project);
			context.ProjectTasks.AddRange(matchingTask, wrongStatusTask);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new TaskFilterRequest(project.Id, "Database", TaskStatus.InProgress, TaskPriority.High));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(matchingTask.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_Pagination_ReturnsCorrectPage()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var tasks = Faker.CreateTask(project.Id).Generate(10);

			context.Projects.Add(project);
			context.ProjectTasks.AddRange(tasks);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null);

			Assert.NotNull(result);
			Assert.Equal(10, result.TotalCount);
			Assert.Equal(10, result.Items.Count);
		}

		[Fact]
		public async Task GetAll_Pagination_WithPageNumberOfTwo_ReturnsCorrectPage()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var tasks = Faker.CreateTask(project.Id).Generate(6);
			var lastTask = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Last Task")
				.Generate();

			context.Projects.Add(project);
			context.ProjectTasks.AddRange([..tasks, lastTask]);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null, 2, 6);

			Assert.NotNull(result);
			Assert.Equal(7, result.TotalCount);
			Assert.Single(result.Items);
			Assert.Contains(result.Items.First().Id, tasks.Append(lastTask).Select(t => t.Id));
		}

		[Fact]
		public async Task Create_WithValidData_CreatedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var request = new CreateTaskRequest(
				ProjectId: project.Id,
				Name: "Design Database Schema",
				Description: null!,
				Status: TaskStatus.Todo,
				Priority: TaskPriority.High,
				DueDate: null);

			var result = await service.CreateAsync(request);

			Assert.Equal("Design Database Schema", result.Name);
			Assert.Equal(project.Id, result.ProjectId);
		}

		[Fact]
		public async Task Delete_ExistingTask_DeletedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var task = Faker.CreateTask(project.Id).Generate();
			context.Projects.Add(project);
			context.ProjectTasks.Add(task);
			await context.SaveChangesAsync();

			var result = await service.DeleteAsync(task.Id);

			Assert.True(result);
		}

		[Fact]
		public async Task Delete_NonExistingTask_ReturnsFalse()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var result = await service.DeleteAsync(Guid.NewGuid());

			Assert.False(result);
		}

		[Fact]
		public async Task GetById_ExistingTask_ReturnsTask()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var task = Faker.CreateTask(project.Id).Generate();
			context.Projects.Add(project);
			context.ProjectTasks.Add(task);
			await context.SaveChangesAsync();

			var result = await service.GetByIdAsync(task.Id);

			Assert.NotNull(result);
			Assert.Equal(task.Id, result.Id);
		}

		[Fact]
		public async Task GetById_NonExistingTask_ReturnsNull()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var result = await service.GetByIdAsync(Guid.NewGuid());

			Assert.Null(result);
		}

		[Fact]
		public async Task Update_WithValidData_UpdatedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var project = Faker.CreateProject().Generate();
			var task = Faker.CreateTask(project.Id)
				.RuleFor(t => t.Name, f => "Original Task")
				.Generate();
			context.Projects.Add(project);
			context.ProjectTasks.Add(task);
			await context.SaveChangesAsync();

			var request = new UpdateTaskRequest(
				ProjectId: project.Id,
				Name: "Updated Task",
				Description: "Updated description",
				Status: TaskStatus.Completed,
				Priority: TaskPriority.Low,
				DueDate: null);

			var result = await service.UpdateAsync(task.Id, request);

			Assert.True(result);
		}
	}
}
