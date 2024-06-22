using EF.Core.Commons.Communication;
using EF.PreparoEntrega.Application.DTOs.Requests;

namespace EF.PreparoEntrega.Application.UseCases.Interfaces;

public interface IConfirmarEntregaUseCase
{
    Task<OperationResult> Handle(ConfirmarEntregaDto confirmarEntregaDto);
}