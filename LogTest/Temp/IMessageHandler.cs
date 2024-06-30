namespace LogTest.Temp;

public interface IMessageHandler
{
    Task HandleMessageAsync(string message, string correlationId);
}