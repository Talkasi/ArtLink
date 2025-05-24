using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Extensions;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Models.Enums;
using ArtLink.Services.Artist;
using ArtLink.Services.Artwork;
using ArtLink.Services.Contract;
using ArtLink.Services.Employer;
using ArtLink.Services.Portfolio;
using ArtLink.Services.Search;
using ArtLink.Services.Technique;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ArtLink.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        try
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            
            Log.Information("Log started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logger error: {ex.Message}");
            Environment.Exit(1);
        }
        
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("ArtistOnly", policy => policy.RequireClaim("Role", 
                Roles.RoleNames[(int)RolesEnum.Artist], Roles.RoleNames[(int)RolesEnum.Admin]))
            .AddPolicy("EmployerOnly", policy => policy.RequireClaim("Role", 
                Roles.RoleNames[(int)RolesEnum.Employer], Roles.RoleNames[(int)RolesEnum.Admin]))
            .AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", Roles.RoleNames[(int)RolesEnum.Admin]))
            .AddPolicy("AuthorizedOnly", policy => policy.RequireClaim("Role", 
                Roles.RoleNames[(int)RolesEnum.Admin], Roles.RoleNames[(int)RolesEnum.Employer],Roles.RoleNames[(int)RolesEnum.Artist]
                ));
        
        builder.Host.UseSerilog();

        builder.Services.AddCors();
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    []
                }
            });
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ArtLinkDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.Migrate<ArtLinkDbContext>();

        builder.Services.AddTransient<IArtworkRepository, ArtworkRepository>();
        builder.Services.AddTransient<IArtistRepository, ArtistRepository>();
        builder.Services.AddTransient<IContractRepository, ContractRepository>();
        builder.Services.AddTransient<IEmployerRepository, EmployerRepository>();
        builder.Services.AddTransient<IPortfolioRepository, PortfolioRepository>();
        builder.Services.AddTransient<ITechniqueRepository, TechniqueRepository>();

        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddTransient<IArtistService, ArtistService>();
        builder.Services.AddTransient<IArtworkService, ArtworkService>();
        builder.Services.AddTransient<IContractService, ContractService>();
        builder.Services.AddTransient<IEmployerService, EmployerService>();
        builder.Services.AddTransient<IPortfolioService, PortfolioService>();
        builder.Services.AddTransient<ISearchService, SearchService>();
        builder.Services.AddTransient<ITechniqueService, TechniqueService>();

        var app = builder.Build();

        app.UseHttpsRedirection();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseSwaggerUI();

        app.UseCors(b =>
        {
            b.AllowAnyOrigin();
            b.AllowAnyHeader();
            b.AllowAnyMethod();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapControllers();
        
        app.Run();
    }
}
