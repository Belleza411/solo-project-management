using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Persistence
{
	public class ProjectDbContext(string connectionString) : DbContext
	{
		public DbSet<Project> Projects => Set<Project>();
		public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
		public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

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

			modelBuilder.Entity<ActivityLog>(entity =>
			{
				entity.HasKey(l => l.Id);

				entity.HasIndex(l => l.CreatedAt);
				entity.HasIndex(l => l.ProjectId);
			});
		}
	}
}
