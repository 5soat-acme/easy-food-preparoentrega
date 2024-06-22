using EF.PreparoEntrega.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.PreparoEntrega.Infra.Data.Mapping;

public class ItemMapping : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("ItensPreparoEntrega");

        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Pedido)
            .WithMany(c => c.Itens);
    }
}