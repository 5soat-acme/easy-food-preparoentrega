using EF.PreparoEntrega.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.PreparoEntrega.Infra.Data.Mapping;

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("PedidosPreparoEntrega");

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.PedidoCorrelacaoId)
            .HasName("IDX_PreparoEntrega_PedidoCorrelacaoId");

        builder.HasMany(c => c.Itens)
            .WithOne(c => c.Pedido)
            .HasForeignKey(c => c.PedidoId);
    }
}