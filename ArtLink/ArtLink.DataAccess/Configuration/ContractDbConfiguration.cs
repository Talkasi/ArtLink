using ArtLink.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtLink.DataAccess.Configuration;

public class ContractDbConfiguration : IEntityTypeConfiguration<ContractDb>
{
    public void Configure(EntityTypeBuilder<ContractDb> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.ArtistId)
            .IsRequired();

        builder.Property(c => c.EmployerId)
            .IsRequired();

        builder.Property(c => c.ProjectDescription)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(c => c.Artist)
            .WithMany()
            .HasForeignKey(c => c.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Employer)
            .WithMany()
            .HasForeignKey(c => c.EmployerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

