using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data.Configurations;

public class AlchimaliaUserConfiguration : IEntityTypeConfiguration<AlchimaliaUser>
{
    public void Configure(EntityTypeBuilder<AlchimaliaUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Auth0Id).IsUnique();
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        // Configure Roles as PostgreSQL array
        builder.Property(x => x.Roles)
            .HasConversion(
                v => v.Select(r => (int)r).ToArray(),
                v => v.Select(r => (UserRole)r).ToList())
            .HasColumnType("integer[]");
        // Configure SelectedAgeGroupIds as PostgreSQL text array
        builder.Property(x => x.SelectedAgeGroupIds)
            .HasConversion(
                v => v == null || v.Count == 0 ? null : v.ToArray(),
                v => v == null ? null : v.ToList())
            .HasColumnType("text[]");
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Auth0Id).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Picture).HasMaxLength(512);
    }
}
