using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace CustomerApplication.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.NationalID, "UQ_Customers_NationalID")
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0"); 

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Age)
                      .HasComputedColumnSql(
                          "(datediff(year,[BirthDate],getdate())-case when dateadd(year,datediff(year,[BirthDate],getdate()),[BirthDate])>getdate() then (1) else (0) end)",
                          stored: false);

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.NationalID).HasMaxLength(14);
                entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerCreatedByNavigations)
                      .HasForeignKey(d => d.CreatedBy)
                      .HasConstraintName("FK_Customers_CreatedBy");

                entity.HasOne(d => d.District).WithMany(p => p.CustomerDistricts)
                      .HasForeignKey(d => d.DistrictId)
                      .HasConstraintName("FK_Customers_District");

                entity.HasOne(d => d.Gender).WithMany(p => p.CustomerGenders)
                      .HasForeignKey(d => d.GenderId)
                      .HasConstraintName("FK_Customers_Gender");

                entity.HasOne(d => d.Governorate).WithMany(p => p.CustomerGovernorates)
                      .HasForeignKey(d => d.GovernorateId)
                      .HasConstraintName("FK_Customers_Governorate");

                entity.HasOne(d => d.User).WithMany(p => p.CustomerUsers)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Customers_User");

                entity.HasOne(d => d.Village).WithMany(p => p.CustomerVillages)
                      .HasForeignKey(d => d.VillageId)
                      .HasConstraintName("FK_Customers_Village");
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CategoryCode).HasMaxLength(64);
                entity.Property(e => e.Code).HasMaxLength(64);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                      .HasForeignKey(d => d.ParentId)
                      .HasConstraintName("FK_Lookups_Parent");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0755442DE3");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.HasIndex(e => e.Name)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0708AD7EF7");

                entity.HasIndex(e => e.Username, "UQ__Users__536C85E425F1EEF8")
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0"); 

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Username).HasMaxLength(100);

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                      .HasForeignKey(d => d.RoleId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK__Users__RoleId__4CA06362");
            });

            ApplyGlobalSoftDeleteFilters(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        private void ApplyGlobalSoftDeleteFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                if (!typeof(ISoftDelete).IsAssignableFrom(clrType))
                    continue;

                var parameter = Expression.Parameter(clrType, "e");

                var isDeletedProp = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));

                Expression isDeletedBool;
                if (isDeletedProp.Type == typeof(bool?))
                {
                    isDeletedBool = Expression.Coalesce(
                        isDeletedProp,
                        Expression.Constant(false, typeof(bool))
                    );
                }
                else if (isDeletedProp.Type == typeof(bool))
                {
                    isDeletedBool = isDeletedProp;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Property '{nameof(ISoftDelete.IsDeleted)}' on '{clrType.Name}' must be bool or bool?.");
                }

                var body = Expression.Not(isDeletedBool);

                var funcType = typeof(Func<,>).MakeGenericType(clrType, typeof(bool));
                var lambda = Expression.Lambda(funcType, body, parameter);

                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
