using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using EF.Api.Contexts.PreparoEntrega.Controllers;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EF.PreparoEntrega.Application.DTOs.Responses;
using FluentAssertions;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.Core.Commons.Communication;

namespace EF.Api.Test.Contexts.PreparoEntrega.Controllers;

public class PreparoControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConfirmarEntregaUseCase> _confirmarEntregaUseCaseMock;
    private readonly Mock<IConsultarPedidoUseCase> _consultarPedidoUseCaseMock;
    private readonly Mock<IFinalizarPreparoUseCase> _finalizarPreparoUseCaseMock;
    private readonly Mock<IIniciarPreparoUseCase> _iniciarPreparoUseCaseMock;
    private readonly PreparoController _preparoController;

    public PreparoControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _confirmarEntregaUseCaseMock = _fixture.Freeze<Mock<IConfirmarEntregaUseCase>>();
        _consultarPedidoUseCaseMock = _fixture.Freeze<Mock<IConsultarPedidoUseCase>>();
        _finalizarPreparoUseCaseMock = _fixture.Freeze<Mock<IFinalizarPreparoUseCase>>();
        _iniciarPreparoUseCaseMock = _fixture.Freeze<Mock<IIniciarPreparoUseCase>>();
        _preparoController = _fixture.Create<PreparoController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoObterPedido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoPreparoDto = _fixture.Create<PedidoPreparoDto>();

        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidoPorId(pedidoId)).ReturnsAsync(pedidoPreparoDto);

        // Act
        var resultado = await _preparoController.ObterPedido(pedidoId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(pedidoPreparoDto);
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoNaoHouverPedido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidoPorId(pedidoId))
            .ReturnsAsync((PedidoPreparoDto?)null);

        // Act
        var resultado = await _preparoController.ObterPedido(pedidoId);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoObterPedidos()
    {
        // Arrange
        var status = _fixture.Create<StatusPreparo>();
        var pedidosPreparoDto = _fixture.CreateMany<PedidoPreparoDto>(5).ToList();

        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidos(status)).ReturnsAsync(pedidosPreparoDto);

        // Act
        var resultado = await _preparoController.ObterPedidos(status);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(pedidosPreparoDto);
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoObterPedidos()
    {
        // Arrange
        var status = _fixture.Create<StatusPreparo>();
        var pedidosPreparoDto = _fixture.CreateMany<PedidoPreparoDto>(5).ToList();

        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidos(status)).ReturnsAsync((IList<PedidoPreparoDto>?)null);

        // Act
        var resultado = await _preparoController.ObterPedidos(status);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoIniciarPreparo()
    {
        // Arrange
        var operationResult = OperationResult.Success();
        var iniciarPreparoDto = _fixture.Create<IniciarPreparoDto>();

        _iniciarPreparoUseCaseMock.Setup(x => x.Handle(iniciarPreparoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _preparoController.IniciarPreparo(iniciarPreparoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharAoIniciarPreparo()
    {
        // Arrange
        var iniciarPreparoDto = _fixture.Create<IniciarPreparoDto>();
        var operationResult = OperationResult.Failure("Erro");

        _iniciarPreparoUseCaseMock.Setup(x => x.Handle(iniciarPreparoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _preparoController.IniciarPreparo(iniciarPreparoDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", operationResult.GetErrorMessages().ToArray() }
        }));
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoFinalizarPreparo()
    {
        // Arrange
        var operationResult = OperationResult.Success();
        var finalizarPreparoDto = _fixture.Create<FinalizarPreparoDto>();

        _finalizarPreparoUseCaseMock.Setup(x => x.Handle(finalizarPreparoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _preparoController.FinalizarPreparo(finalizarPreparoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharAoFinalizarPreparo()
    {
        // Arrange
        var finalizarPreparoDto = _fixture.Create<FinalizarPreparoDto>();
        var operationResult = OperationResult.Failure("Erro");

        _finalizarPreparoUseCaseMock.Setup(x => x.Handle(finalizarPreparoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _preparoController.FinalizarPreparo(finalizarPreparoDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", operationResult.GetErrorMessages().ToArray() }
        }));
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoConfirmarEntrega()
    {
        // Arrange
        var operationResult = OperationResult.Success();
        var confirmarEntregaDto = _fixture.Create<ConfirmarEntregaDto>();

        _confirmarEntregaUseCaseMock.Setup(x => x.Handle(confirmarEntregaDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _preparoController.ConfirmarEntrega(confirmarEntregaDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoConfirmarEntrega()
    {
        // Arrange
        var confirmarEntregaDto = _fixture.Create<ConfirmarEntregaDto>();
        var operationResult = OperationResult.Failure("Erro");

        _confirmarEntregaUseCaseMock.Setup(x => x.Handle(confirmarEntregaDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _preparoController.ConfirmarEntrega(confirmarEntregaDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", operationResult.GetErrorMessages().ToArray() }
        }));
    }
}
