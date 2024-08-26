using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Infra.Commons.Messageria;
using EF.Infra.Commons.Messageria.AWS;
using EF.PreparoEntrega.Application.Events.Messages;
using EF.PreparoEntrega.Domain.Models;
using EF.PreparoEntrega.Infra.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EF.Api.BDD.Test.Support;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public required IServiceProvider ServiceProvider;
    private readonly IFixture _fixture;
    private readonly string _dbNamePreparoEntrega = Guid.NewGuid().ToString();

    public CustomWebApplicationFactory()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(async services => 
        {
            RemoveDbContext(services);
            ConfigureAuth(services);
            RemoveHostedServices(services);
            RemoveProducers(services);
            AddDbContextInMemory(services);
            await CreateDatabases(services);
        });        

        return base.CreateHost(builder);
    }

    private void RemoveDbContext(IServiceCollection services)
    {
        var descriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<PreparoEntregaDbContext>)).ToList();

        foreach(var d in descriptors)
        {
            services.Remove(d);
        }
    }

    private void ConfigureAuth(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Test";
            options.DefaultChallengeScheme = "Test";
        })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
    }

    private void RemoveHostedServices(IServiceCollection services)
    {
        var hostedServiceTypes = new[]
        {
            typeof(PedidoRecebidoConsumer)
        };

        var descriptors = services.Where(d => hostedServiceTypes.Contains(d.ImplementationType)).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    private void RemoveProducers(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(IProducer) && d.ImplementationType == typeof(AwsProducer)).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        services.AddScoped<IProducer, FakeProducer>();
    }

    private void AddDbContextInMemory(IServiceCollection services)
    {
        services.AddDbContext<PreparoEntregaDbContext>(options =>
        {
            options.UseInMemoryDatabase(_dbNamePreparoEntrega);
        });
    }

    private async Task CreateDatabases(IServiceCollection services)
    {
        ServiceProvider = services.BuildServiceProvider();
        using (var scope = ServiceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var dbContextPreparoEntrega = scopedServices.GetRequiredService<PreparoEntregaDbContext>();
            dbContextPreparoEntrega.Database.EnsureDeleted();
            dbContextPreparoEntrega.Database.EnsureCreated();

            await SeedPreparoEntrega(dbContextPreparoEntrega);
        }
    }

    private async Task SeedPreparoEntrega(PreparoEntregaDbContext dbContext)
    {
        var pedido = _fixture.Create<Pedido>();
        pedido.Id = Guid.Parse("f694f3a3-2622-45ea-b168-f573f16165ea");

        await dbContext.Pedidos!.AddAsync(pedido);
        await dbContext.SaveChangesAsync();
    }
}