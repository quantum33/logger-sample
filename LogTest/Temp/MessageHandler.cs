namespace LogTest.Temp;

public class MessageHandler(IServiceScopeFactory scopeFactory, ILogger<MessageHandler> logger)
    : IMessageHandler
{
    public Task HandleMessageAsync(string message, string correlationId)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var monService = scope.ServiceProvider.GetRequiredService<IMyScopedService>();
            monService.ProcessMessage(correlationId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message with CorrelationId: {CorrelationId}", correlationId);
        }

        return Task.CompletedTask;
    }
}