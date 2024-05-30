using EF.PreparoEntrega.Application.UseCases;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Domain.Repository;
using EF.PreparoEntrega.Infra.Data;
using EF.PreparoEntrega.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.PreparoEntrega.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServicesPreparoEntrega(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application - UseCases
        services.AddScoped<IConfirmarEntregaUseCase, ConfirmarEntregaUseCase>();
        services.AddScoped<IConsultarPedidoUseCase, ConsultarPedidoUseCase>();
        services.AddScoped<ICriarPedidoUseCase, CriarPedidoUseCase>();
        services.AddScoped<IFinalizarPreparoUseCase, FinalizarPreparoUseCase>();
        services.AddScoped<IIniciarPreparoUseCase, IniciarPreparoUseCase>();

        // Infra - Data
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddDbContext<PreparoEntregaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}