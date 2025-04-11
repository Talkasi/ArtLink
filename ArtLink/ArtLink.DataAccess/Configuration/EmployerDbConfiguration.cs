using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArtLink.DataAccess.Models;

namespace ArtLink.DataAccess.Configuration;

public class EmployerDbConfiguration : IEntityTypeConfiguration<EmployerDb>
{
    public void Configure(EntityTypeBuilder<EmployerDb> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.PasswordHash)
            .IsRequired();

        builder.Property(e => e.CpFirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.CpLastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(e => e.Contracts)
            .WithOne(c => c.Employer)
            .HasForeignKey(c => c.EmployerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

