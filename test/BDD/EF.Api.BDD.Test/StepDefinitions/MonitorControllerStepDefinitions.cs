using EF.Api.BDD.Test.Support;
using System.Net;
using FluentAssertions;
using EF.PreparoEntrega.Infra.Data;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Api.BDD.Test.StepDefinitions;

[Binding]
[Scope(Tag = "MonitorController")]
public class MonitorControllerStepDefinitions
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private HttpResponseMessage _result;

    public MonitorControllerStepDefinitions(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Given(@"que eu tenho pedidos para monitoramento")]
    public void DadoQueEuTenhoPedidosParaMonitoramento()
    {
    }

    [Given(@"que não existem pedidos para monitoramento")]
    public async Task DadoQueNaoExistemPedidosParaMonitoramento()
    {
        using (var scope = _factory.ServiceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var dbContextPreparoEntrega = scopedServices.GetRequiredService<PreparoEntregaDbContext>();
            dbContextPreparoEntrega.Pedidos!.RemoveRange(dbContextPreparoEntrega.Pedidos.ToList());
            await dbContextPreparoEntrega.SaveChangesAsync();
        }
    }        

    [When(@"eu solicitar os pedidos para monitoramento")]
    public async Task QuandoEuSolicitarOsPedidosParaMonitoramento()
    {
        _result = await _client.GetAsync($"/api/monitor");
    }

    [Then(@"a resposta deve ser (.*)")]
    public void EntaoARespostaDeveSer(int statusCode)
    {
        _result.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
