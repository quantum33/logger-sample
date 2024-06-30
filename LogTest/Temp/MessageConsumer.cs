using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogTest.Temp;

public class MessageConsumer : BackgroundService
{
    private readonly IMessageHandler _messageHandler;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageConsumer(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;

        ConnectionFactory factory = new() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueDeclare(queue: "myQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var correlationId = ea.BasicProperties.CorrelationId;

            await _messageHandler.HandleMessageAsync(message, correlationId);
        };

        _channel.BasicConsume(queue: "myQueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
