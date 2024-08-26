using EF.PreparoEntrega.Application.DTOs.Responses;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.WebApi.Commons.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.PreparoEntrega.Controllers;

[Authorize]
[Route("api/monitor")]
public class MonitorController : CustomControllerBase
{
    private readonly IConsultarPedidoUseCase _consultarPedidoUseCase;

    public MonitorController(IConsultarPedidoUseCase consultarPedidoUseCase)
    {
        _consultarPedidoUseCase = consultarPedidoUseCase;
    }

    /// <summary>
    ///     Obtém os dados de acompanhamento utilizados para exibir o status do pedido na tela de acompanhamento.
    /// </summary>
    /// <response code="200">Status dos pedidos.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PedidoMonitorDto>))]
    [Produces("application/json")]
    [HttpGet]
    public async Task<IActionResult> ObterPedidos()
    {
        var pedidos = await _consultarPedidoUseCase.ObterPedidosMonitor();
        return pedidos is null || !pedidos.Any() ? NotFound() : Respond(pedidos);
    }
}