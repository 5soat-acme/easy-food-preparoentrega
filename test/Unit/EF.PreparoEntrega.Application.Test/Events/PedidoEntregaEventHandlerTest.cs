using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Infra.Commons.Messageria;
using Moq;
using EF.PreparoEntrega.Application.Events;
using EF.PreparoEntrega.Application.Events.Messages;
using EF.PreparoEntrega.Application.Events.Queues;
using System.Text.Json;

namespace EF.PreparoEntrega.Application.Test.Events;

public class PedidoEntregaEventHandlerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IProducer> _producerMock;
    private readonly PedidoEntregaEventHandler _pedidoEntregaEventHandler;

    public PedidoEntregaEventHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _producerMock = _fixture.Freeze<Mock<IProducer>>();
        _pedidoEntregaEventHandler = _fixture.Create<PedidoEntregaEventHandler>();
    }

    [Fact]
    public async Task DeveExecutarEventoPreparoPedidoIniciado()
    {
        // Arrange
        var preparoPedidoIniciadoEvent = _fixture.Create<PreparoPedidoIniciadoEvent>();
        var preparoPedidoIniciadoEventJson = JsonSerializer.Serialize(preparoPedidoIniciadoEvent);

        _producerMock.Setup(x => x.SendMessageAsync(QueuesNames.PreparoPedidoIniciado.ToString(), preparoPedidoIniciadoEventJson));

        // Act
        await _pedidoEntregaEventHandler.Handle(preparoPedidoIniciadoEvent);

        // Assert
        _producerMock.Verify(x => x.SendMessageAsync(QueuesNames.PreparoPedidoIniciado.ToString(), preparoPedidoIniciadoEventJson), Times.Once);
    }

    [Fact]
    public async Task DeveExecutarEventoPreparoPedidoFinalizado()
    {
        // Arrange
        var preparoPedidoFinalizadoEvent = _fixture.Create<PreparoPedidoFinalizadoEvent>();
        var preparoPedidoFinalizadoEventJson = JsonSerializer.Serialize(preparoPedidoFinalizadoEvent);

        _producerMock.Setup(x => x.SendMessageAsync(QueuesNames.PreparoPedidoFinalizado.ToString(), preparoPedidoFinalizadoEventJson));

        // Act
        await _pedidoEntregaEventHandler.Handle(preparoPedidoFinalizadoEvent);

        // Assert
        _producerMock.Verify(x => x.SendMessageAsync(QueuesNames.PreparoPedidoFinalizado.ToString(), preparoPedidoFinalizadoEventJson), Times.Once);
    }

    [Fact]
    public async Task DeveExecutarEventoEntregaRealizadaPedido()
    {
        // Arrange
        var entregaRealizadaEvent = _fixture.Create<EntregaRealizadaEvent>();
        var entregaRealizadaEventJson = JsonSerializer.Serialize(entregaRealizadaEvent);

        _producerMock.Setup(x => x.SendMessageAsync(QueuesNames.EntregaPedidoRealizada.ToString(), entregaRealizadaEventJson));

        // Act
        await _pedidoEntregaEventHandler.Handle(entregaRealizadaEvent);

        // Assert
        _producerMock.Verify(x => x.SendMessageAsync(QueuesNames.EntregaPedidoRealizada.ToString(), entregaRealizadaEventJson), Times.Once);
    }
}