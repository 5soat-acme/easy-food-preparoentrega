using EF.PreparoEntrega.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Commons.Config;

public static class MigrationsConfig
{
    public static WebApplication RunMigrations(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();

            var preparoEntrega = scope.ServiceProvider.GetRequiredService<PreparoEntregaDbContext>();
            preparoEntrega.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return app;
    }
}