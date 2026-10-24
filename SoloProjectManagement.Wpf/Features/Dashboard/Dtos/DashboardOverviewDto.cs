namespace SoloProjectManagement.UI.Features.Dashboard.Dtos
{
	public sealed record DashboardOverviewDto(
		MetricCardDto TotalProjects,
		MetricCardDto TotalTasks,
		MetricCardDto CompletedTasksThisWeek,
		MetricCardDto OverdueTasks);
}
