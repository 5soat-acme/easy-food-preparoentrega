using EF.Core.Commons.Communication;
using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.UseCases;
using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.PreparoEntrega.Application.Events.Messages;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Domain.Repository;

namespace EF.PreparoEntrega.Application.UseCases;

public class FinalizarPreparoUseCase : CommonUseCase, IFinalizarPreparoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public FinalizarPreparoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<OperationResult> Handle(FinalizarPreparoDto finalizarPreparoDto)
    {
        var pedido = await _pedidoRepository.ObterPedidoPorId(finalizarPreparoDto.PedidoId);
        if (pedido is null) throw new DomainException("Pedido inválido");

        pedido.FinalizarPreparo();

        pedido.AddEvent(new PreparoPedidoFinalizadoEvent
        {
            AggregateId = pedido.Id,
            PedidoCorrelacaoId = pedido.PedidoCorrelacaoId
        });

        _pedidoRepository.Atualizar(pedido);

        await PersistData(_pedidoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }
}