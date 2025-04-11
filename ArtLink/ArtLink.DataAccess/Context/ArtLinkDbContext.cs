using ArtLink.DataAccess.Configuration;
using ArtLink.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.DataAccess.Context;

public class ArtLinkDbContext : DbContext
{
    public DbSet<ArtistDb> Artists { get; set; }

    public DbSet<ArtworkDb> Artworks { get; set; }

    public DbSet<ContractDb> Contracts { get; set; }

    public DbSet<EmployerDb> Employers { get; set; }

    public DbSet<PortfolioDb> Portfolios { get; set; }

    public DbSet<TechniqueDb> Techniques { get; set; }

    public ArtLinkDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ArtistDbConfiguration());
        builder.ApplyConfiguration(new ArtworkDbConfiguration());
        builder.ApplyConfiguration(new ContractDbConfiguration());
        builder.ApplyConfiguration(new EmployerDbConfiguration());
        builder.ApplyConfiguration(new PortfolioDbConfiguration());
        builder.ApplyConfiguration(new TechniqueDbConfiguration());
    }
}

