using Bogus;
using EF.PreparoEntrega.Domain.Models;

namespace EF.PreparoEntrega.Domain.Test.Fixtures
{
    [CollectionDefinition(nameof(PreparoEntregaCollection))]
    public class PreparoEntregaCollection : ICollectionFixture<PreparoEntregaFixture>
    {
    }

    public class PreparoEntregaFixture
    {
        public Pedido GerarPedido(Guid? pedidoCorrelacaoId = null)
        {
            return new Pedido(pedidoCorrelacaoId ?? Guid.NewGuid());
        }

        public Item GerarItem(int? quantidade = null, Guid? produtoId = null, string? nomeProduto = null, int? tempoPreparoEstimado = null)
        {
            var item = new Faker<Item>("pt_BR")
                .CustomInstantiator(f => new Item(quantidade ?? f.Random.Int(1, 20), produtoId ?? f.Random.Guid(), nomeProduto ?? f.Commerce.ProductName(), tempoPreparoEstimado ?? f.Random.Number(0, 20)));

            return item.Generate();
        }
    }
}
