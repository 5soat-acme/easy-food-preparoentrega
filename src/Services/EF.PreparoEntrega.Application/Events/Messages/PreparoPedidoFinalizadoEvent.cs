using EF.Core.Commons.Messages.Integrations;

namespace EF.PreparoEntrega.Application.Events.Messages;

public class PreparoPedidoFinalizadoEvent : IntegrationEvent
{
    public Guid PedidoCorrelacaoId { get; set; }
}