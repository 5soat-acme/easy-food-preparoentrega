using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using EF.PreparoEntrega.Application.Events.Queues;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Application.DTOs.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace EF.PreparoEntrega.Application.Events.Messages;

public class PedidoRecebidoConsumer : BackgroundService
{
    private readonly IConsumer<AWSConsumerResponse, AwsConfirmReceipt> _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PedidoRecebidoConsumer(IConsumer<AWSConsumerResponse, AwsConfirmReceipt> consumer,
                        IServiceScopeFactory serviceScopeFactory)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _consumer.ReceiveMessagesAsync(QueuesNames.PedidoRecebido.ToString());

                foreach (var message in response.receiveMessageResponse.Messages)
                {
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        var criarPedidoUseCase = scope.ServiceProvider.GetRequiredService<ICriarPedidoUseCase>();
                        var pedidoRecebido = JsonSerializer.Deserialize<PedidoRecebidoEvent>(message.Body);

                        if (pedidoRecebido != null)
                        {
                            await criarPedidoUseCase.Handle(new CriarPedidoPreparoDto
                            {
                                CorrelacaoId = pedidoRecebido.AggregateId,
                                Itens = pedidoRecebido.Itens.Select(x => new CriarPedidoPreparoDto.ItemPedido
                                {
                                    ProdutoId = x.ProdutoId,
                                    Quantidade = x.Quantidade,
                                    NomeProduto = x.NomeProduto,
                                    TempoPreparoEstimado = x.TempoPreparoEstimado
                                }).ToList()
                            });

                            var confirm = new AwsConfirmReceipt
                            {
                                QueueUrl = response.queueUrl,
                                ReceiptHandle = message.ReceiptHandle
                            };

                            await _consumer.ConfirmReceiptAsync(confirm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de erros ou manipulação de exceções
            }
        }
    }
}
