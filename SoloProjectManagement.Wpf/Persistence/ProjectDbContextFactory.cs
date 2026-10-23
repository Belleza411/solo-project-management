using Microsoft.EntityFrameworkCore.Design;

namespace SoloProjectManagement.UI.Persistence
{
	public class ProjectDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
	{
		public ProjectDbContext CreateDbContext(string[] args)
		{
			return new ProjectDbContext(DatabaseSettings.ConnectionString);
		}
	}
}
