using EF.Core.Commons.DomainObjects;

namespace EF.PreparoEntrega.Domain.Models;

public class Pedido : Entity, IAggregateRoot
{
    private readonly List<Item> _itens;

    public Pedido(Guid pedidoCorrelacaoId)
    {
        PedidoCorrelacaoId = pedidoCorrelacaoId;
        Status = StatusPreparo.Recebido;
        DataCriacao = DateTime.UtcNow;
        _itens = new List<Item>();
    }

    public Guid PedidoCorrelacaoId { get; private set; }
    public int Codigo { get; private set; }
    public StatusPreparo Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public IReadOnlyCollection<Item> Itens => _itens;

    public void IniciarPreparo()
    {
        Status = StatusPreparo.EmPreparacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void FinalizarPreparo()
    {
        Status = StatusPreparo.Pronto;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void ConfirmarEntrega()
    {
        Status = StatusPreparo.Finalizado;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AdicionarItem(Item item)
    {
        _itens.Add(item);
    }

    public void GerarCodigo(int codigo)
    {
        Codigo = codigo;
    }
}