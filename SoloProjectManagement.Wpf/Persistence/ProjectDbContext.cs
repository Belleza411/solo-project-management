using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.Desktop.Models;

namespace SoloProjectManagement.Desktop.Persistence
{
	public class ProjectDbContext(string connectionString) : DbContext
	{
		public DbSet<Project> Projects { get; set; }
		public DbSet<ProjectTask> ProjectTasks { get; set;}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlite(connectionString);
		}
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
				entity.HasKey(p => p.Id);

				entity
					.HasOne(t => t.Project)
					.WithMany(t => t.Tasks)
					.HasForeignKey(t => t.ProjectId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
