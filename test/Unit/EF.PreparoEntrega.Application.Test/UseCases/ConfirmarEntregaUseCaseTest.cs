using AutoFixture.AutoMoq;
using AutoFixture;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Application.UseCases;
using EF.PreparoEntrega.Domain.Repository;
using Moq;
using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.PreparoEntrega.Domain.Models;
using FluentAssertions;
using EF.Core.Commons.DomainObjects;

namespace EF.PreparoEntrega.Application.Test.UseCases;

public class ConfirmarEntregaUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly IConfirmarEntregaUseCase _confirmarEntregaUseCase;

    public ConfirmarEntregaUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _confirmarEntregaUseCase = _fixture.Create<ConfirmarEntregaUseCase>();
    }

    [Fact]
    public async Task DeveConfirmarEntregaPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        var confirmarEntregaDto = _fixture.Build<ConfirmarEntregaDto>().With(x => x.PedidoId, pedido.Id).Create();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(It.IsAny<Pedido>()));        
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _confirmarEntregaUseCase.Handle(confirmarEntregaDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoConfirmarEntregaPedidoInexistente()
    {
        // Arrange
        var confirmarEntregaDto = _fixture.Create<ConfirmarEntregaDto>();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(confirmarEntregaDto.PedidoId)).ReturnsAsync((Pedido?)null);

        // Act
        Func<Task> act = async () => await _confirmarEntregaUseCase.Handle(confirmarEntregaDto);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Pedido inválido");
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(confirmarEntregaDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);       
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        var confirmarEntregaDto = _fixture.Build<ConfirmarEntregaDto>().With(x => x.PedidoId, pedido.Id).Create();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _confirmarEntregaUseCase.Handle(confirmarEntregaDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
