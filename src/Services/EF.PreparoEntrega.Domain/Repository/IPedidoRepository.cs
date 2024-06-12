using EF.Core.Commons.Repository;
using EF.PreparoEntrega.Domain.Models;

namespace EF.PreparoEntrega.Domain.Repository;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido> ObterPedidoPorId(Guid id);
    Task<IEnumerable<Pedido>> ObterPedidos(StatusPreparo? status);
    Task<IEnumerable<Pedido>> ObterPedidosEmAberto();
    void Criar(Pedido pedido);
    void Atualizar(Pedido pedido);
    Task<int> ObterProximoCodigo();
}