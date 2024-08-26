using System.Text.Json.Serialization;
using EF.Api.Commons.Extensions;
using EF.Api.Contexts.PreparoEntrega.Config;
using EF.PreparoEntrega.Application.Events.Messages;
using EF.WebApi.Commons.Identity;

namespace EF.Api.Commons.Config;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfig(env);

        services.AddEventBusConfig();

        services.RegisterServicesPreparoEntrega(configuration);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddJwtConfiguration(configuration);
        services.AddMessageriaConfig(configuration);
        services.AddHostedService<PedidoRecebidoConsumer>();

        return services;
    }

    public static WebApplication UseApiConfig(this WebApplication app)
    {
        app.UseSwaggerConfig();

        app.UseHttpsRedirection();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            await next();
        });

        app.MapControllers();

        app.UseMiddleware<ExceptionMiddleware>();

        app.SubscribeEventHandlers();

        return app;
    }
}