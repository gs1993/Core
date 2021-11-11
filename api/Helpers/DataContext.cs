using Microsoft.EntityFrameworkCore;
using WebApi.Entities.Accounts;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(x => 
            {
                x.ToTable("Accounts").HasKey(k => k.Id);
                x.HasMany(p => p.RefreshTokens)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
                x.OwnsOne(p => p.Name, p =>
                {
                    p.Property(pp => pp.Title).HasColumnName(nameof(Name.Title));
                    p.Property(pp => pp.FirstName).HasColumnName(nameof(Name.FirstName));
                    p.Property(pp => pp.LastName).HasColumnName(nameof(Name.LastName));
                });
                x.OwnsOne(p => p.Password, p =>
                {
                    p.Property(pp => pp.PasswordHash).HasColumnName(nameof(Password.PasswordHash));
                    p.Property(pp => pp.PasswordSalt).HasColumnName(nameof(Password.PasswordSalt));
                });
                x.Property(p => p.Email)
                    .HasConversion(p => p.Value, p => Email.Create(p).Value);
            });

            modelBuilder.Entity<RefreshToken>(x => 
            {
                x.ToTable("RefreshTokens").HasKey(k => k.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}