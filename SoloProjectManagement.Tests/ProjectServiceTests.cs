using SoloProjectManagement.Tests.Helper;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Dtos;
using SoloProjectManagement.UI.Features.Projects.Services;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.Tests
{
	public class ProjectServiceTests
	{
		private readonly SqliteTestDb _db;
		private static ProjectService CreateService(SqliteTestDb db) => new(db.ConnectionString);
		public ProjectServiceTests()
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
		public async Task GetAll_WithOneProject_ReturnsCorrectMapping()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var project = new Project
			{
				Category = ProjectCategory.Desktop,
				Title = "Project Management",
				Status = ProjectStatus.Active,
			};

			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null);

			var dto = new ProjectDto(
				Id: project.Id,
				Category: project.Category,
				Title: project.Title,
				Description: project.Description ?? "No description",
				Status: project.Status,
				DueDate: project.DueDate,
				TotalTasks: 0,
				CompletedTasks: 0,
				MarkedTask: "No active task"
			);

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(dto, result.Items.First());
		}

		[Fact]
		public async Task GetAll_NoFiltersApplied_ReturnsAllProjects()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var projects = Faker.CreateProject().Generate(5);

			context.Projects.AddRange(projects);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null);
			Assert.NotNull(result);
			Assert.Equal(5, result.Items.Count);
		}

		[Fact]
		public async Task GetAll_SearchQueryApplied_ReturnsMatchedProjects()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var projectManagement = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Project Management")
				.Generate();

			var financeApp = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Finance App")
				.Generate();

			var fitnessApp = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Fitness App")
				.Generate();

			context.Projects.AddRange(projectManagement, financeApp, fitnessApp);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new ProjectsFilterRequest("Project", null));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(projectManagement.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_SearchQueryApplied_WithWhitespace_ReturnsMatchedProjects()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var projectManagement = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Project Management")
				.Generate();

			var financeApp = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Finance App")
				.Generate();

			var fitnessApp = Faker.CreateProject()
				.RuleFor(p => p.Title, f => "Fitness App")
				.Generate();

			context.Projects.AddRange(projectManagement, financeApp, fitnessApp);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(new ProjectsFilterRequest("  Project  ", null));

			Assert.NotNull(result);
			Assert.Single(result.Items);
			Assert.Equal(projectManagement.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_StatusFilterApplied_ReturnMatchedProjects()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

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

			var result = await service.GetAllAsync(new ProjectsFilterRequest(null, ProjectStatus.Active));

			Assert.NotNull(result);
			Assert.Equal(2, result.TotalCount);
			Assert.Contains(result.Items, p => p.Status == ProjectStatus.Active);
		}

		[Fact]
		public async Task GetAll_CombinedFilter_ReturnMatchedProjects()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

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

			var result = await service.GetAllAsync(new ProjectsFilterRequest("Project", ProjectStatus.Active));

			Assert.NotNull(result);
			Assert.Equal(1, result.TotalCount);
			Assert.Equal(projectManagement.Id, result.Items.First().Id);
		}

		[Fact]
		public async Task GetAll_Pagination_ReturnsCorrectPage()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);
			var projects = Faker.CreateProject().Generate(10);

			context.Projects.AddRange(projects);
			await context.SaveChangesAsync();

			var result = await service.GetAllAsync(null);

			Assert.NotNull(result);
			Assert.Equal(10, result.TotalCount);
			Assert.Equal(6, result.Items.Count);
		}

		[Fact]
		public async Task Create_WithValidData_CreatedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var request = new CreateProjectRequest(
				Title: "Project Management",
				Description: null,
				Category: ProjectCategory.Desktop,
				Status: ProjectStatus.Active,
				DueDate: null
			);

			var result = await service.CreateAsync(request);

			Assert.Equal("Project Management", result.Title);
		}

		[Fact]
		public async Task Delete_ExistingProject_DeletedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var project = Faker.CreateProject()
				.RuleFor(p => p.Id, f => Guid.Parse("13D01AB1-B58B-4D0C-9E8E-6ECF1CE82A30"))
				.RuleFor(p => p.Title, f => "Project Management")
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.Generate();

			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var result = await service.DeleteAsync(project.Id);

			Assert.True(result);
		}

		[Fact]
		public async Task Delete_NonExistingProject_DeletedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var result = await service.DeleteAsync(Guid.NewGuid());

			Assert.False(result);
		}

		[Fact]
		public async Task GetById_ExistingProject_ReturnsProject()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var project = Faker.CreateProject()
				.RuleFor(p => p.Id, f => Guid.Parse("13D01AB1-B58B-4D0C-9E8E-6ECF1CE82A30"))
				.RuleFor(p => p.Title, f => "Project Management")
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.Generate();

			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var result = await service.GetByIdAsync(project.Id);

			Assert.NotNull(result);
			Assert.Equal(project.Id, result.Id);
		}

		[Fact]
		public async Task GetById_NonExistingProject_ReturnsNull()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var result = await service.GetByIdAsync(Guid.NewGuid());

			Assert.Null(result);
		}

		[Fact]
		public async Task Update_WithValidData_CreatedSuccessfully()
		{
			using var context = _db.CreateContext();
			await context.Database.EnsureCreatedAsync();

			var service = CreateService(_db);

			var project = Faker.CreateProject()
				.RuleFor(p => p.Id, f => Guid.Parse("13D01AB1-B58B-4D0C-9E8E-6ECF1CE82A30"))
				.RuleFor(p => p.Title, f => "Project Management")
				.RuleFor(p => p.Status, f => ProjectStatus.Active)
				.Generate();

			context.Projects.Add(project);
			await context.SaveChangesAsync();

			var request = new UpdateProjectRequest(
				Id: project.Id,
				Title: "Updated Project Management",
				Description: null,
				Category: ProjectCategory.Desktop,
				Status: ProjectStatus.Active,
				DueDate: null
			);

			var result = await service.UpdateAsync(request);

			Assert.True(result);
		}
	}
}
