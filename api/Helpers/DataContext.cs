using Microsoft.EntityFrameworkCore;
using WebApi.Entities.Accounts;
using WebApi.Entities.Product;
using WebApi.Entities.Shared;
using WebApi.Entities.Site;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(x => 
            {
                x.ToTable("Accounts").HasKey(k => k.Id);
                x.HasQueryFilter(x => !x.IsDeleted);
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

            modelBuilder.Entity<Site>(x =>
            {
                x.ToTable("Sites").HasKey(k => k.Id);
                x.HasQueryFilter(x => !x.IsDeleted);
                x.OwnsOne(p => p.Address, p =>
                {
                    p.Property(pp => pp.FirstLine).HasColumnName("AddressFirstLine");
                    p.Property(pp => pp.SecondLine).HasColumnName("AddressSecondLine");
                });
            });

            modelBuilder.Entity<Product>(x =>
            {
                x.ToTable("Products").HasKey(k => k.Id);
                x.HasQueryFilter(x => !x.IsDeleted);
                x.HasIndex(p => p.Name).IsUnique();
                x.OwnsOne(p => p.Price, p =>
                {
                    p.Property(pp => pp.Value).HasColumnName("Price");
                    p.Property(pp => pp.Currency).HasColumnName("Currency");
                });
            });

            modelBuilder.Entity<Order>(x =>
            {
                x.ToTable("Orders").HasKey(k => k.Id);
                x.HasQueryFilter(x => !x.IsDeleted);
                x.OwnsOne(p => p.Name, p =>
                {
                    p.Property(pp => pp.FirstName).HasColumnName(nameof(Name.FirstName));
                    p.Property(pp => pp.LastName).HasColumnName(nameof(Name.LastName));
                });
                x.OwnsOne(p => p.PhoneNumber, p =>
                {
                    p.Property(pp => pp.Value).HasColumnName("PhoneNumber");
                    p.Property(pp => pp.CountryCallingCode).HasColumnName(nameof(PhoneNumber.CountryCallingCode));
                });
                x.Property(p => p.Email)
                    .HasConversion(p => p.Value, p => Email.Create(p).Value);
            });

            modelBuilder.Entity<OrderItem>(x =>
            {
                x.ToTable("OrderItems").HasKey(k => k.Id);
                x.HasQueryFilter(x => !x.IsDeleted);
                x.HasOne(x => x.Product).WithMany();
                x.HasOne(x => x.Order)
                    .WithMany(x => x.OrderItems)
                    .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}