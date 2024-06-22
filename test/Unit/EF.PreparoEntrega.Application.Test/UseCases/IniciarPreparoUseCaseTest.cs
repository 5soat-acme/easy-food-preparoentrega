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

public class IniciarPreparoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly IIniciarPreparoUseCase _iniciarPreparoUseCase;

    public IniciarPreparoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _iniciarPreparoUseCase = _fixture.Create<IniciarPreparoUseCase>();
    }

    [Fact]
    public async Task DeveIniciarPreparoPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        var iniciarPreparoDto = _fixture.Build<IniciarPreparoDto>().With(x => x.PedidoId, pedido.Id).Create();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _iniciarPreparoUseCase.Handle(iniciarPreparoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoIniciarPreparoPedidoInexistente()
    {
        // Arrange
        var iniciarPreparoDto = _fixture.Create<IniciarPreparoDto>();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(iniciarPreparoDto.PedidoId)).ReturnsAsync((Pedido?)null);

        // Act
        Func<Task> act = async () => await _iniciarPreparoUseCase.Handle(iniciarPreparoDto);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Pedido inválido");
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(iniciarPreparoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        var iniciarPreparoDto = _fixture.Build<IniciarPreparoDto>().With(x => x.PedidoId, pedido.Id).Create();

        _pedidoRepository.Setup(x => x.ObterPedidoPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _iniciarPreparoUseCase.Handle(iniciarPreparoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPedidoPorId(pedido.Id), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
