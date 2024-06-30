namespace LogTest.Temp;

public class MyScopedService(ILogger<MyScopedService> logger) : IMyScopedService
{
    public void ProcessMessage(string correlationId)
    {
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            logger.LogInformation("Processing message with CorrelationId: {CorrelationId}", correlationId);
            // Do something with the correlationId
        }
    }
}