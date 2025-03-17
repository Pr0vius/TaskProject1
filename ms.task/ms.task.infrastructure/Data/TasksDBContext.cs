
using Microsoft.EntityFrameworkCore;
using ms.task.domain.Entities;

namespace ms.task.infrastructure.Data
{
    public class TasksDBContext : DbContext
    {
        public TasksDBContext(DbContextOptions<TasksDBContext> options) : base(options) { }
        public DbSet<UserTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.ToTable("tasks");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.UserId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Name)
                    .IsRequired();

                entity.Property(u => u.Description);

                entity.Property(u => u.IsCompleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
