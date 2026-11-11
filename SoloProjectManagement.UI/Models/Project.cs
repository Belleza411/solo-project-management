using System.ComponentModel.DataAnnotations;

namespace SoloProjectManagement.UI.Models
{
	public sealed class Project
	{
		public Guid Id { get; set; }
		public ProjectCategory Category { get; set; } = ProjectCategory.Other;
		[Required, MinLength(5), MaxLength(100)]
		public string Title { get; set; } = string.Empty;
		[MinLength(10), MaxLength(150)]
		public string? Description { get; set; }
		public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
		public DateTime? DueDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
		public DateTime? UpdatedAt { get; set; }
		public List<ProjectTask> Tasks { get; set; } = [];
	}
}