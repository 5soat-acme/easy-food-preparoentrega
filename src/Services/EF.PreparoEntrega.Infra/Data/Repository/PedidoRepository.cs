using EF.Core.Commons.Repository;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.PreparoEntrega.Infra.Data.Repository;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly PreparoEntregaDbContext _context;

    public PedidoRepository(PreparoEntregaDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Pedido> ObterPedidoPorId(Guid id)
    {
        return await _context.Pedidos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Pedido>> ObterPedidos(StatusPreparo? status)
    {
        return await _context.Pedidos
            .Include(c => c.Itens)
            .Where(c => status == null || c.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pedido>> ObterPedidosEmAberto()
    {
        return await _context.Pedidos
            .Include(c => c.Itens)
            .Where(c => c.Status != StatusPreparo.Finalizado)
            .ToListAsync();
    }

    public void Criar(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
    }

    public void Atualizar(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
    }

    public async Task<int> ObterProximoCodigo()
    {
        try
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT nextval('\"CodigoPedidoSequence\"')";
            _context.Database.OpenConnection();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        finally
        {
            _context.Database.CloseConnection();
        }
    }
}