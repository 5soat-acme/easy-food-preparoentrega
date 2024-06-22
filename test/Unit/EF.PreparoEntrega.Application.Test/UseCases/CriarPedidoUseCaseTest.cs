using AutoFixture.AutoMoq;
using AutoFixture;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Application.UseCases;
using EF.PreparoEntrega.Domain.Repository;
using Moq;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Application.DTOs.Requests;
using FluentAssertions;

namespace EF.PreparoEntrega.Application.Test.UseCases;

public class CriarPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly ICriarPedidoUseCase _criarPedidoUseCase;

    public CriarPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _criarPedidoUseCase = _fixture.Create<CriarPedidoUseCase>();
    }

    [Fact]
    public async Task DeveCriarPedido()
    {
        // Arrange
        var criarPedidoDto = _fixture.Create<CriarPedidoPreparoDto>();
        
        _pedidoRepository.Setup(x => x.Criar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _criarPedidoUseCase.Handle(criarPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var criarPedidoDto = _fixture.Create<CriarPedidoPreparoDto>();

        _pedidoRepository.Setup(x => x.Criar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _criarPedidoUseCase.Handle(criarPedidoDto);


        // Assert
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
