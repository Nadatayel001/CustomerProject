using CustomerApplication.CustomerApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LookupConfiguration : IEntityTypeConfiguration<Lookup>
{
    public void Configure(EntityTypeBuilder<Lookup> builder)
    {
        builder.ToTable("Lookups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryCode)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
