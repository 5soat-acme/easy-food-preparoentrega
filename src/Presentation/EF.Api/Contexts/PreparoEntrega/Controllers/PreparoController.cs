using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.PreparoEntrega.Application.DTOs.Responses;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Domain.Models;
using EF.WebApi.Commons.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.PreparoEntrega.Controllers;

[Authorize]
[Route("api/preparo")]
public class PreparoController : CustomControllerBase
{
    private readonly IConfirmarEntregaUseCase _confirmarEntregaUseCase;
    private readonly IConsultarPedidoUseCase _consultarPedidoUseCase;
    private readonly IFinalizarPreparoUseCase _finalizarPreparoUseCase;
    private readonly IIniciarPreparoUseCase _iniciarPreparoUseCase;

    public PreparoController(IConsultarPedidoUseCase consultarPedidoUseCase,
        IConfirmarEntregaUseCase confirmarEntregaUseCase, IFinalizarPreparoUseCase finalizarPreparoUseCase,
        IIniciarPreparoUseCase iniciarPreparoUseCase)
    {
        _consultarPedidoUseCase = consultarPedidoUseCase;
        _confirmarEntregaUseCase = confirmarEntregaUseCase;
        _finalizarPreparoUseCase = finalizarPreparoUseCase;
        _iniciarPreparoUseCase = iniciarPreparoUseCase;
    }

    /// <summary>
    ///     Obtém um pedido
    /// </summary>
    /// ///
    /// <param name="id">Id do pedido</param>
    /// <response code="200">Dados do pedido.</response>
    /// t
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PedidoMonitorDto))]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPedido([FromRoute] Guid id)
    {
        var pedido = await _consultarPedidoUseCase.ObterPedidoPorId(id);
        return pedido is null ? NotFound() : Respond(pedido);
    }

    /// <summary>
    ///     Obtém os dados dos pedidos
    /// </summary>
    /// <response code="200">Dados dos Pedidos.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PedidoMonitorDto>))]
    [Produces("application/json")]
    [HttpGet]
    public async Task<IActionResult> ObterPedidos([FromQuery] StatusPreparo? status)
    {
        var pedidos = await _consultarPedidoUseCase.ObterPedidos(status);
        return pedidos is null || !pedidos.Any() ? NotFound() : Respond(pedidos);
    }

    /// <summary>
    ///     Sinaliza o início do preparo de um pedido (Status = Em Preparacao).
    /// </summary>
    /// <response code="200">Status do pedido alterado com sucesso.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpPost("iniciar")]
    public async Task<IActionResult> IniciarPreparo(IniciarPreparoDto dto)
    {
        var result = await _iniciarPreparoUseCase.Handle(dto);
        if (!result.IsValid) return Respond(result.GetErrorMessages());

        return Respond();
    }

    /// <summary>
    ///     Sinaliza que o pedido está pronto para ser entregue (Status = Pronto)
    /// </summary>
    /// <response code="200">Status do pedido alterado com sucesso.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpPost("finalizar")]
    public async Task<IActionResult> FinalizarPreparo(FinalizarPreparoDto dto)
    {
        var result = await _finalizarPreparoUseCase.Handle(dto);
        if (!result.IsValid) return Respond(result.GetErrorMessages());

        return Respond();
    }

    /// <summary>
    ///     Sinaliza que o pedido foi entregue (Status = Finalizado)
    /// </summary>
    /// <response code="200">Status do pedido alterado com sucesso.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpPost("confirmar-entrega")]
    public async Task<IActionResult> ConfirmarEntrega(ConfirmarEntregaDto dto)
    {
        var result = await _confirmarEntregaUseCase.Handle(dto);
        if (!result.IsValid) return Respond(result.GetErrorMessages());

        return Respond();
    }
}