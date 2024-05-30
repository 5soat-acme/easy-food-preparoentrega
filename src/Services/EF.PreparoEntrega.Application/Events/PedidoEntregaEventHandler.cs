using EF.Core.Commons.Messages;
using EF.Infra.Commons.Messageria;
using EF.PreparoEntrega.Application.Events.Messages;
using EF.PreparoEntrega.Application.Events.Queues;
using System.Text.Json;

namespace EF.PreparoEntrega.Application.Events;

public class PedidoEntregaEventHandler : IEventHandler<PreparoPedidoIniciadoEvent>,
    IEventHandler<PreparoPedidoFinalizadoEvent>, IEventHandler<EntregaRealizadaEvent>
{
    private readonly IProducer _producer;

    public PedidoEntregaEventHandler(IProducer producer)
    {
        _producer = producer;
    }

    public async Task Handle(PreparoPedidoIniciadoEvent notification)
    {
        await _producer.SendMessageAsync(QueuesNames.PreparoPedidoIniciado.ToString(), JsonSerializer.Serialize(notification));
    }

    public async Task Handle(PreparoPedidoFinalizadoEvent notification)
    {
        await _producer.SendMessageAsync(QueuesNames.PreparoPedidoFinalizado.ToString(), JsonSerializer.Serialize(notification));
    }
    
    public async Task Handle(EntregaRealizadaEvent notification)
    {
        await _producer.SendMessageAsync(QueuesNames.EntregaPedidoRealizada.ToString(), JsonSerializer.Serialize(notification));
    }
}