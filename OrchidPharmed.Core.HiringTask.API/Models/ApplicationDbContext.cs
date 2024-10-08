using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace OrchidPharmed.Core.HiringTask.API.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Models.ProjectEntity> Projects { get; set; }
        public DbSet<Models.TaskEntity> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectEntity>().
                HasMany(p => p.Tasks)
                 .WithOne(t => t.Project)
                  .HasForeignKey(t => t.ProjectId);

            modelBuilder.Entity<TaskEntity>()
                .Property(t => t.Status)
                 .HasConversion<string>();
        }

    }
}
