using Microsoft.EntityFrameworkCore;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Services.Artist;
using ArtLink.Services.Artwork;
using ArtLink.Services.Contract;
using ArtLink.Services.Employer;
using ArtLink.Services.Portfolio;
using ArtLink.Services.Search;
using ArtLink.DataAccess.Extensions;

namespace ArtLink.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors();

        // Add services to the container.

        builder.Services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();
        
        builder.Services.AddDbContext<ArtLinkDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
            ServiceLifetime.Transient,
            ServiceLifetime.Transient).Migrate<ArtLinkDbContext>();

        builder.Services.AddTransient<IArtworkRepository, ArtworkRepository>();
        builder.Services.AddTransient<IArtistRepository, ArtistRepository>();
        builder.Services.AddTransient<IContractRepository, ContractRepository>();
        builder.Services.AddTransient<IEmployerRepository, EmployerRepository>();
        builder.Services.AddTransient<IPortfolioRepository, PortfolioRepository>();
        
        builder.Services.AddTransient<IArtistService, ArtistService>();
        builder.Services.AddTransient<IArtworkService, ArtworkService>();
        builder.Services.AddTransient<IContractService, ContractService>();
        builder.Services.AddTransient<IEmployerService, EmployerService>();
        builder.Services.AddTransient<IPortfolioService, PortfolioService>();
        builder.Services.AddTransient<ISearchService, SearchService>();
        
        
        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseCors(b =>
        {
            b.AllowAnyOrigin();
            b.AllowAnyHeader();
            b.AllowAnyMethod();
        });

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}
