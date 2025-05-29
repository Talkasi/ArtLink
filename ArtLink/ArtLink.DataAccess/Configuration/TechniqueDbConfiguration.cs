using ArtLink.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtLink.DataAccess.Configuration;

public class TechniqueDbConfiguration : IEntityTypeConfiguration<TechniqueDb>
{
    public void Configure(EntityTypeBuilder<TechniqueDb> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasMany(t => t.Portfolios)
            .WithOne(p => p.Technique)
            .HasForeignKey(p => p.TechniqueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
