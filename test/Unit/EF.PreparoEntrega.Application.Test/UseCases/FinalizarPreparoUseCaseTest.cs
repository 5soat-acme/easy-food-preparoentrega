using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Core.Commons.DomainObjects;
using EF.PreparoEntrega.Application.DTOs.Requests;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Application.UseCases;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.PreparoEntrega.Application.Test.UseCases;

public class FinalizarPreparoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly IFinalizarPreparoUseCase _finalizarPreparoUseCase;

    public FinalizarPreparoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _finalizarPreparoUseCase = _fixture.Create<FinalizarPreparoUseCase>();
    }

    [Fact]
    public async Task DeveFinalizarPreparoPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        var finalizarPreparoDto = _fixture.Build<FinalizarPreparoDto>().With(x => x.PedidoId, pedido.Id).Create();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _finalizarPreparoUseCase.Handle(finalizarPreparoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoFinalizarPreparoPedidoInexistente()
    {
        // Arrange
        var finalizarPreparoDto = _fixture.Create<FinalizarPreparoDto>();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(finalizarPreparoDto.PedidoId)).ReturnsAsync((Pedido?)null);

        // Act
        Func<Task> act = async () => await _finalizarPreparoUseCase.Handle(finalizarPreparoDto);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Pedido inválido");
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(finalizarPreparoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        var finalizarPreparoDto = _fixture.Build<FinalizarPreparoDto>().With(x => x.PedidoId, pedido.Id).Create();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _finalizarPreparoUseCase.Handle(finalizarPreparoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
