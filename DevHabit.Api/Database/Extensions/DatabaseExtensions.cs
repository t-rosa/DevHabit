using DevHabit.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database.Extensions;

public static class DatabaseExtensions
{
    extension(WebApplication app)
    {
        public async Task ApplyMigrationsAsync()
        {
            using IServiceScope scope = app.Services.CreateScope();
            await using ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await using ApplicationIdentityDbContext identityDb = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

            try
            {
                await db.Database.MigrateAsync();
                app.Logger.LogInformation("Application database migrations applied successfully.");

                await identityDb.Database.MigrateAsync();
                app.Logger.LogInformation("Identityt database migrations applied successfully.");
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "An error occurred while applying database migrations.");
                throw;
            }

        }
    }
}
