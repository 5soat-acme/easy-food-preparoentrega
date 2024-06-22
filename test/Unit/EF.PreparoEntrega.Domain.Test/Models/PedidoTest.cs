using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.PreparoEntrega.Domain.Test.Models
{
    [Collection(nameof(PreparoEntregaCollection))]
    public class PedidoTest(PreparoEntregaFixture fixture)
    {
        [Fact]
        public void DeveCriarPedido()
        {
            // Arrange
            var pedido = fixture.GerarPedido();

            // Act & Assert 
            pedido.Should().BeOfType<Pedido>();
            pedido.Status.Should().Be(StatusPreparo.Recebido);
        }

        [Fact]
        public void DeveIniciarPreparoDoPedido()
        {
            // Arrange
            var pedido = fixture.GerarPedido();

            // Act
            pedido.IniciarPreparo();

            // Assert 
            pedido.Status.Should().Be(StatusPreparo.EmPreparacao);
        }

        [Fact]
        public void DeveFinalizarPreparoDoPedido()
        {
            // Arrange
            var pedido = fixture.GerarPedido();

            // Act
            pedido.FinalizarPreparo();

            // Assert 
            pedido.Status.Should().Be(StatusPreparo.Pronto);
        }

        [Fact]
        public void DeveConfirmarEntregaDoPedido()
        {
            // Arrange
            var pedido = fixture.GerarPedido();

            // Act
            pedido.ConfirmarEntrega();

            // Assert 
            pedido.Status.Should().Be(StatusPreparo.Finalizado);
        }

        [Fact]
        public void DeveGerarCodigoDoPedido()
        {
            // Arrange
            var pedido = fixture.GerarPedido();

            // Act
            var codigo = 10;
            pedido.GerarCodigo(codigo);

            // Assert 
            pedido.Codigo.Should().Be(codigo);
        }

        [Fact]
        public void DeveAdicionarItemAoPedido()
        {
            // Arrange
            var pedido = fixture.GerarPedido();
            var item = fixture.GerarItem();

            // Act
            pedido.AdicionarItem(item);

            // Assert 
            pedido.Itens.Should().Contain(item);
        }
    }
}
