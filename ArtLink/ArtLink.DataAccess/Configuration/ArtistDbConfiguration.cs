using ArtLink.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtLink.DataAccess.Configuration
{
    public class ArtistDbConfiguration : IEntityTypeConfiguration<ArtistDb>
    {
        public void Configure(EntityTypeBuilder<ArtistDb> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Bio)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(a => a.Experience)
                .IsRequired(false);

            builder.Property(a => a.ProfilePicturePath)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.HasMany(a => a.Portfolios)
                .WithOne()
                .HasForeignKey(p => p.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(a => a.Contracts)
                .WithOne()
                .HasForeignKey(p => p.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
