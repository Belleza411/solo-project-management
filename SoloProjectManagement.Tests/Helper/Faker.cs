using Bogus;
using SoloProjectManagement.UI.Models;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.Tests.Helper
{
	public static class Faker
	{
		public static Faker<Project> CreateProject()
		{
			return new Faker<Project>()
				.RuleFor(p => p.Id, f => Guid.NewGuid())
				.RuleFor(p => p.Category, f => f.PickRandom<ProjectCategory>())
				.RuleFor(p => p.Title, f => f.Lorem.Sentence())
				.RuleFor(p => p.Description, f => f.Lorem.Paragraph())
				.RuleFor(p => p.Status, f => f.PickRandom<ProjectStatus>())
				.RuleFor(p => p.DueDate, f => f.Date.Future())
				.RuleFor(p => p.CreatedAt, f => f.Date.Past())
				.RuleFor(p => p.UpdatedAt, f => f.Date.Recent())
				.RuleFor(p => p.Tasks, f => []);
		}

		public static Faker<ProjectTask> CreateTask(Guid projectId)
		{
			return new Faker<ProjectTask>()
				.RuleFor(t => t.Id, f => Guid.NewGuid())
				.RuleFor(t => t.ProjectId, f => projectId)
				.RuleFor(t => t.Name, f => f.Lorem.Sentence())
				.RuleFor(t => t.Description, f => f.Lorem.Paragraph())
				.RuleFor(t => t.Status, f => f.PickRandom<TaskStatus>())
				.RuleFor(t => t.Priority, f => f.PickRandom<TaskPriority>())
				.RuleFor(t => t.DueDate, f => f.Date.Future())
				.RuleFor(t => t.CreatedAt, f => f.Date.Past())
				.RuleFor(t => t.UpdatedAt, f => f.Date.Recent());
		}
	}
}
