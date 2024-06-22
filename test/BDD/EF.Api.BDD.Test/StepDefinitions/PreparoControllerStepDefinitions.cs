using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Api.BDD.Test.Support;
using FluentAssertions;
using System.Net;
using EF.PreparoEntrega.Application.DTOs.Requests;
using System.Text;
using System.Text.Json;

namespace EF.Api.BDD.Test.StepDefinitions;

[Binding]
[Scope(Tag = "PreparoController")]
public class PreparoControllerStepDefinitions
{
    private readonly IFixture _fixture;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private HttpResponseMessage _result;
    private Guid _pedidoId;

    public PreparoControllerStepDefinitions(CustomWebApplicationFactory<Program> factory)
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Given(@"o pedido com id ""(.*)"" existe")]
    public void DadoOPedidoComIdExiste(Guid pedidoId)
    {
        _pedidoId = pedidoId;
    }

    [When(@"eu solicitar o pedido com id ""(.*)""")]
    public async Task QuandoEuSolicitarOPedidoComId(Guid pedidoId)
    {
        _result = await _client.GetAsync($"/api/preparo/{pedidoId}");
    }    

    [Given(@"que existem pedidos com diferentes status")]
    public void DadoQueExistemPedidosComDiferentesStatus()
    {
    }

    [When(@"eu solicitar pedidos com status ""(.*)""")]
    public async Task QuandoEuSolicitarPedidosComStatus(string status)
    {
        _result = await _client.GetAsync($"/api/preparo?status={status}");
    }

    [When(@"eu enviar uma solicitação para iniciar a preparação")]
    public async Task QuandoEuEnviarUmaSolicitacaoParaIniciarAPreparacao()
    {
        var dto = new IniciarPreparoDto()
        {
            PedidoId = _pedidoId
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        _result = await _client.PostAsync($"/api/preparo/iniciar", content);
    }

    [When(@"eu enviar uma solicitação para finalizar a preparação")]
    public async Task QuandoEuEnviarUmaSolicitacaoParaFinalizarAPreparacao()
    {
        var dto = new FinalizarPreparoDto()
        {
            PedidoId = _pedidoId
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        _result = await _client.PostAsync($"/api/preparo/finalizar", content);
    }

    [When(@"eu enviar uma solicitação para confirmar a entrega")]
    public async Task QuandoEuEnviarUmaSolicitacaoParaConfirmarAEntrega()
    {
        var dto = new ConfirmarEntregaDto()
        {
            PedidoId = _pedidoId
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        _result = await _client.PostAsync($"/api/preparo/confirmar-entrega", content);
    }

    [Then(@"a resposta deve ser (.*)")]
    public void EntaoARespostaDeveSer200(int statusCode)
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