using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.Wpf.Models;

namespace SoloProjectManagement.Wpf.Persistence
{
	public class ProjectDbContext : DbContext
	{
		public DbSet<Project> Projects { get; set; }
		public DbSet<ProjectTask> ProjectTasks { get; set;}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Project>(entity =>
			{
				entity.HasKey(p => p.Id);

				entity
					.HasMany(p => p.Tasks)
					.WithOne(t => t.Project)
					.OnDelete(DeleteBehavior.Cascade);
			});


			modelBuilder.Entity<ProjectTask>(entity =>
			{
				entity.HasKey(p => t.Id);

				entity
					.HasOne(t => t.Project)
					.WithMany(t => t.Tasks)
					.HasForeignKey(t => t.ProjectId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
