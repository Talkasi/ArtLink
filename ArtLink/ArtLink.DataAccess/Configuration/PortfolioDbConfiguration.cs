using ArtLink.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtLink.DataAccess.Configuration;

public class PortfolioDbConfiguration : IEntityTypeConfiguration<PortfolioDb>
{
    public void Configure(EntityTypeBuilder<PortfolioDb> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(p => p.Artist)
            .WithMany(a => a.Portfolios)
            .HasForeignKey(p => p.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Technique)
            .WithMany()
            .HasForeignKey(p => p.TechniqueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Artworks)
            .WithOne(a => a.Portfolio)
            .HasForeignKey(a => a.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
