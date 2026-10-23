namespace SoloProjectManagement.UI.Models
{
	public sealed class Project
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
		public DateTime? UpdatedAt { get; set; }
		public List<ProjectTask> Tasks { get; set; } = [];
	}
}