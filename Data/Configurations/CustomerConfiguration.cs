using CustomerApplication.CustomerApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.NationalID)
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(x => x.Salary)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.Gender)
            .WithMany()
            .HasForeignKey(x => x.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Governorate)
            .WithMany()
            .HasForeignKey(x => x.GovernorateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.District)
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Village)
            .WithMany()
            .HasForeignKey(x => x.VillageId)
            .OnDelete(DeleteBehavior.Restrict);

       

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.NationalID)
            .IsUnique();
    }
}
