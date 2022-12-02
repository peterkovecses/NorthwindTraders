using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Infrastructure.Persistence.Services
{
    public static class MigratorAppBuilderExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindContext>();
            var migrations = dbContext.Database.GetMigrations().ToHashSet();
            if (dbContext.Database.GetAppliedMigrations().Any(a => !migrations.Contains(a)))
                throw new InvalidOperationException(
                    "There is already a migration running on the database that has since been deleted from the project. Delete the database or fix the status of the migrations and then restart the application.");
            dbContext.Database.Migrate();

            return app;
        }
    }
}
