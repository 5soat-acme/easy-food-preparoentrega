using EF.Core.Commons.Messages;
using EF.Core.Commons.Repository;
using EF.Infra.Commons.Data;
using EF.Infra.Commons.EventBus;
using EF.PreparoEntrega.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EF.PreparoEntrega.Infra.Data;

public sealed class PreparoEntregaDbContext : DbContext, IUnitOfWork
{
    private readonly IEventBus _bus;

    public PreparoEntregaDbContext(DbContextOptions<PreparoEntregaDbContext> options, IEventBus bus) :
        base(options)
    {
        _bus = bus;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Pedido>? Pedidos { get; set; }
    public DbSet<Item>? Itens { get; set; }

    public async Task<bool> Commit()
    {
        DbContextExtension.SetDates(ChangeTracker.Entries());
        await _bus.PublishEvents(this);
        return await SaveChangesAsync() > 0;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PreparoEntregaDbContext).Assembly);
        modelBuilder.Ignore<Event>();

        modelBuilder.HasSequence<int>("CodigoPedidoSequence").StartsAt(1000).IncrementsBy(1);

        base.OnModelCreating(modelBuilder);
    }
}