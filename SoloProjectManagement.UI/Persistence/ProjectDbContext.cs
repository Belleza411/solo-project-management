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

			var now = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);

			modelBuilder.Entity<Project>().HasData(
				new Project
				{
					Id = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
					Title = "E-Commerce Platform",
					Description = "Develop the core checkout flow and shopping cart service.",
					Category = ProjectCategory.Web,
					Status = ProjectStatus.Active,
					DueDate = now.AddMonths(2),
					CreatedAt = now.AddDays(-10),
					UpdatedAt = now.AddDays(-2)
				},
				new Project
				{
					Id = Guid.Parse("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"),
					Title = "Mobile Fitness Tracker",
					Description = "Build iOS and Android MVP for tracking user daily steps.",
					Category = ProjectCategory.Mobile,
					Status = ProjectStatus.NotStarted,
					DueDate = now.AddMonths(4),
					CreatedAt = now,
					UpdatedAt = null
				},
				new Project
				{
					Id = Guid.Parse("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"),
					Title = "CI/CD Pipeline Optimization",
					Description = "Migrate legacy build pipelines to GitHub Actions for faster builds.",
					Category = ProjectCategory.Other, 
					Status = ProjectStatus.Completed,
					DueDate = now.AddDays(5),
					CreatedAt = now.AddDays(-30),
					UpdatedAt = now.AddDays(-1)
				}
			);
		}
	}
}
