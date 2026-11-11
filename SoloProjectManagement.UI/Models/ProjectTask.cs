using System.ComponentModel.DataAnnotations;

namespace SoloProjectManagement.UI.Models
{
	public sealed class ProjectTask
	{
		public Guid Id { get; set; }
		public Guid ProjectId { get; set; }
		public Project Project { get; set; } = null!;
		[Required, MinLength(5), MaxLength(100)]
		public string Name { get; set; } = string.Empty;
		[MinLength(10), MaxLength(150)]
		public string? Description { get; set; }
		public TaskStatus Status { get; set; } = TaskStatus.Todo;
		public TaskPriority Priority { get; set; } = TaskPriority.Low;
		public DateTime? DueDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
	}
}
