using AutoFixture.AutoMoq;
using AutoFixture;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Application.UseCases;
using EF.PreparoEntrega.Domain.Repository;
using Moq;
using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.PreparoEntrega.Domain.Models;
using FluentAssertions;
using EF.Core.Commons.Repository;

namespace EF.PreparoEntrega.Application.Test.UseCases;

public class ConsultarPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly IConsultarPedidoUseCase _consultarPedidoUseCase;

    public ConsultarPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _consultarPedidoUseCase = _fixture.Create<ConsultarPedidoUseCase>();
    }

    [Fact]
    public async Task DeveObterPedidoPorId()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);

        // Act
        var resultado = await _consultarPedidoUseCase.ObterPedidoPorId(pedido.Id);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(pedido.Id);
    }

    [Fact]
    public async Task DeveObterPedidos()
    {
        // Arrange
        var pedidos = _fixture.CreateMany<Pedido>(5).ToList();
        pedidos.ForEach(x =>
        {
            x.AdicionarItem(_fixture.Create<Item>());
        });

        _pedidoRepository.Setup(x => x.ObterPedidos(null)).ReturnsAsync(pedidos);

        // Act
        var resultado = await _consultarPedidoUseCase.ObterPedidos(null);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidos(null), Times.Once);
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(5);
    }

    [Fact]
    public async Task DeveObterPedidosStatus()
    {
        // Arrange
        var repository = _fixture.Create<IPedidoRepository>();
        IList<Pedido> pedidos = new List<Pedido>();
        _fixture.CreateMany<Pedido>(5).ToList().ForEach(x =>
        {
            x.IniciarPreparo();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
        });
        _fixture.CreateMany<Pedido>(2).ToList().ForEach(x =>
        {
            x.FinalizarPreparo();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
        });

        _pedidoRepository.Setup(x => x.ObterPedidos(StatusPreparo.EmPreparacao))
            .ReturnsAsync(pedidos.Where(x => x.Status == StatusPreparo.EmPreparacao).ToList());

        // Act
        var resultado = await _consultarPedidoUseCase.ObterPedidos(StatusPreparo.EmPreparacao);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidos(StatusPreparo.EmPreparacao), Times.Once);
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(5);
    }

    [Fact]
    public async Task DeveObterPedidosMonitor()
    {
        // Arrange
        var pedidos = _fixture.CreateMany<Pedido>(5).ToList();
        pedidos.ForEach(x =>
        {
            x.AdicionarItem(_fixture.Create<Item>());
        });

        _pedidoRepository.Setup(x => x.ObterPedidosEmAberto()).ReturnsAsync(pedidos);

        // Act
        var resultado = (await _consultarPedidoUseCase.ObterPedidosMonitor())!.ToList();

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidosEmAberto(), Times.Once);
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(5);
    }
}
