using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArtLink.DataAccess.Models;

namespace ArtLink.DataAccess.Configuration
{
    public class ArtworkDbConfiguration : IEntityTypeConfiguration<ArtworkDb>
    {
        public void Configure(EntityTypeBuilder<ArtworkDb> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.PortfolioId)
                .IsRequired();

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Description)
                .IsRequired(false)
                .HasMaxLength(2000);
            
            builder.Property(a => a.ImagePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(a => a.Portfolio)
                .WithMany(p => p.Artworks)
                .HasForeignKey(a => a.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
