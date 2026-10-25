namespace SoloProjectManagement.UI.Models
{
	public sealed class ActivityLog
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public ActivityEntityType EntityType { get; set; }
		public ActivityActionType ActionType { get; set; }
		public Guid? ProjectId { get; set; }
		public Guid? ProjectTaskId { get; set; }
	}
}
