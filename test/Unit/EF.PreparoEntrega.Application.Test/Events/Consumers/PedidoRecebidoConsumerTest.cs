using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Amazon.SQS.Model;
using System.Text.Json;
using EF.Core.Commons.Communication;
using EF.PreparoEntrega.Application.UseCases.Interfaces;
using EF.PreparoEntrega.Application.Events.Messages;
using EF.PreparoEntrega.Application.Events.Queues;
using EF.PreparoEntrega.Application.DTOs.Requests;

namespace EF.PreparoEntrega.Application.Test.Events.Consumers;

public class PedidoRecebidoConsumerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>> _consumerMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ICriarPedidoUseCase> _criarPedidoUseCaseMock;
    private readonly PedidoRecebidoConsumer _pedidoRecebidoConsumer;

    public PedidoRecebidoConsumerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consumerMock = _fixture.Freeze<Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>>>();
        _serviceScopeFactoryMock = _fixture.Freeze<Mock<IServiceScopeFactory>>();
        _serviceScopeMock = _fixture.Freeze<Mock<IServiceScope>>();
        _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
        _criarPedidoUseCaseMock = _fixture.Freeze<Mock<ICriarPedidoUseCase>>();
        _pedidoRecebidoConsumer = _fixture.Create<PedidoRecebidoConsumer>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(ICriarPedidoUseCase))).Returns(_criarPedidoUseCaseMock.Object);
    }

    [Fact]
    public async Task DeveExecutarConsumerCompleto()
    {
        // Arrange
        var pedidoRecebidoEvent = _fixture.Create<PedidoRecebidoEvent>();
        var message = new Message
        {
            Body = JsonSerializer.Serialize(pedidoRecebidoEvent),
            ReceiptHandle = "receipt-handle"
        };

        var response = new AWSConsumerResponse
        {
            receiveMessageResponse = new ReceiveMessageResponse
            {
                Messages = new List<Message> { message }
            },
            queueUrl = "queue-url"
        };

        _consumerMock.Setup(x => x.ReceiveMessagesAsync(QueuesNames.PedidoRecebido.ToString())).ReturnsAsync(response);
        _criarPedidoUseCaseMock.Setup(x => x.Handle(It.IsAny<CriarPedidoPreparoDto>())).ReturnsAsync(It.IsAny<OperationResult>);

        // Act
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            cancellationTokenSource.CancelAfter(300);
            await _pedidoRecebidoConsumer.StartAsync(cancellationTokenSource.Token);
        }

        // Assert
        _consumerMock.Verify(c => c.ReceiveMessagesAsync(It.IsAny<string>()));
        _consumerMock.Verify(c => c.ConfirmReceiptAsync(It.Is<AwsConfirmReceipt>(confirm =>
            confirm.QueueUrl == response.queueUrl &&
            confirm.ReceiptHandle == message.ReceiptHandle
        )));
        _criarPedidoUseCaseMock.Verify(u => u.Handle(It.Is<CriarPedidoPreparoDto>(dto =>
            dto.CorrelacaoId == pedidoRecebidoEvent.AggregateId
        )));
    }
}
