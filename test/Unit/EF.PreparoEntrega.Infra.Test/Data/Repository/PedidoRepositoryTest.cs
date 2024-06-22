using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using EF.PreparoEntrega.Infra.Data;
using EF.Infra.Commons.EventBus;
using Moq;
using EF.PreparoEntrega.Domain.Repository;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Infra.Data.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace EF.PreparoEntrega.Infra.Test.Data.Repository;

public class PedidoRepositoryTest : IDisposable
{
    private readonly PreparoEntregaDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public PedidoRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<PreparoEntregaDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new PreparoEntregaDbContext(options, Mock.Of<IEventBus>());

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Register<IPedidoRepository>(() => new PedidoRepository(_context));
    }

    [Fact]
    public async Task DeveObterPedidoPorId()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<IPedidoRepository>();
        repository.Criar(pedido);
        await _context.Commit();

        // Act
        var result = await repository.ObterPedidoPorId(pedido.Id);

        // Assert
        result.Should().BeEquivalentTo(pedido);
        result.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeveObterPedidos()
    {
        // Arrange
        var repository = _fixture.Create<IPedidoRepository>();
        var pedidos = _fixture.CreateMany<Pedido>(5).ToList();
        pedidos.ForEach(x =>
        {
            x.AdicionarItem(_fixture.Create<Item>());
            repository.Criar(x);
        });

        await _context.Commit();

        // Act
        var result = await repository.ObterPedidos(null);

        // Assert
        result.Should().BeEquivalentTo(pedidos);
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task DeveObterPedidosPorStatus()
    {
        // Arrange
        var repository = _fixture.Create<IPedidoRepository>();
        IList<Pedido> pedidos = new List<Pedido>();
        _fixture.CreateMany<Pedido>(5).ToList().ForEach(x =>
        {
            x.IniciarPreparo();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
            repository.Criar(x);
        });
        _fixture.CreateMany<Pedido>(2).ToList().ForEach(x =>
        {
            x.FinalizarPreparo();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
            repository.Criar(x);
        });

        await _context.Commit();

        // Act
        var resultEmPreparacao = await repository.ObterPedidos(StatusPreparo.EmPreparacao);
        var resultPronto = await repository.ObterPedidos(StatusPreparo.Pronto);

        // Assert
        resultEmPreparacao.Should().HaveCount(5);
        resultPronto.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeveObterPedidosEmAberto()
    {
        // Arrange
        var repository = _fixture.Create<IPedidoRepository>();
        IList<Pedido> pedidos = new List<Pedido>();
        _fixture.CreateMany<Pedido>(5).ToList().ForEach(x =>
        {
            x.IniciarPreparo();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
            repository.Criar(x);
        });
        _fixture.CreateMany<Pedido>(2).ToList().ForEach(x =>
        {
            x.FinalizarPreparo();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
            repository.Criar(x);
        });
        _fixture.CreateMany<Pedido>(3).ToList().ForEach(x =>
        {
            x.ConfirmarEntrega();
            x.AdicionarItem(_fixture.Create<Item>());
            pedidos.Add(x);
            repository.Criar(x);
        });

        await _context.Commit();

        // Act
        var result = await repository.ObterPedidosEmAberto();

        // Assert
        result.Should().HaveCount(7);
    }

    [Fact]
    public async Task DeveCriarPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<IPedidoRepository>();


        // Act
        repository.Criar(pedido);
        var commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Pedidos.Should().Contain(pedido);
        var pedidoSalvo = await _context.Pedidos!.FindAsync(pedido.Id);
        pedidoSalvo.Should().NotBeNull();
        pedidoSalvo.Should().BeEquivalentTo(pedido);
        pedidoSalvo!.Itens.Should().HaveCount(1);
        pedidoSalvo!.DataCriacao.Date.Should().Be(DateTime.UtcNow.Date);
        repository.UnitOfWork.Should().Be(_context);
    }

    [Fact]
    public async Task DeveAtualizarPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<IPedidoRepository>();
        repository.Criar(pedido);
        await _context.Commit();
        _context.Entry(pedido).State = EntityState.Detached;


        // Act
        var pedidoAtualizar = await repository.ObterPedidoPorId(pedido.Id);
        var item = _fixture.Create<Item>();
        pedidoAtualizar!.AdicionarItem(item);
        pedidoAtualizar.ConfirmarEntrega();
        _context.Entry(pedidoAtualizar).State = EntityState.Modified;
        repository.Atualizar(pedidoAtualizar!);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Entry(pedidoAtualizar).State = EntityState.Detached;
        _context.Pedidos.Should().Contain(pedidoAtualizar);
        var pedidoSalvo = await _context.Pedidos!.FindAsync(pedidoAtualizar.Id);
        pedidoSalvo!.Status.Should().Be(StatusPreparo.Finalizado);
        pedidoSalvo!.DataAtualizacao.Should().NotBeNull();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
