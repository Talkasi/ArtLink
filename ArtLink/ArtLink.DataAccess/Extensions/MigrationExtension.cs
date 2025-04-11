using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArtLink.DataAccess.Extensions;

public static class MigrationExtension
{
    public static IServiceCollection Migrate<TContext>(this IServiceCollection serviceCollection) where TContext : DbContext
    {
        var context = serviceCollection.BuildServiceProvider().GetRequiredService<TContext>();
        context.Database.Migrate();

        return serviceCollection;
    }
}