
using Microsoft.EntityFrameworkCore;
using ms.user.domain.Entities;

namespace ms.user.infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                    .IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.Username)
                    .IsUnique();

                entity.Property(u => u.AccountId)
                    .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(255);
                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
