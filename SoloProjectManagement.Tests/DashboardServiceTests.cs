using Microsoft.Extensions.Time.Testing;
using SoloProjectManagement.Tests.Helper;
using SoloProjectManagement.UI.Features.Dashboard.Services;
using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.Tests
{
	public class DashboardServiceTests
	{
		private static DashboardService CreateService(SqliteTestDb db, TimeProvider? clock = null) => new(db.ConnectionString, clock);
		private static readonly DateTimeOffset Now = new(2026, 11, 11, 12, 0, 0, TimeSpan.Zero);
		private readonly SqliteTestDb _db;

		public DashboardServiceTests()
		{
			_db = new SqliteTestDb();
		}

		[Fact]
		public async Task GetOverview_With_EmptyDatabase_ReturnsAllZero()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();
			var service = CreateService(_db, FixedClock());

			var result = await service.GetOverviewAsync();

			Assert.NotNull(result);
			Assert.Equal(0, result.TotalProjects.Value);
			Assert.Equal(0, result.TotalTasks.Value);
			Assert.Equal(0, result.CompletedTasksThisWeek.Value);
			Assert.Equal(0, result.OverdueTasks.Value);
		}

		[Fact]
		public async Task GetOverview_WithProjectsAndTasks_ReturnsCorrectMetric()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var projectManagement = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Project Management")
				.Generate();

			var financeApp = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Finance App")
				.Generate();

			projectManagement.Tasks = new()
			{
				Faker.CreateTask(projectManagement.Id)
					.RuleFor(t => t.Name, f => "Design Database Schema")
					.RuleFor(t => t.Status, f => TaskStatus.Completed)
					.RuleFor(t => t.CreatedAt, f => new DateTime(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc))
					.Generate(),
				Faker.CreateTask(projectManagement.Id)
					.RuleFor(t => t.Name, f => "Design Architecture")
					.RuleFor(t => t.Status, f => TaskStatus.InProgress)
					.Generate()
			};

			financeApp.Tasks = new()
			{
				Faker.CreateTask(financeApp.Id)
					.RuleFor(t => t.Name, f => "Implement Authentication")
					.RuleFor(t => t.Status, f => TaskStatus.Overdue)
					.RuleFor(t => t.Priority, f => TaskPriority.High)
					.Generate(),
				Faker.CreateTask(financeApp.Id)
					.RuleFor(t => t.Name, f => "Implement Authorization")
					.RuleFor(t => t.Status, f => TaskStatus.Overdue)
					.RuleFor(t => t.Priority, f => TaskPriority.Medium)
					.Generate()
			};

			context.Projects.AddRange(projectManagement, financeApp);
			await context.SaveChangesAsync();

			var result = await service.GetOverviewAsync();

			Assert.NotNull(result);
			Assert.Equal(2, result.TotalProjects.Value);
			Assert.Equal(4, result.TotalTasks.Value);
			Assert.Equal(1, result.CompletedTasksThisWeek.Value);
			Assert.Equal(2, result.OverdueTasks.Value);
		}

		[Fact]
		public async Task GetOverview_WithProjects_ButNoTasks_ReturnsFiveTotalProjectMetric()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var projects = Faker.CreateProject().Generate(5);

			context.Projects.AddRange(projects);
			await context.SaveChangesAsync();

			var result = await service.GetOverviewAsync();

			Assert.NotNull(result);
			Assert.Equal(5, result.TotalProjects.Value);
			Assert.Equal(0, result.TotalTasks.Value);
			Assert.Equal(0, result.CompletedTasksThisWeek.Value);
			Assert.Equal(0, result.OverdueTasks.Value);
		}

		[Fact]
		public async Task GetRecentProjects_WithArchivedProjects_ReturnsProjectsExcludingArchived()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var projectManagement = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Project Management")
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.Generate();

			var financeApp = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Finance App")
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.Generate();

			var archivedProject = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Archived Project")
				.RuleFor(p => p.Status, f => ProjectStatus.Archived)
				.Generate();

			context.Projects.AddRange(projectManagement, financeApp, archivedProject);
			await context.SaveChangesAsync();

			var result = await service.GetRecentProjectsAsync();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.DoesNotContain(result, p => p.Status == ProjectStatus.Archived);
		}

		[Fact]
		public async Task GetRecentProjects_WithSixProjects_ExcludingArchived_ReturnsThreeProjects()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var activeProjects = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.Generate(3);

			var notStartedProjects = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.NotStarted)
				.Generate(2);

			var archivedProject = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Archived Project")
				.RuleFor(p => p.Status, f => ProjectStatus.Archived)
				.Generate();
			
			context.Projects.AddRange([..activeProjects, ..notStartedProjects, archivedProject]);
			await context.SaveChangesAsync();

			var result = await service.GetRecentProjectsAsync();

			Assert.NotNull(result);
			Assert.Equal(3, result.Count);
			Assert.DoesNotContain(result, p => p.Status == ProjectStatus.Archived);
		}

		[Fact]
		public async Task GetRecentProjects_WithActiveProjects_ReturnsCorrectOrdering()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var projectOne = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.RuleFor(p => p.CreatedAt, f => new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc))
				.RuleFor(p => p.UpdatedAt, f => null)
				.Generate();

			var projectTwo = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.RuleFor(p => p.CreatedAt, f => new DateTime(2026, 11, 5, 12, 0, 0, DateTimeKind.Utc))
				.RuleFor(p => p.UpdatedAt, f => null)
				.Generate();

			var projectThree = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.RuleFor(p => p.CreatedAt, f => new DateTime(2026, 11, 7, 12, 0, 0, DateTimeKind.Utc))
				.RuleFor(p => p.UpdatedAt, f => new DateTime(2026, 11, 8, 6, 0, 0, DateTimeKind.Utc))
				.Generate();

			var projectFour = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.RuleFor(p => p.CreatedAt, f => new DateTime(2026, 11, 9, 12, 0, 0, DateTimeKind.Utc))
				.RuleFor(p => p.UpdatedAt, f => null)
				.Generate();

			var projectFive = Faker.CreateProject()
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.RuleFor(p => p.CreatedAt, f => new DateTime(2026, 11, 9, 13, 0, 0, DateTimeKind.Utc)) // Changed from 12 to 13
				.RuleFor(p => p.UpdatedAt, f => null)
				.Generate();

			context.Projects.AddRange([projectOne, projectTwo, projectThree, projectFour, projectFive]);
			await context.SaveChangesAsync();

			var result = await service.GetRecentProjectsAsync();

			Assert.NotNull(result);
			Assert.Equal(3, result.Count);
			Assert.Equal(projectFive.Id, result[0].Id);
			Assert.Equal(projectFour.Id, result[1].Id);
			Assert.Equal(projectThree.Id, result[2].Id);
		}

		[Fact]
		public async Task GetRecentTasks_WithTasks_ExcludingCompletedTasks_ReturnsThreeTasks()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var project = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Project Management")
				.Generate();

			project.Tasks = new()
			{
				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Design Database Schema")
					.RuleFor(t => t.Status, f => TaskStatus.Completed)
					.Generate(),
				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Design Architecture")
					.RuleFor(t => t.Status, f => TaskStatus.InProgress)
					.Generate(),
				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Add WPF UI Project")
					.RuleFor(t => t.Status, f => TaskStatus.Todo)
					.Generate(),
				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Prepare Unit Testing Project")
					.RuleFor(t => t.Status, f => TaskStatus.Overdue )
					.Generate()
			};

			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var result = await service.GetRecentTasksAsync();

			Assert.NotNull(result);
			Assert.Equal(3, result.Count);
			Assert.DoesNotContain(result, t => t.Status == TaskStatus.Completed); 
		}

		[Fact]
		public async Task GetRecentTasks_WithTasks_ReturnsCorrectOrdering()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db, FixedClock());

			var project = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Project Management")
				.RuleFor(p => p.CreatedAt, f => new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc))
				.Generate();

			project.Tasks = new()
			{
				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Create Models")
					.RuleFor(t => t.Status, f => TaskStatus.InProgress)
					.RuleFor(t => t.Priority, f => TaskPriority.Medium)
					.RuleFor(t => t.DueDate, f => new DateTime(2026, 11, 6, 12, 0, 0, DateTimeKind.Utc))
					.Generate(),

				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Create Persistence")
					.RuleFor(t => t.Status, f => TaskStatus.Todo)
					.RuleFor(t => t.Priority, f => TaskPriority.High)
					.RuleFor(t => t.DueDate, f => new DateTime(2026, 11, 2, 12, 0, 0, DateTimeKind.Utc))
					.Generate(),

				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Design Database Schema")
					.RuleFor(t => t.Status, f => TaskStatus.Todo)
					.RuleFor(t => t.Priority, f => TaskPriority.High)
					.RuleFor(t => t.DueDate, f => new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc))
					.Generate(),

				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Create View Models")
					.RuleFor(t => t.Status, f => TaskStatus.InProgress)
					.RuleFor(t => t.Priority, f => TaskPriority.Low)
					.RuleFor(t => t.DueDate, f => new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc))
					.Generate(),

				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Design Architecture")
					.RuleFor(t => t.Status, f => TaskStatus.Todo)
					.RuleFor(t => t.Priority, f => TaskPriority.Medium)
					.RuleFor(t => t.DueDate, f => new DateTime(2026, 11, 5, 12, 0, 0, DateTimeKind.Utc))
					.Generate(),

				Faker.CreateTask(project.Id)
					.RuleFor(t => t.Name, f => "Add Navigation")
					.RuleFor(t => t.Status, f => TaskStatus.Todo)
					.RuleFor(t => t.Priority, f => TaskPriority.Low)
					.RuleFor(t => t.DueDate, f => new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc)) 
					.Generate()
			};

			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var result = await service.GetRecentTasksAsync();

			Assert.NotNull(result);
			Assert.Equal(5, result.Count);
			Assert.Equal("Design Database Schema", result[0].Name);
			Assert.Equal("Create Persistence", result[1].Name);
			Assert.Equal("Design Architecture", result[2].Name);
			Assert.Equal("Create Models", result[3].Name);
			Assert.Equal("Create View Models", result[4].Name);
			Assert.DoesNotContain(result, t => t.Name == "Add Navigation");
		}

		private static FakeTimeProvider FixedClock()
		{
			var clock = new FakeTimeProvider(Now);
			clock.SetLocalTimeZone(TimeZoneInfo.Utc);
			return clock;
		} 
	}
}
