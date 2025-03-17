using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ms.auth.domain.Entities;

namespace ms.auth.infrastructure.Data
{
    public class AuthAccountsDBContext(DbContextOptions<AuthAccountsDBContext> options) : DbContext(options)
    {
        public DbSet<AuthenticationAccount> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthenticationAccount>(entity =>
            {
                entity.ToTable("accounts");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                    .IsRequired().HasMaxLength(50);

                entity.HasIndex(u => u.Username)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasConversion(new EnumToStringConverter<Roles>())
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasDefaultValue(Roles.USER)
                    .HasSentinel(Roles.UNKNOWN);

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
