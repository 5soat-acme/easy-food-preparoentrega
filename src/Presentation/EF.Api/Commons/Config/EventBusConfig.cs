using EF.Core.Commons.Messages;
using EF.Core.Commons.Messages.Integrations;
using EF.Infra.Commons.EventBus;
using EF.PreparoEntrega.Application.Events;
using EF.PreparoEntrega.Application.Events.Messages;

namespace EF.Api.Commons.Config;

public static class EventBusConfig
{
    public static IServiceCollection AddEventBusConfig(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        // Preparo entrega
        services.AddScoped<IEventHandler<PreparoPedidoIniciadoEvent>, PedidoEntregaEventHandler>();
        services.AddScoped<IEventHandler<PreparoPedidoFinalizadoEvent>, PedidoEntregaEventHandler>();
        services.AddScoped<IEventHandler<EntregaRealizadaEvent>, PedidoEntregaEventHandler>();

        return services;
    }

    public static WebApplication SubscribeEventHandlers(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var bus = services.GetRequiredService<IEventBus>();

        services.GetRequiredService<IEnumerable<IEventHandler<PreparoPedidoIniciadoEvent>>>().ToList()
            .ForEach(e => bus.Subscribe(e));

        services.GetRequiredService<IEnumerable<IEventHandler<PreparoPedidoFinalizadoEvent>>>().ToList()
            .ForEach(e => bus.Subscribe(e));

        services.GetRequiredService<IEnumerable<IEventHandler<EntregaRealizadaEvent>>>().ToList()
            .ForEach(e => bus.Subscribe(e));

        return app;
    }
}