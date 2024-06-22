using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Api.Contexts.PreparoEntrega.Controllers;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using EF.PreparoEntrega.Application.DTOs.Responses;
using EF.PreparoEntrega.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace EF.Api.Test.Contexts.PreparoEntrega.Controllers;

public class MonitorControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsultarPedidoUseCase> _consultarPedidoUseCaseMock;
    private readonly MonitorController _monitorController;

    public MonitorControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _consultarPedidoUseCaseMock = _fixture.Freeze<Mock<IConsultarPedidoUseCase>>();
        _monitorController = _fixture.Create<MonitorController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoObterPedidos()
    {
        // Arrange
        var status = _fixture.Create<StatusPreparo>();
        var pedidosMonitorDto = _fixture.CreateMany<PedidoMonitorDto>(5).ToList();

        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidosMonitor()).ReturnsAsync(pedidosMonitorDto);

        // Act
        var resultado = await _monitorController.ObterPedidos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(pedidosMonitorDto);
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoObterPedidos()
    {
        // Arrange
        var status = _fixture.Create<StatusPreparo>();
        var pedidosPreparoDto = _fixture.CreateMany<PedidoPreparoDto>(5).ToList();

        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidosMonitor()).ReturnsAsync((IList<PedidoMonitorDto>?)null);

        // Act
        var resultado = await _monitorController.ObterPedidos();

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }
}
