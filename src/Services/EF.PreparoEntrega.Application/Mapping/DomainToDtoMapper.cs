using EF.PreparoEntrega.Application.DTOs.Responses;
using EF.PreparoEntrega.Domain.Models;

namespace EF.PreparoEntrega.Application.Mapping;

public static class DomainToDtoMapper
{
    public static PedidoMonitorDto MapMonitor(Pedido model)
    {
        return new PedidoMonitorDto
        {
            Id = model.Id,
            Codigo = model.Codigo.ToString(),
            Status = model.Status,
            TempoEspera = FormatTempoDecorrido(model.DataCriacao)
        };
    }

    public static IEnumerable<PedidoMonitorDto> MapToMonitorList(IEnumerable<Pedido> models)
    {
        return models.Select(MapMonitor);
    }

    public static PedidoPreparoDto? Map(Pedido? model)
    {
        if (model is null) return null;

        return new PedidoPreparoDto
        {
            Id = model.Id,
            Codigo = model.Codigo.ToString(),
            Status = model.Status,
            DataCriacao = model.DataCriacao,
            Itens = model.Itens.Select(Map).ToList()
        };
    }

    public static IEnumerable<PedidoPreparoDto> MapToList(IEnumerable<Pedido> models)
    {
        return models.Select(Map);
    }

    public static ItemPreparoDto Map(Item model)
    {
        return new ItemPreparoDto
        {
            Quantidade = model.Quantidade,
            ProdutoId = model.ProdutoId,
            NomeProduto = model.NomeProduto,
            TempoPreparoEstimado = model.TempoPreparoEstimado
        };
    }

    private static string FormatTempoDecorrido(DateTime dataCriacao)
    {
        var agoraUtc = DateTime.UtcNow;
        var tempoDecorrido = agoraUtc - dataCriacao;
        tempoDecorrido = tempoDecorrido.Duration();
        return $"{tempoDecorrido.Hours:D2}:{tempoDecorrido.Minutes:D2}:{tempoDecorrido.Seconds:D2}";
    }
}