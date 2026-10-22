namespace SoloProjectManagement.Wpf.Models
{
	public sealed class ProjectTask
	{
		public Guid Id { get; set; }
		public Guid ProjectId { get; set; }
		public Project Project { get; set; } = null!;
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public TaskStatus Status { get; set; } = TaskStatus.Todo;
		public TaskPriority Priority { get; set; } = TaskPriority.Low;
		public DateTime DueDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
	}
}
