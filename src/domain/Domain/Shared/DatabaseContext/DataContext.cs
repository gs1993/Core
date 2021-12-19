using Domain.Accounts.Entities;
using Domain.Orders.Entities;
using Domain.Products.Entities;
using Domain.Shared.ValueObjects;
using Domain.Sites.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Shared.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Shared.DatabaseContext
{
    public class DataContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public DbSet<Account> Accounts { get; protected set; }
        public DbSet<Site> Sites { get; protected set; }
        public DbSet<Product> Products { get; protected set; }
        public DbSet<Order> Orders { get; protected set; }

        public DataContext(DbContextOptions options, IDateTimeProvider dateTimeProvider) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }


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

                x.HasMany(x => x.OrderItems)
                    .WithOne(x => x.Order);
                x.Metadata.FindNavigation(nameof(Order.OrderItems)).SetPropertyAccessMode(PropertyAccessMode.Field);

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
                x.OwnsOne(p => p.FullPrice, p =>
                {
                    p.Property(pp => pp.Value).HasColumnName("Price");
                    p.Property(pp => pp.Currency).HasColumnName("Currency");
                });
                x.OwnsOne(p => p.OrderState, p =>
                {
                    p.Property(pp => pp.Value).HasColumnName("State");
                    p.Property(pp => pp.PaymentStartDate).HasColumnName(nameof(OrderState.PaymentStartDate));
                    p.Property(pp => pp.PaymentInProgressDate).HasColumnName(nameof(OrderState.PaymentInProgressDate));
                    p.Property(pp => pp.PaymentSuccededDate).HasColumnName(nameof(OrderState.PaymentSuccededDate));
                    p.Property(pp => pp.PaymentCanceledDate).HasColumnName(nameof(OrderState.PaymentCanceledDate));
                    p.Property(pp => pp.PaymentErrorDate).HasColumnName(nameof(OrderState.PaymentErrorDate));
                });
            });

            modelBuilder.Entity<OrderItem>(x =>
            {
                x.ToTable("OrderItems").HasKey(k => k.Id);
                x.HasQueryFilter(x => !x.IsDeleted);
                x.HasOne(x => x.Product).WithMany();
            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                    .AddConsole();
            });

            optionsBuilder
                .UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging();
        }

        #region InfrastructureMethods
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            throw new InvalidOperationException("Use attach to add new entity");
        }

        public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Use attach to add new entity");
        }

        public override EntityEntry Add(object entity)
        {
            throw new InvalidOperationException("Use attach to add new entity");
        }

        public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Use attach to add new entity");
        }

        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            (entity as BaseEntity).Delete(_dateTimeProvider.Now);
            return null;
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity
                   && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                    ((BaseEntity)entityEntry.Entity).SetCreateDate(_dateTimeProvider.Now);
                else if (entityEntry.State == EntityState.Modified)
                    ((BaseEntity)entityEntry.Entity).SetUpdateDate(_dateTimeProvider.Now);
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity
                   && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var dateNow = _dateTimeProvider.Now;
            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                    ((BaseEntity)entityEntry.Entity).SetCreateDate(dateNow);
                else if (entityEntry.State == EntityState.Modified)
                    ((BaseEntity)entityEntry.Entity).SetUpdateDate(dateNow);
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}