using EF.Core.Commons.DomainObjects;

namespace EF.PreparoEntrega.Domain.Models;

public class Item : Entity
{
    public Item(int quantidade, Guid produtoId, string nomeProduto, int tempoPreparoEstimado)
    {
        Quantidade = quantidade;
        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        TempoPreparoEstimado = tempoPreparoEstimado;
    }

    public int Quantidade { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; }
    public int TempoPreparoEstimado { get; private set; }
    public Guid PedidoId { get; private set; }
    public Pedido Pedido { get; private set; }
}