using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Domain.Repository;

namespace EF.PreparoEntrega.Application.UseCases;

public class CriarPedidoUseCase : CommonUseCase, ICriarPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public CriarPedidoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<OperationResult> Handle(CriarPedidoPreparoDto criarPedidoPreparoDto)
    {
        var pedido = MapearPedido(criarPedidoPreparoDto);
        var proximoCodigo = await _pedidoRepository.ObterProximoCodigo();
        pedido.GerarCodigo(proximoCodigo);
        _pedidoRepository.Criar(pedido);
        await PersistData(_pedidoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }

    private Pedido MapearPedido(CriarPedidoPreparoDto criarPedidoPreparoDto)
    {
        var pedido = new Pedido(criarPedidoPreparoDto.CorrelacaoId);

        foreach (var item in criarPedidoPreparoDto.Itens)
            pedido.AdicionarItem(new Item(item.Quantidade, item.ProdutoId, item.NomeProduto,
                item.TempoPreparoEstimado));

        return pedido;
    }
}