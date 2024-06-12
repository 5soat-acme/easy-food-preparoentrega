using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.PreparoEntrega.Domain.Test.Models
{
    [Collection(nameof(PreparoEntregaCollection))]
    public class ItemTest(PreparoEntregaFixture fixture)
    {
        [Fact]
        public void DeveCriarItem()
        {
            // Arrange - Act
            var item = fixture.GerarItem();

            // Act - Assert
            item.Should().BeOfType<Item>();
        }
    }
}
